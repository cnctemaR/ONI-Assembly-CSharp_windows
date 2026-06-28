using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Mono.Security.X509;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfoX509Data : KeyInfoClause
	{
		public KeyInfoX509Data()
		{
		}

		public KeyInfoX509Data(byte[] rgbCert)
		{
			this.AddCertificate(new global::System.Security.Cryptography.X509Certificates.X509Certificate(rgbCert));
		}

		public KeyInfoX509Data(global::System.Security.Cryptography.X509Certificates.X509Certificate cert)
		{
			this.AddCertificate(cert);
		}

		public KeyInfoX509Data(global::System.Security.Cryptography.X509Certificates.X509Certificate cert, X509IncludeOption includeOption)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			switch (includeOption)
			{
			case X509IncludeOption.None:
			case X509IncludeOption.EndCertOnly:
				this.AddCertificate(cert);
				break;
			case X509IncludeOption.ExcludeRoot:
				this.AddCertificatesChainFrom(cert, false);
				break;
			case X509IncludeOption.WholeChain:
				this.AddCertificatesChainFrom(cert, true);
				break;
			}
		}

		private void AddCertificatesChainFrom(global::System.Security.Cryptography.X509Certificates.X509Certificate cert, bool root)
		{
			global::System.Security.Cryptography.X509Certificates.X509Chain x509Chain = new global::System.Security.Cryptography.X509Certificates.X509Chain();
			x509Chain.Build(new X509Certificate2(cert));
			foreach (X509ChainElement x509ChainElement in x509Chain.ChainElements)
			{
				byte[] array = x509ChainElement.Certificate.RawData;
				if (!root)
				{
					Mono.Security.X509.X509Certificate x509Certificate = new Mono.Security.X509.X509Certificate(array);
					if (x509Certificate.IsSelfSigned)
					{
						array = null;
					}
				}
				if (array != null)
				{
					this.AddCertificate(new global::System.Security.Cryptography.X509Certificates.X509Certificate(array));
				}
			}
		}

		public ArrayList Certificates
		{
			get
			{
				return this.X509CertificateList;
			}
		}

		public byte[] CRL
		{
			get
			{
				return this.x509crl;
			}
			set
			{
				this.x509crl = value;
			}
		}

		public ArrayList IssuerSerials
		{
			get
			{
				return this.IssuerSerialList;
			}
		}

		public ArrayList SubjectKeyIds
		{
			get
			{
				return this.SubjectKeyIdList;
			}
		}

		public ArrayList SubjectNames
		{
			get
			{
				return this.SubjectNameList;
			}
		}

		public void AddCertificate(global::System.Security.Cryptography.X509Certificates.X509Certificate certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (this.X509CertificateList == null)
			{
				this.X509CertificateList = new ArrayList();
			}
			this.X509CertificateList.Add(certificate);
		}

		public void AddIssuerSerial(string issuerName, string serialNumber)
		{
			if (issuerName == null)
			{
				throw new ArgumentException("issuerName");
			}
			if (this.IssuerSerialList == null)
			{
				this.IssuerSerialList = new ArrayList();
			}
			X509IssuerSerial x509IssuerSerial = new X509IssuerSerial(issuerName, serialNumber);
			this.IssuerSerialList.Add(x509IssuerSerial);
		}

		public void AddSubjectKeyId(byte[] subjectKeyId)
		{
			if (this.SubjectKeyIdList == null)
			{
				this.SubjectKeyIdList = new ArrayList();
			}
			this.SubjectKeyIdList.Add(subjectKeyId);
		}

		[ComVisible(false)]
		public void AddSubjectKeyId(string subjectKeyId)
		{
			if (this.SubjectKeyIdList == null)
			{
				this.SubjectKeyIdList = new ArrayList();
			}
			byte[] array = null;
			if (subjectKeyId != null)
			{
				array = Convert.FromBase64String(subjectKeyId);
			}
			this.SubjectKeyIdList.Add(array);
		}

		public void AddSubjectName(string subjectName)
		{
			if (this.SubjectNameList == null)
			{
				this.SubjectNameList = new ArrayList();
			}
			this.SubjectNameList.Add(subjectName);
		}

		public override XmlElement GetXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("X509Data", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement.SetAttribute("xmlns", "http://www.w3.org/2000/09/xmldsig#");
			if (this.IssuerSerialList != null && this.IssuerSerialList.Count > 0)
			{
				foreach (object obj in this.IssuerSerialList)
				{
					X509IssuerSerial x509IssuerSerial = (X509IssuerSerial)obj;
					XmlElement xmlElement2 = xmlDocument.CreateElement("X509IssuerSerial", "http://www.w3.org/2000/09/xmldsig#");
					XmlElement xmlElement3 = xmlDocument.CreateElement("X509IssuerName", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement3.InnerText = x509IssuerSerial.IssuerName;
					xmlElement2.AppendChild(xmlElement3);
					XmlElement xmlElement4 = xmlDocument.CreateElement("X509SerialNumber", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement4.InnerText = x509IssuerSerial.SerialNumber;
					xmlElement2.AppendChild(xmlElement4);
					xmlElement.AppendChild(xmlElement2);
				}
			}
			if (this.SubjectKeyIdList != null && this.SubjectKeyIdList.Count > 0)
			{
				foreach (object obj2 in this.SubjectKeyIdList)
				{
					byte[] array = (byte[])obj2;
					XmlElement xmlElement5 = xmlDocument.CreateElement("X509SKI", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement5.InnerText = Convert.ToBase64String(array);
					xmlElement.AppendChild(xmlElement5);
				}
			}
			if (this.SubjectNameList != null && this.SubjectNameList.Count > 0)
			{
				foreach (object obj3 in this.SubjectNameList)
				{
					string text = (string)obj3;
					XmlElement xmlElement6 = xmlDocument.CreateElement("X509SubjectName", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement6.InnerText = text;
					xmlElement.AppendChild(xmlElement6);
				}
			}
			if (this.X509CertificateList != null && this.X509CertificateList.Count > 0)
			{
				foreach (object obj4 in this.X509CertificateList)
				{
					global::System.Security.Cryptography.X509Certificates.X509Certificate x509Certificate = (global::System.Security.Cryptography.X509Certificates.X509Certificate)obj4;
					XmlElement xmlElement7 = xmlDocument.CreateElement("X509Certificate", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement7.InnerText = Convert.ToBase64String(x509Certificate.GetRawCertData());
					xmlElement.AppendChild(xmlElement7);
				}
			}
			if (this.x509crl != null)
			{
				XmlElement xmlElement8 = xmlDocument.CreateElement("X509CRL", "http://www.w3.org/2000/09/xmldsig#");
				xmlElement8.InnerText = Convert.ToBase64String(this.x509crl);
				xmlElement.AppendChild(xmlElement8);
			}
			return xmlElement;
		}

		public override void LoadXml(XmlElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (this.IssuerSerialList != null)
			{
				this.IssuerSerialList.Clear();
			}
			if (this.SubjectKeyIdList != null)
			{
				this.SubjectKeyIdList.Clear();
			}
			if (this.SubjectNameList != null)
			{
				this.SubjectNameList.Clear();
			}
			if (this.X509CertificateList != null)
			{
				this.X509CertificateList.Clear();
			}
			this.x509crl = null;
			if (element.LocalName != "X509Data" || element.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
			{
				throw new CryptographicException("element");
			}
			XmlElement[] array = XmlSignature.GetChildElements(element, "X509IssuerSerial");
			if (array != null)
			{
				foreach (XmlElement xmlElement in array)
				{
					XmlElement childElement = XmlSignature.GetChildElement(xmlElement, "X509IssuerName", "http://www.w3.org/2000/09/xmldsig#");
					XmlElement childElement2 = XmlSignature.GetChildElement(xmlElement, "X509SerialNumber", "http://www.w3.org/2000/09/xmldsig#");
					this.AddIssuerSerial(childElement.InnerText, childElement2.InnerText);
				}
			}
			array = XmlSignature.GetChildElements(element, "X509SKI");
			if (array != null)
			{
				for (int j = 0; j < array.Length; j++)
				{
					byte[] array2 = Convert.FromBase64String(array[j].InnerXml);
					this.AddSubjectKeyId(array2);
				}
			}
			array = XmlSignature.GetChildElements(element, "X509SubjectName");
			if (array != null)
			{
				for (int k = 0; k < array.Length; k++)
				{
					this.AddSubjectName(array[k].InnerXml);
				}
			}
			array = XmlSignature.GetChildElements(element, "X509Certificate");
			if (array != null)
			{
				for (int l = 0; l < array.Length; l++)
				{
					byte[] array3 = Convert.FromBase64String(array[l].InnerXml);
					this.AddCertificate(new global::System.Security.Cryptography.X509Certificates.X509Certificate(array3));
				}
			}
			XmlElement childElement3 = XmlSignature.GetChildElement(element, "X509CRL", "http://www.w3.org/2000/09/xmldsig#");
			if (childElement3 != null)
			{
				this.x509crl = Convert.FromBase64String(childElement3.InnerXml);
			}
		}

		private byte[] x509crl;

		private ArrayList IssuerSerialList;

		private ArrayList SubjectKeyIdList;

		private ArrayList SubjectNameList;

		private ArrayList X509CertificateList;
	}
}
