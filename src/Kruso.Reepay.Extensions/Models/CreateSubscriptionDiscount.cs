using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CreateSubscriptionDiscount
	{
		[JsonProperty("handle")]
		public string Handle { get; set; }

		[JsonProperty("discount")]
		public string Discount { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("percentage")]
		public int Percentage { get; set; }

		[JsonProperty("apply_to")]
		public IList<string> Apply_to { get; set; }

		[JsonProperty("fixed_count")]
		public int Fixed_count { get; set; }

		[JsonProperty("fixed_period_unit")]
		public string Fixed_period_unit { get; set; }

		[JsonProperty("fixed_period")]
		public int Fixed_period { get; set; }

		public CreateSubscriptionDiscount() { }

		public CreateSubscriptionDiscount(SubscriptionDiscount subscriptionDiscount) : this()
		{
			if (subscriptionDiscount == null) { return; }
			Discount = subscriptionDiscount.Discount;
		}
	}
}
