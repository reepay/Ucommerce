using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class SubscriptionObject : SubscriptionBase
	{
		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("expires")]
		public string Expires { get; set; }

		[JsonProperty("reactivated")]
		public string Reactivated { get; set; }

		[JsonProperty("timezone")]
		public string Timezone { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("activated")]
		public string Activated { get; set; }

		[JsonProperty("renewing")]
		public bool Renewing { get; set; }

		[JsonProperty("current_period_start")]
		public string Current_period_start { get; set; }

		[JsonProperty("next_period_start")]
		public string Next_period_start { get; set; }

		[JsonProperty("first_period_start")]
		public string First_period_start { get; set; }

		[JsonProperty("last_period_start")]
		public string Last_period_start { get; set; }

		[JsonProperty("trial_start")]
		public string Trial_start { get; set; }

		[JsonProperty("trial_end")]
		public string Trial_end { get; set; }

		[JsonProperty("is_cancelled")]
		public bool Is_cancelled { get; set; }

		[JsonProperty("in_trial")]
		public bool In_trial { get; set; }

		[JsonProperty("has_started")]
		public bool Has_started { get; set; }

		[JsonProperty("renewal_count")]
		public int Renewal_count { get; set; }

		[JsonProperty("cancelled_date")]
		public string Cancelled_date { get; set; }

		[JsonProperty("expired_date")]
		public string Expired_date { get; set; }

		[JsonProperty("expire_reason")]
		public string Expire_reason { get; set; }

		[JsonProperty("on_hold_date")]
		public string On_hold_date { get; set; }

		[JsonProperty("on_hold_reason")]
		public string On_hold_reason { get; set; }

		[JsonProperty("payment_method_added")]
		public bool Payment_method_added { get; set; }

		[JsonProperty("scheduled_plan_change")]
		public string Scheduled_plan_change { get; set; }

		[JsonProperty("reminder_email_sent")]
		public string Reminder_email_sent { get; set; }

		[JsonProperty("failed_invoices")]
		public int Failed_invoices { get; set; }

		[JsonProperty("failed_amount")]
		public int Failed_amount { get; set; }

		[JsonProperty("cancelled_invoices")]
		public int Cancelled_invoices { get; set; }

		[JsonProperty("cancelled_amount")]
		public int Cancelled_amount { get; set; }

		[JsonProperty("pending_invoices")]
		public int Pending_invoices { get; set; }

		[JsonProperty("pending_amount")]
		public int Pending_amount { get; set; }

		[JsonProperty("dunning_invoices")]
		public int Dunning_invoices { get; set; }

		[JsonProperty("dunning_amount")]
		public int Dunning_amount { get; set; }

		[JsonProperty("settled_invoices")]
		public int Settled_invoices { get; set; }

		[JsonProperty("settled_amount")]
		public int Settled_amount { get; set; }

		[JsonProperty("refunded_amount")]
		public int Refunded_amount { get; set; }

		[JsonProperty("pending_additional_costs")]
		public int Pending_additional_costs { get; set; }

		[JsonProperty("pending_additional_cost_amount")]
		public int Pending_additional_cost_amount { get; set; }

		[JsonProperty("transferred_additional_costs")]
		public int Transferred_additional_costs { get; set; }

		[JsonProperty("transferred_additional_cost_amount")]
		public int Transferred_additional_cost_amount { get; set; }

		[JsonProperty("pending_credits")]
		public int Pending_credits { get; set; }

		[JsonProperty("pending_credit_amount")]
		public int Pending_credit_amount { get; set; }

		[JsonProperty("transferred_credits")]
		public int Transferred_credits { get; set; }

		[JsonProperty("transferred_credit_amount")]
		public int Transferred_credit_amount { get; set; }

		[JsonProperty("hosted_page_links")]
		public dynamic Hosted_page_links { get; set; }

		[JsonProperty("pending_change")]
		public SubscriptionChange Pending_change { get; set; }

		[JsonProperty("subscription_changes")]
		public IList<SubscriptionChange> Subscription_changes { get; set; }

		[JsonProperty("subscription_add_ons")]
		public IList<dynamic> Subscription_add_ons { get; set; }

		[JsonProperty("subscription_discounts")]
		public IList<string> Subscription_discounts { get; set; }
	}
}
