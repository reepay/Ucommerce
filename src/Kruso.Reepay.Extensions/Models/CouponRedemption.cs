using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CouponRedemption
	{
		[JsonProperty("coupon")]
		public Coupon Coupon { get; set; }

		[JsonProperty("subscription_discount")]
		public SubscriptionDiscount Subscription_discount { get; set; }
	}
}
