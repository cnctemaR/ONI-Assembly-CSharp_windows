using System;

namespace Steamworks
{
	public enum ESteamNetworkingGetConfigValueResult
	{
		k_ESteamNetworkingGetConfigValue_BadValue = -1,
		k_ESteamNetworkingGetConfigValue_BadScopeObj = -2,
		k_ESteamNetworkingGetConfigValue_BufferTooSmall = -3,
		k_ESteamNetworkingGetConfigValue_OK = 1,
		k_ESteamNetworkingGetConfigValue_OKInherited,
		k_ESteamNetworkingGetConfigValueResult__Force32Bit = 2147483647
	}
}
