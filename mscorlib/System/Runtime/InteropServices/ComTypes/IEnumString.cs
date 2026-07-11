using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("00000101-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IEnumString
	{
		[PreserveSig]
		int Next(int celt, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 0)] [Out] string[] rgelt, IntPtr pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		void Reset();

		void Clone(out IEnumString ppenum);
	}
}
