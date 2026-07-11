using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public enum ComInterfaceType
	{
		InterfaceIsDual,
		InterfaceIsIUnknown,
		InterfaceIsIDispatch
	}
}
