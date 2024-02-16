using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class RefundObject
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("invoice")]
		public string Invoice { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("currency")]
		public string Currency { get; set; }

		[JsonProperty("transaction")]
		public string Transaction { get; set; }

		[JsonProperty("error")]
		public string Error { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("credit_note_id")]
		public string Credit_note_id { get; set; }

		[JsonProperty("ref_transaction")]
		public string Ref_transaction { get; set; }

		[JsonProperty("error_state")]
		public string Error_state { get; set; }

		[JsonProperty("acquirer_message")]
		public string Acquirer_message { get; set; }
	}
}
