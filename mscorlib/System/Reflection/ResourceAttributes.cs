using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum ResourceAttributes
	{
		Public = 1,
		Private = 2
	}
}
