using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class RSAKeyValue : KeyInfoClause
	{
		public RSAKeyValue()
		{
			this._key = RSA.Create();
		}

		public RSAKeyValue(RSA key)
		{
			this._key = key;
		}

		public RSA Key
		{
			get
			{
				return this._key;
			}
			set
			{
				this._key = value;
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
			RSAParameters rsaparameters = this._key.ExportParameters(false);
			XmlElement xmlElement = xmlDocument.CreateElement("KeyValue", "http://www.w3.org/2000/09/xmldsig#");
			XmlElement xmlElement2 = xmlDocument.CreateElement("RSAKeyValue", "http://www.w3.org/2000/09/xmldsig#");
			XmlElement xmlElement3 = xmlDocument.CreateElement("Modulus", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement3.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(rsaparameters.Modulus)));
			xmlElement2.AppendChild(xmlElement3);
			XmlElement xmlElement4 = xmlDocument.CreateElement("Exponent", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement4.AppendChild(xmlDocument.CreateTextNode(Convert.ToBase64String(rsaparameters.Exponent)));
			xmlElement2.AppendChild(xmlElement4);
			xmlElement.AppendChild(xmlElement2);
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "KeyValue" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
			{
				throw new CryptographicException(string.Format("Root element must be {0} element in namespace {1}", "KeyValue", "http://www.w3.org/2000/09/xmldsig#"));
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
			XmlNode xmlNode = value.SelectSingleNode(string.Format("{0}:{1}", "dsig", "RSAKeyValue"), xmlNamespaceManager);
			if (xmlNode == null)
			{
				throw new CryptographicException(string.Format("{0} must contain child element {1}", "KeyValue", "RSAKeyValue"));
			}
			try
			{
				this.Key.ImportParameters(new RSAParameters
				{
					Modulus = Convert.FromBase64String(xmlNode.SelectSingleNode(string.Format("{0}:{1}", "dsig", "Modulus"), xmlNamespaceManager).InnerText),
					Exponent = Convert.FromBase64String(xmlNode.SelectSingleNode(string.Format("{0}:{1}", "dsig", "Exponent"), xmlNamespaceManager).InnerText)
				});
			}
			catch (Exception ex)
			{
				throw new CryptographicException(string.Format("An error occurred parsing the {0} and {1} elements", "Modulus", "Exponent"), ex);
			}
		}

		private RSA _key;

		private const string KeyValueElementName = "KeyValue";

		private const string RSAKeyValueElementName = "RSAKeyValue";

		private const string ModulusElementName = "Modulus";

		private const string ExponentElementName = "Exponent";
	}
}
