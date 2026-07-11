using System;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Schema;

namespace System.Xml.XPath
{
	internal class XNodeNavigator : XPathNavigator, IXmlLineInfo
	{
		public XNodeNavigator(XNode node, XmlNameTable nameTable)
		{
			this.source = node;
			this.nameTable = ((nameTable != null) ? nameTable : XNodeNavigator.CreateNameTable());
		}

		public XNodeNavigator(XNodeNavigator other)
		{
			this.source = other.source;
			this.parent = other.parent;
			this.nameTable = other.nameTable;
		}

		public override string BaseURI
		{
			get
			{
				XObject xobject = this.source as XObject;
				if (xobject != null)
				{
					return xobject.BaseUri;
				}
				if (this.parent != null)
				{
					return this.parent.BaseUri;
				}
				return string.Empty;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				XElement xelement = this.source as XElement;
				if (xelement != null)
				{
					XAttribute xattribute = xelement.lastAttr;
					if (xattribute != null)
					{
						for (;;)
						{
							xattribute = xattribute.next;
							if (!xattribute.IsNamespaceDeclaration)
							{
								break;
							}
							if (xattribute == xelement.lastAttr)
							{
								return false;
							}
						}
						return true;
					}
				}
				return false;
			}
		}

		public override bool HasChildren
		{
			get
			{
				XContainer xcontainer = this.source as XContainer;
				if (xcontainer != null && xcontainer.content != null)
				{
					XNode xnode = xcontainer.content as XNode;
					if (xnode != null)
					{
						for (;;)
						{
							xnode = xnode.next;
							if (XNodeNavigator.IsContent(xcontainer, xnode))
							{
								break;
							}
							if (xnode == xcontainer.content)
							{
								return false;
							}
						}
						return true;
					}
					if (((string)xcontainer.content).Length != 0 && (xcontainer.parent != null || xcontainer is XElement))
					{
						return true;
					}
				}
				return false;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				XElement xelement = this.source as XElement;
				return xelement != null && xelement.IsEmpty;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.nameTable.Add(this.GetLocalName());
			}
		}

		private string GetLocalName()
		{
			XElement xelement = this.source as XElement;
			if (xelement != null)
			{
				return xelement.Name.LocalName;
			}
			XAttribute xattribute = this.source as XAttribute;
			if (xattribute != null)
			{
				if (this.parent != null && xattribute.Name.NamespaceName.Length == 0)
				{
					return string.Empty;
				}
				return xattribute.Name.LocalName;
			}
			else
			{
				XProcessingInstruction xprocessingInstruction = this.source as XProcessingInstruction;
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
					return this.nameTable.Add(this.GetLocalName());
				}
				return this.nameTable.Add(prefix + ":" + this.GetLocalName());
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.nameTable.Add(this.GetNamespaceURI());
			}
		}

