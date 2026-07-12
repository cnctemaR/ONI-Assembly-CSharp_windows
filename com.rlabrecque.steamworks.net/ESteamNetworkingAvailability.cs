using System;

namespace Steamworks
{
	public enum ESteamNetworkingAvailability
	{
		k_ESteamNetworkingAvailability_CannotTry = -102,
		k_ESteamNetworkingAvailability_Failed,
		k_ESteamNetworkingAvailability_Previously,
		k_ESteamNetworkingAvailability_Retrying = -10,
		k_ESteamNetworkingAvailability_NeverTried = 1,
		k_ESteamNetworkingAvailability_Waiting,
		k_ESteamNetworkingAvailability_Attempting,
		k_ESteamNetworkingAvailability_Current = 100,
		k_ESteamNetworkingAvailability_Unknown = 0,
		k_ESteamNetworkingAvailability__Force32bit = 2147483647
	}
}
