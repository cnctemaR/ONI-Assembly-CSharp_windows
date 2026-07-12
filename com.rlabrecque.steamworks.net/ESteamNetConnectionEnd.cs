using System;

namespace Steamworks
{
	public enum ESteamNetConnectionEnd
	{
		k_ESteamNetConnectionEnd_Invalid,
		k_ESteamNetConnectionEnd_App_Min = 1000,
		k_ESteamNetConnectionEnd_App_Generic = 1000,
		k_ESteamNetConnectionEnd_App_Max = 1999,
		k_ESteamNetConnectionEnd_AppException_Min,
		k_ESteamNetConnectionEnd_AppException_Generic = 2000,
		k_ESteamNetConnectionEnd_AppException_Max = 2999,
		k_ESteamNetConnectionEnd_Local_Min,
		k_ESteamNetConnectionEnd_Local_OfflineMode,
		k_ESteamNetConnectionEnd_Local_ManyRelayConnectivity,
		k_ESteamNetConnectionEnd_Local_HostedServerPrimaryRelay,
		k_ESteamNetConnectionEnd_Local_NetworkConfig,
		k_ESteamNetConnectionEnd_Local_Rights,
		k_ESteamNetConnectionEnd_Local_P2P_ICE_NoPublicAddresses,
		k_ESteamNetConnectionEnd_Local_Max = 3999,
		k_ESteamNetConnectionEnd_Remote_Min,
		k_ESteamNetConnectionEnd_Remote_Timeout,
		k_ESteamNetConnectionEnd_Remote_BadCrypt,
		k_ESteamNetConnectionEnd_Remote_BadCert,
		k_ESteamNetConnectionEnd_Remote_BadProtocolVersion = 4006,
		k_ESteamNetConnectionEnd_Remote_P2P_ICE_NoPublicAddresses,
		k_ESteamNetConnectionEnd_Remote_Max = 4999,
		k_ESteamNetConnectionEnd_Misc_Min,
		k_ESteamNetConnectionEnd_Misc_Generic,
		k_ESteamNetConnectionEnd_Misc_InternalError,
		k_ESteamNetConnectionEnd_Misc_Timeout,
		k_ESteamNetConnectionEnd_Misc_SteamConnectivity = 5005,
		k_ESteamNetConnectionEnd_Misc_NoRelaySessionsToClient,
		k_ESteamNetConnectionEnd_Misc_P2P_Rendezvous = 5008,
		k_ESteamNetConnectionEnd_Misc_P2P_NAT_Firewall,
		k_ESteamNetConnectionEnd_Misc_PeerSentNoConnection,
		k_ESteamNetConnectionEnd_Misc_Max = 5999,
		k_ESteamNetConnectionEnd__Force32Bit = 2147483647
	}
}
