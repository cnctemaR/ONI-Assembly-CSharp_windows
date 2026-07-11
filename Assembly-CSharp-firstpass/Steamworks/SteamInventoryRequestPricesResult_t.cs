using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4705)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamInventoryRequestPricesResult_t
	{
		public const int k_iCallback = 4705;

		public EResult m_result;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
		public string m_rgchCurrency;
	}
}
