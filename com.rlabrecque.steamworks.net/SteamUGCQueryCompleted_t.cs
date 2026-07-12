using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3401)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamUGCQueryCompleted_t
	{
		public string m_rgchNextCursor
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_rgchNextCursor_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_rgchNextCursor_, 256);
			}
		}

		public const int k_iCallback = 3401;

		public UGCQueryHandle_t m_handle;

		public EResult m_eResult;

		public uint m_unNumResultsReturned;

		public uint m_unTotalMatchingResults;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bCachedData;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_rgchNextCursor_;
	}
}
