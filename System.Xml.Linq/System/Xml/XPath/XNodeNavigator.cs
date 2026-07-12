using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace System.Xml.XPath
{
	internal class XNodeNavigator : XPathNavigator, IXmlLineInfo
	{
		public XNodeNavigator(XNode node, XmlNameTable nameTable)
		{
			this._source = node;
			this._nameTable = ((nameTable != null) ? nameTable : XNodeNavigator.CreateNameTable());
		}

		public XNodeNavigator(XNodeNavigator other)
		{
			this._source = other._source;
			this._parent = other._parent;
			this._nameTable = other._nameTable;
		}

		public override string BaseURI
		{
			get
			{
				if (this._source != null)
				{
					return this._source.BaseUri;
				}
				if (this._parent != null)
				{
					return this._parent.BaseUri;
				}
				return string.Empty;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				XElement xelement = this._source as XElement;
				if (xelement != null)
				{
					using (IEnumerator<XAttribute> enumerator = xelement.Attributes().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!enumerator.Current.IsNamespaceDeclaration)
							{
								return true;
							}
						}
					}
					return false;
				}
				return false;
			}
		}

		public override bool HasChildren
		{
			get
			{
				XContainer xcontainer = this._source as XContainer;
				if (xcontainer != null)
				{
					foreach (XNode xnode in xcontainer.Nodes())
					{
						if (XNodeNavigator.IsContent(xcontainer, xnode))
						{
							return true;
						}
					}
					return false;
				}
				return false;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				XElement xelement = this._source as XElement;
				return xelement != null && xelement.IsEmpty;
			}
		}

		public override string LocalName
		{
			get
			{
				return this._nameTable.Add(this.GetLocalName());
			}
		}

		private string GetLocalName()
		{
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				return xelement.Name.LocalName;
			}
			XAttribute xattribute = this._source as XAttribute;
			if (xattribute != null)
			{
				if (this._parent != null && xattribute.Name.NamespaceName.Length == 0)
				{
					return string.Empty;
				}
				return xattribute.Name.LocalName;
			}
			else
			{
				XProcessingInstruction xprocessingInstruction = this._source as XProcessingInstruction;
				if (xprocessingInstruction != null)
				{
					return xprocessingInstruction.Target;
				}
				return string.Empty;
			}
		}

		public override string Name
		{
			get
			{
				string prefix = this.GetPrefix();
				if (prefix.Length == 0)
				{
					return this._nameTable.Add(this.GetLocalName());
				}
				return this._nameTable.Add(prefix + ":" + this.GetLocalName());
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this._nameTable.Add(this.GetNamespaceURI());
			}
		}

		private string GetNamespaceURI()
		{
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				return xelement.Name.NamespaceName;
			}
			XAttribute xattribute = this._source as XAttribute;
			if (xattribute == null)
			{
				return string.Empty;
			}
			if (this._parent != null)
			{
				return string.Empty;
			}
			return xattribute.Name.NamespaceName;
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this._nameTable;
			}
		}

		public override XPathNodeType NodeType
		{
			get
			{
				if (this._source != null)
				{
					switch (this._source.NodeType)
					{
					case XmlNodeType.Element:
						return XPathNodeType.Element;
					case XmlNodeType.Attribute:
						if (!((XAttribute)this._source).IsNamespaceDeclaration)
						{
							return XPathNodeType.Attribute;
						}
						return XPathNodeType.Namespace;
					case XmlNodeType.ProcessingInstruction:
						return XPathNodeType.ProcessingInstruction;
					case XmlNodeType.Comment:
						return XPathNodeType.Comment;
					case XmlNodeType.Document:
						return XPathNodeType.Root;
					}
					return XPathNodeType.Text;
				}
				return XPathNodeType.Text;
			}
		}

		public override string Prefix
		{
			get
			{
				return this._nameTable.Add(this.GetPrefix());
			}
		}

		private string GetPrefix()
		{
			XElement xelement = this._source as XElement;
			if (xelement == null)
			{
				XAttribute xattribute = this._source as XAttribute;
				if (xattribute != null)
				{
					if (this._parent != null)
					{
						return string.Empty;
					}
					string prefixOfNamespace = xattribute.GetPrefixOfNamespace(xattribute.Name.Namespace);
					if (prefixOfNamespace != null)
					{
						return prefixOfNamespace;
					}
				}
				return string.Empty;
			}
			string prefixOfNamespace2 = xelement.GetPrefixOfNamespace(xelement.Name.Namespace);
			if (prefixOfNamespace2 != null)
			{
				return prefixOfNamespace2;
			}
			return string.Empty;
		}

		public override object UnderlyingObject
		{
			get
			{
				return this._source;
			}
		}

		public override string Value
		{
			get
			{
				if (this._source != null)
				{
					switch (this._source.NodeType)
					{
					case XmlNodeType.Element:
						return ((XElement)this._source).Value;
					case XmlNodeType.Attribute:
						return ((XAttribute)this._source).Value;
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						return XNodeNavigator.CollectText((XText)this._source);
					case XmlNodeType.ProcessingInstruction:
						return ((XProcessingInstruction)this._source).Data;
					case XmlNodeType.Comment:
						return ((XComment)this._source).Value;
					case XmlNodeType.Document:
					{
						XElement root = ((XDocument)this._source).Root;
						if (root == null)
						{
							return string.Empty;
						}
						return root.Value;
					}
					}
					return string.Empty;
				}
				return string.Empty;
			}
		}

		public override XPathNavigator Clone()
		{
			return new XNodeNavigator(this);
		}

		public override bool IsSamePosition(XPathNavigator navigator)
		{
			XNodeNavigator xnodeNavigator = navigator as XNodeNavigator;
			return xnodeNavigator != null && XNodeNavigator.IsSamePosition(this, xnodeNavigator);
		}

		public override bool MoveTo(XPathNavigator navigator)
		{
			XNodeNavigator xnodeNavigator = navigator as XNodeNavigator;
			if (xnodeNavigator != null)
			{
				this._source = xnodeNavigator._source;
				this._parent = xnodeNavigator._parent;
				return true;
			}
			return false;
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				foreach (XAttribute xattribute in xelement.Attributes())
				{
					if (xattribute.Name.LocalName == localName && xattribute.Name.NamespaceName == namespaceName && !xattribute.IsNamespaceDeclaration)
					{
						this._source = xattribute;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public override bool MoveToChild(string localName, string namespaceName)
		{
			XContainer xcontainer = this._source as XContainer;
			if (xcontainer != null)
			{
				foreach (XElement xelement in xcontainer.Elements())
				{
					if (xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName)
					{
						this._source = xelement;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public override bool MoveToChild(XPathNodeType type)
		{
			XContainer xcontainer = this._source as XContainer;
			if (xcontainer != null)
			{
				int num = XNodeNavigator.GetElementContentMask(type);
				if ((24 & num) != 0 && xcontainer.GetParent() == null && xcontainer is XDocument)
				{
					num &= -25;
				}
				foreach (XNode xnode in xcontainer.Nodes())
				{
					if (((1 << (int)xnode.NodeType) & num) != 0)
					{
						this._source = xnode;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				foreach (XAttribute xattribute in xelement.Attributes())
				{
					if (!xattribute.IsNamespaceDeclaration)
					{
						this._source = xattribute;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public override bool MoveToFirstChild()
		{
			XContainer xcontainer = this._source as XContainer;
			if (xcontainer != null)
			{
				foreach (XNode xnode in xcontainer.Nodes())
				{
					if (XNodeNavigator.IsContent(xcontainer, xnode))
					{
						this._source = xnode;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
		{
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				XAttribute xattribute = null;
				switch (scope)
				{
				case XPathNamespaceScope.All:
					xattribute = XNodeNavigator.GetFirstNamespaceDeclarationGlobal(xelement);
					if (xattribute == null)
					{
						xattribute = XNodeNavigator.GetXmlNamespaceDeclaration();
					}
					break;
				case XPathNamespaceScope.ExcludeXml:
					for (xattribute = XNodeNavigator.GetFirstNamespaceDeclarationGlobal(xelement); xattribute != null; xattribute = XNodeNavigator.GetNextNamespaceDeclarationGlobal(xattribute))
					{
						if (!(xattribute.Name.LocalName == "xml"))
						{
							break;
						}
					}
					break;
				case XPathNamespaceScope.Local:
					xattribute = XNodeNavigator.GetFirstNamespaceDeclarationLocal(xelement);
					break;
				}
				if (xattribute != null)
				{
					this._source = xattribute;
					this._parent = xelement;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToId(string id)
		{
			throw new NotSupportedException("This XPathNavigator does not support IDs.");
		}

		public override bool MoveToNamespace(string localName)
		{
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				if (localName == "xmlns")
				{
					return false;
				}
				if (localName != null && localName.Length == 0)
				{
					localName = "xmlns";
				}
				for (XAttribute xattribute = XNodeNavigator.GetFirstNamespaceDeclarationGlobal(xelement); xattribute != null; xattribute = XNodeNavigator.GetNextNamespaceDeclarationGlobal(xattribute))
				{
					if (xattribute.Name.LocalName == localName)
					{
						this._source = xattribute;
						this._parent = xelement;
						return true;
					}
				}
				if (localName == "xml")
				{
					this._source = XNodeNavigator.GetXmlNamespaceDeclaration();
					this._parent = xelement;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToNext()
		{
			XNode xnode = this._source as XNode;
			if (xnode != null)
			{
				XContainer parent = xnode.GetParent();
				if (parent != null)
				{
					XNode nextNode;
					for (XNode xnode2 = xnode; xnode2 != null; xnode2 = nextNode)
					{
						nextNode = xnode2.NextNode;
						if (nextNode == null)
						{
							break;
						}
						if (XNodeNavigator.IsContent(parent, nextNode) && (!(xnode2 is XText) || !(nextNode is XText)))
						{
							this._source = nextNode;
							return true;
						}
					}
				}
			}
			return false;
		}

		public override bool MoveToNext(string localName, string namespaceName)
		{
			XNode xnode = this._source as XNode;
			if (xnode != null)
			{
				foreach (XElement xelement in xnode.ElementsAfterSelf())
				{
					if (xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName)
					{
						this._source = xelement;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public override bool MoveToNext(XPathNodeType type)
		{
			XNode xnode = this._source as XNode;
			if (xnode != null)
			{
				XContainer parent = xnode.GetParent();
				if (parent != null)
				{
					int num = XNodeNavigator.GetElementContentMask(type);
					if ((24 & num) != 0 && parent.GetParent() == null && parent is XDocument)
					{
						num &= -25;
					}
					XNode nextNode;
					for (XNode xnode2 = xnode; xnode2 != null; xnode2 = nextNode)
					{
						nextNode = xnode2.NextNode;
						if (((1 << (int)nextNode.NodeType) & num) != 0 && (!(xnode2 is XText) || !(nextNode is XText)))
						{
							this._source = nextNode;
							return true;
						}
					}
				}
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			XAttribute xattribute = this._source as XAttribute;
			if (xattribute != null && this._parent == null && (XElement)xattribute.GetParent() != null)
			{
				for (XAttribute xattribute2 = xattribute.NextAttribute; xattribute2 != null; xattribute2 = xattribute2.NextAttribute)
				{
					if (!xattribute2.IsNamespaceDeclaration)
					{
						this._source = xattribute2;
						return true;
					}
				}
			}
			return false;
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope scope)
		{
			XAttribute xattribute = this._source as XAttribute;
			if (xattribute != null && this._parent != null && !XNodeNavigator.IsXmlNamespaceDeclaration(xattribute))
			{
				switch (scope)
				{
				case XPathNamespaceScope.All:
					do
					{
						xattribute = XNodeNavigator.GetNextNamespaceDeclarationGlobal(xattribute);
					}
					while (xattribute != null && XNodeNavigator.HasNamespaceDeclarationInScope(xattribute, this._parent));
					if (xattribute == null && !XNodeNavigator.HasNamespaceDeclarationInScope(XNodeNavigator.GetXmlNamespaceDeclaration(), this._parent))
					{
						xattribute = XNodeNavigator.GetXmlNamespaceDeclaration();
					}
					break;
				case XPathNamespaceScope.ExcludeXml:
					do
					{
						xattribute = XNodeNavigator.GetNextNamespaceDeclarationGlobal(xattribute);
						if (xattribute == null)
						{
							break;
						}
					}
					while (xattribute.Name.LocalName == "xml" || XNodeNavigator.HasNamespaceDeclarationInScope(xattribute, this._parent));
					break;
				case XPathNamespaceScope.Local:
					if (xattribute.GetParent() != this._parent)
					{
						return false;
					}
					xattribute = XNodeNavigator.GetNextNamespaceDeclarationLocal(xattribute);
					break;
				}
				if (xattribute != null)
				{
					this._source = xattribute;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToParent()
		{
			if (this._parent != null)
			{
				this._source = this._parent;
				this._parent = null;
				return true;
			}
			XNode parent = this._source.GetParent();
			if (parent != null)
			{
				this._source = parent;
				return true;
			}
			return false;
		}

		public override bool MoveToPrevious()
		{
			XNode xnode = this._source as XNode;
			if (xnode != null)
			{
				XContainer parent = xnode.GetParent();
				if (parent != null)
				{
					XNode xnode2 = null;
					foreach (XNode xnode3 in parent.Nodes())
					{
						if (xnode3 == xnode)
						{
							if (xnode2 != null)
							{
								this._source = xnode2;
								return true;
							}
							return false;
						}
						else if (XNodeNavigator.IsContent(parent, xnode3))
						{
							xnode2 = xnode3;
						}
					}
					return false;
				}
			}
			return false;
		}

		public override XmlReader ReadSubtree()
		{
			XContainer xcontainer = this._source as XContainer;
			if (xcontainer == null)
			{
				throw new InvalidOperationException(global::SR.Format("This operation is not valid on a node of type {0}.", this.NodeType));
			}
			return xcontainer.CreateReader();
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			IXmlLineInfo source = this._source;
			return source != null && source.HasLineInfo();
		}

		int IXmlLineInfo.LineNumber
		{
			get
			{
				IXmlLineInfo source = this._source;
				if (source != null)
				{
					return source.LineNumber;
				}
				return 0;
			}
		}

		int IXmlLineInfo.LinePosition
		{
			get
			{
				IXmlLineInfo source = this._source;
				if (source != null)
				{
					return source.LinePosition;
				}
				return 0;
			}
		}

		private static string CollectText(XText n)
		{
			string text = n.Value;
			if (n.GetParent() != null)
			{
				foreach (XNode xnode in n.NodesAfterSelf())
				{
					XText xtext = xnode as XText;
					if (xtext == null)
					{
						break;
					}
					text += xtext.Value;
				}
			}
			return text;
		}

		private static XmlNameTable CreateNameTable()
		{
			NameTable nameTable = new NameTable();
			nameTable.Add(string.Empty);
			nameTable.Add(XNodeNavigator.xmlnsPrefixNamespace);
			nameTable.Add(XNodeNavigator.xmlPrefixNamespace);
			return nameTable;
		}

		private static bool IsContent(XContainer c, XNode n)
		{
			return c.GetParent() != null || c is XElement || ((1 << (int)n.NodeType) & 386) != 0;
		}

		private static bool IsSamePosition(XNodeNavigator n1, XNodeNavigator n2)
		{
			return n1._source == n2._source && n1._source.GetParent() == n2._source.GetParent();
		}

		private static bool IsXmlNamespaceDeclaration(XAttribute a)
		{
			return a == XNodeNavigator.GetXmlNamespaceDeclaration();
		}

		private static int GetElementContentMask(XPathNodeType type)
		{
			return XNodeNavigator.s_ElementContentMasks[(int)type];
		}

		private static XAttribute GetFirstNamespaceDeclarationGlobal(XElement e)
		{
			XAttribute firstNamespaceDeclarationLocal;
			for (;;)
			{
				firstNamespaceDeclarationLocal = XNodeNavigator.GetFirstNamespaceDeclarationLocal(e);
				if (firstNamespaceDeclarationLocal != null)
				{
					break;
				}
				e = e.Parent;
				if (e == null)
				{
					goto Block_1;
				}
			}
			return firstNamespaceDeclarationLocal;
			Block_1:
			return null;
		}

		private static XAttribute GetFirstNamespaceDeclarationLocal(XElement e)
		{
			foreach (XAttribute xattribute in e.Attributes())
			{
				if (xattribute.IsNamespaceDeclaration)
				{
					return xattribute;
				}
			}
			return null;
		}

		private static XAttribute GetNextNamespaceDeclarationGlobal(XAttribute a)
		{
			XElement xelement = (XElement)a.GetParent();
			if (xelement == null)
			{
				return null;
			}
			XAttribute nextNamespaceDeclarationLocal = XNodeNavigator.GetNextNamespaceDeclarationLocal(a);
			if (nextNamespaceDeclarationLocal != null)
			{
				return nextNamespaceDeclarationLocal;
			}
			xelement = xelement.Parent;
			if (xelement == null)
			{
				return null;
			}
			return XNodeNavigator.GetFirstNamespaceDeclarationGlobal(xelement);
		}

		private static XAttribute GetNextNamespaceDeclarationLocal(XAttribute a)
		{
			if (a.Parent == null)
			{
				return null;
			}
			for (a = a.NextAttribute; a != null; a = a.NextAttribute)
			{
				if (a.IsNamespaceDeclaration)
				{
					return a;
				}
			}
			return null;
		}

		private static XAttribute GetXmlNamespaceDeclaration()
		{
			if (XNodeNavigator.s_XmlNamespaceDeclaration == null)
			{
				Interlocked.CompareExchange<XAttribute>(ref XNodeNavigator.s_XmlNamespaceDeclaration, new XAttribute(XNamespace.Xmlns.GetName("xml"), XNodeNavigator.xmlPrefixNamespace), null);
			}
			return XNodeNavigator.s_XmlNamespaceDeclaration;
		}

		private static bool HasNamespaceDeclarationInScope(XAttribute a, XElement e)
		{
			XName name = a.Name;
			while (e != null && e != a.GetParent())
			{
				if (e.Attribute(name) != null)
				{
					return true;
				}
				e = e.Parent;
			}
			return false;
		}

		internal static readonly string xmlPrefixNamespace = XNamespace.Xml.NamespaceName;

		internal static readonly string xmlnsPrefixNamespace = XNamespace.Xmlns.NamespaceName;

		private const int DocumentContentMask = 386;

		private static readonly int[] s_ElementContentMasks = new int[] { 0, 2, 0, 0, 24, 0, 0, 128, 256, 410 };

		private new const int TextMask = 24;

		private static XAttribute s_XmlNamespaceDeclaration;

		private XObject _source;

		private XElement _parent;

		private XmlNameTable _nameTable;
	}
}
