using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum CompareOptions
	{
		None = 0,
		IgnoreCase = 1,
		IgnoreNonSpace = 2,
		IgnoreSymbols = 4,
		IgnoreKanaType = 8,
		IgnoreWidth = 16,
		StringSort = 536870912,
		Ordinal = 1073741824,
		OrdinalIgnoreCase = 268435456
	}
}
