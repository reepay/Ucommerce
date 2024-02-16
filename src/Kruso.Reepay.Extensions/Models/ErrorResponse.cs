using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class ErrorResponse
	{
		[JsonProperty("code")]
		public int Code { get; set; }

		[JsonProperty("error")]
		public string Error { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("temestamp")]
		public string TimeStamp { get; set; }

		[JsonProperty("http_status")]
		public int Http_status { get; set; }

		[JsonProperty("http_reason")]
		public string Http_reason { get; set; }

		[JsonProperty("request_id")]
		public string Request_id { get; set; }

		[JsonProperty("transaction_error")]
		public string Transaction_error { get; set; }
	}
}
