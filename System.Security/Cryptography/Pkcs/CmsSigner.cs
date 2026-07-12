using System;
using System.Collections.Generic;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class CmsSigner
	{
		public X509Certificate2 Certificate { get; set; }

		public AsymmetricAlgorithm PrivateKey { get; set; }

		public X509Certificate2Collection Certificates { get; private set; }

		public Oid DigestAlgorithm { get; set; }

		public X509IncludeOption IncludeOption { get; set; }

		public CryptographicAttributeObjectCollection SignedAttributes { get; private set; }

		public CryptographicAttributeObjectCollection UnsignedAttributes { get; private set; }

		public SubjectIdentifierType SignerIdentifierType
		{
			get
			{
				return this._signerIdentifierType;
			}
			set
			{
				if (value < SubjectIdentifierType.IssuerAndSerialNumber || value > SubjectIdentifierType.NoSignature)
				{
					throw new ArgumentException(SR.Format("The subject identifier type {0} is not valid.", value));
				}
				this._signerIdentifierType = value;
			}
		}

		public CmsSigner()
			: this(SubjectIdentifierType.IssuerAndSerialNumber, null)
		{
		}

		public CmsSigner(SubjectIdentifierType signerIdentifierType)
			: this(signerIdentifierType, null)
		{
		}

		public CmsSigner(X509Certificate2 certificate)
			: this(SubjectIdentifierType.IssuerAndSerialNumber, certificate)
		{
		}

		public CmsSigner(CspParameters parameters)
		{
			this.Certificates = new X509Certificate2Collection();
			this.SignedAttributes = new CryptographicAttributeObjectCollection();
			this.UnsignedAttributes = new CryptographicAttributeObjectCollection();
			base..ctor();
			throw new PlatformNotSupportedException();
		}

		public CmsSigner(SubjectIdentifierType signerIdentifierType, X509Certificate2 certificate)
		{
			this.Certificates = new X509Certificate2Collection();
			this.SignedAttributes = new CryptographicAttributeObjectCollection();
			this.UnsignedAttributes = new CryptographicAttributeObjectCollection();
			base..ctor();
			switch (signerIdentifierType)
			{
			case SubjectIdentifierType.Unknown:
				this._signerIdentifierType = SubjectIdentifierType.IssuerAndSerialNumber;
				this.IncludeOption = X509IncludeOption.ExcludeRoot;
				break;
			case SubjectIdentifierType.IssuerAndSerialNumber:
				this._signerIdentifierType = signerIdentifierType;
				this.IncludeOption = X509IncludeOption.ExcludeRoot;
				break;
			case SubjectIdentifierType.SubjectKeyIdentifier:
				this._signerIdentifierType = signerIdentifierType;
				this.IncludeOption = X509IncludeOption.ExcludeRoot;
				break;
			case SubjectIdentifierType.NoSignature:
				this._signerIdentifierType = signerIdentifierType;
				this.IncludeOption = X509IncludeOption.None;
				break;
			default:
				this._signerIdentifierType = SubjectIdentifierType.IssuerAndSerialNumber;
				this.IncludeOption = X509IncludeOption.ExcludeRoot;
				break;
			}
			this.Certificate = certificate;
			this.DigestAlgorithm = new Oid(CmsSigner.s_defaultAlgorithm);
		}

		internal void CheckCertificateValue()
		{
			if (this.SignerIdentifierType == SubjectIdentifierType.NoSignature)
			{
				return;
			}
			if (this.Certificate == null)
			{
				throw new PlatformNotSupportedException("No signer certificate was provided. This platform does not implement the certificate picker UI.");
			}
			if (!this.Certificate.HasPrivateKey)
			{
				throw new CryptographicException("A certificate with a private key is required.");
			}
		}

		internal SignerInfoAsn Sign(ReadOnlyMemory<byte> data, string contentTypeOid, bool silent, out X509Certificate2Collection chainCerts)
		{
			HashAlgorithmName digestAlgorithm = Internal.Cryptography.Helpers.GetDigestAlgorithm(this.DigestAlgorithm);
			IncrementalHash hasher = IncrementalHash.CreateHash(digestAlgorithm);
			hasher.AppendData(data.Span);
			byte[] array = hasher.GetHashAndReset();
			SignerInfoAsn signerInfoAsn = default(SignerInfoAsn);
			signerInfoAsn.DigestAlgorithm.Algorithm = this.DigestAlgorithm;
			CryptographicAttributeObjectCollection signedAttributes = this.SignedAttributes;
			if ((signedAttributes != null && signedAttributes.Count > 0) || contentTypeOid != "1.2.840.113549.1.7.1")
			{
				List<AttributeAsn> list = CmsSigner.BuildAttributes(this.SignedAttributes);
				using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
				{
					asnWriter.PushSetOf();
					asnWriter.WriteOctetString(array);
					asnWriter.PopSetOf();
					list.Add(new AttributeAsn
					{
						AttrType = new Oid("1.2.840.113549.1.9.4", "1.2.840.113549.1.9.4"),
						AttrValues = asnWriter.Encode()
					});
				}
				if (contentTypeOid != null)
				{
					using (AsnWriter asnWriter2 = new AsnWriter(AsnEncodingRules.DER))
					{
						asnWriter2.PushSetOf();
						asnWriter2.WriteObjectIdentifier(contentTypeOid);
						asnWriter2.PopSetOf();
						list.Add(new AttributeAsn
						{
							AttrType = new Oid("1.2.840.113549.1.9.3", "1.2.840.113549.1.9.3"),
							AttrValues = asnWriter2.Encode()
						});
					}
				}
				using (AsnWriter asnWriter3 = AsnSerializer.Serialize<SignedAttributesSet>(new SignedAttributesSet
				{
					SignedAttributes = Internal.Cryptography.Helpers.NormalizeSet<AttributeAsn>(list.ToArray(), delegate(byte[] normalized)
					{
						AsnReader asnReader = new AsnReader(normalized, AsnEncodingRules.DER);
						hasher.AppendData(asnReader.PeekContentBytes().Span);
					})
				}, AsnEncodingRules.BER))
				{
					signerInfoAsn.SignedAttributes = new ReadOnlyMemory<byte>?(asnWriter3.Encode());
				}
				array = hasher.GetHashAndReset();
			}
			switch (this.SignerIdentifierType)
			{
			case SubjectIdentifierType.IssuerAndSerialNumber:
			{
				byte[] serialNumber = this.Certificate.GetSerialNumber();
				Array.Reverse<byte>(serialNumber);
				signerInfoAsn.Sid.IssuerAndSerialNumber = new IssuerAndSerialNumberAsn?(new IssuerAndSerialNumberAsn
				{
					Issuer = this.Certificate.IssuerName.RawData,
					SerialNumber = serialNumber
				});
				signerInfoAsn.Version = 1;
				break;
			}
			case SubjectIdentifierType.SubjectKeyIdentifier:
				signerInfoAsn.Sid.SubjectKeyIdentifier = new ReadOnlyMemory<byte>?(this.Certificate.GetSubjectKeyIdentifier());
				signerInfoAsn.Version = 3;
				break;
			case SubjectIdentifierType.NoSignature:
				signerInfoAsn.Sid.IssuerAndSerialNumber = new IssuerAndSerialNumberAsn?(new IssuerAndSerialNumberAsn
				{
					Issuer = SubjectIdentifier.DummySignerEncodedValue,
					SerialNumber = new byte[1]
				});
				signerInfoAsn.Version = 1;
				break;
			default:
				throw new CryptographicException();
			}
			if (this.UnsignedAttributes != null && this.UnsignedAttributes.Count > 0)
			{
				List<AttributeAsn> list2 = CmsSigner.BuildAttributes(this.UnsignedAttributes);
				signerInfoAsn.UnsignedAttributes = Internal.Cryptography.Helpers.NormalizeSet<AttributeAsn>(list2.ToArray(), null);
			}
			Oid oid;
			ReadOnlyMemory<byte> readOnlyMemory;
			if (!CmsSignature.Sign(array, digestAlgorithm, this.Certificate, silent, out oid, out readOnlyMemory))
			{
				throw new CryptographicException("Could not determine signature algorithm for the signer certificate.");
			}
			signerInfoAsn.SignatureValue = readOnlyMemory;
			signerInfoAsn.SignatureAlgorithm.Algorithm = oid;
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			x509Certificate2Collection.AddRange(this.Certificates);
			if (this.SignerIdentifierType != SubjectIdentifierType.NoSignature)
			{
				if (this.IncludeOption == X509IncludeOption.EndCertOnly)
				{
					x509Certificate2Collection.Add(this.Certificate);
				}
				else if (this.IncludeOption != X509IncludeOption.None)
				{
					X509Chain x509Chain = new X509Chain();
					x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
					x509Chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					if (!x509Chain.Build(this.Certificate))
					{
						foreach (X509ChainStatus x509ChainStatus in x509Chain.ChainStatus)
						{
							if (x509ChainStatus.Status == X509ChainStatusFlags.PartialChain)
							{
								throw new CryptographicException("The certificate chain is incomplete, the self-signed root authority could not be determined.");
							}
						}
					}
					X509ChainElementCollection chainElements = x509Chain.ChainElements;
					int count = chainElements.Count;
					int num = count - 1;
					if (num == 0)
					{
						num = -1;
					}
					for (int j = 0; j < count; j++)
					{
						X509Certificate2 certificate = chainElements[j].Certificate;
						if (j == num && this.IncludeOption == X509IncludeOption.ExcludeRoot && certificate.SubjectName.RawData.AsSpan<byte>().SequenceEqual<byte>(certificate.IssuerName.RawData))
						{
							break;
						}
						x509Certificate2Collection.Add(certificate);
					}
				}
			}
			chainCerts = x509Certificate2Collection;
			return signerInfoAsn;
		}

		internal static List<AttributeAsn> BuildAttributes(CryptographicAttributeObjectCollection attributes)
		{
			List<AttributeAsn> list = new List<AttributeAsn>();
			if (attributes == null || attributes.Count == 0)
			{
				return list;
			}
			foreach (CryptographicAttributeObject cryptographicAttributeObject in attributes)
			{
				using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
				{
					asnWriter.PushSetOf();
					foreach (AsnEncodedData asnEncodedData in cryptographicAttributeObject.Values)
					{
						asnWriter.WriteEncodedValue(asnEncodedData.RawData);
					}
					asnWriter.PopSetOf();
					AttributeAsn attributeAsn = new AttributeAsn
					{
						AttrType = cryptographicAttributeObject.Oid,
						AttrValues = asnWriter.Encode()
					};
					list.Add(attributeAsn);
				}
			}
			return list;
		}

		private static readonly Oid s_defaultAlgorithm = Oid.FromOidValue("1.3.14.3.2.26", OidGroup.HashAlgorithm);

		private SubjectIdentifierType _signerIdentifierType;
	}
}
