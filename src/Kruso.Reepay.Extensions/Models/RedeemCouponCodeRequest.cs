using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class RedeemCouponCodeRequest
	{
		[JsonProperty("code")]
		public string Code { get; set; }
	}
}
