using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4704)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamInventoryStartPurchaseResult_t
	{
		public const int k_iCallback = 4704;

		public EResult m_result;

		public ulong m_ulOrderID;

		public ulong m_ulTransID;
	}
}
