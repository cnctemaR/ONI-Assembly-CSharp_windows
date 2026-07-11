using System;

namespace System.Xml.Linq
{
	internal class XNodeReader : XmlReader, IXmlLineInfo
	{
		internal XNodeReader(XNode node, XmlNameTable nameTable, ReaderOptions options)
		{
			this.source = node;
			this.root = node;
			this.nameTable = ((nameTable != null) ? nameTable : XNodeReader.CreateNameTable());
			this.omitDuplicateNamespaces = (options & ReaderOptions.OmitDuplicateNamespaces) != ReaderOptions.None;
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
							if (!this.omitDuplicateNamespaces || !this.IsDuplicateNamespaceAttribute(xattribute))
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
				XObject xobject = this.source as XObject;
				if (xobject != null)
				{
					return xobject.BaseUri;
				}
				xobject = this.parent as XObject;
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
				XObject xobject = this.source as XObject;
				if (xobject != null)
				{
					return XNodeReader.GetDepth(xobject);
				}
				xobject = this.parent as XObject;
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
				return this.state == ReadState.EndOfFile;
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
				return elementInAttributeScope != null && elementInAttributeScope.lastAttr != null && (!this.omitDuplicateNamespaces || this.GetFirstNonDuplicateNamespaceAttribute(elementInAttributeScope.lastAttr.next) != null);
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
				XObject xobject = this.source as XObject;
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
			if (!this.IsInteractive)
			{
				return string.Empty;
			}
			XElement xelement = this.source as XElement;
			if (xelement != null)
			{
				return xelement.Name.LocalName;
			}
			XAttribute xattribute = this.source as XAttribute;
			if (xattribute != null)
			{
				return xattribute.Name.LocalName;
			}
			XProcessingInstruction xprocessingInstruction = this.source as XProcessingInstruction;
			if (xprocessingInstruction != null)
			{
				return xprocessingInstruction.Target;
			}
			XDocumentType xdocumentType = this.source as XDocumentType;
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
			if (!this.IsInteractive)
			{
				return string.Empty;
			}
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
				return this.nameTable;
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
				XObject xobject = this.source as XObject;
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
					if (this.parent is XDocument)
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
				return this.nameTable.Add(this.GetPrefix());
			}
		}

