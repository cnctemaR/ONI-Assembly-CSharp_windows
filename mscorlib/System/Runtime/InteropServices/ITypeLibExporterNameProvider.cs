using System;

namespace System.Runtime.InteropServices
{
	[Guid("fa1f3615-acb9-486d-9eac-1bef87e36b09")]
	[ComVisible(true)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface ITypeLibExporterNameProvider
	{
		[return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
		string[] GetNames();
	}
}
