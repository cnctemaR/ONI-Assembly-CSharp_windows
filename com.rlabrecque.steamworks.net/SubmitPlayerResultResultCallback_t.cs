using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(5214)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SubmitPlayerResultResultCallback_t
	{
		public const int k_iCallback = 5214;

		public EResult m_eResult;

		public ulong ullUniqueGameID;

		public CSteamID steamIDPlayer;
	}
}
