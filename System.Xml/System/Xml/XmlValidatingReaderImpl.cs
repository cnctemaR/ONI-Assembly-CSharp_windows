using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace System.Xml
{
	internal sealed class XmlValidatingReaderImpl : XmlReader, IXmlLineInfo, IXmlNamespaceResolver
	{
		internal XmlValidatingReaderImpl(XmlReader reader)
		{
			XmlAsyncCheckReader xmlAsyncCheckReader = reader as XmlAsyncCheckReader;
			if (xmlAsyncCheckReader != null)
			{
				reader = xmlAsyncCheckReader.CoreReader;
			}
			this.outerReader = this;
			this.coreReader = reader;
			this.coreReaderNSResolver = reader as IXmlNamespaceResolver;
			this.coreReaderImpl = reader as XmlTextReaderImpl;
			if (this.coreReaderImpl == null)
			{
				XmlTextReader xmlTextReader = reader as XmlTextReader;
				if (xmlTextReader != null)
				{
					this.coreReaderImpl = xmlTextReader.Impl;
				}
			}
			if (this.coreReaderImpl == null)
			{
				throw new ArgumentException(Res.GetString("The XmlReader passed in to construct this XmlValidatingReaderImpl must be an instance of a System.Xml.XmlTextReader."), "reader");
			}
			this.coreReaderImpl.EntityHandling = EntityHandling.ExpandEntities;
			this.coreReaderImpl.XmlValidatingReaderCompatibilityMode = true;
			this.processIdentityConstraints = true;
			this.schemaCollection = new XmlSchemaCollection(this.coreReader.NameTable);
			this.schemaCollection.XmlResolver = this.GetResolver();
			this.eventHandling = new XmlValidatingReaderImpl.ValidationEventHandling(this);
			this.coreReaderImpl.ValidationEventHandling = this.eventHandling;
			this.coreReaderImpl.OnDefaultAttributeUse = new XmlTextReaderImpl.OnDefaultAttributeUseDelegate(this.ValidateDefaultAttributeOnUse);
			this.validationType = ValidationType.Auto;
			this.SetupValidation(ValidationType.Auto);
		}

		internal XmlValidatingReaderImpl(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
			: this(new XmlTextReader(xmlFragment, fragType, context))
		{
			if (this.coreReader.BaseURI.Length > 0)
			{
				this.validator.BaseUri = this.GetResolver().ResolveUri(null, this.coreReader.BaseURI);
			}
			if (context != null)
			{
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.ParseDtdFromContext;
				this.parserContext = context;
			}
		}

		internal XmlValidatingReaderImpl(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
			: this(new XmlTextReader(xmlFragment, fragType, context))
		{
			if (this.coreReader.BaseURI.Length > 0)
			{
				this.validator.BaseUri = this.GetResolver().ResolveUri(null, this.coreReader.BaseURI);
			}
			if (context != null)
			{
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.ParseDtdFromContext;
				this.parserContext = context;
			}
		}

		internal XmlValidatingReaderImpl(XmlReader reader, ValidationEventHandler settingsEventHandler, bool processIdentityConstraints)
		{
			XmlAsyncCheckReader xmlAsyncCheckReader = reader as XmlAsyncCheckReader;
			if (xmlAsyncCheckReader != null)
			{
				reader = xmlAsyncCheckReader.CoreReader;
			}
			this.outerReader = this;
			this.coreReader = reader;
			this.coreReaderImpl = reader as XmlTextReaderImpl;
			if (this.coreReaderImpl == null)
			{
				XmlTextReader xmlTextReader = reader as XmlTextReader;
				if (xmlTextReader != null)
				{
					this.coreReaderImpl = xmlTextReader.Impl;
				}
			}
			if (this.coreReaderImpl == null)
			{
				throw new ArgumentException(Res.GetString("The XmlReader passed in to construct this XmlValidatingReaderImpl must be an instance of a System.Xml.XmlTextReader."), "reader");
			}
			this.coreReaderImpl.XmlValidatingReaderCompatibilityMode = true;
			this.coreReaderNSResolver = reader as IXmlNamespaceResolver;
			this.processIdentityConstraints = processIdentityConstraints;
			this.schemaCollection = new XmlSchemaCollection(this.coreReader.NameTable);
			this.schemaCollection.XmlResolver = this.GetResolver();
			this.eventHandling = new XmlValidatingReaderImpl.ValidationEventHandling(this);
			if (settingsEventHandler != null)
			{
				this.eventHandling.AddHandler(settingsEventHandler);
			}
			this.coreReaderImpl.ValidationEventHandling = this.eventHandling;
			this.coreReaderImpl.OnDefaultAttributeUse = new XmlTextReaderImpl.OnDefaultAttributeUseDelegate(this.ValidateDefaultAttributeOnUse);
			this.validationType = ValidationType.DTD;
			this.SetupValidation(ValidationType.DTD);
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				XmlReaderSettings xmlReaderSettings;
				if (this.coreReaderImpl.V1Compat)
				{
					xmlReaderSettings = null;
				}
				else
				{
					xmlReaderSettings = this.coreReader.Settings;
				}
				if (xmlReaderSettings != null)
				{
					xmlReaderSettings = xmlReaderSettings.Clone();
				}
				else
				{
					xmlReaderSettings = new XmlReaderSettings();
				}
				xmlReaderSettings.ValidationType = ValidationType.DTD;
				if (!this.processIdentityConstraints)
				{
					xmlReaderSettings.ValidationFlags &= ~XmlSchemaValidationFlags.ProcessIdentityConstraints;
				}
				xmlReaderSettings.ReadOnly = true;
				return xmlReaderSettings;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.coreReader.NodeType;
			}
		}

		public override string Name
		{
			get
			{
				return this.coreReader.Name;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.coreReader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.coreReader.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.coreReader.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.coreReader.HasValue;
			}
		}

		public override string Value
		{
			get
			{
				return this.coreReader.Value;
			}
		}

		public override int Depth
		{
			get
			{
				return this.coreReader.Depth;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.coreReader.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.coreReader.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.coreReader.IsDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.coreReader.QuoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.coreReader.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.coreReader.XmlLang;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				if (this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.Init)
				{
					return this.coreReader.ReadState;
				}
				return ReadState.Initial;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.coreReader.EOF;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.coreReader.NameTable;
			}
		}

		internal Encoding Encoding
		{
			get
			{
				return this.coreReaderImpl.Encoding;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.coreReader.AttributeCount;
			}
		}

		public override string GetAttribute(string name)
		{
			return this.coreReader.GetAttribute(name);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return this.coreReader.GetAttribute(localName, namespaceURI);
		}

		public override string GetAttribute(int i)
		{
			return this.coreReader.GetAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			if (!this.coreReader.MoveToAttribute(name))
			{
				return false;
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			return true;
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			if (!this.coreReader.MoveToAttribute(localName, namespaceURI))
			{
				return false;
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			return true;
		}

		public override void MoveToAttribute(int i)
		{
			this.coreReader.MoveToAttribute(i);
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
		}

		public override bool MoveToFirstAttribute()
		{
			if (!this.coreReader.MoveToFirstAttribute())
			{
				return false;
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (!this.coreReader.MoveToNextAttribute())
			{
				return false;
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			return true;
		}

		public override bool MoveToElement()
		{
			if (!this.coreReader.MoveToElement())
			{
				return false;
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			return true;
		}

		public override bool Read()
		{
			switch (this.parsingFunction)
			{
			case XmlValidatingReaderImpl.ParsingFunction.Read:
				break;
			case XmlValidatingReaderImpl.ParsingFunction.Init:
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				if (this.coreReader.ReadState == ReadState.Interactive)
				{
					this.ProcessCoreReaderEvent();
					return true;
				}
				break;
			case XmlValidatingReaderImpl.ParsingFunction.ParseDtdFromContext:
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				this.ParseDtdFromParserContext();
				break;
			case XmlValidatingReaderImpl.ParsingFunction.ResolveEntityInternally:
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				this.ResolveEntityInternally();
				break;
			case XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent:
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				this.readBinaryHelper.Finish();
				break;
			case XmlValidatingReaderImpl.ParsingFunction.ReaderClosed:
			case XmlValidatingReaderImpl.ParsingFunction.Error:
				return false;
			default:
				return false;
			}
			if (this.coreReader.Read())
			{
				this.ProcessCoreReaderEvent();
				return true;
			}
			this.validator.CompleteValidation();
			return false;
		}

		public override void Close()
		{
			this.coreReader.Close();
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.ReaderClosed;
		}

		public override string LookupNamespace(string prefix)
		{
			return this.coreReaderImpl.LookupNamespace(prefix);
		}

		public override bool ReadAttributeValue()
		{
			if (this.parsingFunction == XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent)
			{
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				this.readBinaryHelper.Finish();
			}
			if (!this.coreReader.ReadAttributeValue())
			{
				return false;
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			return true;
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
			if (this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this.outerReader);
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			int num = this.readBinaryHelper.ReadContentAsBase64(buffer, index, count);
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent;
			return num;
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this.outerReader);
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			int num = this.readBinaryHelper.ReadContentAsBinHex(buffer, index, count);
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent;
			return num;
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this.outerReader);
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			int num = this.readBinaryHelper.ReadElementContentAsBase64(buffer, index, count);
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent;
			return num;
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this.outerReader);
			}
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			int num = this.readBinaryHelper.ReadElementContentAsBinHex(buffer, index, count);
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent;
			return num;
		}

		public override bool CanResolveEntity
		{
			get
			{
				return true;
			}
		}

		public override void ResolveEntity()
		{
			if (this.parsingFunction == XmlValidatingReaderImpl.ParsingFunction.ResolveEntityInternally)
			{
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
			}
			this.coreReader.ResolveEntity();
		}

		internal XmlReader OuterReader
		{
			get
			{
				return this.outerReader;
			}
			set
			{
				this.outerReader = value;
			}
		}

		internal void MoveOffEntityReference()
		{
			if (this.outerReader.NodeType == XmlNodeType.EntityReference && this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.ResolveEntityInternally && !this.outerReader.Read())
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
			}
		}

		public override string ReadString()
		{
			this.MoveOffEntityReference();
			return base.ReadString();
		}

		public bool HasLineInfo()
		{
			return true;
		}

		public int LineNumber
		{
			get
			{
				return ((IXmlLineInfo)this.coreReader).LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return ((IXmlLineInfo)this.coreReader).LinePosition;
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			return this.LookupNamespace(prefix);
		}

		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			return this.LookupPrefix(namespaceName);
		}

		internal IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.coreReaderNSResolver.GetNamespacesInScope(scope);
		}

		internal string LookupPrefix(string namespaceName)
		{
			return this.coreReaderNSResolver.LookupPrefix(namespaceName);
		}

		internal event ValidationEventHandler ValidationEventHandler
		{
			add
			{
				this.eventHandling.AddHandler(value);
			}
			remove
			{
				this.eventHandling.RemoveHandler(value);
			}
		}

		internal object SchemaType
		{
			get
			{
				if (this.validationType == ValidationType.None)
				{
					return null;
				}
				XmlSchemaType xmlSchemaType = this.coreReaderImpl.InternalSchemaType as XmlSchemaType;
				if (xmlSchemaType != null && xmlSchemaType.QualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema")
				{
					return xmlSchemaType.Datatype;
				}
				return this.coreReaderImpl.InternalSchemaType;
			}
		}

		internal XmlReader Reader
		{
			get
			{
				return this.coreReader;
			}
		}

		internal XmlTextReaderImpl ReaderImpl
		{
			get
			{
				return this.coreReaderImpl;
			}
		}

		internal ValidationType ValidationType
		{
			get
			{
				return this.validationType;
			}
			set
			{
				if (this.ReadState != ReadState.Initial)
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
				}
				this.validationType = value;
				this.SetupValidation(value);
			}
		}

		internal XmlSchemaCollection Schemas
		{
			get
			{
				return this.schemaCollection;
			}
		}

		internal EntityHandling EntityHandling
		{
			get
			{
				return this.coreReaderImpl.EntityHandling;
			}
			set
			{
				this.coreReaderImpl.EntityHandling = value;
			}
		}

		internal XmlResolver XmlResolver
		{
			set
			{
				this.coreReaderImpl.XmlResolver = value;
				this.validator.XmlResolver = value;
				this.schemaCollection.XmlResolver = value;
			}
		}

		internal bool Namespaces
		{
			get
			{
				return this.coreReaderImpl.Namespaces;
			}
			set
			{
				this.coreReaderImpl.Namespaces = value;
			}
		}

		public object ReadTypedValue()
		{
			if (this.validationType == ValidationType.None)
			{
				return null;
			}
			XmlNodeType nodeType = this.outerReader.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType == XmlNodeType.Attribute)
				{
					return this.coreReaderImpl.InternalTypedValue;
				}
				if (nodeType == XmlNodeType.EndElement)
				{
					return null;
				}
				if (this.coreReaderImpl.V1Compat)
				{
					return null;
				}
				return this.Value;
			}
			else
			{
				if (this.SchemaType == null)
				{
					return null;
				}
				if (((this.SchemaType is XmlSchemaDatatype) ? ((XmlSchemaDatatype)this.SchemaType) : ((XmlSchemaType)this.SchemaType).Datatype) != null)
				{
					if (!this.outerReader.IsEmptyElement)
					{
						while (this.outerReader.Read())
						{
							XmlNodeType nodeType2 = this.outerReader.NodeType;
							if (nodeType2 != XmlNodeType.CDATA && nodeType2 != XmlNodeType.Text && nodeType2 != XmlNodeType.Whitespace && nodeType2 != XmlNodeType.SignificantWhitespace && nodeType2 != XmlNodeType.Comment && nodeType2 != XmlNodeType.ProcessingInstruction)
							{
								if (this.outerReader.NodeType != XmlNodeType.EndElement)
								{
									throw new XmlException("'{0}' is an invalid XmlNodeType.", this.outerReader.NodeType.ToString());
								}
								goto IL_00F3;
							}
						}
						throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
					}
					IL_00F3:
					return this.coreReaderImpl.InternalTypedValue;
				}
				return null;
			}
		}

		private void ParseDtdFromParserContext()
		{
			if (this.parserContext.DocTypeName == null || this.parserContext.DocTypeName.Length == 0)
			{
				return;
			}
			IDtdParser dtdParser = DtdParser.Create();
			XmlTextReaderImpl.DtdParserProxy dtdParserProxy = new XmlTextReaderImpl.DtdParserProxy(this.coreReaderImpl);
			IDtdInfo dtdInfo = dtdParser.ParseFreeFloatingDtd(this.parserContext.BaseURI, this.parserContext.DocTypeName, this.parserContext.PublicId, this.parserContext.SystemId, this.parserContext.InternalSubset, dtdParserProxy);
			this.coreReaderImpl.SetDtdInfo(dtdInfo);
			this.ValidateDtd();
		}

		private void ValidateDtd()
		{
			IDtdInfo dtdInfo = this.coreReaderImpl.DtdInfo;
			if (dtdInfo != null)
			{
				switch (this.validationType)
				{
				case ValidationType.None:
				case ValidationType.DTD:
					break;
				case ValidationType.Auto:
					this.SetupValidation(ValidationType.DTD);
					break;
				default:
					return;
				}
				this.validator.DtdInfo = dtdInfo;
			}
		}

		private void ResolveEntityInternally()
		{
			int depth = this.coreReader.Depth;
			this.outerReader.ResolveEntity();
			while (this.outerReader.Read() && this.coreReader.Depth > depth)
			{
			}
		}

		private void SetupValidation(ValidationType valType)
		{
			this.validator = BaseValidator.CreateInstance(valType, this, this.schemaCollection, this.eventHandling, this.processIdentityConstraints);
			XmlResolver resolver = this.GetResolver();
			this.validator.XmlResolver = resolver;
			if (this.outerReader.BaseURI.Length > 0)
			{
				this.validator.BaseUri = ((resolver == null) ? new Uri(this.outerReader.BaseURI, UriKind.RelativeOrAbsolute) : resolver.ResolveUri(null, this.outerReader.BaseURI));
			}
			this.coreReaderImpl.ValidationEventHandling = ((this.validationType == ValidationType.None) ? null : this.eventHandling);
		}

		private XmlResolver GetResolver()
		{
			XmlResolver resolver = this.coreReaderImpl.GetResolver();
			if (resolver == null && !this.coreReaderImpl.IsResolverSet && !XmlReaderSettings.EnableLegacyXmlSettings())
			{
				if (XmlValidatingReaderImpl.s_tempResolver == null)
				{
					XmlValidatingReaderImpl.s_tempResolver = new XmlUrlResolver();
				}
				return XmlValidatingReaderImpl.s_tempResolver;
			}
			return resolver;
		}

		private void ProcessCoreReaderEvent()
		{
			XmlNodeType nodeType = this.coreReader.NodeType;
			if (nodeType != XmlNodeType.EntityReference)
			{
				if (nodeType == XmlNodeType.DocumentType)
				{
					this.ValidateDtd();
					return;
				}
				if (nodeType == XmlNodeType.Whitespace && (this.coreReader.Depth > 0 || this.coreReaderImpl.FragmentType != XmlNodeType.Document) && this.validator.PreserveWhitespace)
				{
					this.coreReaderImpl.ChangeCurrentNodeType(XmlNodeType.SignificantWhitespace);
				}
			}
			else
			{
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.ResolveEntityInternally;
			}
			this.coreReaderImpl.InternalSchemaType = null;
			this.coreReaderImpl.InternalTypedValue = null;
			this.validator.Validate();
		}

		internal void Close(bool closeStream)
		{
			this.coreReaderImpl.Close(closeStream);
			this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.ReaderClosed;
		}

		internal BaseValidator Validator
		{
			get
			{
				return this.validator;
			}
			set
			{
				this.validator = value;
			}
		}

		internal override XmlNamespaceManager NamespaceManager
		{
			get
			{
				return this.coreReaderImpl.NamespaceManager;
			}
		}

		internal bool StandAlone
		{
			get
			{
				return this.coreReaderImpl.StandAlone;
			}
		}

		internal object SchemaTypeObject
		{
			set
			{
				this.coreReaderImpl.InternalSchemaType = value;
			}
		}

		internal object TypedValueObject
		{
			get
			{
				return this.coreReaderImpl.InternalTypedValue;
			}
			set
			{
				this.coreReaderImpl.InternalTypedValue = value;
			}
		}

		internal bool Normalization
		{
			get
			{
				return this.coreReaderImpl.Normalization;
			}
		}

		internal bool AddDefaultAttribute(SchemaAttDef attdef)
		{
			return this.coreReaderImpl.AddDefaultAttributeNonDtd(attdef);
		}

		internal override IDtdInfo DtdInfo
		{
			get
			{
				return this.coreReaderImpl.DtdInfo;
			}
		}

		internal void ValidateDefaultAttributeOnUse(IDtdDefaultAttributeInfo defaultAttribute, XmlTextReaderImpl coreReader)
		{
			SchemaAttDef schemaAttDef = defaultAttribute as SchemaAttDef;
			if (schemaAttDef == null)
			{
				return;
			}
			if (!schemaAttDef.DefaultValueChecked)
			{
				SchemaInfo schemaInfo = coreReader.DtdInfo as SchemaInfo;
				if (schemaInfo == null)
				{
					return;
				}
				DtdValidator.CheckDefaultValue(schemaAttDef, schemaInfo, this.eventHandling, coreReader.BaseURI);
			}
		}

		public override Task<string> GetValueAsync()
		{
			return this.coreReader.GetValueAsync();
		}

		public override async Task<bool> ReadAsync()
		{
			switch (this.parsingFunction)
			{
			case XmlValidatingReaderImpl.ParsingFunction.Read:
				break;
			case XmlValidatingReaderImpl.ParsingFunction.Init:
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				if (this.coreReader.ReadState == ReadState.Interactive)
				{
					this.ProcessCoreReaderEvent();
					return true;
				}
				break;
			case XmlValidatingReaderImpl.ParsingFunction.ParseDtdFromContext:
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				await this.ParseDtdFromParserContextAsync().ConfigureAwait(false);
				break;
			case XmlValidatingReaderImpl.ParsingFunction.ResolveEntityInternally:
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				await this.ResolveEntityInternallyAsync().ConfigureAwait(false);
				break;
			case XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent:
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				await this.readBinaryHelper.FinishAsync().ConfigureAwait(false);
				break;
			case XmlValidatingReaderImpl.ParsingFunction.ReaderClosed:
			case XmlValidatingReaderImpl.ParsingFunction.Error:
				return false;
			default:
				return false;
			}
			ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.coreReader.ReadAsync().ConfigureAwait(false).GetAwaiter();
			if (!configuredTaskAwaiter.IsCompleted)
			{
				await configuredTaskAwaiter;
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
				configuredTaskAwaiter = configuredTaskAwaiter2;
				configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
			}
			bool flag;
			if (configuredTaskAwaiter.GetResult())
			{
				this.ProcessCoreReaderEvent();
				flag = true;
			}
			else
			{
				this.validator.CompleteValidation();
				flag = false;
			}
			return flag;
		}

		public override async Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count)
		{
			int num;
			if (this.ReadState != ReadState.Interactive)
			{
				num = 0;
			}
			else
			{
				if (this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent)
				{
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this.outerReader);
				}
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				int num2 = await this.readBinaryHelper.ReadContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent;
				num = num2;
			}
			return num;
		}

		public override async Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			int num;
			if (this.ReadState != ReadState.Interactive)
			{
				num = 0;
			}
			else
			{
				if (this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent)
				{
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this.outerReader);
				}
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				int num2 = await this.readBinaryHelper.ReadContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent;
				num = num2;
			}
			return num;
		}

		public override async Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count)
		{
			int num;
			if (this.ReadState != ReadState.Interactive)
			{
				num = 0;
			}
			else
			{
				if (this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent)
				{
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this.outerReader);
				}
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				int num2 = await this.readBinaryHelper.ReadElementContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent;
				num = num2;
			}
			return num;
		}

		public override async Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			int num;
			if (this.ReadState != ReadState.Interactive)
			{
				num = 0;
			}
			else
			{
				if (this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent)
				{
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this.outerReader);
				}
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Read;
				int num2 = await this.readBinaryHelper.ReadElementContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
				this.parsingFunction = XmlValidatingReaderImpl.ParsingFunction.InReadBinaryContent;
				num = num2;
			}
			return num;
		}

		internal async Task MoveOffEntityReferenceAsync()
		{
			if (this.outerReader.NodeType == XmlNodeType.EntityReference && this.parsingFunction != XmlValidatingReaderImpl.ParsingFunction.ResolveEntityInternally)
			{
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.outerReader.ReadAsync().ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
				if (!configuredTaskAwaiter.GetResult())
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
				}
			}
		}

		public async Task<object> ReadTypedValueAsync()
		{
			object obj;
			if (this.validationType == ValidationType.None)
			{
				obj = null;
			}
			else
			{
				XmlNodeType nodeType = this.outerReader.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType != XmlNodeType.EndElement)
						{
							if (this.coreReaderImpl.V1Compat)
							{
								obj = null;
							}
							else
							{
								obj = await this.GetValueAsync().ConfigureAwait(false);
							}
						}
						else
						{
							obj = null;
						}
					}
					else
					{
						obj = this.coreReaderImpl.InternalTypedValue;
					}
				}
				else if (this.SchemaType == null)
				{
					obj = null;
				}
				else if (((this.SchemaType is XmlSchemaDatatype) ? ((XmlSchemaDatatype)this.SchemaType) : ((XmlSchemaType)this.SchemaType).Datatype) != null)
				{
					if (!this.outerReader.IsEmptyElement)
					{
						for (;;)
						{
							ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.outerReader.ReadAsync().ConfigureAwait(false).GetAwaiter();
							if (!configuredTaskAwaiter.IsCompleted)
							{
								await configuredTaskAwaiter;
								ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
								configuredTaskAwaiter = configuredTaskAwaiter2;
								configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
							}
							if (!configuredTaskAwaiter.GetResult())
							{
								break;
							}
							XmlNodeType nodeType2 = this.outerReader.NodeType;
							if (nodeType2 != XmlNodeType.CDATA && nodeType2 != XmlNodeType.Text && nodeType2 != XmlNodeType.Whitespace && nodeType2 != XmlNodeType.SignificantWhitespace && nodeType2 != XmlNodeType.Comment && nodeType2 != XmlNodeType.ProcessingInstruction)
							{
								goto Block_15;
							}
						}
						throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
						Block_15:
						if (this.outerReader.NodeType != XmlNodeType.EndElement)
						{
							throw new XmlException("'{0}' is an invalid XmlNodeType.", this.outerReader.NodeType.ToString());
						}
					}
					obj = this.coreReaderImpl.InternalTypedValue;
				}
				else
				{
					obj = null;
				}
			}
			return obj;
		}

		private async Task ParseDtdFromParserContextAsync()
		{
			if (this.parserContext.DocTypeName != null && this.parserContext.DocTypeName.Length != 0)
			{
				IDtdParser dtdParser = DtdParser.Create();
				XmlTextReaderImpl.DtdParserProxy dtdParserProxy = new XmlTextReaderImpl.DtdParserProxy(this.coreReaderImpl);
				IDtdInfo dtdInfo = await dtdParser.ParseFreeFloatingDtdAsync(this.parserContext.BaseURI, this.parserContext.DocTypeName, this.parserContext.PublicId, this.parserContext.SystemId, this.parserContext.InternalSubset, dtdParserProxy).ConfigureAwait(false);
				this.coreReaderImpl.SetDtdInfo(dtdInfo);
				this.ValidateDtd();
			}
		}

		private async Task ResolveEntityInternallyAsync()
		{
			int initialDepth = this.coreReader.Depth;
			this.outerReader.ResolveEntity();
			ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter;
			do
			{
				configuredTaskAwaiter = this.outerReader.ReadAsync().ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
			}
			while (configuredTaskAwaiter.GetResult() && this.coreReader.Depth > initialDepth);
		}

		private XmlReader coreReader;

		private XmlTextReaderImpl coreReaderImpl;

		private IXmlNamespaceResolver coreReaderNSResolver;

		private ValidationType validationType;

		private BaseValidator validator;

		private XmlSchemaCollection schemaCollection;

		private bool processIdentityConstraints;

		private XmlValidatingReaderImpl.ParsingFunction parsingFunction = XmlValidatingReaderImpl.ParsingFunction.Init;

		private XmlValidatingReaderImpl.ValidationEventHandling eventHandling;

		private XmlParserContext parserContext;

		private ReadContentAsBinaryHelper readBinaryHelper;

		private XmlReader outerReader;

		private static XmlResolver s_tempResolver;

		private enum ParsingFunction
		{
			Read,
			Init,
			ParseDtdFromContext,
			ResolveEntityInternally,
			InReadBinaryContent,
			ReaderClosed,
			Error,
			None
		}

		internal class ValidationEventHandling : IValidationEventHandling
		{
			internal ValidationEventHandling(XmlValidatingReaderImpl reader)
			{
				this.reader = reader;
			}

			object IValidationEventHandling.EventHandler
			{
				get
				{
					return this.eventHandler;
				}
			}

			void IValidationEventHandling.SendEvent(Exception exception, XmlSeverityType severity)
			{
				if (this.eventHandler != null)
				{
					this.eventHandler(this.reader, new ValidationEventArgs((XmlSchemaException)exception, severity));
					return;
				}
				if (this.reader.ValidationType != ValidationType.None && severity == XmlSeverityType.Error)
				{
					throw exception;
				}
			}

			internal void AddHandler(ValidationEventHandler handler)
			{
				this.eventHandler = (ValidationEventHandler)Delegate.Combine(this.eventHandler, handler);
			}

			internal void RemoveHandler(ValidationEventHandler handler)
			{
				this.eventHandler = (ValidationEventHandler)Delegate.Remove(this.eventHandler, handler);
			}

			private XmlValidatingReaderImpl reader;

			private ValidationEventHandler eventHandler;
		}
	}
}
