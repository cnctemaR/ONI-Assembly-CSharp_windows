using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(337)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GameRichPresenceJoinRequested_t
	{
		public string m_rgchConnect
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_rgchConnect_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_rgchConnect_, 256);
			}
		}

		public const int k_iCallback = 337;

		public CSteamID m_steamIDFriend;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_rgchConnect_;
	}
}
