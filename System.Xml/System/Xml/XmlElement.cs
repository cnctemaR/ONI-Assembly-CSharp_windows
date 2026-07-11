using System;
using System.Collections;
using System.Xml.Schema;
using System.Xml.XPath;
using Mono.Xml;

namespace System.Xml
{
	public class XmlElement : XmlLinkedNode, IHasXmlChildNode
	{
		protected internal XmlElement(string prefix, string localName, string namespaceURI, XmlDocument doc)
			: this(prefix, localName, namespaceURI, doc, false)
		{
		}

		internal XmlElement(string prefix, string localName, string namespaceURI, XmlDocument doc, bool atomizedNames)
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
				XmlConvert.VerifyName(localName);
				prefix = doc.NameTable.Add(prefix);
				localName = doc.NameTable.Add(localName);
				namespaceURI = doc.NameTable.Add(namespaceURI);
			}
			this.name = doc.NameCache.Add(prefix, localName, namespaceURI, true);
			if (doc.DocumentType != null)
			{
				DTDAttListDeclaration dtdattListDeclaration = doc.DocumentType.DTD.AttListDecls[localName];
				if (dtdattListDeclaration != null)
				{
					for (int i = 0; i < dtdattListDeclaration.Definitions.Count; i++)
					{
						DTDAttributeDefinition dtdattributeDefinition = dtdattListDeclaration[i];
						if (dtdattributeDefinition.DefaultValue != null)
						{
							this.SetAttribute(dtdattributeDefinition.Name, dtdattributeDefinition.DefaultValue);
							this.Attributes[dtdattributeDefinition.Name].SetDefault();
						}
					}
				}
			}
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

