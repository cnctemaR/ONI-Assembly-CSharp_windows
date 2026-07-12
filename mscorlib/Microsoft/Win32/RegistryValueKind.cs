using System;

namespace Microsoft.Win32
{
	public enum RegistryValueKind
	{
		String = 1,
		ExpandString,
		Binary,
		DWord,
		MultiString = 7,
		QWord = 11,
		Unknown = 0,
		None = -1
	}
}
