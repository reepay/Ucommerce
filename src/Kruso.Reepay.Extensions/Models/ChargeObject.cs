using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class ChargeObject
	{
		[JsonProperty("handle")]
		public string Handle { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("customer")]
		public string Customer { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }

		[JsonProperty("currency")]
		public string Currency { get; set; }

		[JsonProperty("authorized")]
		public string Authorized { get; set; }

		[JsonProperty("settled")]
		public string Settled { get; set; }

		[JsonProperty("cancelled")]
		public string Cancelled { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("transaction")]
		public string Transaction { get; set; }

		[JsonProperty("error")]
		public string Error { get; set; }

		[JsonProperty("processing")]
		public bool Processing { get; set; }

		[JsonProperty("source")]
		public dynamic Source { get; set; }

		[JsonProperty("order_lines")]
		public List<dynamic> Order_lines { get; set; }

		[JsonProperty("refunded_amount")]
		public int Refunded_amount { get; set; }

		[JsonProperty("authorized_amount")]
		public int Authorized_amount { get; set; }

		[JsonProperty("error_state")]
		public string Error_state { get; set; }

		[JsonProperty("recurring_payment_method")]
		public string Recurring_payment_method { get; set; }

		[JsonProperty("billing_address")]
		public Address Billing_address { get; set; }

		[JsonProperty("shipping_address")]
		public Address Shipping_address { get; set; }
	}
}
