using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum DateTimeStyles
	{
		None = 0,
		AllowLeadingWhite = 1,
		AllowTrailingWhite = 2,
		AllowInnerWhite = 4,
		AllowWhiteSpaces = 7,
		NoCurrentDateDefault = 8,
		AdjustToUniversal = 16,
		AssumeLocal = 32,
		AssumeUniversal = 64,
		RoundtripKind = 128
	}
}
