using System;

namespace System.Xml.Linq
{
	internal class XNodeReader : XmlReader, IXmlLineInfo
	{
		internal XNodeReader(XNode node, XmlNameTable nameTable, ReaderOptions options)
		{
			this._source = node;
			this._root = node;
			this._nameTable = ((nameTable != null) ? nameTable : XNodeReader.CreateNameTable());
			this._omitDuplicateNamespaces = (options & ReaderOptions.OmitDuplicateNamespaces) != ReaderOptions.None;
		}

		internal XNodeReader(XNode node, XmlNameTable nameTable)
			: this(node, nameTable, ((node.GetSaveOptionsFromAnnotations() & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None) ? ReaderOptions.OmitDuplicateNamespaces : ReaderOptions.None)
		{
		}

		public override int AttributeCount
		{
			get
			{
				if (!this.IsInteractive)
				{
					return 0;
				}
				int num = 0;
				XElement elementInAttributeScope = this.GetElementInAttributeScope();
				if (elementInAttributeScope != null)
				{
					XAttribute xattribute = elementInAttributeScope.lastAttr;
					if (xattribute != null)
					{
						do
						{
							xattribute = xattribute.next;
							if (!this._omitDuplicateNamespaces || !this.IsDuplicateNamespaceAttribute(xattribute))
							{
								num++;
							}
						}
						while (xattribute != elementInAttributeScope.lastAttr);
					}
				}
				return num;
			}
		}

		public override string BaseURI
		{
			get
			{
				XObject xobject = this._source as XObject;
				if (xobject != null)
				{
					return xobject.BaseUri;
				}
				xobject = this._parent as XObject;
				if (xobject != null)
				{
					return xobject.BaseUri;
				}
				return string.Empty;
			}
		}

		public override int Depth
		{
			get
			{
				if (!this.IsInteractive)
				{
					return 0;
				}
				XObject xobject = this._source as XObject;
				if (xobject != null)
				{
					return XNodeReader.GetDepth(xobject);
				}
				xobject = this._parent as XObject;
				if (xobject != null)
				{
					return XNodeReader.GetDepth(xobject) + 1;
				}
				return 0;
			}
		}

		private static int GetDepth(XObject o)
		{
			int num = 0;
			while (o.parent != null)
			{
				num++;
				o = o.parent;
			}
			if (o is XDocument)
			{
				num--;
			}
			return num;
		}

		public override bool EOF
		{
			get
			{
				return this._state == ReadState.EndOfFile;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				if (!this.IsInteractive)
				{
					return false;
				}
				XElement elementInAttributeScope = this.GetElementInAttributeScope();
				return elementInAttributeScope != null && elementInAttributeScope.lastAttr != null && (!this._omitDuplicateNamespaces || this.GetFirstNonDuplicateNamespaceAttribute(elementInAttributeScope.lastAttr.next) != null);
			}
		}

		public override bool HasValue
		{
			get
			{
				if (!this.IsInteractive)
				{
					return false;
				}
				XObject xobject = this._source as XObject;
				if (xobject != null)
				{
					switch (xobject.NodeType)
					{
					case XmlNodeType.Attribute:
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
					case XmlNodeType.ProcessingInstruction:
					case XmlNodeType.Comment:
					case XmlNodeType.DocumentType:
						return true;
					}
					return false;
				}
				return true;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				if (!this.IsInteractive)
				{
					return false;
				}
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
			if (!this.IsInteractive)
			{
				return string.Empty;
			}
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				return xelement.Name.LocalName;
			}
			XAttribute xattribute = this._source as XAttribute;
			if (xattribute != null)
			{
				return xattribute.Name.LocalName;
			}
			XProcessingInstruction xprocessingInstruction = this._source as XProcessingInstruction;
			if (xprocessingInstruction != null)
			{
				return xprocessingInstruction.Target;
			}
			XDocumentType xdocumentType = this._source as XDocumentType;
			if (xdocumentType != null)
			{
				return xdocumentType.Name;
			}
			return string.Empty;
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
			if (!this.IsInteractive)
			{
				return string.Empty;
			}
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
			string namespaceName = xattribute.Name.NamespaceName;
			if (namespaceName.Length == 0 && xattribute.Name.LocalName == "xmlns")
			{
				return "http://www.w3.org/2000/xmlns/";
			}
			return namespaceName;
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this._nameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (!this.IsInteractive)
				{
					return XmlNodeType.None;
				}
				XObject xobject = this._source as XObject;
				if (xobject != null)
				{
					if (this.IsEndElement)
					{
						return XmlNodeType.EndElement;
					}
					XmlNodeType nodeType = xobject.NodeType;
					if (nodeType != XmlNodeType.Text)
					{
						return nodeType;
					}
					if (xobject.parent != null && xobject.parent.parent == null && xobject.parent is XDocument)
					{
						return XmlNodeType.Whitespace;
					}
					return XmlNodeType.Text;
				}
				else
				{
					if (this._parent is XDocument)
					{
						return XmlNodeType.Whitespace;
					}
					return XmlNodeType.Text;
				}
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
			if (!this.IsInteractive)
			{
				return string.Empty;
			}
			XElement xelement = this._source as XElement;
			if (xelement == null)
			{
				XAttribute xattribute = this._source as XAttribute;
				if (xattribute != null)
				{
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

		public override ReadState ReadState
		{
			get
			{
				return this._state;
			}
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				return new XmlReaderSettings
				{
					CheckCharacters = false
				};
			}
		}

		public override string Value
		{
			get
			{
				if (!this.IsInteractive)
				{
					return string.Empty;
				}
				XObject xobject = this._source as XObject;
				if (xobject != null)
				{
					switch (xobject.NodeType)
					{
					case XmlNodeType.Attribute:
						return ((XAttribute)xobject).Value;
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						return ((XText)xobject).Value;
					case XmlNodeType.ProcessingInstruction:
						return ((XProcessingInstruction)xobject).Data;
					case XmlNodeType.Comment:
						return ((XComment)xobject).Value;
					case XmlNodeType.DocumentType:
						return ((XDocumentType)xobject).InternalSubset;
					}
					return string.Empty;
				}
				return (string)this._source;
			}
		}

		public override string XmlLang
		{
			get
			{
				if (!this.IsInteractive)
				{
					return string.Empty;
				}
				XElement xelement = this.GetElementInScope();
				if (xelement != null)
				{
					XName name = XNamespace.Xml.GetName("lang");
					XAttribute xattribute;
					for (;;)
					{
						xattribute = xelement.Attribute(name);
						if (xattribute != null)
						{
							break;
						}
						xelement = xelement.parent as XElement;
						if (xelement == null)
						{
							goto IL_0049;
						}
					}
					return xattribute.Value;
				}
				IL_0049:
				return string.Empty;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				if (!this.IsInteractive)
				{
					return XmlSpace.None;
				}
				XElement xelement = this.GetElementInScope();
				if (xelement != null)
				{
					XName name = XNamespace.Xml.GetName("space");
					for (;;)
					{
						XAttribute xattribute = xelement.Attribute(name);
						if (xattribute != null)
						{
							string text = xattribute.Value.Trim(XNodeReader.s_WhitespaceChars);
							if (text == "preserve")
							{
								break;
							}
							if (text == "default")
							{
								return XmlSpace.Default;
							}
						}
						xelement = xelement.parent as XElement;
						if (xelement == null)
						{
							return XmlSpace.None;
						}
					}
					return XmlSpace.Preserve;
				}
				return XmlSpace.None;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.ReadState != ReadState.Closed)
			{
				this.Close();
			}
		}

		public override void Close()
		{
			this._source = null;
			this._parent = null;
			this._root = null;
			this._state = ReadState.Closed;
		}

		public override string GetAttribute(string name)
		{
			if (!this.IsInteractive)
			{
				return null;
			}
			XElement elementInAttributeScope = this.GetElementInAttributeScope();
			if (elementInAttributeScope != null)
			{
				string text;
				string text2;
				XNodeReader.GetNameInAttributeScope(name, elementInAttributeScope, out text, out text2);
				XAttribute xattribute = elementInAttributeScope.lastAttr;
				if (xattribute != null)
				{
					for (;;)
					{
						xattribute = xattribute.next;
						if (xattribute.Name.LocalName == text && xattribute.Name.NamespaceName == text2)
						{
							break;
						}
						if (xattribute == elementInAttributeScope.lastAttr)
						{
							goto IL_0082;
						}
					}
					if (this._omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(xattribute))
					{
						return null;
					}
					return xattribute.Value;
				}
				IL_0082:
				return null;
			}
			XDocumentType xdocumentType = this._source as XDocumentType;
			if (xdocumentType != null)
			{
				if (name == "PUBLIC")
				{
					return xdocumentType.PublicId;
				}
				if (name == "SYSTEM")
				{
					return xdocumentType.SystemId;
				}
			}
			return null;
		}

		public override string GetAttribute(string localName, string namespaceName)
		{
			if (!this.IsInteractive)
			{
				return null;
			}
			XElement elementInAttributeScope = this.GetElementInAttributeScope();
			if (elementInAttributeScope != null)
			{
				if (localName == "xmlns")
				{
					if (namespaceName != null && namespaceName.Length == 0)
					{
						return null;
					}
					if (namespaceName == "http://www.w3.org/2000/xmlns/")
					{
						namespaceName = string.Empty;
					}
				}
				XAttribute xattribute = elementInAttributeScope.lastAttr;
				if (xattribute != null)
				{
					for (;;)
					{
						xattribute = xattribute.next;
						if (xattribute.Name.LocalName == localName && xattribute.Name.NamespaceName == namespaceName)
						{
							break;
						}
						if (xattribute == elementInAttributeScope.lastAttr)
						{
							goto IL_009F;
						}
					}
					if (this._omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(xattribute))
					{
						return null;
					}
					return xattribute.Value;
				}
			}
			IL_009F:
			return null;
		}

		public override string GetAttribute(int index)
		{
			if (!this.IsInteractive)
			{
				return null;
			}
			if (index < 0)
			{
				return null;
			}
			XElement elementInAttributeScope = this.GetElementInAttributeScope();
			if (elementInAttributeScope != null)
			{
				XAttribute xattribute = elementInAttributeScope.lastAttr;
				if (xattribute != null)
				{
					for (;;)
					{
						xattribute = xattribute.next;
						if ((!this._omitDuplicateNamespaces || !this.IsDuplicateNamespaceAttribute(xattribute)) && index-- == 0)
						{
							break;
						}
						if (xattribute == elementInAttributeScope.lastAttr)
						{
							goto IL_0054;
						}
					}
					return xattribute.Value;
				}
			}
			IL_0054:
			return null;
		}

		public override string LookupNamespace(string prefix)
		{
			if (!this.IsInteractive)
			{
				return null;
			}
			if (prefix == null)
			{
				return null;
			}
			XElement elementInScope = this.GetElementInScope();
			if (elementInScope != null)
			{
				XNamespace xnamespace = ((prefix.Length == 0) ? elementInScope.GetDefaultNamespace() : elementInScope.GetNamespaceOfPrefix(prefix));
				if (xnamespace != null)
				{
					return this._nameTable.Add(xnamespace.NamespaceName);
				}
			}
			return null;
		}

		public override bool MoveToAttribute(string name)
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			XElement elementInAttributeScope = this.GetElementInAttributeScope();
			if (elementInAttributeScope != null)
			{
				string text;
				string text2;
				XNodeReader.GetNameInAttributeScope(name, elementInAttributeScope, out text, out text2);
				XAttribute xattribute = elementInAttributeScope.lastAttr;
				if (xattribute != null)
				{
					for (;;)
					{
						xattribute = xattribute.next;
						if (xattribute.Name.LocalName == text && xattribute.Name.NamespaceName == text2)
						{
							break;
						}
						if (xattribute == elementInAttributeScope.lastAttr)
						{
							return false;
						}
					}
					if (this._omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(xattribute))
					{
						return false;
					}
					this._source = xattribute;
					this._parent = null;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			XElement elementInAttributeScope = this.GetElementInAttributeScope();
			if (elementInAttributeScope != null)
			{
				if (localName == "xmlns")
				{
					if (namespaceName != null && namespaceName.Length == 0)
					{
						return false;
					}
					if (namespaceName == "http://www.w3.org/2000/xmlns/")
					{
						namespaceName = string.Empty;
					}
				}
				XAttribute xattribute = elementInAttributeScope.lastAttr;
				if (xattribute != null)
				{
					for (;;)
					{
						xattribute = xattribute.next;
						if (xattribute.Name.LocalName == localName && xattribute.Name.NamespaceName == namespaceName)
						{
							break;
						}
						if (xattribute == elementInAttributeScope.lastAttr)
						{
							return false;
						}
					}
					if (this._omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(xattribute))
					{
						return false;
					}
					this._source = xattribute;
					this._parent = null;
					return true;
				}
			}
			return false;
		}

		public override void MoveToAttribute(int index)
		{
			if (!this.IsInteractive)
			{
				return;
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			XElement elementInAttributeScope = this.GetElementInAttributeScope();
			if (elementInAttributeScope != null)
			{
				XAttribute xattribute = elementInAttributeScope.lastAttr;
				if (xattribute != null)
				{
					for (;;)
					{
						xattribute = xattribute.next;
						if ((!this._omitDuplicateNamespaces || !this.IsDuplicateNamespaceAttribute(xattribute)) && index-- == 0)
						{
							break;
						}
						if (xattribute == elementInAttributeScope.lastAttr)
						{
							goto IL_0064;
						}
					}
					this._source = xattribute;
					this._parent = null;
					return;
				}
			}
			IL_0064:
			throw new ArgumentOutOfRangeException("index");
		}

		public override bool MoveToElement()
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			XAttribute xattribute = this._source as XAttribute;
			if (xattribute == null)
			{
				xattribute = this._parent as XAttribute;
			}
			if (xattribute != null && xattribute.parent != null)
			{
				this._source = xattribute.parent;
				this._parent = null;
				return true;
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			XElement elementInAttributeScope = this.GetElementInAttributeScope();
			if (elementInAttributeScope != null && elementInAttributeScope.lastAttr != null)
			{
				if (this._omitDuplicateNamespaces)
				{
					object firstNonDuplicateNamespaceAttribute = this.GetFirstNonDuplicateNamespaceAttribute(elementInAttributeScope.lastAttr.next);
					if (firstNonDuplicateNamespaceAttribute == null)
					{
						return false;
					}
					this._source = firstNonDuplicateNamespaceAttribute;
				}
				else
				{
					this._source = elementInAttributeScope.lastAttr.next;
				}
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				if (this.IsEndElement)
				{
					return false;
				}
				if (xelement.lastAttr != null)
				{
					if (this._omitDuplicateNamespaces)
					{
						object firstNonDuplicateNamespaceAttribute = this.GetFirstNonDuplicateNamespaceAttribute(xelement.lastAttr.next);
						if (firstNonDuplicateNamespaceAttribute == null)
						{
							return false;
						}
						this._source = firstNonDuplicateNamespaceAttribute;
					}
					else
					{
						this._source = xelement.lastAttr.next;
					}
					return true;
				}
				return false;
			}
			else
			{
				XAttribute xattribute = this._source as XAttribute;
				if (xattribute == null)
				{
					xattribute = this._parent as XAttribute;
				}
				if (xattribute != null && xattribute.parent != null && ((XElement)xattribute.parent).lastAttr != xattribute)
				{
					if (this._omitDuplicateNamespaces)
					{
						object firstNonDuplicateNamespaceAttribute2 = this.GetFirstNonDuplicateNamespaceAttribute(xattribute.next);
						if (firstNonDuplicateNamespaceAttribute2 == null)
						{
							return false;
						}
						this._source = firstNonDuplicateNamespaceAttribute2;
					}
					else
					{
						this._source = xattribute.next;
					}
					this._parent = null;
					return true;
				}
				return false;
			}
		}

		public override bool Read()
		{
			ReadState state = this._state;
			if (state != ReadState.Initial)
			{
				return state == ReadState.Interactive && this.Read(false);
			}
			this._state = ReadState.Interactive;
			XDocument xdocument = this._source as XDocument;
			return xdocument == null || this.ReadIntoDocument(xdocument);
		}

		public override bool ReadAttributeValue()
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			XAttribute xattribute = this._source as XAttribute;
			return xattribute != null && this.ReadIntoAttribute(xattribute);
		}

		public override bool ReadToDescendant(string localName, string namespaceName)
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			this.MoveToElement();
			XElement xelement = this._source as XElement;
			if (xelement != null && !xelement.IsEmpty)
			{
				if (this.IsEndElement)
				{
					return false;
				}
				foreach (XElement xelement2 in xelement.Descendants())
				{
					if (xelement2.Name.LocalName == localName && xelement2.Name.NamespaceName == namespaceName)
					{
						this._source = xelement2;
						return true;
					}
				}
				this.IsEndElement = true;
				return false;
			}
			return false;
		}

		public override bool ReadToFollowing(string localName, string namespaceName)
		{
			while (this.Read())
			{
				XElement xelement = this._source as XElement;
				if (xelement != null && !this.IsEndElement && xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName)
				{
					return true;
				}
			}
			return false;
		}

		public override bool ReadToNextSibling(string localName, string namespaceName)
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			this.MoveToElement();
			if (this._source != this._root)
			{
				XNode xnode = this._source as XNode;
				if (xnode != null)
				{
					foreach (XElement xelement in xnode.ElementsAfterSelf())
					{
						if (xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName)
						{
							this._source = xelement;
							this.IsEndElement = false;
							return true;
						}
					}
					if (xnode.parent is XElement)
					{
						this._source = xnode.parent;
						this.IsEndElement = true;
						return false;
					}
					goto IL_00E0;
				}
				if (this._parent is XElement)
				{
					this._source = this._parent;
					this._parent = null;
					this.IsEndElement = true;
					return false;
				}
			}
			IL_00E0:
			return this.ReadToEnd();
		}

		public override void ResolveEntity()
		{
		}

		public override void Skip()
		{
			if (!this.IsInteractive)
			{
				return;
			}
			this.Read(true);
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			if (this.IsEndElement)
			{
				XElement xelement = this._source as XElement;
				if (xelement != null)
				{
					return xelement.Annotation<LineInfoEndElementAnnotation>() != null;
				}
			}
			else
			{
				IXmlLineInfo xmlLineInfo = this._source as IXmlLineInfo;
				if (xmlLineInfo != null)
				{
					return xmlLineInfo.HasLineInfo();
				}
			}
			return false;
		}

		int IXmlLineInfo.LineNumber
		{
			get
			{
				if (this.IsEndElement)
				{
					XElement xelement = this._source as XElement;
					if (xelement != null)
					{
						LineInfoEndElementAnnotation lineInfoEndElementAnnotation = xelement.Annotation<LineInfoEndElementAnnotation>();
						if (lineInfoEndElementAnnotation != null)
						{
							return lineInfoEndElementAnnotation.lineNumber;
						}
					}
				}
				else
				{
					IXmlLineInfo xmlLineInfo = this._source as IXmlLineInfo;
					if (xmlLineInfo != null)
					{
						return xmlLineInfo.LineNumber;
					}
				}
				return 0;
			}
		}

		int IXmlLineInfo.LinePosition
		{
			get
			{
				if (this.IsEndElement)
				{
					XElement xelement = this._source as XElement;
					if (xelement != null)
					{
						LineInfoEndElementAnnotation lineInfoEndElementAnnotation = xelement.Annotation<LineInfoEndElementAnnotation>();
						if (lineInfoEndElementAnnotation != null)
						{
							return lineInfoEndElementAnnotation.linePosition;
						}
					}
				}
				else
				{
					IXmlLineInfo xmlLineInfo = this._source as IXmlLineInfo;
					if (xmlLineInfo != null)
					{
						return xmlLineInfo.LinePosition;
					}
				}
				return 0;
			}
		}

		private bool IsEndElement
		{
			get
			{
				return this._parent == this._source;
			}
			set
			{
				this._parent = (value ? this._source : null);
			}
		}

		private bool IsInteractive
		{
			get
			{
				return this._state == ReadState.Interactive;
			}
		}

		private static XmlNameTable CreateNameTable()
		{
			NameTable nameTable = new NameTable();
			nameTable.Add(string.Empty);
			nameTable.Add("http://www.w3.org/2000/xmlns/");
			nameTable.Add("http://www.w3.org/XML/1998/namespace");
			return nameTable;
		}

		private XElement GetElementInAttributeScope()
		{
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				if (this.IsEndElement)
				{
					return null;
				}
				return xelement;
			}
			else
			{
				XAttribute xattribute = this._source as XAttribute;
				if (xattribute != null)
				{
					return (XElement)xattribute.parent;
				}
				xattribute = this._parent as XAttribute;
				if (xattribute != null)
				{
					return (XElement)xattribute.parent;
				}
				return null;
			}
		}

		private XElement GetElementInScope()
		{
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				return xelement;
			}
			XNode xnode = this._source as XNode;
			if (xnode != null)
			{
				return xnode.parent as XElement;
			}
			XAttribute xattribute = this._source as XAttribute;
			if (xattribute != null)
			{
				return (XElement)xattribute.parent;
			}
			xelement = this._parent as XElement;
			if (xelement != null)
			{
				return xelement;
			}
			xattribute = this._parent as XAttribute;
			if (xattribute != null)
			{
				return (XElement)xattribute.parent;
			}
			return null;
		}

