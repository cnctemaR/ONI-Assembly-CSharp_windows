using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Mono.Security.Cryptography;
using Mono.Security.X509.Extensions;

namespace Mono.Security.X509
{
	public class X509Store
	{
		internal X509Store(string path, bool crl, bool newFormat)
		{
			this._storePath = path;
			this._crl = crl;
			this._newFormat = newFormat;
		}

		public X509CertificateCollection Certificates
		{
			get
			{
				if (this._certificates == null)
				{
					this._certificates = this.BuildCertificatesCollection(this._storePath);
				}
				return this._certificates;
			}
		}

		public ArrayList Crls
		{
			get
			{
				if (!this._crl)
				{
					this._crls = new ArrayList();
				}
				if (this._crls == null)
				{
					this._crls = this.BuildCrlsCollection(this._storePath);
				}
				return this._crls;
			}
		}

		public string Name
		{
			get
			{
				if (this._name == null)
				{
					int num = this._storePath.LastIndexOf(Path.DirectorySeparatorChar);
					this._name = this._storePath.Substring(num + 1);
				}
				return this._name;
			}
		}

		public void Clear()
		{
			this.ClearCertificates();
			this.ClearCrls();
		}

		private void ClearCertificates()
		{
			if (this._certificates != null)
			{
				this._certificates.Clear();
			}
			this._certificates = null;
		}

		private void ClearCrls()
		{
			if (this._crls != null)
			{
				this._crls.Clear();
			}
			this._crls = null;
		}

		public void Import(X509Certificate certificate)
		{
			this.CheckStore(this._storePath, true);
			if (this._newFormat)
			{
				this.ImportNewFormat(certificate);
				return;
			}
			string text = Path.Combine(this._storePath, this.GetUniqueName(certificate, null));
			if (!File.Exists(text))
			{
				text = Path.Combine(this._storePath, this.GetUniqueNameWithSerial(certificate));
				if (!File.Exists(text))
				{
					using (FileStream fileStream = File.Create(text))
					{
						byte[] rawData = certificate.RawData;
						fileStream.Write(rawData, 0, rawData.Length);
						fileStream.Close();
					}
					this.ClearCertificates();
				}
			}
			else
			{
				string text2 = Path.Combine(this._storePath, this.GetUniqueNameWithSerial(certificate));
				if (this.GetUniqueNameWithSerial(this.LoadCertificate(text)) != this.GetUniqueNameWithSerial(certificate))
				{
					using (FileStream fileStream2 = File.Create(text2))
					{
						byte[] rawData2 = certificate.RawData;
						fileStream2.Write(rawData2, 0, rawData2.Length);
						fileStream2.Close();
					}
					this.ClearCertificates();
				}
			}
			CspParameters cspParameters = new CspParameters();
			cspParameters.KeyContainerName = CryptoConvert.ToHex(certificate.Hash);
			if (this._storePath.StartsWith(X509StoreManager.LocalMachinePath) || this._storePath.StartsWith(X509StoreManager.NewLocalMachinePath))
			{
				cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
			}
			this.ImportPrivateKey(certificate, cspParameters);
		}

		public void Import(X509Crl crl)
		{
			this.CheckStore(this._storePath, true);
			if (this._newFormat)
			{
				throw new NotSupportedException();
			}
			string text = Path.Combine(this._storePath, this.GetUniqueName(crl));
			if (!File.Exists(text))
			{
				using (FileStream fileStream = File.Create(text))
				{
					byte[] rawData = crl.RawData;
					fileStream.Write(rawData, 0, rawData.Length);
				}
				this.ClearCrls();
			}
		}

		public void Remove(X509Certificate certificate)
		{
			if (this._newFormat)
			{
				this.RemoveNewFormat(certificate);
				return;
			}
			string text = Path.Combine(this._storePath, this.GetUniqueNameWithSerial(certificate));
			if (File.Exists(text))
			{
				File.Delete(text);
				this.ClearCertificates();
				return;
			}
			text = Path.Combine(this._storePath, this.GetUniqueName(certificate, null));
			if (File.Exists(text))
			{
				File.Delete(text);
				this.ClearCertificates();
			}
		}

