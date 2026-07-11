using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public abstract class EncryptedReference
	{
		protected EncryptedReference()
			: this(string.Empty, new TransformChain())
		{
		}

		protected EncryptedReference(string uri)
			: this(uri, new TransformChain())
		{
		}

		protected EncryptedReference(string uri, TransformChain transformChain)
		{
			this.TransformChain = transformChain;
			this.Uri = uri;
			this._cachedXml = null;
		}

		public string Uri
		{
			get
			{
				return this._uri;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("A Uri attribute is required for a CipherReference element.");
				}
				this._uri = value;
				this._cachedXml = null;
			}
		}

		public TransformChain TransformChain
		{
			get
			{
				if (this._transformChain == null)
				{
					this._transformChain = new TransformChain();
				}
				return this._transformChain;
			}
			set
			{
				this._transformChain = value;
				this._cachedXml = null;
			}
		}

		public void AddTransform(Transform transform)
		{
			this.TransformChain.Add(transform);
		}

		protected string ReferenceType
		{
			get
			{
				return this._referenceType;
			}
			set
			{
				this._referenceType = value;
				this._cachedXml = null;
			}
		}

		protected internal bool CacheValid
		{
			get
			{
				return this._cachedXml != null;
			}
		}

		public virtual XmlElement GetXml()
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
			if (this.ReferenceType == null)
			{
				throw new CryptographicException("The Reference type must be set in an EncryptedReference object.");
			}
			XmlElement xmlElement = document.CreateElement(this.ReferenceType, "http://www.w3.org/2001/04/xmlenc#");
			if (!string.IsNullOrEmpty(this._uri))
			{
				xmlElement.SetAttribute("URI", this._uri);
			}
			if (this.TransformChain.Count > 0)
			{
				xmlElement.AppendChild(this.TransformChain.GetXml(document, "http://www.w3.org/2000/09/xmldsig#"));
			}
			return xmlElement;
		}

		public virtual void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.ReferenceType = value.LocalName;
			this.Uri = Utils.GetAttribute(value, "URI", "http://www.w3.org/2001/04/xmlenc#");
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			XmlNode xmlNode = value.SelectSingleNode("ds:Transforms", xmlNamespaceManager);
			if (xmlNode != null)
			{
				this.TransformChain.LoadXml(xmlNode as XmlElement);
			}
			this._cachedXml = value;
		}

		private string _uri;

		private string _referenceType;

		private TransformChain _transformChain;

		internal XmlElement _cachedXml;
	}
}
