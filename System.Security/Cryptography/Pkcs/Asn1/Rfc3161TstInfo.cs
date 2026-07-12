using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class Rfc3161TstInfo
	{
		internal int Version;

		[ObjectIdentifier(PopulateFriendlyName = true)]
		internal Oid Policy;

		internal MessageImprint MessageImprint;

		[Integer]
		internal ReadOnlyMemory<byte> SerialNumber;

		[GeneralizedTime(DisallowFractions = false)]
		internal DateTimeOffset GenTime;

		[OptionalValue]
		internal Rfc3161Accuracy? Accuracy;

		[DefaultValue(new byte[] { 1, 1, 0 })]
		internal bool Ordering;

		[Integer]
		[OptionalValue]
		internal ReadOnlyMemory<byte>? Nonce;

		[ExpectedTag(0, ExplicitTag = true)]
		[OptionalValue]
		internal GeneralName? Tsa;

		[ExpectedTag(1)]
		[OptionalValue]
		internal X509ExtensionAsn[] Extensions;
	}
}
