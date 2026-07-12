using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal sealed class XPathDocumentNavigator : XPathNavigator, IXmlLineInfo
	{
		public XPathDocumentNavigator(XPathNode[] pageCurrent, int idxCurrent, XPathNode[] pageParent, int idxParent)
		{
			this._pageCurrent = pageCurrent;
			this._pageParent = pageParent;
			this._idxCurrent = idxCurrent;
			this._idxParent = idxParent;
		}

		public XPathDocumentNavigator(XPathDocumentNavigator nav)
			: this(nav._pageCurrent, nav._idxCurrent, nav._pageParent, nav._idxParent)
		{
			this._atomizedLocalName = nav._atomizedLocalName;
		}

		public override string Value
		{
			get
			{
				string value = this._pageCurrent[this._idxCurrent].Value;
				if (value != null)
				{
					return value;
				}
				if (this._idxParent != 0)
				{
					return this._pageParent[this._idxParent].Value;
				}
				string text = string.Empty;
				StringBuilder stringBuilder = null;
				XPathNode[] pageCurrent;
				XPathNode[] array = (pageCurrent = this._pageCurrent);
				int idxCurrent;
				int num = (idxCurrent = this._idxCurrent);
				if (!XPathNodeHelper.GetNonDescendant(ref array, ref num))
				{
					array = null;
					num = 0;
				}
				while (XPathNodeHelper.GetTextFollowing(ref pageCurrent, ref idxCurrent, array, num))
				{
					if (text.Length == 0)
					{
						text = pageCurrent[idxCurrent].Value;
					}
					else
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder();
							stringBuilder.Append(text);
						}
						stringBuilder.Append(pageCurrent[idxCurrent].Value);
					}
				}
				if (stringBuilder == null)
				{
					return text;
				}
				return stringBuilder.ToString();
			}
		}

		public override XPathNavigator Clone()
		{
			return new XPathDocumentNavigator(this._pageCurrent, this._idxCurrent, this._pageParent, this._idxParent);
		}

		public override XPathNodeType NodeType
		{
			get
			{
				return this._pageCurrent[this._idxCurrent].NodeType;
			}
		}

		public override string LocalName
		{
			get
			{
				return this._pageCurrent[this._idxCurrent].LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this._pageCurrent[this._idxCurrent].NamespaceUri;
			}
		}

		public override string Name
		{
			get
			{
				return this._pageCurrent[this._idxCurrent].Name;
			}
		}

		public override string Prefix
		{
			get
			{
				return this._pageCurrent[this._idxCurrent].Prefix;
			}
		}

		public override string BaseURI
		{
			get
			{
				XPathNode[] array;
				int num;
				if (this._idxParent != 0)
				{
					array = this._pageParent;
					num = this._idxParent;
				}
				else
				{
					array = this._pageCurrent;
					num = this._idxCurrent;
				}
				for (;;)
				{
					XPathNodeType nodeType = array[num].NodeType;
					if (nodeType <= XPathNodeType.Element || nodeType == XPathNodeType.ProcessingInstruction)
					{
						break;
					}
					num = array[num].GetParent(out array);
					if (num == 0)
					{
						goto Block_3;
					}
				}
				return array[num].BaseUri;
				Block_3:
				return string.Empty;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this._pageCurrent[this._idxCurrent].AllowShortcutTag;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this._pageCurrent[this._idxCurrent].Document.NameTable;
			}
		}

		public override bool MoveToFirstAttribute()
		{
			XPathNode[] pageCurrent = this._pageCurrent;
			int idxCurrent = this._idxCurrent;
			if (XPathNodeHelper.GetFirstAttribute(ref this._pageCurrent, ref this._idxCurrent))
			{
				this._pageParent = pageCurrent;
				this._idxParent = idxCurrent;
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			return XPathNodeHelper.GetNextAttribute(ref this._pageCurrent, ref this._idxCurrent);
		}

		public override bool HasAttributes
		{
			get
			{
				return this._pageCurrent[this._idxCurrent].HasAttribute;
			}
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			XPathNode[] pageCurrent = this._pageCurrent;
			int idxCurrent = this._idxCurrent;
			if (localName != this._atomizedLocalName)
			{
				this._atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			if (XPathNodeHelper.GetAttribute(ref this._pageCurrent, ref this._idxCurrent, this._atomizedLocalName, namespaceURI))
			{
				this._pageParent = pageCurrent;
				this._idxParent = idxCurrent;
				return true;
			}
			return false;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
		{
			XPathNode[] array;
			int num;
			if (namespaceScope == XPathNamespaceScope.Local)
			{
				num = XPathNodeHelper.GetLocalNamespaces(this._pageCurrent, this._idxCurrent, out array);
			}
			else
			{
				num = XPathNodeHelper.GetInScopeNamespaces(this._pageCurrent, this._idxCurrent, out array);
			}
			while (num != 0)
			{
				if (namespaceScope != XPathNamespaceScope.ExcludeXml || !array[num].IsXmlNamespaceNode)
				{
					this._pageParent = this._pageCurrent;
					this._idxParent = this._idxCurrent;
					this._pageCurrent = array;
					this._idxCurrent = num;
					return true;
				}
				num = array[num].GetSibling(out array);
			}
			return false;
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope scope)
		{
			XPathNode[] pageCurrent = this._pageCurrent;
			int num = this._idxCurrent;
			if (pageCurrent[num].NodeType != XPathNodeType.Namespace)
			{
				return false;
			}
			for (;;)
			{
				num = pageCurrent[num].GetSibling(out pageCurrent);
				if (num == 0)
				{
					break;
				}
				if (scope != XPathNamespaceScope.ExcludeXml)
				{
					goto Block_3;
				}
				if (!pageCurrent[num].IsXmlNamespaceNode)
				{
					goto IL_006A;
				}
			}
			return false;
			Block_3:
			XPathNode[] array;
			if (scope == XPathNamespaceScope.Local && (pageCurrent[num].GetParent(out array) != this._idxParent || array != this._pageParent))
			{
				return false;
			}
			IL_006A:
			this._pageCurrent = pageCurrent;
			this._idxCurrent = num;
			return true;
		}

		public override bool MoveToNext()
		{
			return XPathNodeHelper.GetContentSibling(ref this._pageCurrent, ref this._idxCurrent);
		}

		public override bool MoveToPrevious()
		{
			return this._idxParent == 0 && XPathNodeHelper.GetPreviousContentSibling(ref this._pageCurrent, ref this._idxCurrent);
		}

		public override bool MoveToFirstChild()
		{
			if (this._pageCurrent[this._idxCurrent].HasCollapsedText)
			{
				this._pageParent = this._pageCurrent;
				this._idxParent = this._idxCurrent;
				this._idxCurrent = this._pageCurrent[this._idxCurrent].Document.GetCollapsedTextNode(out this._pageCurrent);
				return true;
			}
			return XPathNodeHelper.GetContentChild(ref this._pageCurrent, ref this._idxCurrent);
		}

		public override bool MoveToParent()
		{
			if (this._idxParent != 0)
			{
				this._pageCurrent = this._pageParent;
				this._idxCurrent = this._idxParent;
				this._pageParent = null;
				this._idxParent = 0;
				return true;
			}
			return XPathNodeHelper.GetParent(ref this._pageCurrent, ref this._idxCurrent);
		}

		public override bool MoveTo(XPathNavigator other)
		{
			XPathDocumentNavigator xpathDocumentNavigator = other as XPathDocumentNavigator;
			if (xpathDocumentNavigator != null)
			{
				this._pageCurrent = xpathDocumentNavigator._pageCurrent;
				this._idxCurrent = xpathDocumentNavigator._idxCurrent;
				this._pageParent = xpathDocumentNavigator._pageParent;
				this._idxParent = xpathDocumentNavigator._idxParent;
				return true;
			}
			return false;
		}

		public override bool MoveToId(string id)
		{
			XPathNode[] array;
			int num = this._pageCurrent[this._idxCurrent].Document.LookupIdElement(id, out array);
			if (num != 0)
			{
				this._pageCurrent = array;
				this._idxCurrent = num;
				this._pageParent = null;
				this._idxParent = 0;
				return true;
			}
			return false;
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			XPathDocumentNavigator xpathDocumentNavigator = other as XPathDocumentNavigator;
			return xpathDocumentNavigator != null && (this._idxCurrent == xpathDocumentNavigator._idxCurrent && this._pageCurrent == xpathDocumentNavigator._pageCurrent && this._idxParent == xpathDocumentNavigator._idxParent) && this._pageParent == xpathDocumentNavigator._pageParent;
		}

		public override bool HasChildren
		{
			get
			{
				return this._pageCurrent[this._idxCurrent].HasContentChild;
			}
		}

		public override void MoveToRoot()
		{
			if (this._idxParent != 0)
			{
				this._pageParent = null;
				this._idxParent = 0;
			}
			this._idxCurrent = this._pageCurrent[this._idxCurrent].GetRoot(out this._pageCurrent);
		}

		public override bool MoveToChild(string localName, string namespaceURI)
		{
			if (localName != this._atomizedLocalName)
			{
				this._atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			return XPathNodeHelper.GetElementChild(ref this._pageCurrent, ref this._idxCurrent, this._atomizedLocalName, namespaceURI);
		}

		public override bool MoveToNext(string localName, string namespaceURI)
		{
			if (localName != this._atomizedLocalName)
			{
				this._atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			return XPathNodeHelper.GetElementSibling(ref this._pageCurrent, ref this._idxCurrent, this._atomizedLocalName, namespaceURI);
		}

		public override bool MoveToChild(XPathNodeType type)
		{
			if (!this._pageCurrent[this._idxCurrent].HasCollapsedText)
			{
				return XPathNodeHelper.GetContentChild(ref this._pageCurrent, ref this._idxCurrent, type);
			}
			if (type != XPathNodeType.Text && type != XPathNodeType.All)
			{
				return false;
			}
			this._pageParent = this._pageCurrent;
			this._idxParent = this._idxCurrent;
			this._idxCurrent = this._pageCurrent[this._idxCurrent].Document.GetCollapsedTextNode(out this._pageCurrent);
			return true;
		}

		public override bool MoveToNext(XPathNodeType type)
		{
			return XPathNodeHelper.GetContentSibling(ref this._pageCurrent, ref this._idxCurrent, type);
		}

		public override bool MoveToFollowing(string localName, string namespaceURI, XPathNavigator end)
		{
			if (localName != this._atomizedLocalName)
			{
				this._atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			XPathNode[] array;
			int followingEnd = this.GetFollowingEnd(end as XPathDocumentNavigator, false, out array);
			if (this._idxParent == 0)
			{
				return XPathNodeHelper.GetElementFollowing(ref this._pageCurrent, ref this._idxCurrent, array, followingEnd, this._atomizedLocalName, namespaceURI);
			}
			if (!XPathNodeHelper.GetElementFollowing(ref this._pageParent, ref this._idxParent, array, followingEnd, this._atomizedLocalName, namespaceURI))
			{
				return false;
			}
			this._pageCurrent = this._pageParent;
			this._idxCurrent = this._idxParent;
			this._pageParent = null;
			this._idxParent = 0;
			return true;
		}

		public override bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
		{
			XPathDocumentNavigator xpathDocumentNavigator = end as XPathDocumentNavigator;
			XPathNode[] array;
			int num;
			if (type == XPathNodeType.Text || type == XPathNodeType.All)
			{
				if (this._pageCurrent[this._idxCurrent].HasCollapsedText)
				{
					if (xpathDocumentNavigator != null && this._idxCurrent == xpathDocumentNavigator._idxParent && this._pageCurrent == xpathDocumentNavigator._pageParent)
					{
						return false;
					}
					this._pageParent = this._pageCurrent;
					this._idxParent = this._idxCurrent;
					this._idxCurrent = this._pageCurrent[this._idxCurrent].Document.GetCollapsedTextNode(out this._pageCurrent);
					return true;
				}
				else if (type == XPathNodeType.Text)
				{
					num = this.GetFollowingEnd(xpathDocumentNavigator, true, out array);
					XPathNode[] array2;
					int num2;
					if (this._idxParent != 0)
					{
						array2 = this._pageParent;
						num2 = this._idxParent;
					}
					else
					{
						array2 = this._pageCurrent;
						num2 = this._idxCurrent;
					}
					if (xpathDocumentNavigator != null && xpathDocumentNavigator._idxParent != 0 && num2 == num && array2 == array)
					{
						return false;
					}
					if (!XPathNodeHelper.GetTextFollowing(ref array2, ref num2, array, num))
					{
						return false;
					}
					if (array2[num2].NodeType == XPathNodeType.Element)
					{
						this._idxCurrent = array2[num2].Document.GetCollapsedTextNode(out this._pageCurrent);
						this._pageParent = array2;
						this._idxParent = num2;
					}
					else
					{
						this._pageCurrent = array2;
						this._idxCurrent = num2;
						this._pageParent = null;
						this._idxParent = 0;
					}
					return true;
				}
			}
			num = this.GetFollowingEnd(xpathDocumentNavigator, false, out array);
			if (this._idxParent == 0)
			{
				return XPathNodeHelper.GetContentFollowing(ref this._pageCurrent, ref this._idxCurrent, array, num, type);
			}
			if (!XPathNodeHelper.GetContentFollowing(ref this._pageParent, ref this._idxParent, array, num, type))
			{
				return false;
			}
			this._pageCurrent = this._pageParent;
			this._idxCurrent = this._idxParent;
			this._pageParent = null;
			this._idxParent = 0;
			return true;
		}

		public override XPathNodeIterator SelectChildren(XPathNodeType type)
		{
			return new XPathDocumentKindChildIterator(this, type);
		}

		public override XPathNodeIterator SelectChildren(string name, string namespaceURI)
		{
			if (name == null || name.Length == 0)
			{
				return base.SelectChildren(name, namespaceURI);
			}
			return new XPathDocumentElementChildIterator(this, name, namespaceURI);
		}

		public override XPathNodeIterator SelectDescendants(XPathNodeType type, bool matchSelf)
		{
			return new XPathDocumentKindDescendantIterator(this, type, matchSelf);
		}

		public override XPathNodeIterator SelectDescendants(string name, string namespaceURI, bool matchSelf)
		{
			if (name == null || name.Length == 0)
			{
				return base.SelectDescendants(name, namespaceURI, matchSelf);
			}
			return new XPathDocumentElementDescendantIterator(this, name, namespaceURI, matchSelf);
		}

		public override XmlNodeOrder ComparePosition(XPathNavigator other)
		{
			XPathDocumentNavigator xpathDocumentNavigator = other as XPathDocumentNavigator;
			if (xpathDocumentNavigator != null)
			{
				XPathDocument document = this._pageCurrent[this._idxCurrent].Document;
				XPathDocument document2 = xpathDocumentNavigator._pageCurrent[xpathDocumentNavigator._idxCurrent].Document;
				if (document == document2)
				{
					int num = this.GetPrimaryLocation();
					int num2 = xpathDocumentNavigator.GetPrimaryLocation();
					if (num == num2)
					{
						num = this.GetSecondaryLocation();
						num2 = xpathDocumentNavigator.GetSecondaryLocation();
						if (num == num2)
						{
							return XmlNodeOrder.Same;
						}
					}
					if (num >= num2)
					{
						return XmlNodeOrder.After;
					}
					return XmlNodeOrder.Before;
				}
			}
			return XmlNodeOrder.Unknown;
		}

		public override bool IsDescendant(XPathNavigator other)
		{
			XPathDocumentNavigator xpathDocumentNavigator = other as XPathDocumentNavigator;
			if (xpathDocumentNavigator != null)
			{
				XPathNode[] pageParent;
				int num;
				if (xpathDocumentNavigator._idxParent != 0)
				{
					pageParent = xpathDocumentNavigator._pageParent;
					num = xpathDocumentNavigator._idxParent;
				}
				else
				{
					num = xpathDocumentNavigator._pageCurrent[xpathDocumentNavigator._idxCurrent].GetParent(out pageParent);
				}
				while (num != 0)
				{
					if (num == this._idxCurrent && pageParent == this._pageCurrent)
					{
						return true;
					}
					num = pageParent[num].GetParent(out pageParent);
				}
			}
			return false;
		}

		private int GetPrimaryLocation()
		{
			if (this._idxParent == 0)
			{
				return XPathNodeHelper.GetLocation(this._pageCurrent, this._idxCurrent);
			}
			return XPathNodeHelper.GetLocation(this._pageParent, this._idxParent);
		}

		private int GetSecondaryLocation()
		{
			if (this._idxParent == 0)
			{
				return int.MinValue;
			}
			XPathNodeType nodeType = this._pageCurrent[this._idxCurrent].NodeType;
			if (nodeType == XPathNodeType.Attribute)
			{
				return XPathNodeHelper.GetLocation(this._pageCurrent, this._idxCurrent);
			}
			if (nodeType == XPathNodeType.Namespace)
			{
				return -2147483647 + XPathNodeHelper.GetLocation(this._pageCurrent, this._idxCurrent);
			}
			return int.MaxValue;
		}

		internal override string UniqueId
		{
			get
			{
				char[] array = new char[16];
				int num = 0;
				array[num++] = XPathNavigator.NodeTypeLetter[(int)this._pageCurrent[this._idxCurrent].NodeType];
				int num2;
				if (this._idxParent != 0)
				{
					num2 = (this._pageParent[0].PageInfo.PageNumber - 1 << 16) | (this._idxParent - 1);
					do
					{
						array[num++] = XPathNavigator.UniqueIdTbl[num2 & 31];
						num2 >>= 5;
					}
					while (num2 != 0);
					array[num++] = '0';
				}
				num2 = (this._pageCurrent[0].PageInfo.PageNumber - 1 << 16) | (this._idxCurrent - 1);
				do
				{
					array[num++] = XPathNavigator.UniqueIdTbl[num2 & 31];
					num2 >>= 5;
				}
				while (num2 != 0);
				return new string(array, 0, num);
			}
		}

		public override object UnderlyingObject
		{
			get
			{
				return this.Clone();
			}
		}

		public bool HasLineInfo()
		{
			return this._pageCurrent[this._idxCurrent].Document.HasLineInfo;
		}

		public int LineNumber
		{
			get
			{
				if (this._idxParent != 0 && this.NodeType == XPathNodeType.Text)
				{
					return this._pageParent[this._idxParent].LineNumber;
				}
				return this._pageCurrent[this._idxCurrent].LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				if (this._idxParent != 0 && this.NodeType == XPathNodeType.Text)
				{
					return this._pageParent[this._idxParent].CollapsedLinePosition;
				}
				return this._pageCurrent[this._idxCurrent].LinePosition;
			}
		}

		public int GetPositionHashCode()
		{
			return this._idxCurrent ^ this._idxParent;
		}

		public bool IsElementMatch(string localName, string namespaceURI)
		{
			if (localName != this._atomizedLocalName)
			{
				this._atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			return this._idxParent == 0 && this._pageCurrent[this._idxCurrent].ElementMatch(this._atomizedLocalName, namespaceURI);
		}

		public bool IsKindMatch(XPathNodeType typ)
		{
			return ((1 << (int)this._pageCurrent[this._idxCurrent].NodeType) & XPathNavigator.GetKindMask(typ)) != 0;
		}

		private int GetFollowingEnd(XPathDocumentNavigator end, bool useParentOfVirtual, out XPathNode[] pageEnd)
		{
			if (end == null || this._pageCurrent[this._idxCurrent].Document != end._pageCurrent[end._idxCurrent].Document)
			{
				pageEnd = null;
				return 0;
			}
			if (end._idxParent == 0)
			{
				pageEnd = end._pageCurrent;
				return end._idxCurrent;
			}
			pageEnd = end._pageParent;
			if (!useParentOfVirtual)
			{
				return end._idxParent + 1;
			}
			return end._idxParent;
		}

		private XPathNode[] _pageCurrent;

		private XPathNode[] _pageParent;

		private int _idxCurrent;

		private int _idxParent;

		private string _atomizedLocalName;
	}
}
