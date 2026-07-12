using System;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct Rfc3161TimeStampResp
	{
		public PkiStatusInfo Status;

		[AnyValue]
		[OptionalValue]
		public ReadOnlyMemory<byte>? TimeStampToken;
	}
}
