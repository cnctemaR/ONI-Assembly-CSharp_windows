using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfoName : KeyInfoClause
	{
		public KeyInfoName()
		{
		}

		public KeyInfoName(string keyName)
		{
			this.name = keyName;
		}

		public string Value
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public override XmlElement GetXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("KeyName", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement.InnerText = this.name;
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			if (value.LocalName != "KeyName" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
			{
				this.name = string.Empty;
			}
			else
			{
				this.name = value.InnerText;
			}
		}

		private string name;
	}
}
