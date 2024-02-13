using System.Collections.Generic;
using Kruso.Reepay.Extensions.Enums;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class ChangeSubscriptionRequest
	{
		[JsonProperty("timing")]
		public string Timing { get; set; }

		[JsonProperty("plan")]
		public string Plan { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("quantity")]
		public int Quantity { get; set; }

		[JsonProperty("billing")]
		public string Billing { get; set; }

		[JsonProperty("amount_incl_vat")]
		public bool Amount_incl_vat { get; set; }

		[JsonProperty("compensation_method")]
		public string Compensation_method { get; set; }

		[JsonProperty("partial_period_handling")]
		public string Partial_period_handling { get; set; }

		[JsonProperty("start_date")]
		public string Start_date { get; set; }

		[JsonProperty("force_new_period")]
		public bool Force_new_period { get; set; }

		[JsonProperty("cancel_change")]
		public bool Cancel_change { get; set; }

		[JsonProperty("add_ons")]
		public IList<dynamic> Add_ons { get; set; }

		[JsonProperty("remove_add_ons")]
		public IList<dynamic> Remove_add_ons { get; set; }

		public ChangeSubscriptionRequest()
		{
			Timing = ChangeSubscriptionTimingEnum.renewal.ToString();
		}
	}
}
