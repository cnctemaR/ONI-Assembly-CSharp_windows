using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum MethodImplOptions
	{
		Unmanaged = 4,
		ForwardRef = 16,
		InternalCall = 4096,
		Synchronized = 32,
		NoInlining = 8,
		PreserveSig = 128,
		NoOptimization = 64
	}
}
