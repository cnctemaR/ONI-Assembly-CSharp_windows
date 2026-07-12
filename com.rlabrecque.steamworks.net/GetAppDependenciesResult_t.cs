using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3416)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GetAppDependenciesResult_t
	{
		public const int k_iCallback = 3416;

		public EResult m_eResult;

		public PublishedFileId_t m_nPublishedFileId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public AppId_t[] m_rgAppIDs;

		public uint m_nNumAppDependencies;

		public uint m_nTotalNumAppDependencies;
	}
}
