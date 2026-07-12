using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(5301)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct JoinPartyCallback_t
	{
		public string m_rgchConnectString
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_rgchConnectString_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_rgchConnectString_, 256);
			}
		}

		public const int k_iCallback = 5301;

		public EResult m_eResult;

		public PartyBeaconID_t m_ulBeaconID;

		public CSteamID m_SteamIDBeaconOwner;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_rgchConnectString_;
	}
}
