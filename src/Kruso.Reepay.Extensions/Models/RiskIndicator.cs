using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class RiskIndicator
	{
		[JsonProperty("delivery_email")]
		public string Delivery_email { get; set; }

		[JsonProperty("delivery_timeframe")]
		public string Delivery_timeframe { get; set; }

		[JsonProperty("gift_card_amount")]
		public int Gift_card_amount { get; set; }

		[JsonProperty("gift_card_count")]
		public int Gift_card_count { get; set; }

		[JsonProperty("gift_card_currency")]
		public string Gift_card_currency { get; set; }

		[JsonProperty("pre_order_date")]
		public string Pre_order_date { get; set; }

		[JsonProperty("pre_order_purchase_indicator")]
		public string Pre_order_purchase_indicator { get; set; }

		[JsonProperty("reorder_items_indicator")]
		public string Reorder_items_indicator { get; set; }

		[JsonProperty("shipping_indicator")]
		public string Shipping_indicator { get; set; }
	}
}
