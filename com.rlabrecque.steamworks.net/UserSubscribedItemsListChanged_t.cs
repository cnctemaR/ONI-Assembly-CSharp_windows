using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3418)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct UserSubscribedItemsListChanged_t
	{
		public const int k_iCallback = 3418;

		public AppId_t m_nAppID;
	}
}
