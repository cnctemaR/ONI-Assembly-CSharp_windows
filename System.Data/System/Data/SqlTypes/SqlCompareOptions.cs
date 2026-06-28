using System;

namespace System.Data.SqlTypes
{
	[Flags]
	[Serializable]
	public enum SqlCompareOptions
	{
		BinarySort = 32768,
		IgnoreCase = 1,
		IgnoreKanaType = 8,
		IgnoreNonSpace = 2,
		IgnoreWidth = 16,
		None = 0
	}
}
