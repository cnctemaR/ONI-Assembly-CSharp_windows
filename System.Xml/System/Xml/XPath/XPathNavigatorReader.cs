using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace System.Xml.XPath
{
	internal class XPathNavigatorReader : XmlReader, IXmlNamespaceResolver
	{
		internal static XmlNodeType ToXmlNodeType(XPathNodeType typ)
		{
			return XPathNavigatorReader.convertFromXPathNodeType[(int)typ];
		}

		internal object UnderlyingObject
		{
			get
			{
				return this.nav.UnderlyingObject;
			}
		}

		public static XPathNavigatorReader Create(XPathNavigator navToRead)
		{
			XPathNavigator xpathNavigator = navToRead.Clone();
			IXmlLineInfo xmlLineInfo = xpathNavigator as IXmlLineInfo;
			IXmlSchemaInfo xmlSchemaInfo = xpathNavigator as IXmlSchemaInfo;
			if (xmlSchemaInfo == null)
			{
				return new XPathNavigatorReader(xpathNavigator, xmlLineInfo, xmlSchemaInfo);
			}
			return new XPathNavigatorReaderWithSI(xpathNavigator, xmlLineInfo, xmlSchemaInfo);
		}

		protected XPathNavigatorReader(XPathNavigator navToRead, IXmlLineInfo xli, IXmlSchemaInfo xsi)
		{
			this.navToRead = navToRead;
			this.lineInfo = xli;
			this.schemaInfo = xsi;
			this.nav = XmlEmptyNavigator.Singleton;
			this.state = XPathNavigatorReader.State.Initial;
			this.depth = 0;
			this.nodeType = XPathNavigatorReader.ToXmlNodeType(this.nav.NodeType);
		}

		protected bool IsReading
		{
			get
			{
				return this.state > XPathNavigatorReader.State.Initial && this.state < XPathNavigatorReader.State.EOF;
			}
		}

		internal override XmlNamespaceManager NamespaceManager
		{
			get
			{
				return XPathNavigator.GetNamespaces(this);
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.navToRead.NameTable;
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.nav.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			return this.nav.LookupNamespace(prefix);
		}

		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			return this.nav.LookupPrefix(namespaceName);
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				return new XmlReaderSettings
				{
					NameTable = this.NameTable,
					ConformanceLevel = ConformanceLevel.Fragment,
					CheckCharacters = false,
					ReadOnly = true
				};
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				if (this.nodeType == XmlNodeType.Text)
				{
					return null;
				}
				return this.nav.SchemaInfo;
			}
		}

		public override Type ValueType
		{
			get
			{
				return this.nav.ValueType;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.nodeType;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (this.nav.NodeType == XPathNodeType.Namespace)
				{
					return this.NameTable.Add("http://www.w3.org/2000/xmlns/");
				}
				if (this.NodeType == XmlNodeType.Text)
				{
					return string.Empty;
				}
				return this.nav.NamespaceURI;
			}
		}

		public override string LocalName
		{
			get
			{
				if (this.nav.NodeType == XPathNodeType.Namespace && this.nav.LocalName.Length == 0)
				{
					return this.NameTable.Add("xmlns");
				}
				if (this.NodeType == XmlNodeType.Text)
				{
					return string.Empty;
				}
				return this.nav.LocalName;
			}
		}

		public override string Prefix
		{
			get
			{
				if (this.nav.NodeType == XPathNodeType.Namespace && this.nav.LocalName.Length != 0)
				{
					return this.NameTable.Add("xmlns");
				}
				if (this.NodeType == XmlNodeType.Text)
				{
					return string.Empty;
				}
				return this.nav.Prefix;
			}
		}

		public override string BaseURI
		{
			get
			{
				if (this.state == XPathNavigatorReader.State.Initial)
				{
					return this.navToRead.BaseURI;
				}
				return this.nav.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.nav.IsEmptyElement;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				XPathNavigator xpathNavigator = this.nav.Clone();
				for (;;)
				{
					if (xpathNavigator.MoveToAttribute("space", "http://www.w3.org/XML/1998/namespace"))
					{
						string text = XmlConvert.TrimString(xpathNavigator.Value);
						if (text == "default")
						{
							break;
						}
						if (text == "preserve")
						{
							return XmlSpace.Preserve;
						}
						xpathNavigator.MoveToParent();
					}
					if (!xpathNavigator.MoveToParent())
					{
						return XmlSpace.None;
					}
				}
				return XmlSpace.Default;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.nav.XmlLang;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.nodeType != XmlNodeType.Element && this.nodeType != XmlNodeType.Document && this.nodeType != XmlNodeType.EndElement && this.nodeType != XmlNodeType.None;
			}
		}

		public override string Value
		{
			get
			{
				if (this.nodeType != XmlNodeType.Element && this.nodeType != XmlNodeType.Document && this.nodeType != XmlNodeType.EndElement && this.nodeType != XmlNodeType.None)
				{
					return this.nav.Value;
				}
				return string.Empty;
			}
		}

		private XPathNavigator GetElemNav()
		{
			switch (this.state)
			{
			case XPathNavigatorReader.State.Content:
				return this.nav.Clone();
			case XPathNavigatorReader.State.Attribute:
			case XPathNavigatorReader.State.AttrVal:
			{
				XPathNavigator xpathNavigator = this.nav.Clone();
				if (xpathNavigator.MoveToParent())
				{
					return xpathNavigator;
				}
				break;
			}
			case XPathNavigatorReader.State.InReadBinary:
			{
				this.state = this.savedState;
				XPathNavigator elemNav = this.GetElemNav();
				this.state = XPathNavigatorReader.State.InReadBinary;
				return elemNav;
			}
			}
			return null;
		}

		private XPathNavigator GetElemNav(out int depth)
		{
			XPathNavigator xpathNavigator = null;
			switch (this.state)
			{
			case XPathNavigatorReader.State.Content:
				if (this.nodeType == XmlNodeType.Element)
				{
					xpathNavigator = this.nav.Clone();
				}
				depth = this.depth;
				return xpathNavigator;
			case XPathNavigatorReader.State.Attribute:
				xpathNavigator = this.nav.Clone();
				xpathNavigator.MoveToParent();
				depth = this.depth - 1;
				return xpathNavigator;
			case XPathNavigatorReader.State.AttrVal:
				xpathNavigator = this.nav.Clone();
				xpathNavigator.MoveToParent();
				depth = this.depth - 2;
				return xpathNavigator;
			case XPathNavigatorReader.State.InReadBinary:
				this.state = this.savedState;
				xpathNavigator = this.GetElemNav(out depth);
				this.state = XPathNavigatorReader.State.InReadBinary;
				return xpathNavigator;
			}
			depth = this.depth;
			return xpathNavigator;
		}

		private void MoveToAttr(XPathNavigator nav, int depth)
		{
			this.nav.MoveTo(nav);
			this.depth = depth;
			this.nodeType = XmlNodeType.Attribute;
			this.state = XPathNavigatorReader.State.Attribute;
		}

		public override int AttributeCount
		{
			get
			{
				if (this.attrCount < 0)
				{
					XPathNavigator elemNav = this.GetElemNav();
					int num = 0;
					if (elemNav != null)
					{
						if (elemNav.MoveToFirstNamespace(XPathNamespaceScope.Local))
						{
							do
							{
								num++;
							}
							while (elemNav.MoveToNextNamespace(XPathNamespaceScope.Local));
							elemNav.MoveToParent();
						}
						if (elemNav.MoveToFirstAttribute())
						{
							do
							{
								num++;
							}
							while (elemNav.MoveToNextAttribute());
						}
					}
					this.attrCount = num;
				}
				return this.attrCount;
			}
		}

		public override string GetAttribute(string name)
		{
			XPathNavigator xpathNavigator = this.nav;
			XPathNodeType xpathNodeType = xpathNavigator.NodeType;
			if (xpathNodeType != XPathNodeType.Element)
			{
				if (xpathNodeType != XPathNodeType.Attribute)
				{
					return null;
				}
				xpathNavigator = xpathNavigator.Clone();
				if (!xpathNavigator.MoveToParent())
				{
					return null;
				}
			}
			string text;
			string text2;
			ValidateNames.SplitQName(name, out text, out text2);
			if (text.Length == 0)
			{
				if (text2 == "xmlns")
				{
					return xpathNavigator.GetNamespace(string.Empty);
				}
				if (xpathNavigator == this.nav)
				{
					xpathNavigator = xpathNavigator.Clone();
				}
				if (xpathNavigator.MoveToAttribute(text2, string.Empty))
				{
					return xpathNavigator.Value;
				}
			}
			else
			{
				if (text == "xmlns")
				{
					return xpathNavigator.GetNamespace(text2);
				}
				if (xpathNavigator == this.nav)
				{
					xpathNavigator = xpathNavigator.Clone();
				}
				if (xpathNavigator.MoveToFirstAttribute())
				{
					while (!(xpathNavigator.LocalName == text2) || !(xpathNavigator.Prefix == text))
					{
						if (!xpathNavigator.MoveToNextAttribute())
						{
							goto IL_00D1;
						}
					}
					return xpathNavigator.Value;
				}
			}
			IL_00D1:
			return null;
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			XPathNavigator xpathNavigator = this.nav;
			XPathNodeType xpathNodeType = xpathNavigator.NodeType;
			if (xpathNodeType != XPathNodeType.Element)
			{
				if (xpathNodeType != XPathNodeType.Attribute)
				{
					return null;
				}
				xpathNavigator = xpathNavigator.Clone();
				if (!xpathNavigator.MoveToParent())
				{
					return null;
				}
			}
			if (namespaceURI == "http://www.w3.org/2000/xmlns/")
			{
				if (localName == "xmlns")
				{
					localName = string.Empty;
				}
				return xpathNavigator.GetNamespace(localName);
			}
			if (namespaceURI == null)
			{
				namespaceURI = string.Empty;
			}
			if (xpathNavigator == this.nav)
			{
				xpathNavigator = xpathNavigator.Clone();
			}
			if (xpathNavigator.MoveToAttribute(localName, namespaceURI))
			{
				return xpathNavigator.Value;
			}
			return null;
		}

		private static string GetNamespaceByIndex(XPathNavigator nav, int index, out int count)
		{
			string value = nav.Value;
			string text = null;
			if (nav.MoveToNextNamespace(XPathNamespaceScope.Local))
			{
				text = XPathNavigatorReader.GetNamespaceByIndex(nav, index, out count);
			}
			else
			{
				count = 0;
			}
			if (count == index)
			{
				text = value;
			}
			count++;
			return text;
		}

		public override string GetAttribute(int index)
		{
			if (index >= 0)
			{
				XPathNavigator elemNav = this.GetElemNav();
				if (elemNav != null)
				{
					if (elemNav.MoveToFirstNamespace(XPathNamespaceScope.Local))
					{
						int num;
						string namespaceByIndex = XPathNavigatorReader.GetNamespaceByIndex(elemNav, index, out num);
						if (namespaceByIndex != null)
						{
							return namespaceByIndex;
						}
						index -= num;
						elemNav.MoveToParent();
					}
					if (elemNav.MoveToFirstAttribute())
					{
						while (index != 0)
						{
							index--;
							if (!elemNav.MoveToNextAttribute())
							{
								goto IL_0051;
							}
						}
						return elemNav.Value;
					}
				}
			}
			IL_0051:
			throw new ArgumentOutOfRangeException("index");
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			int num = this.depth;
			XPathNavigator elemNav = this.GetElemNav(out num);
			if (elemNav != null)
			{
				if (namespaceName == "http://www.w3.org/2000/xmlns/")
				{
					if (localName == "xmlns")
					{
						localName = string.Empty;
					}
					if (!elemNav.MoveToFirstNamespace(XPathNamespaceScope.Local))
					{
						return false;
					}
					while (!(elemNav.LocalName == localName))
					{
						if (!elemNav.MoveToNextNamespace(XPathNamespaceScope.Local))
						{
							return false;
						}
					}
				}
				else
				{
					if (namespaceName == null)
					{
						namespaceName = string.Empty;
					}
					if (!elemNav.MoveToAttribute(localName, namespaceName))
					{
						return false;
					}
				}
				if (this.state == XPathNavigatorReader.State.InReadBinary)
				{
					this.readBinaryHelper.Finish();
					this.state = this.savedState;
				}
				this.MoveToAttr(elemNav, num + 1);
				return true;
			}
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			int num;
			XPathNavigator elemNav = this.GetElemNav(out num);
			if (elemNav != null)
			{
				if (elemNav.MoveToFirstNamespace(XPathNamespaceScope.Local))
				{
					while (elemNav.MoveToNextNamespace(XPathNamespaceScope.Local))
					{
					}
				}
				else if (!elemNav.MoveToFirstAttribute())
				{
					return false;
				}
				if (this.state == XPathNavigatorReader.State.InReadBinary)
				{
					this.readBinaryHelper.Finish();
					this.state = this.savedState;
				}
				this.MoveToAttr(elemNav, num + 1);
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			switch (this.state)
			{
			case XPathNavigatorReader.State.Content:
				return this.MoveToFirstAttribute();
			case XPathNavigatorReader.State.Attribute:
			{
				if (XPathNodeType.Attribute == this.nav.NodeType)
				{
					return this.nav.MoveToNextAttribute();
				}
				XPathNavigator xpathNavigator = this.nav.Clone();
				if (!xpathNavigator.MoveToParent())
				{
					return false;
				}
				if (!xpathNavigator.MoveToFirstNamespace(XPathNamespaceScope.Local))
				{
					return false;
				}
				if (!xpathNavigator.IsSamePosition(this.nav))
				{
					XPathNavigator xpathNavigator2 = xpathNavigator.Clone();
					while (xpathNavigator.MoveToNextNamespace(XPathNamespaceScope.Local))
					{
						if (xpathNavigator.IsSamePosition(this.nav))
						{
							this.nav.MoveTo(xpathNavigator2);
							return true;
						}
						xpathNavigator2.MoveTo(xpathNavigator);
					}
					return false;
				}
				xpathNavigator.MoveToParent();
				if (!xpathNavigator.MoveToFirstAttribute())
				{
					return false;
				}
				this.nav.MoveTo(xpathNavigator);
				return true;
			}
			case XPathNavigatorReader.State.AttrVal:
				this.depth--;
				this.state = XPathNavigatorReader.State.Attribute;
				if (!this.MoveToNextAttribute())
				{
					this.depth++;
					this.state = XPathNavigatorReader.State.AttrVal;
					return false;
				}
				this.nodeType = XmlNodeType.Attribute;
				return true;
			case XPathNavigatorReader.State.InReadBinary:
				this.state = this.savedState;
				if (!this.MoveToNextAttribute())
				{
					this.state = XPathNavigatorReader.State.InReadBinary;
					return false;
				}
				this.readBinaryHelper.Finish();
				return true;
			}
			return false;
		}

		public override bool MoveToAttribute(string name)
		{
			int num;
			XPathNavigator elemNav = this.GetElemNav(out num);
			if (elemNav == null)
			{
				return false;
			}
			string text;
			string empty;
			ValidateNames.SplitQName(name, out text, out empty);
			bool flag;
			if ((flag = text.Length == 0 && empty == "xmlns") || text == "xmlns")
			{
				if (flag)
				{
					empty = string.Empty;
				}
				if (elemNav.MoveToFirstNamespace(XPathNamespaceScope.Local))
				{
					while (!(elemNav.LocalName == empty))
					{
						if (!elemNav.MoveToNextNamespace(XPathNamespaceScope.Local))
						{
							return false;
						}
					}
					goto IL_00B5;
				}
			}
			else if (text.Length == 0)
			{
				if (elemNav.MoveToAttribute(empty, string.Empty))
				{
					goto IL_00B5;
				}
			}
			else if (elemNav.MoveToFirstAttribute())
			{
				while (!(elemNav.LocalName == empty) || !(elemNav.Prefix == text))
				{
					if (!elemNav.MoveToNextAttribute())
					{
						return false;
					}
				}
				goto IL_00B5;
			}
			return false;
			IL_00B5:
			if (this.state == XPathNavigatorReader.State.InReadBinary)
			{
				this.readBinaryHelper.Finish();
				this.state = this.savedState;
			}
			this.MoveToAttr(elemNav, num + 1);
			return true;
		}

		public override bool MoveToElement()
		{
			XPathNavigatorReader.State state = this.state;
			if (state - XPathNavigatorReader.State.Attribute > 1)
			{
				if (state == XPathNavigatorReader.State.InReadBinary)
				{
					this.state = this.savedState;
					if (!this.MoveToElement())
					{
						this.state = XPathNavigatorReader.State.InReadBinary;
						return false;
					}
					this.readBinaryHelper.Finish();
				}
				return false;
			}
			if (!this.nav.MoveToParent())
			{
				return false;
			}
			this.depth--;
			if (this.state == XPathNavigatorReader.State.AttrVal)
			{
				this.depth--;
			}
			this.state = XPathNavigatorReader.State.Content;
			this.nodeType = XmlNodeType.Element;
			return true;
		}

		public override bool EOF
		{
			get
			{
				return this.state == XPathNavigatorReader.State.EOF;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				switch (this.state)
				{
				case XPathNavigatorReader.State.Initial:
					return ReadState.Initial;
				case XPathNavigatorReader.State.Content:
				case XPathNavigatorReader.State.EndElement:
				case XPathNavigatorReader.State.Attribute:
				case XPathNavigatorReader.State.AttrVal:
				case XPathNavigatorReader.State.InReadBinary:
					return ReadState.Interactive;
				case XPathNavigatorReader.State.EOF:
					return ReadState.EndOfFile;
				case XPathNavigatorReader.State.Closed:
					return ReadState.Closed;
				default:
					return ReadState.Error;
				}
			}
		}

		public override void ResolveEntity()
		{
			throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
		}

		public override bool ReadAttributeValue()
		{
			if (this.state == XPathNavigatorReader.State.InReadBinary)
			{
				this.readBinaryHelper.Finish();
				this.state = this.savedState;
			}
			if (this.state == XPathNavigatorReader.State.Attribute)
			{
				this.state = XPathNavigatorReader.State.AttrVal;
				this.nodeType = XmlNodeType.Text;
				this.depth++;
				return true;
			}
			return false;
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XPathNavigatorReader.State.InReadBinary)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.state;
			}
			this.state = this.savedState;
			int num = this.readBinaryHelper.ReadContentAsBase64(buffer, index, count);
			this.savedState = this.state;
			this.state = XPathNavigatorReader.State.InReadBinary;
			return num;
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XPathNavigatorReader.State.InReadBinary)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.state;
			}
			this.state = this.savedState;
			int num = this.readBinaryHelper.ReadContentAsBinHex(buffer, index, count);
			this.savedState = this.state;
			this.state = XPathNavigatorReader.State.InReadBinary;
			return num;
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XPathNavigatorReader.State.InReadBinary)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.state;
			}
			this.state = this.savedState;
			int num = this.readBinaryHelper.ReadElementContentAsBase64(buffer, index, count);
			this.savedState = this.state;
			this.state = XPathNavigatorReader.State.InReadBinary;
			return num;
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XPathNavigatorReader.State.InReadBinary)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.state;
			}
			this.state = this.savedState;
			int num = this.readBinaryHelper.ReadElementContentAsBinHex(buffer, index, count);
			this.savedState = this.state;
			this.state = XPathNavigatorReader.State.InReadBinary;
			return num;
		}

		public override string LookupNamespace(string prefix)
		{
			return this.nav.LookupNamespace(prefix);
		}

		public override int Depth
		{
			get
			{
				return this.depth;
			}
		}

		public override bool Read()
		{
			this.attrCount = -1;
			switch (this.state)
			{
			case XPathNavigatorReader.State.Initial:
				this.nav = this.navToRead;
				this.state = XPathNavigatorReader.State.Content;
				if (this.nav.NodeType == XPathNodeType.Root)
				{
					if (!this.nav.MoveToFirstChild())
					{
						this.SetEOF();
						return false;
					}
					this.readEntireDocument = true;
				}
				else if (XPathNodeType.Attribute == this.nav.NodeType)
				{
					this.state = XPathNavigatorReader.State.Attribute;
				}
				this.nodeType = XPathNavigatorReader.ToXmlNodeType(this.nav.NodeType);
				return true;
			case XPathNavigatorReader.State.Content:
				break;
			case XPathNavigatorReader.State.EndElement:
				goto IL_0114;
			case XPathNavigatorReader.State.Attribute:
			case XPathNavigatorReader.State.AttrVal:
				if (!this.nav.MoveToParent())
				{
					this.SetEOF();
					return false;
				}
				this.nodeType = XPathNavigatorReader.ToXmlNodeType(this.nav.NodeType);
				this.depth--;
				if (this.state == XPathNavigatorReader.State.AttrVal)
				{
					this.depth--;
				}
				break;
			case XPathNavigatorReader.State.InReadBinary:
				this.state = this.savedState;
				this.readBinaryHelper.Finish();
				return this.Read();
			case XPathNavigatorReader.State.EOF:
			case XPathNavigatorReader.State.Closed:
			case XPathNavigatorReader.State.Error:
				return false;
			default:
				return true;
			}
			if (this.nav.MoveToFirstChild())
			{
				this.nodeType = XPathNavigatorReader.ToXmlNodeType(this.nav.NodeType);
				this.depth++;
				this.state = XPathNavigatorReader.State.Content;
				return true;
			}
			if (this.nodeType == XmlNodeType.Element && !this.nav.IsEmptyElement)
			{
				this.nodeType = XmlNodeType.EndElement;
				this.state = XPathNavigatorReader.State.EndElement;
				return true;
			}
			IL_0114:
			if (this.depth == 0 && !this.readEntireDocument)
			{
				this.SetEOF();
				return false;
			}
			if (this.nav.MoveToNext())
			{
				this.nodeType = XPathNavigatorReader.ToXmlNodeType(this.nav.NodeType);
				this.state = XPathNavigatorReader.State.Content;
			}
			else
			{
				if (this.depth <= 0 || !this.nav.MoveToParent())
				{
					this.SetEOF();
					return false;
				}
				this.nodeType = XmlNodeType.EndElement;
				this.state = XPathNavigatorReader.State.EndElement;
				this.depth--;
			}
			return true;
		}

		public override void Close()
		{
			this.nav = XmlEmptyNavigator.Singleton;
			this.nodeType = XmlNodeType.None;
			this.state = XPathNavigatorReader.State.Closed;
			this.depth = 0;
		}

		private void SetEOF()
		{
			this.nav = XmlEmptyNavigator.Singleton;
			this.nodeType = XmlNodeType.None;
			this.state = XPathNavigatorReader.State.EOF;
			this.depth = 0;
		}

		private XPathNavigator nav;

		private XPathNavigator navToRead;

		private int depth;

		private XPathNavigatorReader.State state;

		private XmlNodeType nodeType;

		private int attrCount;

		private bool readEntireDocument;

		protected IXmlLineInfo lineInfo;

		protected IXmlSchemaInfo schemaInfo;

		private ReadContentAsBinaryHelper readBinaryHelper;

		private XPathNavigatorReader.State savedState;

		internal const string space = "space";

		internal static XmlNodeType[] convertFromXPathNodeType = new XmlNodeType[]
		{
			XmlNodeType.Document,
			XmlNodeType.Element,
			XmlNodeType.Attribute,
			XmlNodeType.Attribute,
			XmlNodeType.Text,
			XmlNodeType.SignificantWhitespace,
			XmlNodeType.Whitespace,
			XmlNodeType.ProcessingInstruction,
			XmlNodeType.Comment,
			XmlNodeType.None
		};

		private enum State
		{
			Initial,
			Content,
			EndElement,
			Attribute,
			AttrVal,
			InReadBinary,
			EOF,
			Closed,
			Error
		}
	}
}
