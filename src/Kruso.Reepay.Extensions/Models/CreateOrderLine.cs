using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CreateOrderLine
	{
		[JsonProperty("ordertext")]
		public string Ordertext { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("vat")]
		public double Vat { get; set; }

		[JsonProperty("quantity")]
		public int Quantity { get; set; }

		[JsonProperty("amount_incl_vat")]
		public bool Amount_incl_vat { get; set; }
	}
}
