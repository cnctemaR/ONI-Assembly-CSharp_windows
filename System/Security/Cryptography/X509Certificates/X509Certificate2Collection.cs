using System;
using System.Globalization;

namespace System.Security.Cryptography.X509Certificates
{
	public class X509Certificate2Collection : X509CertificateCollection
	{
		public X509Certificate2Collection()
		{
		}

		public X509Certificate2Collection(X509Certificate2Collection certificates)
		{
			this.AddRange(certificates);
		}

		public X509Certificate2Collection(X509Certificate2 certificate)
		{
			this.Add(certificate);
		}

		public X509Certificate2Collection(X509Certificate2[] certificates)
		{
			this.AddRange(certificates);
		}

		public X509Certificate2 this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("negative index");
				}
				if (index >= base.InnerList.Count)
				{
					throw new ArgumentOutOfRangeException("index >= Count");
				}
				return (X509Certificate2)base.InnerList[index];
			}
			set
			{
				base.InnerList[index] = value;
			}
		}

		public int Add(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			return base.InnerList.Add(certificate);
		}

		[global::System.MonoTODO("Method isn't transactional (like documented)")]
		public void AddRange(X509Certificate2[] certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			for (int i = 0; i < certificates.Length; i++)
			{
				base.InnerList.Add(certificates[i]);
			}
		}

		[global::System.MonoTODO("Method isn't transactional (like documented)")]
		public void AddRange(X509Certificate2Collection certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			base.InnerList.AddRange(certificates);
		}

		public bool Contains(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			foreach (object obj in base.InnerList)
			{
				X509Certificate2 x509Certificate = (X509Certificate2)obj;
				if (x509Certificate.Equals(certificate))
				{
					return true;
				}
			}
			return false;
		}

		[global::System.MonoTODO("only support X509ContentType.Cert")]
		public byte[] Export(X509ContentType contentType)
		{
			return this.Export(contentType, null);
		}

		[global::System.MonoTODO("only support X509ContentType.Cert")]
		public byte[] Export(X509ContentType contentType, string password)
		{
			switch (contentType)
			{
			case X509ContentType.Cert:
			case X509ContentType.SerializedCert:
			case X509ContentType.Pfx:
				if (this.Count > 0)
				{
					return this[this.Count - 1].Export(contentType, password);
				}
				break;
			case X509ContentType.SerializedStore:
				break;
			case X509ContentType.Pkcs7:
				break;
			default:
			{
				string text = global::Locale.GetText("Cannot export certificate(s) to the '{0}' format", new object[] { contentType });
				throw new CryptographicException(text);
			}
			}
			return null;
		}

		[global::System.MonoTODO("Does not support X509FindType.FindByTemplateName, FindByApplicationPolicy and FindByCertificatePolicy")]
		public X509Certificate2Collection Find(X509FindType findType, object findValue, bool validOnly)
		{
			if (findValue == null)
			{
				throw new ArgumentNullException("findValue");
			}
			string text = string.Empty;
			string text2 = string.Empty;
			X509KeyUsageFlags x509KeyUsageFlags = X509KeyUsageFlags.None;
			DateTime dateTime = DateTime.MinValue;
			switch (findType)
			{
			case X509FindType.FindByThumbprint:
			case X509FindType.FindBySubjectName:
			case X509FindType.FindBySubjectDistinguishedName:
			case X509FindType.FindByIssuerName:
			case X509FindType.FindByIssuerDistinguishedName:
			case X509FindType.FindBySerialNumber:
			case X509FindType.FindByTemplateName:
			case X509FindType.FindBySubjectKeyIdentifier:
				try
				{
					text = (string)findValue;
				}
				catch (Exception ex)
				{
					string text3 = global::Locale.GetText("Invalid find value type '{0}', expected '{1}'.", new object[]
					{
						findValue.GetType(),
						"string"
					});
					throw new CryptographicException(text3, ex);
				}
				break;
			case X509FindType.FindByTimeValid:
			case X509FindType.FindByTimeNotYetValid:
			case X509FindType.FindByTimeExpired:
				try
				{
					dateTime = (DateTime)findValue;
				}
				catch (Exception ex2)
				{
					string text4 = global::Locale.GetText("Invalid find value type '{0}', expected '{1}'.", new object[]
					{
						findValue.GetType(),
						"X509DateTime"
					});
					throw new CryptographicException(text4, ex2);
				}
				break;
			case X509FindType.FindByApplicationPolicy:
			case X509FindType.FindByCertificatePolicy:
			case X509FindType.FindByExtension:
				try
				{
					text2 = (string)findValue;
				}
				catch (Exception ex3)
				{
					string text5 = global::Locale.GetText("Invalid find value type '{0}', expected '{1}'.", new object[]
					{
						findValue.GetType(),
						"X509KeyUsageFlags"
					});
					throw new CryptographicException(text5, ex3);
				}
				try
				{
					CryptoConfig.EncodeOID(text2);
				}
				catch (CryptographicUnexpectedOperationException)
				{
					string text6 = global::Locale.GetText("Invalid OID value '{0}'.", new object[] { text2 });
					throw new ArgumentException("findValue", text6);
				}
				break;
			case X509FindType.FindByKeyUsage:
				try
				{
					x509KeyUsageFlags = (X509KeyUsageFlags)((int)findValue);
				}
				catch (Exception ex4)
				{
					string text7 = global::Locale.GetText("Invalid find value type '{0}', expected '{1}'.", new object[]
					{
						findValue.GetType(),
						"X509KeyUsageFlags"
					});
					throw new CryptographicException(text7, ex4);
				}
				break;
			default:
			{
				string text8 = global::Locale.GetText("Invalid find type '{0}'.", new object[] { findType });
				throw new CryptographicException(text8);
			}
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			foreach (object obj in base.InnerList)
			{
				X509Certificate2 x509Certificate = (X509Certificate2)obj;
				bool flag = false;
				switch (findType)
				{
				case X509FindType.FindByThumbprint:
					flag = string.Compare(text, x509Certificate.Thumbprint, true, invariantCulture) == 0 || string.Compare(text, x509Certificate.GetCertHashString(), true, invariantCulture) == 0;
					break;
				case X509FindType.FindBySubjectName:
				{
					string nameInfo = x509Certificate.GetNameInfo(X509NameType.SimpleName, false);
					flag = nameInfo.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0;
					break;
				}
				case X509FindType.FindBySubjectDistinguishedName:
					flag = string.Compare(text, x509Certificate.Subject, true, invariantCulture) == 0;
					break;
				case X509FindType.FindByIssuerName:
				{
					string nameInfo2 = x509Certificate.GetNameInfo(X509NameType.SimpleName, true);
					flag = nameInfo2.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0;
					break;
				}
				case X509FindType.FindByIssuerDistinguishedName:
					flag = string.Compare(text, x509Certificate.Issuer, true, invariantCulture) == 0;
					break;
				case X509FindType.FindBySerialNumber:
					flag = string.Compare(text, x509Certificate.SerialNumber, true, invariantCulture) == 0;
					break;
				case X509FindType.FindByTimeValid:
					flag = dateTime >= x509Certificate.NotBefore && dateTime <= x509Certificate.NotAfter;
					break;
				case X509FindType.FindByTimeNotYetValid:
					flag = dateTime < x509Certificate.NotBefore;
					break;
				case X509FindType.FindByTimeExpired:
					flag = dateTime > x509Certificate.NotAfter;
					break;
				case X509FindType.FindByApplicationPolicy:
					flag = x509Certificate.Extensions.Count == 0;
					break;
				case X509FindType.FindByExtension:
					flag = x509Certificate.Extensions[text2] != null;
					break;
				case X509FindType.FindByKeyUsage:
				{
					X509KeyUsageExtension x509KeyUsageExtension = x509Certificate.Extensions["2.5.29.15"] as X509KeyUsageExtension;
					flag = x509KeyUsageExtension == null || (x509KeyUsageExtension.KeyUsages & x509KeyUsageFlags) == x509KeyUsageFlags;
					break;
				}
				case X509FindType.FindBySubjectKeyIdentifier:
				{
					X509SubjectKeyIdentifierExtension x509SubjectKeyIdentifierExtension = x509Certificate.Extensions["2.5.29.14"] as X509SubjectKeyIdentifierExtension;
					if (x509SubjectKeyIdentifierExtension != null)
					{
						flag = string.Compare(text, x509SubjectKeyIdentifierExtension.SubjectKeyIdentifier, true, invariantCulture) == 0;
					}
					break;
				}
				}
				if (flag)
				{
					if (validOnly)
					{
						try
						{
							if (x509Certificate.Verify())
							{
								x509Certificate2Collection.Add(x509Certificate);
							}
						}
						catch
						{
						}
					}
					else
					{
						x509Certificate2Collection.Add(x509Certificate);
					}
				}
			}
			return x509Certificate2Collection;
		}

		public new X509Certificate2Enumerator GetEnumerator()
		{
			return new X509Certificate2Enumerator(this);
		}

		[global::System.MonoTODO("same limitations as X509Certificate2.Import")]
		public void Import(byte[] rawData)
		{
			X509Certificate2 x509Certificate = new X509Certificate2();
			x509Certificate.Import(rawData);
			this.Add(x509Certificate);
		}

		[global::System.MonoTODO("same limitations as X509Certificate2.Import")]
		public void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			X509Certificate2 x509Certificate = new X509Certificate2();
			x509Certificate.Import(rawData, password, keyStorageFlags);
			this.Add(x509Certificate);
		}

		[global::System.MonoTODO("same limitations as X509Certificate2.Import")]
		public void Import(string fileName)
		{
			X509Certificate2 x509Certificate = new X509Certificate2();
			x509Certificate.Import(fileName);
			this.Add(x509Certificate);
		}

		[global::System.MonoTODO("same limitations as X509Certificate2.Import")]
		public void Import(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			X509Certificate2 x509Certificate = new X509Certificate2();
			x509Certificate.Import(fileName, password, keyStorageFlags);
			this.Add(x509Certificate);
		}

		public void Insert(int index, X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("negative index");
			}
			if (index >= base.InnerList.Count)
			{
				throw new ArgumentOutOfRangeException("index >= Count");
			}
			base.InnerList.Insert(index, certificate);
		}

		public void Remove(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			for (int i = 0; i < base.InnerList.Count; i++)
			{
				X509Certificate x509Certificate = (X509Certificate)base.InnerList[i];
				if (x509Certificate.Equals(certificate))
				{
					base.InnerList.RemoveAt(i);
					return;
				}
			}
		}

		[global::System.MonoTODO("Method isn't transactional (like documented)")]
		public void RemoveRange(X509Certificate2[] certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificate");
			}
			foreach (X509Certificate2 x509Certificate in certificates)
			{
				this.Remove(x509Certificate);
			}
		}

		[global::System.MonoTODO("Method isn't transactional (like documented)")]
		public void RemoveRange(X509Certificate2Collection certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificate");
			}
			foreach (X509Certificate2 x509Certificate in certificates)
			{
				this.Remove(x509Certificate);
			}
		}
	}
}
