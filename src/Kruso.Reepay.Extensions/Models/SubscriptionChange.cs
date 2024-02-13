using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class SubscriptionChange
	{
		[JsonProperty("plan")]
		public string Plan { get; set; }

		[JsonProperty("amount")]
		public int Amount { get; set; }
		
		[JsonProperty("quantity")]
		public int Quantity { get; set; }

		[JsonProperty("pending")]
		public bool Pending { get; set; }

		[JsonProperty("applied")]
		public string Applied { get; set; }

		[JsonProperty("updated")]
		public string Updated { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("amount_incl_vat")]
		public bool Amount_incl_vat { get; set; }

		[JsonProperty("subscription_add_ons")]
		public IList<dynamic> Subscription_add_ons { get; set; }

		[JsonProperty("remove_add_ons")]
		public IList<dynamic> Remove_add_ons { get; set; }
	}
}
