using System;

namespace System.Security.Cryptography
{
	[Flags]
	public enum CngPropertyOptions
	{
		None = 0,
		CustomProperty = 1073741824,
		Persist = -2147483648
	}
}
