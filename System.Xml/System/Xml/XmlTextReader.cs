using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using Mono.Xml;
using Mono.Xml2;

namespace System.Xml
{
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class XmlTextReader : XmlReader, IHasXmlParserContext, IXmlLineInfo, IXmlNamespaceResolver
	{
		protected XmlTextReader()
		{
		}

		public XmlTextReader(Stream input)
			: this(new XmlStreamReader(input))
		{
		}

		public XmlTextReader(string url)
			: this(url, new NameTable())
		{
		}

		public XmlTextReader(TextReader input)
			: this(input, new NameTable())
		{
		}

		protected XmlTextReader(XmlNameTable nt)
			: this(string.Empty, XmlNodeType.Element, null)
		{
		}

		public XmlTextReader(Stream input, XmlNameTable nt)
			: this(new XmlStreamReader(input), nt)
		{
		}

		public XmlTextReader(string url, Stream input)
			: this(url, new XmlStreamReader(input))
		{
		}

		public XmlTextReader(string url, TextReader input)
			: this(url, input, new NameTable())
		{
		}

		public XmlTextReader(string url, XmlNameTable nt)
		{
			this.source = new XmlTextReader(url, nt);
		}

		public XmlTextReader(TextReader input, XmlNameTable nt)
			: this(string.Empty, input, nt)
		{
		}

		public XmlTextReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			this.source = new XmlTextReader(xmlFragment, fragType, context);
		}

		public XmlTextReader(string url, Stream input, XmlNameTable nt)
			: this(url, new XmlStreamReader(input), nt)
		{
		}

		public XmlTextReader(string url, TextReader input, XmlNameTable nt)
		{
			this.source = new XmlTextReader(url, input, nt);
		}

