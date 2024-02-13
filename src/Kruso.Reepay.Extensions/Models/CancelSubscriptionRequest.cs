using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CancelSubscriptionRequest
	{
		[JsonProperty("notice_periods")]
		public int Notice_periods { get; set; }

		[JsonProperty("notice_periods_after_current")]
		public bool Notice_periods_after_current { get; set; }

		[JsonProperty("expire_at")]
		public string Expire_at { get; set; }

		[JsonProperty("trial_with_notice_and_fixation")]
		public bool Trial_with_notice_and_fixation { get; set; }
	}
}
