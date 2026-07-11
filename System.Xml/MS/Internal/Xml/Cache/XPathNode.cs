using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal struct XPathNode
	{
		public XPathNodeType NodeType
		{
			get
			{
				return (XPathNodeType)(this.props & 15U);
			}
		}

		public string Prefix
		{
			get
			{
				return this.info.Prefix;
			}
		}

		public string LocalName
		{
			get
			{
				return this.info.LocalName;
			}
		}

		public string Name
		{
			get
			{
				if (this.Prefix.Length == 0)
				{
					return this.LocalName;
				}
				return this.Prefix + ":" + this.LocalName;
			}
		}

		public string NamespaceUri
		{
			get
			{
				return this.info.NamespaceUri;
			}
		}

		public XPathDocument Document
		{
			get
			{
				return this.info.Document;
			}
		}

		public string BaseUri
		{
			get
			{
				return this.info.BaseUri;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.info.LineNumberBase + (int)((this.props & 16776192U) >> 10);
			}
		}

		public int LinePosition
		{
			get
			{
				return this.info.LinePositionBase + (int)this.posOffset;
			}
		}

		public int CollapsedLinePosition
		{
			get
			{
				return this.LinePosition + (int)(this.props >> 24);
			}
		}

		public XPathNodePageInfo PageInfo
		{
			get
			{
				return this.info.PageInfo;
			}
		}

		public int GetRoot(out XPathNode[] pageNode)
		{
			return this.info.Document.GetRootNode(out pageNode);
		}

		public int GetParent(out XPathNode[] pageNode)
		{
			pageNode = this.info.ParentPage;
			return (int)this.idxParent;
		}

		public int GetSibling(out XPathNode[] pageNode)
		{
			pageNode = this.info.SiblingPage;
			return (int)this.idxSibling;
		}

		public int GetSimilarElement(out XPathNode[] pageNode)
		{
			pageNode = this.info.SimilarElementPage;
			return (int)this.idxSimilar;
		}

		public bool NameMatch(string localName, string namespaceName)
		{
			return this.info.LocalName == localName && this.info.NamespaceUri == namespaceName;
		}

		public bool ElementMatch(string localName, string namespaceName)
		{
			return this.NodeType == XPathNodeType.Element && this.info.LocalName == localName && this.info.NamespaceUri == namespaceName;
		}

		public bool IsXmlNamespaceNode
		{
			get
			{
				string localName = this.info.LocalName;
				return this.NodeType == XPathNodeType.Namespace && localName.Length == 3 && localName == "xml";
			}
		}

		public bool HasSibling
		{
			get
			{
				return this.idxSibling > 0;
			}
		}

		public bool HasCollapsedText
		{
			get
			{
				return (this.props & 128U) > 0U;
			}
		}

		public bool HasAttribute
		{
			get
			{
				return (this.props & 16U) > 0U;
			}
		}

		public bool HasContentChild
		{
			get
			{
				return (this.props & 32U) > 0U;
			}
		}

		public bool HasElementChild
		{
			get
			{
				return (this.props & 64U) > 0U;
			}
		}

		public bool IsAttrNmsp
		{
			get
			{
				XPathNodeType nodeType = this.NodeType;
				return nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace;
			}
		}

		public bool IsText
		{
			get
			{
				return XPathNavigator.IsText(this.NodeType);
			}
		}

		public bool HasNamespaceDecls
		{
			get
			{
				return (this.props & 512U) > 0U;
			}
			set
			{
				if (value)
				{
					this.props |= 512U;
					return;
				}
				this.props &= 255U;
			}
		}

		public bool AllowShortcutTag
		{
			get
			{
				return (this.props & 256U) > 0U;
			}
		}

		public int LocalNameHashCode
		{
			get
			{
				return this.info.LocalNameHashCode;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public void Create(XPathNodePageInfo pageInfo)
		{
			this.info = new XPathNodeInfoAtom(pageInfo);
		}

		public void Create(XPathNodeInfoAtom info, XPathNodeType xptyp, int idxParent)
		{
			this.info = info;
			this.props = (uint)xptyp;
			this.idxParent = (ushort)idxParent;
		}

		public void SetLineInfoOffsets(int lineNumOffset, int linePosOffset)
		{
			this.props |= (uint)((uint)lineNumOffset << 10);
			this.posOffset = (ushort)linePosOffset;
		}

		public void SetCollapsedLineInfoOffset(int posOffset)
		{
			this.props |= (uint)((uint)posOffset << 24);
		}

		public void SetValue(string value)
		{
			this.value = value;
		}

		public void SetEmptyValue(bool allowShortcutTag)
		{
			this.value = string.Empty;
			if (allowShortcutTag)
			{
				this.props |= 256U;
			}
		}

		public void SetCollapsedValue(string value)
		{
			this.value = value;
			this.props |= 160U;
		}

		public void SetParentProperties(XPathNodeType xptyp)
		{
			if (xptyp == XPathNodeType.Attribute)
			{
				this.props |= 16U;
				return;
			}
			this.props |= 32U;
			if (xptyp == XPathNodeType.Element)
			{
				this.props |= 64U;
			}
		}

		public void SetSibling(XPathNodeInfoTable infoTable, XPathNode[] pageSibling, int idxSibling)
		{
			this.idxSibling = (ushort)idxSibling;
			if (pageSibling != this.info.SiblingPage)
			{
				this.info = infoTable.Create(this.info.LocalName, this.info.NamespaceUri, this.info.Prefix, this.info.BaseUri, this.info.ParentPage, pageSibling, this.info.SimilarElementPage, this.info.Document, this.info.LineNumberBase, this.info.LinePositionBase);
			}
		}

		public void SetSimilarElement(XPathNodeInfoTable infoTable, XPathNode[] pageSimilar, int idxSimilar)
		{
			this.idxSimilar = (ushort)idxSimilar;
			if (pageSimilar != this.info.SimilarElementPage)
			{
				this.info = infoTable.Create(this.info.LocalName, this.info.NamespaceUri, this.info.Prefix, this.info.BaseUri, this.info.ParentPage, this.info.SiblingPage, pageSimilar, this.info.Document, this.info.LineNumberBase, this.info.LinePositionBase);
			}
		}

		private XPathNodeInfoAtom info;

		private ushort idxSibling;

		private ushort idxParent;

		private ushort idxSimilar;

		private ushort posOffset;

		private uint props;

		private string value;

		private const uint NodeTypeMask = 15U;

		private const uint HasAttributeBit = 16U;

		private const uint HasContentChildBit = 32U;

		private const uint HasElementChildBit = 64U;

		private const uint HasCollapsedTextBit = 128U;

		private const uint AllowShortcutTagBit = 256U;

		private const uint HasNmspDeclsBit = 512U;

		private const uint LineNumberMask = 16776192U;

		private const int LineNumberShift = 10;

		private const int CollapsedPositionShift = 24;

		public const int MaxLineNumberOffset = 16383;

		public const int MaxLinePositionOffset = 65535;

		public const int MaxCollapsedPositionOffset = 255;
	}
}
