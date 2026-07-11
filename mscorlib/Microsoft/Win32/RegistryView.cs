using System;

namespace Microsoft.Win32
{
	[Serializable]
	public enum RegistryView
	{
		Default,
		Registry64 = 256,
		Registry32 = 512
	}
}
