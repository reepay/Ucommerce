using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class AccountInfo
	{
		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("changed")]
		public string Changed { get; set; }

		[JsonProperty("age_indicator")]
		public string Age_indicator { get; set; }

		[JsonProperty("change_indicator")]
		public string Change_indicator { get; set; }

		[JsonProperty("password_change")]
		public string Password_change { get; set; }

		[JsonProperty("password_change_indicator")]
		public string Password_change_indicator { get; set; }

		[JsonProperty("purchase_count")]
		public int Purchase_count { get; set; }

		[JsonProperty("add_card_attempts")]
		public int Add_card_attempts { get; set; }

		[JsonProperty("transactions_day")]
		public int Transactions_day { get; set; }

		[JsonProperty("transactions_year")]
		public int Transactions_year { get; set; }

		[JsonProperty("card_age")]
		public string Card_age { get; set; }

		[JsonProperty("card_age_indicator")]
		public string Card_age_indicator { get; set; }

		[JsonProperty("shipping_address_usage")]
		public string Shipping_address_usage { get; set; }

		[JsonProperty("shipping_address_usage_indicator")]
		public string Shipping_address_usage_indicator { get; set; }

		[JsonProperty("shipping_name_indicator")]
		public bool Shipping_name_indicator { get; set; }

		[JsonProperty("suspicious_activity")]
		public bool Suspicious_activity { get; set; }
	}
}
