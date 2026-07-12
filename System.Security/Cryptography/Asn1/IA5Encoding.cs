using System;

namespace System.Security.Cryptography.Asn1
{
	internal class IA5Encoding : RestrictedAsciiStringEncoding
	{
		internal IA5Encoding()
			: base(0, 127)
		{
		}
	}
}
