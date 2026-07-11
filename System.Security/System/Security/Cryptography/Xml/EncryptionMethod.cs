using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class EncryptionMethod
	{
		public EncryptionMethod()
		{
			this._cachedXml = null;
		}

		public EncryptionMethod(string algorithm)
		{
			this._algorithm = algorithm;
			this._cachedXml = null;
		}

		private bool CacheValid
		{
			get
			{
				return this._cachedXml != null;
			}
		}

		public int KeySize
		{
			get
			{
				return this._keySize;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value", "The key size should be a non negative integer.");
				}
				this._keySize = value;
				this._cachedXml = null;
			}
		}

		public string KeyAlgorithm
		{
			get
			{
				return this._algorithm;
			}
			set
			{
				this._algorithm = value;
				this._cachedXml = null;
			}
		}

		public XmlElement GetXml()
		{
			if (this.CacheValid)
			{
				return this._cachedXml;
			}
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement("EncryptionMethod", "http://www.w3.org/2001/04/xmlenc#");
			if (!string.IsNullOrEmpty(this._algorithm))
			{
				xmlElement.SetAttribute("Algorithm", this._algorithm);
			}
			if (this._keySize > 0)
			{
				XmlElement xmlElement2 = document.CreateElement("KeySize", "http://www.w3.org/2001/04/xmlenc#");
				xmlElement2.AppendChild(document.CreateTextNode(this._keySize.ToString(null, null)));
				xmlElement.AppendChild(xmlElement2);
			}
			return xmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
			this._algorithm = Utils.GetAttribute(value, "Algorithm", "http://www.w3.org/2001/04/xmlenc#");
			XmlNode xmlNode = value.SelectSingleNode("enc:KeySize", xmlNamespaceManager);
			if (xmlNode != null)
			{
				this.KeySize = Convert.ToInt32(Utils.DiscardWhiteSpaces(xmlNode.InnerText), null);
			}
			this._cachedXml = value;
		}

		private XmlElement _cachedXml;

		private int _keySize;

		private string _algorithm;
	}
}
