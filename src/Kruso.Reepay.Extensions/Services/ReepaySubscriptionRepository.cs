using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Kruso.Reepay.Extensions.Enums;
using Kruso.Reepay.Extensions.Models;
using Kruso.Reepay.Extensions.Services.Interfaces;
using Ucommerce.EntitiesV2;
using Ucommerce.Transactions.Payments.Common;

namespace Kruso.Reepay.Extensions.Services
{
	public class ReepaySubscriptionRepository : IReepaySubscriptionRepository
	{
		private readonly IReepayLogger<IReepayRepository> _logger;
		private readonly IReepayGateway _reepayGateway;
		private readonly IReepayPropertyNames _reepayPropertyNames;

		public ReepaySubscriptionRepository(
			IReepayLogger<IReepayRepository> logger,
			IReepayGateway reepayGateway,
			IReepayPropertyNames propertyNames)
		{
			_logger = logger;
			_reepayGateway = reepayGateway;
			_reepayPropertyNames = propertyNames;
		}

		public async Task<bool?> SubscriptionIsSettledSuccessfully(PaymentMethod paymentMethod, string handle)
		{
			try
			{
				var subscriptionObject = await GetSubscriptionOrNull(paymentMethod, handle).ConfigureAwait(false);

				if (subscriptionObject == null)
				{
					_logger.LogWarning($"Subscription object is null, {handle}");
					throw new Exception(
						$"error: Subscription object is null for handle {handle}");
				}

				if (subscriptionObject.State == SubscriptionStateEnum.active.ToString()
					&& subscriptionObject.Settled_invoices > 0)
				{
					return true;
				}

				if (subscriptionObject.State == SubscriptionStateEnum.expired.ToString()
					|| subscriptionObject.Failed_invoices > 0)
				{
					return false;
				}

				// pending status
				return null;
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
				return null;
			}
		}

		public virtual Task<SubscriptionObject> GetSubscription(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			return _reepayGateway.GetSubscription(paymentMethod, subscriptionHandle);
		}

		public virtual Task<IList<SubscriptionDiscount>> GetSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			return _reepayGateway.GetSubscriptionDiscounts(paymentMethod, subscriptionHandle);
		}

		public virtual async Task<SubscriptionObject> CreateSubscription(Payment payment, string customerHandle, string source, string subscriptionHandle = "", bool useDiscounts = true)
		{
			var plan = _reepayPropertyNames.GetPropertyValueFromOrder(payment.PurchaseOrder, _reepayPropertyNames.SubscriptionPlan);
			if (string.IsNullOrWhiteSpace(subscriptionHandle))
			{
				subscriptionHandle = $"{customerHandle}-{payment?.PurchaseOrder?.OrderId}";
			}

			if (!useDiscounts)
			{
				var data = CreateSubscriptionRequestData(payment.PaymentMethod, customerHandle, source, plan, subscriptionHandle);
				data.Conditional_create = true;
				return await _reepayGateway.CreateSubscription(payment.PaymentMethod, data).ConfigureAwait(false);
			}

			var subscriptionDiscounts = await GetSubscriptionDiscounts(payment, subscriptionHandle, false).ConfigureAwait(false);
			var requestData = CreateSubscriptionRequestData(payment.PaymentMethod, customerHandle, source, plan, subscriptionHandle, GetCouponCodes(payment?.PurchaseOrder), subscriptionDiscounts);
			requestData.Conditional_create = true;
			return await _reepayGateway.CreateSubscription(payment.PaymentMethod, requestData).ConfigureAwait(false);
		}

