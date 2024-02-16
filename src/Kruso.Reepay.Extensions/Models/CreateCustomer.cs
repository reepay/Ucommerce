using System;
using Newtonsoft.Json;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;

namespace Kruso.Reepay.Extensions.Models
{
	public class CreateCustomer : CustomerAddress
	{
		[JsonProperty("handle")]
		public string Handle { get; set; }

		[JsonProperty("test")]
		public bool Test { get; set; }

		[JsonProperty("metadata")]
		public dynamic Metadata { get; set; }

		[JsonProperty("generate_handle")]
		public bool Generate_handle { get; set; }

		public CreateCustomer() { }

		public CreateCustomer(PaymentMethod paymentMethod)
		{
			try
			{
				Test = paymentMethod.DynamicProperty<bool>().Test;
			}
			catch (Exception)
			{
				Test = false;
			}
		}
	}
}
