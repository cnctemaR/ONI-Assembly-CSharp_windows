using System;

namespace Steamworks
{
	[Flags]
	public enum EMarketNotAllowedReasonFlags
	{
		k_EMarketNotAllowedReason_None = 0,
		k_EMarketNotAllowedReason_TemporaryFailure = 1,
		k_EMarketNotAllowedReason_AccountDisabled = 2,
		k_EMarketNotAllowedReason_AccountLockedDown = 4,
		k_EMarketNotAllowedReason_AccountLimited = 8,
		k_EMarketNotAllowedReason_TradeBanned = 16,
		k_EMarketNotAllowedReason_AccountNotTrusted = 32,
		k_EMarketNotAllowedReason_SteamGuardNotEnabled = 64,
		k_EMarketNotAllowedReason_SteamGuardOnlyRecentlyEnabled = 128,
		k_EMarketNotAllowedReason_RecentPasswordReset = 256,
		k_EMarketNotAllowedReason_NewPaymentMethod = 512,
		k_EMarketNotAllowedReason_InvalidCookie = 1024,
		k_EMarketNotAllowedReason_UsingNewDevice = 2048,
		k_EMarketNotAllowedReason_RecentSelfRefund = 4096,
		k_EMarketNotAllowedReason_NewPaymentMethodCannotBeVerified = 8192,
		k_EMarketNotAllowedReason_NoRecentPurchases = 16384,
		k_EMarketNotAllowedReason_AcceptedWalletGift = 32768
	}
}
