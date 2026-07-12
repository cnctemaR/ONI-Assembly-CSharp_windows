using System;

namespace System.Security.Cryptography.Asn1
{
	internal enum TagClass : byte
	{
		Universal,
		Application = 64,
		ContextSpecific = 128,
		Private = 192
	}
}
