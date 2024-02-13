using System.Collections.Generic;
using System.Threading.Tasks;
using Kruso.Reepay.Extensions.Models;
using Ucommerce.EntitiesV2;

namespace Kruso.Reepay.Extensions.Services.Interfaces
{
	public interface IReepayGateway
	{
		Task<string> CreateSessionUrl(PaymentMethod paymentMethod, SessionData sessionData);
		Task<ChargeObject> GetCharge(PaymentMethod paymentMethod, string handle);
		Task<InvoiceObject> GetInvoice(PaymentMethod paymentMethod, string handle);
		Task<ChargeObject> CreateCharge(PaymentMethod paymentMethod, CreateChargeRequest requestData);
		Task<CustomerObject> CreateCustomer(PaymentMethod paymentMethod, CreateCustomer requestData);
		Task<CustomerObject> GetCustomer(PaymentMethod paymentMethod, string handle);
		Task<SubscriptionObject> CreateSubscription(PaymentMethod paymentMethod, CreateSubscriptionRequest requestData);
		Task<SubscriptionObject> GetSubscription(PaymentMethod paymentMethod, string handle);
		Task<IList<SubscriptionDiscount>> GetSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle);
		Task<RefundObject> CreateRefund(PaymentMethod paymentMethod, CreateRefundRequest requestData);
		Task<RefundObject> GetRefund(PaymentMethod paymentMethod, string id);
		Task<ChargeObject> SettleCharge(PaymentMethod paymentMethod, string chargeHandle, SettleChargeRequest requestData);
		Task<ChargeObject> CancelCharge(PaymentMethod paymentMethod, string chargeHandle);
		Task<SubscriptionObject> ChangeSubscription(PaymentMethod paymentMethod, string subscriptionHandle, ChangeSubscriptionRequest requestData);
		Task UpdateCustomer(PaymentMethod paymentMethod, string customerHandle, CustomerAddress requestData);
		Task<CardObject> AddCardToCustomer(PaymentMethod paymentMethod, string customerHandle, AddCardPaymentMethodRequest requestData);
		Task<SubscriptionObject> CancelSubscription(PaymentMethod paymentMethod, string subscriptionHandle, CancelSubscriptionRequest requestData);
		Task<SubscriptionObject> UnCancelSubscription(PaymentMethod paymentMethod, string subscriptionHandle);
		Task RemoveAllSubscriptionCards(PaymentMethod paymentMethod, string subscriptionHandle);
		Task SetSubscriptionCard(PaymentMethod paymentMethod, string subscriptionHandle, SetPaymentMethodRequest requestData);
		Task DeleteSubscriptionDiscount(PaymentMethod paymentMethod, string subscriptionHandle, string subscriptionDiscountHandle);
		Task DeleteAllSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle);
		Task AddSubscriptionDiscount(PaymentMethod paymentMethod, string subscriptionHandle, CreateSubscriptionDiscount discountData);
		Task AddSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle, IList<CreateSubscriptionDiscount> subscriptionDiscounts);
		Task RedeemCouponCode(PaymentMethod paymentMethod, string subscriptionHandle, RedeemCouponCodeRequest requestData);
		Task RedeemCouponCodes(PaymentMethod paymentMethod, string subscriptionHandle, IList<string> couponCodes);
		Task<IList<WebhookObject>> GetWebhooks(PaymentMethod paymentMethod, string id);
	}
}
