using Ucommerce.EntitiesV2;

namespace Kruso.Reepay.Extensions.Services.Interfaces
{
	public interface IReepayPropertyNames
	{
		string IsSubscriptionPayment { get; set; }
		string ShouldSubscriptionBeCreated { get; set; }
		string CardOnFile { get; set; }
		string Customer { get; set; }
		string SubscriptionPlan { get; set; }
		string DiscountName { get; set; }
		string CouponCode { get; set; }
		string SubscriptionAmount { get; set; }
		string SubscriptionDiscountAmount { get; set; }
		string SubscriptionHandle { get; set; }
		string IsSubscriptionSettled { get; set; }
		string IsCardChange { get; set; }
		string CurrentSubscriptionHandle { get; set; }
		string OrderText { get; set; }
		string ShouldSubscriptionDiscountsRemainActive { get; set; }
		string GetPropertyValueFromOrder(PurchaseOrder purchaseOrder, string propertyName);
		bool GetBoolPropertyValueFromOrder(PurchaseOrder purchaseOrder, string propertyName);
	}
}
