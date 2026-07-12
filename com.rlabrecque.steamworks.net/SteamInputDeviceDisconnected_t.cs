using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(2802)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamInputDeviceDisconnected_t
	{
		public const int k_iCallback = 2802;

		public InputHandle_t m_ulDisconnectedDeviceHandle;
	}
}
