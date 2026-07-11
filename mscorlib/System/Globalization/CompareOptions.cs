using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum CompareOptions
	{
		None = 0,
		IgnoreCase = 1,
		IgnoreNonSpace = 2,
		IgnoreSymbols = 4,
		IgnoreKanaType = 8,
		IgnoreWidth = 16,
		OrdinalIgnoreCase = 268435456,
		StringSort = 536870912,
		Ordinal = 1073741824
	}
}
