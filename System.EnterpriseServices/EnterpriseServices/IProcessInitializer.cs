using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[Guid("1113f52d-dc7f-4943-aed6-88d04027e32a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IProcessInitializer
	{
		void Shutdown();

		void Startup([MarshalAs(UnmanagedType.IUnknown)] [In] object punkProcessControl);
	}
}
