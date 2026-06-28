using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Mono.Security.Authenticode;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	[MonoTODO("X509ContentType.SerializedCert isn't supported (anywhere in the class)")]
	[ComVisible(true)]
	[Serializable]
	public class X509Certificate : ISerializable, IDeserializationCallback
	{
		internal X509Certificate(byte[] data, bool dates)
		{
			if (data != null)
			{
				this.Import(data, null, X509KeyStorageFlags.DefaultKeySet);
				this.hideDates = !dates;
			}
		}

		public X509Certificate(byte[] data)
			: this(data, true)
		{
		}

		public X509Certificate(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentException("Invalid handle.");
			}
			this.InitFromHandle(handle);
		}

		public X509Certificate(X509Certificate cert)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			if (cert != null)
			{
				byte[] rawCertData = cert.GetRawCertData();
				if (rawCertData != null)
				{
					this.x509 = new X509Certificate(rawCertData);
				}
				this.hideDates = false;
			}
		}

		public X509Certificate()
		{
		}

		public X509Certificate(byte[] rawData, string password)
		{
			this.Import(rawData, password, X509KeyStorageFlags.DefaultKeySet);
		}

		[MonoTODO("SecureString support is incomplete")]
		public X509Certificate(byte[] rawData, SecureString password)
		{
			this.Import(rawData, password, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(rawData, password, keyStorageFlags);
		}

		[MonoTODO("SecureString support is incomplete")]
		public X509Certificate(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(rawData, password, keyStorageFlags);
		}

		public X509Certificate(string fileName)
		{
			this.Import(fileName, null, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate(string fileName, string password)
		{
			this.Import(fileName, password, X509KeyStorageFlags.DefaultKeySet);
		}

		[MonoTODO("SecureString support is incomplete")]
		public X509Certificate(string fileName, SecureString password)
		{
			this.Import(fileName, password, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(fileName, password, keyStorageFlags);
		}

		[MonoTODO("SecureString support is incomplete")]
		public X509Certificate(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(fileName, password, keyStorageFlags);
		}

		public X509Certificate(SerializationInfo info, StreamingContext context)
		{
			byte[] array = (byte[])info.GetValue("RawData", typeof(byte[]));
			this.Import(array, null, X509KeyStorageFlags.DefaultKeySet);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("RawData", this.x509.RawData);
		}

		private string tostr(byte[] data)
		{
			if (data != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < data.Length; i++)
				{
					stringBuilder.Append(data[i].ToString("X2"));
				}
				return stringBuilder.ToString();
			}
			return null;
		}

		public static X509Certificate CreateFromCertFile(string filename)
		{
			byte[] array = X509Certificate.Load(filename);
			return new X509Certificate(array);
		}

		[MonoTODO("Incomplete - minimal validation in this version")]
		public static X509Certificate CreateFromSignedFile(string filename)
		{
			try
			{
				AuthenticodeDeformatter authenticodeDeformatter = new AuthenticodeDeformatter(filename);
				if (authenticodeDeformatter.SigningCertificate != null)
				{
					return new X509Certificate(authenticodeDeformatter.SigningCertificate.RawData);
				}
			}
			catch (SecurityException)
			{
				throw;
			}
			catch (Exception ex)
			{
				string text = Locale.GetText("Couldn't extract digital signature from {0}.", new object[] { filename });
				throw new COMException(text, ex);
			}
			throw new CryptographicException(Locale.GetText("{0} isn't signed.", new object[] { filename }));
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		private void InitFromHandle(IntPtr handle)
		{
			if (handle != IntPtr.Zero)
			{
				X509Certificate.CertificateContext certificateContext = (X509Certificate.CertificateContext)Marshal.PtrToStructure(handle, typeof(X509Certificate.CertificateContext));
				byte[] array = new byte[certificateContext.cbCertEncoded];
				Marshal.Copy(certificateContext.pbCertEncoded, array, 0, (int)certificateContext.cbCertEncoded);
				this.x509 = new X509Certificate(array);
			}
		}

		public virtual bool Equals(X509Certificate other)
		{
			if (other == null)
			{
				return false;
			}
			if (other.x509 == null)
			{
				if (this.x509 == null)
				{
					return true;
				}
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			else
			{
				byte[] rawData = other.x509.RawData;
				if (rawData == null)
				{
					return this.x509 == null || this.x509.RawData == null;
				}
				if (this.x509 == null)
				{
					return false;
				}
				if (this.x509.RawData == null)
				{
					return false;
				}
				if (rawData.Length == this.x509.RawData.Length)
				{
					for (int i = 0; i < rawData.Length; i++)
					{
						if (rawData[i] != this.x509.RawData[i])
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		public virtual byte[] GetCertHash()
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			if (this.cachedCertificateHash == null && this.x509 != null)
			{
				SHA1 sha = SHA1.Create();
				this.cachedCertificateHash = sha.ComputeHash(this.x509.RawData);
			}
			return this.cachedCertificateHash;
		}

		public virtual string GetCertHashString()
		{
			return this.tostr(this.GetCertHash());
		}

		public virtual string GetEffectiveDateString()
		{
			if (this.hideDates)
			{
				return null;
			}
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return this.x509.ValidFrom.ToLocalTime().ToString();
		}

		public virtual string GetExpirationDateString()
		{
			if (this.hideDates)
			{
				return null;
			}
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return this.x509.ValidUntil.ToLocalTime().ToString();
		}

		public virtual string GetFormat()
		{
			return "X509";
		}

		public override int GetHashCode()
		{
			if (this.x509 == null)
			{
				return 0;
			}
			if (this.cachedCertificateHash == null)
			{
				this.GetCertHash();
			}
			if (this.cachedCertificateHash != null && this.cachedCertificateHash.Length >= 4)
			{
				return ((int)this.cachedCertificateHash[0] << 24) | ((int)this.cachedCertificateHash[1] << 16) | ((int)this.cachedCertificateHash[2] << 8) | (int)this.cachedCertificateHash[3];
			}
			return 0;
		}

		[Obsolete("Use the Issuer property.")]
		public virtual string GetIssuerName()
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return this.x509.IssuerName;
		}

		public virtual string GetKeyAlgorithm()
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return this.x509.KeyAlgorithm;
		}

		public virtual byte[] GetKeyAlgorithmParameters()
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			byte[] keyAlgorithmParameters = this.x509.KeyAlgorithmParameters;
			if (keyAlgorithmParameters == null)
			{
				throw new CryptographicException(Locale.GetText("Parameters not part of the certificate"));
			}
			return keyAlgorithmParameters;
		}

		public virtual string GetKeyAlgorithmParametersString()
		{
			return this.tostr(this.GetKeyAlgorithmParameters());
		}

		[Obsolete("Use the Subject property.")]
		public virtual string GetName()
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return this.x509.SubjectName;
		}

		public virtual byte[] GetPublicKey()
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return this.x509.PublicKey;
		}

		public virtual string GetPublicKeyString()
		{
			return this.tostr(this.GetPublicKey());
		}

		public virtual byte[] GetRawCertData()
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return this.x509.RawData;
		}

		public virtual string GetRawCertDataString()
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return this.tostr(this.x509.RawData);
		}

		public virtual byte[] GetSerialNumber()
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return this.x509.SerialNumber;
		}

		public virtual string GetSerialNumberString()
		{
			byte[] serialNumber = this.GetSerialNumber();
			Array.Reverse(serialNumber);
			return this.tostr(serialNumber);
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public virtual string ToString(bool fVerbose)
		{
			if (!fVerbose || this.x509 == null)
			{
				return base.ToString();
			}
			string newLine = Environment.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[Subject]{0}  {1}{0}{0}", newLine, this.Subject);
			stringBuilder.AppendFormat("[Issuer]{0}  {1}{0}{0}", newLine, this.Issuer);
			stringBuilder.AppendFormat("[Not Before]{0}  {1}{0}{0}", newLine, this.GetEffectiveDateString());
			stringBuilder.AppendFormat("[Not After]{0}  {1}{0}{0}", newLine, this.GetExpirationDateString());
			stringBuilder.AppendFormat("[Thumbprint]{0}  {1}{0}", newLine, this.GetCertHashString());
			stringBuilder.Append(newLine);
			return stringBuilder.ToString();
		}

		private static byte[] Load(string fileName)
		{
			byte[] array = null;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Close();
			}
			return array;
		}

		public string Issuer
		{
			get
			{
				if (this.x509 == null)
				{
					throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
				}
				if (this.issuer_name == null)
				{
					this.issuer_name = X501.ToString(this.x509.GetIssuerName(), true, ", ", true);
				}
				return this.issuer_name;
			}
		}

		public string Subject
		{
			get
			{
				if (this.x509 == null)
				{
					throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
				}
				if (this.subject_name == null)
				{
					this.subject_name = X501.ToString(this.x509.GetSubjectName(), true, ", ", true);
				}
				return this.subject_name;
			}
		}

		[ComVisible(false)]
		public IntPtr Handle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			X509Certificate x509Certificate = obj as X509Certificate;
			return x509Certificate != null && this.Equals(x509Certificate);
		}

		[MonoTODO("X509ContentType.Pfx/Pkcs12 and SerializedCert are not supported")]
		[ComVisible(false)]
		public virtual byte[] Export(X509ContentType contentType)
		{
			return this.Export(contentType, null);
		}

		[MonoTODO("X509ContentType.Pfx/Pkcs12 and SerializedCert are not supported")]
		[ComVisible(false)]
		public virtual byte[] Export(X509ContentType contentType, string password)
		{
			byte[] array = ((password != null) ? Encoding.UTF8.GetBytes(password) : null);
			return this.Export(contentType, array);
		}

		[MonoTODO("X509ContentType.Pfx/Pkcs12 and SerializedCert are not supported. SecureString support is incomplete.")]
		public virtual byte[] Export(X509ContentType contentType, SecureString password)
		{
			byte[] array = ((password != null) ? password.GetBuffer() : null);
			return this.Export(contentType, array);
		}

		internal byte[] Export(X509ContentType contentType, byte[] password)
		{
			if (this.x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			byte[] rawData;
			try
			{
				switch (contentType)
				{
				case X509ContentType.Cert:
					rawData = this.x509.RawData;
					break;
				case X509ContentType.SerializedCert:
					throw new NotSupportedException();
				case X509ContentType.Pfx:
					throw new NotSupportedException();
				default:
				{
					string text = Locale.GetText("This certificate format '{0}' cannot be exported.", new object[] { contentType });
					throw new CryptographicException(text);
				}
				}
			}
			finally
			{
				if (password != null)
				{
					Array.Clear(password, 0, password.Length);
				}
			}
			return rawData;
		}

		[ComVisible(false)]
		public virtual void Import(byte[] rawData)
		{
			this.Import(rawData, null, X509KeyStorageFlags.DefaultKeySet);
		}

		[MonoTODO("missing KeyStorageFlags support")]
		[ComVisible(false)]
		public virtual void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Reset();
			if (password == null)
			{
				try
				{
					this.x509 = new X509Certificate(rawData);
				}
				catch (Exception ex)
				{
					try
					{
						PKCS12 pkcs = new PKCS12(rawData);
						if (pkcs.Certificates.Count > 0)
						{
							this.x509 = pkcs.Certificates[0];
						}
						else
						{
							this.x509 = null;
						}
					}
					catch
					{
						string text = Locale.GetText("Unable to decode certificate.");
						throw new CryptographicException(text, ex);
					}
				}
			}
			else
			{
				try
				{
					PKCS12 pkcs2 = new PKCS12(rawData, password);
					if (pkcs2.Certificates.Count > 0)
					{
						this.x509 = pkcs2.Certificates[0];
					}
					else
					{
						this.x509 = null;
					}
				}
				catch
				{
					this.x509 = new X509Certificate(rawData);
				}
			}
		}

		[MonoTODO("SecureString support is incomplete")]
		public virtual void Import(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(rawData, null, keyStorageFlags);
		}

		[ComVisible(false)]
		public virtual void Import(string fileName)
		{
			byte[] array = X509Certificate.Load(fileName);
			this.Import(array, null, X509KeyStorageFlags.DefaultKeySet);
		}

		[ComVisible(false)]
		[MonoTODO("missing KeyStorageFlags support")]
		public virtual void Import(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] array = X509Certificate.Load(fileName);
			this.Import(array, password, keyStorageFlags);
		}

		[MonoTODO("SecureString support is incomplete, missing KeyStorageFlags support")]
		public virtual void Import(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] array = X509Certificate.Load(fileName);
			this.Import(array, null, keyStorageFlags);
		}

		[ComVisible(false)]
		public virtual void Reset()
		{
			this.x509 = null;
			this.issuer_name = null;
			this.subject_name = null;
			this.hideDates = false;
			this.cachedCertificateHash = null;
		}

		private X509Certificate x509;

		private bool hideDates;

		private byte[] cachedCertificateHash;

		private string issuer_name;

		private string subject_name;

		internal struct CertificateContext
		{
			public uint dwCertEncodingType;

			public IntPtr pbCertEncoded;

			public uint cbCertEncoded;

			public IntPtr pCertInfo;

			public IntPtr hCertStore;
		}
	}
}