		private string GetPrefix()
		{
			if (!this.IsInteractive)
			{
				return string.Empty;
			}
			XElement xelement = this.source as XElement;
			if (xelement == null)
			{
				XAttribute xattribute = this.source as XAttribute;
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
				return this.state;
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
				XObject xobject = this.source as XObject;
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
				return (string)this.source;
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
							string text = xattribute.Value.Trim(new char[] { ' ', '\t', '\n', '\r' });
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

		public override void Close()
		{
			this.source = null;
			this.parent = null;
			this.root = null;
			this.state = ReadState.Closed;
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
					if (this.omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(xattribute))
					{
						return null;
					}
					return xattribute.Value;
				}
				IL_0082:
				return null;
			}
			XDocumentType xdocumentType = this.source as XDocumentType;
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
					if (this.omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(xattribute))
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
						if ((!this.omitDuplicateNamespaces || !this.IsDuplicateNamespaceAttribute(xattribute)) && index-- == 0)
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
					return this.nameTable.Add(xnamespace.NamespaceName);
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
					if (this.omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(xattribute))
					{
						return false;
					}
					this.source = xattribute;
					this.parent = null;
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
					if (this.omitDuplicateNamespaces && this.IsDuplicateNamespaceAttribute(xattribute))
					{
						return false;
					}
					this.source = xattribute;
					this.parent = null;
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
						if ((!this.omitDuplicateNamespaces || !this.IsDuplicateNamespaceAttribute(xattribute)) && index-- == 0)
						{
							break;
						}
						if (xattribute == elementInAttributeScope.lastAttr)
						{
							goto IL_0064;
						}
					}
					this.source = xattribute;
					this.parent = null;
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
			XAttribute xattribute = this.source as XAttribute;
			if (xattribute == null)
			{
				xattribute = this.parent as XAttribute;
			}
			if (xattribute != null && xattribute.parent != null)
			{
				this.source = xattribute.parent;
				this.parent = null;
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
				if (this.omitDuplicateNamespaces)
				{
					object firstNonDuplicateNamespaceAttribute = this.GetFirstNonDuplicateNamespaceAttribute(elementInAttributeScope.lastAttr.next);
					if (firstNonDuplicateNamespaceAttribute == null)
					{
						return false;
					}
					this.source = firstNonDuplicateNamespaceAttribute;
				}
				else
				{
					this.source = elementInAttributeScope.lastAttr.next;
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
			XElement xelement = this.source as XElement;
			if (xelement != null)
			{
				if (this.IsEndElement)
				{
					return false;
				}
				if (xelement.lastAttr != null)
				{
					if (this.omitDuplicateNamespaces)
					{
						object firstNonDuplicateNamespaceAttribute = this.GetFirstNonDuplicateNamespaceAttribute(xelement.lastAttr.next);
						if (firstNonDuplicateNamespaceAttribute == null)
						{
							return false;
						}
						this.source = firstNonDuplicateNamespaceAttribute;
					}
					else
					{
						this.source = xelement.lastAttr.next;
					}
					return true;
				}
				return false;
			}
			else
			{
				XAttribute xattribute = this.source as XAttribute;
				if (xattribute == null)
				{
					xattribute = this.parent as XAttribute;
				}
				if (xattribute != null && xattribute.parent != null && ((XElement)xattribute.parent).lastAttr != xattribute)
				{
					if (this.omitDuplicateNamespaces)
					{
						object firstNonDuplicateNamespaceAttribute2 = this.GetFirstNonDuplicateNamespaceAttribute(xattribute.next);
						if (firstNonDuplicateNamespaceAttribute2 == null)
						{
							return false;
						}
						this.source = firstNonDuplicateNamespaceAttribute2;
					}
					else
					{
						this.source = xattribute.next;
					}
					this.parent = null;
					return true;
				}
				return false;
			}
		}

		public override bool Read()
		{
			ReadState readState = this.state;
			if (readState != ReadState.Initial)
			{
				return readState == ReadState.Interactive && this.Read(false);
			}
			this.state = ReadState.Interactive;
			XDocument xdocument = this.source as XDocument;
			return xdocument == null || this.ReadIntoDocument(xdocument);
		}

		public override bool ReadAttributeValue()
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			XAttribute xattribute = this.source as XAttribute;
			return xattribute != null && this.ReadIntoAttribute(xattribute);
		}

