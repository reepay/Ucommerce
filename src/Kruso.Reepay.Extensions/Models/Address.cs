using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class Address : CustomerAddress
	{
		[JsonProperty("attention")]
		public string Attention { get; set; }

		[JsonProperty("state_or_province")]
		public string State_or_province { get; set; }
	}
}
