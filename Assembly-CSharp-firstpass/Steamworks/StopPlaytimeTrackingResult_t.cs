using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3411)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct StopPlaytimeTrackingResult_t
	{
		public const int k_iCallback = 3411;

		public EResult m_eResult;
	}
}
