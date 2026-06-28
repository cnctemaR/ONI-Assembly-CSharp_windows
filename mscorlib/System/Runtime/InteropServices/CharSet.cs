using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public enum CharSet
	{
		None = 1,
		Ansi,
		Unicode,
		Auto
	}
}
