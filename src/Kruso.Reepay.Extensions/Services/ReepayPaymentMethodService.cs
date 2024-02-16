using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Kruso.Reepay.Extensions.Enums;
using Kruso.Reepay.Extensions.Models;
using Kruso.Reepay.Extensions.Services.Interfaces;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Infrastructure.Environment;
using Ucommerce.Transactions.Payments;
using Ucommerce.Transactions.Payments.Common;

namespace Kruso.Reepay.Extensions.Services
{
    /// <summary>
    /// Implementation of the https://reepay.com/ payment provider.
    /// </summary>
    public class ReepayPaymentMethodService : ExternalPaymentMethodService
    {
        private readonly IWebRuntimeInspector _webRuntimeInspector;
        private readonly IReepayLogger<ReepayPaymentMethodService> _logger;
        private readonly IReepayRepository _reepayRepository;
        private readonly IReepaySubscriptionRepository _reepaySubscriptionRepository;

        public ReepayPaymentMethodService(
            IWebRuntimeInspector webRuntimeInspector,
            IReepayLogger<ReepayPaymentMethodService> logger,
            IReepayRepository reepayRepository,
            IReepaySubscriptionRepository reepaySubscriptionRepository)
        {
            _webRuntimeInspector = webRuntimeInspector;
            _logger = logger;
            _reepayRepository = reepayRepository;
            _reepaySubscriptionRepository = reepaySubscriptionRepository;
        }

        public override string RenderPage(PaymentRequest paymentRequest)
        {
            throw new NotSupportedException("Reepay does not need a local form. Use RequestPayment instead.");
        }

        public override Payment RequestPayment(PaymentRequest paymentRequest)
        {
            try
			{
                if (paymentRequest.Payment == null)
                    paymentRequest.Payment = CreatePayment(paymentRequest);

                var url = _reepayRepository.GetSessionUrl(paymentRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                HttpContext.Current.Response.Redirect(url, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                return paymentRequest.Payment;
            }
            catch (Exception ex)
			{
                try
				{
                    _logger.LogException(ex, "Unable to create payment session - will remove card on file property");
                    RemoveCardOnFileProperty(paymentRequest.PurchaseOrder);
                    var url1 = _reepayRepository.GetSessionUrl(paymentRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                    HttpContext.Current.Response.Redirect(url1, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return paymentRequest.Payment;
                }
                catch(Exception ex1)
				{
                    _logger.LogException(ex1, "Unable to create payment session without card on file");
                    throw ex1;
                }
            }
        }

        /// <summary>
        /// Processes the callback and excecutes a pipeline if there is one specified for this paymentmethodservice.
        /// </summary>
        /// <param name="payment">The payment to process.</param>
        public override void ProcessCallback(Payment payment)
        {
            _logger.LogInfo($"ProcessCallback is hit with payment status: {payment?.PaymentStatus?.PaymentStatusId}");

            try
            {
                var responseFromReepay = new StringBuilder();
                string[] keys = HttpContext.Current.Request.QueryString.AllKeys;
                foreach (string r in keys)
                {
                    if (r == "payment_method")
					{
                        responseFromReepay.AppendLine(string.Format("{0}:{1}", r, "hidden_for_security_reasons"));
                        continue;
                    }

                    responseFromReepay.AppendLine(string.Format("{0}:{1}", r, HttpContext.Current.Request.QueryString[r]));
                }
                _logger.LogInfo(responseFromReepay.ToString());

                if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["error"]))
                {
                    _logger.LogWarning(HttpContext.Current.Request.QueryString["error"]);
                    CompleteRequestError(payment, PaymentStatusCode.Declined);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["cancel"]))
                {
                    _logger.LogInfo($"Cancel url is hit, cancel={HttpContext.Current.Request.QueryString["cancel"]}");
                    CompleteRequestCancel(payment);
                    return;
                }

                if (payment.PaymentStatus.PaymentStatusId != (int)PaymentStatusCode.Declined)
				{
                    Guard.Against.PaymentNotPendingAuthorization(payment);
                }

                Guard.Against.MissingHttpContext(_webRuntimeInspector);
                Guard.Against.MissingRequestParameter("id");

                var source = string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["payment_method"])
                    ? _reepayRepository.CardOnFilePropertyValue(payment.PurchaseOrder)
                    : HttpContext.Current.Request.QueryString["payment_method"];

                RemoveCardOnFileProperty(payment.PurchaseOrder);

                if (_reepayRepository.IsCardChange(payment.PurchaseOrder))
				{
                    CardChangeCallbackResult(payment, source);
                    return;
                }

                if (_reepayRepository.IsSubscriptionPayment(payment.PurchaseOrder))
				{
                    SubscriptionPaymentCallbackResult(payment, source);
                    return;
                }

                var charge = CreateChargeOnCheckout(payment, source);
                EnsurePaymentIsValid(payment, out bool? isPaymentValid, false, charge);
                if (isPaymentValid == true)
				{
                    _logger.LogInfo($"ProcessCallback is validated successfully");
                    HandleCreateSubscriptionOnCheckout(payment, source);
                    CompleteRequestSuccess(payment, PaymentStatusCode.Authorized);
                    return;
                }

                _logger.LogWarning("Payment for order '" + payment.PurchaseOrder.OrderId + "' not validated");
                payment.PaymentStatus = PaymentStatus.Get((int)PaymentStatusCode.Declined);
                payment.Save();
            }
            catch (ReepayException ex)
            {
                _logger.LogReepayException(ex);
                var errorMessage = $"{ex.ReepayError.Error} {ex.ReepayError.Transaction_error}";
                CompleteRequestError(payment, null, errorMessage);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                CompleteRequestError(payment, null, ex.Message);
                return;
            }

            CompleteRequestError(payment);
        }

