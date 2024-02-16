using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class Phone
	{
		[JsonProperty("cc")]
		public string Cc { get; set; }

		[JsonProperty("subscriber")]
		public string Subscriber { get; set; }
	}
}
