using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4705)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamInventoryRequestPricesResult_t
	{
		public string m_rgchCurrency
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_rgchCurrency_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_rgchCurrency_, 4);
			}
		}

		public const int k_iCallback = 4705;

		public EResult m_result;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		private byte[] m_rgchCurrency_;
	}
}
