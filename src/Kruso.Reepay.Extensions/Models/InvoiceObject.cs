using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class InvoiceObject
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("handle")]
		public string Handle { get; set; }

		[JsonProperty("customer")]
		public string Customer { get; set; }

		[JsonProperty("subscription")]
		public string Subscription { get; set; }

		[JsonProperty("plan")]
		public string Plan { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("processing")]
		public bool Processing { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("number")]
		public int Number { get; set; }

		[JsonProperty("currency")]
		public string Currency { get; set; }

		[JsonProperty("due")]
		public string Due { get; set; }

		[JsonProperty("failed")]
		public string Failed { get; set; }

		[JsonProperty("settled")]
		public string Settled { get; set; }

		[JsonProperty("cancelled")]
		public string Cancelled { get; set; }

		[JsonProperty("authorized")]
		public string Authorized { get; set; }

		[JsonProperty("credits")]
		public IList<dynamic> Credits { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("plan_version")]
		public int Plan_version { get; set; }

		[JsonProperty("dunning_plan")]
		public string Dunning_plan { get; set; }

		[JsonProperty("discount_amount")]
		public int Discount_amount { get; set; }

		[JsonProperty("org_amount")]
		public int Org_amount { get; set; }

		[JsonProperty("amount_vat")]
		public int Amount_vat { get; set; }

		[JsonProperty("amount_ex_vat")]
		public int Amount_ex_vat { get; set; }

		[JsonProperty("settled_amount")]
		public int Settled_amount { get; set; }

		[JsonProperty("refunded_amount")]
		public int Refunded_amount { get; set; }

		[JsonProperty("authorized_amount")]
		public int Authorized_amount { get; set; }

		[JsonProperty("credited_amount")]
		public int Credited_amount { get; set; }

		[JsonProperty("period_number")]
		public int Period_number { get; set; }

		[JsonProperty("order_lines")]
		public List<dynamic> Order_lines { get; set; }

		[JsonProperty("additional_costs")]
		public List<dynamic> Additional_costs { get; set; }

		[JsonProperty("transactions")]
		public List<dynamic> Transactions { get; set; }

		[JsonProperty("credit_notes")]
		public List<dynamic> Credit_notes { get; set; }

		[JsonProperty("dunning_start")]
		public string Dunning_start { get; set; }

		[JsonProperty("dunning_count")]
		public int Dunning_count { get; set; }

		[JsonProperty("dunning_expired")]
		public string Dunning_expired { get; set; }

		[JsonProperty("period_from")]
		public string Period_from { get; set; }

		[JsonProperty("period_to")]
		public string Period_to { get; set; }

		[JsonProperty("settle_later")]
		public bool Settle_later { get; set; }

		[JsonProperty("settle_later_payment_method")]
		public string Settle_later_payment_method { get; set; }

		[JsonProperty("billing_address")]
		public Address Billing_address { get; set; }

		[JsonProperty("shipping_address")]
		public Address Shipping_address { get; set; }
	}
}
