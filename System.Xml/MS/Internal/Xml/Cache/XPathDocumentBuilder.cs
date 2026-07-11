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
			this.nodePageFact.Init(256);
			this.nmspPageFact.Init(16);
			this.stkNmsp = new Stack<XPathNodeRef>();
			this.Initialize(doc, lineInfo, baseUri, flags);
		}

		public void Initialize(XPathDocument doc, IXmlLineInfo lineInfo, string baseUri, XPathDocument.LoadFlags flags)
		{
			this.doc = doc;
			this.nameTable = doc.NameTable;
			this.atomizeNames = (flags & XPathDocument.LoadFlags.AtomizeNames) > XPathDocument.LoadFlags.None;
			this.idxParent = (this.idxSibling = 0);
			this.elemNameIndex = new XPathNodeRef[64];
			this.textBldr.Initialize(lineInfo);
			this.lineInfo = lineInfo;
			this.lineNumBase = 0;
			this.linePosBase = 0;
			this.infoTable = new XPathNodeInfoTable();
			XPathNode[] array;
			int num = this.NewNode(out array, XPathNodeType.Text, string.Empty, string.Empty, string.Empty, string.Empty);
			this.doc.SetCollapsedTextNode(array, num);
			this.idxNmsp = this.NewNamespaceNode(out this.pageNmsp, this.nameTable.Add("xml"), this.nameTable.Add("http://www.w3.org/XML/1998/namespace"), null, 0);
			this.doc.SetXmlNamespaceNode(this.pageNmsp, this.idxNmsp);
			if ((flags & XPathDocument.LoadFlags.Fragment) == XPathDocument.LoadFlags.None)
			{
				this.idxParent = this.NewNode(out this.pageParent, XPathNodeType.Root, string.Empty, string.Empty, string.Empty, baseUri);
				this.doc.SetRootNode(this.pageParent, this.idxParent);
				return;
			}
			this.doc.SetRootNode(this.nodePageFact.NextNodePage, this.nodePageFact.NextNodeIndex);
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
			if (this.atomizeNames)
			{
				prefix = this.nameTable.Add(prefix);
				localName = this.nameTable.Add(localName);
				ns = this.nameTable.Add(ns);
			}
			this.AddSibling(XPathNodeType.Element, localName, ns, prefix, baseUri);
			this.pageParent = this.pageSibling;
			this.idxParent = this.idxSibling;
			this.idxSibling = 0;
			int num = this.pageParent[this.idxParent].LocalNameHashCode & 63;
			this.elemNameIndex[num] = this.LinkSimilarElements(this.elemNameIndex[num].Page, this.elemNameIndex[num].Index, this.pageParent, this.idxParent);
			if (this.elemIdMap != null)
			{
				this.idAttrName = (XmlQualifiedName)this.elemIdMap[new XmlQualifiedName(localName, prefix)];
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
			if (!this.pageParent[this.idxParent].HasContentChild)
			{
				TextBlockType textType = this.textBldr.TextType;
				if (textType == TextBlockType.Text)
				{
					if (this.lineInfo != null)
					{
						if (this.textBldr.LineNumber != this.pageParent[this.idxParent].LineNumber)
						{
							goto IL_00CD;
						}
						int num = this.textBldr.LinePosition - this.pageParent[this.idxParent].LinePosition;
						if (num < 0 || num > 255)
						{
							goto IL_00CD;
						}
						this.pageParent[this.idxParent].SetCollapsedLineInfoOffset(num);
					}
					this.pageParent[this.idxParent].SetCollapsedValue(this.textBldr.ReadText());
					goto IL_012D;
				}
				if (textType - TextBlockType.SignificantWhitespace > 1)
				{
					this.pageParent[this.idxParent].SetEmptyValue(allowShortcutTag);
					goto IL_012D;
				}
				IL_00CD:
				this.CachedTextNode();
				this.pageParent[this.idxParent].SetValue(this.pageSibling[this.idxSibling].Value);
			}
			else if (this.textBldr.HasText)
			{
				this.CachedTextNode();
			}
			IL_012D:
			if (this.pageParent[this.idxParent].HasNamespaceDecls)
			{
				this.doc.AddNamespace(this.pageParent, this.idxParent, this.pageNmsp, this.idxNmsp);
				XPathNodeRef xpathNodeRef = this.stkNmsp.Pop();
				this.pageNmsp = xpathNodeRef.Page;
				this.idxNmsp = xpathNodeRef.Index;
			}
			this.pageSibling = this.pageParent;
			this.idxSibling = this.idxParent;
			this.idxParent = this.pageParent[this.idxParent].GetParent(out this.pageParent);
		}

		public override void WriteStartAttribute(string prefix, string localName, string namespaceName)
		{
			if (this.atomizeNames)
			{
				prefix = this.nameTable.Add(prefix);
				localName = this.nameTable.Add(localName);
				namespaceName = this.nameTable.Add(namespaceName);
			}
			this.AddSibling(XPathNodeType.Attribute, localName, namespaceName, prefix, string.Empty);
		}

		public override void WriteEndAttribute()
		{
			this.pageSibling[this.idxSibling].SetValue(this.textBldr.ReadText());
			if (this.idAttrName != null && this.pageSibling[this.idxSibling].LocalName == this.idAttrName.Name && this.pageSibling[this.idxSibling].Prefix == this.idAttrName.Namespace)
			{
				this.doc.AddIdElement(this.pageSibling[this.idxSibling].Value, this.pageParent, this.idxParent);
			}
		}

		public override void WriteCData(string text)
		{
			this.WriteString(text, TextBlockType.Text);
		}

		public override void WriteComment(string text)
		{
			this.AddSibling(XPathNodeType.Comment, string.Empty, string.Empty, string.Empty, string.Empty);
			this.pageSibling[this.idxSibling].SetValue(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.WriteProcessingInstruction(name, text, string.Empty);
		}

		public void WriteProcessingInstruction(string name, string text, string baseUri)
		{
			if (this.atomizeNames)
			{
				name = this.nameTable.Add(name);
			}
			this.AddSibling(XPathNodeType.ProcessingInstruction, name, string.Empty, string.Empty, baseUri);
			this.pageSibling[this.idxSibling].SetValue(text);
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
			this.textBldr.WriteTextBlock(text, textType);
		}

		public override void WriteEntityRef(string name)
		{
			throw new NotImplementedException();
		}

		public override void WriteCharEntity(char ch)
		{
			char[] array = new char[] { ch };
			this.WriteString(new string(array), TextBlockType.Text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			char[] array = new char[] { highChar, lowChar };
			this.WriteString(new string(array), TextBlockType.Text);
		}

		public override void Close()
		{
			if (this.textBldr.HasText)
			{
				this.CachedTextNode();
			}
			XPathNode[] array;
			if (this.doc.GetRootNode(out array) == this.nodePageFact.NextNodeIndex && array == this.nodePageFact.NextNodePage)
			{
				this.AddSibling(XPathNodeType.Text, string.Empty, string.Empty, string.Empty, string.Empty);
				this.pageSibling[this.idxSibling].SetValue(string.Empty);
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
			if (this.atomizeNames)
			{
				prefix = this.nameTable.Add(prefix);
			}
			namespaceName = this.nameTable.Add(namespaceName);
			XPathNode[] array = this.pageNmsp;
			int num = this.idxNmsp;
			while (num != 0 && array[num].LocalName != prefix)
			{
				num = array[num].GetSibling(out array);
			}
			XPathNode[] array2;
			int num2 = this.NewNamespaceNode(out array2, prefix, namespaceName, this.pageParent, this.idxParent);
			if (num != 0)
			{
				XPathNode[] array3 = this.pageNmsp;
				int sibling = this.idxNmsp;
				XPathNode[] array4 = array2;
				int num3 = num2;
				while (sibling != num || array3 != array)
				{
					XPathNode[] array5;
					int num4 = array3[sibling].GetParent(out array5);
					num4 = this.NewNamespaceNode(out array5, array3[sibling].LocalName, array3[sibling].Value, array5, num4);
					array4[num3].SetSibling(this.infoTable, array5, num4);
					array4 = array5;
					num3 = num4;
					sibling = array3[sibling].GetSibling(out array3);
				}
				num = array[num].GetSibling(out array);
				if (num != 0)
				{
					array4[num3].SetSibling(this.infoTable, array, num);
				}
			}
			else if (this.idxParent != 0)
			{
				array2[num2].SetSibling(this.infoTable, this.pageNmsp, this.idxNmsp);
			}
			else
			{
				this.doc.SetRootNode(array2, num2);
			}
			if (this.idxParent != 0)
			{
				if (!this.pageParent[this.idxParent].HasNamespaceDecls)
				{
					this.stkNmsp.Push(new XPathNodeRef(this.pageNmsp, this.idxNmsp));
					this.pageParent[this.idxParent].HasNamespaceDecls = true;
				}
				this.pageNmsp = array2;
				this.idxNmsp = num2;
			}
		}

		public void CreateIdTables(IDtdInfo dtdInfo)
		{
			foreach (IDtdAttributeListInfo dtdAttributeListInfo in dtdInfo.GetAttributeLists())
			{
				IDtdAttributeInfo dtdAttributeInfo = dtdAttributeListInfo.LookupIdAttribute();
				if (dtdAttributeInfo != null)
				{
					if (this.elemIdMap == null)
					{
						this.elemIdMap = new Hashtable();
					}
					this.elemIdMap.Add(new XmlQualifiedName(dtdAttributeListInfo.LocalName, dtdAttributeListInfo.Prefix), new XmlQualifiedName(dtdAttributeInfo.LocalName, dtdAttributeInfo.Prefix));
				}
			}
		}

		private XPathNodeRef LinkSimilarElements(XPathNode[] pagePrev, int idxPrev, XPathNode[] pageNext, int idxNext)
		{
			if (pagePrev != null)
			{
				pagePrev[idxPrev].SetSimilarElement(this.infoTable, pageNext, idxNext);
			}
			return new XPathNodeRef(pageNext, idxNext);
		}

		private int NewNamespaceNode(out XPathNode[] page, string prefix, string namespaceUri, XPathNode[] pageElem, int idxElem)
		{
			XPathNode[] array;
			int num;
			this.nmspPageFact.AllocateSlot(out array, out num);
			int num2;
			int num3;
			this.ComputeLineInfo(false, out num2, out num3);
			XPathNodeInfoAtom xpathNodeInfoAtom = this.infoTable.Create(prefix, string.Empty, string.Empty, string.Empty, pageElem, array, null, this.doc, this.lineNumBase, this.linePosBase);
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
			this.nodePageFact.AllocateSlot(out array, out num);
			int num2;
			int num3;
			this.ComputeLineInfo(XPathNavigator.IsText(xptyp), out num2, out num3);
			XPathNodeInfoAtom xpathNodeInfoAtom = this.infoTable.Create(localName, namespaceUri, prefix, baseUri, this.pageParent, array, array, this.doc, this.lineNumBase, this.linePosBase);
			array[num].Create(xpathNodeInfoAtom, xptyp, this.idxParent);
			array[num].SetLineInfoOffsets(num2, num3);
			page = array;
			return num;
		}

		private void ComputeLineInfo(bool isTextNode, out int lineNumOffset, out int linePosOffset)
		{
			if (this.lineInfo == null)
			{
				lineNumOffset = 0;
				linePosOffset = 0;
				return;
			}
			int num;
			int num2;
			if (isTextNode)
			{
				num = this.textBldr.LineNumber;
				num2 = this.textBldr.LinePosition;
			}
			else
			{
				num = this.lineInfo.LineNumber;
				num2 = this.lineInfo.LinePosition;
			}
			lineNumOffset = num - this.lineNumBase;
			if (lineNumOffset < 0 || lineNumOffset > 16383)
			{
				this.lineNumBase = num;
				lineNumOffset = 0;
			}
			linePosOffset = num2 - this.linePosBase;
			if (linePosOffset < 0 || linePosOffset > 65535)
			{
				this.linePosBase = num2;
				linePosOffset = 0;
			}
		}

		private void AddSibling(XPathNodeType xptyp, string localName, string namespaceUri, string prefix, string baseUri)
		{
			if (this.textBldr.HasText)
			{
				this.CachedTextNode();
			}
			XPathNode[] array;
			int num = this.NewNode(out array, xptyp, localName, namespaceUri, prefix, baseUri);
			if (this.idxParent != 0)
			{
				this.pageParent[this.idxParent].SetParentProperties(xptyp);
				if (this.idxSibling != 0)
				{
					this.pageSibling[this.idxSibling].SetSibling(this.infoTable, array, num);
				}
			}
			this.pageSibling = array;
			this.idxSibling = num;
		}

		private void CachedTextNode()
		{
			TextBlockType textType = this.textBldr.TextType;
			string text = this.textBldr.ReadText();
			this.AddSibling((XPathNodeType)textType, string.Empty, string.Empty, string.Empty, string.Empty);
			this.pageSibling[this.idxSibling].SetValue(text);
		}

		private XPathDocumentBuilder.NodePageFactory nodePageFact;

		private XPathDocumentBuilder.NodePageFactory nmspPageFact;

		private XPathDocumentBuilder.TextBlockBuilder textBldr;

		private Stack<XPathNodeRef> stkNmsp;

		private XPathNodeInfoTable infoTable;

		private XPathDocument doc;

		private IXmlLineInfo lineInfo;

		private XmlNameTable nameTable;

		private bool atomizeNames;

		private XPathNode[] pageNmsp;

		private int idxNmsp;

		private XPathNode[] pageParent;

		private int idxParent;

		private XPathNode[] pageSibling;

		private int idxSibling;

		private int lineNumBase;

		private int linePosBase;

		private XmlQualifiedName idAttrName;

		private Hashtable elemIdMap;

		private XPathNodeRef[] elemNameIndex;

		private const int ElementIndexSize = 64;

		private struct NodePageFactory
		{
			public void Init(int initialPageSize)
			{
				this.pageSize = initialPageSize;
				this.page = new XPathNode[this.pageSize];
				this.pageInfo = new XPathNodePageInfo(null, 1);
				this.page[0].Create(this.pageInfo);
			}

			public XPathNode[] NextNodePage
			{
				get
				{
					return this.page;
				}
			}

			public int NextNodeIndex
			{
				get
				{
					return this.pageInfo.NodeCount;
				}
			}

			public void AllocateSlot(out XPathNode[] page, out int idx)
			{
				page = this.page;
				idx = this.pageInfo.NodeCount;
				XPathNodePageInfo xpathNodePageInfo = this.pageInfo;
				int num = xpathNodePageInfo.NodeCount + 1;
				xpathNodePageInfo.NodeCount = num;
				if (num >= this.page.Length)
				{
					if (this.pageSize < 65536)
					{
						this.pageSize *= 2;
					}
					this.page = new XPathNode[this.pageSize];
					this.pageInfo.NextPage = this.page;
					this.pageInfo = new XPathNodePageInfo(page, this.pageInfo.PageNumber + 1);
					this.page[0].Create(this.pageInfo);
				}
			}

			private XPathNode[] page;

			private XPathNodePageInfo pageInfo;

			private int pageSize;
		}

		private struct TextBlockBuilder
		{
			public void Initialize(IXmlLineInfo lineInfo)
			{
				this.lineInfo = lineInfo;
				this.textType = TextBlockType.None;
			}

			public TextBlockType TextType
			{
				get
				{
					return this.textType;
				}
			}

			public bool HasText
			{
				get
				{
					return this.textType > TextBlockType.None;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNum;
				}
			}

			public int LinePosition
			{
				get
				{
					return this.linePos;
				}
			}

			public void WriteTextBlock(string text, TextBlockType textType)
			{
				if (text.Length != 0)
				{
					if (this.textType == TextBlockType.None)
					{
						this.text = text;
						this.textType = textType;
						if (this.lineInfo != null)
						{
							this.lineNum = this.lineInfo.LineNumber;
							this.linePos = this.lineInfo.LinePosition;
							return;
						}
					}
					else
					{
						this.text += text;
						if (textType < this.textType)
						{
							this.textType = textType;
						}
					}
				}
			}

			public string ReadText()
			{
				if (this.textType == TextBlockType.None)
				{
					return string.Empty;
				}
				this.textType = TextBlockType.None;
				return this.text;
			}

			private IXmlLineInfo lineInfo;

			private TextBlockType textType;

			private string text;

			private int lineNum;

			private int linePos;
		}
	}
}
