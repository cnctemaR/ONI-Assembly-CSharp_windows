using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(165)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct StoreAuthURLResponse_t
	{
		public string m_szURL
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_szURL_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_szURL_, 512);
			}
		}

		public const int k_iCallback = 165;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
		private byte[] m_szURL_;
	}
}