		public override XmlAttributeCollection Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new XmlAttributeCollection(this);
				}
				return this.attributes;
			}
		}

		public virtual bool HasAttributes
		{
			get
			{
				return this.attributes != null && this.attributes.Count > 0;
			}
		}

		public override string InnerText
		{
			get
			{
				return base.InnerText;
			}
			set
			{
				if (this.FirstChild != null && this.FirstChild.NextSibling == null && this.FirstChild.NodeType == XmlNodeType.Text)
				{
					this.FirstChild.Value = value;
				}
				else
				{
					while (this.FirstChild != null)
					{
						this.RemoveChild(this.FirstChild);
					}
					base.AppendChild(this.OwnerDocument.CreateTextNode(value), false);
				}
			}
		}

		public override string InnerXml
		{
			get
			{
				return base.InnerXml;
			}
			set
			{
				while (this.FirstChild != null)
				{
					this.RemoveChild(this.FirstChild);
				}
				XmlNamespaceManager xmlNamespaceManager = base.ConstructNamespaceManager();
				XmlParserContext xmlParserContext = new XmlParserContext(this.OwnerDocument.NameTable, xmlNamespaceManager, (this.OwnerDocument.DocumentType == null) ? null : this.OwnerDocument.DocumentType.DTD, this.BaseURI, this.XmlLang, this.XmlSpace, null);
				XmlTextReader xmlTextReader = new XmlTextReader(value, XmlNodeType.Element, xmlParserContext);
				xmlTextReader.XmlResolver = this.OwnerDocument.Resolver;
				for (;;)
				{
					XmlNode xmlNode = this.OwnerDocument.ReadNode(xmlTextReader);
					if (xmlNode == null)
					{
						break;
					}
					this.AppendChild(xmlNode);
				}
			}
		}

		public bool IsEmpty
		{
			get
			{
				return !this.isNotEmpty && this.FirstChild == null;
			}
			set
			{
				this.isNotEmpty = !value;
				if (value)
				{
					while (this.FirstChild != null)
					{
						this.RemoveChild(this.FirstChild);
					}
				}
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

		public override XmlNode NextSibling
		{
			get
			{
				return (this.ParentNode != null && ((IHasXmlChildNode)this.ParentNode).LastLinkedChild != this) ? base.NextLinkedSibling : null;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Element;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Element;
			}
		}

		public override XmlDocument OwnerDocument
		{
			get
			{
				return base.OwnerDocument;
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
					throw new ArgumentException("This node is readonly.");
				}
				if (value == null)
				{
					value = string.Empty;
				}
				if (!string.Empty.Equals(value) && !XmlChar.IsNCName(value))
				{
					throw new ArgumentException("Specified name is not a valid NCName: " + value);
				}
				value = this.OwnerDocument.NameTable.Add(value);
				this.name = this.OwnerDocument.NameCache.Add(value, this.name.LocalName, this.name.NS, true);
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return base.ParentNode;
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

		public override XmlNode CloneNode(bool deep)
		{
			XmlElement xmlElement = this.OwnerDocument.CreateElement(this.name.Prefix, this.name.LocalName, this.name.NS, true);
			for (int i = 0; i < this.Attributes.Count; i++)
			{
				xmlElement.SetAttributeNode((XmlAttribute)this.Attributes[i].CloneNode(true));
			}
			if (deep)
			{
				for (int j = 0; j < this.ChildNodes.Count; j++)
				{
					xmlElement.AppendChild(this.ChildNodes[j].CloneNode(true), false);
				}
			}
			return xmlElement;
		}

		public virtual string GetAttribute(string name)
		{
			XmlNode namedItem = this.Attributes.GetNamedItem(name);
			return (namedItem == null) ? string.Empty : namedItem.Value;
		}

		public virtual string GetAttribute(string localName, string namespaceURI)
		{
			XmlNode namedItem = this.Attributes.GetNamedItem(localName, namespaceURI);
			return (namedItem == null) ? string.Empty : namedItem.Value;
		}

		public virtual XmlAttribute GetAttributeNode(string name)
		{
			XmlNode namedItem = this.Attributes.GetNamedItem(name);
			return (namedItem == null) ? null : (namedItem as XmlAttribute);
		}

		public virtual XmlAttribute GetAttributeNode(string localName, string namespaceURI)
		{
			XmlNode namedItem = this.Attributes.GetNamedItem(localName, namespaceURI);
			return (namedItem == null) ? null : (namedItem as XmlAttribute);
		}

		public virtual XmlNodeList GetElementsByTagName(string name)
		{
			ArrayList arrayList = new ArrayList();
			base.SearchDescendantElements(name, name == "*", arrayList);
			return new XmlNodeArrayList(arrayList);
		}

		public virtual XmlNodeList GetElementsByTagName(string localName, string namespaceURI)
		{
			ArrayList arrayList = new ArrayList();
			base.SearchDescendantElements(localName, localName == "*", namespaceURI, namespaceURI == "*", arrayList);
			return new XmlNodeArrayList(arrayList);
		}

		public virtual bool HasAttribute(string name)
		{
			XmlNode namedItem = this.Attributes.GetNamedItem(name);
			return namedItem != null;
		}

		public virtual bool HasAttribute(string localName, string namespaceURI)
		{
			XmlNode namedItem = this.Attributes.GetNamedItem(localName, namespaceURI);
			return namedItem != null;
		}

		public override void RemoveAll()
		{
			base.RemoveAll();
		}

		public virtual void RemoveAllAttributes()
		{
			if (this.attributes != null)
			{
				this.attributes.RemoveAll();
			}
		}

		public virtual void RemoveAttribute(string name)
		{
			if (this.attributes == null)
			{
				return;
			}
			XmlAttribute xmlAttribute = this.Attributes.GetNamedItem(name) as XmlAttribute;
			if (xmlAttribute != null)
			{
				this.Attributes.Remove(xmlAttribute);
			}
		}

		public virtual void RemoveAttribute(string localName, string namespaceURI)
		{
			if (this.attributes == null)
			{
				return;
			}
			XmlAttribute xmlAttribute = this.attributes.GetNamedItem(localName, namespaceURI) as XmlAttribute;
			if (xmlAttribute != null)
			{
				this.Attributes.Remove(xmlAttribute);
			}
		}

		public virtual XmlNode RemoveAttributeAt(int i)
		{
			if (this.attributes == null || this.attributes.Count <= i)
			{
				return null;
			}
			return this.Attributes.RemoveAt(i);
		}

		public virtual XmlAttribute RemoveAttributeNode(XmlAttribute oldAttr)
		{
			if (this.attributes == null)
			{
				return null;
			}
			return this.Attributes.Remove(oldAttr);
		}

		public virtual XmlAttribute RemoveAttributeNode(string localName, string namespaceURI)
		{
			if (this.attributes == null)
			{
				return null;
			}
			return this.Attributes.Remove(this.attributes[localName, namespaceURI]);
		}

		public virtual void SetAttribute(string name, string value)
		{
			XmlAttribute xmlAttribute = this.Attributes[name];
			if (xmlAttribute == null)
			{
				xmlAttribute = this.OwnerDocument.CreateAttribute(name);
				xmlAttribute.Value = value;
				this.Attributes.SetNamedItem(xmlAttribute);
			}
			else
			{
				xmlAttribute.Value = value;
			}
		}

		public virtual string SetAttribute(string localName, string namespaceURI, string value)
		{
			XmlAttribute xmlAttribute = this.Attributes[localName, namespaceURI];
			if (xmlAttribute == null)
			{
				xmlAttribute = this.OwnerDocument.CreateAttribute(localName, namespaceURI);
				xmlAttribute.Value = value;
				this.Attributes.SetNamedItem(xmlAttribute);
			}
			else
			{
				xmlAttribute.Value = value;
			}
			return xmlAttribute.Value;
		}

		public virtual XmlAttribute SetAttributeNode(XmlAttribute newAttr)
		{
			if (newAttr.OwnerElement != null)
			{
				throw new InvalidOperationException("Specified attribute is already an attribute of another element.");
			}
			XmlAttribute xmlAttribute = this.Attributes.SetNamedItem(newAttr) as XmlAttribute;
			return (xmlAttribute != newAttr) ? xmlAttribute : null;
		}

		public virtual XmlAttribute SetAttributeNode(string localName, string namespaceURI)
		{
			XmlConvert.VerifyNCName(localName);
			return this.Attributes.Append(this.OwnerDocument.CreateAttribute(string.Empty, localName, namespaceURI, false, true));
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
			w.WriteStartElement((this.name.NS != null && this.name.NS.Length != 0) ? this.name.Prefix : string.Empty, this.name.LocalName, this.name.NS);
			if (this.HasAttributes)
			{
				for (int i = 0; i < this.Attributes.Count; i++)
				{
					this.Attributes[i].WriteTo(w);
				}
			}
			this.WriteContentTo(w);
			if (this.IsEmpty)
			{
				w.WriteEndElement();
			}
			else
			{
				w.WriteFullEndElement();
			}
		}

		private XmlAttributeCollection attributes;

		private XmlNameEntry name;

		private XmlLinkedNode lastLinkedChild;

		private bool isNotEmpty;

		private IXmlSchemaInfo schemaInfo;
	}
}
