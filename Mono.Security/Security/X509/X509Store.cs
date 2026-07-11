using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Mono.Security.X509.Extensions;

namespace Mono.Security.X509
{
	public class X509Store
	{
		internal X509Store(string path, bool crl)
		{
			this._storePath = path;
			this._crl = crl;
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
			if (this._certificates != null)
			{
				this._certificates.Clear();
			}
			this._certificates = null;
			if (this._crls != null)
			{
				this._crls.Clear();
			}
			this._crls = null;
		}

		public void Import(X509Certificate certificate)
		{
			this.CheckStore(this._storePath, true);
			string text = Path.Combine(this._storePath, this.GetUniqueName(certificate));
			if (!File.Exists(text))
			{
				using (FileStream fileStream = File.Create(text))
				{
					byte[] rawData = certificate.RawData;
					fileStream.Write(rawData, 0, rawData.Length);
					fileStream.Close();
				}
			}
		}

		public void Import(X509Crl crl)
		{
			this.CheckStore(this._storePath, true);
			string text = Path.Combine(this._storePath, this.GetUniqueName(crl));
			if (!File.Exists(text))
			{
				using (FileStream fileStream = File.Create(text))
				{
					byte[] rawData = crl.RawData;
					fileStream.Write(rawData, 0, rawData.Length);
				}
			}
		}

		public void Remove(X509Certificate certificate)
		{
			string text = Path.Combine(this._storePath, this.GetUniqueName(certificate));
			if (File.Exists(text))
			{
				File.Delete(text);
			}
		}

		public void Remove(X509Crl crl)
		{
			string text = Path.Combine(this._storePath, this.GetUniqueName(crl));
			if (File.Exists(text))
			{
				File.Delete(text);
			}
		}

		private string GetUniqueName(X509Certificate certificate)
		{
			byte[] array = this.GetUniqueName(certificate.Extensions);
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
			byte[] array = this.GetUniqueName(crl.Extensions);
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

		private byte[] GetUniqueName(X509ExtensionCollection extensions)
		{
			X509Extension x509Extension = extensions["2.5.29.14"];
			if (x509Extension == null)
			{
				return null;
			}
			SubjectKeyIdentifierExtension subjectKeyIdentifierExtension = new SubjectKeyIdentifierExtension(x509Extension);
			return subjectKeyIdentifierExtension.Identifier;
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
			byte[] array = this.Load(filename);
			return new X509Certificate(array);
		}

		private X509Crl LoadCrl(string filename)
		{
			byte[] array = this.Load(filename);
			return new X509Crl(array);
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
			string[] files = Directory.GetFiles(text, "*.cer");
			if (files != null && files.Length > 0)
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
			if (files != null && files.Length > 0)
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

		private string _storePath;

		private X509CertificateCollection _certificates;

		private ArrayList _crls;

		private bool _crl;

		private string _name;
	}
}