		public virtual async Task<SubscriptionObject> CreateSubscription(PaymentMethod paymentMethod,
			string customerHandle,
			string plan,
			string source,
			bool useDiscounts = true,
			string subscriptionHandle = "",
			string trialPeriod = "",
			string startDate = "",
			string couponCode = "",
			string discountName = "",
			decimal discountAmount = 0m)
		{
			if (!useDiscounts)
			{
				var data = CreateSubscriptionRequestData(paymentMethod, customerHandle, source, plan, subscriptionHandle);
				if (!string.IsNullOrWhiteSpace(startDate))
				{
					data.Start_date = startDate;
				}
				if (!string.IsNullOrWhiteSpace(trialPeriod))
				{
					data.Trial_period = trialPeriod;
				}

				return await _reepayGateway.CreateSubscription(paymentMethod, data).ConfigureAwait(false);
			}

			var subscriptionDiscounts = await GetSubscriptionDiscounts(paymentMethod, subscriptionHandle, discountName, discountAmount, false).ConfigureAwait(false);
			var requestData = CreateSubscriptionRequestData(paymentMethod, customerHandle, source, plan, subscriptionHandle, GetCouponCodes(couponCode), subscriptionDiscounts);
			if (!string.IsNullOrWhiteSpace(startDate))
			{
				requestData.Start_date = startDate;
			}
			if (!string.IsNullOrWhiteSpace(trialPeriod))
			{
				requestData.Trial_period = trialPeriod;
			}

			return await _reepayGateway.CreateSubscription(paymentMethod, requestData).ConfigureAwait(false);
		}

		public virtual Task UpdateCreditCardInfo(Payment payment, string source)
		{
			var subscriptionHandle = _reepayPropertyNames.GetPropertyValueFromOrder(payment.PurchaseOrder, _reepayPropertyNames.CurrentSubscriptionHandle);
			return UpdateCreditCardInfo(payment?.PaymentMethod, source, subscriptionHandle);
		}

