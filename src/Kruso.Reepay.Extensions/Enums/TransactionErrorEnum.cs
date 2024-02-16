namespace Kruso.Reepay.Extensions.Enums
{
	public enum TransactionErrorEnum
	{
		// Hard declines
		credit_card_expired = 0,
		declined_by_acquirer,
		credit_card_lost_or_stolen,
		credit_card_suspected_fraud,
		refund_amount_too_high,
		authorization_expired,
		authorization_amount_exceeded,
		authorization_voided,
		sca_required,
		risk_filter_block,

		// Soft declines
		insufficient_funds,
		settle_blocked,

		// Processing errors
		acquirer_communication_error,
		acquirer_error,
		acquirer_integration_error,
		acquirer_authentication_error,
		acquirer_configuration_error,
		acquirer_rejected_error

		//Attention! Add new states below only.
	}
}
