using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class SubscriptionBase
	{
		[JsonProperty("handle")]
		public string Handle { get; set; }

		[JsonProperty("customer")]
		public string Customer { get; set; }

		[JsonProperty("plan")]
		public string Plan { get; set; }

		[JsonProperty("test")]
		public bool Test { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }
		
		[JsonProperty("quantity")]
		public int Quantity { get; set; }

		[JsonProperty("plan_version")]
		public int Plan_version { get; set; }

		[JsonProperty("amount_incl_vat")]
		public bool Amount_incl_vat { get; set; }

		[JsonProperty("start_date")]
		public string Start_date { get; set; }

		[JsonProperty("end_date")]
		public string End_date { get; set; }

		[JsonProperty("grace_duration")]
		public int Grace_duration { get; set; }
	}
}
