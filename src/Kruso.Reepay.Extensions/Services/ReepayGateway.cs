using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kruso.Reepay.Extensions.Models;
using Kruso.Reepay.Extensions.Services.Interfaces;
using Newtonsoft.Json;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;

namespace Kruso.Reepay.Extensions.Services
{
	public class ReepayGateway : IReepayGateway, IDisposable
	{
		private const string ApiSessionUrl = "https://checkout-api.reepay.com/v1/session/";
		private const string BaseApiUrl = "https://api.reepay.com/v1/";
		private const string ChargeUrl = BaseApiUrl + "charge";
		private const string SubscriptionUrl = BaseApiUrl + "subscription/";
		private const string RefundUrl = BaseApiUrl + "refund";
		private const string CustomerUrl = BaseApiUrl + "customer";
		private const string WebhookUrl = BaseApiUrl + "webhook/";
		private const string InvoiceUrl = BaseApiUrl + "invoice/";

		private readonly IReepayLogger<IReepayGateway> _logger;
		private readonly HttpClient _client;
		private bool _authHeaderAdded;

		private readonly JsonSerializerSettings _jsonSerializerSettings;

		public ReepayGateway(IReepayLogger<IReepayGateway> logger)
		{
			_logger = logger;
			_client = new HttpClient();
			_client.Timeout = new TimeSpan(0, 5, 0);
			_client.DefaultRequestHeaders.Accept.Clear();
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_authHeaderAdded = false;
			_jsonSerializerSettings = new JsonSerializerSettings
			{
				DefaultValueHandling = DefaultValueHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};
		}

		public void Dispose()
		{
			_client.Dispose();
		}

		public async Task<string> CreateSessionUrl(PaymentMethod paymentMethod, SessionData sessionData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var url = ApiSessionUrl + sessionData.Type;
			var sessionUrl = await CreateSessionUrl(url, sessionData.Data).ConfigureAwait(false);

			return sessionUrl;
		}

		public async Task<ChargeObject> GetCharge(PaymentMethod paymentMethod, string handle)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var chargeObject = await GetCharge(handle).ConfigureAwait(false);

			return chargeObject;
		}

		public async Task<InvoiceObject> GetInvoice(PaymentMethod paymentMethod, string handle)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var invoiceObject = await GetInvoice(handle).ConfigureAwait(false);