        /// <summary>
		/// Cancels the payment from the payment provider. This is often used when you need to call external services to handle the cancel process.
		/// </summary>
		/// <param name="payment">The payment.</param>
		/// <param name="status">The status.</param>
		/// <returns></returns>
        protected override bool CancelPaymentInternal(Payment payment, out string status)
        {
			try
			{
                var cancelChargeResult = _reepayRepository.CancelCharge(payment).ConfigureAwait(false).GetAwaiter().GetResult();
                if (cancelChargeResult?.State == ChargeStateEnum.cancelled.ToString())
                {
                    status = $"Status: {cancelChargeResult.State}, {cancelChargeResult.Amount / 100m:N2}";
                    return true;
                }

                if (cancelChargeResult?.State == ChargeStateEnum.failed.ToString())
                {
                    status = $"Status: {cancelChargeResult.State}, {cancelChargeResult.Error_state} {cancelChargeResult.Error}";
                    _logger.LogWarning($"Cancel failed for order {payment?.PurchaseOrder?.OrderId}, error: {cancelChargeResult.Error_state} {cancelChargeResult.Error}");
                    return false;
                }

                EnsureChargeIsCancelled(payment, out bool? isCancelSuccessful);
                if (isCancelSuccessful == true)
                {
                    status = $"Status: {ChargeStateEnum.cancelled}";
                    return true;
                }

                status = $"Status: {ChargeStateEnum.failed}";
                return false;
            }
            catch (ReepayException ex)
            {
                _logger.LogReepayException(ex);
                status = $"Status: {ChargeStateEnum.failed}, {ex.ReepayError.Error} {ex.ReepayError.Transaction_error}";
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                status = $"Status: {ChargeStateEnum.failed}, {ex.Message}";
                return false;
            }
        }

