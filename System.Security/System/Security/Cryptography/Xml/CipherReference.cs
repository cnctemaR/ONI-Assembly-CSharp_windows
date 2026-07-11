using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public sealed class CipherReference : EncryptedReference
	{
		public CipherReference()
		{
			base.ReferenceType = "CipherReference";
		}

		public CipherReference(string uri)
			: base(uri)
		{
			base.ReferenceType = "CipherReference";
		}

		public CipherReference(string uri, TransformChain transformChain)
			: base(uri, transformChain)
		{
			base.ReferenceType = "CipherReference";
		}

		internal byte[] CipherValue
		{
			get
			{
				if (!base.CacheValid)
				{
					return null;
				}
				return this._cipherValue;
			}
			set
			{
				this._cipherValue = value;
			}
		}

		public override XmlElement GetXml()
		{
			if (base.CacheValid)
			{
				return this._cachedXml;
			}
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal new XmlElement GetXml(XmlDocument document)
		{
			if (base.ReferenceType == null)
			{
				throw new CryptographicException("The Reference type must be set in an EncryptedReference object.");
			}
			XmlElement xmlElement = document.CreateElement(base.ReferenceType, "http://www.w3.org/2001/04/xmlenc#");
			if (!string.IsNullOrEmpty(base.Uri))
			{
				xmlElement.SetAttribute("URI", base.Uri);
			}
			if (base.TransformChain.Count > 0)
			{
				xmlElement.AppendChild(base.TransformChain.GetXml(document, "http://www.w3.org/2001/04/xmlenc#"));
			}
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			base.ReferenceType = value.LocalName;
			base.Uri = Utils.GetAttribute(value, "URI", "http://www.w3.org/2001/04/xmlenc#");
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
			XmlNode xmlNode = value.SelectSingleNode("enc:Transforms", xmlNamespaceManager);
			if (xmlNode != null)
			{
				base.TransformChain.LoadXml(xmlNode as XmlElement);
			}
			this._cachedXml = value;
		}

		private byte[] _cipherValue;
	}
}
