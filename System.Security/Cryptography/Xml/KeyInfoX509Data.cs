using System;
using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfoX509Data : KeyInfoClause
	{
		public KeyInfoX509Data()
		{
		}

		public KeyInfoX509Data(byte[] rgbCert)
		{
			X509Certificate2 x509Certificate = new X509Certificate2(rgbCert);
			this.AddCertificate(x509Certificate);
		}

		public KeyInfoX509Data(X509Certificate cert)
		{
			this.AddCertificate(cert);
		}

		public KeyInfoX509Data(X509Certificate cert, X509IncludeOption includeOption)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			X509Certificate2 x509Certificate = new X509Certificate2(cert);
			switch (includeOption)
			{
			case X509IncludeOption.ExcludeRoot:
			{
				X509Chain x509Chain = new X509Chain();
				x509Chain.Build(x509Certificate);
				if (x509Chain.ChainStatus.Length != 0 && (x509Chain.ChainStatus[0].Status & X509ChainStatusFlags.PartialChain) == X509ChainStatusFlags.PartialChain)
				{
					throw new CryptographicException("A certificate chain could not be built to a trusted root authority.");
				}
				X509ChainElementCollection x509ChainElementCollection = x509Chain.ChainElements;
				for (int i = 0; i < (Utils.IsSelfSigned(x509Chain) ? 1 : (x509ChainElementCollection.Count - 1)); i++)
				{
					this.AddCertificate(x509ChainElementCollection[i].Certificate);
				}
				return;
			}
			case X509IncludeOption.EndCertOnly:
				this.AddCertificate(x509Certificate);
				return;
			case X509IncludeOption.WholeChain:
			{
				X509Chain x509Chain = new X509Chain();
				x509Chain.Build(x509Certificate);
				if (x509Chain.ChainStatus.Length != 0 && (x509Chain.ChainStatus[0].Status & X509ChainStatusFlags.PartialChain) == X509ChainStatusFlags.PartialChain)
				{
					throw new CryptographicException("A certificate chain could not be built to a trusted root authority.");
				}
				X509ChainElementCollection x509ChainElementCollection = x509Chain.ChainElements;
				foreach (X509ChainElement x509ChainElement in x509ChainElementCollection)
				{
					this.AddCertificate(x509ChainElement.Certificate);
				}
				return;
			}
			default:
				return;
			}
		}

		public ArrayList Certificates
		{
			get
			{
				return this._certificates;
			}
		}

		public void AddCertificate(X509Certificate certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (this._certificates == null)
			{
				this._certificates = new ArrayList();
			}
			X509Certificate2 x509Certificate = new X509Certificate2(certificate);
			this._certificates.Add(x509Certificate);
		}

		public ArrayList SubjectKeyIds
		{
			get
			{
				return this._subjectKeyIds;
			}
		}

		public void AddSubjectKeyId(byte[] subjectKeyId)
		{
			if (this._subjectKeyIds == null)
			{
				this._subjectKeyIds = new ArrayList();
			}
			this._subjectKeyIds.Add(subjectKeyId);
		}

		public void AddSubjectKeyId(string subjectKeyId)
		{
			if (this._subjectKeyIds == null)
			{
				this._subjectKeyIds = new ArrayList();
			}
			this._subjectKeyIds.Add(Utils.DecodeHexString(subjectKeyId));
		}

		public ArrayList SubjectNames
		{
			get
			{
				return this._subjectNames;
			}
		}

		public void AddSubjectName(string subjectName)
		{
			if (this._subjectNames == null)
			{
				this._subjectNames = new ArrayList();
			}
			this._subjectNames.Add(subjectName);
		}

		public ArrayList IssuerSerials
		{
			get
			{
				return this._issuerSerials;
			}
		}

		public void AddIssuerSerial(string issuerName, string serialNumber)
		{
			if (string.IsNullOrEmpty(issuerName))
			{
				throw new ArgumentException("String cannot be empty or null.", "issuerName");
			}
			if (string.IsNullOrEmpty(serialNumber))
			{
				throw new ArgumentException("String cannot be empty or null.", "serialNumber");
			}
			BigInteger bigInteger;
			if (!BigInteger.TryParse(serialNumber, NumberStyles.AllowHexSpecifier, NumberFormatInfo.CurrentInfo, out bigInteger))
			{
				throw new ArgumentException("X509 issuer serial number is invalid.", "serialNumber");
			}
			if (this._issuerSerials == null)
			{
				this._issuerSerials = new ArrayList();
			}
			this._issuerSerials.Add(Utils.CreateX509IssuerSerial(issuerName, bigInteger.ToString()));
		}

		internal void InternalAddIssuerSerial(string issuerName, string serialNumber)
		{
			if (this._issuerSerials == null)
			{
				this._issuerSerials = new ArrayList();
			}
			this._issuerSerials.Add(Utils.CreateX509IssuerSerial(issuerName, serialNumber));
		}

		public byte[] CRL
		{
			get
			{
				return this._CRL;
			}
			set
			{
				this._CRL = value;
			}
		}

		private void Clear()
		{
			this._CRL = null;
			if (this._subjectKeyIds != null)
			{
				this._subjectKeyIds.Clear();
			}
			if (this._subjectNames != null)
			{
				this._subjectNames.Clear();
			}
			if (this._issuerSerials != null)
			{
				this._issuerSerials.Clear();
			}
			if (this._certificates != null)
			{
				this._certificates.Clear();
			}
		}

		public override XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal override XmlElement GetXml(XmlDocument xmlDocument)
		{
			XmlElement xmlElement = xmlDocument.CreateElement("X509Data", "http://www.w3.org/2000/09/xmldsig#");
			if (this._issuerSerials != null)
			{
				foreach (object obj in this._issuerSerials)
				{
					X509IssuerSerial x509IssuerSerial = (X509IssuerSerial)obj;
					XmlElement xmlElement2 = xmlDocument.CreateElement("X509IssuerSerial", "http://www.w3.org/2000/09/xmldsig#");
					XmlElement xmlElement3 = xmlDocument.CreateElement("X509IssuerName", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement3.AppendChild(xmlDocument.CreateTextNode(x509IssuerSerial.IssuerName));
					xmlElement2.AppendChild(xmlElement3);
					XmlElement xmlElement4 = xmlDocument.CreateElement("X509SerialNumber", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement4.AppendChild(xmlDocument.CreateTextNode(x509IssuerSerial.SerialNumber));
					xmlElement2.AppendChild(xmlElement4);
					xmlElement.AppendChild(xmlElement2);
				}
			}
			if (this._subjectKeyIds != null)
			{
				foreach (object obj2 in this._subjectKeyIds)
				{
					byte[] array = (byte[])obj2;
					XmlElement xmlElement5 = xmlDocument.CreateElement("X509SKI", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement5.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(array)));
					xmlElement.AppendChild(xmlElement5);
				}
			}
			if (this._subjectNames != null)
			{
				foreach (object obj3 in this._subjectNames)
				{
					string text = (string)obj3;
					XmlElement xmlElement6 = xmlDocument.CreateElement("X509SubjectName", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement6.AppendChild(xmlDocument.CreateTextNode(text));
					xmlElement.AppendChild(xmlElement6);
				}
			}
			if (this._certificates != null)
			{
				foreach (object obj4 in this._certificates)
				{
					X509Certificate x509Certificate = (X509Certificate)obj4;
					XmlElement xmlElement7 = xmlDocument.CreateElement("X509Certificate", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement7.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(x509Certificate.GetRawCertData())));
					xmlElement.AppendChild(xmlElement7);
				}
			}
			if (this._CRL != null)
			{
				XmlElement xmlElement8 = xmlDocument.CreateElement("X509CRL", "http://www.w3.org/2000/09/xmldsig#");
				xmlElement8.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(this._CRL)));
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
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(element.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			XmlNodeList xmlNodeList = element.SelectNodes("ds:X509IssuerSerial", xmlNamespaceManager);
			XmlNodeList xmlNodeList2 = element.SelectNodes("ds:X509SKI", xmlNamespaceManager);
			XmlNodeList xmlNodeList3 = element.SelectNodes("ds:X509SubjectName", xmlNamespaceManager);
			XmlNodeList xmlNodeList4 = element.SelectNodes("ds:X509Certificate", xmlNamespaceManager);
			XmlNodeList xmlNodeList5 = element.SelectNodes("ds:X509CRL", xmlNamespaceManager);
			if (xmlNodeList5.Count == 0 && xmlNodeList.Count == 0 && xmlNodeList2.Count == 0 && xmlNodeList3.Count == 0 && xmlNodeList4.Count == 0)
			{
				throw new CryptographicException("Malformed element {0}.", "X509Data");
			}
			this.Clear();
			if (xmlNodeList5.Count != 0)
			{
				this._CRL = Convert.FromBase64String(Utils.DiscardWhiteSpaces(xmlNodeList5.Item(0).InnerText));
			}
			foreach (object obj in xmlNodeList)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNode xmlNode2 = xmlNode.SelectSingleNode("ds:X509IssuerName", xmlNamespaceManager);
				XmlNode xmlNode3 = xmlNode.SelectSingleNode("ds:X509SerialNumber", xmlNamespaceManager);
				if (xmlNode2 == null || xmlNode3 == null)
				{
					throw new CryptographicException("Malformed element {0}.", "IssuerSerial");
				}
				this.InternalAddIssuerSerial(xmlNode2.InnerText.Trim(), xmlNode3.InnerText.Trim());
			}
			foreach (object obj2 in xmlNodeList2)
			{
				XmlNode xmlNode4 = (XmlNode)obj2;
				this.AddSubjectKeyId(Convert.FromBase64String(Utils.DiscardWhiteSpaces(xmlNode4.InnerText)));
			}
			foreach (object obj3 in xmlNodeList3)
			{
				XmlNode xmlNode5 = (XmlNode)obj3;
				this.AddSubjectName(xmlNode5.InnerText.Trim());
			}
			foreach (object obj4 in xmlNodeList4)
			{
				XmlNode xmlNode6 = (XmlNode)obj4;
				this.AddCertificate(new X509Certificate2(Convert.FromBase64String(Utils.DiscardWhiteSpaces(xmlNode6.InnerText))));
			}
		}

		private ArrayList _certificates;

		private ArrayList _issuerSerials;

		private ArrayList _subjectKeyIds;

		private ArrayList _subjectNames;

		private byte[] _CRL;
	}
}
