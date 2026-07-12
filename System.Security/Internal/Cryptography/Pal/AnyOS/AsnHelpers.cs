using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace Internal.Cryptography.Pal.AnyOS
{
	internal static class AsnHelpers
	{
		internal static SubjectIdentifierOrKey ToSubjectIdentifierOrKey(this OriginatorIdentifierOrKeyAsn originator)
		{
			if (originator.IssuerAndSerialNumber != null)
			{
				IssuerAndSerialNumberAsn issuerAndSerialNumberAsn = originator.IssuerAndSerialNumber.Value;
				X500DistinguishedName x500DistinguishedName = new X500DistinguishedName(issuerAndSerialNumberAsn.Issuer.ToArray());
				SubjectIdentifierOrKeyType subjectIdentifierOrKeyType = SubjectIdentifierOrKeyType.IssuerAndSerialNumber;
				string name = x500DistinguishedName.Name;
				issuerAndSerialNumberAsn = originator.IssuerAndSerialNumber.Value;
				return new SubjectIdentifierOrKey(subjectIdentifierOrKeyType, new X509IssuerSerial(name, issuerAndSerialNumberAsn.SerialNumber.Span.ToBigEndianHex()));
			}
			if (originator.SubjectKeyIdentifier != null)
			{
				return new SubjectIdentifierOrKey(SubjectIdentifierOrKeyType.SubjectKeyIdentifier, originator.SubjectKeyIdentifier.Value.Span.ToBigEndianHex());
			}
			if (originator.OriginatorKey != null)
			{
				OriginatorPublicKeyAsn originatorKey = originator.OriginatorKey;
				return new SubjectIdentifierOrKey(SubjectIdentifierOrKeyType.PublicKeyInfo, new PublicKeyInfo(originatorKey.Algorithm.ToPresentationObject(), originatorKey.PublicKey.ToArray()));
			}
			return new SubjectIdentifierOrKey(SubjectIdentifierOrKeyType.Unknown, string.Empty);
		}

		internal unsafe static AlgorithmIdentifier ToPresentationObject(this AlgorithmIdentifierAsn asn)
		{
			string value = asn.Algorithm.Value;
			int num;
			if (!(value == "1.2.840.113549.3.2"))
			{
				if (!(value == "1.2.840.113549.3.4"))
				{
					if (!(value == "1.3.14.3.2.7"))
					{
						if (!(value == "1.2.840.113549.3.7"))
						{
							num = 0;
						}
						else
						{
							num = 192;
						}
					}
					else
					{
						num = 64;
					}
				}
				else if (asn.Parameters == null)
				{
					num = 0;
				}
				else
				{
					int num2 = 0;
					AsnReader asnReader = new AsnReader(asn.Parameters.Value, AsnEncodingRules.BER);
					if (asnReader.PeekTag() != Asn1Tag.Null)
					{
						ReadOnlyMemory<byte> readOnlyMemory;
						if (asnReader.TryGetPrimitiveOctetStringBytes(out readOnlyMemory))
						{
							num2 = readOnlyMemory.Length;
						}
						else
						{
							Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)16], 16);
							if (!asnReader.TryCopyOctetStringBytes(span, out num2))
							{
								throw new CryptographicException();
							}
						}
					}
					num = 128 - 8 * num2;
				}
			}
			else if (asn.Parameters == null)
			{
				num = 0;
			}
			else
			{
				int effectiveKeyBits = AsnSerializer.Deserialize<Rc2CbcParameters>(asn.Parameters.Value, AsnEncodingRules.BER).GetEffectiveKeyBits();
				if (effectiveKeyBits <= 56)
				{
					if (effectiveKeyBits != 40 && effectiveKeyBits != 56)
					{
						goto IL_00A3;
					}
				}
				else if (effectiveKeyBits != 64 && effectiveKeyBits != 128)
				{
					goto IL_00A3;
				}
				num = effectiveKeyBits;
				goto IL_0139;
				IL_00A3:
				num = 0;
			}
			IL_0139:
			return new AlgorithmIdentifier(new Oid(asn.Algorithm), num);
		}
	}
}
