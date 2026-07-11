using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Mono.Security.Authenticode;

namespace System.Security.Cryptography.X509Certificates
{
	[ComVisible(true)]
	[MonoTODO("X509ContentType.SerializedCert isn't supported (anywhere in the class)")]
	[Serializable]
	public class X509Certificate : IDeserializationCallback, ISerializable, IDisposable
	{
		public static X509Certificate CreateFromCertFile(string filename)
		{
			return new X509Certificate(File.ReadAllBytes(filename));
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
				throw new COMException(Locale.GetText("Couldn't extract digital signature from {0}.", new object[] { filename }), ex);
			}
			throw new CryptographicException(Locale.GetText("{0} isn't signed.", new object[] { filename }));
		}

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
			this.impl = X509Helper.InitFromHandle(handle);
		}

		internal X509Certificate(X509CertificateImpl impl)
		{
			if (impl == null)
			{
				throw new ArgumentNullException("impl");
			}
			this.impl = X509Helper.InitFromCertificate(impl);
		}

		public X509Certificate(X509Certificate cert)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			this.impl = X509Helper.InitFromCertificate(cert);
			this.hideDates = false;
		}

		internal void ImportHandle(X509CertificateImpl impl)
		{
			this.Reset();
			this.impl = impl;
		}

		internal X509CertificateImpl Impl
		{
			get
			{
				X509Helper.ThrowIfContextInvalid(this.impl);
				return this.impl;
			}
		}

		internal bool IsValid
		{
			get
			{
				return X509Helper.IsValid(this.impl);
			}
		}

		internal void ThrowIfContextInvalid()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
		}

		public virtual bool Equals(X509Certificate other)
		{
			if (other == null)
			{
				return false;
			}
			if (X509Helper.IsValid(other.impl))
			{
				return object.Equals(this.impl, other.impl);
			}
			if (!X509Helper.IsValid(this.impl))
			{
				return true;
			}
			throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
		}

		public virtual byte[] GetCertHash()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
			return this.impl.GetCertHash();
		}

		public virtual string GetCertHashString()
		{
			return X509Helper.ToHexString(this.GetCertHash());
		}

		public virtual string GetEffectiveDateString()
		{
			if (this.hideDates)
			{
				return null;
			}
			X509Helper.ThrowIfContextInvalid(this.impl);
			return this.impl.GetValidFrom().ToLocalTime().ToString();
		}

		public virtual string GetExpirationDateString()
		{
			if (this.hideDates)
			{
				return null;
			}
			X509Helper.ThrowIfContextInvalid(this.impl);
			return this.impl.GetValidUntil().ToLocalTime().ToString();
		}

		public virtual string GetFormat()
		{
			return "X509";
		}

		public override int GetHashCode()
		{
			if (!X509Helper.IsValid(this.impl))
			{
				return 0;
			}
			return this.impl.GetHashCode();
		}

		[Obsolete("Use the Issuer property.")]
		public virtual string GetIssuerName()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
			return this.impl.GetIssuerName(true);
		}

		public virtual string GetKeyAlgorithm()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
			return this.impl.GetKeyAlgorithm();
		}

		public virtual byte[] GetKeyAlgorithmParameters()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
			byte[] keyAlgorithmParameters = this.impl.GetKeyAlgorithmParameters();
			if (keyAlgorithmParameters == null)
			{
				throw new CryptographicException(Locale.GetText("Parameters not part of the certificate"));
			}
			return keyAlgorithmParameters;
		}

		public virtual string GetKeyAlgorithmParametersString()
		{
			return X509Helper.ToHexString(this.GetKeyAlgorithmParameters());
		}

		[Obsolete("Use the Subject property.")]
		public virtual string GetName()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
			return this.impl.GetSubjectName(true);
		}

		public virtual byte[] GetPublicKey()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
			return this.impl.GetPublicKey();
		}

		public virtual string GetPublicKeyString()
		{
			return X509Helper.ToHexString(this.GetPublicKey());
		}

		public virtual byte[] GetRawCertData()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
			return this.impl.GetRawCertData();
		}

		public virtual string GetRawCertDataString()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
			return X509Helper.ToHexString(this.impl.GetRawCertData());
		}

		public virtual byte[] GetSerialNumber()
		{
			X509Helper.ThrowIfContextInvalid(this.impl);
			return this.impl.GetSerialNumber();
		}

		public virtual string GetSerialNumberString()
		{
			byte[] serialNumber = this.GetSerialNumber();
			Array.Reverse<byte>(serialNumber);
			return X509Helper.ToHexString(serialNumber);
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public virtual string ToString(bool fVerbose)
		{
			if (!fVerbose || !X509Helper.IsValid(this.impl))
			{
				return base.ToString();
			}
			return this.impl.ToString(true);
		}

		protected static string FormatDate(DateTime date)
		{
			throw new NotImplementedException();
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

		public string Issuer
		{
			get
			{
				X509Helper.ThrowIfContextInvalid(this.impl);
				if (this.issuer_name == null)
				{
					this.issuer_name = this.impl.GetIssuerName(false);
				}
				return this.issuer_name;
			}
		}

		public string Subject
		{
			get
			{
				X509Helper.ThrowIfContextInvalid(this.impl);
				if (this.subject_name == null)
				{
					this.subject_name = this.impl.GetSubjectName(false);
				}
				return this.subject_name;
			}
		}

		[ComVisible(false)]
		public IntPtr Handle
		{
			get
			{
				if (X509Helper.IsValid(this.impl))
				{
					return this.impl.Handle;
				}
				return IntPtr.Zero;
			}
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			X509Certificate x509Certificate = obj as X509Certificate;
			return x509Certificate != null && this.Equals(x509Certificate);
		}

		[ComVisible(false)]
		[MonoTODO("X509ContentType.Pfx/Pkcs12 and SerializedCert are not supported")]
		public virtual byte[] Export(X509ContentType contentType)
		{
			return this.Export(contentType, null);
		}

		[MonoTODO("X509ContentType.Pfx/Pkcs12 and SerializedCert are not supported")]
		[ComVisible(false)]
		public virtual byte[] Export(X509ContentType contentType, string password)
		{
			byte[] array = ((password == null) ? null : Encoding.UTF8.GetBytes(password));
			return this.Export(contentType, array);
		}

		[MonoTODO("X509ContentType.Pfx/Pkcs12 and SerializedCert are not supported. SecureString support is incomplete.")]
		public virtual byte[] Export(X509ContentType contentType, SecureString password)
		{
			byte[] array = ((password == null) ? null : password.GetBuffer());
			return this.Export(contentType, array);
		}

		internal byte[] Export(X509ContentType contentType, byte[] password)
		{
			byte[] array;
			try
			{
				X509Helper.ThrowIfContextInvalid(this.impl);
				array = this.impl.Export(contentType, password);
			}
			finally
			{
				if (password != null)
				{
					Array.Clear(password, 0, password.Length);
				}
			}
			return array;
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
			this.impl = X509Helper.Import(rawData, password, keyStorageFlags);
		}

		[MonoTODO("SecureString support is incomplete")]
		public virtual void Import(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			this.Import(rawData, null, keyStorageFlags);
		}

		[ComVisible(false)]
		public virtual void Import(string fileName)
		{
			byte[] array = File.ReadAllBytes(fileName);
			this.Import(array, null, X509KeyStorageFlags.DefaultKeySet);
		}

		[ComVisible(false)]
		[MonoTODO("missing KeyStorageFlags support")]
		public virtual void Import(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] array = File.ReadAllBytes(fileName);
			this.Import(array, password, keyStorageFlags);
		}

		[MonoTODO("SecureString support is incomplete, missing KeyStorageFlags support")]
		public virtual void Import(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] array = File.ReadAllBytes(fileName);
			this.Import(array, null, keyStorageFlags);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (!X509Helper.IsValid(this.impl))
			{
				throw new NullReferenceException();
			}
			info.AddValue("RawData", this.impl.GetRawCertData());
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Reset();
			}
		}

		[ComVisible(false)]
		public virtual void Reset()
		{
			if (this.impl != null)
			{
				this.impl.Dispose();
				this.impl = null;
			}
			this.issuer_name = null;
			this.subject_name = null;
			this.hideDates = false;
		}

		private X509CertificateImpl impl;

		private bool hideDates;

		private string issuer_name;

		private string subject_name;
	}
}
