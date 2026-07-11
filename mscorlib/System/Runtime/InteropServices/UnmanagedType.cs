using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public enum UnmanagedType
	{
		Bool = 2,
		I1,
		U1,
		I2,
		U2,
		I4,
		U4,
		I8,
		U8,
		R4,
		R8,
		Currency = 15,
		BStr = 19,
		LPStr,
		LPWStr,
		LPTStr,
		ByValTStr,
		IUnknown = 25,
		IDispatch,
		Struct,
		Interface,
		SafeArray,
		ByValArray,
		SysInt,
		SysUInt,
		VBByRefStr = 34,
		AnsiBStr,
		TBStr,
		VariantBool,
		FunctionPtr,
		AsAny = 40,
		LPArray = 42,
		LPStruct,
		CustomMarshaler,
		Error
	}
}
