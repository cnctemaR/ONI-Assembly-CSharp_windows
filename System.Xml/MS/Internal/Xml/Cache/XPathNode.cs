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
				return (XPathNodeType)(this._props & 15U);
			}
		}

		public string Prefix
		{
			get
			{
				return this._info.Prefix;
			}
		}

		public string LocalName
		{
			get
			{
				return this._info.LocalName;
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
				return this._info.NamespaceUri;
			}
		}

		public XPathDocument Document
		{
			get
			{
				return this._info.Document;
			}
		}

		public string BaseUri
		{
			get
			{
				return this._info.BaseUri;
			}
		}

		public int LineNumber
		{
			get
			{
				return this._info.LineNumberBase + (int)((this._props & 16776192U) >> 10);
			}
		}

		public int LinePosition
		{
			get
			{
				return this._info.LinePositionBase + (int)this._posOffset;
			}
		}

		public int CollapsedLinePosition
		{
			get
			{
				return this.LinePosition + (int)(this._props >> 24);
			}
		}

		public XPathNodePageInfo PageInfo
		{
			get
			{
				return this._info.PageInfo;
			}
		}

		public int GetRoot(out XPathNode[] pageNode)
		{
			return this._info.Document.GetRootNode(out pageNode);
		}

		public int GetParent(out XPathNode[] pageNode)
		{
			pageNode = this._info.ParentPage;
			return (int)this._idxParent;
		}

		public int GetSibling(out XPathNode[] pageNode)
		{
			pageNode = this._info.SiblingPage;
			return (int)this._idxSibling;
		}

		public int GetSimilarElement(out XPathNode[] pageNode)
		{
			pageNode = this._info.SimilarElementPage;
			return (int)this._idxSimilar;
		}

		public bool NameMatch(string localName, string namespaceName)
		{
			return this._info.LocalName == localName && this._info.NamespaceUri == namespaceName;
		}

		public bool ElementMatch(string localName, string namespaceName)
		{
			return this.NodeType == XPathNodeType.Element && this._info.LocalName == localName && this._info.NamespaceUri == namespaceName;
		}

		public bool IsXmlNamespaceNode
		{
			get
			{
				string localName = this._info.LocalName;
				return this.NodeType == XPathNodeType.Namespace && localName.Length == 3 && localName == "xml";
			}
		}

		public bool HasSibling
		{
			get
			{
				return this._idxSibling > 0;
			}
		}

		public bool HasCollapsedText
		{
			get
			{
				return (this._props & 128U) > 0U;
			}
		}

		public bool HasAttribute
		{
			get
			{
				return (this._props & 16U) > 0U;
			}
		}

		public bool HasContentChild
		{
			get
			{
				return (this._props & 32U) > 0U;
			}
		}

		public bool HasElementChild
		{
			get
			{
				return (this._props & 64U) > 0U;
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
				return (this._props & 512U) > 0U;
			}
			set
			{
				if (value)
				{
					this._props |= 512U;
					return;
				}
				this._props &= 255U;
			}
		}

		public bool AllowShortcutTag
		{
			get
			{
				return (this._props & 256U) > 0U;
			}
		}

		public int LocalNameHashCode
		{
			get
			{
				return this._info.LocalNameHashCode;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
		}

		public void Create(XPathNodePageInfo pageInfo)
		{
			this._info = new XPathNodeInfoAtom(pageInfo);
		}

		public void Create(XPathNodeInfoAtom info, XPathNodeType xptyp, int idxParent)
		{
			this._info = info;
			this._props = (uint)xptyp;
			this._idxParent = (ushort)idxParent;
		}

		public void SetLineInfoOffsets(int lineNumOffset, int linePosOffset)
		{
			this._props |= (uint)((uint)lineNumOffset << 10);
			this._posOffset = (ushort)linePosOffset;
		}

		public void SetCollapsedLineInfoOffset(int posOffset)
		{
			this._props |= (uint)((uint)posOffset << 24);
		}

		public void SetValue(string value)
		{
			this._value = value;
		}

		public void SetEmptyValue(bool allowShortcutTag)
		{
			this._value = string.Empty;
			if (allowShortcutTag)
			{
				this._props |= 256U;
			}
		}

		public void SetCollapsedValue(string value)
		{
			this._value = value;
			this._props |= 160U;
		}

		public void SetParentProperties(XPathNodeType xptyp)
		{
			if (xptyp == XPathNodeType.Attribute)
			{
				this._props |= 16U;
				return;
			}
			this._props |= 32U;
			if (xptyp == XPathNodeType.Element)
			{
				this._props |= 64U;
			}
		}

		public void SetSibling(XPathNodeInfoTable infoTable, XPathNode[] pageSibling, int idxSibling)
		{
			this._idxSibling = (ushort)idxSibling;
			if (pageSibling != this._info.SiblingPage)
			{
				this._info = infoTable.Create(this._info.LocalName, this._info.NamespaceUri, this._info.Prefix, this._info.BaseUri, this._info.ParentPage, pageSibling, this._info.SimilarElementPage, this._info.Document, this._info.LineNumberBase, this._info.LinePositionBase);
			}
		}

		public void SetSimilarElement(XPathNodeInfoTable infoTable, XPathNode[] pageSimilar, int idxSimilar)
		{
			this._idxSimilar = (ushort)idxSimilar;
			if (pageSimilar != this._info.SimilarElementPage)
			{
				this._info = infoTable.Create(this._info.LocalName, this._info.NamespaceUri, this._info.Prefix, this._info.BaseUri, this._info.ParentPage, this._info.SiblingPage, pageSimilar, this._info.Document, this._info.LineNumberBase, this._info.LinePositionBase);
			}
		}

		private XPathNodeInfoAtom _info;

		private ushort _idxSibling;

		private ushort _idxParent;

		private ushort _idxSimilar;

		private ushort _posOffset;

		private uint _props;

		private string _value;

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
