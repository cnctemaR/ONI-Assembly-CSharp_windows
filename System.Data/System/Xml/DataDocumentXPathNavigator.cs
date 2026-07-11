using System;
using System.Xml.XPath;

namespace System.Xml
{
	internal sealed class DataDocumentXPathNavigator : XPathNavigator, IHasXmlNode
	{
		internal DataDocumentXPathNavigator(XmlDataDocument doc, XmlNode node)
		{
			this._curNode = new XPathNodePointer(this, doc, node);
			this._temp = new XPathNodePointer(this, doc, node);
			this._doc = doc;
		}

		private DataDocumentXPathNavigator(DataDocumentXPathNavigator other)
		{
			this._curNode = other._curNode.Clone(this);
			this._temp = other._temp.Clone(this);
			this._doc = other._doc;
		}

		public override XPathNavigator Clone()
		{
			return new DataDocumentXPathNavigator(this);
		}

		internal XPathNodePointer CurNode
		{
			get
			{
				return this._curNode;
			}
		}

		internal XmlDataDocument Document
		{
			get
			{
				return this._doc;
			}
		}

		public override XPathNodeType NodeType
		{
			get
			{
				return this._curNode.NodeType;
			}
		}

		public override string LocalName
		{
			get
			{
				return this._curNode.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this._curNode.NamespaceURI;
			}
		}

		public override string Name
		{
			get
			{
				return this._curNode.Name;
			}
		}

		public override string Prefix
		{
			get
			{
				return this._curNode.Prefix;
			}
		}

		public override string Value
		{
			get
			{
				XPathNodeType nodeType = this._curNode.NodeType;
				if (nodeType != XPathNodeType.Element && nodeType != XPathNodeType.Root)
				{
					return this._curNode.Value;
				}
				return this._curNode.InnerText;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this._curNode.BaseURI;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this._curNode.XmlLang;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this._curNode.IsEmptyElement;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this._doc.NameTable;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				return this._curNode.AttributeCount > 0;
			}
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			if (this._curNode.NodeType != XPathNodeType.Element)
			{
				return string.Empty;
			}
			this._temp.MoveTo(this._curNode);
			if (!this._temp.MoveToAttribute(localName, namespaceURI))
			{
				return string.Empty;
			}
			return this._temp.Value;
		}

		public override string GetNamespace(string name)
		{
			return this._curNode.GetNamespace(name);
		}

		public override bool MoveToNamespace(string name)
		{
			return this._curNode.NodeType == XPathNodeType.Element && this._curNode.MoveToNamespace(name);
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
		{
			return this._curNode.NodeType == XPathNodeType.Element && this._curNode.MoveToFirstNamespace(namespaceScope);
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
		{
			return this._curNode.NodeType == XPathNodeType.Namespace && this._curNode.MoveToNextNamespace(namespaceScope);
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			return this._curNode.NodeType == XPathNodeType.Element && this._curNode.MoveToAttribute(localName, namespaceURI);
		}

		public override bool MoveToFirstAttribute()
		{
			return this._curNode.NodeType == XPathNodeType.Element && this._curNode.MoveToNextAttribute(true);
		}

		public override bool MoveToNextAttribute()
		{
			return this._curNode.NodeType == XPathNodeType.Attribute && this._curNode.MoveToNextAttribute(false);
		}

		public override bool MoveToNext()
		{
			return this._curNode.NodeType != XPathNodeType.Attribute && this._curNode.MoveToNextSibling();
		}

		public override bool MoveToPrevious()
		{
			return this._curNode.NodeType != XPathNodeType.Attribute && this._curNode.MoveToPreviousSibling();
		}

		public override bool MoveToFirst()
		{
			return this._curNode.NodeType != XPathNodeType.Attribute && this._curNode.MoveToFirst();
		}

		public override bool HasChildren
		{
			get
			{
				return this._curNode.HasChildren;
			}
		}

		public override bool MoveToFirstChild()
		{
			return this._curNode.MoveToFirstChild();
		}

		public override bool MoveToParent()
		{
			return this._curNode.MoveToParent();
		}

		public override void MoveToRoot()
		{
			this._curNode.MoveToRoot();
		}

		public override bool MoveTo(XPathNavigator other)
		{
			if (other != null)
			{
				DataDocumentXPathNavigator dataDocumentXPathNavigator = other as DataDocumentXPathNavigator;
				if (dataDocumentXPathNavigator != null && this._curNode.MoveTo(dataDocumentXPathNavigator.CurNode))
				{
					this._doc = this._curNode.Document;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToId(string id)
		{
			return false;
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			if (other != null)
			{
				DataDocumentXPathNavigator dataDocumentXPathNavigator = other as DataDocumentXPathNavigator;
				if (dataDocumentXPathNavigator != null && this._doc == dataDocumentXPathNavigator.Document && this._curNode.IsSamePosition(dataDocumentXPathNavigator.CurNode))
				{
					return true;
				}
			}
			return false;
		}

		XmlNode IHasXmlNode.GetNode()
		{
			return this._curNode.Node;
		}

		public override XmlNodeOrder ComparePosition(XPathNavigator other)
		{
			if (other == null)
			{
				return XmlNodeOrder.Unknown;
			}
			DataDocumentXPathNavigator dataDocumentXPathNavigator = other as DataDocumentXPathNavigator;
			if (dataDocumentXPathNavigator != null && dataDocumentXPathNavigator.Document == this._doc)
			{
				return this._curNode.ComparePosition(dataDocumentXPathNavigator.CurNode);
			}
			return XmlNodeOrder.Unknown;
		}

		private readonly XPathNodePointer _curNode;

		private XmlDataDocument _doc;

		private readonly XPathNodePointer _temp;
	}
}