		private string GetNamespaceURI()
		{
			XElement xelement = this.source as XElement;
			if (xelement != null)
			{
				return xelement.Name.NamespaceName;
			}
			XAttribute xattribute = this.source as XAttribute;
			if (xattribute == null)
			{
				return string.Empty;
			}
			if (this.parent != null)
			{
				return string.Empty;
			}
			return xattribute.Name.NamespaceName;
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		public override XPathNodeType NodeType
		{
			get
			{
				XObject xobject = this.source as XObject;
				if (xobject != null)
				{
					switch (xobject.NodeType)
					{
					case XmlNodeType.Element:
						return XPathNodeType.Element;
					case XmlNodeType.Attribute:
						if (this.parent != null)
						{
							return XPathNodeType.Namespace;
						}
						return XPathNodeType.Attribute;
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
				return this.nameTable.Add(this.GetPrefix());
			}
		}

		private string GetPrefix()
		{
			XElement xelement = this.source as XElement;
			if (xelement == null)
			{
				XAttribute xattribute = this.source as XAttribute;
				if (xattribute != null)
				{
					if (this.parent != null)
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
				if (this.source is string)
				{
					this.source = this.parent.LastNode;
					this.parent = null;
				}
				return this.source;
			}
		}

		public override string Value
		{
			get
			{
				XObject xobject = this.source as XObject;
				if (xobject != null)
				{
					switch (xobject.NodeType)
					{
					case XmlNodeType.Element:
						return ((XElement)xobject).Value;
					case XmlNodeType.Attribute:
						return ((XAttribute)xobject).Value;
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						return XNodeNavigator.CollectText((XText)xobject);
					case XmlNodeType.ProcessingInstruction:
						return ((XProcessingInstruction)xobject).Data;
					case XmlNodeType.Comment:
						return ((XComment)xobject).Value;
					case XmlNodeType.Document:
					{
						XElement root = ((XDocument)xobject).Root;
						if (root == null)
						{
							return string.Empty;
						}
						return root.Value;
					}
					}
					return string.Empty;
				}
				return (string)this.source;
			}
		}

		public override bool CheckValidity(XmlSchemaSet schemas, ValidationEventHandler validationEventHandler)
		{
			throw new NotSupportedException(Res.GetString("NotSupported_CheckValidity"));
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
				this.source = xnodeNavigator.source;
				this.parent = xnodeNavigator.parent;
				return true;
			}
			return false;
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			XElement xelement = this.source as XElement;
			if (xelement != null)
			{
				XAttribute xattribute = xelement.lastAttr;
				if (xattribute != null)
				{
					for (;;)
					{
						xattribute = xattribute.next;
						if (xattribute.Name.LocalName == localName && xattribute.Name.NamespaceName == namespaceName && !xattribute.IsNamespaceDeclaration)
						{
							break;
						}
						if (xattribute == xelement.lastAttr)
						{
							return false;
						}
					}
					this.source = xattribute;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToChild(string localName, string namespaceName)
		{
			XContainer xcontainer = this.source as XContainer;
			if (xcontainer != null && xcontainer.content != null)
			{
				XNode xnode = xcontainer.content as XNode;
				if (xnode != null)
				{
					XElement xelement;
					for (;;)
					{
						xnode = xnode.next;
						xelement = xnode as XElement;
						if (xelement != null && xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName)
						{
							break;
						}
						if (xnode == xcontainer.content)
						{
							return false;
						}
					}
					this.source = xelement;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToChild(XPathNodeType type)
		{
			XContainer xcontainer = this.source as XContainer;
			if (xcontainer != null && xcontainer.content != null)
			{
				XNode xnode = xcontainer.content as XNode;
				if (xnode != null)
				{
					int num = XNodeNavigator.GetElementContentMask(type);
					if ((24 & num) != 0 && xcontainer.parent == null && xcontainer is XDocument)
					{
						num &= -25;
					}
					for (;;)
					{
						xnode = xnode.next;
						if (((1 << (int)xnode.NodeType) & num) != 0)
						{
							break;
						}
						if (xnode == xcontainer.content)
						{
							return false;
						}
					}
					this.source = xnode;
					return true;
				}
				string text = (string)xcontainer.content;
				if (text.Length != 0)
				{
					int elementContentMask = XNodeNavigator.GetElementContentMask(type);
					if ((24 & elementContentMask) != 0 && xcontainer.parent == null && xcontainer is XDocument)
					{
						return false;
					}
					if ((8 & elementContentMask) != 0)
					{
						this.source = text;
						this.parent = (XElement)xcontainer;
						return true;
					}
				}
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			XElement xelement = this.source as XElement;
			if (xelement != null)
			{
				XAttribute xattribute = xelement.lastAttr;
				if (xattribute != null)
				{
					for (;;)
					{
						xattribute = xattribute.next;
						if (!xattribute.IsNamespaceDeclaration)
						{
							break;
						}
						if (xattribute == xelement.lastAttr)
						{
							return false;
						}
					}
					this.source = xattribute;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToFirstChild()
		{
			XContainer xcontainer = this.source as XContainer;
			if (xcontainer != null && xcontainer.content != null)
			{
				XNode xnode = xcontainer.content as XNode;
				if (xnode != null)
				{
					for (;;)
					{
						xnode = xnode.next;
						if (XNodeNavigator.IsContent(xcontainer, xnode))
						{
							break;
						}
						if (xnode == xcontainer.content)
						{
							return false;
						}
					}
					this.source = xnode;
					return true;
				}
				string text = (string)xcontainer.content;
				if (text.Length != 0 && (xcontainer.parent != null || xcontainer is XElement))
				{
					this.source = text;
					this.parent = (XElement)xcontainer;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
		{
			XElement xelement = this.source as XElement;
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
					this.source = xattribute;
					this.parent = xelement;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToId(string id)
		{
			throw new NotSupportedException(Res.GetString("NotSupported_MoveToId"));
		}

		public override bool MoveToNamespace(string localName)
		{
			XElement xelement = this.source as XElement;
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
						this.source = xattribute;
						this.parent = xelement;
						return true;
					}
				}
				if (localName == "xml")
				{
					this.source = XNodeNavigator.GetXmlNamespaceDeclaration();
					this.parent = xelement;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToNext()
		{
			XNode xnode = this.source as XNode;
			if (xnode != null)
			{
				XContainer xcontainer = xnode.parent;
				if (xcontainer != null && xnode != xcontainer.content)
				{
					XNode next;
					for (;;)
					{
						next = xnode.next;
						if (XNodeNavigator.IsContent(xcontainer, next) && (!(xnode is XText) || !(next is XText)))
						{
							break;
						}
						xnode = next;
						if (xnode == xcontainer.content)
						{
							return false;
						}
					}
					this.source = next;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToNext(string localName, string namespaceName)
		{
			XNode xnode = this.source as XNode;
			if (xnode != null)
			{
				XContainer xcontainer = xnode.parent;
				if (xcontainer != null && xnode != xcontainer.content)
				{
					XElement xelement;
					for (;;)
					{
						xnode = xnode.next;
						xelement = xnode as XElement;
						if (xelement != null && xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName)
						{
							break;
						}
						if (xnode == xcontainer.content)
						{
							return false;
						}
					}
					this.source = xelement;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToNext(XPathNodeType type)
		{
			XNode xnode = this.source as XNode;
			if (xnode != null)
			{
				XContainer xcontainer = xnode.parent;
				if (xcontainer != null && xnode != xcontainer.content)
				{
					int num = XNodeNavigator.GetElementContentMask(type);
					if ((24 & num) != 0 && xcontainer.parent == null && xcontainer is XDocument)
					{
						num &= -25;
					}
					XNode next;
					for (;;)
					{
						next = xnode.next;
						if (((1 << (int)next.NodeType) & num) != 0 && (!(xnode is XText) || !(next is XText)))
						{
							break;
						}
						xnode = next;
						if (xnode == xcontainer.content)
						{
							return false;
						}
					}
					this.source = next;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			XAttribute xattribute = this.source as XAttribute;
			if (xattribute != null && this.parent == null)
			{
				XElement xelement = (XElement)xattribute.parent;
				if (xelement != null)
				{
					while (xattribute != xelement.lastAttr)
					{
						xattribute = xattribute.next;
						if (!xattribute.IsNamespaceDeclaration)
						{
							this.source = xattribute;
							return true;
						}
					}
				}
			}
			return false;
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope scope)
		{
			XAttribute xattribute = this.source as XAttribute;
			if (xattribute != null && this.parent != null && !XNodeNavigator.IsXmlNamespaceDeclaration(xattribute))
			{
				switch (scope)
				{
				case XPathNamespaceScope.All:
					do
					{
						xattribute = XNodeNavigator.GetNextNamespaceDeclarationGlobal(xattribute);
					}
					while (xattribute != null && XNodeNavigator.HasNamespaceDeclarationInScope(xattribute, this.parent));
					if (xattribute == null && !XNodeNavigator.HasNamespaceDeclarationInScope(XNodeNavigator.GetXmlNamespaceDeclaration(), this.parent))
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
					while (xattribute.Name.LocalName == "xml" || XNodeNavigator.HasNamespaceDeclarationInScope(xattribute, this.parent));
					break;
				case XPathNamespaceScope.Local:
					if (xattribute.parent != this.parent)
					{
						return false;
					}
					xattribute = XNodeNavigator.GetNextNamespaceDeclarationLocal(xattribute);
					break;
				}
				if (xattribute != null)
				{
					this.source = xattribute;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToParent()
		{
			if (this.parent != null)
			{
				this.source = this.parent;
				this.parent = null;
				return true;
			}
			XObject xobject = (XObject)this.source;
			if (xobject.parent != null)
			{
				this.source = xobject.parent;
				return true;
			}
			return false;
		}

		public override bool MoveToPrevious()
		{
			XNode xnode = this.source as XNode;
			if (xnode != null)
			{
				XContainer xcontainer = xnode.parent;
				if (xcontainer != null)
				{
					XNode xnode2 = (XNode)xcontainer.content;
					if (xnode2.next != xnode)
					{
						XNode xnode3 = null;
						do
						{
							xnode2 = xnode2.next;
							if (XNodeNavigator.IsContent(xcontainer, xnode2))
							{
								xnode3 = ((xnode3 is XText && xnode2 is XText) ? xnode3 : xnode2);
							}
						}
						while (xnode2.next != xnode);
						if (xnode3 != null)
						{
							this.source = xnode3;
							return true;
						}
					}
				}
			}
			return false;
		}

		public override XmlReader ReadSubtree()
		{
			XContainer xcontainer = this.source as XContainer;
			if (xcontainer == null)
			{
				throw new InvalidOperationException(Res.GetString("InvalidOperation_BadNodeType", new object[] { this.NodeType }));
			}
			return new XNodeReader(xcontainer, this.nameTable);
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			IXmlLineInfo xmlLineInfo = this.source as IXmlLineInfo;
			return xmlLineInfo != null && xmlLineInfo.HasLineInfo();
		}

		int IXmlLineInfo.LineNumber
		{
			get
			{
				IXmlLineInfo xmlLineInfo = this.source as IXmlLineInfo;
				if (xmlLineInfo != null)
				{
					return xmlLineInfo.LineNumber;
				}
				return 0;
			}
		}

		int IXmlLineInfo.LinePosition
		{
			get
			{
				IXmlLineInfo xmlLineInfo = this.source as IXmlLineInfo;
				if (xmlLineInfo != null)
				{
					return xmlLineInfo.LinePosition;
				}
				return 0;
			}
		}

		private static string CollectText(XText n)
		{
			string text = n.Value;
			if (n.parent != null)
			{
				while (n != n.parent.content)
				{
					n = n.next as XText;
					if (n == null)
					{
						break;
					}
					text += n.Value;
				}
			}
			return text;
		}

		private static XmlNameTable CreateNameTable()
		{
			NameTable nameTable = new NameTable();
			nameTable.Add(string.Empty);
			nameTable.Add("http://www.w3.org/2000/xmlns/");
			nameTable.Add("http://www.w3.org/XML/1998/namespace");
			return nameTable;
		}

		private static bool IsContent(XContainer c, XNode n)
		{
			return c.parent != null || c is XElement || ((1 << (int)n.NodeType) & 386) != 0;
		}

		private static bool IsSamePosition(XNodeNavigator n1, XNodeNavigator n2)
		{
			if (n1.source == n2.source && n1.parent == n2.parent)
			{
				return true;
			}
			if ((n1.parent != null) ^ (n2.parent != null))
			{
				XText xtext = n1.source as XText;
				if (xtext != null)
				{
					return xtext.Value == n2.source && xtext.parent == n2.parent;
				}
				XText xtext2 = n2.source as XText;
				if (xtext2 != null)
				{
					return xtext2.Value == n1.source && xtext2.parent == n1.parent;
				}
			}
			return false;
		}

		private static bool IsXmlNamespaceDeclaration(XAttribute a)
		{
			return a == XNodeNavigator.GetXmlNamespaceDeclaration();
		}

		private static int GetElementContentMask(XPathNodeType type)
		{
			return XNodeNavigator.ElementContentMasks[(int)type];
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
				e = e.parent as XElement;
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
			XAttribute xattribute = e.lastAttr;
			if (xattribute != null)
			{
				for (;;)
				{
					xattribute = xattribute.next;
					if (xattribute.IsNamespaceDeclaration)
					{
						break;
					}
					if (xattribute == e.lastAttr)
					{
						goto IL_0024;
					}
				}
				return xattribute;
			}
			IL_0024:
			return null;
		}

		private static XAttribute GetNextNamespaceDeclarationGlobal(XAttribute a)
		{
			XElement xelement = (XElement)a.parent;
			if (xelement == null)
			{
				return null;
			}
			XAttribute nextNamespaceDeclarationLocal = XNodeNavigator.GetNextNamespaceDeclarationLocal(a);
			if (nextNamespaceDeclarationLocal != null)
			{
				return nextNamespaceDeclarationLocal;
			}
			xelement = xelement.parent as XElement;
			if (xelement == null)
			{
				return null;
			}
			return XNodeNavigator.GetFirstNamespaceDeclarationGlobal(xelement);
		}

		private static XAttribute GetNextNamespaceDeclarationLocal(XAttribute a)
		{
			XElement xelement = (XElement)a.parent;
			if (xelement == null)
			{
				return null;
			}
			while (a != xelement.lastAttr)
			{
				a = a.next;
				if (a.IsNamespaceDeclaration)
				{
					return a;
				}
			}
			return null;
		}

		private static XAttribute GetXmlNamespaceDeclaration()
		{
			if (XNodeNavigator.XmlNamespaceDeclaration == null)
			{
				Interlocked.CompareExchange<XAttribute>(ref XNodeNavigator.XmlNamespaceDeclaration, new XAttribute(XNamespace.Xmlns.GetName("xml"), "http://www.w3.org/XML/1998/namespace"), null);
			}
			return XNodeNavigator.XmlNamespaceDeclaration;
		}

		private static bool HasNamespaceDeclarationInScope(XAttribute a, XElement e)
		{
			XName name = a.Name;
			while (e != null && e != a.parent)
			{
				if (e.Attribute(name) != null)
				{
					return true;
				}
				e = e.parent as XElement;
			}
			return false;
		}

		private const int DocumentContentMask = 386;

		private static readonly int[] ElementContentMasks = new int[] { 0, 2, 0, 0, 24, 0, 0, 128, 256, 410 };

		private new const int TextMask = 24;

		private static XAttribute XmlNamespaceDeclaration;

		private object source;

		private XElement parent;

		private XmlNameTable nameTable;
	}
}
