using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class RSAKeyValue : KeyInfoClause
	{
		public RSAKeyValue()
		{
			this.rsa = RSA.Create();
		}

		public RSAKeyValue(RSA key)
		{
			this.rsa = key;
		}

		public RSA Key
		{
			get
			{
				return this.rsa;
			}
			set
			{
				this.rsa = value;
			}
		}

		public override XmlElement GetXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("KeyValue", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement.SetAttribute("xmlns", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement.InnerXml = this.rsa.ToXmlString(false);
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			if (value.LocalName != "KeyValue" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
			{
				throw new CryptographicException("value");
			}
			this.rsa.FromXmlString(value.InnerXml);
		}

		private RSA rsa;
	}
}
