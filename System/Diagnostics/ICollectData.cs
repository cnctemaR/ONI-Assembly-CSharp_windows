using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("73386977-D6FD-11D2-BED5-00C04F79E3AE")]
	[ComImport]
	public interface ICollectData
	{
		void CloseData();

		[return: MarshalAs(UnmanagedType.I4)]
		void CollectData([MarshalAs(UnmanagedType.I4)] [In] int id, [MarshalAs(UnmanagedType.SysInt)] [In] IntPtr valueName, [MarshalAs(UnmanagedType.SysInt)] [In] IntPtr data, [MarshalAs(UnmanagedType.I4)] [In] int totalBytes, [MarshalAs(UnmanagedType.SysInt)] out IntPtr res);
	}
}
