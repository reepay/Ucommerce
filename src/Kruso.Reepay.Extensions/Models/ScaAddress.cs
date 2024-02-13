using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class ScaAddress : AddressBase
	{
		[JsonProperty("address1")]
		public string Address1 { get; set; }

		[JsonProperty("address3")]
		public string Address3 { get; set; }

		[JsonProperty("state_or_province")]
		public string State_or_province { get; set; }
	}
}
