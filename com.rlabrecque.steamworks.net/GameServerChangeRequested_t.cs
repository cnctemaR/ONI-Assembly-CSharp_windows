using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(332)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GameServerChangeRequested_t
	{
		public string m_rgchServer
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_rgchServer_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_rgchServer_, 64);
			}
		}

		public string m_rgchPassword
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_rgchPassword_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_rgchPassword_, 64);
			}
		}

		public const int k_iCallback = 332;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private byte[] m_rgchServer_;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		private byte[] m_rgchPassword_;
	}
}
