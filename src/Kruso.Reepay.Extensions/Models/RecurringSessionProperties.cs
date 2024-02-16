using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class RecurringSessionProperties : SessionProperties
	{	
		[JsonProperty("customer")]
		public string Customer { get; set; }

		[JsonProperty("currency")]
		public string Currency { get; set; }

		[JsonProperty("create_customer")]
		public CreateCustomer Create_customer { get; set; }

		[JsonProperty("order_text")]
		public string Order_text { get; set; }

		public RecurringSessionProperties() : base()
        {

        }
	}
}
