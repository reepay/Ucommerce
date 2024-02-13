using System;
using System.Collections.Generic;
using Kruso.Reepay.Extensions.Enums;
using Newtonsoft.Json;
using Ucommerce.EntitiesV2;
using Ucommerce.Extensions;

namespace Kruso.Reepay.Extensions.Models
{
	public class CreateSubscriptionRequest : SubscriptionBase
	{
		[JsonProperty("metadata")]
		public dynamic Metadata { get; set; }

		[JsonProperty("source")]
		public string Source { get; set; }

		[JsonProperty("create_customer")]
		public CreateCustomer Create_customer { get; set; }

		[JsonProperty("generate_handle")]
		public bool Generate_handle { get; set; }

		[JsonProperty("no_trial")]
		public bool No_trial { get; set; }

		[JsonProperty("no_setup_fee")]
		public bool No_setup_fee { get; set; }

		[JsonProperty("trial_period")]
		public string Trial_period { get; set; }

		[JsonProperty("coupon_codes")]
		public IList<string> Coupon_codes { get; set; }

		[JsonProperty("subscription_discounts")]
		public IList<CreateSubscriptionDiscount> Subscription_discounts { get; set; }

		[JsonProperty("add_ons")]
		public IList<dynamic> Add_ons { get; set; }

		[JsonProperty("additional_costs")]
		public IList<dynamic> Additional_costs { get; set; }

		[JsonProperty("signup_method")]
		public string Signup_method { get; set; }

		[JsonProperty("conditional_create")]
		public bool Conditional_create { get; set; }

		public CreateSubscriptionRequest()
		{
			Signup_method = SubscriptionSignupMethodEnum.source.ToString();
		}

		public CreateSubscriptionRequest(PaymentMethod paymentMethod) : this()
		{
			try
			{
				Test = paymentMethod.DynamicProperty<bool>().Test;
			}
			catch(Exception)
			{
				Test = false;
			}
		}
	}
}
