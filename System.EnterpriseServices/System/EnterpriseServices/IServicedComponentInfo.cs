using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("8165B19E-8D3A-4d0b-80C8-97DE310DB583")]
	public interface IServicedComponentInfo
	{
		void GetComponentInfo(ref int infoMask, out string[] infoArray);
	}
}
