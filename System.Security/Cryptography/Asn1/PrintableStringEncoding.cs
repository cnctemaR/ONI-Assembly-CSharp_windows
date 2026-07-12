using System;

namespace System.Security.Cryptography.Asn1
{
	internal class PrintableStringEncoding : RestrictedAsciiStringEncoding
	{
		internal PrintableStringEncoding()
			: base("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 '()+,-./:=?")
		{
		}
	}
}
