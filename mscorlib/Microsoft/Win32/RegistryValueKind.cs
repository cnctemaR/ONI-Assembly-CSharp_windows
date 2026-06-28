using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
{
	[ComVisible(true)]
	public enum RegistryValueKind
	{
		Unknown,
		String,
		ExpandString,
		Binary,
		DWord,
		MultiString = 7,
		QWord = 11
	}
}
