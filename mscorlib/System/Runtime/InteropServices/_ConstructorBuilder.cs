using System;
using System.Reflection.Emit;

namespace System.Runtime.InteropServices
{
	[CLSCompliant(false)]
	[TypeLibImportClass(typeof(ConstructorBuilder))]
	[Guid("ED3E4384-D7E2-3FA7-8FFD-8940D330519A")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComVisible(true)]
	public interface _ConstructorBuilder
	{
		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