        /// <summary>
		/// Acquires the payment from the payment provider. This is often used when you need to call external services to handle the acquire process.
		/// </summary>
		/// <param name="payment">The payment.</param>
		/// <param name="status">The status.</param>
		/// <returns></returns>
        protected override bool AcquirePaymentInternal(Payment payment, out string status)
        {
            try
            {
                var settleChargeResult = _reepayRepository.SettleCharge(payment).ConfigureAwait(false).GetAwaiter().GetResult();
                if (settleChargeResult?.State == ChargeStateEnum.settled.ToString())
                {
                    status = $"Status: {settleChargeResult.State}, {settleChargeResult.Amount / 100m:N2}";
                    return true;
                }

                if (settleChargeResult?.State == ChargeStateEnum.failed.ToString() || settleChargeResult?.State == ChargeStateEnum.cancelled.ToString())
                {
                    status = $"Status: {settleChargeResult.State}, {settleChargeResult.Error_state} {settleChargeResult.Error}";
                    _logger.LogWarning($"Settle failed for order {payment?.PurchaseOrder?.OrderId}, error: {settleChargeResult.Error_state} {settleChargeResult.Error}");
                    return false;
                }

                EnsurePaymentIsValid(payment, out bool? isSettleSuccessful, true);
                if (isSettleSuccessful == true)
                {
                    status = $"Status: {ChargeStateEnum.settled}, {settleChargeResult?.Amount / 100m:N2}";
                    return true;
                }

                status = $"Status: {ChargeStateEnum.failed}";
                return false;
            }
            catch (ReepayException ex)
            {
                _logger.LogReepayException(ex);
                status = $"Status: {ChargeStateEnum.failed}, {ex.ReepayError.Error} {ex.ReepayError.Transaction_error}";
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                status = $"Status: {ChargeStateEnum.failed}, {ex.Message}";
                return false;
            }
        }

		/// <summary>
		/// Refunds the payment from the payment provider. This is often used when you need to call external services to handle the refund process.
		/// </summary>
		/// <param name="payment">The payment.</param>
		/// <param name="status">The status.</param>
		/// <returns></returns>
		protected override bool RefundPaymentInternal(Payment payment, out string status)
		{
			try
			{
                var createRefundResult = _reepayRepository.CreateRefund(payment).ConfigureAwait(false).GetAwaiter().GetResult();
                if (createRefundResult?.State == RefundStateEnum.refunded.ToString())
                {
                    status = $"Status: {createRefundResult.State}, {createRefundResult.Amount / 100m:N2}";
                    return true;
                }

                if (createRefundResult?.State == RefundStateEnum.failed.ToString())
                {
                    status = $"Status: {createRefundResult.State}, {createRefundResult.Error_state} {createRefundResult.Error}";
                    _logger.LogWarning($"Refund failed for order {payment?.PurchaseOrder?.OrderId}, error: {createRefundResult.Error_state} {createRefundResult.Error}");
                    return false;
                }

                EnsureRefundIsSuccessful(payment.PaymentMethod, createRefundResult.Id, out bool? isRefundSuccessful);
                if (isRefundSuccessful == true)
                {
                    status = $"Status: {RefundStateEnum.refunded}";
                    return true;
                }

                status = $"Status: {RefundStateEnum.failed}";
                return false;
            }
            catch (ReepayException ex)
            {
                _logger.LogReepayException(ex);
                status = $"Status: {RefundStateEnum.failed}, {ex.ReepayError.Error} {ex.ReepayError.Transaction_error}";
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                status = $"Status: {RefundStateEnum.failed}, {ex.Message}";
                return false;
            }
		}

        protected void CardChangeCallbackResult(Payment payment, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
			{
                _logger.LogWarning($"Cannot UpdateCreditCardInfo because source is empty for order {payment?.PurchaseOrder?.OrderId}");
                CompleteRequestError(payment, null, "No card info provided");
                return;
            }

            _reepaySubscriptionRepository.UpdateCreditCardInfo(payment, source).ConfigureAwait(false).GetAwaiter().GetResult();
            CompleteRequestSuccess(payment, PaymentStatusCode.Acquired);
        }

