using System;

namespace System.Reflection
{
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
		AggressiveInlining = 256,
		NoOptimization = 64,
		MaxMethodImplVal = 65535,
		SecurityMitigations = 1024
	}
}
