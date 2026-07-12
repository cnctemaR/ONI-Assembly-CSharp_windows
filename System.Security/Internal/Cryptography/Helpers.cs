using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.Pkcs.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Internal.Cryptography
{
	internal static class Helpers
	{
		internal static void AppendData(this IncrementalHash hasher, ReadOnlySpan<byte> data)
		{
			hasher.AppendData(data.ToArray());
		}

		internal static HashAlgorithmName GetDigestAlgorithm(Oid oid)
		{
			return Helpers.GetDigestAlgorithm(oid.Value);
		}

		internal static HashAlgorithmName GetDigestAlgorithm(string oidValue)
		{
			if (oidValue == "1.2.840.113549.2.5")
			{
				return HashAlgorithmName.MD5;
			}
			if (oidValue == "1.3.14.3.2.26")
			{
				return HashAlgorithmName.SHA1;
			}
			if (oidValue == "2.16.840.1.101.3.4.2.1")
			{
				return HashAlgorithmName.SHA256;
			}
			if (oidValue == "2.16.840.1.101.3.4.2.2")
			{
				return HashAlgorithmName.SHA384;
			}
			if (!(oidValue == "2.16.840.1.101.3.4.2.3"))
			{
				throw new CryptographicException("'{0}' is not a known hash algorithm.", oidValue);
			}
			return HashAlgorithmName.SHA512;
		}

		internal static string GetOidFromHashAlgorithm(HashAlgorithmName algName)
		{
			if (algName == HashAlgorithmName.MD5)
			{
				return "1.2.840.113549.2.5";
			}
			if (algName == HashAlgorithmName.SHA1)
			{
				return "1.3.14.3.2.26";
			}
			if (algName == HashAlgorithmName.SHA256)
			{
				return "2.16.840.1.101.3.4.2.1";
			}
			if (algName == HashAlgorithmName.SHA384)
			{
				return "2.16.840.1.101.3.4.2.2";
			}
			if (algName == HashAlgorithmName.SHA512)
			{
				return "2.16.840.1.101.3.4.2.3";
			}
			throw new CryptographicException("Unknown algorithm '{0}'.", algName.Name);
		}

		public static byte[] Resize(this byte[] a, int size)
		{
			Array.Resize<byte>(ref a, size);
			return a;
		}

		public static void RemoveAt<T>(ref T[] arr, int idx)
		{
			if (arr.Length == 1)
			{
				arr = Array.Empty<T>();
				return;
			}
			T[] array = new T[arr.Length - 1];
			if (idx != 0)
			{
				Array.Copy(arr, 0, array, 0, idx);
			}
			if (idx < array.Length)
			{
				Array.Copy(arr, idx + 1, array, idx, array.Length - idx);
			}
			arr = array;
		}

		public static T[] NormalizeSet<T>(T[] setItems, Action<byte[]> encodedValueProcessor = null)
		{
			byte[] array = AsnSerializer.Serialize<Helpers.AsnSet<T>>(new Helpers.AsnSet<T>
			{
				SetData = setItems
			}, AsnEncodingRules.DER).Encode();
			ref Helpers.AsnSet<T> ptr = AsnSerializer.Deserialize<Helpers.AsnSet<T>>(array, AsnEncodingRules.DER);
			if (encodedValueProcessor != null)
			{
				encodedValueProcessor(array);
			}
			return ptr.SetData;
		}

		internal static byte[] EncodeContentInfo<T>(T value, string contentType, AsnEncodingRules ruleSet = AsnEncodingRules.DER)
		{
			byte[] array;
			using (AsnWriter asnWriter = AsnSerializer.Serialize<T>(value, ruleSet))
			{
				using (AsnWriter asnWriter2 = AsnSerializer.Serialize<ContentInfoAsn>(new ContentInfoAsn
				{
					ContentType = contentType,
					Content = asnWriter.Encode()
				}, ruleSet))
				{
					array = asnWriter2.Encode();
				}
			}
			return array;
		}

		public static CmsRecipientCollection DeepCopy(this CmsRecipientCollection recipients)
		{
			CmsRecipientCollection cmsRecipientCollection = new CmsRecipientCollection();
			foreach (CmsRecipient cmsRecipient in recipients)
			{
				X509Certificate2 certificate = cmsRecipient.Certificate;
				X509Certificate2 x509Certificate = new X509Certificate2(certificate.Handle);
				CmsRecipient cmsRecipient2 = new CmsRecipient(cmsRecipient.RecipientIdentifierType, x509Certificate);
				cmsRecipientCollection.Add(cmsRecipient2);
				GC.KeepAlive(certificate);
			}
			return cmsRecipientCollection;
		}

		public static byte[] UnicodeToOctetString(this string s)
		{
			byte[] array = new byte[2 * (s.Length + 1)];
			Encoding.Unicode.GetBytes(s, 0, s.Length, array, 0);
			return array;
		}

		public static string OctetStringToUnicode(this byte[] octets)
		{
			if (octets.Length < 2)
			{
				return string.Empty;
			}
			return Encoding.Unicode.GetString(octets, 0, octets.Length - 2);
		}

		public static X509Certificate2Collection GetStoreCertificates(StoreName storeName, StoreLocation storeLocation, bool openExistingOnly)
		{
			X509Certificate2Collection certificates;
			using (X509Store x509Store = new X509Store(storeName, storeLocation))
			{
				OpenFlags openFlags = OpenFlags.IncludeArchived;
				if (openExistingOnly)
				{
					openFlags |= OpenFlags.OpenExistingOnly;
				}
				x509Store.Open(openFlags);
				certificates = x509Store.Certificates;
			}
			return certificates;
		}

		public static X509Certificate2 TryFindMatchingCertificate(this X509Certificate2Collection certs, SubjectIdentifier recipientIdentifier)
		{
			SubjectIdentifierType type = recipientIdentifier.Type;
			if (type != SubjectIdentifierType.IssuerAndSerialNumber)
			{
				if (type != SubjectIdentifierType.SubjectKeyIdentifier)
				{
					throw new CryptographicException();
				}
				byte[] array = ((string)recipientIdentifier.Value).ToSkiBytes();
				foreach (X509Certificate2 x509Certificate in certs)
				{
					byte[] subjectKeyIdentifier = PkcsPal.Instance.GetSubjectKeyIdentifier(x509Certificate);
					if (Helpers.AreByteArraysEqual(array, subjectKeyIdentifier))
					{
						return x509Certificate;
					}
				}
			}
			else
			{
				X509IssuerSerial x509IssuerSerial = (X509IssuerSerial)recipientIdentifier.Value;
				byte[] array2 = x509IssuerSerial.SerialNumber.ToSerialBytes();
				string issuerName = x509IssuerSerial.IssuerName;
				foreach (X509Certificate2 x509Certificate2 in certs)
				{
					if (Helpers.AreByteArraysEqual(x509Certificate2.GetSerialNumber(), array2) && x509Certificate2.Issuer == issuerName)
					{
						return x509Certificate2;
					}
				}
			}
			return null;
		}

		private static bool AreByteArraysEqual(byte[] ba1, byte[] ba2)
		{
			if (ba1.Length != ba2.Length)
			{
				return false;
			}
			for (int i = 0; i < ba1.Length; i++)
			{
				if (ba1[i] != ba2[i])
				{
					return false;
				}
			}
			return true;
		}

		private static byte[] ToSkiBytes(this string skiString)
		{
			return skiString.UpperHexStringToByteArray();
		}

		public static string ToSkiString(this byte[] skiBytes)
		{
			return Helpers.ToUpperHexString(skiBytes);
		}

		public static string ToBigEndianHex(this ReadOnlySpan<byte> bytes)
		{
			return Helpers.ToUpperHexString(bytes);
		}

		private static byte[] ToSerialBytes(this string serialString)
		{
			byte[] array = serialString.UpperHexStringToByteArray();
			Array.Reverse<byte>(array);
			return array;
		}

		public static string ToSerialString(this byte[] serialBytes)
		{
			serialBytes = serialBytes.CloneByteArray();
			Array.Reverse<byte>(serialBytes);
			return Helpers.ToUpperHexString(serialBytes);
		}

		private static string ToUpperHexString(ReadOnlySpan<byte> ba)
		{
			StringBuilder stringBuilder = new StringBuilder(ba.Length * 2);
			for (int i = 0; i < ba.Length; i++)
			{
				stringBuilder.Append(ba[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}

		private static byte[] UpperHexStringToByteArray(this string normalizedString)
		{
			byte[] array = new byte[normalizedString.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				char c = normalizedString[i * 2];
				byte b = (byte)(Helpers.UpperHexCharToNybble(c) << 4);
				c = normalizedString[i * 2 + 1];
				b |= Helpers.UpperHexCharToNybble(c);
				array[i] = b;
			}
			return array;
		}

		private static byte UpperHexCharToNybble(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return (byte)(c - '0');
			}
			if (c >= 'A' && c <= 'F')
			{
				return (byte)(c - 'A' + '\n');
			}
			throw new CryptographicException();
		}

		public static Pkcs9AttributeObject CreateBestPkcs9AttributeObjectAvailable(Oid oid, byte[] encodedAttribute)
		{
			Pkcs9AttributeObject pkcs9AttributeObject = new Pkcs9AttributeObject(oid, encodedAttribute);
			string value = oid.Value;
			if (!(value == "1.3.6.1.4.1.311.88.2.1"))
			{
				if (!(value == "1.3.6.1.4.1.311.88.2.2"))
				{
					if (!(value == "1.2.840.113549.1.9.5"))
					{
						if (!(value == "1.2.840.113549.1.9.3"))
						{
							if (value == "1.2.840.113549.1.9.4")
							{
								pkcs9AttributeObject = Helpers.Upgrade<Pkcs9MessageDigest>(pkcs9AttributeObject);
							}
						}
						else
						{
							pkcs9AttributeObject = Helpers.Upgrade<Pkcs9ContentType>(pkcs9AttributeObject);
						}
					}
					else
					{
						pkcs9AttributeObject = Helpers.Upgrade<Pkcs9SigningTime>(pkcs9AttributeObject);
					}
				}
				else
				{
					pkcs9AttributeObject = Helpers.Upgrade<Pkcs9DocumentDescription>(pkcs9AttributeObject);
				}
			}
			else
			{
				pkcs9AttributeObject = Helpers.Upgrade<Pkcs9DocumentName>(pkcs9AttributeObject);
			}
			return pkcs9AttributeObject;
		}

		private static T Upgrade<T>(Pkcs9AttributeObject basicAttribute) where T : Pkcs9AttributeObject, new()
		{
			T t = new T();
			t.CopyFrom(basicAttribute);
			return t;
		}

		public static byte[] GetSubjectKeyIdentifier(this X509Certificate2 certificate)
		{
			X509Extension x509Extension = certificate.Extensions["2.5.29.14"];
			if (x509Extension == null)
			{
				byte[] array;
				using (HashAlgorithm hashAlgorithm = SHA1.Create())
				{
					array = hashAlgorithm.ComputeHash(Helpers.GetSubjectPublicKeyInfo(certificate).ToArray());
				}
				return array;
			}
			ReadOnlyMemory<byte> readOnlyMemory;
			if (new AsnReader(x509Extension.RawData, AsnEncodingRules.DER).TryGetPrimitiveOctetStringBytes(out readOnlyMemory))
			{
				return readOnlyMemory.ToArray();
			}
			throw new CryptographicException("ASN1 corrupted data.");
		}

		internal static byte[] OneShot(this ICryptoTransform transform, byte[] data)
		{
			return transform.OneShot(data, 0, data.Length);
		}

		internal static byte[] OneShot(this ICryptoTransform transform, byte[] data, int offset, int length)
		{
			if (transform.CanTransformMultipleBlocks)
			{
				return transform.TransformFinalBlock(data, offset, length);
			}
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
				{
					cryptoStream.Write(data, offset, length);
				}
				array = memoryStream.ToArray();
			}
			return array;
		}

		private static ReadOnlyMemory<byte> GetSubjectPublicKeyInfo(X509Certificate2 certificate)
		{
			return AsnSerializer.Deserialize<Helpers.Certificate>(certificate.RawData, AsnEncodingRules.DER).TbsCertificate.SubjectPublicKeyInfo;
		}

		private struct Certificate
		{
			internal Helpers.TbsCertificateLite TbsCertificate;

			internal AlgorithmIdentifierAsn AlgorithmIdentifier;

			[BitString]
			internal ReadOnlyMemory<byte> SignatureValue;
		}

		private struct TbsCertificateLite
		{
			[ExpectedTag(0, ExplicitTag = true)]
			[DefaultValue(new byte[] { 160, 3, 2, 1, 0 })]
			internal int Version;

			[Integer]
			internal ReadOnlyMemory<byte> SerialNumber;

			internal AlgorithmIdentifierAsn AlgorithmIdentifier;

			[AnyValue]
			[ExpectedTag(TagClass.Universal, 16)]
			internal ReadOnlyMemory<byte> Issuer;

			[AnyValue]
			[ExpectedTag(TagClass.Universal, 16)]
			internal ReadOnlyMemory<byte> Validity;

			[AnyValue]
			[ExpectedTag(TagClass.Universal, 16)]
			internal ReadOnlyMemory<byte> Subject;

			[AnyValue]
			[ExpectedTag(TagClass.Universal, 16)]
			internal ReadOnlyMemory<byte> SubjectPublicKeyInfo;

			[BitString]
			[ExpectedTag(1)]
			[OptionalValue]
			internal ReadOnlyMemory<byte>? IssuerUniqueId;

			[OptionalValue]
			[BitString]
			[ExpectedTag(2)]
			internal ReadOnlyMemory<byte>? SubjectUniqueId;

			[ExpectedTag(3)]
			[AnyValue]
			[OptionalValue]
			internal ReadOnlyMemory<byte>? Extensions;
		}

		internal struct AsnSet<T>
		{
			[SetOf]
			public T[] SetData;
		}
	}
}