		public void Remove(X509Crl crl)
		{
			if (this._newFormat)
			{
				throw new NotSupportedException();
			}
			string text = Path.Combine(this._storePath, this.GetUniqueName(crl));
			if (File.Exists(text))
			{
				File.Delete(text);
				this.ClearCrls();
			}
		}

		private void ImportNewFormat(X509Certificate certificate)
		{
			using (X509Certificate x509Certificate = new X509Certificate(certificate.RawData))
			{
				long subjectNameHash = X509Helper2.GetSubjectNameHash(x509Certificate);
				string text = Path.Combine(this._storePath, string.Format("{0:x8}.0", subjectNameHash));
				if (!File.Exists(text))
				{
					using (FileStream fileStream = File.Create(text))
					{
						X509Helper2.ExportAsPEM(x509Certificate, fileStream, true);
					}
					this.ClearCertificates();
				}
			}
		}

		private void RemoveNewFormat(X509Certificate certificate)
		{
			using (X509Certificate x509Certificate = new X509Certificate(certificate.RawData))
			{
				long subjectNameHash = X509Helper2.GetSubjectNameHash(x509Certificate);
				string text = Path.Combine(this._storePath, string.Format("{0:x8}.0", subjectNameHash));
				if (File.Exists(text))
				{
					File.Delete(text);
					this.ClearCertificates();
				}
			}
		}

		private string GetUniqueNameWithSerial(X509Certificate certificate)
		{
			return this.GetUniqueName(certificate, certificate.SerialNumber);
		}

		private string GetUniqueName(X509Certificate certificate, byte[] serial = null)
		{
			byte[] array = this.GetUniqueName(certificate.Extensions, serial);
			string text;
			if (array == null)
			{
				text = "tbp";
				array = certificate.Hash;
			}
			else
			{
				text = "ski";
			}
			return this.GetUniqueName(text, array, ".cer");
		}

		private string GetUniqueName(X509Crl crl)
		{
			byte[] array = this.GetUniqueName(crl.Extensions, null);
			string text;
			if (array == null)
			{
				text = "tbp";
				array = crl.Hash;
			}
			else
			{
				text = "ski";
			}
			return this.GetUniqueName(text, array, ".crl");
		}

		private byte[] GetUniqueName(X509ExtensionCollection extensions, byte[] serial = null)
		{
			X509Extension x509Extension = extensions["2.5.29.14"];
			if (x509Extension == null)
			{
				return null;
			}
			SubjectKeyIdentifierExtension subjectKeyIdentifierExtension = new SubjectKeyIdentifierExtension(x509Extension);
			if (serial == null)
			{
				return subjectKeyIdentifierExtension.Identifier;
			}
			byte[] array = new byte[subjectKeyIdentifierExtension.Identifier.Length + serial.Length];
			Buffer.BlockCopy(subjectKeyIdentifierExtension.Identifier, 0, array, 0, subjectKeyIdentifierExtension.Identifier.Length);
			Buffer.BlockCopy(serial, 0, array, subjectKeyIdentifierExtension.Identifier.Length, serial.Length);
			return array;
		}

		private string GetUniqueName(string method, byte[] name, string fileExtension)
		{
			StringBuilder stringBuilder = new StringBuilder(method);
			stringBuilder.Append("-");
			foreach (byte b in name)
			{
				stringBuilder.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}
			stringBuilder.Append(fileExtension);
			return stringBuilder.ToString();
		}

		private byte[] Load(string filename)
		{
			byte[] array = null;
			using (FileStream fileStream = File.OpenRead(filename))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Close();
			}
			return array;
		}

		private X509Certificate LoadCertificate(string filename)
		{
			X509Certificate x509Certificate = new X509Certificate(this.Load(filename));
			CspParameters cspParameters = new CspParameters();
			cspParameters.KeyContainerName = CryptoConvert.ToHex(x509Certificate.Hash);
			if (this._storePath.StartsWith(X509StoreManager.LocalMachinePath) || this._storePath.StartsWith(X509StoreManager.NewLocalMachinePath))
			{
				cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
			}
			KeyPairPersistence keyPairPersistence = new KeyPairPersistence(cspParameters);
			try
			{
				if (!keyPairPersistence.Load())
				{
					return x509Certificate;
				}
			}
			catch
			{
				return x509Certificate;
			}
			if (x509Certificate.RSA != null)
			{
				x509Certificate.RSA = new RSACryptoServiceProvider(cspParameters);
			}
			else if (x509Certificate.DSA != null)
			{
				x509Certificate.DSA = new DSACryptoServiceProvider(cspParameters);
			}
			return x509Certificate;
		}

