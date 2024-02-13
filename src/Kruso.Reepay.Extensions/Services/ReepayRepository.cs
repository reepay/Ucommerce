using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Kruso.Reepay.Extensions.Enums;
using Kruso.Reepay.Extensions.Models;
using Kruso.Reepay.Extensions.Services.Interfaces;
using Newtonsoft.Json;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;
using Ucommerce.Transactions.Payments;
using Ucommerce.Transactions.Payments.Common;
using Ucommerce.Web;

namespace Kruso.Reepay.Extensions.Services
{
	public class ReepayRepository : IReepayRepository
	{
		private readonly IAbsoluteUrlService _absoluteUrlService;
		private readonly IReepayLogger<IReepayRepository> _logger;
		private readonly ICallbackUrl _callbackUrl;
		private readonly IReepayGateway _reepayGateway;
		private readonly IReepayPropertyNames _reepayPropertyNames;
		private readonly JsonSerializerSettings _jsonSerializerSettings;

		public ReepayRepository(
			IAbsoluteUrlService absoluteUrlService,
			IReepayLogger<IReepayRepository> logger,
			ICallbackUrl callbackUrl,
			IReepayGateway reepayGateway,
			IReepayPropertyNames propertyNames)
		{
			_absoluteUrlService = absoluteUrlService;
			_logger = logger;
			_callbackUrl = callbackUrl;
			_reepayGateway = reepayGateway;
			_reepayPropertyNames = propertyNames;
			_jsonSerializerSettings = new JsonSerializerSettings
			{
				DefaultValueHandling = DefaultValueHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};
		}

		public async Task<string> GetSessionUrl(PaymentRequest paymentRequest)
		{
			return await _reepayGateway.CreateSessionUrl(paymentRequest.PaymentMethod, GetSessionData(paymentRequest)).ConfigureAwait(false);
		}

		public async Task<bool?> PaymentIsValid(PaymentMethod paymentMethod, string handle, bool isSettle = false, ChargeObject chargeObject = null)
		{
			try
			{
				if (chargeObject == null)
				{
					chargeObject = await _reepayGateway.GetCharge(paymentMethod, handle).ConfigureAwait(false);
				}

				if (chargeObject == null)
				{
					_logger.LogWarning($"Charge object is null, {handle}");
					throw new Exception(
						$"error: Charge object is null for handle {handle}");
				}

				if (chargeObject.State == ChargeStateEnum.authorized.ToString() || chargeObject.State == ChargeStateEnum.settled.ToString())
				{
					if (!isSettle)
					{
						return true;
					}

					if (chargeObject.State == ChargeStateEnum.settled.ToString())
					{
						return true;
					}
				}

				if (chargeObject.State == ChargeStateEnum.cancelled.ToString() || chargeObject.State == ChargeStateEnum.failed.ToString())
				{
					return false;
				}

				// pending status
				_logger.LogWarning($"ChargeObject state for handle {handle} is pending");
				return null;
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
				return null;
			}
		}

		public async Task<bool?> PaymentIsCancelled(PaymentMethod paymentMethod, string handle)
		{
			try
			{
				var chargeObject = await _reepayGateway.GetCharge(paymentMethod, handle).ConfigureAwait(false);

				if (chargeObject == null)
				{
					_logger.LogWarning($"Charge object is null, {handle}");
					throw new Exception(
						$"error: Charge object is null for handle {handle}");
				}

				if (chargeObject.State == ChargeStateEnum.cancelled.ToString())
				{
					return true;
				}

				if (chargeObject.State == ChargeStateEnum.failed.ToString() || chargeObject.State == ChargeStateEnum.settled.ToString())
				{
					return false;
				}

				// pending status
				_logger.LogWarning($"ChargeObject state for handle {handle} is pending");
				return null;
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
				return null;
			}
		}

		public async Task<bool?> RefundIsSuccessful(PaymentMethod paymentMethod, string id)
		{
			try
			{
				var refundObject = await _reepayGateway.GetRefund(paymentMethod, id).ConfigureAwait(false);

				if (refundObject == null)
				{
					_logger.LogWarning($"Refund object is null, {id}");
					throw new Exception(
						$"error: Refund object is null for id {id}");
				}

				if (refundObject.State == RefundStateEnum.refunded.ToString())
				{
					return true;
				}

				if (refundObject.State == ChargeStateEnum.failed.ToString())
				{
					return false;
				}

				// pending status
				_logger.LogWarning($"RefundObject state for id {id} is pending");
				return null;
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
				return null;
			}
		}

