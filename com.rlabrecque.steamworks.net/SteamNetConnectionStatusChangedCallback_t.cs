using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1221)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamNetConnectionStatusChangedCallback_t
	{
		public const int k_iCallback = 1221;

		public HSteamNetConnection m_hConn;

		public SteamNetConnectionInfo_t m_info;

		public ESteamNetworkingConnectionState m_eOldState;
	}
}
