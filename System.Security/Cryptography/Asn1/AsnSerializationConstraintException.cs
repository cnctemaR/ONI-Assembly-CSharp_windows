using System;

namespace System.Security.Cryptography.Asn1
{
	internal class AsnSerializationConstraintException : CryptographicException
	{
		public AsnSerializationConstraintException()
		{
		}

		public AsnSerializationConstraintException(string message)
			: base(message)
		{
		}

		public AsnSerializationConstraintException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