		public virtual async Task UpdateCreditCardInfo(PaymentMethod paymentMethod, string source, string subscriptionHandle)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				_logger.LogWarning($"Cannot UpdateCreditCardInfo because source is empty for subscription {subscriptionHandle}");
				return;
			}

			if (string.IsNullOrWhiteSpace(subscriptionHandle))
			{
				_logger.LogWarning($"Cannot UpdateCreditCardInfo because subscriptionHandle is empty");
				return;
			}

			var subscription = await GetSubscriptionOrNull(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			if (subscription == null)
			{
				_logger.LogWarning($"Cannot UpdateCreditCardInfo because subscription with handle {subscriptionHandle} is null");
				return;
			}

			CreateSubscriptionRequest subscriptionRequestData = null;
			try
			{
				subscriptionRequestData = await CreateSubscriptionRequestData(paymentMethod, subscription, source).ConfigureAwait(false);
			}
			catch(Exception ex)
			{
				_logger.LogException(ex);
			}

			var setSubscriptionCardData = new SetPaymentMethodRequest()
			{
				Source = source
			};

			await _reepayGateway.RemoveAllSubscriptionCards(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			await _reepayGateway.SetSubscriptionCard(paymentMethod, subscriptionHandle, setSubscriptionCardData).ConfigureAwait(false);
			subscription = await GetSubscriptionOrNull(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			if (subscription == null)
			{
				subscription = await _reepayGateway.CreateSubscription(paymentMethod, subscriptionRequestData).ConfigureAwait(false);
				if (subscription == null)
				{
					_logger.LogWarning($"Cannot UpdateCreditCardInfo because subscription with handle {subscriptionHandle} become null and can't be recreated");
					return;
				}
			}

			_logger.LogInfo($"CreditCardInfo was updated successfully for subscription with handle {subscriptionHandle}");
		}

		public virtual async Task<SubscriptionObject> ChangeSubscription(PaymentMethod paymentMethod, string subscriptionHandle, string plan)
		{
			if (string.IsNullOrWhiteSpace(subscriptionHandle))
			{
				_logger.LogWarning($"Cannot ChangeSubscription because subscriptionHandle is empty");
				return null;
			}

			var subscription = await GetSubscriptionOrNull(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			if (subscription == null)
			{
				_logger.LogWarning($"Cannot ChangeSubscription because subscription does not exist");
				return null;
			}

			await DeleteSubscriptionDiscountsWithoutErrors(paymentMethod, subscriptionHandle).ConfigureAwait(false);

			SubscriptionObject changedSubscription = null;
			if (!string.IsNullOrWhiteSpace(plan))
			{
				var requestData = new ChangeSubscriptionRequest()
				{
					Plan = plan
				};

				if (plan == subscription.Plan)
				{
					requestData.Cancel_change = true;
				}

				changedSubscription = await ChangeSubscription(paymentMethod, subscriptionHandle, requestData).ConfigureAwait(false);
				if (changedSubscription != null)
				{
					if ((plan != subscription.Plan && changedSubscription.Pending_change == null) || (plan == subscription.Plan && changedSubscription.Pending_change != null))
					{
						_logger.LogWarning($"Subscription {subscriptionHandle} was not changed to plan {plan}");
					}
				}
			}

			_logger.LogInfo($"Subscription with handle {subscriptionHandle} was changed successfully");

			return changedSubscription ?? await GetSubscriptionOrNull(paymentMethod, subscriptionHandle).ConfigureAwait(false);
		}

		public virtual async Task<bool> AddSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle, IList<string> couponCodes = null, IList<CreateSubscriptionDiscount> subscriptionDiscounts = null)
		{
			try
			{
				if (couponCodes != null && couponCodes.Count > 0)
				{
					await _reepayGateway.RedeemCouponCodes(paymentMethod, subscriptionHandle, couponCodes).ConfigureAwait(false);
				}

				if (subscriptionDiscounts != null && subscriptionDiscounts.Count > 0)
				{
					await _reepayGateway.AddSubscriptionDiscounts(paymentMethod, subscriptionHandle, subscriptionDiscounts).ConfigureAwait(false);
				}

				_logger.LogInfo($"Discounts were added successfully to subscription with handle {subscriptionHandle}");
				return true;
			}
			catch(Exception ex)
			{
				_logger.LogException(ex);
				return false;
			}
		}

		public virtual Task<SubscriptionObject> CancelSubscription(PaymentMethod paymentMethod, string subscriptionHandle, CancelSubscriptionRequest requestData = null)
		{
			return _reepayGateway.CancelSubscription(paymentMethod, subscriptionHandle, requestData);
		}

		public virtual Task<SubscriptionObject> UnCancelSubscription(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			return _reepayGateway.UnCancelSubscription(paymentMethod, subscriptionHandle);
		}

		public virtual Task DeleteSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			return _reepayGateway.DeleteAllSubscriptionDiscounts(paymentMethod, subscriptionHandle);
		}

		public async Task<string> GenerateSubscriptionDiscountHandle(PaymentMethod paymentMethod, string subscriptionHandle, bool subscriptionExists = true)
		{
			if (!subscriptionExists)
			{
				return $"{subscriptionHandle}-discount";
			}

			var discounts = await GetSubscriptionDiscountsWithoutCoupons(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			if (discounts == null || discounts.Count() == 0)
			{
				return $"{subscriptionHandle}-discount";
			}

			var existingHandle = discounts.FirstOrDefault().Handle;
			return GenerateNextSubscriptionDiscountHandle(subscriptionHandle, existingHandle);
		}

		public async Task DeleteUsedDiscounts(Payment payment, string subscriptionHandle)
		{
			if (!_reepayPropertyNames.GetBoolPropertyValueFromOrder(payment?.PurchaseOrder, _reepayPropertyNames.ShouldSubscriptionDiscountsRemainActive))
			{
				await DeleteSubscriptionDiscountsWithoutErrors(payment.PaymentMethod, subscriptionHandle).ConfigureAwait(false);
			}
		}

		public Task DeleteUsedDiscounts(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			return DeleteSubscriptionDiscountsWithoutErrors(paymentMethod, subscriptionHandle);
		}

		protected virtual IList<string> GetCouponCodes(PurchaseOrder order)
		{
			if (order == null) return null;

			var couponCode = _reepayPropertyNames.GetPropertyValueFromOrder(order, _reepayPropertyNames.CouponCode);
			return GetCouponCodes(couponCode);
		}

		protected virtual IList<string> GetCouponCodes(string couponCode)
		{
			if (string.IsNullOrWhiteSpace(couponCode))
			{
				return null;
			}

			return new List<string>() { couponCode };
		}

		protected virtual async Task<IList<CreateSubscriptionDiscount>> GetSubscriptionDiscounts(Payment payment, string subscriptionHandle, bool subscriptionExists = true)
		{
			var order = payment?.PurchaseOrder;
			if (order == null) return null;

			var discountName = _reepayPropertyNames.GetPropertyValueFromOrder(order, _reepayPropertyNames.DiscountName);
			if (string.IsNullOrWhiteSpace(discountName))
			{
				return null;
			}

			var discountAmountStr = _reepayPropertyNames.GetPropertyValueFromOrder(order, _reepayPropertyNames.SubscriptionDiscountAmount);
			IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
			if (!decimal.TryParse(discountAmountStr.Replace(",", "."), NumberStyles.AllowDecimalPoint, formatter, out decimal discountAmount)) discountAmount = 0m;

			return await GetSubscriptionDiscounts(payment?.PaymentMethod, subscriptionHandle, discountName, discountAmount, subscriptionExists).ConfigureAwait(false);
		}

		protected virtual async Task<IList<CreateSubscriptionDiscount>> GetSubscriptionDiscounts(PaymentMethod paymentMethod, string subscriptionHandle, string discountName, decimal discountAmount, bool subscriptionExists = true)
		{
			if (string.IsNullOrWhiteSpace(discountName))
			{
				return null;
			}

			var subscriptionDiscountHandle = await GenerateSubscriptionDiscountHandle(paymentMethod, subscriptionHandle, subscriptionExists);
			var subscriptionDiscount = new CreateSubscriptionDiscount
			{
				Handle = subscriptionDiscountHandle,
				Discount = discountName
			};

			if (discountAmount > 0m)
			{
				subscriptionDiscount.Amount = discountAmount.ToCents();
			}

			return new List<CreateSubscriptionDiscount>() { subscriptionDiscount };
		}

		private async Task<SubscriptionObject> ChangeSubscription(PaymentMethod paymentMethod, string subscriptionHandle, ChangeSubscriptionRequest requestData)
		{
			try
			{
				return await _reepayGateway.ChangeSubscription(paymentMethod, subscriptionHandle, requestData).ConfigureAwait(false);
			}
			catch(Exception ex)
			{
				_logger.LogException(ex);
			}

			return null;
		}

		private CreateSubscriptionRequest CreateSubscriptionRequestData(PaymentMethod paymentMethod, string customerHandle, string source, string plan, string subscriptionHandle = "", IList<string> couponCodes = null, IList<CreateSubscriptionDiscount> subscriptionDiscounts = null)
		{
			var requestData = new CreateSubscriptionRequest(paymentMethod)
			{
				Plan = plan,
				Customer = customerHandle,
				Source = source,
				Coupon_codes = couponCodes,
				Subscription_discounts = subscriptionDiscounts
			};

			if (string.IsNullOrWhiteSpace(subscriptionHandle))
			{
				requestData.Generate_handle = true;
			}
			else
			{
				requestData.Handle = subscriptionHandle;
			}

			return requestData;
		}

		private async Task<CreateSubscriptionRequest> CreateSubscriptionRequestData(PaymentMethod paymentMethod, SubscriptionObject subscription, string source)
		{
			if (subscription == null)
			{
				return new CreateSubscriptionRequest(paymentMethod);
			}

			var subscriptionHandle = subscription.Handle;
			var customerHandle = subscription.Customer;
			var plan = subscription.Plan;
			var subscriptionDiscounts = await GetSubscriptionDiscountsOrNull(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			if (subscriptionDiscounts == null || subscriptionDiscounts.Count() == 0)
			{
				return CreateSubscriptionRequestData(paymentMethod, customerHandle, source, plan, subscriptionHandle);
			}

			var couponCodes = subscriptionDiscounts
				.Where(discount => !string.IsNullOrWhiteSpace(discount.Coupon) && discount.State == SubscriptionDiscountStateEnum.active.ToString())
				.Select(discount => discount.Coupon)
				.ToList();

			IList<CreateSubscriptionDiscount> subscriptionDiscountsToCreate = null;
			var discountsWithoutCoupons = await GetSubscriptionDiscountsWithoutCoupons(paymentMethod, subscriptionHandle, true, subscriptionDiscounts).ConfigureAwait(false);
			if (discountsWithoutCoupons != null && discountsWithoutCoupons.Count() > 0)
			{
				subscriptionDiscountsToCreate = discountsWithoutCoupons
				.Select(discount => new CreateSubscriptionDiscount(discount))
				.ToList();
				var lastDiscountHandle = discountsWithoutCoupons.FirstOrDefault().Handle;
				for (var i = subscriptionDiscountsToCreate.Count() - 1; i >= 0; i--)
				{
					subscriptionDiscountsToCreate[i].Handle = GenerateNextSubscriptionDiscountHandle(subscriptionHandle, lastDiscountHandle);
					lastDiscountHandle = subscriptionDiscountsToCreate[i].Handle;
				}
			}

			return CreateSubscriptionRequestData(paymentMethod, customerHandle, source, plan, subscriptionHandle, couponCodes, subscriptionDiscountsToCreate);
		}

		private async Task<IList<SubscriptionDiscount>> GetSubscriptionDiscountsWithoutCoupons(PaymentMethod paymentMethod, string subscriptionHandle, bool active = false, IList<SubscriptionDiscount> allSubscriptionDiscounts = null)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(subscriptionHandle))
				{
					return null;
				}

				if (allSubscriptionDiscounts == null)
				{
					allSubscriptionDiscounts = await _reepayGateway.GetSubscriptionDiscounts(paymentMethod, subscriptionHandle).ConfigureAwait(false);
				}

				if (allSubscriptionDiscounts == null || allSubscriptionDiscounts.Count() == 0)
				{
					return null;
				}

				var subscriptionDiscounts = allSubscriptionDiscounts.Where(discount => string.IsNullOrWhiteSpace(discount.Coupon)).OrderByDescending(discount => discount.Created).ToList();
				if (subscriptionDiscounts == null || subscriptionDiscounts.Count() == 0)
				{
					return null;
				}

				if (active)
				{
					subscriptionDiscounts = subscriptionDiscounts.Where(discount => discount.State == SubscriptionDiscountStateEnum.active.ToString()).ToList();
				}

				return subscriptionDiscounts;
			}
			catch(Exception ex)
			{
				_logger.LogException(ex);
				return null;
			}
		}

		private string GenerateNextSubscriptionDiscountHandle(string subscriptionHandle, string existingHandle)
		{
			if (string.IsNullOrWhiteSpace(existingHandle))
			{
				return $"{subscriptionHandle}-discount";
			}

			if (!existingHandle.Contains("-discount"))
			{
				return $"{existingHandle}-discount";
			}

			if (existingHandle.EndsWith("-discount"))
			{
				return $"{existingHandle}@1";
			}

			var number = existingHandle.Split('@').LastOrDefault();
			if (!int.TryParse(number, out int result)) result = 0;

			return $"{existingHandle.Split('@').FirstOrDefault()}@{result + 1}";
		}

		private async Task<SubscriptionObject> GetSubscriptionOrNull(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			try
			{
				return await _reepayGateway.GetSubscription(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			}
			catch (ReepayException ex)
			{
				_logger.LogReepayException(ex);
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
			}

			return null;
		}

		private async Task<IList<SubscriptionDiscount>> GetSubscriptionDiscountsOrNull(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			try
			{
				return await _reepayGateway.GetSubscriptionDiscounts(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			}
			catch (ReepayException ex)
			{
				_logger.LogReepayException(ex);
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
			}

			return null;
		}

		private async Task DeleteSubscriptionDiscountsWithoutErrors(PaymentMethod paymentMethod, string subscriptionHandle)
		{
			try
			{
				await _reepayGateway.DeleteAllSubscriptionDiscounts(paymentMethod, subscriptionHandle).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
			}
		}
	}
}
