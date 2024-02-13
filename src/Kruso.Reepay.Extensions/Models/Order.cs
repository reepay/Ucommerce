using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class Order
	{
		[JsonProperty("handle")]
		public string Handle { get; set; }

		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("currency")]
		public string Currency { get; set; }

		[JsonProperty("customer")]
		public CreateCustomer Customer { get; set; }

		[JsonProperty("metadata")]
		public dynamic Metadata { get; set; }

		[JsonProperty("ordertext")]
		public string Ordertext { get; set; }

		[JsonProperty("order_lines")]
		public IList<dynamic> Order_lines { get; set; }

		[JsonProperty("customer_handle")]
		public string Customer_handle { get; set; }

		[JsonProperty("billing_address")]
		public Address Billing_address { get; set; }

		[JsonProperty("shipping_address")]
		public Address Shipping_address { get; set; }
	}
}
