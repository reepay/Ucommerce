using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class AddCardPaymentMethodRequest
	{
		[JsonProperty("reference")]
		public string Reference { get; set; }

		[JsonProperty("card_token")]
		public string Card_token { get; set; }
	}
}
