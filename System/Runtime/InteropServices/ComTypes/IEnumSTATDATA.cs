using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("00000103-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IEnumSTATDATA
	{
		void Clone(out IEnumSTATDATA newEnum);

		[PreserveSig]
		int Next(int celt, [MarshalAs(UnmanagedType.LPArray)] [Out] STATDATA[] rgelt, [MarshalAs(UnmanagedType.LPArray)] [Out] int[] pceltFetched);

		[PreserveSig]
		int Reset();

		[PreserveSig]
		int Skip(int celt);
	}
}
