using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal sealed class XPathDocumentBuilder : XmlRawWriter
	{
		public XPathDocumentBuilder(XPathDocument doc, IXmlLineInfo lineInfo, string baseUri, XPathDocument.LoadFlags flags)
		{
			this._nodePageFact.Init(256);
			this._nmspPageFact.Init(16);
			this._stkNmsp = new Stack<XPathNodeRef>();
			this.Initialize(doc, lineInfo, baseUri, flags);
		}

		public void Initialize(XPathDocument doc, IXmlLineInfo lineInfo, string baseUri, XPathDocument.LoadFlags flags)
		{
			this._doc = doc;
			this._nameTable = doc.NameTable;
			this._atomizeNames = (flags & XPathDocument.LoadFlags.AtomizeNames) > XPathDocument.LoadFlags.None;
			this._idxParent = (this._idxSibling = 0);
			this._elemNameIndex = new XPathNodeRef[64];
			this._textBldr.Initialize(lineInfo);
			this._lineInfo = lineInfo;
			this._lineNumBase = 0;
			this._linePosBase = 0;
			this._infoTable = new XPathNodeInfoTable();
			XPathNode[] array;
			int num = this.NewNode(out array, XPathNodeType.Text, string.Empty, string.Empty, string.Empty, string.Empty);
			this._doc.SetCollapsedTextNode(array, num);
			this._idxNmsp = this.NewNamespaceNode(out this._pageNmsp, this._nameTable.Add("xml"), this._nameTable.Add("http://www.w3.org/XML/1998/namespace"), null, 0);
			this._doc.SetXmlNamespaceNode(this._pageNmsp, this._idxNmsp);
			if ((flags & XPathDocument.LoadFlags.Fragment) == XPathDocument.LoadFlags.None)
			{
				this._idxParent = this.NewNode(out this._pageParent, XPathNodeType.Root, string.Empty, string.Empty, string.Empty, baseUri);
				this._doc.SetRootNode(this._pageParent, this._idxParent);
				return;
			}
			this._doc.SetRootNode(this._nodePageFact.NextNodePage, this._nodePageFact.NextNodeIndex);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.WriteStartElement(prefix, localName, ns, string.Empty);
		}

		public void WriteStartElement(string prefix, string localName, string ns, string baseUri)
		{
			if (this._atomizeNames)
			{
				prefix = this._nameTable.Add(prefix);
				localName = this._nameTable.Add(localName);
				ns = this._nameTable.Add(ns);
			}
			this.AddSibling(XPathNodeType.Element, localName, ns, prefix, baseUri);
			this._pageParent = this._pageSibling;
			this._idxParent = this._idxSibling;
			this._idxSibling = 0;
			int num = this._pageParent[this._idxParent].LocalNameHashCode & 63;
			this._elemNameIndex[num] = this.LinkSimilarElements(this._elemNameIndex[num].Page, this._elemNameIndex[num].Index, this._pageParent, this._idxParent);
			if (this._elemIdMap != null)
			{
				this._idAttrName = (XmlQualifiedName)this._elemIdMap[new XmlQualifiedName(localName, prefix)];
			}
		}

		public override void WriteEndElement()
		{
			this.WriteEndElement(true);
		}

		public override void WriteFullEndElement()
		{
			this.WriteEndElement(false);
		}

		internal override void WriteEndElement(string prefix, string localName, string namespaceName)
		{
			this.WriteEndElement(true);
		}

		internal override void WriteFullEndElement(string prefix, string localName, string namespaceName)
		{
			this.WriteEndElement(false);
		}

		public void WriteEndElement(bool allowShortcutTag)
		{
			if (!this._pageParent[this._idxParent].HasContentChild)
			{
				TextBlockType textType = this._textBldr.TextType;
				if (textType == TextBlockType.Text)
				{
					if (this._lineInfo != null)
					{
						if (this._textBldr.LineNumber != this._pageParent[this._idxParent].LineNumber)
						{
							goto IL_00CD;
						}
						int num = this._textBldr.LinePosition - this._pageParent[this._idxParent].LinePosition;
						if (num < 0 || num > 255)
						{
							goto IL_00CD;
						}
						this._pageParent[this._idxParent].SetCollapsedLineInfoOffset(num);
					}
					this._pageParent[this._idxParent].SetCollapsedValue(this._textBldr.ReadText());
					goto IL_012D;
				}
				if (textType - TextBlockType.SignificantWhitespace > 1)
				{
					this._pageParent[this._idxParent].SetEmptyValue(allowShortcutTag);
					goto IL_012D;
				}
				IL_00CD:
				this.CachedTextNode();
				this._pageParent[this._idxParent].SetValue(this._pageSibling[this._idxSibling].Value);
			}
			else if (this._textBldr.HasText)
			{
				this.CachedTextNode();
			}
			IL_012D:
			if (this._pageParent[this._idxParent].HasNamespaceDecls)
			{
				this._doc.AddNamespace(this._pageParent, this._idxParent, this._pageNmsp, this._idxNmsp);
				XPathNodeRef xpathNodeRef = this._stkNmsp.Pop();
				this._pageNmsp = xpathNodeRef.Page;
				this._idxNmsp = xpathNodeRef.Index;
			}
			this._pageSibling = this._pageParent;
			this._idxSibling = this._idxParent;
			this._idxParent = this._pageParent[this._idxParent].GetParent(out this._pageParent);
		}

		public override void WriteStartAttribute(string prefix, string localName, string namespaceName)
		{
			if (this._atomizeNames)
			{
				prefix = this._nameTable.Add(prefix);
				localName = this._nameTable.Add(localName);
				namespaceName = this._nameTable.Add(namespaceName);
			}
			this.AddSibling(XPathNodeType.Attribute, localName, namespaceName, prefix, string.Empty);
		}

		public override void WriteEndAttribute()
		{
			this._pageSibling[this._idxSibling].SetValue(this._textBldr.ReadText());
			if (this._idAttrName != null && this._pageSibling[this._idxSibling].LocalName == this._idAttrName.Name && this._pageSibling[this._idxSibling].Prefix == this._idAttrName.Namespace)
			{
				this._doc.AddIdElement(this._pageSibling[this._idxSibling].Value, this._pageParent, this._idxParent);
			}
		}

		public override void WriteCData(string text)
		{
			this.WriteString(text, TextBlockType.Text);
		}

		public override void WriteComment(string text)
		{
			this.AddSibling(XPathNodeType.Comment, string.Empty, string.Empty, string.Empty, string.Empty);
			this._pageSibling[this._idxSibling].SetValue(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.WriteProcessingInstruction(name, text, string.Empty);
		}

		public void WriteProcessingInstruction(string name, string text, string baseUri)
		{
			if (this._atomizeNames)
			{
				name = this._nameTable.Add(name);
			}
			this.AddSibling(XPathNodeType.ProcessingInstruction, name, string.Empty, string.Empty, baseUri);
			this._pageSibling[this._idxSibling].SetValue(text);
		}

		public override void WriteWhitespace(string ws)
		{
			this.WriteString(ws, TextBlockType.Whitespace);
		}

		public override void WriteString(string text)
		{
			this.WriteString(text, TextBlockType.Text);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.WriteString(new string(buffer, index, count), TextBlockType.Text);
		}

		public override void WriteRaw(string data)
		{
			this.WriteString(data, TextBlockType.Text);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.WriteString(new string(buffer, index, count), TextBlockType.Text);
		}

		public void WriteString(string text, TextBlockType textType)
		{
			this._textBldr.WriteTextBlock(text, textType);
		}

		public override void WriteEntityRef(string name)
		{
			throw new NotImplementedException();
		}

		public override void WriteCharEntity(char ch)
		{
			this.WriteString(new string(ch, 1), TextBlockType.Text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			char[] array = new char[] { highChar, lowChar };
			this.WriteString(new string(array), TextBlockType.Text);
		}

		public override void Close()
		{
			if (this._textBldr.HasText)
			{
				this.CachedTextNode();
			}
			XPathNode[] array;
			if (this._doc.GetRootNode(out array) == this._nodePageFact.NextNodeIndex && array == this._nodePageFact.NextNodePage)
			{
				this.AddSibling(XPathNodeType.Text, string.Empty, string.Empty, string.Empty, string.Empty);
				this._pageSibling[this._idxSibling].SetValue(string.Empty);
			}
		}

		public override void Flush()
		{
		}

		internal override void WriteXmlDeclaration(XmlStandalone standalone)
		{
		}

		internal override void WriteXmlDeclaration(string xmldecl)
		{
		}

		internal override void StartElementContent()
		{
		}

		internal override void WriteNamespaceDeclaration(string prefix, string namespaceName)
		{
			if (this._atomizeNames)
			{
				prefix = this._nameTable.Add(prefix);
			}
			namespaceName = this._nameTable.Add(namespaceName);
			XPathNode[] pageNmsp = this._pageNmsp;
			int num = this._idxNmsp;
			while (num != 0 && pageNmsp[num].LocalName != prefix)
			{
				num = pageNmsp[num].GetSibling(out pageNmsp);
			}
			XPathNode[] array;
			int num2 = this.NewNamespaceNode(out array, prefix, namespaceName, this._pageParent, this._idxParent);
			if (num != 0)
			{
				XPathNode[] pageNmsp2 = this._pageNmsp;
				int num3 = this._idxNmsp;
				XPathNode[] array2 = array;
				int num4 = num2;
				while (num3 != num || pageNmsp2 != pageNmsp)
				{
					XPathNode[] array3;
					int num5 = pageNmsp2[num3].GetParent(out array3);
					num5 = this.NewNamespaceNode(out array3, pageNmsp2[num3].LocalName, pageNmsp2[num3].Value, array3, num5);
					array2[num4].SetSibling(this._infoTable, array3, num5);
					array2 = array3;
					num4 = num5;
					num3 = pageNmsp2[num3].GetSibling(out pageNmsp2);
				}
				num = pageNmsp[num].GetSibling(out pageNmsp);
				if (num != 0)
				{
					array2[num4].SetSibling(this._infoTable, pageNmsp, num);
				}
			}
			else if (this._idxParent != 0)
			{
				array[num2].SetSibling(this._infoTable, this._pageNmsp, this._idxNmsp);
			}
			else
			{
				this._doc.SetRootNode(array, num2);
			}
			if (this._idxParent != 0)
			{
				if (!this._pageParent[this._idxParent].HasNamespaceDecls)
				{
					this._stkNmsp.Push(new XPathNodeRef(this._pageNmsp, this._idxNmsp));
					this._pageParent[this._idxParent].HasNamespaceDecls = true;
				}
				this._pageNmsp = array;
				this._idxNmsp = num2;
			}
		}

		public void CreateIdTables(IDtdInfo dtdInfo)
		{
			foreach (IDtdAttributeListInfo dtdAttributeListInfo in dtdInfo.GetAttributeLists())
			{
				IDtdAttributeInfo dtdAttributeInfo = dtdAttributeListInfo.LookupIdAttribute();
				if (dtdAttributeInfo != null)
				{
					if (this._elemIdMap == null)
					{
						this._elemIdMap = new Hashtable();
					}
					this._elemIdMap.Add(new XmlQualifiedName(dtdAttributeListInfo.LocalName, dtdAttributeListInfo.Prefix), new XmlQualifiedName(dtdAttributeInfo.LocalName, dtdAttributeInfo.Prefix));
				}
			}
		}

		private XPathNodeRef LinkSimilarElements(XPathNode[] pagePrev, int idxPrev, XPathNode[] pageNext, int idxNext)
		{
			if (pagePrev != null)
			{
				pagePrev[idxPrev].SetSimilarElement(this._infoTable, pageNext, idxNext);
			}
			return new XPathNodeRef(pageNext, idxNext);
		}

		private int NewNamespaceNode(out XPathNode[] page, string prefix, string namespaceUri, XPathNode[] pageElem, int idxElem)
		{
			XPathNode[] array;
			int num;
			this._nmspPageFact.AllocateSlot(out array, out num);
			int num2;
			int num3;
			this.ComputeLineInfo(false, out num2, out num3);
			XPathNodeInfoAtom xpathNodeInfoAtom = this._infoTable.Create(prefix, string.Empty, string.Empty, string.Empty, pageElem, array, null, this._doc, this._lineNumBase, this._linePosBase);
			array[num].Create(xpathNodeInfoAtom, XPathNodeType.Namespace, idxElem);
			array[num].SetValue(namespaceUri);
			array[num].SetLineInfoOffsets(num2, num3);
			page = array;
			return num;
		}

		private int NewNode(out XPathNode[] page, XPathNodeType xptyp, string localName, string namespaceUri, string prefix, string baseUri)
		{
			XPathNode[] array;
			int num;
			this._nodePageFact.AllocateSlot(out array, out num);
			int num2;
			int num3;
			this.ComputeLineInfo(XPathNavigator.IsText(xptyp), out num2, out num3);
			XPathNodeInfoAtom xpathNodeInfoAtom = this._infoTable.Create(localName, namespaceUri, prefix, baseUri, this._pageParent, array, array, this._doc, this._lineNumBase, this._linePosBase);
			array[num].Create(xpathNodeInfoAtom, xptyp, this._idxParent);
			array[num].SetLineInfoOffsets(num2, num3);
			page = array;
			return num;
		}

		private void ComputeLineInfo(bool isTextNode, out int lineNumOffset, out int linePosOffset)
		{
			if (this._lineInfo == null)
			{
				lineNumOffset = 0;
				linePosOffset = 0;
				return;
			}
			int num;
			int num2;
			if (isTextNode)
			{
				num = this._textBldr.LineNumber;
				num2 = this._textBldr.LinePosition;
			}
			else
			{
				num = this._lineInfo.LineNumber;
				num2 = this._lineInfo.LinePosition;
			}
			lineNumOffset = num - this._lineNumBase;
			if (lineNumOffset < 0 || lineNumOffset > 16383)
			{
				this._lineNumBase = num;
				lineNumOffset = 0;
			}
			linePosOffset = num2 - this._linePosBase;
			if (linePosOffset < 0 || linePosOffset > 65535)
			{
				this._linePosBase = num2;
				linePosOffset = 0;
			}
		}

		private void AddSibling(XPathNodeType xptyp, string localName, string namespaceUri, string prefix, string baseUri)
		{
			if (this._textBldr.HasText)
			{
				this.CachedTextNode();
			}
			XPathNode[] array;
			int num = this.NewNode(out array, xptyp, localName, namespaceUri, prefix, baseUri);
			if (this._idxParent != 0)
			{
				this._pageParent[this._idxParent].SetParentProperties(xptyp);
				if (this._idxSibling != 0)
				{
					this._pageSibling[this._idxSibling].SetSibling(this._infoTable, array, num);
				}
			}
			this._pageSibling = array;
			this._idxSibling = num;
		}

		private void CachedTextNode()
		{
			TextBlockType textType = this._textBldr.TextType;
			string text = this._textBldr.ReadText();
			this.AddSibling((XPathNodeType)textType, string.Empty, string.Empty, string.Empty, string.Empty);
			this._pageSibling[this._idxSibling].SetValue(text);
		}

		private XPathDocumentBuilder.NodePageFactory _nodePageFact;

		private XPathDocumentBuilder.NodePageFactory _nmspPageFact;

		private XPathDocumentBuilder.TextBlockBuilder _textBldr;

		private Stack<XPathNodeRef> _stkNmsp;

		private XPathNodeInfoTable _infoTable;

		private XPathDocument _doc;

		private IXmlLineInfo _lineInfo;

		private XmlNameTable _nameTable;

		private bool _atomizeNames;

		private XPathNode[] _pageNmsp;

		private int _idxNmsp;

		private XPathNode[] _pageParent;

		private int _idxParent;

		private XPathNode[] _pageSibling;

		private int _idxSibling;

		private int _lineNumBase;

		private int _linePosBase;

		private XmlQualifiedName _idAttrName;

		private Hashtable _elemIdMap;

		private XPathNodeRef[] _elemNameIndex;

		private const int ElementIndexSize = 64;

		private struct NodePageFactory
		{
			public void Init(int initialPageSize)
			{
				this._pageSize = initialPageSize;
				this._page = new XPathNode[this._pageSize];
				this._pageInfo = new XPathNodePageInfo(null, 1);
				this._page[0].Create(this._pageInfo);
			}

			public XPathNode[] NextNodePage
			{
				get
				{
					return this._page;
				}
			}

			public int NextNodeIndex
			{
				get
				{
					return this._pageInfo.NodeCount;
				}
			}

			public void AllocateSlot(out XPathNode[] page, out int idx)
			{
				page = this._page;
				idx = this._pageInfo.NodeCount;
				XPathNodePageInfo pageInfo = this._pageInfo;
				int num = pageInfo.NodeCount + 1;
				pageInfo.NodeCount = num;
				if (num >= this._page.Length)
				{
					if (this._pageSize < 65536)
					{
						this._pageSize *= 2;
					}
					this._page = new XPathNode[this._pageSize];
					this._pageInfo.NextPage = this._page;
					this._pageInfo = new XPathNodePageInfo(page, this._pageInfo.PageNumber + 1);
					this._page[0].Create(this._pageInfo);
				}
			}

			private XPathNode[] _page;

			private XPathNodePageInfo _pageInfo;

			private int _pageSize;
		}

		private struct TextBlockBuilder
		{
			public void Initialize(IXmlLineInfo lineInfo)
			{
				this._lineInfo = lineInfo;
				this._textType = TextBlockType.None;
			}

			public TextBlockType TextType
			{
				get
				{
					return this._textType;
				}
			}

			public bool HasText
			{
				get
				{
					return this._textType > TextBlockType.None;
				}
			}

			public int LineNumber
			{
				get
				{
					return this._lineNum;
				}
			}

			public int LinePosition
			{
				get
				{
					return this._linePos;
				}
			}

			public void WriteTextBlock(string text, TextBlockType textType)
			{
				if (text.Length != 0)
				{
					if (this._textType == TextBlockType.None)
					{
						this._text = text;
						this._textType = textType;
						if (this._lineInfo != null)
						{
							this._lineNum = this._lineInfo.LineNumber;
							this._linePos = this._lineInfo.LinePosition;
							return;
						}
					}
					else
					{
						this._text += text;
						if (textType < this._textType)
						{
							this._textType = textType;
						}
					}
				}
			}

			public string ReadText()
			{
				if (this._textType == TextBlockType.None)
				{
					return string.Empty;
				}
				this._textType = TextBlockType.None;
				return this._text;
			}

			private IXmlLineInfo _lineInfo;

			private TextBlockType _textType;

			private string _text;

			private int _lineNum;

			private int _linePos;
		}
	}
}
