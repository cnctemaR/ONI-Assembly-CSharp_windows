using System;

namespace Steamworks
{
	public enum ESteamNetworkingConfigScope
	{
		k_ESteamNetworkingConfig_Global = 1,
		k_ESteamNetworkingConfig_SocketsInterface,
		k_ESteamNetworkingConfig_ListenSocket,
		k_ESteamNetworkingConfig_Connection,
		k_ESteamNetworkingConfigScope__Force32Bit = 2147483647
	}
}