        protected void SubscriptionPaymentCallbackResult(Payment payment, string source)
        {
            var subscription = _reepaySubscriptionRepository.CreateSubscription(payment, HttpContext.Current.Request.QueryString["customer"], source).ConfigureAwait(false).GetAwaiter().GetResult();
            payment.PurchaseOrder[_reepayRepository.SubscriptionHandlePropertyName()] = subscription?.Handle;      
            EnsureSubscriptionIsSettled(payment.PaymentMethod, subscription?.Handle, out bool? isSubscriptionSettled);
            if (isSubscriptionSettled == true)
            {
                _logger.LogInfo($"ProcessCallback: subscription {subscription?.Handle} with plan {subscription?.Plan} created successfully for customer {subscription?.Customer}");
                payment.PurchaseOrder[_reepayRepository.SubscriptionSettledPropertyName()] = "true";
                payment.PurchaseOrder.Save();
                _reepaySubscriptionRepository.DeleteUsedDiscounts(payment, subscription?.Handle).ConfigureAwait(false).GetAwaiter().GetResult();
                CompleteRequestSuccess(payment, PaymentStatusCode.Acquired);
                return;
            }

            payment.PurchaseOrder.Save();
            _logger.LogWarning($"Subscription {subscription?.Handle} with plan {subscription?.Plan} was not settled for customer {subscription?.Customer}");
            CompleteRequestError(payment, PaymentStatusCode.Declined);
        }

