using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[Guid("55e3ea25-55cb-4650-8887-18e8d30bb4bc")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IRegistrationHelper
	{
		void InstallAssembly([MarshalAs(UnmanagedType.BStr)] [In] string assembly, [MarshalAs(UnmanagedType.BStr)] [In] [Out] ref string application, [MarshalAs(UnmanagedType.BStr)] [In] [Out] ref string tlb, [In] InstallationFlags installFlags);

		void UninstallAssembly([MarshalAs(UnmanagedType.BStr)] [In] string assembly, [MarshalAs(UnmanagedType.BStr)] [In] string application);
	}
}
