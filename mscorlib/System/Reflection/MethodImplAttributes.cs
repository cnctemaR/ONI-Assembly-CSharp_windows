using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public enum MethodImplAttributes
	{
		CodeTypeMask = 3,
		IL = 0,
		Native,
		OPTIL,
		Runtime,
		ManagedMask,
		Unmanaged = 4,
		Managed = 0,
		ForwardRef = 16,
		PreserveSig = 128,
		InternalCall = 4096,
		Synchronized = 32,
		NoInlining = 8,
		MaxMethodImplVal = 65535
	}
}