		public Uri GetAcceptUrl(PaymentMethod paymentMethod, PurchaseOrder order = null)
		{
			try
			{
				if (order != null && IsCardChange(order) && !string.IsNullOrWhiteSpace(paymentMethod.DynamicProperty<string>().Accept_url_change_cc))
				{
					return new Uri(_absoluteUrlService.GetAbsoluteUrl(paymentMethod.DynamicProperty<string>().Accept_url_change_cc));
				}
			}
			catch(Exception ex)
			{
				_logger.LogException(ex);
			}

			return new Uri(_absoluteUrlService.GetAbsoluteUrl(paymentMethod.DynamicProperty<string>().Accept_url));
		}

		public Uri GetErrorUrl(PaymentMethod paymentMethod)
		{
			return new Uri(_absoluteUrlService.GetAbsoluteUrl(paymentMethod.DynamicProperty<string>().Error_url));
		}

		public Uri GetCancelUrl(PaymentMethod paymentMethod, PurchaseOrder order = null)
		{
			try
			{
				if (order != null && (IsCardChange(order) || IsSubscriptionPayment(order)) && !string.IsNullOrWhiteSpace(paymentMethod.DynamicProperty<string>().Cancel_url_my_account))
				{
					return new Uri(_absoluteUrlService.GetAbsoluteUrl(paymentMethod.DynamicProperty<string>().Cancel_url_my_account));
				}
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
			}

			return new Uri(_absoluteUrlService.GetAbsoluteUrl(paymentMethod.DynamicProperty<string>().Cancel_url));
		}

		public virtual async Task<RefundObject> CreateRefund(Payment payment)
		{
			if (payment == null || payment.PurchaseOrder == null)
			{
				_logger.LogWarning("Payment or order is empty, cannot get order id");
				return null;
			}

			var amount = payment.PurchaseOrder.OrderTotal;
			if (ShouldSubscriptionBeCreated(payment.PurchaseOrder))
			{
				amount -= GetSubscriptionAmount(payment.PurchaseOrder);
			}

			var requestData = new CreateRefundRequest()
			{
				Invoice = payment.PurchaseOrder.OrderGuid.ToString(),
				Amount = amount.ToCents()
			};

			return await _reepayGateway.CreateRefund(payment.PaymentMethod, requestData).ConfigureAwait(false);
		}

		public virtual async Task<ChargeObject> SettleCharge(Payment payment)
		{
			if (payment == null || payment.PurchaseOrder == null)
			{
				_logger.LogWarning("Payment or order is empty, cannot get order id");
				return null;
			}

			var amount = payment.PurchaseOrder.OrderTotal;
			if (ShouldSubscriptionBeCreated(payment.PurchaseOrder))
			{
				amount -= GetSubscriptionAmount(payment.PurchaseOrder);
			}

			var requestData = new SettleChargeRequest()
			{
				Amount = amount.ToCents()
			};

			return await _reepayGateway.SettleCharge(payment.PaymentMethod, payment.PurchaseOrder.OrderGuid.ToString(), requestData).ConfigureAwait(false);
		}

		public virtual async Task<ChargeObject> CancelCharge(Payment payment)
		{
			if (payment == null || payment.PurchaseOrder == null)
			{
				_logger.LogWarning("Payment or order is empty, cannot get order id");
				return null;
			}

			return await _reepayGateway.CancelCharge(payment.PaymentMethod, payment.PurchaseOrder.OrderGuid.ToString()).ConfigureAwait(false);
		}

		public virtual Task<ChargeObject> CreateCharge(Payment payment, string source)
		{
			var requestData = new CreateChargeRequest(GetReepayOrder(payment))
			{
				Source = source
			};

			return _reepayGateway.CreateCharge(payment.PaymentMethod, requestData);
		}

