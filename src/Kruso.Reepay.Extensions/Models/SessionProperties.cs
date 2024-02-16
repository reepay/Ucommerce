using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class SessionProperties
	{
		[JsonProperty("configuration")]
		public string Configuration { get; set; }

		[JsonProperty("locale")]
		public string Locale { get; set; }

		[JsonProperty("ttl")]
		public string Ttl { get; set; }

		[JsonProperty("phone")]
		public string Phone { get; set; }

		[JsonProperty("accept_url")]
		public string Accept_url { get; set; }

		[JsonProperty("cancel_url")]
		public string Cancel_url { get; set; }

		[JsonProperty("payment_methods")]
		public IList<string> Payment_methods { get; set; }

		[JsonProperty("card_on_file")]
		public string Card_on_file { get; set; }

		[JsonProperty("button_text")]
		public string Button_text { get; set; }

		[JsonProperty("recurring_average_amount")]
		public long Recurring_average_amount { get; set; }

		[JsonProperty("sca_data")]
		public ScaData Sca_data { get; set; }

		public SessionProperties()
		{

		}
	}
}
