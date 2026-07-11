using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1303)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStorageAppSyncProgress_t
	{
		public string m_rgchCurrentFile
		{
			get
			{
				return InteropHelp.ByteArrayToStringUTF8(this.m_rgchCurrentFile_);
			}
			set
			{
				InteropHelp.StringToByteArrayUTF8(value, this.m_rgchCurrentFile_, 260);
			}
		}

		public const int k_iCallback = 1303;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
		private byte[] m_rgchCurrentFile_;

		public AppId_t m_nAppID;

		public uint m_uBytesTransferredThisChunk;

		public double m_dAppPercentComplete;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bUploading;
	}
}
