using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class Coupon
	{
		[JsonProperty("handle")]
		public string Handle { get; set; }

		[JsonProperty("code")]
		public string Code { get; set; }

		[JsonProperty("discount")]
		public string Discount { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("redemptions")]
		public int Redemptions { get; set; }

		[JsonProperty("expired")]
		public string Expired { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("all_plans")]
		public bool All_plans { get; set; }

		[JsonProperty("eligible_plans")]
		public IList<string> Eligible_plans { get; set; }

		[JsonProperty("max_redemptions")]
		public int Max_redemptions { get; set; }

		[JsonProperty("valid_until")]
		public string Valid_until { get; set; }

		[JsonProperty("expire_reason")]
		public string Expire_reason { get; set; }
	}
}
