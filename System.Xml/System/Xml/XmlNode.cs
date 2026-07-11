using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	[DebuggerDisplay("{debuggerDisplayProxy}")]
	public abstract class XmlNode : ICloneable, IEnumerable, IXPathNavigable
	{
		internal XmlNode()
		{
		}

		internal XmlNode(XmlDocument doc)
		{
			if (doc == null)
			{
				throw new ArgumentException(Res.GetString("Cannot create a node without an owner document."));
			}
			this.parentNode = doc;
		}

		public virtual XPathNavigator CreateNavigator()
		{
			XmlDocument xmlDocument = this as XmlDocument;
			if (xmlDocument != null)
			{
				return xmlDocument.CreateNavigator(this);
			}
			return this.OwnerDocument.CreateNavigator(this);
		}

		public XmlNode SelectSingleNode(string xpath)
		{
			XmlNodeList xmlNodeList = this.SelectNodes(xpath);
			if (xmlNodeList == null)
			{
				return null;
			}
			return xmlNodeList[0];
		}

		public XmlNode SelectSingleNode(string xpath, XmlNamespaceManager nsmgr)
		{
			XPathNavigator xpathNavigator = this.CreateNavigator();
			if (xpathNavigator == null)
			{
				return null;
			}
			XPathExpression xpathExpression = xpathNavigator.Compile(xpath);
			xpathExpression.SetContext(nsmgr);
			return new XPathNodeList(xpathNavigator.Select(xpathExpression))[0];
		}

		public XmlNodeList SelectNodes(string xpath)
		{
			XPathNavigator xpathNavigator = this.CreateNavigator();
			if (xpathNavigator == null)
			{
				return null;
			}
			return new XPathNodeList(xpathNavigator.Select(xpath));
		}

		public XmlNodeList SelectNodes(string xpath, XmlNamespaceManager nsmgr)
		{
			XPathNavigator xpathNavigator = this.CreateNavigator();
			if (xpathNavigator == null)
			{
				return null;
			}
			XPathExpression xpathExpression = xpathNavigator.Compile(xpath);
			xpathExpression.SetContext(nsmgr);
			return new XPathNodeList(xpathNavigator.Select(xpathExpression));
		}

		public abstract string Name { get; }

		public virtual string Value
		{
			get
			{
				return null;
			}
			set
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Res.GetString("Cannot set a value on node type '{0}'."), this.NodeType.ToString()));
			}
		}

		public abstract XmlNodeType NodeType { get; }

		public virtual XmlNode ParentNode
		{
			get
			{
				if (this.parentNode.NodeType != XmlNodeType.Document)
				{
					return this.parentNode;
				}
				XmlLinkedNode xmlLinkedNode = this.parentNode.FirstChild as XmlLinkedNode;
				if (xmlLinkedNode != null)
				{
					XmlLinkedNode xmlLinkedNode2 = xmlLinkedNode;
					while (xmlLinkedNode2 != this)
					{
						xmlLinkedNode2 = xmlLinkedNode2.next;
						if (xmlLinkedNode2 == null || xmlLinkedNode2 == xmlLinkedNode)
						{
							goto IL_0045;
						}
					}
					return this.parentNode;
				}
				IL_0045:
				return null;
			}
		}

		public virtual XmlNodeList ChildNodes
		{
			get
			{
				return new XmlChildNodes(this);
			}
		}

		public virtual XmlNode PreviousSibling
		{
			get
			{
				return null;
			}
		}

		public virtual XmlNode NextSibling
		{
			get
			{
				return null;
			}
		}

		public virtual XmlAttributeCollection Attributes
		{
			get
			{
				return null;
			}
		}

		public virtual XmlDocument OwnerDocument
		{
			get
			{
				if (this.parentNode.NodeType == XmlNodeType.Document)
				{
					return (XmlDocument)this.parentNode;
				}
				return this.parentNode.OwnerDocument;
			}
		}

		public virtual XmlNode FirstChild
		{
			get
			{
				XmlLinkedNode lastNode = this.LastNode;
				if (lastNode != null)
				{
					return lastNode.next;
				}
				return null;
			}
		}

		public virtual XmlNode LastChild
		{
			get
			{
				return this.LastNode;
			}
		}

		internal virtual bool IsContainer
		{
			get
			{
				return false;
			}
		}

		internal virtual XmlLinkedNode LastNode
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		internal bool AncestorNode(XmlNode node)
		{
			XmlNode xmlNode = this.ParentNode;
			while (xmlNode != null && xmlNode != this)
			{
				if (xmlNode == node)
				{
					return true;
				}
				xmlNode = xmlNode.ParentNode;
			}
			return false;
		}

		internal bool IsConnected()
		{
			XmlNode xmlNode = this.ParentNode;
			while (xmlNode != null && xmlNode.NodeType != XmlNodeType.Document)
			{
				xmlNode = xmlNode.ParentNode;
			}
			return xmlNode != null;
		}

		public virtual XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
		{
			if (this == newChild || this.AncestorNode(newChild))
			{
				throw new ArgumentException(Res.GetString("Cannot insert a node or any ancestor of that node as a child of itself."));
			}
			if (refChild == null)
			{
				return this.AppendChild(newChild);
			}
			if (!this.IsContainer)
			{
				throw new InvalidOperationException(Res.GetString("The current node cannot contain other nodes."));
			}
			if (refChild.ParentNode != this)
			{
				throw new ArgumentException(Res.GetString("The reference node is not a child of this node."));
			}
			if (newChild == refChild)
			{
				return newChild;
			}
			XmlDocument ownerDocument = newChild.OwnerDocument;
			XmlDocument ownerDocument2 = this.OwnerDocument;
			if (ownerDocument != null && ownerDocument != ownerDocument2 && ownerDocument != this)
			{
				throw new ArgumentException(Res.GetString("The node to be inserted is from a different document context."));
			}
			if (!this.CanInsertBefore(newChild, refChild))
			{
				throw new InvalidOperationException(Res.GetString("Cannot insert the node in the specified location."));
			}
			if (newChild.ParentNode != null)
			{
				newChild.ParentNode.RemoveChild(newChild);
			}
			if (newChild.NodeType == XmlNodeType.DocumentFragment)
			{
				XmlNode firstChild;
				XmlNode xmlNode = (firstChild = newChild.FirstChild);
				if (firstChild != null)
				{
					newChild.RemoveChild(firstChild);
					this.InsertBefore(firstChild, refChild);
					this.InsertAfter(newChild, firstChild);
				}
				return xmlNode;
			}
			if (!(newChild is XmlLinkedNode) || !this.IsValidChildType(newChild.NodeType))
			{
				throw new InvalidOperationException(Res.GetString("The specified node cannot be inserted as the valid child of this node, because the specified node is the wrong type."));
			}
			XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)newChild;
			XmlLinkedNode xmlLinkedNode2 = (XmlLinkedNode)refChild;
			string value = newChild.Value;
			XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(newChild, newChild.ParentNode, this, value, value, XmlNodeChangedAction.Insert);
			if (eventArgs != null)
			{
				this.BeforeEvent(eventArgs);
			}
			if (xmlLinkedNode2 == this.FirstChild)
			{
				xmlLinkedNode.next = xmlLinkedNode2;
				this.LastNode.next = xmlLinkedNode;
				xmlLinkedNode.SetParent(this);
				if (xmlLinkedNode.IsText && xmlLinkedNode2.IsText)
				{
					XmlNode.NestTextNodes(xmlLinkedNode, xmlLinkedNode2);
				}
			}
			else
			{
				XmlLinkedNode xmlLinkedNode3 = (XmlLinkedNode)xmlLinkedNode2.PreviousSibling;
				xmlLinkedNode.next = xmlLinkedNode2;
				xmlLinkedNode3.next = xmlLinkedNode;
				xmlLinkedNode.SetParent(this);
				if (xmlLinkedNode3.IsText)
				{
					if (xmlLinkedNode.IsText)
					{
						XmlNode.NestTextNodes(xmlLinkedNode3, xmlLinkedNode);
						if (xmlLinkedNode2.IsText)
						{
							XmlNode.NestTextNodes(xmlLinkedNode, xmlLinkedNode2);
						}
					}
					else if (xmlLinkedNode2.IsText)
					{
						XmlNode.UnnestTextNodes(xmlLinkedNode3, xmlLinkedNode2);
					}
				}
				else if (xmlLinkedNode.IsText && xmlLinkedNode2.IsText)
				{
					XmlNode.NestTextNodes(xmlLinkedNode, xmlLinkedNode2);
				}
			}
			if (eventArgs != null)
			{
				this.AfterEvent(eventArgs);
			}
			return xmlLinkedNode;
		}

		public virtual XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
		{
			if (this == newChild || this.AncestorNode(newChild))
			{
				throw new ArgumentException(Res.GetString("Cannot insert a node or any ancestor of that node as a child of itself."));
			}
			if (refChild == null)
			{
				return this.PrependChild(newChild);
			}
			if (!this.IsContainer)
			{
				throw new InvalidOperationException(Res.GetString("The current node cannot contain other nodes."));
			}
			if (refChild.ParentNode != this)
			{
				throw new ArgumentException(Res.GetString("The reference node is not a child of this node."));
			}
			if (newChild == refChild)
			{
				return newChild;
			}
			XmlDocument ownerDocument = newChild.OwnerDocument;
			XmlDocument ownerDocument2 = this.OwnerDocument;
			if (ownerDocument != null && ownerDocument != ownerDocument2 && ownerDocument != this)
			{
				throw new ArgumentException(Res.GetString("The node to be inserted is from a different document context."));
			}
			if (!this.CanInsertAfter(newChild, refChild))
			{
				throw new InvalidOperationException(Res.GetString("Cannot insert the node in the specified location."));
			}
			if (newChild.ParentNode != null)
			{
				newChild.ParentNode.RemoveChild(newChild);
			}
			if (newChild.NodeType == XmlNodeType.DocumentFragment)
			{
				XmlNode xmlNode = refChild;
				XmlNode firstChild = newChild.FirstChild;
				XmlNode nextSibling;
				for (XmlNode xmlNode2 = firstChild; xmlNode2 != null; xmlNode2 = nextSibling)
				{
					nextSibling = xmlNode2.NextSibling;
					newChild.RemoveChild(xmlNode2);
					this.InsertAfter(xmlNode2, xmlNode);
					xmlNode = xmlNode2;
				}
				return firstChild;
			}
			if (!(newChild is XmlLinkedNode) || !this.IsValidChildType(newChild.NodeType))
			{
				throw new InvalidOperationException(Res.GetString("The specified node cannot be inserted as the valid child of this node, because the specified node is the wrong type."));
			}
			XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)newChild;
			XmlLinkedNode xmlLinkedNode2 = (XmlLinkedNode)refChild;
			string value = newChild.Value;
			XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(newChild, newChild.ParentNode, this, value, value, XmlNodeChangedAction.Insert);
			if (eventArgs != null)
			{
				this.BeforeEvent(eventArgs);
			}
			if (xmlLinkedNode2 == this.LastNode)
			{
				xmlLinkedNode.next = xmlLinkedNode2.next;
				xmlLinkedNode2.next = xmlLinkedNode;
				this.LastNode = xmlLinkedNode;
				xmlLinkedNode.SetParent(this);
				if (xmlLinkedNode2.IsText && xmlLinkedNode.IsText)
				{
					XmlNode.NestTextNodes(xmlLinkedNode2, xmlLinkedNode);
				}
			}
			else
			{
				XmlLinkedNode next = xmlLinkedNode2.next;
				xmlLinkedNode.next = next;
				xmlLinkedNode2.next = xmlLinkedNode;
				xmlLinkedNode.SetParent(this);
				if (xmlLinkedNode2.IsText)
				{
					if (xmlLinkedNode.IsText)
					{
						XmlNode.NestTextNodes(xmlLinkedNode2, xmlLinkedNode);
						if (next.IsText)
						{
							XmlNode.NestTextNodes(xmlLinkedNode, next);
						}
					}
					else if (next.IsText)
					{
						XmlNode.UnnestTextNodes(xmlLinkedNode2, next);
					}
				}
				else if (xmlLinkedNode.IsText && next.IsText)
				{
					XmlNode.NestTextNodes(xmlLinkedNode, next);
				}
			}
			if (eventArgs != null)
			{
				this.AfterEvent(eventArgs);
			}
			return xmlLinkedNode;
		}

		public virtual XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
		{
			XmlNode nextSibling = oldChild.NextSibling;
			this.RemoveChild(oldChild);
			this.InsertBefore(newChild, nextSibling);
			return oldChild;
		}

		public virtual XmlNode RemoveChild(XmlNode oldChild)
		{
			if (!this.IsContainer)
			{
				throw new InvalidOperationException(Res.GetString("The current node cannot contain other nodes, so the node to be removed is not its child."));
			}
			if (oldChild.ParentNode != this)
			{
				throw new ArgumentException(Res.GetString("The node to be removed is not a child of this node."));
			}
			XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)oldChild;
			string value = xmlLinkedNode.Value;
			XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(xmlLinkedNode, this, null, value, value, XmlNodeChangedAction.Remove);
			if (eventArgs != null)
			{
				this.BeforeEvent(eventArgs);
			}
			XmlLinkedNode lastNode = this.LastNode;
			if (xmlLinkedNode == this.FirstChild)
			{
				if (xmlLinkedNode == lastNode)
				{
					this.LastNode = null;
					xmlLinkedNode.next = null;
					xmlLinkedNode.SetParent(null);
				}
				else
				{
					XmlLinkedNode next = xmlLinkedNode.next;
					if (next.IsText && xmlLinkedNode.IsText)
					{
						XmlNode.UnnestTextNodes(xmlLinkedNode, next);
					}
					lastNode.next = next;
					xmlLinkedNode.next = null;
					xmlLinkedNode.SetParent(null);
				}
			}
			else if (xmlLinkedNode == lastNode)
			{
				XmlLinkedNode xmlLinkedNode2 = (XmlLinkedNode)xmlLinkedNode.PreviousSibling;
				xmlLinkedNode2.next = xmlLinkedNode.next;
				this.LastNode = xmlLinkedNode2;
				xmlLinkedNode.next = null;
				xmlLinkedNode.SetParent(null);
			}
			else
			{
				XmlLinkedNode xmlLinkedNode3 = (XmlLinkedNode)xmlLinkedNode.PreviousSibling;
				XmlLinkedNode next2 = xmlLinkedNode.next;
				if (next2.IsText)
				{
					if (xmlLinkedNode3.IsText)
					{
						XmlNode.NestTextNodes(xmlLinkedNode3, next2);
					}
					else if (xmlLinkedNode.IsText)
					{
						XmlNode.UnnestTextNodes(xmlLinkedNode, next2);
					}
				}
				xmlLinkedNode3.next = next2;
				xmlLinkedNode.next = null;
				xmlLinkedNode.SetParent(null);
			}
			if (eventArgs != null)
			{
				this.AfterEvent(eventArgs);
			}
			return oldChild;
		}

		public virtual XmlNode PrependChild(XmlNode newChild)
		{
			return this.InsertBefore(newChild, this.FirstChild);
		}

		public virtual XmlNode AppendChild(XmlNode newChild)
		{
			XmlDocument xmlDocument = this.OwnerDocument;
			if (xmlDocument == null)
			{
				xmlDocument = this as XmlDocument;
			}
			if (!this.IsContainer)
			{
				throw new InvalidOperationException(Res.GetString("The current node cannot contain other nodes."));
			}
			if (this == newChild || this.AncestorNode(newChild))
			{
				throw new ArgumentException(Res.GetString("Cannot insert a node or any ancestor of that node as a child of itself."));
			}
			if (newChild.ParentNode != null)
			{
				newChild.ParentNode.RemoveChild(newChild);
			}
			XmlDocument ownerDocument = newChild.OwnerDocument;
			if (ownerDocument != null && ownerDocument != xmlDocument && ownerDocument != this)
			{
				throw new ArgumentException(Res.GetString("The node to be inserted is from a different document context."));
			}
			if (newChild.NodeType == XmlNodeType.DocumentFragment)
			{
				XmlNode firstChild = newChild.FirstChild;
				XmlNode nextSibling;
				for (XmlNode xmlNode = firstChild; xmlNode != null; xmlNode = nextSibling)
				{
					nextSibling = xmlNode.NextSibling;
					newChild.RemoveChild(xmlNode);
					this.AppendChild(xmlNode);
				}
				return firstChild;
			}
			if (!(newChild is XmlLinkedNode) || !this.IsValidChildType(newChild.NodeType))
			{
				throw new InvalidOperationException(Res.GetString("The specified node cannot be inserted as the valid child of this node, because the specified node is the wrong type."));
			}
			if (!this.CanInsertAfter(newChild, this.LastChild))
			{
				throw new InvalidOperationException(Res.GetString("Cannot insert the node in the specified location."));
			}
			string value = newChild.Value;
			XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(newChild, newChild.ParentNode, this, value, value, XmlNodeChangedAction.Insert);
			if (eventArgs != null)
			{
				this.BeforeEvent(eventArgs);
			}
			XmlLinkedNode lastNode = this.LastNode;
			XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)newChild;
			if (lastNode == null)
			{
				xmlLinkedNode.next = xmlLinkedNode;
				this.LastNode = xmlLinkedNode;
				xmlLinkedNode.SetParent(this);
			}
			else
			{
				xmlLinkedNode.next = lastNode.next;
				lastNode.next = xmlLinkedNode;
				this.LastNode = xmlLinkedNode;
				xmlLinkedNode.SetParent(this);
				if (lastNode.IsText && xmlLinkedNode.IsText)
				{
					XmlNode.NestTextNodes(lastNode, xmlLinkedNode);
				}
			}
			if (eventArgs != null)
			{
				this.AfterEvent(eventArgs);
			}
			return xmlLinkedNode;
		}

		internal virtual XmlNode AppendChildForLoad(XmlNode newChild, XmlDocument doc)
		{
			XmlNodeChangedEventArgs insertEventArgsForLoad = doc.GetInsertEventArgsForLoad(newChild, this);
			if (insertEventArgsForLoad != null)
			{
				doc.BeforeEvent(insertEventArgsForLoad);
			}
			XmlLinkedNode lastNode = this.LastNode;
			XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)newChild;
			if (lastNode == null)
			{
				xmlLinkedNode.next = xmlLinkedNode;
				this.LastNode = xmlLinkedNode;
				xmlLinkedNode.SetParentForLoad(this);
			}
			else
			{
				xmlLinkedNode.next = lastNode.next;
				lastNode.next = xmlLinkedNode;
				this.LastNode = xmlLinkedNode;
				if (lastNode.IsText && xmlLinkedNode.IsText)
				{
					XmlNode.NestTextNodes(lastNode, xmlLinkedNode);
				}
				else
				{
					xmlLinkedNode.SetParentForLoad(this);
				}
			}
			if (insertEventArgsForLoad != null)
			{
				doc.AfterEvent(insertEventArgsForLoad);
			}
			return xmlLinkedNode;
		}

		internal virtual bool IsValidChildType(XmlNodeType type)
		{
			return false;
		}

		internal virtual bool CanInsertBefore(XmlNode newChild, XmlNode refChild)
		{
			return true;
		}

		internal virtual bool CanInsertAfter(XmlNode newChild, XmlNode refChild)
		{
			return true;
		}

		public virtual bool HasChildNodes
		{
			get
			{
				return this.LastNode != null;
			}
		}

		public abstract XmlNode CloneNode(bool deep);

		internal virtual void CopyChildren(XmlDocument doc, XmlNode container, bool deep)
		{
			for (XmlNode xmlNode = container.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				this.AppendChildForLoad(xmlNode.CloneNode(deep), doc);
			}
		}

		public virtual void Normalize()
		{
			XmlNode xmlNode = null;
			StringBuilder stringBuilder = new StringBuilder();
			XmlNode xmlNode2 = this.FirstChild;
			while (xmlNode2 != null)
			{
				XmlNode nextSibling = xmlNode2.NextSibling;
				XmlNodeType nodeType = xmlNode2.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					xmlNode2.Normalize();
					goto IL_0069;
				}
				if (nodeType != XmlNodeType.Text && nodeType - XmlNodeType.Whitespace > 1)
				{
					goto IL_0069;
				}
				stringBuilder.Append(xmlNode2.Value);
				if (this.NormalizeWinner(xmlNode, xmlNode2) == xmlNode)
				{
					this.RemoveChild(xmlNode2);
				}
				else
				{
					if (xmlNode != null)
					{
						this.RemoveChild(xmlNode);
					}
					xmlNode = xmlNode2;
				}
				IL_0088:
				xmlNode2 = nextSibling;
				continue;
				IL_0069:
				if (xmlNode != null)
				{
					xmlNode.Value = stringBuilder.ToString();
					xmlNode = null;
				}
				stringBuilder.Remove(0, stringBuilder.Length);
				goto IL_0088;
			}
			if (xmlNode != null && stringBuilder.Length > 0)
			{
				xmlNode.Value = stringBuilder.ToString();
			}
		}

		private XmlNode NormalizeWinner(XmlNode firstNode, XmlNode secondNode)
		{
			if (firstNode == null)
			{
				return secondNode;
			}
			if (firstNode.NodeType == XmlNodeType.Text)
			{
				return firstNode;
			}
			if (secondNode.NodeType == XmlNodeType.Text)
			{
				return secondNode;
			}
			if (firstNode.NodeType == XmlNodeType.SignificantWhitespace)
			{
				return firstNode;
			}
			if (secondNode.NodeType == XmlNodeType.SignificantWhitespace)
			{
				return secondNode;
			}
			if (firstNode.NodeType == XmlNodeType.Whitespace)
			{
				return firstNode;
			}
			if (secondNode.NodeType == XmlNodeType.Whitespace)
			{
				return secondNode;
			}
			return null;
		}

		public virtual bool Supports(string feature, string version)
		{
			return string.Compare("XML", feature, StringComparison.OrdinalIgnoreCase) == 0 && (version == null || version == "1.0" || version == "2.0");
		}

		public virtual string NamespaceURI
		{
			get
			{
				return string.Empty;
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

		public abstract string LocalName { get; }

		public virtual bool IsReadOnly
		{
			get
			{
				XmlDocument ownerDocument = this.OwnerDocument;
				return XmlNode.HasReadOnlyParent(this);
			}
		}

		internal static bool HasReadOnlyParent(XmlNode n)
		{
			while (n != null)
			{
				XmlNodeType nodeType = n.NodeType;
				if (nodeType != XmlNodeType.Attribute)
				{
					if (nodeType - XmlNodeType.EntityReference <= 1)
					{
						return true;
					}
					n = n.ParentNode;
				}
				else
				{
					n = ((XmlAttribute)n).OwnerElement;
				}
			}
			return false;
		}

		public virtual XmlNode Clone()
		{
			return this.CloneNode(true);
		}

		object ICloneable.Clone()
		{
			return this.CloneNode(true);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new XmlChildEnumerator(this);
		}

		public IEnumerator GetEnumerator()
		{
			return new XmlChildEnumerator(this);
		}

		private void AppendChildText(StringBuilder builder)
		{
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.FirstChild == null)
				{
					if (xmlNode.NodeType == XmlNodeType.Text || xmlNode.NodeType == XmlNodeType.CDATA || xmlNode.NodeType == XmlNodeType.Whitespace || xmlNode.NodeType == XmlNodeType.SignificantWhitespace)
					{
						builder.Append(xmlNode.InnerText);
					}
				}
				else
				{
					xmlNode.AppendChildText(builder);
				}
			}
		}

		public virtual string InnerText
		{
			get
			{
				XmlNode firstChild = this.FirstChild;
				if (firstChild == null)
				{
					return string.Empty;
				}
				if (firstChild.NextSibling == null)
				{
					XmlNodeType nodeType = firstChild.NodeType;
					if (nodeType - XmlNodeType.Text <= 1 || nodeType - XmlNodeType.Whitespace <= 1)
					{
						return firstChild.Value;
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				this.AppendChildText(stringBuilder);
				return stringBuilder.ToString();
			}
			set
			{
				XmlNode firstChild = this.FirstChild;
				if (firstChild != null && firstChild.NextSibling == null && firstChild.NodeType == XmlNodeType.Text)
				{
					firstChild.Value = value;
					return;
				}
				this.RemoveAll();
				this.AppendChild(this.OwnerDocument.CreateTextNode(value));
			}
		}

		public virtual string OuterXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				XmlDOMTextWriter xmlDOMTextWriter = new XmlDOMTextWriter(stringWriter);
				try
				{
					this.WriteTo(xmlDOMTextWriter);
				}
				finally
				{
					xmlDOMTextWriter.Close();
				}
				return stringWriter.ToString();
			}
		}

		public virtual string InnerXml
		{
			get
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				XmlDOMTextWriter xmlDOMTextWriter = new XmlDOMTextWriter(stringWriter);
				try
				{
					this.WriteContentTo(xmlDOMTextWriter);
				}
				finally
				{
					xmlDOMTextWriter.Close();
				}
				return stringWriter.ToString();
			}
			set
			{
				throw new InvalidOperationException(Res.GetString("Cannot set the 'InnerXml' for the current node because it is either read-only or cannot have children."));
			}
		}

		public virtual IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return XmlDocument.NotKnownSchemaInfo;
			}
		}

		public virtual string BaseURI
		{
			get
			{
				for (XmlNode xmlNode = this.ParentNode; xmlNode != null; xmlNode = xmlNode.ParentNode)
				{
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType == XmlNodeType.EntityReference)
					{
						return ((XmlEntityReference)xmlNode).ChildBaseURI;
					}
					if (nodeType == XmlNodeType.Document || nodeType == XmlNodeType.Entity || nodeType == XmlNodeType.Attribute)
					{
						return xmlNode.BaseURI;
					}
				}
				return string.Empty;
			}
		}

		public abstract void WriteTo(XmlWriter w);

		public abstract void WriteContentTo(XmlWriter w);

		public virtual void RemoveAll()
		{
			XmlNode nextSibling;
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = nextSibling)
			{
				nextSibling = xmlNode.NextSibling;
				this.RemoveChild(xmlNode);
			}
		}

		internal XmlDocument Document
		{
			get
			{
				if (this.NodeType == XmlNodeType.Document)
				{
					return (XmlDocument)this;
				}
				return this.OwnerDocument;
			}
		}

		public virtual string GetNamespaceOfPrefix(string prefix)
		{
			string namespaceOfPrefixStrict = this.GetNamespaceOfPrefixStrict(prefix);
			if (namespaceOfPrefixStrict == null)
			{
				return string.Empty;
			}
			return namespaceOfPrefixStrict;
		}

		internal string GetNamespaceOfPrefixStrict(string prefix)
		{
			XmlDocument document = this.Document;
			if (document != null)
			{
				prefix = document.NameTable.Get(prefix);
				if (prefix == null)
				{
					return null;
				}
				XmlNode xmlNode = this;
				while (xmlNode != null)
				{
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						if (xmlElement.HasAttributes)
						{
							XmlAttributeCollection attributes = xmlElement.Attributes;
							if (prefix.Length == 0)
							{
								for (int i = 0; i < attributes.Count; i++)
								{
									XmlAttribute xmlAttribute = attributes[i];
									if (xmlAttribute.Prefix.Length == 0 && Ref.Equal(xmlAttribute.LocalName, document.strXmlns))
									{
										return xmlAttribute.Value;
									}
								}
							}
							else
							{
								for (int j = 0; j < attributes.Count; j++)
								{
									XmlAttribute xmlAttribute2 = attributes[j];
									if (Ref.Equal(xmlAttribute2.Prefix, document.strXmlns))
									{
										if (Ref.Equal(xmlAttribute2.LocalName, prefix))
										{
											return xmlAttribute2.Value;
										}
									}
									else if (Ref.Equal(xmlAttribute2.Prefix, prefix))
									{
										return xmlAttribute2.NamespaceURI;
									}
								}
							}
						}
						if (Ref.Equal(xmlNode.Prefix, prefix))
						{
							return xmlNode.NamespaceURI;
						}
						xmlNode = xmlNode.ParentNode;
					}
					else if (xmlNode.NodeType == XmlNodeType.Attribute)
					{
						xmlNode = ((XmlAttribute)xmlNode).OwnerElement;
					}
					else
					{
						xmlNode = xmlNode.ParentNode;
					}
				}
				if (Ref.Equal(document.strXml, prefix))
				{
					return document.strReservedXml;
				}
				if (Ref.Equal(document.strXmlns, prefix))
				{
					return document.strReservedXmlns;
				}
			}
			return null;
		}

		public virtual string GetPrefixOfNamespace(string namespaceURI)
		{
			string prefixOfNamespaceStrict = this.GetPrefixOfNamespaceStrict(namespaceURI);
			if (prefixOfNamespaceStrict == null)
			{
				return string.Empty;
			}
			return prefixOfNamespaceStrict;
		}

		internal string GetPrefixOfNamespaceStrict(string namespaceURI)
		{
			XmlDocument document = this.Document;
			if (document != null)
			{
				namespaceURI = document.NameTable.Add(namespaceURI);
				XmlNode xmlNode = this;
				while (xmlNode != null)
				{
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						if (xmlElement.HasAttributes)
						{
							XmlAttributeCollection attributes = xmlElement.Attributes;
							for (int i = 0; i < attributes.Count; i++)
							{
								XmlAttribute xmlAttribute = attributes[i];
								if (xmlAttribute.Prefix.Length == 0)
								{
									if (Ref.Equal(xmlAttribute.LocalName, document.strXmlns) && xmlAttribute.Value == namespaceURI)
									{
										return string.Empty;
									}
								}
								else if (Ref.Equal(xmlAttribute.Prefix, document.strXmlns))
								{
									if (xmlAttribute.Value == namespaceURI)
									{
										return xmlAttribute.LocalName;
									}
								}
								else if (Ref.Equal(xmlAttribute.NamespaceURI, namespaceURI))
								{
									return xmlAttribute.Prefix;
								}
							}
						}
						if (Ref.Equal(xmlNode.NamespaceURI, namespaceURI))
						{
							return xmlNode.Prefix;
						}
						xmlNode = xmlNode.ParentNode;
					}
					else if (xmlNode.NodeType == XmlNodeType.Attribute)
					{
						xmlNode = ((XmlAttribute)xmlNode).OwnerElement;
					}
					else
					{
						xmlNode = xmlNode.ParentNode;
					}
				}
				if (Ref.Equal(document.strReservedXml, namespaceURI))
				{
					return document.strXml;
				}
				if (Ref.Equal(document.strReservedXmlns, namespaceURI))
				{
					return document.strXmlns;
				}
			}
			return null;
		}

		public virtual XmlElement this[string name]
		{
			get
			{
				for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
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
				for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.LocalName == localname && xmlNode.NamespaceURI == ns)
					{
						return (XmlElement)xmlNode;
					}
				}
				return null;
			}
		}

		internal virtual void SetParent(XmlNode node)
		{
			if (node == null)
			{
				this.parentNode = this.OwnerDocument;
				return;
			}
			this.parentNode = node;
		}

		internal virtual void SetParentForLoad(XmlNode node)
		{
			this.parentNode = node;
		}

		internal static void SplitName(string name, out string prefix, out string localName)
		{
			int num = name.IndexOf(':');
			if (-1 == num || num == 0 || name.Length - 1 == num)
			{
				prefix = string.Empty;
				localName = name;
				return;
			}
			prefix = name.Substring(0, num);
			localName = name.Substring(num + 1);
		}

		internal virtual XmlNode FindChild(XmlNodeType type)
		{
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType == type)
				{
					return xmlNode;
				}
			}
			return null;
		}

		internal virtual XmlNodeChangedEventArgs GetEventArgs(XmlNode node, XmlNode oldParent, XmlNode newParent, string oldValue, string newValue, XmlNodeChangedAction action)
		{
			XmlDocument ownerDocument = this.OwnerDocument;
			if (ownerDocument == null)
			{
				return null;
			}
			if (!ownerDocument.IsLoading && ((newParent != null && newParent.IsReadOnly) || (oldParent != null && oldParent.IsReadOnly)))
			{
				throw new InvalidOperationException(Res.GetString("This node is read-only. It cannot be modified."));
			}
			return ownerDocument.GetEventArgs(node, oldParent, newParent, oldValue, newValue, action);
		}

		internal virtual void BeforeEvent(XmlNodeChangedEventArgs args)
		{
			if (args != null)
			{
				this.OwnerDocument.BeforeEvent(args);
			}
		}

		internal virtual void AfterEvent(XmlNodeChangedEventArgs args)
		{
			if (args != null)
			{
				this.OwnerDocument.AfterEvent(args);
			}
		}

		internal virtual XmlSpace XmlSpace
		{
			get
			{
				XmlNode xmlNode = this;
				for (;;)
				{
					XmlElement xmlElement = xmlNode as XmlElement;
					if (xmlElement != null && xmlElement.HasAttribute("xml:space"))
					{
						string text = XmlConvert.TrimString(xmlElement.GetAttribute("xml:space"));
						if (text == "default")
						{
							break;
						}
						if (text == "preserve")
						{
							return XmlSpace.Preserve;
						}
					}
					xmlNode = xmlNode.ParentNode;
					if (xmlNode == null)
					{
						return XmlSpace.None;
					}
				}
				return XmlSpace.Default;
			}
		}

		internal virtual string XmlLang
		{
			get
			{
				XmlNode xmlNode = this;
				XmlElement xmlElement;
				for (;;)
				{
					xmlElement = xmlNode as XmlElement;
					if (xmlElement != null && xmlElement.HasAttribute("xml:lang"))
					{
						break;
					}
					xmlNode = xmlNode.ParentNode;
					if (xmlNode == null)
					{
						goto Block_3;
					}
				}
				return xmlElement.GetAttribute("xml:lang");
				Block_3:
				return string.Empty;
			}
		}

		internal virtual XPathNodeType XPNodeType
		{
			get
			{
				return (XPathNodeType)(-1);
			}
		}

		internal virtual string XPLocalName
		{
			get
			{
				return string.Empty;
			}
		}

		internal virtual string GetXPAttribute(string localName, string namespaceURI)
		{
			return string.Empty;
		}

		internal virtual bool IsText
		{
			get
			{
				return false;
			}
		}

		public virtual XmlNode PreviousText
		{
			get
			{
				return null;
			}
		}

		internal static void NestTextNodes(XmlNode prevNode, XmlNode nextNode)
		{
			nextNode.parentNode = prevNode;
		}

		internal static void UnnestTextNodes(XmlNode prevNode, XmlNode nextNode)
		{
			nextNode.parentNode = prevNode.ParentNode;
		}

		private object debuggerDisplayProxy
		{
			get
			{
				return new DebuggerDisplayXmlNodeProxy(this);
			}
		}

		internal XmlNode parentNode;
	}
}
