using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00020412-0000-0000-C000-000000000046")]
	[ComImport]
	public interface ITypeInfo2 : ITypeInfo
	{
		void AddressOfMember(int memid, INVOKEKIND invKind, out IntPtr ppv);

		void CreateInstance([MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

		void GetContainingTypeLib(out ITypeLib ppTLB, out int pIndex);

		void GetDllEntry(int memid, INVOKEKIND invKind, IntPtr pBstrDllName, IntPtr pBstrName, IntPtr pwOrdinal);

		void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile);

		void GetIDsOfNames([MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeConst = 0, SizeParamIndex = 1)] [In] string[] rgszNames, int cNames, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 1)] [Out] int[] pMemId);

		void GetImplTypeFlags(int index, out IMPLTYPEFLAGS pImplTypeFlags);

		void GetTypeKind(out TYPEKIND pTypeKind);

		void GetTypeFlags(out int pTypeFlags);

		void GetFuncDesc(int index, out IntPtr ppFuncDesc);

		void GetMops(int memid, out string pBstrMops);

		void GetNames(int memid, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] [Out] string[] rgBstrNames, int cMaxNames, out int pcNames);

		void GetRefTypeInfo(int hRef, out ITypeInfo ppTI);

		void GetRefTypeOfImplType(int index, out int href);

		void GetTypeAttr(out IntPtr ppTypeAttr);

		void GetTypeComp(out ITypeComp ppTComp);

		void GetVarDesc(int index, out IntPtr ppVarDesc);

		void GetFuncIndexOfMemId(int memid, INVOKEKIND invKind, out int pFuncIndex);

		void GetVarIndexOfMemId(int memid, out int pVarIndex);

		void GetCustData(ref Guid guid, out object pVarVal);

		void GetFuncCustData(int index, ref Guid guid, out object pVarVal);

		void GetParamCustData(int indexFunc, int indexParam, ref Guid guid, out object pVarVal);

		void GetVarCustData(int index, ref Guid guid, out object pVarVal);

		void GetImplTypeCustData(int index, ref Guid guid, out object pVarVal);

		[LCIDConversion(1)]
		void GetDocumentation2(int memid, out string pbstrHelpString, out int pdwHelpStringContext, out string pbstrHelpStringDll);

		void GetAllCustData(IntPtr pCustData);

		void GetAllFuncCustData(int index, IntPtr pCustData);

		void GetAllParamCustData(int indexFunc, int indexParam, IntPtr pCustData);

		void GetAllVarCustData(int index, IntPtr pCustData);

		void GetAllImplTypeCustData(int index, IntPtr pCustData);

		void Invoke([MarshalAs(UnmanagedType.IUnknown)] object pvInstance, int memid, short wFlags, ref DISPPARAMS pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, out int puArgErr);

		[PreserveSig]
		void ReleaseTypeAttr(IntPtr pTypeAttr);

		[PreserveSig]
		void ReleaseFuncDesc(IntPtr pFuncDesc);

		[PreserveSig]
		void ReleaseVarDesc(IntPtr pVarDesc);
	}
}
