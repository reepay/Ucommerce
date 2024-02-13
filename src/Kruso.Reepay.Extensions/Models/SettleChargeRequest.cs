using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class SettleChargeRequest
	{
		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("ordertext")]
		public string Ordertext { get; set; }

		[JsonProperty("order_lines")]
		public IList<CreateOrderLine> Order_lines { get; set; }
	}
}
