using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1251)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamNetworkingMessagesSessionRequest_t
	{
		public const int k_iCallback = 1251;

		public SteamNetworkingIdentity m_identityRemote;
	}
}
