using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public sealed class EncryptedData : EncryptedType
	{
		public override XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument());
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			if (this.CipherData == null)
			{
				throw new CryptographicException("Cipher data is not specified.");
			}
			XmlElement xmlElement = document.CreateElement("EncryptedData", "http://www.w3.org/2001/04/xmlenc#");
			if (this.EncryptionMethod != null)
			{
				xmlElement.AppendChild(this.EncryptionMethod.GetXml(document));
			}
			if (base.KeyInfo != null)
			{
				xmlElement.AppendChild(document.ImportNode(base.KeyInfo.GetXml(), true));
			}
			if (this.CipherData != null)
			{
				xmlElement.AppendChild(this.CipherData.GetXml(document));
			}
			if (this.EncryptionProperties.Count > 0)
			{
				XmlElement xmlElement2 = document.CreateElement("EncryptionProperties", "http://www.w3.org/2001/04/xmlenc#");
				foreach (object obj in this.EncryptionProperties)
				{
					EncryptionProperty encryptionProperty = (EncryptionProperty)obj;
					xmlElement2.AppendChild(encryptionProperty.GetXml(document));
				}
				xmlElement.AppendChild(xmlElement2);
			}
			if (this.Id != null)
			{
				xmlElement.SetAttribute("Id", this.Id);
			}
			if (this.Type != null)
			{
				xmlElement.SetAttribute("Type", this.Type);
			}
			if (this.MimeType != null)
			{
				xmlElement.SetAttribute("MimeType", this.MimeType);
			}
			if (this.Encoding != null)
			{
				xmlElement.SetAttribute("Encoding", this.Encoding);
			}
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "EncryptedData" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed EncryptedData element.");
			}
			this.EncryptionMethod = null;
			this.EncryptionMethod = null;
			this.EncryptionProperties.Clear();
			this.Id = null;
			this.Type = null;
			this.MimeType = null;
			this.Encoding = null;
			foreach (object obj in value.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlWhitespace))
				{
					string localName = xmlNode.LocalName;
					switch (localName)
					{
					case "EncryptionMethod":
						this.EncryptionMethod = new EncryptionMethod();
						this.EncryptionMethod.LoadXml((XmlElement)xmlNode);
						break;
					case "KeyInfo":
						base.KeyInfo = new KeyInfo();
						base.KeyInfo.LoadXml((XmlElement)xmlNode);
						break;
					case "CipherData":
						this.CipherData = new CipherData();
						this.CipherData.LoadXml((XmlElement)xmlNode);
						break;
					case "EncryptionProperties":
						foreach (object obj2 in ((XmlElement)xmlNode).GetElementsByTagName("EncryptionProperty", "http://www.w3.org/2001/04/xmlenc#"))
						{
							XmlElement xmlElement = (XmlElement)obj2;
							this.EncryptionProperties.Add(new EncryptionProperty(xmlElement));
						}
						break;
					}
				}
			}
			if (value.HasAttribute("Id"))
			{
				this.Id = value.Attributes["Id"].Value;
			}
			if (value.HasAttribute("Type"))
			{
				this.Type = value.Attributes["Type"].Value;
			}
			if (value.HasAttribute("MimeType"))
			{
				this.MimeType = value.Attributes["MimeType"].Value;
			}
			if (value.HasAttribute("Encoding"))
			{
				this.Encoding = value.Attributes["Encoding"].Value;
			}
		}
	}
}
