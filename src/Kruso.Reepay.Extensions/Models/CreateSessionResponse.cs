using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CreateSessionResponse
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
