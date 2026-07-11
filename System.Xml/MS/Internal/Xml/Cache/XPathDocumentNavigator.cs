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
			this.pageCurrent = pageCurrent;
			this.pageParent = pageParent;
			this.idxCurrent = idxCurrent;
			this.idxParent = idxParent;
		}

		public XPathDocumentNavigator(XPathDocumentNavigator nav)
			: this(nav.pageCurrent, nav.idxCurrent, nav.pageParent, nav.idxParent)
		{
			this.atomizedLocalName = nav.atomizedLocalName;
		}

		public override string Value
		{
			get
			{
				string value = this.pageCurrent[this.idxCurrent].Value;
				if (value != null)
				{
					return value;
				}
				if (this.idxParent != 0)
				{
					return this.pageParent[this.idxParent].Value;
				}
				string text = string.Empty;
				StringBuilder stringBuilder = null;
				XPathNode[] array2;
				XPathNode[] array = (array2 = this.pageCurrent);
				int num2;
				int num = (num2 = this.idxCurrent);
				if (!XPathNodeHelper.GetNonDescendant(ref array, ref num))
				{
					array = null;
					num = 0;
				}
				while (XPathNodeHelper.GetTextFollowing(ref array2, ref num2, array, num))
				{
					if (text.Length == 0)
					{
						text = array2[num2].Value;
					}
					else
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder();
							stringBuilder.Append(text);
						}
						stringBuilder.Append(array2[num2].Value);
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
			return new XPathDocumentNavigator(this.pageCurrent, this.idxCurrent, this.pageParent, this.idxParent);
		}

		public override XPathNodeType NodeType
		{
			get
			{
				return this.pageCurrent[this.idxCurrent].NodeType;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.pageCurrent[this.idxCurrent].LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.pageCurrent[this.idxCurrent].NamespaceUri;
			}
		}

		public override string Name
		{
			get
			{
				return this.pageCurrent[this.idxCurrent].Name;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.pageCurrent[this.idxCurrent].Prefix;
			}
		}

		public override string BaseURI
		{
			get
			{
				XPathNode[] array;
				int parent;
				if (this.idxParent != 0)
				{
					array = this.pageParent;
					parent = this.idxParent;
				}
				else
				{
					array = this.pageCurrent;
					parent = this.idxCurrent;
				}
				for (;;)
				{
					XPathNodeType nodeType = array[parent].NodeType;
					if (nodeType <= XPathNodeType.Element || nodeType == XPathNodeType.ProcessingInstruction)
					{
						break;
					}
					parent = array[parent].GetParent(out array);
					if (parent == 0)
					{
						goto Block_3;
					}
				}
				return array[parent].BaseUri;
				Block_3:
				return string.Empty;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.pageCurrent[this.idxCurrent].AllowShortcutTag;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.pageCurrent[this.idxCurrent].Document.NameTable;
			}
		}

		public override bool MoveToFirstAttribute()
		{
			XPathNode[] array = this.pageCurrent;
			int num = this.idxCurrent;
			if (XPathNodeHelper.GetFirstAttribute(ref this.pageCurrent, ref this.idxCurrent))
			{
				this.pageParent = array;
				this.idxParent = num;
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			return XPathNodeHelper.GetNextAttribute(ref this.pageCurrent, ref this.idxCurrent);
		}

		public override bool HasAttributes
		{
			get
			{
				return this.pageCurrent[this.idxCurrent].HasAttribute;
			}
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			XPathNode[] array = this.pageCurrent;
			int num = this.idxCurrent;
			if (localName != this.atomizedLocalName)
			{
				this.atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			if (XPathNodeHelper.GetAttribute(ref this.pageCurrent, ref this.idxCurrent, this.atomizedLocalName, namespaceURI))
			{
				this.pageParent = array;
				this.idxParent = num;
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
				num = XPathNodeHelper.GetLocalNamespaces(this.pageCurrent, this.idxCurrent, out array);
			}
			else
			{
				num = XPathNodeHelper.GetInScopeNamespaces(this.pageCurrent, this.idxCurrent, out array);
			}
			while (num != 0)
			{
				if (namespaceScope != XPathNamespaceScope.ExcludeXml || !array[num].IsXmlNamespaceNode)
				{
					this.pageParent = this.pageCurrent;
					this.idxParent = this.idxCurrent;
					this.pageCurrent = array;
					this.idxCurrent = num;
					return true;
				}
				num = array[num].GetSibling(out array);
			}
			return false;
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope scope)
		{
			XPathNode[] array = this.pageCurrent;
			int sibling = this.idxCurrent;
			if (array[sibling].NodeType != XPathNodeType.Namespace)
			{
				return false;
			}
			for (;;)
			{
				sibling = array[sibling].GetSibling(out array);
				if (sibling == 0)
				{
					break;
				}
				if (scope != XPathNamespaceScope.ExcludeXml)
				{
					goto Block_3;
				}
				if (!array[sibling].IsXmlNamespaceNode)
				{
					goto IL_006A;
				}
			}
			return false;
			Block_3:
			XPathNode[] array2;
			if (scope == XPathNamespaceScope.Local && (array[sibling].GetParent(out array2) != this.idxParent || array2 != this.pageParent))
			{
				return false;
			}
			IL_006A:
			this.pageCurrent = array;
			this.idxCurrent = sibling;
			return true;
		}

		public override bool MoveToNext()
		{
			return XPathNodeHelper.GetContentSibling(ref this.pageCurrent, ref this.idxCurrent);
		}

		public override bool MoveToPrevious()
		{
			return this.idxParent == 0 && XPathNodeHelper.GetPreviousContentSibling(ref this.pageCurrent, ref this.idxCurrent);
		}

		public override bool MoveToFirstChild()
		{
			if (this.pageCurrent[this.idxCurrent].HasCollapsedText)
			{
				this.pageParent = this.pageCurrent;
				this.idxParent = this.idxCurrent;
				this.idxCurrent = this.pageCurrent[this.idxCurrent].Document.GetCollapsedTextNode(out this.pageCurrent);
				return true;
			}
			return XPathNodeHelper.GetContentChild(ref this.pageCurrent, ref this.idxCurrent);
		}

		public override bool MoveToParent()
		{
			if (this.idxParent != 0)
			{
				this.pageCurrent = this.pageParent;
				this.idxCurrent = this.idxParent;
				this.pageParent = null;
				this.idxParent = 0;
				return true;
			}
			return XPathNodeHelper.GetParent(ref this.pageCurrent, ref this.idxCurrent);
		}

		public override bool MoveTo(XPathNavigator other)
		{
			XPathDocumentNavigator xpathDocumentNavigator = other as XPathDocumentNavigator;
			if (xpathDocumentNavigator != null)
			{
				this.pageCurrent = xpathDocumentNavigator.pageCurrent;
				this.idxCurrent = xpathDocumentNavigator.idxCurrent;
				this.pageParent = xpathDocumentNavigator.pageParent;
				this.idxParent = xpathDocumentNavigator.idxParent;
				return true;
			}
			return false;
		}

		public override bool MoveToId(string id)
		{
			XPathNode[] array;
			int num = this.pageCurrent[this.idxCurrent].Document.LookupIdElement(id, out array);
			if (num != 0)
			{
				this.pageCurrent = array;
				this.idxCurrent = num;
				this.pageParent = null;
				this.idxParent = 0;
				return true;
			}
			return false;
		}

		public override bool IsSamePosition(XPathNavigator other)
		{
			XPathDocumentNavigator xpathDocumentNavigator = other as XPathDocumentNavigator;
			return xpathDocumentNavigator != null && (this.idxCurrent == xpathDocumentNavigator.idxCurrent && this.pageCurrent == xpathDocumentNavigator.pageCurrent && this.idxParent == xpathDocumentNavigator.idxParent) && this.pageParent == xpathDocumentNavigator.pageParent;
		}

		public override bool HasChildren
		{
			get
			{
				return this.pageCurrent[this.idxCurrent].HasContentChild;
			}
		}

		public override void MoveToRoot()
		{
			if (this.idxParent != 0)
			{
				this.pageParent = null;
				this.idxParent = 0;
			}
			this.idxCurrent = this.pageCurrent[this.idxCurrent].GetRoot(out this.pageCurrent);
		}

		public override bool MoveToChild(string localName, string namespaceURI)
		{
			if (localName != this.atomizedLocalName)
			{
				this.atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			return XPathNodeHelper.GetElementChild(ref this.pageCurrent, ref this.idxCurrent, this.atomizedLocalName, namespaceURI);
		}

		public override bool MoveToNext(string localName, string namespaceURI)
		{
			if (localName != this.atomizedLocalName)
			{
				this.atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			return XPathNodeHelper.GetElementSibling(ref this.pageCurrent, ref this.idxCurrent, this.atomizedLocalName, namespaceURI);
		}

		public override bool MoveToChild(XPathNodeType type)
		{
			if (!this.pageCurrent[this.idxCurrent].HasCollapsedText)
			{
				return XPathNodeHelper.GetContentChild(ref this.pageCurrent, ref this.idxCurrent, type);
			}
			if (type != XPathNodeType.Text && type != XPathNodeType.All)
			{
				return false;
			}
			this.pageParent = this.pageCurrent;
			this.idxParent = this.idxCurrent;
			this.idxCurrent = this.pageCurrent[this.idxCurrent].Document.GetCollapsedTextNode(out this.pageCurrent);
			return true;
		}

		public override bool MoveToNext(XPathNodeType type)
		{
			return XPathNodeHelper.GetContentSibling(ref this.pageCurrent, ref this.idxCurrent, type);
		}

		public override bool MoveToFollowing(string localName, string namespaceURI, XPathNavigator end)
		{
			if (localName != this.atomizedLocalName)
			{
				this.atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			XPathNode[] array;
			int followingEnd = this.GetFollowingEnd(end as XPathDocumentNavigator, false, out array);
			if (this.idxParent == 0)
			{
				return XPathNodeHelper.GetElementFollowing(ref this.pageCurrent, ref this.idxCurrent, array, followingEnd, this.atomizedLocalName, namespaceURI);
			}
			if (!XPathNodeHelper.GetElementFollowing(ref this.pageParent, ref this.idxParent, array, followingEnd, this.atomizedLocalName, namespaceURI))
			{
				return false;
			}
			this.pageCurrent = this.pageParent;
			this.idxCurrent = this.idxParent;
			this.pageParent = null;
			this.idxParent = 0;
			return true;
		}

		public override bool MoveToFollowing(XPathNodeType type, XPathNavigator end)
		{
			XPathDocumentNavigator xpathDocumentNavigator = end as XPathDocumentNavigator;
			XPathNode[] array;
			int num;
			if (type == XPathNodeType.Text || type == XPathNodeType.All)
			{
				if (this.pageCurrent[this.idxCurrent].HasCollapsedText)
				{
					if (xpathDocumentNavigator != null && this.idxCurrent == xpathDocumentNavigator.idxParent && this.pageCurrent == xpathDocumentNavigator.pageParent)
					{
						return false;
					}
					this.pageParent = this.pageCurrent;
					this.idxParent = this.idxCurrent;
					this.idxCurrent = this.pageCurrent[this.idxCurrent].Document.GetCollapsedTextNode(out this.pageCurrent);
					return true;
				}
				else if (type == XPathNodeType.Text)
				{
					num = this.GetFollowingEnd(xpathDocumentNavigator, true, out array);
					XPathNode[] array2;
					int num2;
					if (this.idxParent != 0)
					{
						array2 = this.pageParent;
						num2 = this.idxParent;
					}
					else
					{
						array2 = this.pageCurrent;
						num2 = this.idxCurrent;
					}
					if (xpathDocumentNavigator != null && xpathDocumentNavigator.idxParent != 0 && num2 == num && array2 == array)
					{
						return false;
					}
					if (!XPathNodeHelper.GetTextFollowing(ref array2, ref num2, array, num))
					{
						return false;
					}
					if (array2[num2].NodeType == XPathNodeType.Element)
					{
						this.idxCurrent = array2[num2].Document.GetCollapsedTextNode(out this.pageCurrent);
						this.pageParent = array2;
						this.idxParent = num2;
					}
					else
					{
						this.pageCurrent = array2;
						this.idxCurrent = num2;
						this.pageParent = null;
						this.idxParent = 0;
					}
					return true;
				}
			}
			num = this.GetFollowingEnd(xpathDocumentNavigator, false, out array);
			if (this.idxParent == 0)
			{
				return XPathNodeHelper.GetContentFollowing(ref this.pageCurrent, ref this.idxCurrent, array, num, type);
			}
			if (!XPathNodeHelper.GetContentFollowing(ref this.pageParent, ref this.idxParent, array, num, type))
			{
				return false;
			}
			this.pageCurrent = this.pageParent;
			this.idxCurrent = this.idxParent;
			this.pageParent = null;
			this.idxParent = 0;
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
				XPathDocument document = this.pageCurrent[this.idxCurrent].Document;
				XPathDocument document2 = xpathDocumentNavigator.pageCurrent[xpathDocumentNavigator.idxCurrent].Document;
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
				XPathNode[] array;
				int num;
				if (xpathDocumentNavigator.idxParent != 0)
				{
					array = xpathDocumentNavigator.pageParent;
					num = xpathDocumentNavigator.idxParent;
				}
				else
				{
					num = xpathDocumentNavigator.pageCurrent[xpathDocumentNavigator.idxCurrent].GetParent(out array);
				}
				while (num != 0)
				{
					if (num == this.idxCurrent && array == this.pageCurrent)
					{
						return true;
					}
					num = array[num].GetParent(out array);
				}
			}
			return false;
		}

		private int GetPrimaryLocation()
		{
			if (this.idxParent == 0)
			{
				return XPathNodeHelper.GetLocation(this.pageCurrent, this.idxCurrent);
			}
			return XPathNodeHelper.GetLocation(this.pageParent, this.idxParent);
		}

		private int GetSecondaryLocation()
		{
			if (this.idxParent == 0)
			{
				return int.MinValue;
			}
			XPathNodeType nodeType = this.pageCurrent[this.idxCurrent].NodeType;
			if (nodeType == XPathNodeType.Attribute)
			{
				return XPathNodeHelper.GetLocation(this.pageCurrent, this.idxCurrent);
			}
			if (nodeType == XPathNodeType.Namespace)
			{
				return -2147483647 + XPathNodeHelper.GetLocation(this.pageCurrent, this.idxCurrent);
			}
			return int.MaxValue;
		}

		internal override string UniqueId
		{
			get
			{
				char[] array = new char[16];
				int num = 0;
				array[num++] = XPathNavigator.NodeTypeLetter[(int)this.pageCurrent[this.idxCurrent].NodeType];
				int num2;
				if (this.idxParent != 0)
				{
					num2 = (this.pageParent[0].PageInfo.PageNumber - 1 << 16) | (this.idxParent - 1);
					do
					{
						array[num++] = XPathNavigator.UniqueIdTbl[num2 & 31];
						num2 >>= 5;
					}
					while (num2 != 0);
					array[num++] = '0';
				}
				num2 = (this.pageCurrent[0].PageInfo.PageNumber - 1 << 16) | (this.idxCurrent - 1);
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
			return this.pageCurrent[this.idxCurrent].Document.HasLineInfo;
		}

		public int LineNumber
		{
			get
			{
				if (this.idxParent != 0 && this.NodeType == XPathNodeType.Text)
				{
					return this.pageParent[this.idxParent].LineNumber;
				}
				return this.pageCurrent[this.idxCurrent].LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				if (this.idxParent != 0 && this.NodeType == XPathNodeType.Text)
				{
					return this.pageParent[this.idxParent].CollapsedLinePosition;
				}
				return this.pageCurrent[this.idxCurrent].LinePosition;
			}
		}

		public int GetPositionHashCode()
		{
			return this.idxCurrent ^ this.idxParent;
		}

		public bool IsElementMatch(string localName, string namespaceURI)
		{
			if (localName != this.atomizedLocalName)
			{
				this.atomizedLocalName = ((localName != null) ? this.NameTable.Get(localName) : null);
			}
			return this.idxParent == 0 && this.pageCurrent[this.idxCurrent].ElementMatch(this.atomizedLocalName, namespaceURI);
		}

		public bool IsContentKindMatch(XPathNodeType typ)
		{
			return ((1 << (int)this.pageCurrent[this.idxCurrent].NodeType) & XPathNavigator.GetContentKindMask(typ)) != 0;
		}

		public bool IsKindMatch(XPathNodeType typ)
		{
			return ((1 << (int)this.pageCurrent[this.idxCurrent].NodeType) & XPathNavigator.GetKindMask(typ)) != 0;
		}

		private int GetFollowingEnd(XPathDocumentNavigator end, bool useParentOfVirtual, out XPathNode[] pageEnd)
		{
			if (end == null || this.pageCurrent[this.idxCurrent].Document != end.pageCurrent[end.idxCurrent].Document)
			{
				pageEnd = null;
				return 0;
			}
			if (end.idxParent == 0)
			{
				pageEnd = end.pageCurrent;
				return end.idxCurrent;
			}
			pageEnd = end.pageParent;
			if (!useParentOfVirtual)
			{
				return end.idxParent + 1;
			}
			return end.idxParent;
		}

		private XPathNode[] pageCurrent;

		private XPathNode[] pageParent;

		private int idxCurrent;

		private int idxParent;

		private string atomizedLocalName;
	}
}
