using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1103)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct UserAchievementStored_t
	{
		public string m_rgchAchievementName
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_rgchAchievementName_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_rgchAchievementName_, 128);
			}
		}

		public const int k_iCallback = 1103;

		public ulong m_nGameID;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bGroupAchievement;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private byte[] m_rgchAchievementName_;

		public uint m_nCurProgress;

		public uint m_nMaxProgress;
	}
}
