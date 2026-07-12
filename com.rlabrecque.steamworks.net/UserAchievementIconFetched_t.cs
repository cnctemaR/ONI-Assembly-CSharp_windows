using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1109)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct UserAchievementIconFetched_t
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

		public const int k_iCallback = 1109;

		public CGameID m_nGameID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private byte[] m_rgchAchievementName_;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bAchieved;

		public int m_nIconHandle;
	}
}
