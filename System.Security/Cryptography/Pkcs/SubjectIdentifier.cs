using System;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SubjectIdentifier
	{
		internal SubjectIdentifier(SubjectIdentifierType type, object value)
		{
			this.Type = type;
			this.Value = value;
		}

		internal SubjectIdentifier(SignerIdentifierAsn signerIdentifierAsn)
			: this(signerIdentifierAsn.IssuerAndSerialNumber, signerIdentifierAsn.SubjectKeyIdentifier)
		{
		}

		internal unsafe SubjectIdentifier(IssuerAndSerialNumberAsn? issuerAndSerialNumber, ReadOnlyMemory<byte>? subjectKeyIdentifier)
		{
			if (issuerAndSerialNumber != null)
			{
				IssuerAndSerialNumberAsn issuerAndSerialNumberAsn = issuerAndSerialNumber.Value;
				ReadOnlySpan<byte> span = issuerAndSerialNumberAsn.Issuer.Span;
				issuerAndSerialNumberAsn = issuerAndSerialNumber.Value;
				ReadOnlySpan<byte> span2 = issuerAndSerialNumberAsn.SerialNumber.Span;
				bool flag = false;
				for (int i = 0; i < span2.Length; i++)
				{
					if (*span2[i] != 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag && SubjectIdentifier.DummySignerEncodedValue.AsSpan<byte>().SequenceEqual<byte>(span))
				{
					this.Type = SubjectIdentifierType.NoSignature;
					this.Value = null;
					return;
				}
				this.Type = SubjectIdentifierType.IssuerAndSerialNumber;
				X500DistinguishedName x500DistinguishedName = new X500DistinguishedName(span.ToArray());
				this.Value = new X509IssuerSerial(x500DistinguishedName.Name, span2.ToBigEndianHex());
				return;
			}
			else
			{
				if (subjectKeyIdentifier != null)
				{
					this.Type = SubjectIdentifierType.SubjectKeyIdentifier;
					this.Value = subjectKeyIdentifier.Value.Span.ToBigEndianHex();
					return;
				}
				throw new CryptographicException();
			}
		}

		public SubjectIdentifierType Type { get; }

		public object Value { get; }

		internal SubjectIdentifier()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private const string DummySignerSubjectName = "CN=Dummy Signer";

		internal static readonly byte[] DummySignerEncodedValue = new X500DistinguishedName("CN=Dummy Signer").RawData;
	}
}
