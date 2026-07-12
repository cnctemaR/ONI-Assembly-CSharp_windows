using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(5304)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct ChangeNumOpenSlotsCallback_t
	{
		public const int k_iCallback = 5304;

		public EResult m_eResult;
	}
}
