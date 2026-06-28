using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public enum LayoutKind
	{
		Sequential,
		Explicit = 2,
		Auto
	}
}