		public virtual Task<CustomerObject> CreateCustomer(PaymentMethod paymentMethod,
			string customerHandle,
			string email,
			string address,
			string address2,
			string city,
			string country,
			string phone,
			string company,
			string vat,
			string first_name,
			string last_name,
			string postal_code)
		{
			var requestData = new CreateCustomer()
			{
				Email = email,
				Address = address,
				Address2 = address2,
				City = city,
				Company = company,
				Country = country,
				First_name = first_name,
				Last_name = last_name,
				Phone = phone,
				Postal_code = postal_code,
				Vat = vat
			};

			if (string.IsNullOrWhiteSpace(customerHandle))
			{
				requestData.Generate_handle = true;
			}
			else
			{
				requestData.Handle = customerHandle;
			}

			return _reepayGateway.CreateCustomer(paymentMethod, requestData);
		}

		public virtual async Task UpdateCustomer(PaymentMethod paymentMethod,
			string customerHandle,
			string email,
			string address,
			string address2,
			string city,
			string country,
			string phone,
			string company,
			string vat,
			string first_name,
			string last_name,
			string postal_code)
		{
			if (string.IsNullOrWhiteSpace(customerHandle))
			{
				_logger.LogWarning("customer handle is empty, cannot update customer in Reepay");
				return;
			}

			var requestData = new CustomerAddress()
			{
				Email = email,
				Address = address,
				Address2 = address2,
				City = city,
				Company = company,
				Country = country,
				First_name = first_name,
				Last_name = last_name,
				Phone = phone,
				Postal_code = postal_code,
				Vat = vat
			};

			await _reepayGateway.UpdateCustomer(paymentMethod, customerHandle, requestData).ConfigureAwait(false);
		}

		public virtual async Task<CardObject> AddCardToCustomer(PaymentMethod paymentMethod, string customerHandle, string cardToken)
		{
			if (string.IsNullOrWhiteSpace(customerHandle))
			{
				_logger.LogWarning("customer handle is empty, cannot add card to customer in Reepay");
				return null;
			}

			if (string.IsNullOrWhiteSpace(cardToken))
			{
				_logger.LogWarning($"cardToken is empty, cannot add card to customer {customerHandle} in Reepay");
				return null;
			}

			return await _reepayGateway.AddCardToCustomer(paymentMethod, customerHandle, new AddCardPaymentMethodRequest() { Card_token = cardToken }).ConfigureAwait(false);
		}

		public virtual Task<IList<WebhookObject>> GetWebhooks(PaymentMethod paymentMethod, string id)
		{
			return _reepayGateway.GetWebhooks(paymentMethod, id);
		}

		public virtual Task<InvoiceObject> GetInvoice(PaymentMethod paymentMethod, string handle)
		{
			return _reepayGateway.GetInvoice(paymentMethod, handle);
		}

		public virtual Task<CustomerObject> GetCustomer(PaymentMethod paymentMethod, string handle)
		{
			return _reepayGateway.GetCustomer(paymentMethod, handle);
		}

		public virtual bool IsCardChange(PurchaseOrder purchaseOrder)
		{
			return _reepayPropertyNames.GetBoolPropertyValueFromOrder(purchaseOrder, _reepayPropertyNames.IsCardChange);
		}

		/// <summary>
		/// Returns true if only subscription (with no other products) should be bought
		/// </summary>
		/// <param name="purchaseOrder"></param>
		/// <returns></returns>
		public virtual bool IsSubscriptionPayment(PurchaseOrder purchaseOrder)
		{
			return _reepayPropertyNames.GetBoolPropertyValueFromOrder(purchaseOrder, _reepayPropertyNames.IsSubscriptionPayment);
		}

		/// <summary>
		/// Returns true if subscription should be bought together with other products
		/// </summary>
		/// <param name="purchaseOrder"></param>
		/// <returns></returns>
		public virtual bool ShouldSubscriptionBeCreated(PurchaseOrder purchaseOrder)
		{
			return _reepayPropertyNames.GetBoolPropertyValueFromOrder(purchaseOrder, _reepayPropertyNames.ShouldSubscriptionBeCreated);
		}

		public virtual string SubscriptionHandlePropertyName()
		{
			return _reepayPropertyNames.SubscriptionHandle;
		}

		public virtual string SubscriptionSettledPropertyName()
		{
			return _reepayPropertyNames.IsSubscriptionSettled;
		}

