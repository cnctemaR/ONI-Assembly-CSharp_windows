using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("B196B284-BAB4-101A-B69C-00AA00341D07")]
	[ComImport]
	public interface IConnectionPointContainer
	{
		void EnumConnectionPoints(out IEnumConnectionPoints ppEnum);

		void FindConnectionPoint([In] ref Guid riid, out IConnectionPoint ppCP);
	}
}
