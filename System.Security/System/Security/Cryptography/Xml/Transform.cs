using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public abstract class Transform
	{
		internal string BaseURI
		{
			get
			{
				return this._baseUri;
			}
			set
			{
				this._baseUri = value;
			}
		}

		internal SignedXml SignedXml
		{
			get
			{
				return this._signedXml;
			}
			set
			{
				this._signedXml = value;
			}
		}

		internal Reference Reference
		{
			get
			{
				return this._reference;
			}
			set
			{
				this._reference = value;
			}
		}

		public string Algorithm
		{
			get
			{
				return this._algorithm;
			}
			set
			{
				this._algorithm = value;
			}
		}

		public XmlResolver Resolver
		{
			internal get
			{
				return this._xmlResolver;
			}
			set
			{
				this._xmlResolver = value;
				this._bResolverSet = true;
			}
		}

		internal bool ResolverSet
		{
			get
			{
				return this._bResolverSet;
			}
		}

		public abstract Type[] InputTypes { get; }

		public abstract Type[] OutputTypes { get; }

		internal bool AcceptsType(Type inputType)
		{
			if (this.InputTypes != null)
			{
				for (int i = 0; i < this.InputTypes.Length; i++)
				{
					if (inputType == this.InputTypes[i] || inputType.IsSubclassOf(this.InputTypes[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		public XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			return this.GetXml(document, "Transform");
		}

		internal XmlElement GetXml(XmlDocument document, string name)
		{
			XmlElement xmlElement = document.CreateElement(name, "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(this.Algorithm))
			{
				xmlElement.SetAttribute("Algorithm", this.Algorithm);
			}
			XmlNodeList innerXml = this.GetInnerXml();
			if (innerXml != null)
			{
				foreach (object obj in innerXml)
				{
					XmlNode xmlNode = (XmlNode)obj;
					xmlElement.AppendChild(document.ImportNode(xmlNode, true));
				}
			}
			return xmlElement;
		}

		public abstract void LoadInnerXml(XmlNodeList nodeList);

		protected abstract XmlNodeList GetInnerXml();

		public abstract void LoadInput(object obj);

		public abstract object GetOutput();

		public abstract object GetOutput(Type type);

		public virtual byte[] GetDigestedOutput(HashAlgorithm hash)
		{
			return hash.ComputeHash((Stream)this.GetOutput(typeof(Stream)));
		}

		public XmlElement Context
		{
			get
			{
				if (this._context != null)
				{
					return this._context;
				}
				Reference reference = this.Reference;
				SignedXml signedXml = ((reference == null) ? this.SignedXml : reference.SignedXml);
				if (signedXml == null)
				{
					return null;
				}
				return signedXml._context;
			}
			set
			{
				this._context = value;
			}
		}

		public Hashtable PropagatedNamespaces
		{
			get
			{
				if (this._propagatedNamespaces != null)
				{
					return this._propagatedNamespaces;
				}
				Reference reference = this.Reference;
				SignedXml signedXml = ((reference == null) ? this.SignedXml : reference.SignedXml);
				if (reference != null && (reference.ReferenceTargetType != ReferenceTargetType.UriReference || string.IsNullOrEmpty(reference.Uri) || reference.Uri[0] != '#'))
				{
					this._propagatedNamespaces = new Hashtable(0);
					return this._propagatedNamespaces;
				}
				CanonicalXmlNodeList canonicalXmlNodeList = null;
				if (reference != null)
				{
					canonicalXmlNodeList = reference._namespaces;
				}
				else if (((signedXml != null) ? signedXml._context : null) != null)
				{
					canonicalXmlNodeList = Utils.GetPropagatedAttributes(signedXml._context);
				}
				if (canonicalXmlNodeList == null)
				{
					this._propagatedNamespaces = new Hashtable(0);
					return this._propagatedNamespaces;
				}
				this._propagatedNamespaces = new Hashtable(canonicalXmlNodeList.Count);
				foreach (object obj in canonicalXmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string text = ((xmlNode.Prefix.Length > 0) ? (xmlNode.Prefix + ":" + xmlNode.LocalName) : xmlNode.LocalName);
					if (!this._propagatedNamespaces.Contains(text))
					{
						this._propagatedNamespaces.Add(text, xmlNode.Value);
					}
				}
				return this._propagatedNamespaces;
			}
		}

		private string _algorithm;

		private string _baseUri;

		internal XmlResolver _xmlResolver;

		private bool _bResolverSet;

		private SignedXml _signedXml;

		private Reference _reference;

		private Hashtable _propagatedNamespaces;

		private XmlElement _context;
	}
}