		public override bool ReadToDescendant(string localName, string namespaceName)
		{
			if (!this.IsInteractive)
			{
				return false;
			}
			this.MoveToElement();
			XElement xelement = this.source as XElement;
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
						this.source = xelement2;
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
				XElement xelement = this.source as XElement;
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
			if (this.source != this.root)
			{
				XNode xnode = this.source as XNode;
				if (xnode != null)
				{
					foreach (XElement xelement in xnode.ElementsAfterSelf())
					{
						if (xelement.Name.LocalName == localName && xelement.Name.NamespaceName == namespaceName)
						{
							this.source = xelement;
							this.IsEndElement = false;
							return true;
						}
					}
					if (xnode.parent is XElement)
					{
						this.source = xnode.parent;
						this.IsEndElement = true;
						return false;
					}
					goto IL_00E0;
				}
				if (this.parent is XElement)
				{
					this.source = this.parent;
					this.parent = null;
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

		internal override IDtdInfo DtdInfo
		{
			get
			{
				if (this.dtdInfoInitialized)
				{
					return this.dtdInfo;
				}
				this.dtdInfoInitialized = true;
				XDocumentType xdocumentType = this.source as XDocumentType;
				if (xdocumentType == null)
				{
					for (XNode xnode = this.root; xnode != null; xnode = xnode.parent)
					{
						XDocument xdocument = xnode as XDocument;
						if (xdocument != null)
						{
							xdocumentType = xdocument.DocumentType;
							break;
						}
					}
				}
				if (xdocumentType != null)
				{
					this.dtdInfo = xdocumentType.DtdInfo;
				}
				return this.dtdInfo;
			}
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			if (this.IsEndElement)
			{
				XElement xelement = this.source as XElement;
				if (xelement != null)
				{
					return xelement.Annotation<LineInfoEndElementAnnotation>() != null;
				}
			}
			else
			{
				IXmlLineInfo xmlLineInfo = this.source as IXmlLineInfo;
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
					XElement xelement = this.source as XElement;
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
					IXmlLineInfo xmlLineInfo = this.source as IXmlLineInfo;
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
					XElement xelement = this.source as XElement;
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
					IXmlLineInfo xmlLineInfo = this.source as IXmlLineInfo;
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
				return this.parent == this.source;
			}
			set
			{
				this.parent = (value ? this.source : null);
			}
		}

		private bool IsInteractive
		{
			get
			{
				return this.state == ReadState.Interactive;
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
			XElement xelement = this.source as XElement;
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
				XAttribute xattribute = this.source as XAttribute;
				if (xattribute != null)
				{
					return (XElement)xattribute.parent;
				}
				xattribute = this.parent as XAttribute;
				if (xattribute != null)
				{
					return (XElement)xattribute.parent;
				}
				return null;
			}
		}

		private XElement GetElementInScope()
		{
			XElement xelement = this.source as XElement;
			if (xelement != null)
			{
				return xelement;
			}
			XNode xnode = this.source as XNode;
			if (xnode != null)
			{
				return xnode.parent as XElement;
			}
			XAttribute xattribute = this.source as XAttribute;
			if (xattribute != null)
			{
				return (XElement)xattribute.parent;
			}
			xelement = this.parent as XElement;
			if (xelement != null)
			{
				return xelement;
			}
			xattribute = this.parent as XAttribute;
			if (xattribute != null)
			{
				return (XElement)xattribute.parent;
			}
			return null;
		}

		private static void GetNameInAttributeScope(string qualifiedName, XElement e, out string localName, out string namespaceName)
		{
			if (qualifiedName != null && qualifiedName.Length != 0)
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
			XElement xelement = this.source as XElement;
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
				XNode xnode = this.source as XNode;
				if (xnode != null)
				{
					return this.ReadOverNode(xnode);
				}
				XAttribute xattribute = this.source as XAttribute;
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
				this.source = xnode.next;
				return true;
			}
			string text = d.content as string;
			if (text != null && text.Length > 0)
			{
				this.source = text;
				this.parent = d;
				return true;
			}
			return this.ReadToEnd();
		}

		private bool ReadIntoElement(XElement e)
		{
			XNode xnode = e.content as XNode;
			if (xnode != null)
			{
				this.source = xnode.next;
				return true;
			}
			string text = e.content as string;
			if (text != null)
			{
				if (text.Length > 0)
				{
					this.source = text;
					this.parent = e;
				}
				else
				{
					this.source = e;
					this.IsEndElement = true;
				}
				return true;
			}
			return this.ReadToEnd();
		}

		private bool ReadIntoAttribute(XAttribute a)
		{
			this.source = a.value;
			this.parent = a;
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
			if (n == this.root)
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
				this.source = n.parent;
				this.IsEndElement = true;
			}
			else
			{
				this.source = next;
				this.IsEndElement = false;
			}
			return true;
		}

		private bool ReadOverText(bool skipContent)
		{
			if (this.parent is XElement)
			{
				this.source = this.parent;
				this.parent = null;
				this.IsEndElement = true;
				return true;
			}
			if (this.parent is XAttribute)
			{
				XAttribute xattribute = (XAttribute)this.parent;
				this.parent = null;
				return this.ReadOverAttribute(xattribute, skipContent);
			}
			return this.ReadToEnd();
		}

		private bool ReadToEnd()
		{
			this.state = ReadState.EndOfFile;
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
			if (xelement == this.root || xelement == null)
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
				if (xelement == this.root)
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

		private object source;

		private object parent;

		private ReadState state;

		private XNode root;

		private XmlNameTable nameTable;

		private bool omitDuplicateNamespaces;

		private IDtdInfo dtdInfo;

		private bool dtdInfoInitialized;
	}
}
