using System;
using System.Reflection.Emit;

namespace System.Runtime.InteropServices
{
	[TypeLibImportClass(typeof(AssemblyBuilder))]
	[ComVisible(true)]
	[CLSCompliant(false)]
	[Guid("BEBB2505-8B54-3443-AEAD-142A16DD9CC7")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface _AssemblyBuilder
	{
		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
