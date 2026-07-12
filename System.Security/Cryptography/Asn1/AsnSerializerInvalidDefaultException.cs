using System;

namespace System.Security.Cryptography.Asn1
{
	internal class AsnSerializerInvalidDefaultException : AsnSerializationConstraintException
	{
		internal AsnSerializerInvalidDefaultException()
		{
		}

		internal AsnSerializerInvalidDefaultException(Exception innerException)
			: base(string.Empty, innerException)
		{
		}
	}
}
