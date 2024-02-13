using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kruso.Reepay.Extensions.Models;
using Ucommerce.EntitiesV2;
using Ucommerce.Transactions.Payments;

namespace Kruso.Reepay.Extensions.Services.Interfaces
{
	public interface IReepayRepository
	{
		Task<string> GetSessionUrl(PaymentRequest paymentRequest);
		Task<bool?> PaymentIsValid(PaymentMethod paymentMethod, string handle, bool isSettle = false, ChargeObject chargeObject = null);
		Task<bool?> PaymentIsCancelled(PaymentMethod paymentMethod, string handle);
		Task<bool?> RefundIsSuccessful(PaymentMethod paymentMethod, string id);
		Uri GetAcceptUrl(PaymentMethod paymentMethod, PurchaseOrder order = null);
		Uri GetErrorUrl(PaymentMethod paymentMethod);
		Uri GetCancelUrl(PaymentMethod paymentMethod, PurchaseOrder order = null);
		Task<RefundObject> CreateRefund(Payment payment);
		Task<ChargeObject> SettleCharge(Payment payment);
		Task<ChargeObject> CancelCharge(Payment payment);
		Task<ChargeObject> CreateCharge(Payment payment, string source);
		Task<CustomerObject> CreateCustomer(PaymentMethod paymentMethod, string customerHandle, string email, string address, string address2, string city, string country, string phone, string company, string vat, string first_name, string last_name, string postal_code);
		Task UpdateCustomer(PaymentMethod paymentMethod, string customerHandle,	string email, string address, string address2, string city,	string country, string phone, string company, string vat, string first_name, string last_name, string postal_code);
		Task<CardObject> AddCardToCustomer(PaymentMethod paymentMethod, string customerHandle, string cardToken);
		Task<IList<WebhookObject>> GetWebhooks(PaymentMethod paymentMethod, string id);
		Task<InvoiceObject> GetInvoice(PaymentMethod paymentMethod, string handle);
		Task<CustomerObject> GetCustomer(PaymentMethod paymentMethod, string handle);
		string SubscriptionHandlePropertyName();
		string SubscriptionSettledPropertyName();
		string CardOnFilePropertyName();
		string CardOnFilePropertyValue(PurchaseOrder purchaseOrder);
		bool IsCardChange(PurchaseOrder purchaseOrder);

		/// <summary>
		/// Returns true if only subscription (with no other products) should be bought
		/// </summary>
		/// <param name="purchaseOrder"></param>
		/// <returns></returns>
		bool IsSubscriptionPayment(PurchaseOrder purchaseOrder);

		/// <summary>
		/// Returns true if subscription should be created (not depending on the existance of other products in the purchaseOrder)
		/// </summary>
		/// <param name="purchaseOrder"></param>
		/// <returns></returns>
		bool ShouldSubscriptionBeCreated(PurchaseOrder purchaseOrder);
	}
}
