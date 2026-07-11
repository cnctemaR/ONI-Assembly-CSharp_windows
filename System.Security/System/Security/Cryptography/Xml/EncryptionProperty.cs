using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public sealed class EncryptionProperty
	{
		public EncryptionProperty()
		{
		}

		public EncryptionProperty(XmlElement elementProperty)
		{
			if (elementProperty == null)
			{
				throw new ArgumentNullException("elementProperty");
			}
			if (elementProperty.LocalName != "EncryptionProperty" || elementProperty.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed encryption property element.");
			}
			this._elemProp = elementProperty;
			this._cachedXml = null;
		}

		public string Id
		{
			get
			{
				return this._id;
			}
		}

		public string Target
		{
			get
			{
				return this._target;
			}
		}

		public XmlElement PropertyElement
		{
			get
			{
				return this._elemProp;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.LocalName != "EncryptionProperty" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
				{
					throw new CryptographicException("Malformed encryption property element.");
				}
				this._elemProp = value;
				this._cachedXml = null;
			}
		}

		private bool CacheValid
		{
			get
			{
				return this._cachedXml != null;
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
			return document.ImportNode(this._elemProp, true) as XmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "EncryptionProperty" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed encryption property element.");
			}
			this._cachedXml = value;
			this._id = Utils.GetAttribute(value, "Id", "http://www.w3.org/2001/04/xmlenc#");
			this._target = Utils.GetAttribute(value, "Target", "http://www.w3.org/2001/04/xmlenc#");
			this._elemProp = value;
		}

		private string _target;

		private string _id;

		private XmlElement _elemProp;

		private XmlElement _cachedXml;
	}
}
