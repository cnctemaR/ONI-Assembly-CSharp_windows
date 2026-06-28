using System;

namespace System.Runtime.InteropServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Obsolete]
	[Guid("00020404-0000-0000-c000-000000000046")]
	[ComImport]
	public interface UCOMIEnumVARIANT
	{
		[PreserveSig]
		int Next(int celt, int rgvar, int pceltFetched);

		[PreserveSig]
		int Skip(int celt);

		[PreserveSig]
		int Reset();

		void Clone(int ppenum);
	}
}
