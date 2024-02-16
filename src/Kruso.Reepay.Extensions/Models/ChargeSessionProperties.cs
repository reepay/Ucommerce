using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class ChargeSessionProperties : SessionProperties
	{		
		[JsonProperty("invoice")]
		public string Invoice { get; set; }

		[JsonProperty("settle")]
		public bool Settle { get; set; }

		[JsonProperty("order")]
		public Order Order { get; set; }

		[JsonProperty("recurring")]
		public bool Recurring { get; set; }

		[JsonProperty("text_on_statement")]
		public string Text_on_statement { get; set; }

		[JsonProperty("recurring_optional")]
		public bool Recurring_optional { get; set; }

		[JsonProperty("recurring_optional_text")]
		public string Recurring_optional_text { get; set; }

		public ChargeSessionProperties() : base()
        {

        }
	}
}