		private X509Crl LoadCrl(string filename)
		{
			return new X509Crl(this.Load(filename));
		}

		private bool CheckStore(string path, bool throwException)
		{
			bool flag;
			try
			{
				if (Directory.Exists(path))
				{
					flag = true;
				}
				else
				{
					Directory.CreateDirectory(path);
					flag = Directory.Exists(path);
				}
			}
			catch
			{
				if (throwException)
				{
					throw;
				}
				flag = false;
			}
			return flag;
		}

		private X509CertificateCollection BuildCertificatesCollection(string storeName)
		{
			X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
			string text = Path.Combine(this._storePath, storeName);
			if (!this.CheckStore(text, false))
			{
				return x509CertificateCollection;
			}
			string[] files = Directory.GetFiles(text, this._newFormat ? "*.0" : "*.cer");
			if (files != null && files.Length != 0)
			{
				foreach (string text2 in files)
				{
					try
					{
						X509Certificate x509Certificate = this.LoadCertificate(text2);
						x509CertificateCollection.Add(x509Certificate);
					}
					catch
					{
					}
				}
			}
			return x509CertificateCollection;
		}

		private ArrayList BuildCrlsCollection(string storeName)
		{
			ArrayList arrayList = new ArrayList();
			string text = Path.Combine(this._storePath, storeName);
			if (!this.CheckStore(text, false))
			{
				return arrayList;
			}
			string[] files = Directory.GetFiles(text, "*.crl");
			if (files != null && files.Length != 0)
			{
				foreach (string text2 in files)
				{
					try
					{
						X509Crl x509Crl = this.LoadCrl(text2);
						arrayList.Add(x509Crl);
					}
					catch
					{
					}
				}
			}
			return arrayList;
		}

		private void ImportPrivateKey(X509Certificate certificate, CspParameters cspParams)
		{
			RSACryptoServiceProvider rsacryptoServiceProvider = certificate.RSA as RSACryptoServiceProvider;
			if (rsacryptoServiceProvider != null)
			{
				if (rsacryptoServiceProvider.PublicOnly)
				{
					return;
				}
				RSACryptoServiceProvider rsacryptoServiceProvider2 = new RSACryptoServiceProvider(cspParams);
				rsacryptoServiceProvider2.ImportParameters(rsacryptoServiceProvider.ExportParameters(true));
				rsacryptoServiceProvider2.PersistKeyInCsp = true;
				return;
			}
			else
			{
				RSAManaged rsamanaged = certificate.RSA as RSAManaged;
				if (rsamanaged == null)
				{
					DSACryptoServiceProvider dsacryptoServiceProvider = certificate.DSA as DSACryptoServiceProvider;
					if (dsacryptoServiceProvider != null)
					{
						if (dsacryptoServiceProvider.PublicOnly)
						{
							return;
						}
						DSACryptoServiceProvider dsacryptoServiceProvider2 = new DSACryptoServiceProvider(cspParams);
						dsacryptoServiceProvider2.ImportParameters(dsacryptoServiceProvider.ExportParameters(true));
						dsacryptoServiceProvider2.PersistKeyInCsp = true;
					}
					return;
				}
				if (rsamanaged.PublicOnly)
				{
					return;
				}
				RSACryptoServiceProvider rsacryptoServiceProvider3 = new RSACryptoServiceProvider(cspParams);
				rsacryptoServiceProvider3.ImportParameters(rsamanaged.ExportParameters(true));
				rsacryptoServiceProvider3.PersistKeyInCsp = true;
				return;
			}
		}

		private string _storePath;

		private X509CertificateCollection _certificates;

		private ArrayList _crls;

		private bool _crl;

		private bool _newFormat;

		private string _name;
	}
}
