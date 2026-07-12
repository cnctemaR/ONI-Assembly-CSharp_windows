using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Internal.Cryptography.Pal;
using Microsoft.Win32.SafeHandles;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	internal abstract class X509Certificate2ImplUnix : X509Certificate2Impl
	{
		private void EnsureCertData()
		{
			if (this.readCertData)
			{
				return;
			}
			base.ThrowIfContextInvalid();
			this.certData = new CertificateData(this.GetRawCertData());
			this.readCertData = true;
		}

		protected abstract byte[] GetRawCertData();

		public sealed override bool Archived
		{
			get
			{
				return false;
			}
			set
			{
				throw new PlatformNotSupportedException(SR.Format("The {0} value cannot be set on Unix.", "Archived"));
			}
		}

		public sealed override string KeyAlgorithm
		{
			get
			{
				this.EnsureCertData();
				return this.certData.PublicKeyAlgorithm.AlgorithmId;
			}
		}

		public sealed override byte[] KeyAlgorithmParameters
		{
			get
			{
				this.EnsureCertData();
				return this.certData.PublicKeyAlgorithm.Parameters;
			}
		}

		public sealed override byte[] PublicKeyValue
		{
			get
			{
				this.EnsureCertData();
				return this.certData.PublicKey;
			}
		}

		public sealed override byte[] SerialNumber
		{
			get
			{
				this.EnsureCertData();
				return this.certData.SerialNumber;
			}
		}

		public sealed override string SignatureAlgorithm
		{
			get
			{
				this.EnsureCertData();
				return this.certData.SignatureAlgorithm.AlgorithmId;
			}
		}

		public sealed override string FriendlyName
		{
			get
			{
				return "";
			}
			set
			{
				throw new PlatformNotSupportedException(SR.Format("The {0} value cannot be set on Unix.", "FriendlyName"));
			}
		}

		public sealed override int Version
		{
			get
			{
				this.EnsureCertData();
				return this.certData.Version + 1;
			}
		}

		public sealed override X500DistinguishedName SubjectName
		{
			get
			{
				this.EnsureCertData();
				return this.certData.Subject;
			}
		}

		public sealed override X500DistinguishedName IssuerName
		{
			get
			{
				this.EnsureCertData();
				return this.certData.Issuer;
			}
		}

		public sealed override string Subject
		{
			get
			{
				return this.SubjectName.Name;
			}
		}

		public sealed override string Issuer
		{
			get
			{
				return this.IssuerName.Name;
			}
		}

		public sealed override string LegacySubject
		{
			get
			{
				return this.SubjectName.Decode(X500DistinguishedNameFlags.None);
			}
		}

		public sealed override string LegacyIssuer
		{
			get
			{
				return this.IssuerName.Decode(X500DistinguishedNameFlags.None);
			}
		}

		public sealed override byte[] RawData
		{
			get
			{
				this.EnsureCertData();
				return this.certData.RawData;
			}
		}

		public sealed override byte[] Thumbprint
		{
			get
			{
				this.EnsureCertData();
				byte[] array;
				using (SHA1 sha = SHA1.Create())
				{
					array = sha.ComputeHash(this.certData.RawData);
				}
				return array;
			}
		}

		public sealed override string GetNameInfo(X509NameType nameType, bool forIssuer)
		{
			this.EnsureCertData();
			return this.certData.GetNameInfo(nameType, forIssuer);
		}

		public sealed override IEnumerable<X509Extension> Extensions
		{
			get
			{
				this.EnsureCertData();
				return this.certData.Extensions;
			}
		}

		public sealed override DateTime NotAfter
		{
			get
			{
				this.EnsureCertData();
				return this.certData.NotAfter.ToLocalTime();
			}
		}

		public sealed override DateTime NotBefore
		{
			get
			{
				this.EnsureCertData();
				return this.certData.NotBefore.ToLocalTime();
			}
		}

		public sealed override void AppendPrivateKeyInfo(StringBuilder sb)
		{
			if (!this.HasPrivateKey)
			{
				return;
			}
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("[Private Key]");
		}

		public override void Reset()
		{
			this.readCertData = false;
		}

		public sealed override byte[] Export(X509ContentType contentType, SafePasswordHandle password)
		{
			base.ThrowIfContextInvalid();
			switch (contentType)
			{
			case X509ContentType.Cert:
				return this.RawData;
			case X509ContentType.SerializedCert:
			case X509ContentType.SerializedStore:
				throw new PlatformNotSupportedException("X509ContentType.SerializedCert and X509ContentType.SerializedStore are not supported on Unix.");
			case X509ContentType.Pfx:
				return this.ExportPkcs12(password);
			case X509ContentType.Pkcs7:
				return this.ExportPkcs12(null);
			default:
				throw new CryptographicException("Invalid content type.");
			}
		}

		private byte[] ExportPkcs12(SafePasswordHandle password)
		{
			if (password == null || password.IsInvalid)
			{
				return this.ExportPkcs12(null);
			}
			string text = password.Mono_DangerousGetString();
			return this.ExportPkcs12(text);
		}

		private byte[] ExportPkcs12(string password)
		{
			PKCS12 pkcs = new PKCS12();
			byte[] bytes;
			try
			{
				Hashtable hashtable = new Hashtable();
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = arrayList;
				byte[] array = new byte[4];
				array[0] = 1;
				arrayList2.Add(array);
				hashtable.Add("1.2.840.113549.1.9.21", arrayList);
				if (password != null)
				{
					pkcs.Password = password;
				}
				pkcs.AddCertificate(new X509Certificate(this.RawData), hashtable);
				if (this.IntermediateCertificates != null)
				{
					for (int i = 0; i < this.IntermediateCertificates.Count; i++)
					{
						pkcs.AddCertificate(new X509Certificate(this.IntermediateCertificates[i].RawData));
					}
				}
				AsymmetricAlgorithm privateKey = this.PrivateKey;
				if (privateKey != null)
				{
					pkcs.AddPkcs8ShroudedKeyBag(privateKey, hashtable);
				}
				bytes = pkcs.GetBytes();
			}
			finally
			{
				pkcs.Password = null;
			}
			return bytes;
		}

		private bool readCertData;

		private CertificateData certData;
	}
}
