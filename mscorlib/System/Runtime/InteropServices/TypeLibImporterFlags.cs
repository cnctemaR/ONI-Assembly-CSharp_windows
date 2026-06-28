using System;

namespace System.Runtime.InteropServices
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum TypeLibImporterFlags
	{
		PrimaryInteropAssembly = 1,
		UnsafeInterfaces = 2,
		SafeArrayAsSystemArray = 4,
		TransformDispRetVals = 8,
		None = 0,
		PreventClassMembers = 16,
		ImportAsAgnostic = 2048,
		ImportAsItanium = 1024,
		ImportAsX64 = 512,
		ImportAsX86 = 256,
		ReflectionOnlyLoading = 4096,
		SerializableValueClasses = 32
	}
}
