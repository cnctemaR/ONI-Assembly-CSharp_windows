using System;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml.Linq
{
	internal class XNodeNavigator : XPathNavigator
	{
		public XNodeNavigator(XNode node, XmlNameTable nameTable)
		{
			this.node = node;
			this.name_table = nameTable;
		}

		public XNodeNavigator(XNodeNavigator other)
		{
			this.node = other.node;
			this.attr = other.attr;
			this.name_table = other.name_table;
		}

		public override string BaseURI
		{
			get
			{
				return this.node.BaseUri ?? string.Empty;
			}
		}

		public override bool CanEdit
		{
			get
			{
				return true;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				XElement xelement = this.node as XElement;
				return xelement != null && xelement.HasAttributes;
			}
		}

		public override bool HasChildren
		{
			get
			{
				XContainer xcontainer = this.node as XContainer;
				return xcontainer != null && xcontainer.FirstNode != null;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				XElement xelement = this.node as XElement;
				return xelement != null && xelement.IsEmpty;
			}
		}

		public override string LocalName
		{
			get
			{
				switch (this.NodeType)
				{
				case XPathNodeType.Element:
					return ((XElement)this.node).Name.LocalName;
				case XPathNodeType.Attribute:
					return this.attr.Name.LocalName;
				case XPathNodeType.Namespace:
					return (!(this.attr.Name.Namespace == XNamespace.None)) ? this.attr.Name.LocalName : string.Empty;
				case XPathNodeType.ProcessingInstruction:
					return ((XProcessingInstruction)this.node).Target;
				}
				return string.Empty;
			}
		}

		public override string Name
		{
			get
			{
				XPathNodeType nodeType = this.NodeType;
				XName xname;
				if (nodeType != XPathNodeType.Element)
				{
					if (nodeType != XPathNodeType.Attribute)
					{
						return this.LocalName;
					}
					xname = this.attr.Name;
				}
				else
				{
					xname = ((XElement)this.node).Name;
				}
				if (xname.Namespace == XNamespace.None)
				{
					return xname.LocalName;
				}
				XElement xelement = (this.node as XElement) ?? this.node.Parent;
				if (xelement == null)
				{
					return xname.LocalName;
				}
				string prefixOfNamespace = xelement.GetPrefixOfNamespace(xname.Namespace);
				return (prefixOfNamespace.Length <= 0) ? xname.LocalName : (prefixOfNamespace + ":" + xname.LocalName);
			}
		}

		public override string NamespaceURI
		{
			get
			{
				switch (this.NodeType)
				{
				case XPathNodeType.Element:
					return ((XElement)this.node).Name.NamespaceName;
				case XPathNodeType.Attribute:
					return this.attr.Name.NamespaceName;
				case XPathNodeType.Namespace:
					return this.attr.Value;
				default:
					return string.Empty;
				}
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.name_table;
			}
		}

		public override XPathNodeType NodeType
		{
			get
			{
				if (this.attr != null)
				{
					return (!this.attr.IsNamespaceDeclaration) ? XPathNodeType.Attribute : XPathNodeType.Namespace;
				}
				XmlNodeType nodeType = this.node.NodeType;
				switch (nodeType)
				{
				case XmlNodeType.ProcessingInstruction:
					return XPathNodeType.ProcessingInstruction;
				case XmlNodeType.Comment:
					return XPathNodeType.Comment;
				case XmlNodeType.Document:
					return XPathNodeType.Root;
				default:
					if (nodeType != XmlNodeType.Element)
					{
						return XPathNodeType.Text;
					}
					return XPathNodeType.Element;
				}
			}
		}

		public override string Prefix
		{
			get
			{
				XPathNodeType nodeType = this.NodeType;
				XName xname;
				if (nodeType != XPathNodeType.Element)
				{
					if (nodeType != XPathNodeType.Attribute)
					{
						return this.LocalName;
					}
					xname = this.attr.Name;
				}
				else
				{
					xname = ((XElement)this.node).Name;
				}
				if (xname.Namespace == XNamespace.None)
				{
					return string.Empty;
				}
				XElement xelement = (this.node as XElement) ?? this.node.Parent;
				if (xelement == null)
				{
					return string.Empty;
				}
				return xelement.GetPrefixOfNamespace(xname.Namespace);
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return null;
			}
		}

		public override object UnderlyingObject
		{
			get
			{
				return (this.attr == null) ? this.node : this.attr;
			}
		}

		public override string Value
		{
			get
			{
				if (this.attr != null)
				{
					return this.attr.Value;
				}
				switch (this.NodeType)
				{
				case XPathNodeType.Root:
				case XPathNodeType.Element:
					return this.GetInnerText((XContainer)this.node);
				case XPathNodeType.Text:
					return ((XText)this.node).Value;
				case XPathNodeType.ProcessingInstruction:
					return ((XProcessingInstruction)this.node).Data;
				case XPathNodeType.Comment:
					return ((XComment)this.node).Value;
				}
				return string.Empty;
			}
		}

		private string GetInnerText(XContainer node)
		{
			StringBuilder stringBuilder = null;
			foreach (XNode xnode in node.Nodes())
			{
				this.GetInnerText(xnode, ref stringBuilder);
			}
			return (stringBuilder == null) ? string.Empty : stringBuilder.ToString();
		}

		private void GetInnerText(XNode n, ref StringBuilder sb)
		{
			switch (n.NodeType)
			{
			case XmlNodeType.Element:
				foreach (XNode xnode in ((XElement)n).Nodes())
				{
					this.GetInnerText(xnode, ref sb);
				}
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				if (sb == null)
				{
					sb = new StringBuilder();
				}
				sb.Append(((XText)n).Value);
				break;
			}
		}

		public override XPathNavigator Clone()
		{
			return new XNodeNavigator(this);
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			XNodeNavigator xnodeNavigator = other as XNodeNavigator;
			return xnodeNavigator != null && xnodeNavigator.node.Owner == this.node.Owner && this.node == xnodeNavigator.node && this.attr == xnodeNavigator.attr;
		}

		public override bool MoveTo(XPathNavigator other)
		{
			XNodeNavigator xnodeNavigator = other as XNodeNavigator;
			if (xnodeNavigator == null || xnodeNavigator.node.Owner != this.node.Owner)
			{
				return false;
			}
			this.node = xnodeNavigator.node;
			this.attr = xnodeNavigator.attr;
			return true;
		}

		public override bool MoveToFirstAttribute()
		{
			XElement xelement = this.node as XElement;
			if (xelement == null || !xelement.HasAttributes)
			{
				return false;
			}
			foreach (XAttribute xattribute in xelement.Attributes())
			{
				if (!xattribute.IsNamespaceDeclaration)
				{
					this.attr = xattribute;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToFirstChild()
		{
			XContainer xcontainer = this.node as XContainer;
			if (xcontainer == null)
			{
				return false;
			}
			this.node = xcontainer.FirstNode;
			this.attr = null;
			return true;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
		{
			XElement xelement = this.node as XElement;
			while (xelement != null)
			{
				foreach (XAttribute xattribute in xelement.Attributes())
				{
					if (xattribute.IsNamespaceDeclaration)
					{
						this.attr = xattribute;
						return true;
					}
				}
				if (scope == XPathNamespaceScope.Local)
				{
					return false;
				}
				xelement = xelement.Parent;
				continue;
			}
			if (scope != XPathNamespaceScope.All)
			{
				return false;
			}
			this.attr = XNodeNavigator.attr_ns_xml;
			return true;
		}

		public override bool MoveToId(string id)
		{
			throw new NotSupportedException("This XPathNavigator does not support IDs");
		}

		public override bool MoveToNext()
		{
			if (this.node.NextNode == null)
			{
				return false;
			}
			this.node = this.node.NextNode;
			this.attr = null;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (this.attr == null)
			{
				return false;
			}
			if (this.attr.NextAttribute == null)
			{
				return false;
			}
			for (XAttribute xattribute = this.attr.NextAttribute; xattribute != null; xattribute = xattribute.NextAttribute)
			{
				if (!xattribute.IsNamespaceDeclaration)
				{
					this.attr = xattribute;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope scope)
		{
			if (this.attr == null)
			{
				return false;
			}
			for (XAttribute xattribute = this.attr.NextAttribute; xattribute != null; xattribute = xattribute.NextAttribute)
			{
				if (xattribute.IsNamespaceDeclaration)
				{
					this.attr = xattribute;
					return true;
				}
			}
			if (scope == XPathNamespaceScope.Local)
			{
				return false;
			}
			for (XElement xelement = this.attr.Parent.Parent; xelement != null; xelement = xelement.Parent)
			{
				foreach (XAttribute xattribute2 in xelement.Attributes())
				{
					if (xattribute2.IsNamespaceDeclaration)
					{
						this.attr = xattribute2;
						return true;
					}
				}
			}
			if (scope != XPathNamespaceScope.All)
			{
				return false;
			}
			this.attr = XNodeNavigator.attr_ns_xml;
			return true;
		}

		public override bool MoveToParent()
		{
			if (this.attr != null)
			{
				this.attr = null;
				return true;
			}
			if (this.node.Parent == null)
			{
				return false;
			}
			this.node = this.node.Parent;
			return true;
		}

		public override bool MoveToPrevious()
		{
			if (this.node.PreviousNode == null)
			{
				return false;
			}
			this.node = this.node.PreviousNode;
			this.attr = null;
			return true;
		}

		public override void MoveToRoot()
		{
			this.node = this.node.Owner;
			this.attr = null;
		}

		private static readonly XAttribute attr_ns_xml = new XAttribute(XNamespace.Xmlns.GetName("xml"), XNamespace.Xml.NamespaceName);

		private XNode node;

		private XAttribute attr;

		private XmlNameTable name_table;
	}
}
