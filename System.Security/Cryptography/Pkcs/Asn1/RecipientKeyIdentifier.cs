using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class RecipientKeyIdentifier
	{
		[OctetString]
		internal ReadOnlyMemory<byte> SubjectKeyIdentifier;

		[OptionalValue]
		[GeneralizedTime]
		internal DateTimeOffset? Date;

		[OptionalValue]
		internal OtherKeyAttributeAsn? Other;
	}
}
