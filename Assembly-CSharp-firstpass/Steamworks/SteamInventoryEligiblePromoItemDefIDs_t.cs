using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4703)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamInventoryEligiblePromoItemDefIDs_t
	{
		public const int k_iCallback = 4703;

		public EResult m_result;

		public CSteamID m_steamID;

		public int m_numEligiblePromoItemDefs;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bCachedData;
	}
}
