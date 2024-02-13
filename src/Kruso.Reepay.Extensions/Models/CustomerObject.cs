using Newtonsoft.Json;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;

namespace Kruso.Reepay.Extensions.Models
{
	public class CustomerObject : CustomerAddress
	{
		[JsonProperty("handle")]
		public string Handle { get; set; }

		[JsonProperty("test")]
		public bool Test { get; set; }

		[JsonProperty("subscriptions")]
		public int Subscriptions { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("deleted")]
		public string Deleted { get; set; }

		[JsonProperty("active_subscriptions")]
		public int Active_subscriptions { get; set; }

		[JsonProperty("trial_active_subscriptions")]
		public int Trial_active_subscriptions { get; set; }

		[JsonProperty("trial_cancelled_subscriptions")]
		public int Trial_cancelled_subscriptions { get; set; }

		[JsonProperty("expired_subscriptions")]
		public int Expired_subscriptions { get; set; }

		[JsonProperty("on_hold_subscriptions")]
		public int On_hold_subscriptions { get; set; }

		[JsonProperty("cancelled_subscriptions")]
		public int Cancelled_subscriptions { get; set; }

		[JsonProperty("non_renewing_subscriptions")]
		public int Non_renewing_subscriptions { get; set; }

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
	}
}
