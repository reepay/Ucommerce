namespace Kruso.Reepay.Extensions.Enums
{
	public enum WebhookSubscriptionEventEnum
	{
		subscription_created,					// Subscription has been created
		subscription_payment_method_added,		// A payment method has been added to the subscription for the first time
		subscription_payment_method_changed,	// The payment method has been changed for a subscription with an existing payment method
		subscription_trial_end,					// The trial period for subscription has ended
		subscription_renewal,					// An invoice has been made and new billing period has started for subscription
		subscription_cancelled,					// Subscription has been cancelled to expire at end of current billing period
		subscription_uncancelled,				// A previous cancellation has been cancelled
		subscription_on_hold,					// Subscription has been put on hold by request
		subscription_on_hold_dunning,			// Subscription has been put on hold due to a failed dunning process
		subscription_reactivated,				// Subscription on hold has been reactivated to active state
		subscription_expired,					// Subscription has expired either by request, end of fixed life time or because cancelled and billing period has ended
		subscription_expired_dunning,			// Subscription has expired due to a failed dunning process
		subscription_changed,					// Subscription scheduling or pricing has been changed, e.g. by changed plan or changed next period start
	}
}