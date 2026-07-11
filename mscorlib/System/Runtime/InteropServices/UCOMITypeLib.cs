using System;

namespace System.Runtime.InteropServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00020402-0000-0000-c000-000000000046")]
	[Obsolete]
	[ComImport]
	public interface UCOMITypeLib
	{
		[PreserveSig]
		int GetTypeInfoCount();

		void GetTypeInfo(int index, out UCOMITypeInfo ppTI);

		void GetTypeInfoType(int index, out TYPEKIND pTKind);

		void GetTypeInfoOfGuid(ref Guid guid, out UCOMITypeInfo ppTInfo);

		void GetLibAttr(out IntPtr ppTLibAttr);

		void GetTypeComp(out UCOMITypeComp ppTComp);

		void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);

		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsName([MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, int lHashVal);

		void FindName([MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, int lHashVal, [MarshalAs(UnmanagedType.LPArray)] [Out] UCOMITypeInfo[] ppTInfo, [MarshalAs(UnmanagedType.LPArray)] [Out] int[] rgMemId, ref short pcFound);

		[PreserveSig]
		void ReleaseTLibAttr(IntPtr pTLibAttr);
	}
}
