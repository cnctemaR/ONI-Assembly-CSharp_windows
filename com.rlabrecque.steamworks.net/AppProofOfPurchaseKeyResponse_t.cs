using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1021)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct AppProofOfPurchaseKeyResponse_t
	{
		public string m_rgchKey
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_rgchKey_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_rgchKey_, 240);
			}
		}

		public const int k_iCallback = 1021;

		public EResult m_eResult;

		public uint m_nAppID;

		public uint m_cchKeyLength;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 240)]
		private byte[] m_rgchKey_;
	}
}