		public XmlTextReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			this.source = new XmlTextReader(xmlFragment, fragType, context);
		}

		internal XmlTextReader(string baseURI, TextReader xmlFragment, XmlNodeType fragType)
		{
			this.source = new XmlTextReader(baseURI, xmlFragment, fragType);
		}

		internal XmlTextReader(string baseURI, TextReader xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			this.source = new XmlTextReader(baseURI, xmlFragment, fragType, context);
		}

		internal XmlTextReader(bool dummy, XmlResolver resolver, string url, XmlNodeType fragType, XmlParserContext context)
		{
			this.source = new XmlTextReader(dummy, resolver, url, fragType, context);
		}

		private XmlTextReader(XmlTextReader entityContainer, bool insideAttribute)
		{
			this.source = entityContainer;
			this.entityInsideAttribute = insideAttribute;
		}

		XmlParserContext IHasXmlParserContext.ParserContext
		{
			get
			{
				return this.ParserContext;
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupPrefix(string ns)
		{
			return ((IXmlNamespaceResolver)this.Current).LookupPrefix(ns);
		}

		private XmlReader Current
		{
			get
			{
				return (this.entity == null || this.entity.ReadState == ReadState.Initial) ? this.source : this.entity;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.Current.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.Current.BaseURI;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override bool CanReadValueChunk
		{
			get
			{
				return true;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return true;
			}
		}

		public override int Depth
		{
			get
			{
				if (this.entity != null && this.entity.ReadState == ReadState.Interactive)
				{
					return this.source.Depth + this.entity.Depth + 1;
				}
				return this.source.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.source.EOF;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.Current.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.Current.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.Current.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.Current.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				return this.Current.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.Current.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.Current.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (this.entity != null)
				{
					return (this.entity.ReadState != ReadState.Initial) ? ((!this.entity.EOF) ? this.entity.NodeType : XmlNodeType.EndEntity) : this.source.NodeType;
				}
				return this.source.NodeType;
			}
		}

		internal XmlParserContext ParserContext
		{
			get
			{
				return ((IHasXmlParserContext)this.Current).ParserContext;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.Current.Prefix;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.Current.QuoteChar;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return (this.entity == null) ? this.source.ReadState : ReadState.Interactive;
			}
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				return base.Settings;
			}
		}

		public override string Value
		{
			get
			{
				return this.Current.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.Current.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.Current.XmlSpace;
			}
		}

		internal bool CharacterChecking
		{
			get
			{
				if (this.entity != null)
				{
					return this.entity.CharacterChecking;
				}
				return this.source.CharacterChecking;
			}
			set
			{
				if (this.entity != null)
				{
					this.entity.CharacterChecking = value;
				}
				this.source.CharacterChecking = value;
			}
		}

		internal bool CloseInput
		{
			get
			{
				if (this.entity != null)
				{
					return this.entity.CloseInput;
				}
				return this.source.CloseInput;
			}
			set
			{
				if (this.entity != null)
				{
					this.entity.CloseInput = value;
				}
				this.source.CloseInput = value;
			}
		}

		internal ConformanceLevel Conformance
		{
			get
			{
				return this.source.Conformance;
			}
			set
			{
				if (this.entity != null)
				{
					this.entity.Conformance = value;
				}
				this.source.Conformance = value;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				return this.source.Resolver;
			}
		}

		private void CopyProperties(XmlTextReader other)
		{
			this.CharacterChecking = other.CharacterChecking;
			this.CloseInput = other.CloseInput;
			if (other.Settings != null)
			{
				this.Conformance = other.Settings.ConformanceLevel;
			}
			this.XmlResolver = other.Resolver;
		}

		public Encoding Encoding
		{
			get
			{
				if (this.entity != null)
				{
					return this.entity.Encoding;
				}
				return this.source.Encoding;
			}
		}

		public EntityHandling EntityHandling
		{
			get
			{
				return this.source.EntityHandling;
			}
			set
			{
				if (this.entity != null)
				{
					this.entity.EntityHandling = value;
				}
				this.source.EntityHandling = value;
			}
		}

		public int LineNumber
		{
			get
			{
				if (this.entity != null)
				{
					return this.entity.LineNumber;
				}
				return this.source.LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				if (this.entity != null)
				{
					return this.entity.LinePosition;
				}
				return this.source.LinePosition;
			}
		}

		public bool Namespaces
		{
			get
			{
				return this.source.Namespaces;
			}
			set
			{
				if (this.entity != null)
				{
					this.entity.Namespaces = value;
				}
				this.source.Namespaces = value;
			}
		}

		public bool Normalization
		{
			get
			{
				return this.source.Normalization;
			}
			set
			{
				if (this.entity != null)
				{
					this.entity.Normalization = value;
				}
				this.source.Normalization = value;
			}
		}

		public bool ProhibitDtd
		{
			get
			{
				return this.source.ProhibitDtd;
			}
			set
			{
				if (this.entity != null)
				{
					this.entity.ProhibitDtd = value;
				}
				this.source.ProhibitDtd = value;
			}
		}

		public WhitespaceHandling WhitespaceHandling
		{
			get
			{
				return this.source.WhitespaceHandling;
			}
			set
			{
				if (this.entity != null)
				{
					this.entity.WhitespaceHandling = value;
				}
				this.source.WhitespaceHandling = value;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				if (this.entity != null)
				{
					this.entity.XmlResolver = value;
				}
				this.source.XmlResolver = value;
			}
		}

		internal void AdjustLineInfoOffset(int lineNumberOffset, int linePositionOffset)
		{
			if (this.entity != null)
			{
				this.entity.AdjustLineInfoOffset(lineNumberOffset, linePositionOffset);
			}
			this.source.AdjustLineInfoOffset(lineNumberOffset, linePositionOffset);
		}

		internal void SetNameTable(XmlNameTable nameTable)
		{
			if (this.entity != null)
			{
				this.entity.SetNameTable(nameTable);
			}
			this.source.SetNameTable(nameTable);
		}

		internal void SkipTextDeclaration()
		{
			if (this.entity != null)
			{
				this.entity.SkipTextDeclaration();
			}
			else
			{
				this.source.SkipTextDeclaration();
			}
		}

		public override void Close()
		{
			if (this.entity != null)
			{
				this.entity.Close();
			}
			this.source.Close();
		}

		public override string GetAttribute(int i)
		{
			return this.Current.GetAttribute(i);
		}

		public override string GetAttribute(string name)
		{
			return this.Current.GetAttribute(name);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return this.Current.GetAttribute(localName, namespaceURI);
		}

		public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return ((IXmlNamespaceResolver)this.Current).GetNamespacesInScope(scope);
		}

		public override string LookupNamespace(string prefix)
		{
			return this.Current.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.CloseEntity();
			}
			this.Current.MoveToAttribute(i);
			this.insideAttribute = true;
		}

		public override bool MoveToAttribute(string name)
		{
			if (this.entity != null && !this.entityInsideAttribute)
			{
				return this.entity.MoveToAttribute(name);
			}
			if (!this.source.MoveToAttribute(name))
			{
				return false;
			}
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.CloseEntity();
			}
			this.insideAttribute = true;
			return true;
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			if (this.entity != null && !this.entityInsideAttribute)
			{
				return this.entity.MoveToAttribute(localName, namespaceName);
			}
			if (!this.source.MoveToAttribute(localName, namespaceName))
			{
				return false;
			}
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.CloseEntity();
			}
			this.insideAttribute = true;
			return true;
		}

		public override bool MoveToElement()
		{
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.CloseEntity();
			}
			if (!this.Current.MoveToElement())
			{
				return false;
			}
			this.insideAttribute = false;
			return true;
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.entity != null && !this.entityInsideAttribute)
			{
				return this.entity.MoveToFirstAttribute();
			}
			if (!this.source.MoveToFirstAttribute())
			{
				return false;
			}
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.CloseEntity();
			}
			this.insideAttribute = true;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (this.entity != null && !this.entityInsideAttribute)
			{
				return this.entity.MoveToNextAttribute();
			}
			if (!this.source.MoveToNextAttribute())
			{
				return false;
			}
			if (this.entity != null && this.entityInsideAttribute)
			{
				this.CloseEntity();
			}
			this.insideAttribute = true;
			return true;
		}

		public override bool Read()
		{
			this.insideAttribute = false;
			if (this.entity != null && (this.entityInsideAttribute || this.entity.EOF))
			{
				this.CloseEntity();
			}
			if (this.entity != null)
			{
				if (this.entity.Read())
				{
					return true;
				}
				if (this.EntityHandling == EntityHandling.ExpandEntities)
				{
					this.CloseEntity();
					return this.Read();
				}
				return true;
			}
			else
			{
				if (!this.source.Read())
				{
					return false;
				}
				if (this.EntityHandling == EntityHandling.ExpandEntities && this.source.NodeType == XmlNodeType.EntityReference)
				{
					this.ResolveEntity();
					return this.Read();
				}
				return true;
			}
		}

		public override bool ReadAttributeValue()
		{
			if (this.entity != null && this.entityInsideAttribute)
			{
				if (!this.entity.EOF)
				{
					this.entity.Read();
					return true;
				}
				this.CloseEntity();
			}
			return this.Current.ReadAttributeValue();
		}

		public override string ReadString()
		{
			return base.ReadString();
		}

		public void ResetState()
		{
			if (this.entity != null)
			{
				this.CloseEntity();
			}
			this.source.ResetState();
		}

		public override void ResolveEntity()
		{
			if (this.entity != null)
			{
				this.entity.ResolveEntity();
			}
			else
			{
				if (this.source.NodeType != XmlNodeType.EntityReference)
				{
					throw new InvalidOperationException("The current node is not an Entity Reference");
				}
				XmlTextReader xmlTextReader = null;
				if (this.ParserContext.Dtd != null)
				{
					xmlTextReader = this.ParserContext.Dtd.GenerateEntityContentReader(this.source.Name, this.ParserContext);
				}
				if (xmlTextReader == null)
				{
					throw new XmlException(this, this.BaseURI, string.Format("Reference to undeclared entity '{0}'.", this.source.Name));
				}
				if (this.entityNameStack == null)
				{
					this.entityNameStack = new Stack<string>();
				}
				else if (this.entityNameStack.Contains(this.Name))
				{
					throw new XmlException(string.Format("General entity '{0}' has an invalid recursive reference to itself.", this.Name));
				}
				this.entityNameStack.Push(this.Name);
				this.entity = new XmlTextReader(xmlTextReader, this.insideAttribute);
				this.entity.entityNameStack = this.entityNameStack;
				this.entity.CopyProperties(this);
			}
		}

		private void CloseEntity()
		{
			this.entity.Close();
			this.entity = null;
			this.entityNameStack.Pop();
		}

		public override void Skip()
		{
			base.Skip();
		}

		[MonoTODO]
		public TextReader GetRemainder()
		{
			if (this.entity != null)
			{
				this.entity.Close();
				this.entity = null;
				this.entityNameStack.Pop();
			}
			return this.source.GetRemainder();
		}

		public bool HasLineInfo()
		{
			return true;
		}

		[MonoTODO]
		public int ReadBase64(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadBase64(buffer, offset, length);
			}
			return this.source.ReadBase64(buffer, offset, length);
		}

		[MonoTODO]
		public int ReadBinHex(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadBinHex(buffer, offset, length);
			}
			return this.source.ReadBinHex(buffer, offset, length);
		}

		[MonoTODO]
		public int ReadChars(char[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadChars(buffer, offset, length);
			}
			return this.source.ReadChars(buffer, offset, length);
		}

		[MonoTODO]
		public override int ReadContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadContentAsBase64(buffer, offset, length);
			}
			return this.source.ReadContentAsBase64(buffer, offset, length);
		}

		[MonoTODO]
		public override int ReadContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadContentAsBinHex(buffer, offset, length);
			}
			return this.source.ReadContentAsBinHex(buffer, offset, length);
		}

		[MonoTODO]
		public override int ReadElementContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadElementContentAsBase64(buffer, offset, length);
			}
			return this.source.ReadElementContentAsBase64(buffer, offset, length);
		}

		[MonoTODO]
		public override int ReadElementContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (this.entity != null)
			{
				return this.entity.ReadElementContentAsBinHex(buffer, offset, length);
			}
			return this.source.ReadElementContentAsBinHex(buffer, offset, length);
		}

		private XmlTextReader entity;

		private XmlTextReader source;

		private bool entityInsideAttribute;

		private bool insideAttribute;

		private Stack<string> entityNameStack;
	}
}
