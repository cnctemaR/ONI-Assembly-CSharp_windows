using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("b196b286-bab4-101a-b69c-00aa00341d07")]
	[ComImport]
	public interface IConnectionPoint
	{
		void GetConnectionInterface(out Guid pIID);

		void GetConnectionPointContainer(out IConnectionPointContainer ppCPC);

		void Advise([MarshalAs(UnmanagedType.Interface)] object pUnkSink, out int pdwCookie);

		void Unadvise(int dwCookie);

		void EnumConnections(out IEnumConnections ppEnum);
	}
}
