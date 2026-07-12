using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3412)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct AddUGCDependencyResult_t
	{
		public const int k_iCallback = 3412;

		public EResult m_eResult;

		public PublishedFileId_t m_nPublishedFileId;

		public PublishedFileId_t m_nChildPublishedFileId;
	}
}
