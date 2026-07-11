using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1030)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct TimedTrialStatus_t
	{
		public const int k_iCallback = 1030;

		public AppId_t m_unAppID;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bIsOffline;

		public uint m_unSecondsAllowed;

		public uint m_unSecondsPlayed;
	}
}
