using System;

namespace System.Runtime.InteropServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Obsolete]
	[Guid("00000101-0000-0000-c000-000000000046")]
	[ComImport]
	public interface UCOMIEnumString
	{
		[PreserveSig]
		int Next(int celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeConst = 0, SizeParamIndex = 0)] [Out] string[] rgelt, out int pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		[PreserveSig]
		int Reset();

		void Clone(out UCOMIEnumString ppenum);
	}
}