			return invoiceObject;
		}

		public async Task<CustomerObject> GetCustomer(PaymentMethod paymentMethod, string handle)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var customerObject = await GetCustomer(handle).ConfigureAwait(false);

			return customerObject;
		}

		public async Task<ChargeObject> CreateCharge(PaymentMethod paymentMethod, CreateChargeRequest requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var chargeObject = await CreateCharge(requestData).ConfigureAwait(false);

			return chargeObject;
		}

		public async Task<SubscriptionObject> GetSubscription(PaymentMethod paymentMethod, string handle)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var subscriptionObject = await GetSubscription(handle).ConfigureAwait(false);

			return subscriptionObject;
		}

		public async Task<IList<WebhookObject>> GetWebhooks(PaymentMethod paymentMethod, string id)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var webhooks = await GetWebhooks(id).ConfigureAwait(false);

			return webhooks;
		}

		public async Task<CustomerObject> CreateCustomer(PaymentMethod paymentMethod, CreateCustomer requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var customer = await CreateCustomer(requestData).ConfigureAwait(false);

			return customer;
		}

		public async Task<SubscriptionObject> CreateSubscription(PaymentMethod paymentMethod, CreateSubscriptionRequest requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var subscription = await CreateSubscription(requestData).ConfigureAwait(false);

			return subscription;
		}

		public async Task<IList<SubscriptionDiscount>> GetSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var subscriptionDiscountList = await GetSubscriptionDiscounts(subscriptionHandle).ConfigureAwait(false);

			return subscriptionDiscountList;
		}

		public async Task<RefundObject> CreateRefund(PaymentMethod paymentMethod, CreateRefundRequest requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var refund = await CreateRefund(requestData).ConfigureAwait(false);

			return refund;
		}

		public async Task<ChargeObject> SettleCharge(PaymentMethod paymentMethod, string chargeHandle, SettleChargeRequest requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var charge = await SettleCharge(chargeHandle, requestData).ConfigureAwait(false);

			return charge;
		}

		public async Task<ChargeObject> CancelCharge(PaymentMethod paymentMethod, string chargeHandle)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var charge = await CancelCharge(chargeHandle).ConfigureAwait(false);

			return charge;
		}

		public async Task<RefundObject> GetRefund(PaymentMethod paymentMethod, string id)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var refundObject = await GetRefund(id).ConfigureAwait(false);

			return refundObject;
		}

		public async Task<SubscriptionObject> ChangeSubscription(PaymentMethod paymentMethod, string subscriptionHandle, ChangeSubscriptionRequest requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var subscription = await ChangeSubscription(subscriptionHandle, requestData).ConfigureAwait(false);

			return subscription;
		}

		public async Task UpdateCustomer(PaymentMethod paymentMethod, string customerHandle, CustomerAddress requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			await UpdateCustomer(customerHandle, requestData).ConfigureAwait(false);
		}

		public async Task<CardObject> AddCardToCustomer(PaymentMethod paymentMethod, string customerHandle, AddCardPaymentMethodRequest requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var cardObject = await AddCardToCustomer(customerHandle, requestData).ConfigureAwait(false);

			return cardObject;
		}

		public async Task<SubscriptionObject> CancelSubscription(PaymentMethod paymentMethod, string subscriptionHandle, CancelSubscriptionRequest requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var subscription = await CancelSubscription(subscriptionHandle, requestData).ConfigureAwait(false);

			return subscription;
		}

		public async Task<SubscriptionObject> UnCancelSubscription(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			var subscription = await UnCancelSubscription(subscriptionHandle).ConfigureAwait(false);

			return subscription;
		}

		public async Task RemoveAllSubscriptionCards(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			await RemoveAllSubscriptionCards(subscriptionHandle).ConfigureAwait(false);
		}

		public async Task SetSubscriptionCard(PaymentMethod paymentMethod, string subscriptionHandle, SetPaymentMethodRequest requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			await SetSubscriptionCard(subscriptionHandle, requestData).ConfigureAwait(false);
		}

		public async Task DeleteSubscriptionDiscount(PaymentMethod paymentMethod, string subscriptionHandle, string subscriptionDiscountHandle)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			await DeleteSubscriptionDiscount(subscriptionHandle, subscriptionDiscountHandle).ConfigureAwait(false);
		}

		public async Task DeleteAllSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			IList<SubscriptionDiscount> subscriptionDiscounts = null;
			try
			{
				subscriptionDiscounts = await GetSubscriptionDiscounts(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			}
			catch (ReepayException ex)
			{
				_logger.LogReepayException(ex);
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
			}

			if (subscriptionDiscounts == null || subscriptionDiscounts.Count() == 0) return;

			var listOfTasks = subscriptionDiscounts
				.Select(discount => DeleteSubscriptionDiscount(paymentMethod, subscriptionHandle, discount.Handle));

			await Task.WhenAll(listOfTasks);
		}

		public async Task AddSubscriptionDiscount(PaymentMethod paymentMethod, string subscriptionHandle, CreateSubscriptionDiscount discountData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			await AddSubscriptionDiscount(subscriptionHandle, discountData).ConfigureAwait(false);
		}

		public Task AddSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle, IList<CreateSubscriptionDiscount> subscriptionDiscounts)
		{
			var listOfTasks = subscriptionDiscounts
				.Select(discount => AddSubscriptionDiscount(paymentMethod, subscriptionHandle, discount));

			return Task.WhenAll(listOfTasks);
		}

		public async Task RedeemCouponCode(PaymentMethod paymentMethod, string subscriptionHandle, RedeemCouponCodeRequest requestData)
		{
			EnsureAuthHeaderAdded(paymentMethod);
			await RedeemCouponCode(subscriptionHandle, requestData).ConfigureAwait(false);
		}

		public Task RedeemCouponCodes(PaymentMethod paymentMethod, string subscriptionHandle, IList<string> couponCodes)
		{
			var listOfTasks = couponCodes
				.Select(code => RedeemCouponCode(
					paymentMethod,
					subscriptionHandle,
					new RedeemCouponCodeRequest() { Code = code }
				));

			return Task.WhenAll(listOfTasks);
		}

		#region Private
		private async Task<string> CreateSessionUrl(string url, string data)
		{
			HttpContent body = new StringContent(data);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to create session, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {data}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<CreateSessionResponse>().ConfigureAwait(false);
			return res?.Url;
		}

		private async Task<ChargeObject> GetCharge(string handle)
		{
			var url = ChargeUrl + "/" + handle;
			HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to get charge object, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<ChargeObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<InvoiceObject> GetInvoice(string handle)
		{
			var url = InvoiceUrl + handle;
			HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to get invoice object, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<InvoiceObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<CustomerObject> GetCustomer(string handle)
		{
			try
			{
				var url = CustomerUrl + "/" + handle;
				HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);

				if (!response.IsSuccessStatusCode)
				{
					var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
					_logger.LogWarning($"Unable to get customer object, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
					throw new ReepayException(resErr);
				}

				var res = await response.Content.ReadAsAsync<CustomerObject>().ConfigureAwait(false);
				return res;
			}
			catch(Exception ex)
			{
				_logger.LogException(ex);
				return null;
			}
		}

		private async Task<ChargeObject> CreateCharge(CreateChargeRequest data)
		{
			var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await _client.PostAsync(ChargeUrl, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to create subscription, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {ChargeUrl}, input data: {stringData}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<ChargeObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<CustomerObject> CreateCustomer(CreateCustomer data)
		{
			var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await _client.PostAsync(CustomerUrl, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to create customer, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {CustomerUrl}, input data: {stringData}");
				throw new ReepayException(resErr);

			}

			var res = await response.Content.ReadAsAsync<CustomerObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<SubscriptionObject> CreateSubscription(CreateSubscriptionRequest data)
		{
			var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await _client.PostAsync(SubscriptionUrl, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to create subscription, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {SubscriptionUrl}, input data: {stringData}");
				throw new ReepayException(resErr);

			}

			var res = await response.Content.ReadAsAsync<SubscriptionObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<SubscriptionObject> GetSubscription(string handle)
		{
			var url = SubscriptionUrl + handle;
			HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to get subscription object, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);

			}

			var res = await response.Content.ReadAsAsync<SubscriptionObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<IList<WebhookObject>> GetWebhooks(string id)
		{
			var url = WebhookUrl + id;
			HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to get webhooks, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);

			}

			var res = await response.Content.ReadAsAsync<IList<WebhookObject>>().ConfigureAwait(false);
			return res;
		}

		private async Task<IList<SubscriptionDiscount>> GetSubscriptionDiscounts(string subscriptionHandle)
		{
			var url = SubscriptionUrl + subscriptionHandle + "/discount";
			HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to get subscription discounts, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<IList<SubscriptionDiscount>>().ConfigureAwait(false);
			return res;
		}

		private async Task<RefundObject> CreateRefund(CreateRefundRequest data)
		{
			var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await _client.PostAsync(RefundUrl, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to create refund, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {RefundUrl}, input data: {stringData}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<RefundObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<ChargeObject> SettleCharge(string handle, SettleChargeRequest data)
		{
			var url = ChargeUrl + "/" + handle + "/settle";
			var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to settle charge, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<ChargeObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<ChargeObject> CancelCharge(string handle)
		{
			var url = ChargeUrl + "/" + handle + "/cancel";
			HttpContent body = new StringContent("");
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to cancel charge, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<ChargeObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<RefundObject> GetRefund(string id)
		{
			var url = RefundUrl + $"/{id}";
			HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to get refund object, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<RefundObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<SubscriptionObject> ChangeSubscription(string subscriptionHandle, ChangeSubscriptionRequest data)
		{
			var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var url = SubscriptionUrl + subscriptionHandle;
			HttpResponseMessage response = await _client.PutAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to change subscription, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<SubscriptionObject>().ConfigureAwait(false);
			return res;
		}

		private async Task UpdateCustomer(string customerHandle, CustomerAddress data)
		{
			var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var url = CustomerUrl + "/" + customerHandle;
			HttpResponseMessage response = await _client.PutAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to update customer, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
				throw new ReepayException(resErr);
			}
		}

		private async Task<CardObject> AddCardToCustomer(string customerHandle, AddCardPaymentMethodRequest data)
		{
			var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var url = CustomerUrl + "/" + customerHandle + "/payment_method/card";
			HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to add card to customer, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<CardObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<SubscriptionObject> CancelSubscription(string subscriptionHandle, CancelSubscriptionRequest data)
		{
			var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var url = SubscriptionUrl + subscriptionHandle + "/cancel";
			HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to cancel subscription, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<SubscriptionObject>().ConfigureAwait(false);
			return res;
		}

		private async Task<SubscriptionObject> UnCancelSubscription(string subscriptionHandle)
		{
			HttpContent body = new StringContent("");
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var url = SubscriptionUrl + subscriptionHandle + "/uncancel";
			HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to unCancel subscription, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);
			}

			var res = await response.Content.ReadAsAsync<SubscriptionObject>().ConfigureAwait(false);
			return res;
		}

		private async Task RemoveAllSubscriptionCards(string subscriptionHandle)
		{
			var url = SubscriptionUrl + subscriptionHandle + "/payment_method";
			HttpResponseMessage response = await _client.DeleteAsync(url).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to remove all subscription cards, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);
			}
		}

		private async Task SetSubscriptionCard(string subscriptionHandle, SetPaymentMethodRequest requestData)
		{
			var stringData = JsonConvert.SerializeObject(requestData, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var url = SubscriptionUrl + subscriptionHandle + "/payment_method";
			HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to set subscription card, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
				throw new ReepayException(resErr);
			}
		}

		private async Task DeleteSubscriptionDiscount(string subscriptionHandle, string subscriptionDiscountHandle)
		{
			var url = SubscriptionUrl + subscriptionHandle + "/discount/" + subscriptionDiscountHandle;
			HttpResponseMessage response = await _client.DeleteAsync(url).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to delete subscription discount, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
				throw new ReepayException(resErr);
			}
		}

		private async Task AddSubscriptionDiscount(string subscriptionHandle, CreateSubscriptionDiscount discountData)
		{
			var stringData = JsonConvert.SerializeObject(discountData, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var url = SubscriptionUrl + subscriptionHandle + "/discount";
			HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to add subscription discount, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
				throw new ReepayException(resErr);
			}
		}

		private async Task RedeemCouponCode(string subscriptionHandle, RedeemCouponCodeRequest requestData)
		{
			var stringData = JsonConvert.SerializeObject(requestData, _jsonSerializerSettings);
			HttpContent body = new StringContent(stringData);
			body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var url = SubscriptionUrl + subscriptionHandle + "/coupon";
			HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var resErr = await response.Content.ReadAsAsync<ErrorResponse>().ConfigureAwait(false);
				_logger.LogWarning($"Unable to redeem a coupon code for subscription, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
				throw new ReepayException(resErr);
			}
		}

		private void EnsureAuthHeaderAdded(PaymentMethod paymentMethod)
		{
			try
			{
				if (!_authHeaderAdded)
				{
					var reepayPrivateKey = paymentMethod.DynamicProperty<string>().ReepayPrivateKey;
					var credentialBase64 = Convert.ToBase64String(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(reepayPrivateKey + ":"));
					_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentialBase64);
					_authHeaderAdded = true;
				}
			}
			catch(ReepayException ex)
			{
				_logger.LogReepayException(ex, "AuthHeader was not added");
				throw ex;
			}
			catch(Exception ex)
			{
				_logger.LogException(ex, "AuthHeader was not added");
				throw ex;
			}
		}

		#endregion
	}
}
