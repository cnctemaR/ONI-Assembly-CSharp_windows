using System;

namespace System.Security.Cryptography.Asn1
{
	internal class VisibleStringEncoding : RestrictedAsciiStringEncoding
	{
		internal VisibleStringEncoding()
			: base(32, 126)
		{
		}
	}
}
