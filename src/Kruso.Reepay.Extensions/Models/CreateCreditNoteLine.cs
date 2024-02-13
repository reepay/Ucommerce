using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CreateCreditNoteLine
	{
		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("quantity")]
		public int Quantity { get; set; }

		[JsonProperty("order_line_id")]
		public string Order_line_id { get; set; }
	}
}
