using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3417)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct DeleteItemResult_t
	{
		public const int k_iCallback = 3417;

		public EResult m_eResult;

		public PublishedFileId_t m_nPublishedFileId;
	}
}
