using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(5701)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamRemotePlaySessionConnected_t
	{
		public const int k_iCallback = 5701;

		public uint m_unSessionID;
	}
}
