using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000103-0000-0000-C000-000000000046")]
	[ComImport]
	public interface IEnumFORMATETC
	{
		void Clone(out IEnumFORMATETC newEnum);

		[PreserveSig]
		int Next(int celt, [MarshalAs(UnmanagedType.LPArray)] [Out] FORMATETC[] rgelt, [MarshalAs(UnmanagedType.LPArray)] [Out] int[] pceltFetched);

		[PreserveSig]
		int Reset();

		[PreserveSig]
		int Skip(int celt);
	}
}
