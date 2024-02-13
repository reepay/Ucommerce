using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class SubscriptionDiscount
	{
		[JsonProperty("handle")]
		public string Handle { get; set; }

		[JsonProperty("discount")]
		public string Discount { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("coupon")]
		public string Coupon { get; set; }

		[JsonProperty("deleted")]
		public string Deleted { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("percentage")]
		public int Percentage { get; set; }

		[JsonProperty("count")]
		public int Count { get; set; }

		[JsonProperty("invoices")]
		public IList<dynamic> Invoices { get; set; }

		[JsonProperty("apply_to")]
		public IList<dynamic> Apply_to { get; set; }

		[JsonProperty("fixed_count")]
		public int Fixed_count { get; set; }

		[JsonProperty("fixed_period_unit")]
		public int Fixed_period_unit { get; set; }

		[JsonProperty("fixed_period")]
		public int Fixed_period { get; set; }

		[JsonProperty("fixed_usage_reached")]
		public bool Fixed_usage_reached { get; set; }

		[JsonProperty("fixed_period_passed")]
		public bool Fixed_period_passed { get; set; }
	}
}
