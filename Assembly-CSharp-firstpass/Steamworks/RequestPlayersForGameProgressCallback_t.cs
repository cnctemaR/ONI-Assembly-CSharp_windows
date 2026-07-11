using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(5211)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RequestPlayersForGameProgressCallback_t
	{
		public const int k_iCallback = 5211;

		public EResult m_eResult;

		public ulong m_ullSearchID;
	}
}
