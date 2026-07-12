using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1252)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamNetworkingMessagesSessionFailed_t
	{
		public const int k_iCallback = 1252;

		public SteamNetConnectionInfo_t m_info;
	}
}
