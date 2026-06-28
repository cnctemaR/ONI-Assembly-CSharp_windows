using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using Mono.Security;
using Mono.Security.X509;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SignedCms
	{
		public SignedCms()
		{
			this._certs = new X509Certificate2Collection();
			this._info = new SignerInfoCollection();
		}

		public SignedCms(ContentInfo content)
			: this(content, false)
		{
		}

		public SignedCms(ContentInfo content, bool detached)
			: this()
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			this._content = content;
			this._detached = detached;
		}

		public SignedCms(SubjectIdentifierType signerIdentifierType)
			: this()
		{
			this._type = signerIdentifierType;
		}

		public SignedCms(SubjectIdentifierType signerIdentifierType, ContentInfo content)
			: this(content, false)
		{
			this._type = signerIdentifierType;
		}

		public SignedCms(SubjectIdentifierType signerIdentifierType, ContentInfo content, bool detached)
			: this(content, detached)
		{
			this._type = signerIdentifierType;
		}

		public X509Certificate2Collection Certificates
		{
			get
			{
				return this._certs;
			}
		}

		public ContentInfo ContentInfo
		{
			get
			{
				if (this._content == null)
				{
					Oid oid = new Oid("1.2.840.113549.1.7.1");
					this._content = new ContentInfo(oid, new byte[0]);
				}
				return this._content;
			}
		}

		public bool Detached
		{
			get
			{
				return this._detached;
			}
		}

		public SignerInfoCollection SignerInfos
		{
			get
			{
				return this._info;
			}
		}

		public int Version
		{
			get
			{
				return this._version;
			}
		}

		[MonoTODO]
		public void CheckSignature(bool verifySignatureOnly)
		{
			foreach (SignerInfo signerInfo in this._info)
			{
				signerInfo.CheckSignature(verifySignatureOnly);
			}
		}

		[MonoTODO]
		public void CheckSignature(X509Certificate2Collection extraStore, bool verifySignatureOnly)
		{
			foreach (SignerInfo signerInfo in this._info)
			{
				signerInfo.CheckSignature(extraStore, verifySignatureOnly);
			}
		}

		[MonoTODO]
		public void CheckHash()
		{
			throw new InvalidOperationException(string.Empty);
		}

		[MonoTODO]
		public void ComputeSignature()
		{
			throw new CryptographicException(string.Empty);
		}

		[MonoTODO]
		public void ComputeSignature(CmsSigner signer)
		{
			this.ComputeSignature();
		}

		[MonoTODO]
		public void ComputeSignature(CmsSigner signer, bool silent)
		{
			this.ComputeSignature();
		}

		private string ToString(byte[] array, bool reverse)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (reverse)
			{
				for (int i = array.Length - 1; i >= 0; i--)
				{
					stringBuilder.Append(array[i].ToString("X2"));
				}
			}
			else
			{
				for (int j = 0; j < array.Length; j++)
				{
					stringBuilder.Append(array[j].ToString("X2"));
				}
			}
			return stringBuilder.ToString();
		}

		private byte[] GetKeyIdentifier(Mono.Security.X509.X509Certificate x509)
		{
			Mono.Security.X509.X509Extension x509Extension = x509.Extensions["2.5.29.14"];
			if (x509Extension != null)
			{
				ASN1 asn = new ASN1(x509Extension.Value.Value);
				return asn.Value;
			}
			ASN1 asn2 = new ASN1(48);
			ASN1 asn3 = asn2.Add(new ASN1(48));
			asn3.Add(new ASN1(CryptoConfig.EncodeOID(x509.KeyAlgorithm)));
			asn3.Add(new ASN1(x509.KeyAlgorithmParameters));
			byte[] publicKey = x509.PublicKey;
			byte[] array = new byte[publicKey.Length + 1];
			Array.Copy(publicKey, 0, array, 1, publicKey.Length);
			asn2.Add(new ASN1(3, array));
			SHA1 sha = SHA1.Create();
			return sha.ComputeHash(asn2.GetBytes());
		}

		[MonoTODO("incomplete - missing attributes")]
		public void Decode(byte[] encodedMessage)
		{
			PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(encodedMessage);
			if (contentInfo.ContentType != "1.2.840.113549.1.7.2")
			{
				throw new Exception(string.Empty);
			}
			PKCS7.SignedData signedData = new PKCS7.SignedData(contentInfo.Content);
			SubjectIdentifierType subjectIdentifierType = SubjectIdentifierType.Unknown;
			object obj = null;
			X509Certificate2 x509Certificate = null;
			if (signedData.SignerInfo.Certificate != null)
			{
				x509Certificate = new X509Certificate2(signedData.SignerInfo.Certificate.RawData);
			}
			else if (signedData.SignerInfo.IssuerName != null && signedData.SignerInfo.SerialNumber != null)
			{
				byte[] serialNumber = signedData.SignerInfo.SerialNumber;
				Array.Reverse(serialNumber);
				subjectIdentifierType = SubjectIdentifierType.IssuerAndSerialNumber;
				X509IssuerSerial x509IssuerSerial = new X509IssuerSerial
				{
					IssuerName = signedData.SignerInfo.IssuerName,
					SerialNumber = this.ToString(serialNumber, true)
				};
				obj = x509IssuerSerial;
				foreach (Mono.Security.X509.X509Certificate x509Certificate2 in signedData.Certificates)
				{
					if (x509Certificate2.IssuerName == signedData.SignerInfo.IssuerName && this.ToString(x509Certificate2.SerialNumber, true) == x509IssuerSerial.SerialNumber)
					{
						x509Certificate = new X509Certificate2(x509Certificate2.RawData);
						break;
					}
				}
			}
			else if (signedData.SignerInfo.SubjectKeyIdentifier != null)
			{
				string text = this.ToString(signedData.SignerInfo.SubjectKeyIdentifier, false);
				subjectIdentifierType = SubjectIdentifierType.SubjectKeyIdentifier;
				obj = text;
				foreach (Mono.Security.X509.X509Certificate x509Certificate3 in signedData.Certificates)
				{
					if (this.ToString(this.GetKeyIdentifier(x509Certificate3), false) == text)
					{
						x509Certificate = new X509Certificate2(x509Certificate3.RawData);
						break;
					}
				}
			}
			SignerInfo signerInfo = new SignerInfo(signedData.SignerInfo.HashName, x509Certificate, subjectIdentifierType, obj, (int)signedData.SignerInfo.Version);
			this._info.Add(signerInfo);
			ASN1 content = signedData.ContentInfo.Content;
			Oid oid = new Oid(signedData.ContentInfo.ContentType);
			this._content = new ContentInfo(oid, content[0].Value);
			foreach (Mono.Security.X509.X509Certificate x509Certificate4 in signedData.Certificates)
			{
				this._certs.Add(new X509Certificate2(x509Certificate4.RawData));
			}
			this._version = (int)signedData.Version;
		}

		[MonoTODO]
		public byte[] Encode()
		{
			return null;
		}

		[MonoTODO]
		public void RemoveSignature(SignerInfo signerInfo)
		{
		}

		[MonoTODO]
		public void RemoveSignature(int index)
		{
		}

		private ContentInfo _content;

		private bool _detached;

		private SignerInfoCollection _info;

		private X509Certificate2Collection _certs;

		private SubjectIdentifierType _type;

		private int _version;
	}
}
