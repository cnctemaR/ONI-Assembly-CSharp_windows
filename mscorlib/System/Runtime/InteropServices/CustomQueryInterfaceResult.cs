using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(false)]
	[Serializable]
	public enum CustomQueryInterfaceResult
	{
		Handled,
		NotHandled,
		Failed
	}
}
