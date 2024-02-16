using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class AddressBase
	{
		[JsonProperty("address2")]
		public string Address2 { get; set; }

		[JsonProperty("city")]
		public string City { get; set; }

		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("postal_code")]
		public string Postal_code { get; set; }
	}
}