		private static void GetNameInAttributeScope(string qualifiedName, XElement e, out string localName, out string namespaceName)
		{
			if (!string.IsNullOrEmpty(qualifiedName))
			{
				int num = qualifiedName.IndexOf(':');
				if (num != 0 && num != qualifiedName.Length - 1)
				{
					if (num == -1)
					{
						localName = qualifiedName;
						namespaceName = string.Empty;
						return;
					}
					XNamespace namespaceOfPrefix = e.GetNamespaceOfPrefix(qualifiedName.Substring(0, num));
					if (namespaceOfPrefix != null)
					{
						localName = qualifiedName.Substring(num + 1, qualifiedName.Length - num - 1);
						namespaceName = namespaceOfPrefix.NamespaceName;
						return;
					}
				}
			}
			localName = null;
			namespaceName = null;
		}

		private bool Read(bool skipContent)
		{
			XElement xelement = this._source as XElement;
			if (xelement != null)
			{
				if (xelement.IsEmpty || this.IsEndElement || skipContent)
				{
					return this.ReadOverNode(xelement);
				}
				return this.ReadIntoElement(xelement);
			}
			else
			{
				XNode xnode = this._source as XNode;
				if (xnode != null)
				{
					return this.ReadOverNode(xnode);
				}
				XAttribute xattribute = this._source as XAttribute;
				if (xattribute != null)
				{
					return this.ReadOverAttribute(xattribute, skipContent);
				}
				return this.ReadOverText(skipContent);
			}
		}

