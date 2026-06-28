using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class DSAKeyValue : KeyInfoClause
	{
		public DSAKeyValue()
		{
			this.dsa = DSA.Create();
		}

		public DSAKeyValue(DSA key)
		{
			this.dsa = key;
		}

		public DSA Key
		{
			get
			{
				return this.dsa;
			}
			set
			{
				this.dsa = value;
			}
		}

		public override XmlElement GetXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("KeyValue", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement.SetAttribute("xmlns", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement.InnerXml = this.dsa.ToXmlString(false);
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
			this.dsa.FromXmlString(value.InnerXml);
		}

		private DSA dsa;
	}
}
