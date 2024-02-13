using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CardObject
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("customer")]
		public string Customer { get; set; }

		[JsonProperty("reference")]
		public string Reference { get; set; }

		[JsonProperty("failed")]
		public string Failed { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("fingerprint")]
		public string Fingerprint { get; set; }

		[JsonProperty("reactivated")]
		public string Reactivated { get; set; }

		[JsonProperty("gw_ref")]
		public string Gw_ref { get; set; }

		[JsonProperty("card_type")]
		public string Card_type { get; set; }

		[JsonProperty("exp_date")]
		public string Exp_date { get; set; }

		[JsonProperty("masked_card")]
		public string Masked_card { get; set; }

		[JsonProperty("last_success")]
		public string Last_success { get; set; }

		[JsonProperty("last_failed")]
		public string Last_failed { get; set; }

		[JsonProperty("first_fail")]
		public string First_fail { get; set; }

		[JsonProperty("error_code")]
		public string Error_code { get; set; }

		[JsonProperty("error_state")]
		public string Error_state { get; set; }

		[JsonProperty("strong_authentication_status")]
		public string Strong_authentication_status { get; set; }

		[JsonProperty("three_d_secure_status")]
		public string Three_d_secure_status { get; set; }

		[JsonProperty("risk_rule")]
		public string Risk_rule { get; set; }

		[JsonProperty("card_country")]
		public string Card_country { get; set; }
	}
}
