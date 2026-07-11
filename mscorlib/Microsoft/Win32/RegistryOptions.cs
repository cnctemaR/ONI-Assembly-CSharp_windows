using System;

namespace Microsoft.Win32
{
	[Flags]
	[Serializable]
	public enum RegistryOptions
	{
		None = 0,
		Volatile = 1
	}
}
