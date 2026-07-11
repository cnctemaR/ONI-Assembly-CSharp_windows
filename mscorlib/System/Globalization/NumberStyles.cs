using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum NumberStyles
	{
		None = 0,
		AllowLeadingWhite = 1,
		AllowTrailingWhite = 2,
		AllowLeadingSign = 4,
		AllowTrailingSign = 8,
		AllowParentheses = 16,
		AllowDecimalPoint = 32,
		AllowThousands = 64,
		AllowExponent = 128,
		AllowCurrencySymbol = 256,
		AllowHexSpecifier = 512,
		Integer = 7,
		HexNumber = 515,
		Number = 111,
		Float = 167,
		Currency = 383,
		Any = 511
	}
}
