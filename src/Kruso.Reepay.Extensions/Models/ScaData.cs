using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class ScaData
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("home_phone")]
		public Phone Home_phone { get; set; }

		[JsonProperty("mobile_phone")]
		public Phone Mobile_phone { get; set; }

		[JsonProperty("work_phone")]
		public Phone Work_phone { get; set; }

		[JsonProperty("billing_address")]
		public ScaAddress Billing_address { get; set; }

		[JsonProperty("shipping_address")]
		public ScaAddress Shipping_address { get; set; }

		[JsonProperty("address_match")]
		public bool Address_match { get; set; }

		[JsonProperty("account_id")]
		public string Account_id { get; set; }

		[JsonProperty("challenge_indicator")]
		public string Challenge_indicator { get; set; }

		[JsonProperty("risk_indicator")]
		public RiskIndicator Risk_indicator { get; set; }

		[JsonProperty("account_info")]
		public AccountInfo Account_info { get; set; }
	}
}
