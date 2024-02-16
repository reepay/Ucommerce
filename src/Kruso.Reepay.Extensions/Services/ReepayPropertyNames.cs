using Kruso.Reepay.Extensions.Services.Interfaces;
using Ucommerce.EntitiesV2;

namespace Kruso.Reepay.Extensions.Services
{
	class ReepayPropertyNames : IReepayPropertyNames
	{
		private string _isSubscriptionPayment = "reepay_isSubscriptionPayment";

		private string _shouldSubscriptionBeCreated = "reepay_shouldSubscriptionBeCreated";

		private string _cardOnFile = "reepay_cardOnFile";

		private string _customer = "reepay_customer";

		private string _subscriptionPlan = "reepay_plan";

		private string _discountName = "reepay_discount";

		private string _couponCode = "reepay_coupon";

		private string _subscriptionAmount = "reepay_subscriptionAmount";

		private string _subscriptionDiscountAmount = "reepay_subscriptionDiscountAmount";

		private string _subscriptionHandle = "reepay_subscriptionHandle";

		private string _subscriptionSettled = "reepay_isSubscriptionSettled";

		private string _isCardChange = "reepay_isCardChange";

		private string _currentSubscriptionHandle = "reepay_currentSubscriptionHandle";

		private string _orderText = "reepay_orderText";

		private string _shouldSubscriptionDiscountsRemainActive = "reepay_shouldSubscriptionDiscountsRemainActive";

		string IReepayPropertyNames.IsSubscriptionPayment { 
			get
			{ 
				return _isSubscriptionPayment;
			}
			set
			{ 
				_isSubscriptionPayment = value;
			}
		}

		string IReepayPropertyNames.ShouldSubscriptionBeCreated
		{
			get
			{
				return _shouldSubscriptionBeCreated;
			}
			set
			{
				_shouldSubscriptionBeCreated = value;
			}
		}

		string IReepayPropertyNames.CardOnFile
		{
			get
			{
				return _cardOnFile;
			}
			set
			{
				_cardOnFile = value;
			}
		}

		string IReepayPropertyNames.Customer
		{
			get
			{
				return _customer;
			}
			set
			{
				_customer = value;
			}
		}

		string IReepayPropertyNames.SubscriptionPlan
		{
			get
			{
				return _subscriptionPlan;
			}
			set
			{
				_subscriptionPlan = value;
			}
		}

		string IReepayPropertyNames.DiscountName
		{
			get
			{
				return _discountName;
			}
			set
			{
				_discountName = value;
			}
		}

		string IReepayPropertyNames.CouponCode
		{
			get
			{
				return _couponCode;
			}
			set
			{
				_couponCode = value;
			}
		}

		string IReepayPropertyNames.SubscriptionAmount
		{
			get
			{
				return _subscriptionAmount;
			}
			set
			{
				_subscriptionAmount = value;
			}
		}

		string IReepayPropertyNames.SubscriptionDiscountAmount
		{
			get
			{
				return _subscriptionDiscountAmount;
			}
			set
			{
				_subscriptionDiscountAmount = value;
			}
		}

		string IReepayPropertyNames.SubscriptionHandle
		{
			get
			{
				return _subscriptionHandle;
			}
			set
			{
				_subscriptionHandle = value;
			}
		}

		string IReepayPropertyNames.IsSubscriptionSettled
		{
			get
			{
				return _subscriptionSettled;
			}
			set
			{
				_subscriptionSettled = value;
			}
		}

		string IReepayPropertyNames.IsCardChange
		{
			get
			{
				return _isCardChange;
			}
			set
			{
				_isCardChange = value;
			}
		}

		string IReepayPropertyNames.CurrentSubscriptionHandle
		{
			get
			{
				return _currentSubscriptionHandle;
			}
			set
			{
				_currentSubscriptionHandle = value;
			}
		}

		string IReepayPropertyNames.OrderText
		{
			get
			{
				return _orderText;
			}
			set
			{
				_orderText = value;
			}
		}

		string IReepayPropertyNames.ShouldSubscriptionDiscountsRemainActive
		{
			get
			{
				return _shouldSubscriptionDiscountsRemainActive;
			}
			set
			{
				_shouldSubscriptionDiscountsRemainActive = value;
			}
		}

		public virtual string GetPropertyValueFromOrder(PurchaseOrder purchaseOrder, string propertyName)
		{
			if (purchaseOrder == null) return string.Empty;
			return purchaseOrder[propertyName];
		}

		public virtual bool GetBoolPropertyValueFromOrder(PurchaseOrder purchaseOrder, string propertyName)
		{
			if (purchaseOrder == null) return false;

			if (bool.TryParse(purchaseOrder[propertyName], out bool propertyValue))
			{
				return propertyValue;
			}

			return false;
		}
	}
}
