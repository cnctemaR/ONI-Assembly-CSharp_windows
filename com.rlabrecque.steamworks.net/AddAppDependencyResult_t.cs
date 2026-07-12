using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3414)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct AddAppDependencyResult_t
	{
		public const int k_iCallback = 3414;

		public EResult m_eResult;

		public PublishedFileId_t m_nPublishedFileId;

		public AppId_t m_nAppID;
	}
}
