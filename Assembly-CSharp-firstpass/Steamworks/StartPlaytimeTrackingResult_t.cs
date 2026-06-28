using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3410)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct StartPlaytimeTrackingResult_t
	{
		public const int k_iCallback = 3410;

		public EResult m_eResult;
	}
}
