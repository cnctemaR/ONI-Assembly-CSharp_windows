using System;

namespace System.Runtime.InteropServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Obsolete]
	[Guid("b196b287-bab4-101a-b69c-00aa00341d07")]
	[ComImport]
	public interface UCOMIEnumConnections
	{
		[PreserveSig]
		int Next(int celt, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] [Out] CONNECTDATA[] rgelt, out int pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		[PreserveSig]
		void Reset();

		void Clone(out UCOMIEnumConnections ppenum);
	}
}
