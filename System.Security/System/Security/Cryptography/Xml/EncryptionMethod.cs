using System;
using System.Collections.Generic;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class EncryptionMethod
	{
		public EncryptionMethod()
		{
			this.KeyAlgorithm = null;
		}

		public EncryptionMethod(string strAlgorithm)
		{
			this.KeyAlgorithm = strAlgorithm;
		}

		public string KeyAlgorithm
		{
			get
			{
				return this.algorithm;
			}
			set
			{
				this.algorithm = value;
			}
		}

		public int KeySize
		{
			get
			{
				return this.keySize;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("The key size should be a non negative integer.");
				}
				this.keySize = value;
			}
		}

		public XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument());
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement("EncryptionMethod", "http://www.w3.org/2001/04/xmlenc#");
			if (this.KeySize != 0)
			{
				XmlElement xmlElement2 = document.CreateElement("KeySize", "http://www.w3.org/2001/04/xmlenc#");
				xmlElement2.InnerText = string.Format("{0}", this.keySize);
				xmlElement.AppendChild(xmlElement2);
			}
			if (this.KeyAlgorithm != null)
			{
				xmlElement.SetAttribute("Algorithm", this.KeyAlgorithm);
			}
			return xmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "EncryptionMethod" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed EncryptionMethod element.");
			}
			this.KeyAlgorithm = null;
			foreach (object obj in value.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlWhitespace))
				{
					string localName = xmlNode.LocalName;
					if (localName != null)
					{
						if (EncryptionMethod.<>f__switch$mapA == null)
						{
							EncryptionMethod.<>f__switch$mapA = new Dictionary<string, int>(1) { { "KeySize", 0 } };
						}
						int num;
						if (EncryptionMethod.<>f__switch$mapA.TryGetValue(localName, out num))
						{
							if (num == 0)
							{
								this.KeySize = int.Parse(xmlNode.InnerText);
							}
						}
					}
				}
			}
			if (value.HasAttribute("Algorithm"))
			{
				this.KeyAlgorithm = value.Attributes["Algorithm"].Value;
			}
		}

		private string algorithm;

		private int keySize;
	}
}
