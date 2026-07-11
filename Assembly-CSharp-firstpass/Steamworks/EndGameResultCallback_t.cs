using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(5215)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct EndGameResultCallback_t
	{
		public const int k_iCallback = 5215;

		public EResult m_eResult;

		public ulong ullUniqueGameID;
	}
}