		public virtual string CardOnFilePropertyName()
		{
			return _reepayPropertyNames.CardOnFile;
		}

		public virtual string CardOnFilePropertyValue(PurchaseOrder purchaseOrder)
		{
			return _reepayPropertyNames.GetPropertyValueFromOrder(purchaseOrder, _reepayPropertyNames.CardOnFile);
		}

		protected virtual string GetChargeSessionProperties(PaymentRequest paymentRequest)
		{
			try
			{
				var order = paymentRequest?.PurchaseOrder;
				if (order == null)
				{
					_logger.LogWarning("Order is null");
					return JsonConvert.SerializeObject(EmptySessionProperties(paymentRequest), _jsonSerializerSettings);
				}

				var chargeSessionProperties = new ChargeSessionProperties();
				InitSessionProperties(paymentRequest, chargeSessionProperties);
				if (string.IsNullOrWhiteSpace(chargeSessionProperties.Card_on_file))
				{
					chargeSessionProperties.Recurring_optional = true;
				}
				chargeSessionProperties.Order = GetReepayOrder(paymentRequest.Payment);

				return JsonConvert.SerializeObject(chargeSessionProperties, _jsonSerializerSettings);
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
				return JsonConvert.SerializeObject(EmptySessionProperties(paymentRequest), _jsonSerializerSettings);
			}
		}

		protected virtual string GetRecurringSessionProperties(PaymentRequest paymentRequest)
		{
			try
			{
				var order = paymentRequest?.PurchaseOrder;
				if (order == null)
				{
					_logger.LogWarning("Order is null");
					return JsonConvert.SerializeObject(EmptySessionProperties(paymentRequest), _jsonSerializerSettings);
				}

				var recurringSessionProperties = new RecurringSessionProperties();
				InitSessionProperties(paymentRequest, recurringSessionProperties);
				recurringSessionProperties.Create_customer = GetReepayCustomer(paymentRequest.Payment);
				if (!IsCardChange(order))
				{
					SetButtonTextForSubscription(paymentRequest, recurringSessionProperties);
				}

				return JsonConvert.SerializeObject(recurringSessionProperties, _jsonSerializerSettings);
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
				return JsonConvert.SerializeObject(EmptySessionProperties(paymentRequest), _jsonSerializerSettings);
			}
		}

		protected virtual void InitSessionProperties<T>(PaymentRequest paymentRequest, T sessionProperties) where T : SessionProperties
		{
			var callbackUrl = _callbackUrl.GetCallbackUrl("(auto)", paymentRequest.Payment);
			sessionProperties.Accept_url = callbackUrl;
			sessionProperties.Cancel_url = $"{callbackUrl}?cancel=true";

			var cardOnFile = _reepayPropertyNames.GetPropertyValueFromOrder(paymentRequest?.PurchaseOrder, _reepayPropertyNames.CardOnFile);
			if (!string.IsNullOrWhiteSpace(cardOnFile))
			{
				sessionProperties.Card_on_file = cardOnFile;
			}
		}

		protected virtual CreateCustomer GetReepayCustomer(Payment payment)
		{
			var order = payment?.PurchaseOrder;
			if (order == null) return new CreateCustomer(payment?.PaymentMethod)
			{
				Generate_handle = true,
				Country = "DK"
			};

			var billingAddress = order.BillingAddress;
			var customer = order.Customer;
			var reepayCustomer = new CreateCustomer(payment?.PaymentMethod)
			{
				Email = customer?.EmailAddress ?? billingAddress?.EmailAddress ?? string.Empty,
				First_name = customer?.FirstName ?? billingAddress?.FirstName ?? string.Empty,
				Last_name = customer?.LastName ?? billingAddress?.LastName ?? string.Empty,
				Phone = customer?.PhoneNumber ?? billingAddress?.PhoneNumber ?? string.Empty,
				Address = billingAddress?.Line1 ?? string.Empty,
				Address2 = billingAddress?.Line2 ?? string.Empty,
				City = billingAddress?.City ?? string.Empty,
				Company = billingAddress?.CompanyName ?? string.Empty,
				Country = billingAddress?.Country?.TwoLetterISORegionName ?? "DK",
				Postal_code = billingAddress?.PostalCode ?? string.Empty
			};

			var customerHandle = _reepayPropertyNames.GetPropertyValueFromOrder(order, _reepayPropertyNames.Customer);
			if (!string.IsNullOrWhiteSpace(customerHandle))
			{
				reepayCustomer.Handle = customerHandle;
				return reepayCustomer;
			}

			if (customer != null)
			{
				reepayCustomer.Handle = customer.Id.ToString();
				return reepayCustomer;
			}

			reepayCustomer.Generate_handle = true;
			return reepayCustomer;
		}

