using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("BD3E2E12-42DD-40f4-A09A-95A50C58304B")]
	[ComImport]
	public interface IServiceCall
	{
		void OnCall();
	}
}