        protected ChargeObject CreateChargeOnCheckout(Payment payment, string source)
        {
            ChargeObject charge = null;
            try
            {
                if (_reepayRepository.ShouldSubscriptionBeCreated(payment.PurchaseOrder))
                {
                    charge = _reepayRepository.CreateCharge(payment, source).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            catch (ReepayException ex)
            {
                _logger.LogReepayException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

            return charge;
        }

        protected void HandleCreateSubscriptionOnCheckout(Payment payment, string source)
		{
            try
            {
                if (_reepayRepository.ShouldSubscriptionBeCreated(payment.PurchaseOrder))
				{
                    var subscription = CreateSubscription(payment, source);
                    payment.PurchaseOrder[_reepayRepository.SubscriptionHandlePropertyName()] = subscription?.Handle;
                    EnsureSubscriptionIsSettled(payment.PaymentMethod, subscription?.Handle, out bool? isSubscriptionSettled);
                    if (isSubscriptionSettled == true)
                    {
                        _logger.LogInfo($"Subscription {subscription?.Handle} with plan {subscription?.Plan} created successfully for customer {subscription?.Customer}");
                        payment.PurchaseOrder[_reepayRepository.SubscriptionSettledPropertyName()] = "true";
                        _reepaySubscriptionRepository.DeleteUsedDiscounts(payment, subscription?.Handle).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    else
                    {
                        _logger.LogWarning($"Subscription {subscription?.Handle} with plan {subscription?.Plan} was not settled for customer {subscription?.Customer}");
                    }

                    payment.PurchaseOrder.Save();
                }
            }
            catch (ReepayException ex)
            {
                _logger.LogReepayException(ex);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        protected void EnsurePaymentIsValid(Payment payment, out bool? isPaymentValid, bool isSettle = false, ChargeObject chargeObject = null)
		{
            _logger.LogInfo($"ProcessCallback is validating the payment");
            isPaymentValid = _reepayRepository.PaymentIsValid(payment.PaymentMethod, payment.PurchaseOrder.OrderGuid.ToString(), isSettle, chargeObject).ConfigureAwait(false).GetAwaiter().GetResult();
            var endTime = DateTime.UtcNow.AddSeconds(60);
            while (isPaymentValid == null && endTime > DateTime.UtcNow)
            {
                Thread.Sleep(1000);
                isPaymentValid = _reepayRepository.PaymentIsValid(payment.PaymentMethod, payment.PurchaseOrder.OrderGuid.ToString(), isSettle).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        protected void EnsureSubscriptionIsSettled(PaymentMethod paymentMethod, string subscriptionHandle, out bool? isSubscriptionSettled)
        {
            _logger.LogInfo($"ProcessCallback is validating the subscription");
            isSubscriptionSettled = _reepaySubscriptionRepository.SubscriptionIsSettledSuccessfully(paymentMethod, subscriptionHandle).ConfigureAwait(false).GetAwaiter().GetResult();
            var endTime = DateTime.UtcNow.AddSeconds(60);
            while (isSubscriptionSettled == null && endTime > DateTime.UtcNow)
            {
                Thread.Sleep(1000);
                isSubscriptionSettled = _reepaySubscriptionRepository.SubscriptionIsSettledSuccessfully(paymentMethod, subscriptionHandle).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        protected void EnsureRefundIsSuccessful(PaymentMethod paymentMethod, string refundId, out bool? isRefundSuccessful)
        {
            _logger.LogInfo($"ProcessCallback is validating the refund");
            isRefundSuccessful = _reepayRepository.RefundIsSuccessful(paymentMethod, refundId).ConfigureAwait(false).GetAwaiter().GetResult();
            var endTime = DateTime.UtcNow.AddSeconds(60);
            while (isRefundSuccessful == null && endTime > DateTime.UtcNow)
            {
                Thread.Sleep(1000);
                isRefundSuccessful = _reepayRepository.RefundIsSuccessful(paymentMethod, refundId).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        protected void EnsureChargeIsCancelled(Payment payment, out bool? isCancelSuccessful)
		{
            _logger.LogInfo($"ProcessCallback is validating the payment cancellation");
            isCancelSuccessful = _reepayRepository.PaymentIsCancelled(payment.PaymentMethod, payment.PurchaseOrder.OrderGuid.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
            var endTime = DateTime.UtcNow.AddSeconds(60);
            while (isCancelSuccessful == null && endTime > DateTime.UtcNow)
            {
                Thread.Sleep(1000);
                isCancelSuccessful = _reepayRepository.PaymentIsCancelled(payment.PaymentMethod, payment.PurchaseOrder.OrderGuid.ToString()).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        protected void RemoveCardOnFileProperty(PurchaseOrder order)
		{
            var cardOnFileProperty = order.GetOrderProperties()
                    .Where(x => x.Key == _reepayRepository.CardOnFilePropertyName()).FirstOrDefault();
            if (cardOnFileProperty != null)
            {
                order.RemoveOrderProperty(cardOnFileProperty);
                order.Save();
            }
        }

        protected virtual void CompleteRequestSuccess(Payment payment, PaymentStatusCode? paymentStatusCode)
		{
            if (paymentStatusCode != null)
			{
                payment.PaymentStatus = PaymentStatus.Get((int)paymentStatusCode);
            }
            
            ProcessPaymentRequest(new PaymentRequest(payment.PurchaseOrder, payment));
            HttpContext.Current.Response.Redirect(_reepayRepository.GetAcceptUrl(payment.PaymentMethod, payment.PurchaseOrder).AddOrderGuidParameter(payment.PurchaseOrder).ToString(), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected virtual void CompleteRequestError(Payment payment, PaymentStatusCode? paymentStatusCode = null, string errorMessage = "")
        {
            if (paymentStatusCode != null)
            {
                payment.PaymentStatus = PaymentStatus.Get((int)paymentStatusCode);
                payment.Save();
            }

            var uriToRedirect = _reepayRepository.GetErrorUrl(payment.PaymentMethod).AddOrderGuidParameter(payment.PurchaseOrder);
            if (!string.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["error"]))
            {
                uriToRedirect = uriToRedirect.AddQueryStringParameter("error", HttpContext.Current.Request.QueryString["error"]);
            }
            else if (!string.IsNullOrWhiteSpace(errorMessage))
			{
                uriToRedirect = uriToRedirect.AddQueryStringParameter("error", errorMessage);
            }

            HttpContext.Current.Response.Redirect(uriToRedirect.ToString(), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected virtual void CompleteRequestCancel(Payment payment)
        {
            HttpContext.Current.Response.Redirect(_reepayRepository.GetCancelUrl(payment.PaymentMethod, payment.PurchaseOrder).ToString(), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private SubscriptionObject CreateSubscription(Payment payment, string source)
		{
            SubscriptionObject subscription;
            try
            {
                subscription = _reepaySubscriptionRepository.CreateSubscription(payment, HttpContext.Current.Request.QueryString["customer"], source).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (ReepayException ex)
            {
                switch (ex.ReepayError.Code)
                {
                    case 83:
                    case 85:
                    case 86:
                    case 87:
                        subscription = _reepaySubscriptionRepository.CreateSubscription(payment, HttpContext.Current.Request.QueryString["customer"], source, "", false).ConfigureAwait(false).GetAwaiter().GetResult();
                        break;
                    default: throw (ex);
                }
            }

            return subscription;
        }
    }
}
