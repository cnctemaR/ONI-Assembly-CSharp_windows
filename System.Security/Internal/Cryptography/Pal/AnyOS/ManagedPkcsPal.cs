using System;
using System.Buffers;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;

namespace Internal.Cryptography.Pal.AnyOS
{
	internal sealed class ManagedPkcsPal : PkcsPal
	{
		public override byte[] EncodeOctetString(byte[] octets)
		{
			byte[] array;
			using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
			{
				asnWriter.WriteOctetString(octets);
				array = asnWriter.Encode();
			}
			return array;
		}

		public unsafe override byte[] DecodeOctetString(byte[] encodedOctets)
		{
			AsnReader asnReader = new AsnReader(encodedOctets, AsnEncodingRules.BER);
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)256], 256);
			ReadOnlySpan<byte> readOnlySpan = default(Span<byte>);
			byte[] array = null;
			byte[] array2;
			try
			{
				ReadOnlyMemory<byte> readOnlyMemory;
				if (!asnReader.TryGetPrimitiveOctetStringBytes(out readOnlyMemory))
				{
					int num;
					if (asnReader.TryCopyOctetStringBytes(span, out num))
					{
						readOnlySpan = span.Slice(0, num);
					}
					else
					{
						array = ArrayPool<byte>.Shared.Rent(asnReader.PeekContentBytes().Length);
						if (!asnReader.TryCopyOctetStringBytes(array, out num))
						{
							throw new CryptographicException();
						}
						readOnlySpan = new ReadOnlySpan<byte>(array, 0, num);
					}
				}
				else
				{
					readOnlySpan = readOnlyMemory.Span;
				}
				if (asnReader.HasData)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				array2 = readOnlySpan.ToArray();
			}
			finally
			{
				if (array != null)
				{
					Array.Clear(array, 0, readOnlySpan.Length);
					ArrayPool<byte>.Shared.Return(array, false);
				}
			}
			return array2;
		}

		public override byte[] EncodeUtcTime(DateTime utcTime)
		{
			byte[] array;
			using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER))
			{
				try
				{
					if (utcTime.Kind == DateTimeKind.Unspecified)
					{
						asnWriter.WriteUtcTime(utcTime.ToLocalTime(), 1950);
					}
					else
					{
						asnWriter.WriteUtcTime(utcTime, 1950);
					}
					array = asnWriter.Encode();
				}
				catch (ArgumentException ex)
				{
					throw new CryptographicException(ex.Message, ex);
				}
			}
			return array;
		}

		public override DateTime DecodeUtcTime(byte[] encodedUtcTime)
		{
			AsnReader asnReader = new AsnReader(encodedUtcTime, AsnEncodingRules.BER);
			DateTimeOffset utcTime = asnReader.GetUtcTime(2049);
			if (asnReader.HasData)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			return utcTime.UtcDateTime;
		}

		public override string DecodeOid(byte[] encodedOid)
		{
			if (ManagedPkcsPal.s_invalidEmptyOid.AsSpan<byte>().SequenceEqual<byte>(encodedOid))
			{
				return string.Empty;
			}
			AsnReader asnReader = new AsnReader(encodedOid, AsnEncodingRules.BER);
			string text = asnReader.ReadObjectIdentifierAsString();
			if (asnReader.HasData)
			{
				throw new CryptographicException("ASN1 corrupted data.");
			}
			return text;
		}

		public override Oid GetEncodedMessageType(byte[] encodedMessage)
		{
			ContentInfoAsn contentInfoAsn = AsnSerializer.Deserialize<ContentInfoAsn>(new AsnReader(encodedMessage, AsnEncodingRules.BER).GetEncodedValue(), AsnEncodingRules.BER);
			string contentType = contentInfoAsn.ContentType;
			if (contentType == "1.2.840.113549.1.7.1" || contentType == "1.2.840.113549.1.7.2" || contentType == "1.2.840.113549.1.7.3" || contentType == "1.2.840.113549.1.7.4" || contentType == "1.2.840.113549.1.7.5" || contentType == "1.2.840.113549.1.7.6")
			{
				return new Oid(contentInfoAsn.ContentType);
			}
			throw new CryptographicException("Invalid cryptographic message type.");
		}

		public override DecryptorPal Decode(byte[] encodedMessage, out int version, out ContentInfo contentInfo, out AlgorithmIdentifier contentEncryptionAlgorithm, out X509Certificate2Collection originatorCerts, out CryptographicAttributeObjectCollection unprotectedAttributes)
		{
			ContentInfoAsn contentInfoAsn = AsnSerializer.Deserialize<ContentInfoAsn>(new AsnReader(encodedMessage, AsnEncodingRules.BER).GetEncodedValue(), AsnEncodingRules.BER);
			if (contentInfoAsn.ContentType != "1.2.840.113549.1.7.3")
			{
				throw new CryptographicException("Invalid cryptographic message type.");
			}
			byte[] array = contentInfoAsn.Content.ToArray();
			EnvelopedDataAsn envelopedDataAsn = AsnSerializer.Deserialize<EnvelopedDataAsn>(array, AsnEncodingRules.BER);
			version = envelopedDataAsn.Version;
			contentInfo = new ContentInfo(new Oid(envelopedDataAsn.EncryptedContentInfo.ContentType), ((envelopedDataAsn.EncryptedContentInfo.EncryptedContent != null) ? envelopedDataAsn.EncryptedContentInfo.EncryptedContent.GetValueOrDefault().ToArray() : null) ?? Array.Empty<byte>());
			contentEncryptionAlgorithm = envelopedDataAsn.EncryptedContentInfo.ContentEncryptionAlgorithm.ToPresentationObject();
			originatorCerts = new X509Certificate2Collection();
			if (envelopedDataAsn.OriginatorInfo != null && envelopedDataAsn.OriginatorInfo.CertificateSet != null)
			{
				foreach (CertificateChoiceAsn certificateChoiceAsn in envelopedDataAsn.OriginatorInfo.CertificateSet)
				{
					if (certificateChoiceAsn.Certificate != null)
					{
						originatorCerts.Add(new X509Certificate2(certificateChoiceAsn.Certificate.Value.ToArray()));
					}
				}
			}
			unprotectedAttributes = SignerInfo.MakeAttributeCollection(envelopedDataAsn.UnprotectedAttributes);
			List<RecipientInfo> list = new List<RecipientInfo>();
			foreach (RecipientInfoAsn recipientInfoAsn in envelopedDataAsn.RecipientInfos)
			{
				if (recipientInfoAsn.Ktri != null)
				{
					list.Add(new KeyTransRecipientInfo(new ManagedPkcsPal.ManagedKeyTransPal(recipientInfoAsn.Ktri)));
				}
				else
				{
					if (recipientInfoAsn.Kari == null)
					{
						throw new CryptographicException();
					}
					for (int j = 0; j < recipientInfoAsn.Kari.RecipientEncryptedKeys.Length; j++)
					{
						list.Add(new KeyAgreeRecipientInfo(new ManagedPkcsPal.ManagedKeyAgreePal(recipientInfoAsn.Kari, j)));
					}
				}
			}
			return new ManagedPkcsPal.ManagedDecryptorPal(array, envelopedDataAsn, new RecipientInfoCollection(list));
		}

		public unsafe override byte[] Encrypt(CmsRecipientCollection recipients, ContentInfo contentInfo, AlgorithmIdentifier contentEncryptionAlgorithm, X509Certificate2Collection originatorCerts, CryptographicAttributeObjectCollection unprotectedAttributes)
		{
			byte[] array2;
			byte[] array3;
			byte[] array = this.EncryptContent(contentInfo, contentEncryptionAlgorithm, out array2, out array3);
			byte[] array4;
			if ((array4 = array2) == null || array4.Length == 0)
			{
				byte* ptr = null;
			}
			else
			{
				byte* ptr = &array4[0];
			}
			byte[] array5;
			try
			{
				array5 = ManagedPkcsPal.Encrypt(recipients, contentInfo, contentEncryptionAlgorithm, originatorCerts, unprotectedAttributes, array, array2, array3);
			}
			finally
			{
				Array.Clear(array2, 0, array2.Length);
			}
			return array5;
		}

		private static byte[] Encrypt(CmsRecipientCollection recipients, ContentInfo contentInfo, AlgorithmIdentifier contentEncryptionAlgorithm, X509Certificate2Collection originatorCerts, CryptographicAttributeObjectCollection unprotectedAttributes, byte[] encryptedContent, byte[] cek, byte[] parameterBytes)
		{
			EnvelopedDataAsn envelopedDataAsn = default(EnvelopedDataAsn);
			envelopedDataAsn.EncryptedContentInfo.ContentType = contentInfo.ContentType.Value;
			envelopedDataAsn.EncryptedContentInfo.ContentEncryptionAlgorithm.Algorithm = contentEncryptionAlgorithm.Oid;
			envelopedDataAsn.EncryptedContentInfo.ContentEncryptionAlgorithm.Parameters = new ReadOnlyMemory<byte>?(parameterBytes);
			envelopedDataAsn.EncryptedContentInfo.EncryptedContent = new ReadOnlyMemory<byte>?(encryptedContent);
			EnvelopedDataAsn envelopedDataAsn2 = envelopedDataAsn;
			if (unprotectedAttributes != null && unprotectedAttributes.Count > 0)
			{
				List<AttributeAsn> list = CmsSigner.BuildAttributes(unprotectedAttributes);
				envelopedDataAsn2.UnprotectedAttributes = Helpers.NormalizeSet<AttributeAsn>(list.ToArray(), null);
			}
			if (originatorCerts != null && originatorCerts.Count > 0)
			{
				CertificateChoiceAsn[] array = new CertificateChoiceAsn[originatorCerts.Count];
				for (int i = 0; i < originatorCerts.Count; i++)
				{
					array[i].Certificate = new ReadOnlyMemory<byte>?(originatorCerts[i].RawData);
				}
				envelopedDataAsn2.OriginatorInfo = new OriginatorInfoAsn
				{
					CertificateSet = array
				};
			}
			envelopedDataAsn2.RecipientInfos = new RecipientInfoAsn[recipients.Count];
			bool flag = true;
			for (int j = 0; j < recipients.Count; j++)
			{
				CmsRecipient cmsRecipient = recipients[j];
				if (!(cmsRecipient.Certificate.GetKeyAlgorithm() == "1.2.840.113549.1.1.1"))
				{
					throw new CryptographicException("Unknown algorithm '{0}'.", cmsRecipient.Certificate.GetKeyAlgorithm());
				}
				bool flag2;
				envelopedDataAsn2.RecipientInfos[j].Ktri = ManagedPkcsPal.MakeKtri(cek, cmsRecipient, out flag2);
				flag = flag && flag2;
			}
			if (envelopedDataAsn2.OriginatorInfo != null || !flag || envelopedDataAsn2.UnprotectedAttributes != null)
			{
				envelopedDataAsn2.Version = 2;
			}
			return Helpers.EncodeContentInfo<EnvelopedDataAsn>(envelopedDataAsn2, "1.2.840.113549.1.7.3", AsnEncodingRules.DER);
		}

		private byte[] EncryptContent(ContentInfo contentInfo, AlgorithmIdentifier contentEncryptionAlgorithm, out byte[] cek, out byte[] parameterBytes)
		{
			byte[] array2;
			using (SymmetricAlgorithm symmetricAlgorithm = ManagedPkcsPal.OpenAlgorithm(contentEncryptionAlgorithm))
			{
				using (ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor())
				{
					cek = symmetricAlgorithm.Key;
					if (symmetricAlgorithm is RC2)
					{
						using (AsnWriter asnWriter = AsnSerializer.Serialize<Rc2CbcParameters>(new Rc2CbcParameters(symmetricAlgorithm.IV, symmetricAlgorithm.KeySize), AsnEncodingRules.DER))
						{
							parameterBytes = asnWriter.Encode();
							goto IL_005F;
						}
					}
					parameterBytes = this.EncodeOctetString(symmetricAlgorithm.IV);
					IL_005F:
					byte[] array = contentInfo.Content;
					if (contentInfo.ContentType.Value == "1.2.840.113549.1.7.1")
					{
						array = this.EncodeOctetString(array);
						array2 = cryptoTransform.OneShot(array);
					}
					else if (contentInfo.Content.Length == 0)
					{
						array2 = cryptoTransform.OneShot(contentInfo.Content);
					}
					else
					{
						AsnReader asnReader = new AsnReader(contentInfo.Content, AsnEncodingRules.BER);
						array2 = cryptoTransform.OneShot(asnReader.PeekContentBytes().ToArray());
					}
				}
			}
			return array2;
		}

		public override Exception CreateRecipientsNotFoundException()
		{
			return new CryptographicException("The enveloped-data message does not contain the specified recipient.");
		}

		public override Exception CreateRecipientInfosAfterEncryptException()
		{
			return ManagedPkcsPal.CreateInvalidMessageTypeException();
		}

		public override Exception CreateDecryptAfterEncryptException()
		{
			return ManagedPkcsPal.CreateInvalidMessageTypeException();
		}

		public override Exception CreateDecryptTwiceException()
		{
			return ManagedPkcsPal.CreateInvalidMessageTypeException();
		}

		private static Exception CreateInvalidMessageTypeException()
		{
			return new CryptographicException("Invalid cryptographic message type.");
		}

		private static KeyTransRecipientInfoAsn MakeKtri(byte[] cek, CmsRecipient recipient, out bool v0Recipient)
		{
			KeyTransRecipientInfoAsn keyTransRecipientInfoAsn = new KeyTransRecipientInfoAsn();
			if (recipient.RecipientIdentifierType == SubjectIdentifierType.SubjectKeyIdentifier)
			{
				keyTransRecipientInfoAsn.Version = 2;
				keyTransRecipientInfoAsn.Rid.SubjectKeyIdentifier = new ReadOnlyMemory<byte>?(recipient.Certificate.GetSubjectKeyIdentifier());
			}
			else
			{
				if (recipient.RecipientIdentifierType != SubjectIdentifierType.IssuerAndSerialNumber)
				{
					throw new CryptographicException("The subject identifier type {0} is not valid.", recipient.RecipientIdentifierType.ToString());
				}
				byte[] serialNumber = recipient.Certificate.GetSerialNumber();
				Array.Reverse<byte>(serialNumber);
				IssuerAndSerialNumberAsn issuerAndSerialNumberAsn = new IssuerAndSerialNumberAsn
				{
					Issuer = recipient.Certificate.IssuerName.RawData,
					SerialNumber = serialNumber
				};
				keyTransRecipientInfoAsn.Rid.IssuerAndSerialNumber = new IssuerAndSerialNumberAsn?(issuerAndSerialNumberAsn);
			}
			RSAEncryptionPadding rsaencryptionPadding;
			if (recipient.Certificate.GetKeyAlgorithm() == "1.2.840.113549.1.1.7")
			{
				rsaencryptionPadding = RSAEncryptionPadding.OaepSHA1;
				keyTransRecipientInfoAsn.KeyEncryptionAlgorithm.Algorithm = new Oid("1.2.840.113549.1.1.7", "1.2.840.113549.1.1.7");
				keyTransRecipientInfoAsn.KeyEncryptionAlgorithm.Parameters = new ReadOnlyMemory<byte>?(ManagedPkcsPal.s_rsaOaepSha1Parameters);
			}
			else
			{
				rsaencryptionPadding = RSAEncryptionPadding.Pkcs1;
				keyTransRecipientInfoAsn.KeyEncryptionAlgorithm.Algorithm = new Oid("1.2.840.113549.1.1.1", "1.2.840.113549.1.1.1");
				keyTransRecipientInfoAsn.KeyEncryptionAlgorithm.Parameters = new ReadOnlyMemory<byte>?(ManagedPkcsPal.s_rsaPkcsParameters);
			}
			using (RSA rsapublicKey = recipient.Certificate.GetRSAPublicKey())
			{
				keyTransRecipientInfoAsn.EncryptedKey = rsapublicKey.Encrypt(cek, rsaencryptionPadding);
			}
			v0Recipient = keyTransRecipientInfoAsn.Version == 0;
			return keyTransRecipientInfoAsn;
		}

		public override void AddCertsFromStoreForDecryption(X509Certificate2Collection certs)
		{
			certs.AddRange(Helpers.GetStoreCertificates(StoreName.My, StoreLocation.CurrentUser, false));
			try
			{
				certs.AddRange(Helpers.GetStoreCertificates(StoreName.My, StoreLocation.LocalMachine, false));
			}
			catch (CryptographicException)
			{
			}
		}

		public override byte[] GetSubjectKeyIdentifier(X509Certificate2 certificate)
		{
			return certificate.GetSubjectKeyIdentifier();
		}

		public override T GetPrivateKeyForSigning<T>(X509Certificate2 certificate, bool silent)
		{
			return this.GetPrivateKey<T>(certificate);
		}

		public override T GetPrivateKeyForDecryption<T>(X509Certificate2 certificate, bool silent)
		{
			return this.GetPrivateKey<T>(certificate);
		}

		private T GetPrivateKey<T>(X509Certificate2 certificate) where T : AsymmetricAlgorithm
		{
			if (typeof(T) == typeof(RSA))
			{
				return (T)((object)certificate.GetRSAPrivateKey());
			}
			if (typeof(T) == typeof(ECDsa))
			{
				return (T)((object)certificate.GetECDsaPrivateKey());
			}
			return default(T);
		}

		private static SymmetricAlgorithm OpenAlgorithm(AlgorithmIdentifierAsn contentEncryptionAlgorithm)
		{
			SymmetricAlgorithm symmetricAlgorithm = ManagedPkcsPal.OpenAlgorithm(contentEncryptionAlgorithm.Algorithm);
			if (symmetricAlgorithm is RC2)
			{
				if (contentEncryptionAlgorithm.Parameters == null)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				Rc2CbcParameters rc2CbcParameters = AsnSerializer.Deserialize<Rc2CbcParameters>(contentEncryptionAlgorithm.Parameters.Value, AsnEncodingRules.BER);
				symmetricAlgorithm.KeySize = rc2CbcParameters.GetEffectiveKeyBits();
				symmetricAlgorithm.IV = rc2CbcParameters.Iv.ToArray();
			}
			else
			{
				if (contentEncryptionAlgorithm.Parameters == null)
				{
					throw new CryptographicException("ASN1 corrupted data.");
				}
				AsnReader asnReader = new AsnReader(contentEncryptionAlgorithm.Parameters.Value, AsnEncodingRules.BER);
				ReadOnlyMemory<byte> readOnlyMemory;
				if (asnReader.TryGetPrimitiveOctetStringBytes(out readOnlyMemory))
				{
					symmetricAlgorithm.IV = readOnlyMemory.ToArray();
				}
				else
				{
					byte[] array = new byte[symmetricAlgorithm.BlockSize / 8];
					int num;
					if (!asnReader.TryCopyOctetStringBytes(array, out num) || num != array.Length)
					{
						throw new CryptographicException("ASN1 corrupted data.");
					}
					symmetricAlgorithm.IV = array;
				}
			}
			return symmetricAlgorithm;
		}

		private static SymmetricAlgorithm OpenAlgorithm(AlgorithmIdentifier algorithmIdentifier)
		{
			SymmetricAlgorithm symmetricAlgorithm = ManagedPkcsPal.OpenAlgorithm(algorithmIdentifier.Oid);
			if (symmetricAlgorithm is RC2)
			{
				if (algorithmIdentifier.KeyLength != 0)
				{
					symmetricAlgorithm.KeySize = algorithmIdentifier.KeyLength;
				}
				else
				{
					symmetricAlgorithm.KeySize = 128;
				}
			}
			return symmetricAlgorithm;
		}

		private static SymmetricAlgorithm OpenAlgorithm(Oid algorithmIdentifier)
		{
			string value = algorithmIdentifier.Value;
			SymmetricAlgorithm symmetricAlgorithm;
			if (!(value == "1.2.840.113549.3.2"))
			{
				if (!(value == "1.3.14.3.2.7"))
				{
					if (!(value == "1.2.840.113549.3.7"))
					{
						if (!(value == "2.16.840.1.101.3.4.1.2"))
						{
							if (!(value == "2.16.840.1.101.3.4.1.22"))
							{
								if (!(value == "2.16.840.1.101.3.4.1.42"))
								{
									throw new CryptographicException("Unknown algorithm '{0}'.", algorithmIdentifier.Value);
								}
								symmetricAlgorithm = Aes.Create();
								symmetricAlgorithm.KeySize = 256;
							}
							else
							{
								symmetricAlgorithm = Aes.Create();
								symmetricAlgorithm.KeySize = 192;
							}
						}
						else
						{
							symmetricAlgorithm = Aes.Create();
							symmetricAlgorithm.KeySize = 128;
						}
					}
					else
					{
						symmetricAlgorithm = TripleDES.Create();
					}
				}
				else
				{
					symmetricAlgorithm = DES.Create();
				}
			}
			else
			{
				symmetricAlgorithm = RC2.Create();
			}
			symmetricAlgorithm.Padding = PaddingMode.PKCS7;
			symmetricAlgorithm.Mode = CipherMode.CBC;
			return symmetricAlgorithm;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ManagedPkcsPal()
		{
			byte[] array = new byte[2];
			array[0] = 6;
			ManagedPkcsPal.s_invalidEmptyOid = array;
			byte[] array2 = new byte[2];
			array2[0] = 5;
			ManagedPkcsPal.s_rsaPkcsParameters = array2;
			byte[] array3 = new byte[2];
			array3[0] = 48;
			ManagedPkcsPal.s_rsaOaepSha1Parameters = array3;
		}

		private static readonly byte[] s_invalidEmptyOid;

		private static readonly byte[] s_rsaPkcsParameters;

		private static readonly byte[] s_rsaOaepSha1Parameters;

		private sealed class ManagedDecryptorPal : DecryptorPal
		{
			public ManagedDecryptorPal(byte[] dataCopy, EnvelopedDataAsn envelopedDataAsn, RecipientInfoCollection recipientInfos)
				: base(recipientInfos)
			{
				this._dataCopy = dataCopy;
				this._envelopedData = envelopedDataAsn;
			}

			public unsafe override ContentInfo TryDecrypt(RecipientInfo recipientInfo, X509Certificate2 cert, X509Certificate2Collection originatorCerts, X509Certificate2Collection extraStore, out Exception exception)
			{
				ManagedPkcsPal.ManagedKeyTransPal managedKeyTransPal = recipientInfo.Pal as ManagedPkcsPal.ManagedKeyTransPal;
				if (managedKeyTransPal != null)
				{
					byte[] array = managedKeyTransPal.DecryptCek(cert, out exception);
					byte[] array2;
					if ((array2 = array) == null || array2.Length == 0)
					{
						byte* ptr = null;
					}
					else
					{
						byte* ptr = &array2[0];
					}
					byte[] array3;
					try
					{
						if (exception != null)
						{
							return null;
						}
						ReadOnlyMemory<byte>? encryptedContent = this._envelopedData.EncryptedContentInfo.EncryptedContent;
						if (encryptedContent == null)
						{
							exception = null;
							return new ContentInfo(new Oid(this._envelopedData.EncryptedContentInfo.ContentType), Array.Empty<byte>());
						}
						array3 = this.DecryptContent(encryptedContent.Value, array, out exception);
					}
					finally
					{
						if (array != null)
						{
							Array.Clear(array, 0, array.Length);
						}
					}
					array2 = null;
					if (exception != null)
					{
						return null;
					}
					if (this._envelopedData.EncryptedContentInfo.ContentType == "1.2.840.113549.1.7.1")
					{
						byte[] array4 = null;
						try
						{
							AsnReader asnReader = new AsnReader(array3, AsnEncodingRules.BER);
							ReadOnlyMemory<byte> readOnlyMemory;
							if (asnReader.TryGetPrimitiveOctetStringBytes(out readOnlyMemory))
							{
								array3 = readOnlyMemory.ToArray();
							}
							else
							{
								array4 = ArrayPool<byte>.Shared.Rent(array3.Length);
								int num;
								if (asnReader.TryCopyOctetStringBytes(array4, out num))
								{
									Span<byte> span = new Span<byte>(array4, 0, num);
									array3 = span.ToArray();
									span.Clear();
								}
							}
							goto IL_017A;
						}
						catch (CryptographicException)
						{
							goto IL_017A;
						}
						finally
						{
							if (array4 != null)
							{
								ArrayPool<byte>.Shared.Return(array4, false);
							}
						}
					}
					array3 = ManagedPkcsPal.ManagedDecryptorPal.GetAsnSequenceWithContentNoValidation(array3);
					IL_017A:
					exception = null;
					return new ContentInfo(new Oid(this._envelopedData.EncryptedContentInfo.ContentType), array3);
				}
				exception = new CryptographicException("The recipient type '{0}' is not supported for encryption or decryption on this platform.", recipientInfo.Type.ToString());
				return null;
			}

			private static byte[] GetAsnSequenceWithContentNoValidation(ReadOnlySpan<byte> content)
			{
				byte[] array2;
				using (AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.BER))
				{
					asnWriter.WriteOctetString(content);
					byte[] array = asnWriter.Encode();
					array[0] = 48;
					array2 = array;
				}
				return array2;
			}

			private byte[] DecryptContent(ReadOnlyMemory<byte> encryptedContent, byte[] cek, out Exception exception)
			{
				exception = null;
				int length = encryptedContent.Length;
				byte[] array = ArrayPool<byte>.Shared.Rent(length);
				byte[] array2;
				try
				{
					encryptedContent.CopyTo(array);
					using (SymmetricAlgorithm symmetricAlgorithm = ManagedPkcsPal.OpenAlgorithm(this._envelopedData.EncryptedContentInfo.ContentEncryptionAlgorithm))
					{
						using (ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor(cek, symmetricAlgorithm.IV))
						{
							array2 = cryptoTransform.OneShot(array, 0, length);
						}
					}
				}
				catch (CryptographicException ex)
				{
					exception = ex;
					array2 = null;
				}
				finally
				{
					Array.Clear(array, 0, length);
					ArrayPool<byte>.Shared.Return(array, false);
					array = null;
				}
				return array2;
			}

			public override void Dispose()
			{
			}

			private byte[] _dataCopy;

			private EnvelopedDataAsn _envelopedData;
		}

		private sealed class ManagedKeyAgreePal : KeyAgreeRecipientInfoPal
		{
			internal ManagedKeyAgreePal(KeyAgreeRecipientInfoAsn asn, int index)
			{
				this._asn = asn;
				this._index = index;
			}

			public override byte[] EncryptedKey
			{
				get
				{
					return this._asn.RecipientEncryptedKeys[this._index].EncryptedKey.ToArray();
				}
			}

			public override AlgorithmIdentifier KeyEncryptionAlgorithm
			{
				get
				{
					return this._asn.KeyEncryptionAlgorithm.ToPresentationObject();
				}
			}

			public override SubjectIdentifier RecipientIdentifier
			{
				get
				{
					IssuerAndSerialNumberAsn? issuerAndSerialNumber = this._asn.RecipientEncryptedKeys[this._index].Rid.IssuerAndSerialNumber;
					RecipientKeyIdentifier rkeyId = this._asn.RecipientEncryptedKeys[this._index].Rid.RKeyId;
					return new SubjectIdentifier(issuerAndSerialNumber, (rkeyId != null) ? new ReadOnlyMemory<byte>?(rkeyId.SubjectKeyIdentifier) : null);
				}
			}

			public override int Version
			{
				get
				{
					return this._asn.Version;
				}
			}

			public override DateTime Date
			{
				get
				{
					KeyAgreeRecipientIdentifierAsn rid = this._asn.RecipientEncryptedKeys[this._index].Rid;
					if (rid.RKeyId == null)
					{
						throw new InvalidOperationException("The Date property is not available for none KID key agree recipient.");
					}
					if (rid.RKeyId.Date == null)
					{
						return DateTime.FromFileTimeUtc(0L);
					}
					return rid.RKeyId.Date.Value.LocalDateTime;
				}
			}

			public override SubjectIdentifierOrKey OriginatorIdentifierOrKey
			{
				get
				{
					return this._asn.Originator.ToSubjectIdentifierOrKey();
				}
			}

			public override CryptographicAttributeObject OtherKeyAttribute
			{
				get
				{
					KeyAgreeRecipientIdentifierAsn rid = this._asn.RecipientEncryptedKeys[this._index].Rid;
					if (rid.RKeyId == null)
					{
						throw new InvalidOperationException("The Date property is not available for none KID key agree recipient.");
					}
					if (rid.RKeyId.Other == null)
					{
						return null;
					}
					Oid oid = new Oid(rid.RKeyId.Other.Value.KeyAttrId);
					byte[] array = Array.Empty<byte>();
					OtherKeyAttributeAsn otherKeyAttributeAsn = rid.RKeyId.Other.Value;
					if (otherKeyAttributeAsn.KeyAttr != null)
					{
						otherKeyAttributeAsn = rid.RKeyId.Other.Value;
						array = otherKeyAttributeAsn.KeyAttr.Value.ToArray();
					}
					AsnEncodedDataCollection asnEncodedDataCollection = new AsnEncodedDataCollection(new Pkcs9AttributeObject(oid, array));
					return new CryptographicAttributeObject(oid, asnEncodedDataCollection);
				}
			}

			private readonly KeyAgreeRecipientInfoAsn _asn;

			private readonly int _index;
		}

		private sealed class ManagedKeyTransPal : KeyTransRecipientInfoPal
		{
			internal ManagedKeyTransPal(KeyTransRecipientInfoAsn asn)
			{
				this._asn = asn;
			}

			public override byte[] EncryptedKey
			{
				get
				{
					return this._asn.EncryptedKey.ToArray();
				}
			}

			public override AlgorithmIdentifier KeyEncryptionAlgorithm
			{
				get
				{
					return this._asn.KeyEncryptionAlgorithm.ToPresentationObject();
				}
			}

			public override SubjectIdentifier RecipientIdentifier
			{
				get
				{
					return new SubjectIdentifier(this._asn.Rid.IssuerAndSerialNumber, this._asn.Rid.SubjectKeyIdentifier);
				}
			}

			public override int Version
			{
				get
				{
					return this._asn.Version;
				}
			}

			internal byte[] DecryptCek(X509Certificate2 cert, out Exception exception)
			{
				ReadOnlyMemory<byte>? parameters = this._asn.KeyEncryptionAlgorithm.Parameters;
				string value = this._asn.KeyEncryptionAlgorithm.Algorithm.Value;
				RSAEncryptionPadding rsaencryptionPadding;
				if (!(value == "1.2.840.113549.1.1.1"))
				{
					if (!(value == "1.2.840.113549.1.1.7"))
					{
						exception = new CryptographicException("Unknown algorithm '{0}'.", this._asn.KeyEncryptionAlgorithm.Algorithm.Value);
						return null;
					}
					if (parameters != null && !parameters.Value.Span.SequenceEqual<byte>(ManagedPkcsPal.s_rsaOaepSha1Parameters))
					{
						exception = new CryptographicException("ASN1 corrupted data.");
						return null;
					}
					rsaencryptionPadding = RSAEncryptionPadding.OaepSHA1;
				}
				else
				{
					if (parameters != null && !parameters.Value.Span.SequenceEqual<byte>(ManagedPkcsPal.s_rsaPkcsParameters))
					{
						exception = new CryptographicException("ASN1 corrupted data.");
						return null;
					}
					rsaencryptionPadding = RSAEncryptionPadding.Pkcs1;
				}
				byte[] array = null;
				int num = 0;
				byte[] array2;
				try
				{
					using (RSA rsaprivateKey = cert.GetRSAPrivateKey())
					{
						if (rsaprivateKey == null)
						{
							exception = new CryptographicException("A certificate with a private key is required.");
							array2 = null;
						}
						else
						{
							exception = null;
							array2 = rsaprivateKey.Decrypt(this._asn.EncryptedKey.Span.ToArray(), rsaencryptionPadding);
						}
					}
				}
				catch (CryptographicException ex)
				{
					exception = ex;
					array2 = null;
				}
				finally
				{
					if (array != null)
					{
						Array.Clear(array, 0, num);
						ArrayPool<byte>.Shared.Return(array, false);
					}
				}
				return array2;
			}

			private readonly KeyTransRecipientInfoAsn _asn;
		}
	}
}
