using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CustomerAddress : AddressBase
	{
		[JsonProperty("company")]
		public string Company { get; set; }

		[JsonProperty("vat")]
		public string Vat { get; set; }

		[JsonProperty("address")]
		public string Address { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("phone")]
		public string Phone { get; set; }

		[JsonProperty("first_name")]
		public string First_name { get; set; }

		[JsonProperty("last_name")]
		public string Last_name { get; set; }
	}
}
