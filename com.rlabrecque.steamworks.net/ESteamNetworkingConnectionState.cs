using System;

namespace Steamworks
{
	public enum ESteamNetworkingConnectionState
	{
		k_ESteamNetworkingConnectionState_None,
		k_ESteamNetworkingConnectionState_Connecting,
		k_ESteamNetworkingConnectionState_FindingRoute,
		k_ESteamNetworkingConnectionState_Connected,
		k_ESteamNetworkingConnectionState_ClosedByPeer,
		k_ESteamNetworkingConnectionState_ProblemDetectedLocally,
		k_ESteamNetworkingConnectionState_FinWait = -1,
		k_ESteamNetworkingConnectionState_Linger = -2,
		k_ESteamNetworkingConnectionState_Dead = -3,
		k_ESteamNetworkingConnectionState__Force32Bit = 2147483647
	}
}
