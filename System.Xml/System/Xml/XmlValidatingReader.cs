using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Xml.Schema;
using Mono.Xml;
using Mono.Xml.Schema;

namespace System.Xml
{
	[Obsolete("Use XmlReader created by XmlReader.Create() method using appropriate XmlReaderSettings instead.")]
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class XmlValidatingReader : XmlReader, IHasXmlParserContext, IXmlLineInfo, IXmlNamespaceResolver
	{
		public XmlValidatingReader(XmlReader reader)
		{
			this.sourceReader = reader;
			this.xmlTextReader = reader as XmlTextReader;
			if (this.xmlTextReader == null)
			{
				this.resolver = new XmlUrlResolver();
			}
			this.entityHandling = EntityHandling.ExpandEntities;
			this.validationType = ValidationType.Auto;
			this.storedCharacters = new StringBuilder();
		}

		public XmlValidatingReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
			: this(new XmlTextReader(xmlFragment, fragType, context))
		{
		}

		public XmlValidatingReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
			: this(new XmlTextReader(xmlFragment, fragType, context))
		{
		}

		public event ValidationEventHandler ValidationEventHandler;

		XmlParserContext IHasXmlParserContext.ParserContext
		{
			get
			{
				if (this.dtdReader != null)
				{
					return this.dtdReader.ParserContext;
				}
				IHasXmlParserContext hasXmlParserContext = this.sourceReader as IHasXmlParserContext;
				return (hasXmlParserContext == null) ? null : hasXmlParserContext.ParserContext;
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return ((IHasXmlParserContext)this).ParserContext.NamespaceManager.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupPrefix(string ns)
		{
			IXmlNamespaceResolver xmlNamespaceResolver;
			if (this.validatingReader != null)
			{
				xmlNamespaceResolver = this.sourceReader as IXmlNamespaceResolver;
			}
			else
			{
				xmlNamespaceResolver = this.validatingReader as IXmlNamespaceResolver;
			}
			return (xmlNamespaceResolver == null) ? null : xmlNamespaceResolver.LookupNamespace(ns);
		}

		public override int AttributeCount
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.AttributeCount : 0;
			}
		}

		public override string BaseURI
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.BaseURI : this.sourceReader.BaseURI;
			}
		}

		public override bool CanReadBinaryContent
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
				return (this.validatingReader != null) ? this.validatingReader.Depth : 0;
			}
		}

		public Encoding Encoding
		{
			get
			{
				if (this.xmlTextReader != null)
				{
					return this.xmlTextReader.Encoding;
				}
				throw new NotSupportedException("Encoding is supported only for XmlTextReader.");
			}
		}

		public EntityHandling EntityHandling
		{
			get
			{
				return this.entityHandling;
			}
			set
			{
				this.entityHandling = value;
				if (this.dtdReader != null)
				{
					this.dtdReader.EntityHandling = value;
				}
			}
		}

		public override bool EOF
		{
			get
			{
				return this.validatingReader != null && this.validatingReader.EOF;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.validatingReader != null && this.validatingReader.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.validatingReader != null && this.validatingReader.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.validatingReader != null && this.validatingReader.IsEmptyElement;
			}
		}

		public int LineNumber
		{
			get
			{
				if (this.IsDefault)
				{
					return 0;
				}
				IXmlLineInfo xmlLineInfo = this.validatingReader as IXmlLineInfo;
				return (xmlLineInfo == null) ? 0 : xmlLineInfo.LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				if (this.IsDefault)
				{
					return 0;
				}
				IXmlLineInfo xmlLineInfo = this.validatingReader as IXmlLineInfo;
				return (xmlLineInfo == null) ? 0 : xmlLineInfo.LinePosition;
			}
		}

		public override string LocalName
		{
			get
			{
				if (this.validatingReader == null)
				{
					return string.Empty;
				}
				if (this.Namespaces)
				{
					return this.validatingReader.LocalName;
				}
				return this.validatingReader.Name;
			}
		}

		public override string Name
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.Name : string.Empty;
			}
		}

		public bool Namespaces
		{
			get
			{
				return this.xmlTextReader == null || this.xmlTextReader.Namespaces;
			}
			set
			{
				if (this.ReadState != ReadState.Initial)
				{
					throw new InvalidOperationException("Namespaces have to be set before reading.");
				}
				if (this.xmlTextReader != null)
				{
					this.xmlTextReader.Namespaces = value;
					return;
				}
				throw new NotSupportedException("Property 'Namespaces' is supported only for XmlTextReader.");
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (this.validatingReader == null)
				{
					return string.Empty;
				}
				if (this.Namespaces)
				{
					return this.validatingReader.NamespaceURI;
				}
				return string.Empty;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.NameTable : this.sourceReader.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.NodeType : XmlNodeType.None;
			}
		}

		public override string Prefix
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.Prefix : string.Empty;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.QuoteChar : this.sourceReader.QuoteChar;
			}
		}

		public XmlReader Reader
		{
			get
			{
				return this.sourceReader;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				if (this.validatingReader == null)
				{
					return ReadState.Initial;
				}
				return this.validatingReader.ReadState;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				if (this.xmlTextReader != null)
				{
					return this.xmlTextReader.Resolver;
				}
				if (this.resolverSpecified)
				{
					return this.resolver;
				}
				return null;
			}
		}

		public XmlSchemaCollection Schemas
		{
			get
			{
				if (this.schemas == null)
				{
					this.schemas = new XmlSchemaCollection(this.NameTable);
				}
				return this.schemas;
			}
		}

		public object SchemaType
		{
			get
			{
				return this.schemaInfo.SchemaType;
			}
		}

		[MonoTODO]
		public override XmlReaderSettings Settings
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.Settings : this.sourceReader.Settings;
			}
		}

		[MonoTODO]
		public ValidationType ValidationType
		{
			get
			{
				return this.validationType;
			}
			set
			{
				if (this.ReadState != ReadState.Initial)
				{
					throw new InvalidOperationException("ValidationType cannot be set after the first call to Read method.");
				}
				switch (this.validationType)
				{
				case ValidationType.None:
				case ValidationType.Auto:
				case ValidationType.DTD:
				case ValidationType.Schema:
					this.validationType = value;
					break;
				case ValidationType.XDR:
					throw new NotSupportedException();
				}
			}
		}

		public override string Value
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.Value : string.Empty;
			}
		}

		public override string XmlLang
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.XmlLang : string.Empty;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				this.resolverSpecified = true;
				this.resolver = value;
				if (this.xmlTextReader != null)
				{
					this.xmlTextReader.XmlResolver = value;
				}
				XsdValidatingReader xsdValidatingReader = this.validatingReader as XsdValidatingReader;
				if (xsdValidatingReader != null)
				{
					xsdValidatingReader.XmlResolver = value;
				}
				DTDValidatingReader dtdvalidatingReader = this.validatingReader as DTDValidatingReader;
				if (dtdvalidatingReader != null)
				{
					dtdvalidatingReader.XmlResolver = value;
				}
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return (this.validatingReader != null) ? this.validatingReader.XmlSpace : XmlSpace.None;
			}
		}

		public override void Close()
		{
			if (this.validatingReader == null)
			{
				this.sourceReader.Close();
			}
			else
			{
				this.validatingReader.Close();
			}
		}

		public override string GetAttribute(int i)
		{
			if (this.validatingReader == null)
			{
				throw new IndexOutOfRangeException("Reader is not started.");
			}
			return this.validatingReader[i];
		}

		public override string GetAttribute(string name)
		{
			return (this.validatingReader != null) ? this.validatingReader[name] : null;
		}

		public override string GetAttribute(string localName, string namespaceName)
		{
			return (this.validatingReader != null) ? this.validatingReader[localName, namespaceName] : null;
		}

		public bool HasLineInfo()
		{
			IXmlLineInfo xmlLineInfo = this.validatingReader as IXmlLineInfo;
			return xmlLineInfo != null && xmlLineInfo.HasLineInfo();
		}

		public override string LookupNamespace(string prefix)
		{
			if (this.validatingReader != null)
			{
				return this.validatingReader.LookupNamespace(prefix);
			}
			return this.sourceReader.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			if (this.validatingReader == null)
			{
				throw new IndexOutOfRangeException("Reader is not started.");
			}
			this.validatingReader.MoveToAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			return this.validatingReader != null && this.validatingReader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string localName, string namespaceName)
		{
			return this.validatingReader != null && this.validatingReader.MoveToAttribute(localName, namespaceName);
		}

		public override bool MoveToElement()
		{
			return this.validatingReader != null && this.validatingReader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return this.validatingReader != null && this.validatingReader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this.validatingReader != null && this.validatingReader.MoveToNextAttribute();
		}

		[MonoTODO]
		public override bool Read()
		{
			if (this.validatingReader == null)
			{
				switch (this.ValidationType)
				{
				case ValidationType.None:
				case ValidationType.Auto:
					break;
				case ValidationType.DTD:
					this.validatingReader = (this.dtdReader = new DTDValidatingReader(this.sourceReader, this));
					this.dtdReader.XmlResolver = this.Resolver;
					goto IL_00F3;
				case ValidationType.XDR:
					throw new NotSupportedException();
				case ValidationType.Schema:
					break;
				default:
					goto IL_00F3;
				}
				this.dtdReader = new DTDValidatingReader(this.sourceReader, this);
				XsdValidatingReader xsdValidatingReader = new XsdValidatingReader(this.dtdReader);
				XsdValidatingReader xsdValidatingReader2 = xsdValidatingReader;
				xsdValidatingReader2.ValidationEventHandler = (ValidationEventHandler)Delegate.Combine(xsdValidatingReader2.ValidationEventHandler, new ValidationEventHandler(this.OnValidationEvent));
				xsdValidatingReader.ValidationType = this.ValidationType;
				xsdValidatingReader.Schemas = this.Schemas.SchemaSet;
				xsdValidatingReader.XmlResolver = this.Resolver;
				this.validatingReader = xsdValidatingReader;
				this.dtdReader.XmlResolver = this.Resolver;
				IL_00F3:
				this.schemaInfo = this.validatingReader as IHasXmlSchemaInfo;
			}
			return this.validatingReader.Read();
		}

		public override bool ReadAttributeValue()
		{
			return this.validatingReader != null && this.validatingReader.ReadAttributeValue();
		}

		public override string ReadString()
		{
			return base.ReadString();
		}

		public object ReadTypedValue()
		{
			if (this.dtdReader == null)
			{
				return null;
			}
			XmlSchemaDatatype xmlSchemaDatatype = this.schemaInfo.SchemaType as XmlSchemaDatatype;
			if (xmlSchemaDatatype == null)
			{
				XmlSchemaType xmlSchemaType = this.schemaInfo.SchemaType as XmlSchemaType;
				if (xmlSchemaType != null)
				{
					xmlSchemaDatatype = xmlSchemaType.Datatype;
				}
			}
			if (xmlSchemaDatatype == null)
			{
				return null;
			}
			XmlNodeType nodeType = this.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					return null;
				}
				return xmlSchemaDatatype.ParseValue(this.Value, this.NameTable, this.dtdReader.ParserContext.NamespaceManager);
			}
			else
			{
				if (this.IsEmptyElement)
				{
					return null;
				}
				this.storedCharacters.Length = 0;
				bool flag = true;
				for (;;)
				{
					this.Read();
					XmlNodeType nodeType2 = this.NodeType;
					switch (nodeType2)
					{
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						goto IL_00C6;
					default:
						if (nodeType2 == XmlNodeType.Whitespace || nodeType2 == XmlNodeType.SignificantWhitespace)
						{
							goto IL_00C6;
						}
						flag = false;
						break;
					case XmlNodeType.Comment:
						break;
					}
					IL_00E9:
					if (!flag || this.EOF)
					{
						break;
					}
					continue;
					IL_00C6:
					this.storedCharacters.Append(this.Value);
					goto IL_00E9;
				}
				return xmlSchemaDatatype.ParseValue(this.storedCharacters.ToString(), this.NameTable, this.dtdReader.ParserContext.NamespaceManager);
			}
		}

		public override void ResolveEntity()
		{
			this.validatingReader.ResolveEntity();
		}

		internal void OnValidationEvent(object o, ValidationEventArgs e)
		{
			if (this.ValidationEventHandler != null)
			{
				this.ValidationEventHandler(o, e);
			}
			else if (this.ValidationType != ValidationType.None && e.Severity == XmlSeverityType.Error)
			{
				throw e.Exception;
			}
		}

		[MonoTODO]
		public override int ReadContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (this.validatingReader != null)
			{
				return this.validatingReader.ReadContentAsBase64(buffer, offset, length);
			}
			return this.sourceReader.ReadContentAsBase64(buffer, offset, length);
		}

		[MonoTODO]
		public override int ReadContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (this.validatingReader != null)
			{
				return this.validatingReader.ReadContentAsBinHex(buffer, offset, length);
			}
			return this.sourceReader.ReadContentAsBinHex(buffer, offset, length);
		}

		[MonoTODO]
		public override int ReadElementContentAsBase64(byte[] buffer, int offset, int length)
		{
			if (this.validatingReader != null)
			{
				return this.validatingReader.ReadElementContentAsBase64(buffer, offset, length);
			}
			return this.sourceReader.ReadElementContentAsBase64(buffer, offset, length);
		}

		[MonoTODO]
		public override int ReadElementContentAsBinHex(byte[] buffer, int offset, int length)
		{
			if (this.validatingReader != null)
			{
				return this.validatingReader.ReadElementContentAsBinHex(buffer, offset, length);
			}
			return this.sourceReader.ReadElementContentAsBinHex(buffer, offset, length);
		}

		private EntityHandling entityHandling;

		private XmlReader sourceReader;

		private XmlTextReader xmlTextReader;

		private XmlReader validatingReader;

		private XmlResolver resolver;

		private bool resolverSpecified;

		private ValidationType validationType;

		private XmlSchemaCollection schemas;

		private DTDValidatingReader dtdReader;

		private IHasXmlSchemaInfo schemaInfo;

		private StringBuilder storedCharacters;
	}
}
