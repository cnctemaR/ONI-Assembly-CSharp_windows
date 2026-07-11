using System;

namespace System.Runtime.InteropServices
{
	[Guid("b196b286-bab4-101a-b69c-00aa00341d07")]
	[Obsolete]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface UCOMIConnectionPoint
	{
		void GetConnectionInterface(out Guid pIID);

		void GetConnectionPointContainer(out UCOMIConnectionPointContainer ppCPC);

		void Advise([MarshalAs(UnmanagedType.Interface)] object pUnkSink, out int pdwCookie);

		void Unadvise(int dwCookie);

		void EnumConnections(out UCOMIEnumConnections ppEnum);
	}
}
