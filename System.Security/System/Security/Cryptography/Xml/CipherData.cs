using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public sealed class CipherData
	{
		public CipherData()
		{
		}

		public CipherData(byte[] cipherValue)
		{
			this.CipherValue = cipherValue;
		}

		public CipherData(CipherReference cipherReference)
		{
			this.CipherReference = cipherReference;
		}

		public CipherReference CipherReference
		{
			get
			{
				return this.cipherReference;
			}
			set
			{
				if (this.CipherValue != null)
				{
					throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
				}
				this.cipherReference = value;
			}
		}

		public byte[] CipherValue
		{
			get
			{
				return this.cipherValue;
			}
			set
			{
				if (this.CipherReference != null)
				{
					throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
				}
				this.cipherValue = value;
			}
		}

		public XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument());
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			if (this.CipherReference == null && this.CipherValue == null)
			{
				throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
			}
			XmlElement xmlElement = document.CreateElement("CipherData", "http://www.w3.org/2001/04/xmlenc#");
			if (this.CipherReference != null)
			{
				xmlElement.AppendChild(document.ImportNode(this.cipherReference.GetXml(), true));
			}
			if (this.CipherValue != null)
			{
				XmlElement xmlElement2 = document.CreateElement("CipherValue", "http://www.w3.org/2001/04/xmlenc#");
				StreamReader streamReader = new StreamReader(new CryptoStream(new MemoryStream(this.cipherValue), new ToBase64Transform(), CryptoStreamMode.Read));
				xmlElement2.InnerText = streamReader.ReadToEnd();
				streamReader.Close();
				xmlElement.AppendChild(xmlElement2);
			}
			return xmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			this.CipherReference = null;
			this.CipherValue = null;
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "CipherData" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed Cipher Data element.");
			}
			foreach (object obj in value.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlWhitespace))
				{
					string localName = xmlNode.LocalName;
					if (localName != null)
					{
						if (CipherData.<>f__switch$map1 == null)
						{
							CipherData.<>f__switch$map1 = new Dictionary<string, int>(2)
							{
								{ "CipherReference", 0 },
								{ "CipherValue", 1 }
							};
						}
						int num;
						if (CipherData.<>f__switch$map1.TryGetValue(localName, out num))
						{
							if (num != 0)
							{
								if (num == 1)
								{
									this.CipherValue = Convert.FromBase64String(xmlNode.InnerText);
								}
							}
							else
							{
								this.cipherReference = new CipherReference();
								this.cipherReference.LoadXml((XmlElement)xmlNode);
							}
						}
					}
				}
			}
			if (this.CipherReference == null && this.CipherValue == null)
			{
				throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
			}
		}

		private byte[] cipherValue;

		private CipherReference cipherReference;
	}
}
