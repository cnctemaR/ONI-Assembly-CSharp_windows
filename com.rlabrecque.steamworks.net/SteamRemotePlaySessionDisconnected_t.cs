using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(5702)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamRemotePlaySessionDisconnected_t
	{
		public const int k_iCallback = 5702;

		public RemotePlaySessionID_t m_unSessionID;
	}
}
