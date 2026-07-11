using System;
using System.Xml.Schema;
using System.Xml.XPath;
using Mono.Xml;

namespace System.Xml
{
	public class XmlAttribute : XmlNode, IHasXmlChildNode
	{
		protected internal XmlAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc)
			: this(prefix, localName, namespaceURI, doc, false, true)
		{
		}

		internal XmlAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc, bool atomizedNames, bool checkNamespace)
			: base(doc)
		{
			if (!atomizedNames)
			{
				if (prefix == null)
				{
					prefix = string.Empty;
				}
				if (namespaceURI == null)
				{
					namespaceURI = string.Empty;
				}
			}
			if (checkNamespace && (prefix == "xmlns" || (prefix == string.Empty && localName == "xmlns")))
			{
				if (namespaceURI != "http://www.w3.org/2000/xmlns/")
				{
					throw new ArgumentException("Invalid attribute namespace for namespace declaration.");
				}
				if (prefix == "xml" && namespaceURI != "http://www.w3.org/XML/1998/namespace")
				{
					throw new ArgumentException("Invalid attribute namespace for namespace declaration.");
				}
			}
			if (!atomizedNames)
			{
				if (prefix != string.Empty && !XmlChar.IsName(prefix))
				{
					throw new ArgumentException("Invalid attribute prefix.");
				}
				if (!XmlChar.IsName(localName))
				{
					throw new ArgumentException("Invalid attribute local name.");
				}
				prefix = doc.NameTable.Add(prefix);
				localName = doc.NameTable.Add(localName);
				namespaceURI = doc.NameTable.Add(namespaceURI);
			}
			this.name = doc.NameCache.Add(prefix, localName, namespaceURI, true);
		}

		XmlLinkedNode IHasXmlChildNode.LastLinkedChild
		{
			get
			{
				return this.lastLinkedChild;
			}
			set
			{
				this.lastLinkedChild = value;
			}
		}

		public override string BaseURI
		{
			get
			{
				return (this.OwnerElement == null) ? string.Empty : this.OwnerElement.BaseURI;
			}
		}

		public override string InnerText
		{
			set
			{
				this.Value = value;
			}
		}

		public override string InnerXml
		{
			set
			{
				this.RemoveAll();
				XmlNamespaceManager xmlNamespaceManager = base.ConstructNamespaceManager();
				XmlParserContext xmlParserContext = new XmlParserContext(this.OwnerDocument.NameTable, xmlNamespaceManager, (this.OwnerDocument.DocumentType == null) ? null : this.OwnerDocument.DocumentType.DTD, this.BaseURI, this.XmlLang, this.XmlSpace, null);
				XmlTextReader xmlTextReader = new XmlTextReader(value, XmlNodeType.Attribute, xmlParserContext);
				xmlTextReader.XmlResolver = this.OwnerDocument.Resolver;
				xmlTextReader.Read();
				this.OwnerDocument.ReadAttributeNodeValue(xmlTextReader, this);
			}
		}

		public override string LocalName
		{
			get
			{
				return this.name.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return this.name.GetPrefixedName(this.OwnerDocument.NameCache);
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.name.NS;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Attribute;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Attribute;
			}
		}

		public override XmlDocument OwnerDocument
		{
			get
			{
				return base.OwnerDocument;
			}
		}

		public virtual XmlElement OwnerElement
		{
			get
			{
				return base.AttributeOwnerElement;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.name.Prefix;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new XmlException("This node is readonly.");
				}
				if (this.name.Prefix == "xmlns" && value != "xmlns")
				{
					throw new ArgumentException("Cannot bind to the reserved namespace.");
				}
				value = this.OwnerDocument.NameTable.Add(value);
				this.name = this.OwnerDocument.NameCache.Add(value, this.name.LocalName, this.name.NS, true);
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return this.schemaInfo;
			}
			internal set
			{
				this.schemaInfo = value;
			}
		}

		public virtual bool Specified
		{
			get
			{
				return !this.isDefault;
			}
		}

		public override string Value
		{
			get
			{
				return this.InnerText;
			}
			set
			{
				if (this.IsReadOnly)
				{
					throw new ArgumentException("Attempt to modify a read-only node.");
				}
				this.OwnerDocument.CheckIdTableUpdate(this, this.InnerText, value);
				XmlNode xmlNode = this.FirstChild as XmlCharacterData;
				if (xmlNode == null)
				{
					this.RemoveAll();
					base.AppendChild(this.OwnerDocument.CreateTextNode(value), false);
				}
				else if (this.FirstChild.NextSibling != null)
				{
					this.RemoveAll();
					base.AppendChild(this.OwnerDocument.CreateTextNode(value), false);
				}
				else
				{
					xmlNode.Value = value;
				}
				this.isDefault = false;
			}
		}

		internal override string XmlLang
		{
			get
			{
				return (this.OwnerElement == null) ? string.Empty : this.OwnerElement.XmlLang;
			}
		}

		internal override XmlSpace XmlSpace
		{
			get
			{
				return (this.OwnerElement == null) ? XmlSpace.None : this.OwnerElement.XmlSpace;
			}
		}

		public override XmlNode AppendChild(XmlNode child)
		{
			return base.AppendChild(child);
		}

		public override XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
		{
			return base.InsertBefore(newChild, refChild);
		}

		public override XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
		{
			return base.InsertAfter(newChild, refChild);
		}

		public override XmlNode PrependChild(XmlNode node)
		{
			return base.PrependChild(node);
		}

		public override XmlNode RemoveChild(XmlNode node)
		{
			return base.RemoveChild(node);
		}

		public override XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
		{
			return base.ReplaceChild(newChild, oldChild);
		}

		public override XmlNode CloneNode(bool deep)
		{
			XmlNode xmlNode = this.OwnerDocument.CreateAttribute(this.name.Prefix, this.name.LocalName, this.name.NS, true, false);
			if (deep)
			{
				for (XmlNode xmlNode2 = this.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
				{
					xmlNode.AppendChild(xmlNode2.CloneNode(deep), false);
				}
			}
			return xmlNode;
		}

		internal void SetDefault()
		{
			this.isDefault = true;
		}

		public override void WriteContentTo(XmlWriter w)
		{
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				xmlNode.WriteTo(w);
			}
		}

		public override void WriteTo(XmlWriter w)
		{
			if (this.isDefault)
			{
				return;
			}
			w.WriteStartAttribute((this.name.NS.Length <= 0) ? string.Empty : this.name.Prefix, this.name.LocalName, this.name.NS);
			this.WriteContentTo(w);
			w.WriteEndAttribute();
		}

		internal DTDAttributeDefinition GetAttributeDefinition()
		{
			if (this.OwnerElement == null)
			{
				return null;
			}
			DTDAttListDeclaration dtdattListDeclaration = ((this.OwnerDocument.DocumentType == null) ? null : this.OwnerDocument.DocumentType.DTD.AttListDecls[this.OwnerElement.Name]);
			return (dtdattListDeclaration == null) ? null : dtdattListDeclaration[this.Name];
		}

		private XmlNameEntry name;

		internal bool isDefault;

		private XmlLinkedNode lastLinkedChild;

		private IXmlSchemaInfo schemaInfo;
	}
}