		protected virtual Models.Address GetReepayAddress(OrderAddress orderAddress)
		{
			if (orderAddress == null) return null;
			return new Models.Address()
			{
				Company = orderAddress.CompanyName,
				Address = orderAddress.AddressName,
				City = orderAddress.City,
				Country = orderAddress.Country?.TwoLetterISORegionName ?? "DK",
				Email = orderAddress.EmailAddress,
				Phone = orderAddress.PhoneNumber,
				Postal_code = orderAddress.PostalCode,
				First_name = orderAddress.FirstName,
				Last_name = orderAddress.LastName,
				Attention = orderAddress.Attention,
				Address2 = orderAddress.Line2,
				State_or_province = orderAddress.State
			};
		}

		protected virtual Order GetReepayOrder(Payment payment)
		{
			var order = payment?.PurchaseOrder;
			if (order == null) return new Order();

			var amount = order.OrderTotal ?? 0m;
			if (ShouldSubscriptionBeCreated(order))
			{
				amount -= GetSubscriptionAmount(order);
			}

			if (amount < 0m) amount = 0m;

			return new Order()
			{
				Handle = order.OrderGuid.ToString(),
				Amount = amount.ToCents(),
				Currency = order.BillingCurrency?.ISOCode ?? "DKK",
				Customer = GetReepayCustomer(payment),
				Billing_address = GetReepayAddress(order.BillingAddress),
				Shipping_address = GetReepayAddress(order.Shipments?.LastOrDefault()?.ShipmentAddress),
				Ordertext = _reepayPropertyNames.GetPropertyValueFromOrder(order, _reepayPropertyNames.OrderText)
			};
		}

		protected virtual decimal GetSubscriptionAmount(PurchaseOrder order)
		{
			if (order == null) return 0m;

			try
			{
				var subscriptionAmountStr = _reepayPropertyNames.GetPropertyValueFromOrder(order, _reepayPropertyNames.SubscriptionAmount);
				IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
				if (!decimal.TryParse(subscriptionAmountStr.Replace(",", "."), NumberStyles.AllowDecimalPoint, formatter, out var subscriptionAmount)) subscriptionAmount = 0m;

				return subscriptionAmount;
			}
			catch(Exception ex)
			{
				_logger.LogException(ex);
				return 0m;
			}
		}

		private SessionProperties EmptySessionProperties(PaymentRequest paymentRequest)
		{
			if (IsSubscriptionPayment(paymentRequest.PurchaseOrder) || IsCardChange(paymentRequest.PurchaseOrder))
			{
				return new RecurringSessionProperties();
			}

			return new ChargeSessionProperties();
		}	

		private SessionData GetSessionData(PaymentRequest paymentRequest)
		{
			if (IsSubscriptionPayment(paymentRequest.PurchaseOrder) 
				|| IsCardChange(paymentRequest.PurchaseOrder)
				|| ShouldSubscriptionBeCreated(paymentRequest.PurchaseOrder))
			{
				return new SessionData()
				{
					Data = GetRecurringSessionProperties(paymentRequest),
					Type = "recurring"
				};
			}

			return new SessionData()
			{
				Data = GetChargeSessionProperties(paymentRequest),
				Type = "charge"
			};
		}

		private void SetButtonTextForSubscription(PaymentRequest paymentRequest, RecurringSessionProperties recurringSessionProperties)
		{
			try
			{
				var order = paymentRequest?.PurchaseOrder;
				if (order != null && !string.IsNullOrWhiteSpace(paymentRequest?.PaymentMethod?.DynamicProperty<string>().Button_text_for_subscription))
				{
					recurringSessionProperties.Button_text = paymentRequest.PaymentMethod.DynamicProperty<string>().Button_text_for_subscription;
				}
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
			}
		}
	}
}
