using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(164)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GameWebCallback_t
	{
		public string m_szURL
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_szURL_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_szURL_, 256);
			}
		}

		public const int k_iCallback = 164;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_szURL_;
	}
}
