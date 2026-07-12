using System;

namespace Steamworks
{
	public enum ESteamNetworkingIdentityType
	{
		k_ESteamNetworkingIdentityType_Invalid,
		k_ESteamNetworkingIdentityType_SteamID = 16,
		k_ESteamNetworkingIdentityType_IPAddress = 1,
		k_ESteamNetworkingIdentityType_GenericString,
		k_ESteamNetworkingIdentityType_GenericBytes,
		k_ESteamNetworkingIdentityType_UnknownType,
		k_ESteamNetworkingIdentityType__Force32bit = 2147483647
	}
}