		private bool ReadIntoDocument(XDocument d)
		{
			XNode xnode = d.content as XNode;
			if (xnode != null)
			{
				this._source = xnode.next;
				return true;
			}
			string text = d.content as string;
			if (text != null && text.Length > 0)
			{
				this._source = text;
				this._parent = d;
				return true;
			}
			return this.ReadToEnd();
		}

		private bool ReadIntoElement(XElement e)
		{
			XNode xnode = e.content as XNode;
			if (xnode != null)
			{
				this._source = xnode.next;
				return true;
			}
			string text = e.content as string;
			if (text != null)
			{
				if (text.Length > 0)
				{
					this._source = text;
					this._parent = e;
				}
				else
				{
					this._source = e;
					this.IsEndElement = true;
				}
				return true;
			}
			return this.ReadToEnd();
		}

		private bool ReadIntoAttribute(XAttribute a)
		{
			this._source = a.value;
			this._parent = a;
			return true;
		}

		private bool ReadOverAttribute(XAttribute a, bool skipContent)
		{
			XElement xelement = (XElement)a.parent;
			if (xelement == null)
			{
				return this.ReadToEnd();
			}
			if (xelement.IsEmpty || skipContent)
			{
				return this.ReadOverNode(xelement);
			}
			return this.ReadIntoElement(xelement);
		}

