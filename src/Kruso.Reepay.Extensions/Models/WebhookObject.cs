using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class WebhookObject
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("event")]
		public string Event { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("password")]
		public string Password { get; set; }

		[JsonProperty("content")]
		public string Content { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("success")]
		public string Success { get; set; }

		[JsonProperty("count")]
		public int Count { get; set; }

		[JsonProperty("last_fail")]
		public string Last_fail { get; set; }

		[JsonProperty("first_fail")]
		public string First_fail { get; set; }

		[JsonProperty("alert_count")]
		public int Alert_count { get; set; }

		[JsonProperty("alert_sent")]
		public string Alert_sent { get; set; }
	}
}