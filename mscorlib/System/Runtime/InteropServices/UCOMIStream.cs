using System;

namespace System.Runtime.InteropServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Obsolete]
	[Guid("0000000c-0000-0000-c000-000000000046")]
	[ComImport]
	public interface UCOMIStream
	{
		void Read([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [Out] byte[] pv, int cb, IntPtr pcbRead);

		void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbWritten);

		void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition);

		void SetSize(long libNewSize);

		void CopyTo(UCOMIStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten);

		void Commit(int grfCommitFlags);

		void Revert();

		void LockRegion(long libOffset, long cb, int dwLockType);

		void UnlockRegion(long libOffset, long cb, int dwLockType);

		void Stat(out STATSTG pstatstg, int grfStatFlag);

		void Clone(out UCOMIStream ppstm);
	}
}
