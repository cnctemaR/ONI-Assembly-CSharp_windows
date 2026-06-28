using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[Guid("00020411-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface ITypeLib2 : ITypeLib
	{
		void FindName([MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, int lHashVal, [MarshalAs(UnmanagedType.LPArray)] [Out] ITypeInfo[] ppTInfo, [MarshalAs(UnmanagedType.LPArray)] [Out] int[] rgMemId, ref short pcFound);

		void GetCustData(ref Guid guid, out object pVarVal);

		void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);

		void GetLibAttr(out IntPtr ppTLibAttr);

		void GetLibStatistics(IntPtr pcUniqueNames, out int pcchUniqueNames);

		[LCIDConversion(1)]
		void GetDocumentation2(int index, out string pbstrHelpString, out int pdwHelpStringContext, out string pbstrHelpStringDll);

		void GetAllCustData(IntPtr pCustData);

		void GetTypeComp(out ITypeComp ppTComp);

		void GetTypeInfo(int index, out ITypeInfo ppTI);

		void GetTypeInfoOfGuid(ref Guid guid, out ITypeInfo ppTInfo);

		void GetTypeInfoType(int index, out TYPEKIND pTKind);

		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsName([MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, int lHashVal);

		[PreserveSig]
		void ReleaseTLibAttr(IntPtr pTLibAttr);

		[PreserveSig]
		int GetTypeInfoCount();
	}
}
