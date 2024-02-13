using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class SetPaymentMethodRequest
	{
		[JsonProperty("source")]
		public string Source { get; set; }
	}
}
