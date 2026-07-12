using System;

namespace System.Runtime.InteropServices
{
	[Obsolete]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("b196b284-bab4-101a-b69c-00aa00341d07")]
	[ComImport]
	public interface UCOMIConnectionPointContainer
	{
		void EnumConnectionPoints(out UCOMIEnumConnectionPoints ppEnum);

		void FindConnectionPoint(ref Guid riid, out UCOMIConnectionPoint ppCP);
	}
}
