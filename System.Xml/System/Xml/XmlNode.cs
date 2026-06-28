using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	public abstract class XmlNode : IEnumerable, ICloneable, IXPathNavigable
	{
		internal XmlNode(XmlDocument ownerDocument)
		{
			this.ownerDocument = ownerDocument;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public virtual XmlAttributeCollection Attributes
		{
			get
			{
				return null;
			}
		}

		public virtual string BaseURI
		{
			get
			{
				return (this.ParentNode == null) ? string.Empty : this.ParentNode.ChildrenBaseURI;
			}
		}

		internal virtual string ChildrenBaseURI
		{
			get
			{
				return this.BaseURI;
			}
		}

		public virtual XmlNodeList ChildNodes
		{
			get
			{
				IHasXmlChildNode hasXmlChildNode = this as IHasXmlChildNode;
				if (hasXmlChildNode == null)
				{
					return XmlNode.emptyList;
				}
				if (this.childNodes == null)
				{
					this.childNodes = new XmlNodeListChildren(hasXmlChildNode);
				}
				return this.childNodes;
			}
		}

		public virtual XmlNode FirstChild
		{
			get
			{
				IHasXmlChildNode hasXmlChildNode = this as IHasXmlChildNode;
				XmlLinkedNode xmlLinkedNode = ((hasXmlChildNode != null) ? hasXmlChildNode.LastLinkedChild : null);
				return (xmlLinkedNode != null) ? xmlLinkedNode.NextLinkedSibling : null;
			}
		}

		public virtual bool HasChildNodes
		{
			get
			{
				return this.LastChild != null;
			}
		}

		public virtual string InnerText
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
				{
					return this.Value;
				}
				if (this.FirstChild == null)
				{
					return string.Empty;
				}
				if (this.FirstChild == this.LastChild)
				{
					return (this.FirstChild.NodeType == XmlNodeType.Comment) ? string.Empty : this.FirstChild.InnerText;
				}
				StringBuilder stringBuilder = null;
				this.AppendChildValues(ref stringBuilder);
				return (stringBuilder != null) ? stringBuilder.ToString() : string.Empty;
			}
			set
			{
				if (!(this is XmlDocumentFragment))
				{
					throw new InvalidOperationException("This node is read only. Cannot be modified.");
				}
				this.RemoveAll();
				this.AppendChild(this.OwnerDocument.CreateTextNode(value));
			}
		}

		private void AppendChildValues(ref StringBuilder builder)
		{
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
				{
					if (builder == null)
					{
						builder = new StringBuilder();
					}
					builder.Append(xmlNode.Value);
				}
				xmlNode.AppendChildValues(ref builder);
			}
		}

		public virtual string InnerXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				this.WriteContentTo(xmlTextWriter);
				return stringWriter.GetStringBuilder().ToString();
			}
			set
			{
				throw new InvalidOperationException("This node is readonly or doesn't have any children.");
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				XmlNode xmlNode = this;
				for (;;)
				{
					switch (xmlNode.NodeType)
					{
					case XmlNodeType.Attribute:
						xmlNode = ((XmlAttribute)xmlNode).OwnerElement;
						break;
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						goto IL_003D;
					case XmlNodeType.EntityReference:
					case XmlNodeType.Entity:
						return true;
					default:
						goto IL_003D;
					}
					IL_0049:
					if (xmlNode == null)
					{
						return false;
					}
					continue;
					IL_003D:
					xmlNode = xmlNode.ParentNode;
					goto IL_0049;
				}
				return true;
			}
		}

		public virtual XmlElement this[string name]
		{
			get
			{
				for (int i = 0; i < this.ChildNodes.Count; i++)
				{
					XmlNode xmlNode = this.ChildNodes[i];
					if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == name)
					{
						return (XmlElement)xmlNode;
					}
				}
				return null;
			}
		}

		public virtual XmlElement this[string localname, string ns]
		{
			get
			{
				for (int i = 0; i < this.ChildNodes.Count; i++)
				{
					XmlNode xmlNode = this.ChildNodes[i];
					if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.LocalName == localname && xmlNode.NamespaceURI == ns)
					{
						return (XmlElement)xmlNode;
					}
				}
				return null;
			}
		}

		public virtual XmlNode LastChild
		{
			get
			{
				IHasXmlChildNode hasXmlChildNode = this as IHasXmlChildNode;
				return (hasXmlChildNode != null) ? hasXmlChildNode.LastLinkedChild : null;
			}
		}

		public abstract string LocalName { get; }

		public abstract string Name { get; }

		public virtual string NamespaceURI
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual XmlNode NextSibling
		{
			get
			{
				return null;
			}
		}

		public abstract XmlNodeType NodeType { get; }

		internal virtual XPathNodeType XPathNodeType
		{
			get
			{
				throw new InvalidOperationException("Can not get XPath node type from " + base.GetType().ToString());
			}
		}

		public virtual string OuterXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				this.WriteTo(xmlTextWriter);
				return stringWriter.ToString();
			}
		}

		public virtual XmlDocument OwnerDocument
		{
			get
			{
				return this.ownerDocument;
			}
		}

		public virtual XmlNode ParentNode
		{
			get
			{
				return this.parentNode;
			}
		}

		public virtual string Prefix
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		public virtual XmlNode PreviousSibling
		{
			get
			{
				return null;
			}
		}

		public virtual string Value
		{
			get
			{
				return null;
			}
			set
			{
				throw new InvalidOperationException("This node does not have a value");
			}
		}

		internal virtual string XmlLang
		{
			get
			{
				if (this.Attributes != null)
				{
					for (int i = 0; i < this.Attributes.Count; i++)
					{
						XmlAttribute xmlAttribute = this.Attributes[i];
						if (xmlAttribute.Name == "xml:lang")
						{
							return xmlAttribute.Value;
						}
					}
				}
				return (this.ParentNode == null) ? this.OwnerDocument.XmlLang : this.ParentNode.XmlLang;
			}
		}

		internal virtual XmlSpace XmlSpace
		{
			get
			{
				if (this.Attributes != null)
				{
					for (int i = 0; i < this.Attributes.Count; i++)
					{
						XmlAttribute xmlAttribute = this.Attributes[i];
						if (xmlAttribute.Name == "xml:space")
						{
							string value = xmlAttribute.Value;
							if (value != null)
							{
								if (XmlNode.<>f__switch$map2B == null)
								{
									XmlNode.<>f__switch$map2B = new Dictionary<string, int>(2)
									{
										{ "preserve", 0 },
										{ "default", 1 }
									};
								}
								int num;
								if (XmlNode.<>f__switch$map2B.TryGetValue(value, out num))
								{
									if (num == 0)
									{
										return XmlSpace.Preserve;
									}
									if (num == 1)
									{
										return XmlSpace.Default;
									}
								}
							}
							break;
						}
					}
				}
				return (this.ParentNode == null) ? this.OwnerDocument.XmlSpace : this.ParentNode.XmlSpace;
			}
		}

		public virtual IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return null;
			}
			internal set
			{
			}
		}

		public virtual XmlNode AppendChild(XmlNode newChild)
		{
			return this.InsertBefore(newChild, null);
		}

		internal XmlNode AppendChild(XmlNode newChild, bool checkNodeType)
		{
			return this.InsertBefore(newChild, null, checkNodeType, true);
		}

		public virtual XmlNode Clone()
		{
			return this.CloneNode(true);
		}

		public abstract XmlNode CloneNode(bool deep);

		public virtual XPathNavigator CreateNavigator()
		{
			return this.OwnerDocument.CreateNavigator(this);
		}

		public IEnumerator GetEnumerator()
		{
			return this.ChildNodes.GetEnumerator();
		}

		public virtual string GetNamespaceOfPrefix(string prefix)
		{
			if (prefix != null)
			{
				if (XmlNode.<>f__switch$map2C == null)
				{
					XmlNode.<>f__switch$map2C = new Dictionary<string, int>(2)
					{
						{ "xml", 0 },
						{ "xmlns", 1 }
					};
				}
				int num;
				if (XmlNode.<>f__switch$map2C.TryGetValue(prefix, out num))
				{
					if (num == 0)
					{
						return "http://www.w3.org/XML/1998/namespace";
					}
					if (num == 1)
					{
						return "http://www.w3.org/2000/xmlns/";
					}
				}
				XmlNodeType nodeType = this.NodeType;
				XmlNode xmlNode;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						xmlNode = this.ParentNode;
					}
					else
					{
						xmlNode = ((XmlAttribute)this).OwnerElement;
						if (xmlNode == null)
						{
							return string.Empty;
						}
					}
				}
				else
				{
					xmlNode = this;
				}
				while (xmlNode != null)
				{
					if (xmlNode.Prefix == prefix)
					{
						return xmlNode.NamespaceURI;
					}
					if (xmlNode.NodeType == XmlNodeType.Element && ((XmlElement)xmlNode).HasAttributes)
					{
						int count = xmlNode.Attributes.Count;
						for (int i = 0; i < count; i++)
						{
							XmlAttribute xmlAttribute = xmlNode.Attributes[i];
							if ((prefix == xmlAttribute.LocalName && xmlAttribute.Prefix == "xmlns") || (xmlAttribute.Name == "xmlns" && prefix == string.Empty))
							{
								return xmlAttribute.Value;
							}
						}
					}
					xmlNode = xmlNode.ParentNode;
				}
				return string.Empty;
			}
			throw new ArgumentNullException("prefix");
		}

		public virtual string GetPrefixOfNamespace(string namespaceURI)
		{
			if (namespaceURI != null)
			{
				if (XmlNode.<>f__switch$map2D == null)
				{
					XmlNode.<>f__switch$map2D = new Dictionary<string, int>(2)
					{
						{ "http://www.w3.org/XML/1998/namespace", 0 },
						{ "http://www.w3.org/2000/xmlns/", 1 }
					};
				}
				int num;
				if (XmlNode.<>f__switch$map2D.TryGetValue(namespaceURI, out num))
				{
					if (num == 0)
					{
						return "xml";
					}
					if (num == 1)
					{
						return "xmlns";
					}
				}
			}
			XmlNodeType nodeType = this.NodeType;
			XmlNode xmlNode;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					xmlNode = this.ParentNode;
				}
				else
				{
					xmlNode = ((XmlAttribute)this).OwnerElement;
				}
			}
			else
			{
				xmlNode = this;
			}
			while (xmlNode != null)
			{
				if (xmlNode.NodeType == XmlNodeType.Element && ((XmlElement)xmlNode).HasAttributes)
				{
					for (int i = 0; i < xmlNode.Attributes.Count; i++)
					{
						XmlAttribute xmlAttribute = xmlNode.Attributes[i];
						if (xmlAttribute.Prefix == "xmlns" && xmlAttribute.Value == namespaceURI)
						{
							return xmlAttribute.LocalName;
						}
						if (xmlAttribute.Name == "xmlns" && xmlAttribute.Value == namespaceURI)
						{
							return string.Empty;
						}
					}
				}
				xmlNode = xmlNode.ParentNode;
			}
			return string.Empty;
		}

		public virtual XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
		{
			XmlNode xmlNode = null;
			if (refChild != null)
			{
				xmlNode = refChild.NextSibling;
			}
			else if (this.FirstChild != null)
			{
				xmlNode = this.FirstChild;
			}
			return this.InsertBefore(newChild, xmlNode);
		}

		public virtual XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
		{
			return this.InsertBefore(newChild, refChild, true, true);
		}

		internal bool IsAncestor(XmlNode newChild)
		{
			for (XmlNode xmlNode = this.ParentNode; xmlNode != null; xmlNode = xmlNode.ParentNode)
			{
				if (xmlNode == newChild)
				{
					return true;
				}
			}
			return false;
		}

		internal XmlNode InsertBefore(XmlNode newChild, XmlNode refChild, bool checkNodeType, bool raiseEvent)
		{
			if (checkNodeType)
			{
				this.CheckNodeInsertion(newChild, refChild);
			}
			if (newChild == refChild)
			{
				return newChild;
			}
			IHasXmlChildNode hasXmlChildNode = (IHasXmlChildNode)this;
			XmlDocument xmlDocument = ((this.NodeType != XmlNodeType.Document) ? this.OwnerDocument : ((XmlDocument)this));
			if (raiseEvent)
			{
				xmlDocument.onNodeInserting(newChild, this);
			}
			if (newChild.ParentNode != null)
			{
				newChild.ParentNode.RemoveChild(newChild, checkNodeType);
			}
			if (newChild.NodeType == XmlNodeType.DocumentFragment)
			{
				while (newChild.FirstChild != null)
				{
					this.InsertBefore(newChild.FirstChild, refChild);
				}
			}
			else
			{
				XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)newChild;
				xmlLinkedNode.parentNode = this;
				if (refChild == null)
				{
					if (hasXmlChildNode.LastLinkedChild != null)
					{
						XmlLinkedNode xmlLinkedNode2 = (XmlLinkedNode)this.FirstChild;
						hasXmlChildNode.LastLinkedChild.NextLinkedSibling = xmlLinkedNode;
						hasXmlChildNode.LastLinkedChild = xmlLinkedNode;
						xmlLinkedNode.NextLinkedSibling = xmlLinkedNode2;
					}
					else
					{
						hasXmlChildNode.LastLinkedChild = xmlLinkedNode;
						hasXmlChildNode.LastLinkedChild.NextLinkedSibling = xmlLinkedNode;
					}
				}
				else
				{
					XmlLinkedNode xmlLinkedNode3 = refChild.PreviousSibling as XmlLinkedNode;
					if (xmlLinkedNode3 == null)
					{
						hasXmlChildNode.LastLinkedChild.NextLinkedSibling = xmlLinkedNode;
					}
					else
					{
						xmlLinkedNode3.NextLinkedSibling = xmlLinkedNode;
					}
					xmlLinkedNode.NextLinkedSibling = refChild as XmlLinkedNode;
				}
				switch (newChild.NodeType)
				{
				case XmlNodeType.EntityReference:
					((XmlEntityReference)newChild).SetReferencedEntityContent();
					break;
				}
				if (raiseEvent)
				{
					xmlDocument.onNodeInserted(newChild, newChild.ParentNode);
				}
			}
			return newChild;
		}

		private void CheckNodeInsertion(XmlNode newChild, XmlNode refChild)
		{
			XmlDocument xmlDocument = ((this.NodeType != XmlNodeType.Document) ? this.OwnerDocument : ((XmlDocument)this));
			if (this.NodeType != XmlNodeType.Element && this.NodeType != XmlNodeType.Attribute && this.NodeType != XmlNodeType.Document && this.NodeType != XmlNodeType.DocumentFragment)
			{
				throw new InvalidOperationException(string.Format("Node cannot be appended to current node {0}.", this.NodeType));
			}
			XmlNodeType nodeType = this.NodeType;
			if (nodeType == XmlNodeType.Element)
			{
				XmlNodeType nodeType2 = newChild.NodeType;
				switch (nodeType2)
				{
				case XmlNodeType.Entity:
				case XmlNodeType.Document:
				case XmlNodeType.DocumentType:
				case XmlNodeType.Notation:
					break;
				default:
					if (nodeType2 != XmlNodeType.Attribute && nodeType2 != XmlNodeType.XmlDeclaration)
					{
						goto IL_0125;
					}
					break;
				}
				throw new InvalidOperationException("Cannot insert specified type of node as a child of this node.");
			}
			if (nodeType == XmlNodeType.Attribute)
			{
				switch (newChild.NodeType)
				{
				case XmlNodeType.Text:
				case XmlNodeType.EntityReference:
					goto IL_0125;
				}
				throw new InvalidOperationException(string.Format("Cannot insert specified type of node {0} as a child of this node {1}.", newChild.NodeType, this.NodeType));
			}
			IL_0125:
			if (this.IsReadOnly)
			{
				throw new InvalidOperationException("The node is readonly.");
			}
			if (newChild.OwnerDocument != xmlDocument)
			{
				throw new ArgumentException("Can't append a node created by another document.");
			}
			if (refChild != null && refChild.ParentNode != this)
			{
				throw new ArgumentException("The reference node is not a child of this node.");
			}
			if (this == xmlDocument && xmlDocument.DocumentElement != null && newChild is XmlElement && newChild != xmlDocument.DocumentElement)
			{
				throw new XmlException("multiple document element not allowed.");
			}
			if (newChild == this || this.IsAncestor(newChild))
			{
				throw new ArgumentException("Cannot insert a node or any ancestor of that node as a child of itself.");
			}
		}

		public virtual void Normalize()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = this.ChildNodes.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				XmlNode xmlNode = this.ChildNodes[i];
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.SignificantWhitespace && nodeType != XmlNodeType.Text)
				{
					xmlNode.Normalize();
					this.NormalizeRange(num, i, stringBuilder);
					num = i + 1;
				}
				else
				{
					stringBuilder.Append(xmlNode.Value);
				}
			}
			if (num < count)
			{
				this.NormalizeRange(num, count, stringBuilder);
			}
		}

		private void NormalizeRange(int start, int i, StringBuilder tmpBuilder)
		{
			int num = -1;
			for (int j = start; j < i; j++)
			{
				XmlNode xmlNode = this.ChildNodes[j];
				if (xmlNode.NodeType == XmlNodeType.Text)
				{
					num = j;
					break;
				}
				if (xmlNode.NodeType == XmlNodeType.SignificantWhitespace)
				{
					num = j;
				}
			}
			if (num >= 0)
			{
				for (int k = start; k < num; k++)
				{
					this.RemoveChild(this.ChildNodes[start]);
				}
				int num2 = i - num - 1;
				for (int l = 0; l < num2; l++)
				{
					this.RemoveChild(this.ChildNodes[start + 1]);
				}
			}
			if (num >= 0)
			{
				this.ChildNodes[start].Value = tmpBuilder.ToString();
			}
			tmpBuilder.Length = 0;
		}

		public virtual XmlNode PrependChild(XmlNode newChild)
		{
			return this.InsertAfter(newChild, null);
		}

		public virtual void RemoveAll()
		{
			if (this.Attributes != null)
			{
				this.Attributes.RemoveAll();
			}
			XmlNode nextSibling;
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = nextSibling)
			{
				nextSibling = xmlNode.NextSibling;
				this.RemoveChild(xmlNode);
			}
		}

		public virtual XmlNode RemoveChild(XmlNode oldChild)
		{
			return this.RemoveChild(oldChild, true);
		}

		private void CheckNodeRemoval()
		{
			if (this.NodeType != XmlNodeType.Attribute && this.NodeType != XmlNodeType.Element && this.NodeType != XmlNodeType.Document && this.NodeType != XmlNodeType.DocumentFragment)
			{
				throw new ArgumentException(string.Format("This {0} node cannot remove its child.", this.NodeType));
			}
			if (this.IsReadOnly)
			{
				throw new ArgumentException(string.Format("This {0} node is read only.", this.NodeType));
			}
		}

		internal XmlNode RemoveChild(XmlNode oldChild, bool checkNodeType)
		{
			if (oldChild == null)
			{
				throw new NullReferenceException();
			}
			XmlDocument xmlDocument = ((this.NodeType != XmlNodeType.Document) ? this.OwnerDocument : ((XmlDocument)this));
			if (oldChild.ParentNode != this)
			{
				throw new ArgumentException("The node to be removed is not a child of this node.");
			}
			if (checkNodeType)
			{
				xmlDocument.onNodeRemoving(oldChild, oldChild.ParentNode);
			}
			if (checkNodeType)
			{
				this.CheckNodeRemoval();
			}
			IHasXmlChildNode hasXmlChildNode = (IHasXmlChildNode)this;
			if (object.ReferenceEquals(hasXmlChildNode.LastLinkedChild, hasXmlChildNode.LastLinkedChild.NextLinkedSibling) && object.ReferenceEquals(hasXmlChildNode.LastLinkedChild, oldChild))
			{
				hasXmlChildNode.LastLinkedChild = null;
			}
			else
			{
				XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)oldChild;
				XmlLinkedNode xmlLinkedNode2 = hasXmlChildNode.LastLinkedChild;
				XmlLinkedNode xmlLinkedNode3 = (XmlLinkedNode)this.FirstChild;
				while (!object.ReferenceEquals(xmlLinkedNode2.NextLinkedSibling, hasXmlChildNode.LastLinkedChild) && !object.ReferenceEquals(xmlLinkedNode2.NextLinkedSibling, xmlLinkedNode))
				{
					xmlLinkedNode2 = xmlLinkedNode2.NextLinkedSibling;
				}
				if (!object.ReferenceEquals(xmlLinkedNode2.NextLinkedSibling, xmlLinkedNode))
				{
					throw new ArgumentException();
				}
				xmlLinkedNode2.NextLinkedSibling = xmlLinkedNode.NextLinkedSibling;
				if (xmlLinkedNode.NextLinkedSibling == xmlLinkedNode3)
				{
					hasXmlChildNode.LastLinkedChild = xmlLinkedNode2;
				}
				xmlLinkedNode.NextLinkedSibling = null;
			}
			if (checkNodeType)
			{
				xmlDocument.onNodeRemoved(oldChild, oldChild.ParentNode);
			}
			oldChild.parentNode = null;
			return oldChild;
		}

		public virtual XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
		{
			if (oldChild.ParentNode != this)
			{
				throw new ArgumentException("The node to be removed is not a child of this node.");
			}
			if (newChild == this || this.IsAncestor(newChild))
			{
				throw new InvalidOperationException("Cannot insert a node or any ancestor of that node as a child of itself.");
			}
			XmlNode nextSibling = oldChild.NextSibling;
			this.RemoveChild(oldChild);
			this.InsertBefore(newChild, nextSibling);
			return oldChild;
		}

		internal XmlElement AttributeOwnerElement
		{
			get
			{
				return (XmlElement)this.parentNode;
			}
			set
			{
				this.parentNode = value;
			}
		}

		internal void SearchDescendantElements(string name, bool matchAll, ArrayList list)
		{
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if (matchAll || xmlNode.Name == name)
					{
						list.Add(xmlNode);
					}
					xmlNode.SearchDescendantElements(name, matchAll, list);
				}
			}
		}

		internal void SearchDescendantElements(string name, bool matchAllName, string ns, bool matchAllNS, ArrayList list)
		{
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if ((matchAllName || xmlNode.LocalName == name) && (matchAllNS || xmlNode.NamespaceURI == ns))
					{
						list.Add(xmlNode);
					}
					xmlNode.SearchDescendantElements(name, matchAllName, ns, matchAllNS, list);
				}
			}
		}

		public XmlNodeList SelectNodes(string xpath)
		{
			return this.SelectNodes(xpath, null);
		}

		public XmlNodeList SelectNodes(string xpath, XmlNamespaceManager nsmgr)
		{
			XPathNavigator xpathNavigator = this.CreateNavigator();
			XPathExpression xpathExpression = xpathNavigator.Compile(xpath);
			if (nsmgr != null)
			{
				xpathExpression.SetContext(nsmgr);
			}
			XPathNodeIterator xpathNodeIterator = xpathNavigator.Select(xpathExpression);
			return new XmlIteratorNodeList(xpathNodeIterator);
		}

		public XmlNode SelectSingleNode(string xpath)
		{
			return this.SelectSingleNode(xpath, null);
		}

		public XmlNode SelectSingleNode(string xpath, XmlNamespaceManager nsmgr)
		{
			XPathNavigator xpathNavigator = this.CreateNavigator();
			XPathExpression xpathExpression = xpathNavigator.Compile(xpath);
			if (nsmgr != null)
			{
				xpathExpression.SetContext(nsmgr);
			}
			XPathNodeIterator xpathNodeIterator = xpathNavigator.Select(xpathExpression);
			if (!xpathNodeIterator.MoveNext())
			{
				return null;
			}
			return ((IHasXmlNode)xpathNodeIterator.Current).GetNode();
		}

		public virtual bool Supports(string feature, string version)
		{
			return string.Compare(feature, "xml", true, CultureInfo.InvariantCulture) == 0 && (string.Compare(version, "1.0", true, CultureInfo.InvariantCulture) == 0 || string.Compare(version, "2.0", true, CultureInfo.InvariantCulture) == 0);
		}

		public abstract void WriteContentTo(XmlWriter w);

		public abstract void WriteTo(XmlWriter w);

		internal XmlNamespaceManager ConstructNamespaceManager()
		{
			XmlDocument xmlDocument = ((!(this is XmlDocument)) ? this.OwnerDocument : ((XmlDocument)this));
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
			XmlNodeType nodeType = this.NodeType;
			XmlElement xmlElement;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					xmlElement = this.ParentNode as XmlElement;
				}
				else
				{
					xmlElement = ((XmlAttribute)this).OwnerElement;
				}
			}
			else
			{
				xmlElement = this as XmlElement;
			}
			while (xmlElement != null)
			{
				for (int i = 0; i < xmlElement.Attributes.Count; i++)
				{
					XmlAttribute xmlAttribute = xmlElement.Attributes[i];
					if (xmlAttribute.Prefix == "xmlns")
					{
						if (xmlNamespaceManager.LookupNamespace(xmlAttribute.LocalName) != xmlAttribute.Value)
						{
							xmlNamespaceManager.AddNamespace(xmlAttribute.LocalName, xmlAttribute.Value);
						}
					}
					else if (xmlAttribute.Name == "xmlns" && xmlNamespaceManager.LookupNamespace(string.Empty) != xmlAttribute.Value)
					{
						xmlNamespaceManager.AddNamespace(string.Empty, xmlAttribute.Value);
					}
				}
				xmlElement = xmlElement.ParentNode as XmlElement;
			}
			return xmlNamespaceManager;
		}

		private static XmlNode.EmptyNodeList emptyList = new XmlNode.EmptyNodeList();

		private XmlDocument ownerDocument;

		private XmlNode parentNode;

		private XmlNodeListChildren childNodes;

		private class EmptyNodeList : XmlNodeList
		{
			public override int Count
			{
				get
				{
					return 0;
				}
			}

			public override IEnumerator GetEnumerator()
			{
				return XmlNode.EmptyNodeList.emptyEnumerator;
			}

			public override XmlNode Item(int index)
			{
				return null;
			}

			private static IEnumerator emptyEnumerator = new object[0].GetEnumerator();
		}
	}
}