		private bool ReadOverNode(XNode n)
		{
			if (n == this._root)
			{
				return this.ReadToEnd();
			}
			XNode next = n.next;
			if (next == null || next == n || n == n.parent.content)
			{
				if (n.parent == null || (n.parent.parent == null && n.parent is XDocument))
				{
					return this.ReadToEnd();
				}
				this._source = n.parent;
				this.IsEndElement = true;
			}
			else
			{
				this._source = next;
				this.IsEndElement = false;
			}
			return true;
		}

		private bool ReadOverText(bool skipContent)
		{
			if (this._parent is XElement)
			{
				this._source = this._parent;
				this._parent = null;
				this.IsEndElement = true;
				return true;
			}
			XAttribute xattribute = this._parent as XAttribute;
			if (xattribute != null)
			{
				this._parent = null;
				return this.ReadOverAttribute(xattribute, skipContent);
			}
			return this.ReadToEnd();
		}

		private bool ReadToEnd()
		{
			this._state = ReadState.EndOfFile;
			return false;
		}

		private bool IsDuplicateNamespaceAttribute(XAttribute candidateAttribute)
		{
			return candidateAttribute.IsNamespaceDeclaration && this.IsDuplicateNamespaceAttributeInner(candidateAttribute);
		}

		private bool IsDuplicateNamespaceAttributeInner(XAttribute candidateAttribute)
		{
			if (candidateAttribute.Name.LocalName == "xml")
			{
				return true;
			}
			XElement xelement = candidateAttribute.parent as XElement;
			if (xelement == this._root || xelement == null)
			{
				return false;
			}
			for (xelement = xelement.parent as XElement; xelement != null; xelement = xelement.parent as XElement)
			{
				XAttribute xattribute = xelement.lastAttr;
				if (xattribute != null)
				{
					while (!(xattribute.name == candidateAttribute.name))
					{
						xattribute = xattribute.next;
						if (xattribute == xelement.lastAttr)
						{
							goto IL_0085;
						}
					}
					return xattribute.Value == candidateAttribute.Value;
				}
				IL_0085:
				if (xelement == this._root)
				{
					return false;
				}
			}
			return false;
		}

		private XAttribute GetFirstNonDuplicateNamespaceAttribute(XAttribute candidate)
		{
			if (!this.IsDuplicateNamespaceAttribute(candidate))
			{
				return candidate;
			}
			XElement xelement = candidate.parent as XElement;
			if (xelement != null && candidate != xelement.lastAttr)
			{
				for (;;)
				{
					candidate = candidate.next;
					if (!this.IsDuplicateNamespaceAttribute(candidate))
					{
						break;
					}
					if (candidate == xelement.lastAttr)
					{
						goto IL_003F;
					}
				}
				return candidate;
			}
			IL_003F:
			return null;
		}

		private static readonly char[] s_WhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

		private object _source;

		private object _parent;

		private ReadState _state;

		private XNode _root;

		private XmlNameTable _nameTable;

		private bool _omitDuplicateNamespaces;
	}
}
