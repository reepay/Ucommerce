using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class WebhookContent
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("event_id")]
		public string Event_id { get; set; }

		[JsonProperty("event_type")]
		public string Event_type { get; set; }

		[JsonProperty("timestamp")]
		public string Timestamp { get; set; }

		[JsonProperty("signature")]
		public string Signature { get; set; }

		[JsonProperty("customer")]
		public string Customer { get; set; }

		[JsonProperty("payment_method")]
		public string Payment_method { get; set; }

		[JsonProperty("payment_method_reference")]
		public string Payment_method_reference { get; set; }

		[JsonProperty("subscription")]
		public string Subscription { get; set; }

		[JsonProperty("invoice")]
		public string Invoice { get; set; }

		[JsonProperty("transaction")]
		public string Transaction { get; set; }

		[JsonProperty("credit_note")]
		public string Credit_note { get; set; }

		[JsonProperty("credit")]
		public string Credit { get; set; }
	}
}