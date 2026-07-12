using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SignerInfo
	{
		public int Version { get; }

		public SubjectIdentifier SignerIdentifier { get; }

		internal SignerInfo(ref SignerInfoAsn parsedData, SignedCms ownerDocument)
		{
			this.Version = parsedData.Version;
			this.SignerIdentifier = new SubjectIdentifier(parsedData.Sid);
			this._digestAlgorithm = parsedData.DigestAlgorithm.Algorithm;
			this._signedAttributesMemory = parsedData.SignedAttributes;
			this._signatureAlgorithm = parsedData.SignatureAlgorithm.Algorithm;
			this._signatureAlgorithmParameters = parsedData.SignatureAlgorithm.Parameters;
			this._signature = parsedData.SignatureValue;
			this._unsignedAttributes = parsedData.UnsignedAttributes;
			if (this._signedAttributesMemory != null)
			{
				SignedAttributesSet signedAttributesSet = AsnSerializer.Deserialize<SignedAttributesSet>(this._signedAttributesMemory.Value, AsnEncodingRules.BER);
				this._signedAttributes = signedAttributesSet.SignedAttributes;
			}
			this._document = ownerDocument;
		}

		public CryptographicAttributeObjectCollection SignedAttributes
		{
			get
			{
				if (this._parsedSignedAttrs == null)
				{
					this._parsedSignedAttrs = SignerInfo.MakeAttributeCollection(this._signedAttributes);
				}
				return this._parsedSignedAttrs;
			}
		}

		public CryptographicAttributeObjectCollection UnsignedAttributes
		{
			get
			{
				if (this._parsedUnsignedAttrs == null)
				{
					this._parsedUnsignedAttrs = SignerInfo.MakeAttributeCollection(this._unsignedAttributes);
				}
				return this._parsedUnsignedAttrs;
			}
		}

		internal ReadOnlyMemory<byte> GetSignatureMemory()
		{
			return this._signature;
		}

		public byte[] GetSignature()
		{
			return this._signature.ToArray();
		}

		public X509Certificate2 Certificate
		{
			get
			{
				if (this._signerCertificate == null)
				{
					this._signerCertificate = this.FindSignerCertificate();
				}
				return this._signerCertificate;
			}
		}

		public SignerInfoCollection CounterSignerInfos
		{
			get
			{
				if (this._parentSignerInfo != null || this._unsignedAttributes == null || this._unsignedAttributes.Length == 0)
				{
					return new SignerInfoCollection();
				}
				return this.GetCounterSigners(this._unsignedAttributes);
			}
		}

		public Oid DigestAlgorithm
		{
			get
			{
				return new Oid(this._digestAlgorithm);
			}
		}

		public Oid SignatureAlgorithm
		{
			get
			{
				return new Oid(this._signatureAlgorithm);
			}
		}

		private SignerInfoCollection GetCounterSigners(AttributeAsn[] unsignedAttrs)
		{
			List<SignerInfo> list = new List<SignerInfo>();
			foreach (AttributeAsn attributeAsn in unsignedAttrs)
			{
				if (attributeAsn.AttrType.Value == "1.2.840.113549.1.9.6")
				{
					AsnReader asnReader = new AsnReader(attributeAsn.AttrValues, AsnEncodingRules.BER);
					AsnReader asnReader2 = asnReader.ReadSetOf(false);
					if (asnReader.HasData)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
					while (asnReader2.HasData)
					{
						SignerInfoAsn signerInfoAsn = AsnSerializer.Deserialize<SignerInfoAsn>(asnReader2.GetEncodedValue(), AsnEncodingRules.BER);
						SignerInfo signerInfo = new SignerInfo(ref signerInfoAsn, this._document)
						{
							_parentSignerInfo = this
						};
						list.Add(signerInfo);
					}
				}
			}
			return new SignerInfoCollection(list.ToArray());
		}

		public void ComputeCounterSignature()
		{
			throw new PlatformNotSupportedException("No signer certificate was provided. This platform does not implement the certificate picker UI.");
		}

		public void ComputeCounterSignature(CmsSigner signer)
		{
			if (this._parentSignerInfo != null)
			{
				throw new CryptographicException("Only one level of counter-signatures are supported on this platform.");
			}
			if (signer == null)
			{
				throw new ArgumentNullException("signer");
			}
			signer.CheckCertificateValue();
			int num = this._document.SignerInfos.FindIndexForSigner(this);
			if (num < 0)
			{
				throw new CryptographicException("Cannot find the original signer.");
			}
			SignerInfo signerInfo = this._document.SignerInfos[num];
			X509Certificate2Collection x509Certificate2Collection;
			SignerInfoAsn signerInfoAsn = signer.Sign(signerInfo._signature, null, false, out x509Certificate2Collection);
			AttributeAsn attributeAsn;
			using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
			{
				asnWriter.PushSetOf();
				AsnSerializer.Serialize<SignerInfoAsn>(signerInfoAsn, asnWriter);
				asnWriter.PopSetOf();
				attributeAsn = new AttributeAsn
				{
					AttrType = new Oid("1.2.840.113549.1.9.6", "1.2.840.113549.1.9.6"),
					AttrValues = asnWriter.Encode()
				};
			}
			ref SignerInfoAsn ptr = ref this._document.GetRawData().SignerInfos[num];
			int num2;
			if (ptr.UnsignedAttributes == null)
			{
				ptr.UnsignedAttributes = new AttributeAsn[1];
				num2 = 0;
			}
			else
			{
				num2 = ptr.UnsignedAttributes.Length;
				Array.Resize<AttributeAsn>(ref ptr.UnsignedAttributes, num2 + 1);
			}
			ptr.UnsignedAttributes[num2] = attributeAsn;
			this._document.UpdateCertificatesFromAddition(x509Certificate2Collection);
			this._document.Reencode();
		}

		public void RemoveCounterSignature(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("childIndex");
			}
			int num = this._document.SignerInfos.FindIndexForSigner(this);
			if (num < 0)
			{
				throw new CryptographicException("Cannot find the original signer.");
			}
			ref SignerInfoAsn ptr = ref this._document.GetRawData().SignerInfos[num];
			if (ptr.UnsignedAttributes == null)
			{
				throw new CryptographicException("The signed cryptographic message does not have a signer for the specified signer index.");
			}
			int num2 = -1;
			int num3 = -1;
			bool flag = false;
			int num4 = 0;
			AttributeAsn[] unsignedAttributes = ptr.UnsignedAttributes;
			for (int i = 0; i < unsignedAttributes.Length; i++)
			{
				AttributeAsn attributeAsn = unsignedAttributes[i];
				if (attributeAsn.AttrType.Value == "1.2.840.113549.1.9.6")
				{
					AsnReader asnReader = new AsnReader(attributeAsn.AttrValues, AsnEncodingRules.BER);
					AsnReader asnReader2 = asnReader.ReadSetOf(false);
					if (asnReader.HasData)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
					int num5 = 0;
					while (asnReader2.HasData)
					{
						asnReader2.GetEncodedValue();
						if (num4 == index)
						{
							num2 = i;
							num3 = num5;
						}
						num4++;
						num5++;
					}
					if (num3 == 0 && num5 == 1)
					{
						flag = true;
					}
					if (num2 >= 0)
					{
						break;
					}
				}
			}
			if (num2 < 0)
			{
				throw new CryptographicException("The signed cryptographic message does not have a signer for the specified signer index.");
			}
			if (!flag)
			{
				ref AttributeAsn ptr2 = ref unsignedAttributes[num2];
				using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.BER))
				{
					asnWriter.PushSetOf();
					AsnReader asnReader3 = new AsnReader(ptr2.AttrValues, asnWriter.RuleSet);
					AsnReader asnReader4 = asnReader3.ReadSetOf(false);
					asnReader3.ThrowIfNotEmpty();
					int num6 = 0;
					while (asnReader4.HasData)
					{
						ReadOnlyMemory<byte> encodedValue = asnReader4.GetEncodedValue();
						if (num6 != num3)
						{
							asnWriter.WriteEncodedValue(encodedValue);
						}
						num6++;
					}
					asnWriter.PopSetOf();
					ptr2.AttrValues = asnWriter.Encode();
				}
				return;
			}
			if (unsignedAttributes.Length == 1)
			{
				ptr.UnsignedAttributes = null;
				return;
			}
			Internal.Cryptography.Helpers.RemoveAt<AttributeAsn>(ref ptr.UnsignedAttributes, num2);
		}

		public void RemoveCounterSignature(SignerInfo counterSignerInfo)
		{
			if (counterSignerInfo == null)
			{
				throw new ArgumentNullException("counterSignerInfo");
			}
			SignerInfoCollection signerInfos = this._document.SignerInfos;
			int num = signerInfos.FindIndexForSigner(this);
			if (num < 0)
			{
				throw new CryptographicException("Cannot find the original signer.");
			}
			num = signerInfos[num].CounterSignerInfos.FindIndexForSigner(counterSignerInfo);
			if (num < 0)
			{
				throw new CryptographicException("Cannot find the original signer.");
			}
			this.RemoveCounterSignature(num);
		}

		public void CheckSignature(bool verifySignatureOnly)
		{
			this.CheckSignature(new X509Certificate2Collection(), verifySignatureOnly);
		}

		public void CheckSignature(X509Certificate2Collection extraStore, bool verifySignatureOnly)
		{
			if (extraStore == null)
			{
				throw new ArgumentNullException("extraStore");
			}
			X509Certificate2 x509Certificate = this.Certificate;
			if (x509Certificate == null)
			{
				x509Certificate = SignerInfo.FindSignerCertificate(this.SignerIdentifier, extraStore);
				if (x509Certificate == null)
				{
					throw new CryptographicException("Cannot find the original signer.");
				}
			}
			this.Verify(extraStore, x509Certificate, verifySignatureOnly);
		}

		public void CheckHash()
		{
			if (!this.CheckHash(false) && !this.CheckHash(true))
			{
				throw new CryptographicException("Invalid signature.");
			}
		}

		private bool CheckHash(bool compatMode)
		{
			bool flag;
			using (IncrementalHash incrementalHash = this.PrepareDigest(compatMode))
			{
				if (incrementalHash == null)
				{
					flag = false;
				}
				else
				{
					byte[] hashAndReset = incrementalHash.GetHashAndReset();
					flag = this._signature.Span.SequenceEqual<byte>(hashAndReset);
				}
			}
			return flag;
		}

		private X509Certificate2 FindSignerCertificate()
		{
			return SignerInfo.FindSignerCertificate(this.SignerIdentifier, this._document.Certificates);
		}

		private static X509Certificate2 FindSignerCertificate(SubjectIdentifier signerIdentifier, X509Certificate2Collection extraStore)
		{
			if (extraStore == null || extraStore.Count == 0)
			{
				return null;
			}
			X509Certificate2Collection x509Certificate2Collection = null;
			X509Certificate2 x509Certificate = null;
			SubjectIdentifierType type = signerIdentifier.Type;
			if (type != SubjectIdentifierType.IssuerAndSerialNumber)
			{
				if (type == SubjectIdentifierType.SubjectKeyIdentifier)
				{
					x509Certificate2Collection = extraStore.Find(X509FindType.FindBySubjectKeyIdentifier, signerIdentifier.Value, false);
					if (x509Certificate2Collection.Count > 0)
					{
						x509Certificate = x509Certificate2Collection[0];
					}
				}
			}
			else
			{
				X509IssuerSerial x509IssuerSerial = (X509IssuerSerial)signerIdentifier.Value;
				x509Certificate2Collection = extraStore.Find(X509FindType.FindBySerialNumber, x509IssuerSerial.SerialNumber, false);
				foreach (X509Certificate2 x509Certificate2 in x509Certificate2Collection)
				{
					if (x509Certificate2.IssuerName.Name == x509IssuerSerial.IssuerName)
					{
						x509Certificate = x509Certificate2;
						break;
					}
				}
			}
			if (x509Certificate2Collection != null)
			{
				foreach (X509Certificate2 x509Certificate3 in x509Certificate2Collection)
				{
					if (x509Certificate3 != x509Certificate)
					{
						x509Certificate3.Dispose();
					}
				}
			}
			return x509Certificate;
		}

		private IncrementalHash PrepareDigest(bool compatMode)
		{
			IncrementalHash incrementalHash = IncrementalHash.CreateHash(this.GetDigestAlgorithm());
			if (this._parentSignerInfo == null)
			{
				if (this._document.Detached)
				{
					ref SignedDataAsn rawData = ref this._document.GetRawData();
					ReadOnlyMemory<byte>? content = rawData.EncapContentInfo.Content;
					if (content != null)
					{
						incrementalHash.AppendData(SignedCms.GetContent(content.Value, rawData.EncapContentInfo.ContentType).Span);
					}
				}
				incrementalHash.AppendData(this._document.GetHashableContentSpan());
			}
			else
			{
				incrementalHash.AppendData(this._parentSignerInfo._signature.Span);
			}
			bool flag = this._parentSignerInfo != null || this._signedAttributes != null;
			if (this._signedAttributes != null)
			{
				byte[] hashAndReset = incrementalHash.GetHashAndReset();
				using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
				{
					if (compatMode)
					{
						asnWriter.PushSequence();
					}
					else
					{
						asnWriter.PushSetOf();
					}
					foreach (AttributeAsn attributeAsn in this._signedAttributes)
					{
						AsnSerializer.Serialize<AttributeAsn>(attributeAsn, asnWriter);
						if (attributeAsn.AttrType.Value == "1.2.840.113549.1.9.4")
						{
							CryptographicAttributeObject cryptographicAttributeObject = SignerInfo.MakeAttribute(attributeAsn);
							if (cryptographicAttributeObject.Values.Count != 1)
							{
								throw new CryptographicException("The hash value is not correct.");
							}
							Pkcs9MessageDigest pkcs9MessageDigest = (Pkcs9MessageDigest)cryptographicAttributeObject.Values[0];
							if (!hashAndReset.AsSpan<byte>().SequenceEqual<byte>(pkcs9MessageDigest.MessageDigest))
							{
								throw new CryptographicException("The hash value is not correct.");
							}
							flag = false;
						}
					}
					if (compatMode)
					{
						asnWriter.PopSequence();
						byte[] array = asnWriter.Encode();
						array[0] = 49;
						incrementalHash.AppendData(array);
						goto IL_01C4;
					}
					asnWriter.PopSetOf();
					incrementalHash.AppendData(asnWriter.Encode());
					goto IL_01C4;
				}
			}
			if (compatMode)
			{
				return null;
			}
			IL_01C4:
			if (flag)
			{
				throw new CryptographicException("The cryptographic message does not contain an expected authenticated attribute.");
			}
			return incrementalHash;
		}

		private void Verify(X509Certificate2Collection extraStore, X509Certificate2 certificate, bool verifySignatureOnly)
		{
			CmsSignature cmsSignature = CmsSignature.Resolve(this.SignatureAlgorithm.Value);
			if (cmsSignature == null)
			{
				throw new CryptographicException("Unknown algorithm '{0}'.", this.SignatureAlgorithm.Value);
			}
			if (!this.VerifySignature(cmsSignature, certificate, false) && !this.VerifySignature(cmsSignature, certificate, true))
			{
				throw new CryptographicException("Invalid signature.");
			}
			if (!verifySignatureOnly)
			{
				X509Chain x509Chain = new X509Chain();
				x509Chain.ChainPolicy.ExtraStore.AddRange(extraStore);
				x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
				x509Chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
				if (!x509Chain.Build(certificate))
				{
					throw new CryptographicException("Certificate trust could not be established. The first reported error is: {0}", x509Chain.ChainStatus.FirstOrDefault<X509ChainStatus>().StatusInformation);
				}
				foreach (X509Extension x509Extension in certificate.Extensions)
				{
					if (x509Extension.Oid.Value == "2.5.29.15")
					{
						X509KeyUsageExtension x509KeyUsageExtension = x509Extension as X509KeyUsageExtension;
						if (x509KeyUsageExtension == null)
						{
							x509KeyUsageExtension = new X509KeyUsageExtension();
							x509KeyUsageExtension.CopyFrom(x509Extension);
						}
						if ((x509KeyUsageExtension.KeyUsages & (X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature)) == X509KeyUsageFlags.None)
						{
							throw new CryptographicException("The certificate is not valid for the requested usage.");
						}
					}
				}
			}
		}

		private bool VerifySignature(CmsSignature signatureProcessor, X509Certificate2 certificate, bool compatMode)
		{
			bool flag;
			using (IncrementalHash incrementalHash = this.PrepareDigest(compatMode))
			{
				if (incrementalHash == null)
				{
					flag = false;
				}
				else
				{
					byte[] hashAndReset = incrementalHash.GetHashAndReset();
					byte[] array = this._signature.ToArray();
					flag = signatureProcessor.VerifySignature(hashAndReset, array, this.DigestAlgorithm.Value, incrementalHash.AlgorithmName, this._signatureAlgorithmParameters, certificate);
				}
			}
			return flag;
		}

		private HashAlgorithmName GetDigestAlgorithm()
		{
			return Internal.Cryptography.Helpers.GetDigestAlgorithm(this.DigestAlgorithm.Value);
		}

		internal static CryptographicAttributeObjectCollection MakeAttributeCollection(AttributeAsn[] attributes)
		{
			CryptographicAttributeObjectCollection cryptographicAttributeObjectCollection = new CryptographicAttributeObjectCollection();
			if (attributes == null)
			{
				return cryptographicAttributeObjectCollection;
			}
			foreach (AttributeAsn attributeAsn in attributes)
			{
				cryptographicAttributeObjectCollection.AddWithoutMerge(SignerInfo.MakeAttribute(attributeAsn));
			}
			return cryptographicAttributeObjectCollection;
		}

		private static CryptographicAttributeObject MakeAttribute(AttributeAsn attribute)
		{
			Oid oid = new Oid(attribute.AttrType);
			AsnReader asnReader = new AsnReader(attribute.AttrValues, AsnEncodingRules.BER);
			AsnReader asnReader2 = asnReader.ReadSetOf(false);
			if (asnReader.HasData)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			AsnEncodedDataCollection asnEncodedDataCollection = new AsnEncodedDataCollection();
			while (asnReader2.HasData)
			{
				byte[] array = asnReader2.GetEncodedValue().ToArray();
				asnEncodedDataCollection.Add(Internal.Cryptography.Helpers.CreateBestPkcs9AttributeObjectAvailable(oid, array));
			}
			return new CryptographicAttributeObject(oid, asnEncodedDataCollection);
		}

		internal SignerInfo()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly Oid _digestAlgorithm;

		private readonly AttributeAsn[] _signedAttributes;

		private readonly ReadOnlyMemory<byte>? _signedAttributesMemory;

		private readonly Oid _signatureAlgorithm;

		private readonly ReadOnlyMemory<byte>? _signatureAlgorithmParameters;

		private readonly ReadOnlyMemory<byte> _signature;

		private readonly AttributeAsn[] _unsignedAttributes;

		private readonly SignedCms _document;

		private X509Certificate2 _signerCertificate;

		private SignerInfo _parentSignerInfo;

		private CryptographicAttributeObjectCollection _parsedSignedAttrs;

		private CryptographicAttributeObjectCollection _parsedUnsignedAttrs;
	}
}
