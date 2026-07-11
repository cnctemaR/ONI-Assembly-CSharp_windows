using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(166)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct MarketEligibilityResponse_t
	{
		public const int k_iCallback = 166;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bAllowed;

		public EMarketNotAllowedReasonFlags m_eNotAllowedReason;

		public RTime32 m_rtAllowedAtTime;

		public int m_cdaySteamGuardRequiredDays;

		public int m_cdayNewDeviceCooldown;
	}
}
