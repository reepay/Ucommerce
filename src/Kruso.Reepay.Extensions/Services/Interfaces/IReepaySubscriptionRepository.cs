using System.Collections.Generic;
using System.Threading.Tasks;
using Kruso.Reepay.Extensions.Models;
using Ucommerce.EntitiesV2;

namespace Kruso.Reepay.Extensions.Services.Interfaces
{
	public interface IReepaySubscriptionRepository
	{
		Task<SubscriptionObject> GetSubscription(PaymentMethod paymentMethod, string subscriptionHandle);
		Task<IList<SubscriptionDiscount>> GetSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle);
		Task<SubscriptionObject> CreateSubscription(Payment payment, string customerHandle, string source, string subscriptionHandle = "", bool useDiscounts = true);
		Task<SubscriptionObject> CreateSubscription(PaymentMethod paymentMethod,
			string customerHandle,
			string plan,
			string source,
			bool useDiscounts = true,
			string subscriptionHandle = "",
			string trialPeriod = "",
			string startDate = "",
			string couponCode = "",
			string discountName = "",
			decimal discountAmount = 0m);
		Task UpdateCreditCardInfo(Payment payment, string source);
		Task UpdateCreditCardInfo(PaymentMethod paymentMethod, string source, string subscriptionHandle);
		Task<SubscriptionObject> ChangeSubscription(PaymentMethod paymentMethod, string subscriptionHandle, string plan);
		Task<bool> AddSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle, IList<string> couponCodes = null, IList<CreateSubscriptionDiscount> subscriptionDiscounts = null);
		Task<SubscriptionObject> CancelSubscription(PaymentMethod paymentMethod, string subscriptionHandle, CancelSubscriptionRequest requestData = null);
		Task<SubscriptionObject> UnCancelSubscription(PaymentMethod paymentMethod, string subscriptionHandle);
		Task<string> GenerateSubscriptionDiscountHandle(PaymentMethod paymentMethod, string subscriptionHandle, bool subscriptionExists = true);
		Task DeleteSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle);
		Task DeleteUsedDiscounts(Payment payment, string subscriptionHandle);
		Task DeleteUsedDiscounts(PaymentMethod paymentMethod, string subscriptionHandle);
		Task<bool?> SubscriptionIsSettledSuccessfully(PaymentMethod paymentMethod, string handle);
	}
}
