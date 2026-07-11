using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1281)]
	public struct SteamRelayNetworkStatus_t
	{
		public string m_debugMsg
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_debugMsg_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_debugMsg_, 256);
			}
		}

		public const int k_iCallback = 1281;

		public ESteamNetworkingAvailability m_eAvail;

		public int m_bPingMeasurementInProgress;

		public ESteamNetworkingAvailability m_eAvailNetworkConfig;

		public ESteamNetworkingAvailability m_eAvailAnyRelay;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		private byte[] m_debugMsg_;
	}
}
