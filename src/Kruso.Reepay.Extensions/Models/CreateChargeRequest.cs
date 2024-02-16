using Newtonsoft.Json;

namespace Kruso.Reepay.Extensions.Models
{
	public class CreateChargeRequest : Order
	{
		[JsonProperty("source")]
		public string Source { get; set; }

		[JsonProperty("settle")]
		public bool Settle { get; set; }

		[JsonProperty("recurring")]
		public bool Recurring { get; set; }

		[JsonProperty("text_on_statement")]
		public string Text_on_statement { get; set; }

		public CreateChargeRequest()
        {
			Recurring = true;
        }

		public CreateChargeRequest(Order order) : this()
		{
			if (order == null) return;

			Handle = order.Handle;
			Key = order.Key;
			Amount = order.Amount;
			Currency = order.Currency;
			Customer = order.Customer;
			Metadata = order.Metadata;
			Ordertext = order.Ordertext;
			Order_lines = order.Order_lines;
			Customer_handle = order.Customer_handle;
			Billing_address = order.Billing_address;
			Shipping_address = order.Shipping_address;
		}
	}
}
