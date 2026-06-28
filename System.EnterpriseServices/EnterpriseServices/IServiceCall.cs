using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[Guid("BD3E2E12-42DD-40f4-A09A-95A50C58304B")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IServiceCall
	{
		void OnCall();
	}
}
