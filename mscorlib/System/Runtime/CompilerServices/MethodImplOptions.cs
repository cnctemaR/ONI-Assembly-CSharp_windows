using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum MethodImplOptions
	{
		Unmanaged = 4,
		ForwardRef = 16,
		PreserveSig = 128,
		InternalCall = 4096,
		Synchronized = 32,
		NoInlining = 8,
		[ComVisible(false)]
		AggressiveInlining = 256,
		NoOptimization = 64
	}
}
