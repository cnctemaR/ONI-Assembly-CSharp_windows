using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace System.Xml
{
	internal class XsdValidatingReader : XmlReader, IXmlSchemaInfo, IXmlLineInfo, IXmlNamespaceResolver
	{
		internal XsdValidatingReader(XmlReader reader, XmlResolver xmlResolver, XmlReaderSettings readerSettings, XmlSchemaObject partialValidationType)
		{
			this.coreReader = reader;
			this.coreReaderNSResolver = reader as IXmlNamespaceResolver;
			this.lineInfo = reader as IXmlLineInfo;
			this.coreReaderNameTable = this.coreReader.NameTable;
			if (this.coreReaderNSResolver == null)
			{
				this.nsManager = new XmlNamespaceManager(this.coreReaderNameTable);
				this.manageNamespaces = true;
			}
			this.thisNSResolver = this;
			this.xmlResolver = xmlResolver;
			this.processInlineSchema = (readerSettings.ValidationFlags & XmlSchemaValidationFlags.ProcessInlineSchema) > XmlSchemaValidationFlags.None;
			this.Init();
			this.SetupValidator(readerSettings, reader, partialValidationType);
			this.validationEvent = readerSettings.GetEventHandler();
		}

		internal XsdValidatingReader(XmlReader reader, XmlResolver xmlResolver, XmlReaderSettings readerSettings)
			: this(reader, xmlResolver, readerSettings, null)
		{
		}

		private void Init()
		{
			this.validationState = XsdValidatingReader.ValidatingReaderState.Init;
			this.defaultAttributes = new ArrayList();
			this.currentAttrIndex = -1;
			this.attributePSVINodes = new AttributePSVIInfo[8];
			this.valueGetter = new XmlValueGetter(this.GetStringValue);
			XsdValidatingReader.TypeOfString = typeof(string);
			this.xmlSchemaInfo = new XmlSchemaInfo();
			this.NsXmlNs = this.coreReaderNameTable.Add("http://www.w3.org/2000/xmlns/");
			this.NsXs = this.coreReaderNameTable.Add("http://www.w3.org/2001/XMLSchema");
			this.NsXsi = this.coreReaderNameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
			this.XsiType = this.coreReaderNameTable.Add("type");
			this.XsiNil = this.coreReaderNameTable.Add("nil");
			this.XsiSchemaLocation = this.coreReaderNameTable.Add("schemaLocation");
			this.XsiNoNamespaceSchemaLocation = this.coreReaderNameTable.Add("noNamespaceSchemaLocation");
			this.XsdSchema = this.coreReaderNameTable.Add("schema");
		}

		private void SetupValidator(XmlReaderSettings readerSettings, XmlReader reader, XmlSchemaObject partialValidationType)
		{
			this.validator = new XmlSchemaValidator(this.coreReaderNameTable, readerSettings.Schemas, this.thisNSResolver, readerSettings.ValidationFlags);
			this.validator.XmlResolver = this.xmlResolver;
			this.validator.SourceUri = XmlConvert.ToUri(reader.BaseURI);
			this.validator.ValidationEventSender = this;
			this.validator.ValidationEventHandler += readerSettings.GetEventHandler();
			this.validator.LineInfoProvider = this.lineInfo;
			if (this.validator.ProcessSchemaHints)
			{
				this.validator.SchemaSet.ReaderSettings.DtdProcessing = readerSettings.DtdProcessing;
			}
			this.validator.SetDtdSchemaInfo(reader.DtdInfo);
			if (partialValidationType != null)
			{
				this.validator.Initialize(partialValidationType);
				return;
			}
			this.validator.Initialize();
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				XmlReaderSettings xmlReaderSettings = this.coreReader.Settings;
				if (xmlReaderSettings != null)
				{
					xmlReaderSettings = xmlReaderSettings.Clone();
				}
				if (xmlReaderSettings == null)
				{
					xmlReaderSettings = new XmlReaderSettings();
				}
				xmlReaderSettings.Schemas = this.validator.SchemaSet;
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.ValidationFlags = this.validator.ValidationFlags;
				xmlReaderSettings.ReadOnly = true;
				return xmlReaderSettings;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.NodeType;
				}
				XmlNodeType nodeType = this.coreReader.NodeType;
				if (nodeType == XmlNodeType.Whitespace && (this.validator.CurrentContentType == XmlSchemaContentType.TextOnly || this.validator.CurrentContentType == XmlSchemaContentType.Mixed))
				{
					return XmlNodeType.SignificantWhitespace;
				}
				return nodeType;
			}
		}

		public override string Name
		{
			get
			{
				if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute)
				{
					return this.coreReader.Name;
				}
				string defaultAttributePrefix = this.validator.GetDefaultAttributePrefix(this.cachedNode.Namespace);
				if (defaultAttributePrefix != null && defaultAttributePrefix.Length != 0)
				{
					return string.Concat(new string[] { defaultAttributePrefix + ":" + this.cachedNode.LocalName });
				}
				return this.cachedNode.LocalName;
			}
		}

		public override string LocalName
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.LocalName;
				}
				return this.coreReader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.Namespace;
				}
				return this.coreReader.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.Prefix;
				}
				return this.coreReader.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.validationState < XsdValidatingReader.ValidatingReaderState.None || this.coreReader.HasValue;
			}
		}

		public override string Value
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.RawValue;
				}
				return this.coreReader.Value;
			}
		}

		public override int Depth
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.Depth;
				}
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
				return this.validationState == XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute || this.coreReader.IsDefault;
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

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return this;
			}
		}

		public override Type ValueType
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType != XmlNodeType.EndElement)
						{
							goto IL_0062;
						}
					}
					else
					{
						if (this.attributePSVI != null && this.AttributeSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
						{
							return this.AttributeSchemaInfo.SchemaType.Datatype.ValueType;
						}
						goto IL_0062;
					}
				}
				if (this.xmlSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
				{
					return this.xmlSchemaInfo.SchemaType.Datatype.ValueType;
				}
				IL_0062:
				return XsdValidatingReader.TypeOfString;
			}
		}

		public override object ReadContentAsObject()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsObject");
			}
			return this.InternalReadContentAsObject(true);
		}

		public override bool ReadContentAsBoolean()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsBoolean");
			}
			object obj = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			bool flag;
			try
			{
				if (xmlSchemaType != null)
				{
					flag = xmlSchemaType.ValueConverter.ToBoolean(obj);
				}
				else
				{
					flag = XmlUntypedConverter.Untyped.ToBoolean(obj);
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", ex3, this);
			}
			return flag;
		}

		public override DateTime ReadContentAsDateTime()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsDateTime");
			}
			object obj = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			DateTime dateTime;
			try
			{
				if (xmlSchemaType != null)
				{
					dateTime = xmlSchemaType.ValueConverter.ToDateTime(obj);
				}
				else
				{
					dateTime = XmlUntypedConverter.Untyped.ToDateTime(obj);
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", ex3, this);
			}
			return dateTime;
		}

		public override double ReadContentAsDouble()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsDouble");
			}
			object obj = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			double num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToDouble(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToDouble(obj);
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", ex3, this);
			}
			return num;
		}

		public override float ReadContentAsFloat()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsFloat");
			}
			object obj = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			float num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToSingle(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToSingle(obj);
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", ex3, this);
			}
			return num;
		}

		public override decimal ReadContentAsDecimal()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsDecimal");
			}
			object obj = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			decimal num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToDecimal(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToDecimal(obj);
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", ex3, this);
			}
			return num;
		}

		public override int ReadContentAsInt()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsInt");
			}
			object obj = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			int num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToInt32(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToInt32(obj);
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", ex3, this);
			}
			return num;
		}

		public override long ReadContentAsLong()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsLong");
			}
			object obj = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			long num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToInt64(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToInt64(obj);
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", ex3, this);
			}
			return num;
		}

		public override string ReadContentAsString()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsString");
			}
			object obj = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			string text;
			try
			{
				if (xmlSchemaType != null)
				{
					text = xmlSchemaType.ValueConverter.ToString(obj);
				}
				else
				{
					text = obj as string;
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex3, this);
			}
			return text;
		}

		public override object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAs");
			}
			string text;
			object obj = this.InternalReadContentAsObject(false, out text);
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			object obj2;
			try
			{
				if (xmlSchemaType != null)
				{
					if (returnType == typeof(DateTimeOffset) && xmlSchemaType.Datatype is Datatype_dateTimeBase)
					{
						obj = text;
					}
					obj2 = xmlSchemaType.ValueConverter.ChangeType(obj, returnType);
				}
				else
				{
					obj2 = XmlUntypedConverter.Untyped.ChangeType(obj, returnType, namespaceResolver);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex3, this);
			}
			return obj2;
		}

		public override object ReadElementContentAsObject()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsObject");
			}
			XmlSchemaType xmlSchemaType;
			return this.InternalReadElementContentAsObject(out xmlSchemaType, true);
		}

		public override bool ReadElementContentAsBoolean()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsBoolean");
			}
			XmlSchemaType xmlSchemaType;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType);
			bool flag;
			try
			{
				if (xmlSchemaType != null)
				{
					flag = xmlSchemaType.ValueConverter.ToBoolean(obj);
				}
				else
				{
					flag = XmlUntypedConverter.Untyped.ToBoolean(obj);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", ex3, this);
			}
			return flag;
		}

		public override DateTime ReadElementContentAsDateTime()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsDateTime");
			}
			XmlSchemaType xmlSchemaType;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType);
			DateTime dateTime;
			try
			{
				if (xmlSchemaType != null)
				{
					dateTime = xmlSchemaType.ValueConverter.ToDateTime(obj);
				}
				else
				{
					dateTime = XmlUntypedConverter.Untyped.ToDateTime(obj);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", ex3, this);
			}
			return dateTime;
		}

		public override double ReadElementContentAsDouble()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsDouble");
			}
			XmlSchemaType xmlSchemaType;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType);
			double num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToDouble(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToDouble(obj);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", ex3, this);
			}
			return num;
		}

		public override float ReadElementContentAsFloat()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsFloat");
			}
			XmlSchemaType xmlSchemaType;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType);
			float num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToSingle(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToSingle(obj);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", ex3, this);
			}
			return num;
		}

		public override decimal ReadElementContentAsDecimal()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsDecimal");
			}
			XmlSchemaType xmlSchemaType;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType);
			decimal num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToDecimal(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToDecimal(obj);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", ex3, this);
			}
			return num;
		}

		public override int ReadElementContentAsInt()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsInt");
			}
			XmlSchemaType xmlSchemaType;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType);
			int num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToInt32(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToInt32(obj);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", ex3, this);
			}
			return num;
		}

		public override long ReadElementContentAsLong()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsLong");
			}
			XmlSchemaType xmlSchemaType;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType);
			long num;
			try
			{
				if (xmlSchemaType != null)
				{
					num = xmlSchemaType.ValueConverter.ToInt64(obj);
				}
				else
				{
					num = XmlUntypedConverter.Untyped.ToInt64(obj);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", ex3, this);
			}
			return num;
		}

		public override string ReadElementContentAsString()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsString");
			}
			XmlSchemaType xmlSchemaType;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType);
			string text;
			try
			{
				if (xmlSchemaType != null)
				{
					text = xmlSchemaType.ValueConverter.ToString(obj);
				}
				else
				{
					text = obj as string;
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex3, this);
			}
			return text;
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAs");
			}
			XmlSchemaType xmlSchemaType;
			string text;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType, false, out text);
			object obj2;
			try
			{
				if (xmlSchemaType != null)
				{
					if (returnType == typeof(DateTimeOffset) && xmlSchemaType.Datatype is Datatype_dateTimeBase)
					{
						obj = text;
					}
					obj2 = xmlSchemaType.ValueConverter.ChangeType(obj, returnType, namespaceResolver);
				}
				else
				{
					obj2 = XmlUntypedConverter.Untyped.ChangeType(obj, returnType, namespaceResolver);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex3, this);
			}
			return obj2;
		}

		public override int AttributeCount
		{
			get
			{
				return this.attributeCount;
			}
		}

		public override string GetAttribute(string name)
		{
			string text = this.coreReader.GetAttribute(name);
			if (text == null && this.attributeCount > 0)
			{
				ValidatingReaderNodeData defaultAttribute = this.GetDefaultAttribute(name, false);
				if (defaultAttribute != null)
				{
					text = defaultAttribute.RawValue;
				}
			}
			return text;
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			string attribute = this.coreReader.GetAttribute(name, namespaceURI);
			if (attribute == null && this.attributeCount > 0)
			{
				namespaceURI = ((namespaceURI == null) ? string.Empty : this.coreReaderNameTable.Get(namespaceURI));
				name = this.coreReaderNameTable.Get(name);
				if (name == null || namespaceURI == null)
				{
					return null;
				}
				ValidatingReaderNodeData defaultAttribute = this.GetDefaultAttribute(name, namespaceURI, false);
				if (defaultAttribute != null)
				{
					return defaultAttribute.RawValue;
				}
			}
			return attribute;
		}

		public override string GetAttribute(int i)
		{
			if (this.attributeCount == 0)
			{
				return null;
			}
			if (i < this.coreReaderAttributeCount)
			{
				return this.coreReader.GetAttribute(i);
			}
			int num = i - this.coreReaderAttributeCount;
			return ((ValidatingReaderNodeData)this.defaultAttributes[num]).RawValue;
		}

		public override bool MoveToAttribute(string name)
		{
			if (!this.coreReader.MoveToAttribute(name))
			{
				if (this.attributeCount > 0)
				{
					ValidatingReaderNodeData defaultAttribute = this.GetDefaultAttribute(name, true);
					if (defaultAttribute != null)
					{
						this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
						this.attributePSVI = defaultAttribute.AttInfo;
						this.cachedNode = defaultAttribute;
						goto IL_0057;
					}
				}
				return false;
			}
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
			this.attributePSVI = this.GetAttributePSVI(name);
			IL_0057:
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			return true;
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			name = this.coreReaderNameTable.Get(name);
			ns = ((ns != null) ? this.coreReaderNameTable.Get(ns) : string.Empty);
			if (name == null || ns == null)
			{
				return false;
			}
			if (this.coreReader.MoveToAttribute(name, ns))
			{
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
				if (this.inlineSchemaParser == null)
				{
					this.attributePSVI = this.GetAttributePSVI(name, ns);
				}
				else
				{
					this.attributePSVI = null;
				}
			}
			else
			{
				ValidatingReaderNodeData defaultAttribute = this.GetDefaultAttribute(name, ns, true);
				if (defaultAttribute == null)
				{
					return false;
				}
				this.attributePSVI = defaultAttribute.AttInfo;
				this.cachedNode = defaultAttribute;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			return true;
		}

		public override void MoveToAttribute(int i)
		{
			if (i < 0 || i >= this.attributeCount)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			this.currentAttrIndex = i;
			if (i < this.coreReaderAttributeCount)
			{
				this.coreReader.MoveToAttribute(i);
				if (this.inlineSchemaParser == null)
				{
					this.attributePSVI = this.attributePSVINodes[i];
				}
				else
				{
					this.attributePSVI = null;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
			}
			else
			{
				int num = i - this.coreReaderAttributeCount;
				this.cachedNode = (ValidatingReaderNodeData)this.defaultAttributes[num];
				this.attributePSVI = this.cachedNode.AttInfo;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.coreReader.MoveToFirstAttribute())
			{
				this.currentAttrIndex = 0;
				if (this.inlineSchemaParser == null)
				{
					this.attributePSVI = this.attributePSVINodes[0];
				}
				else
				{
					this.attributePSVI = null;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
			}
			else
			{
				if (this.defaultAttributes.Count <= 0)
				{
					return false;
				}
				this.cachedNode = (ValidatingReaderNodeData)this.defaultAttributes[0];
				this.attributePSVI = this.cachedNode.AttInfo;
				this.currentAttrIndex = 0;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (this.currentAttrIndex + 1 < this.coreReaderAttributeCount)
			{
				this.coreReader.MoveToNextAttribute();
				this.currentAttrIndex++;
				if (this.inlineSchemaParser == null)
				{
					this.attributePSVI = this.attributePSVINodes[this.currentAttrIndex];
				}
				else
				{
					this.attributePSVI = null;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
			}
			else
			{
				if (this.currentAttrIndex + 1 >= this.attributeCount)
				{
					return false;
				}
				int num = this.currentAttrIndex + 1;
				this.currentAttrIndex = num;
				int num2 = num - this.coreReaderAttributeCount;
				this.cachedNode = (ValidatingReaderNodeData)this.defaultAttributes[num2];
				this.attributePSVI = this.cachedNode.AttInfo;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			return true;
		}

		public override bool MoveToElement()
		{
			if (this.coreReader.MoveToElement() || this.validationState < XsdValidatingReader.ValidatingReaderState.None)
			{
				this.currentAttrIndex = -1;
				this.validationState = XsdValidatingReader.ValidatingReaderState.ClearAttributes;
				return true;
			}
			return false;
		}

		public override bool Read()
		{
			switch (this.validationState)
			{
			case XsdValidatingReader.ValidatingReaderState.OnReadAttributeValue:
			case XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute:
			case XsdValidatingReader.ValidatingReaderState.OnAttribute:
			case XsdValidatingReader.ValidatingReaderState.ClearAttributes:
				this.ClearAttributesInfo();
				if (this.inlineSchemaParser != null)
				{
					this.validationState = XsdValidatingReader.ValidatingReaderState.ParseInlineSchema;
					goto IL_007C;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				break;
			case XsdValidatingReader.ValidatingReaderState.None:
				return false;
			case XsdValidatingReader.ValidatingReaderState.Init:
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				if (this.coreReader.ReadState == ReadState.Interactive)
				{
					this.ProcessReaderEvent();
					return true;
				}
				break;
			case XsdValidatingReader.ValidatingReaderState.Read:
				break;
			case XsdValidatingReader.ValidatingReaderState.ParseInlineSchema:
				goto IL_007C;
			case XsdValidatingReader.ValidatingReaderState.ReadAhead:
				this.ClearAttributesInfo();
				this.ProcessReaderEvent();
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				return true;
			case XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent:
				this.validationState = this.savedState;
				this.readBinaryHelper.Finish();
				return this.Read();
			case XsdValidatingReader.ValidatingReaderState.ReaderClosed:
			case XsdValidatingReader.ValidatingReaderState.EOF:
				return false;
			default:
				return false;
			}
			if (this.coreReader.Read())
			{
				this.ProcessReaderEvent();
				return true;
			}
			this.validator.EndValidation();
			if (this.coreReader.EOF)
			{
				this.validationState = XsdValidatingReader.ValidatingReaderState.EOF;
			}
			return false;
			IL_007C:
			this.ProcessInlineSchema();
			return true;
		}

		public override bool EOF
		{
			get
			{
				return this.coreReader.EOF;
			}
		}

		public override void Close()
		{
			this.coreReader.Close();
			this.validationState = XsdValidatingReader.ValidatingReaderState.ReaderClosed;
		}

		public override ReadState ReadState
		{
			get
			{
				if (this.validationState != XsdValidatingReader.ValidatingReaderState.Init)
				{
					return this.coreReader.ReadState;
				}
				return ReadState.Initial;
			}
		}

		public override void Skip()
		{
			int depth = this.Depth;
			XmlNodeType nodeType = this.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					goto IL_0081;
				}
				this.MoveToElement();
			}
			if (!this.coreReader.IsEmptyElement)
			{
				bool flag = true;
				if ((this.xmlSchemaInfo.IsUnionType || this.xmlSchemaInfo.IsDefault) && this.coreReader is XsdCachingReader)
				{
					flag = false;
				}
				this.coreReader.Skip();
				this.validationState = XsdValidatingReader.ValidatingReaderState.ReadAhead;
				if (flag)
				{
					this.validator.SkipToEndElement(this.xmlSchemaInfo);
				}
			}
			IL_0081:
			this.Read();
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.coreReaderNameTable;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			return this.thisNSResolver.LookupNamespace(prefix);
		}

		public override void ResolveEntity()
		{
			throw new InvalidOperationException();
		}

		public override bool ReadAttributeValue()
		{
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			if (this.NodeType != XmlNodeType.Attribute)
			{
				return false;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute)
			{
				this.cachedNode = this.CreateDummyTextNode(this.cachedNode.RawValue, this.cachedNode.Depth + 1);
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadAttributeValue;
				return true;
			}
			return this.coreReader.ReadAttributeValue();
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
			if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.validationState;
			}
			this.validationState = this.savedState;
			int num = this.readBinaryHelper.ReadContentAsBase64(buffer, index, count);
			this.savedState = this.validationState;
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
			return num;
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.validationState;
			}
			this.validationState = this.savedState;
			int num = this.readBinaryHelper.ReadContentAsBinHex(buffer, index, count);
			this.savedState = this.validationState;
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
			return num;
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.validationState;
			}
			this.validationState = this.savedState;
			int num = this.readBinaryHelper.ReadElementContentAsBase64(buffer, index, count);
			this.savedState = this.validationState;
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
			return num;
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.validationState;
			}
			this.validationState = this.savedState;
			int num = this.readBinaryHelper.ReadElementContentAsBinHex(buffer, index, count);
			this.savedState = this.validationState;
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
			return num;
		}

		bool IXmlSchemaInfo.IsDefault
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType == XmlNodeType.EndElement)
						{
							return this.xmlSchemaInfo.IsDefault;
						}
					}
					else if (this.attributePSVI != null)
					{
						return this.AttributeSchemaInfo.IsDefault;
					}
					return false;
				}
				if (!this.coreReader.IsEmptyElement)
				{
					this.GetIsDefault();
				}
				return this.xmlSchemaInfo.IsDefault;
			}
		}

		bool IXmlSchemaInfo.IsNil
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				return (nodeType == XmlNodeType.Element || nodeType == XmlNodeType.EndElement) && this.xmlSchemaInfo.IsNil;
			}
		}

		XmlSchemaValidity IXmlSchemaInfo.Validity
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType == XmlNodeType.EndElement)
						{
							return this.xmlSchemaInfo.Validity;
						}
					}
					else if (this.attributePSVI != null)
					{
						return this.AttributeSchemaInfo.Validity;
					}
					return XmlSchemaValidity.NotKnown;
				}
				if (this.coreReader.IsEmptyElement)
				{
					return this.xmlSchemaInfo.Validity;
				}
				if (this.xmlSchemaInfo.Validity == XmlSchemaValidity.Valid)
				{
					return XmlSchemaValidity.NotKnown;
				}
				return this.xmlSchemaInfo.Validity;
			}
		}

		XmlSchemaSimpleType IXmlSchemaInfo.MemberType
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					if (!this.coreReader.IsEmptyElement)
					{
						this.GetMemberType();
					}
					return this.xmlSchemaInfo.MemberType;
				}
				if (nodeType != XmlNodeType.Attribute)
				{
					if (nodeType != XmlNodeType.EndElement)
					{
						return null;
					}
					return this.xmlSchemaInfo.MemberType;
				}
				else
				{
					if (this.attributePSVI != null)
					{
						return this.AttributeSchemaInfo.MemberType;
					}
					return null;
				}
			}
		}

		XmlSchemaType IXmlSchemaInfo.SchemaType
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType != XmlNodeType.EndElement)
						{
							return null;
						}
					}
					else
					{
						if (this.attributePSVI != null)
						{
							return this.AttributeSchemaInfo.SchemaType;
						}
						return null;
					}
				}
				return this.xmlSchemaInfo.SchemaType;
			}
		}

		XmlSchemaElement IXmlSchemaInfo.SchemaElement
		{
			get
			{
				if (this.NodeType == XmlNodeType.Element || this.NodeType == XmlNodeType.EndElement)
				{
					return this.xmlSchemaInfo.SchemaElement;
				}
				return null;
			}
		}

		XmlSchemaAttribute IXmlSchemaInfo.SchemaAttribute
		{
			get
			{
				if (this.NodeType == XmlNodeType.Attribute && this.attributePSVI != null)
				{
					return this.AttributeSchemaInfo.SchemaAttribute;
				}
				return null;
			}
		}

		public bool HasLineInfo()
		{
			return true;
		}

		public int LineNumber
		{
			get
			{
				if (this.lineInfo != null)
				{
					return this.lineInfo.LineNumber;
				}
				return 0;
			}
		}

		public int LinePosition
		{
			get
			{
				if (this.lineInfo != null)
				{
					return this.lineInfo.LinePosition;
				}
				return 0;
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			if (this.coreReaderNSResolver != null)
			{
				return this.coreReaderNSResolver.GetNamespacesInScope(scope);
			}
			return this.nsManager.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			if (this.coreReaderNSResolver != null)
			{
				return this.coreReaderNSResolver.LookupNamespace(prefix);
			}
			return this.nsManager.LookupNamespace(prefix);
		}

		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			if (this.coreReaderNSResolver != null)
			{
				return this.coreReaderNSResolver.LookupPrefix(namespaceName);
			}
			return this.nsManager.LookupPrefix(namespaceName);
		}

		private object GetStringValue()
		{
			return this.coreReader.Value;
		}

		private XmlSchemaType ElementXmlType
		{
			get
			{
				return this.xmlSchemaInfo.XmlType;
			}
		}

		private XmlSchemaType AttributeXmlType
		{
			get
			{
				if (this.attributePSVI != null)
				{
					return this.AttributeSchemaInfo.XmlType;
				}
				return null;
			}
		}

		private XmlSchemaInfo AttributeSchemaInfo
		{
			get
			{
				return this.attributePSVI.attributeSchemaInfo;
			}
		}

		private void ProcessReaderEvent()
		{
			if (this.replayCache)
			{
				return;
			}
			switch (this.coreReader.NodeType)
			{
			case XmlNodeType.Element:
				this.ProcessElementEvent();
				return;
			case XmlNodeType.Attribute:
			case XmlNodeType.Entity:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Notation:
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
				return;
			case XmlNodeType.EntityReference:
				throw new InvalidOperationException();
			case XmlNodeType.DocumentType:
				this.validator.SetDtdSchemaInfo(this.coreReader.DtdInfo);
				break;
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
				return;
			case XmlNodeType.EndElement:
				this.ProcessEndElementEvent();
				return;
			default:
				return;
			}
		}

		private void ProcessElementEvent()
		{
			if (!this.processInlineSchema || !this.IsXSDRoot(this.coreReader.LocalName, this.coreReader.NamespaceURI) || this.coreReader.Depth <= 0)
			{
				this.atomicValue = null;
				this.originalAtomicValueString = null;
				this.xmlSchemaInfo.Clear();
				if (this.manageNamespaces)
				{
					this.nsManager.PushScope();
				}
				string text = null;
				string text2 = null;
				string text3 = null;
				string text4 = null;
				if (this.coreReader.MoveToFirstAttribute())
				{
					do
					{
						string namespaceURI = this.coreReader.NamespaceURI;
						string localName = this.coreReader.LocalName;
						if (Ref.Equal(namespaceURI, this.NsXsi))
						{
							if (Ref.Equal(localName, this.XsiSchemaLocation))
							{
								text = this.coreReader.Value;
							}
							else if (Ref.Equal(localName, this.XsiNoNamespaceSchemaLocation))
							{
								text2 = this.coreReader.Value;
							}
							else if (Ref.Equal(localName, this.XsiType))
							{
								text4 = this.coreReader.Value;
							}
							else if (Ref.Equal(localName, this.XsiNil))
							{
								text3 = this.coreReader.Value;
							}
						}
						if (this.manageNamespaces && Ref.Equal(this.coreReader.NamespaceURI, this.NsXmlNs))
						{
							this.nsManager.AddNamespace((this.coreReader.Prefix.Length == 0) ? string.Empty : this.coreReader.LocalName, this.coreReader.Value);
						}
					}
					while (this.coreReader.MoveToNextAttribute());
					this.coreReader.MoveToElement();
				}
				this.validator.ValidateElement(this.coreReader.LocalName, this.coreReader.NamespaceURI, this.xmlSchemaInfo, text4, text3, text, text2);
				this.ValidateAttributes();
				this.validator.ValidateEndOfAttributes(this.xmlSchemaInfo);
				if (this.coreReader.IsEmptyElement)
				{
					this.ProcessEndElementEvent();
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.ClearAttributes;
				return;
			}
			this.xmlSchemaInfo.Clear();
			this.attributeCount = (this.coreReaderAttributeCount = this.coreReader.AttributeCount);
			if (!this.coreReader.IsEmptyElement)
			{
				this.inlineSchemaParser = new Parser(SchemaType.XSD, this.coreReaderNameTable, this.validator.SchemaSet.GetSchemaNames(this.coreReaderNameTable), this.validationEvent);
				this.inlineSchemaParser.StartParsing(this.coreReader, null);
				this.inlineSchemaParser.ParseReaderNode();
				this.validationState = XsdValidatingReader.ValidatingReaderState.ParseInlineSchema;
				return;
			}
			this.validationState = XsdValidatingReader.ValidatingReaderState.ClearAttributes;
		}

		private void ProcessEndElementEvent()
		{
			this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
			this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
			if (this.xmlSchemaInfo.IsDefault)
			{
				int depth = this.coreReader.Depth;
				this.coreReader = this.GetCachingReader();
				this.cachingReader.RecordTextNode(this.xmlSchemaInfo.XmlType.ValueConverter.ToString(this.atomicValue), this.originalAtomicValueString, depth + 1, 0, 0);
				this.cachingReader.RecordEndElementNode();
				this.cachingReader.SetToReplayMode();
				this.replayCache = true;
				return;
			}
			if (this.manageNamespaces)
			{
				this.nsManager.PopScope();
			}
		}

		private void ValidateAttributes()
		{
			this.attributeCount = (this.coreReaderAttributeCount = this.coreReader.AttributeCount);
			int num = 0;
			bool flag = false;
			if (this.coreReader.MoveToFirstAttribute())
			{
				do
				{
					string localName = this.coreReader.LocalName;
					string namespaceURI = this.coreReader.NamespaceURI;
					AttributePSVIInfo attributePSVIInfo = this.AddAttributePSVI(num);
					attributePSVIInfo.localName = localName;
					attributePSVIInfo.namespaceUri = namespaceURI;
					if (namespaceURI == this.NsXmlNs)
					{
						num++;
					}
					else
					{
						attributePSVIInfo.typedAttributeValue = this.validator.ValidateAttribute(localName, namespaceURI, this.valueGetter, attributePSVIInfo.attributeSchemaInfo);
						if (!flag)
						{
							flag = attributePSVIInfo.attributeSchemaInfo.Validity == XmlSchemaValidity.Invalid;
						}
						num++;
					}
				}
				while (this.coreReader.MoveToNextAttribute());
			}
			this.coreReader.MoveToElement();
			if (flag)
			{
				this.xmlSchemaInfo.Validity = XmlSchemaValidity.Invalid;
			}
			this.validator.GetUnspecifiedDefaultAttributes(this.defaultAttributes, true);
			this.attributeCount += this.defaultAttributes.Count;
		}

		private void ClearAttributesInfo()
		{
			this.attributeCount = 0;
			this.coreReaderAttributeCount = 0;
			this.currentAttrIndex = -1;
			this.defaultAttributes.Clear();
			this.attributePSVI = null;
		}

		private AttributePSVIInfo GetAttributePSVI(string name)
		{
			if (this.inlineSchemaParser != null)
			{
				return null;
			}
			string text;
			string text2;
			ValidateNames.SplitQName(name, out text, out text2);
			text = this.coreReaderNameTable.Add(text);
			text2 = this.coreReaderNameTable.Add(text2);
			string text3;
			if (text.Length == 0)
			{
				text3 = string.Empty;
			}
			else
			{
				text3 = this.thisNSResolver.LookupNamespace(text);
			}
			return this.GetAttributePSVI(text2, text3);
		}

		private AttributePSVIInfo GetAttributePSVI(string localName, string ns)
		{
			for (int i = 0; i < this.coreReaderAttributeCount; i++)
			{
				AttributePSVIInfo attributePSVIInfo = this.attributePSVINodes[i];
				if (attributePSVIInfo != null && Ref.Equal(localName, attributePSVIInfo.localName) && Ref.Equal(ns, attributePSVIInfo.namespaceUri))
				{
					this.currentAttrIndex = i;
					return attributePSVIInfo;
				}
			}
			return null;
		}

		private ValidatingReaderNodeData GetDefaultAttribute(string name, bool updatePosition)
		{
			string text;
			string text2;
			ValidateNames.SplitQName(name, out text, out text2);
			text = this.coreReaderNameTable.Add(text);
			text2 = this.coreReaderNameTable.Add(text2);
			string text3;
			if (text.Length == 0)
			{
				text3 = string.Empty;
			}
			else
			{
				text3 = this.thisNSResolver.LookupNamespace(text);
			}
			return this.GetDefaultAttribute(text2, text3, updatePosition);
		}

		private ValidatingReaderNodeData GetDefaultAttribute(string attrLocalName, string ns, bool updatePosition)
		{
			for (int i = 0; i < this.defaultAttributes.Count; i++)
			{
				ValidatingReaderNodeData validatingReaderNodeData = (ValidatingReaderNodeData)this.defaultAttributes[i];
				if (Ref.Equal(validatingReaderNodeData.LocalName, attrLocalName) && Ref.Equal(validatingReaderNodeData.Namespace, ns))
				{
					if (updatePosition)
					{
						this.currentAttrIndex = this.coreReader.AttributeCount + i;
					}
					return validatingReaderNodeData;
				}
			}
			return null;
		}

		private AttributePSVIInfo AddAttributePSVI(int attIndex)
		{
			AttributePSVIInfo attributePSVIInfo = this.attributePSVINodes[attIndex];
			if (attributePSVIInfo != null)
			{
				attributePSVIInfo.Reset();
				return attributePSVIInfo;
			}
			if (attIndex >= this.attributePSVINodes.Length - 1)
			{
				AttributePSVIInfo[] array = new AttributePSVIInfo[this.attributePSVINodes.Length * 2];
				Array.Copy(this.attributePSVINodes, 0, array, 0, this.attributePSVINodes.Length);
				this.attributePSVINodes = array;
			}
			attributePSVIInfo = this.attributePSVINodes[attIndex];
			if (attributePSVIInfo == null)
			{
				attributePSVIInfo = new AttributePSVIInfo();
				this.attributePSVINodes[attIndex] = attributePSVIInfo;
			}
			return attributePSVIInfo;
		}

		private bool IsXSDRoot(string localName, string ns)
		{
			return Ref.Equal(ns, this.NsXs) && Ref.Equal(localName, this.XsdSchema);
		}

		private void ProcessInlineSchema()
		{
			if (this.coreReader.Read())
			{
				if (this.coreReader.NodeType == XmlNodeType.Element)
				{
					this.attributeCount = (this.coreReaderAttributeCount = this.coreReader.AttributeCount);
				}
				else
				{
					this.ClearAttributesInfo();
				}
				if (!this.inlineSchemaParser.ParseReaderNode())
				{
					this.inlineSchemaParser.FinishParsing();
					XmlSchema xmlSchema = this.inlineSchemaParser.XmlSchema;
					this.validator.AddSchema(xmlSchema);
					this.inlineSchemaParser = null;
					this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				}
			}
		}

		private object InternalReadContentAsObject()
		{
			return this.InternalReadContentAsObject(false);
		}

		private object InternalReadContentAsObject(bool unwrapTypedValue)
		{
			string text;
			return this.InternalReadContentAsObject(unwrapTypedValue, out text);
		}

		private object InternalReadContentAsObject(bool unwrapTypedValue, out string originalStringValue)
		{
			XmlNodeType nodeType = this.NodeType;
			if (nodeType == XmlNodeType.Attribute)
			{
				originalStringValue = this.Value;
				if (this.attributePSVI != null && this.attributePSVI.typedAttributeValue != null)
				{
					if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute)
					{
						XmlSchemaAttribute schemaAttribute = this.attributePSVI.attributeSchemaInfo.SchemaAttribute;
						originalStringValue = ((schemaAttribute.DefaultValue != null) ? schemaAttribute.DefaultValue : schemaAttribute.FixedValue);
					}
					return this.ReturnBoxedValue(this.attributePSVI.typedAttributeValue, this.AttributeSchemaInfo.XmlType, unwrapTypedValue);
				}
				return this.Value;
			}
			else if (nodeType == XmlNodeType.EndElement)
			{
				if (this.atomicValue != null)
				{
					originalStringValue = this.originalAtomicValueString;
					return this.atomicValue;
				}
				originalStringValue = string.Empty;
				return string.Empty;
			}
			else
			{
				if (this.validator.CurrentContentType == XmlSchemaContentType.TextOnly)
				{
					object obj = this.ReturnBoxedValue(this.ReadTillEndElement(), this.xmlSchemaInfo.XmlType, unwrapTypedValue);
					originalStringValue = this.originalAtomicValueString;
					return obj;
				}
				XsdCachingReader xsdCachingReader = this.coreReader as XsdCachingReader;
				if (xsdCachingReader != null)
				{
					originalStringValue = xsdCachingReader.ReadOriginalContentAsString();
				}
				else
				{
					originalStringValue = base.InternalReadContentAsString();
				}
				return originalStringValue;
			}
		}

		private object InternalReadElementContentAsObject(out XmlSchemaType xmlType)
		{
			return this.InternalReadElementContentAsObject(out xmlType, false);
		}

		private object InternalReadElementContentAsObject(out XmlSchemaType xmlType, bool unwrapTypedValue)
		{
			string text;
			return this.InternalReadElementContentAsObject(out xmlType, unwrapTypedValue, out text);
		}

		private object InternalReadElementContentAsObject(out XmlSchemaType xmlType, bool unwrapTypedValue, out string originalString)
		{
			xmlType = null;
			object obj;
			if (this.IsEmptyElement)
			{
				if (this.xmlSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
				{
					obj = this.ReturnBoxedValue(this.atomicValue, this.xmlSchemaInfo.XmlType, unwrapTypedValue);
				}
				else
				{
					obj = this.atomicValue;
				}
				originalString = this.originalAtomicValueString;
				xmlType = this.ElementXmlType;
				this.Read();
				return obj;
			}
			this.Read();
			if (this.NodeType == XmlNodeType.EndElement)
			{
				if (this.xmlSchemaInfo.IsDefault)
				{
					if (this.xmlSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
					{
						obj = this.ReturnBoxedValue(this.atomicValue, this.xmlSchemaInfo.XmlType, unwrapTypedValue);
					}
					else
					{
						obj = this.atomicValue;
					}
					originalString = this.originalAtomicValueString;
				}
				else
				{
					obj = string.Empty;
					originalString = string.Empty;
				}
			}
			else
			{
				if (this.NodeType == XmlNodeType.Element)
				{
					throw new XmlException("ReadElementContentAs() methods cannot be called on an element that has child elements.", string.Empty, this);
				}
				obj = this.InternalReadContentAsObject(unwrapTypedValue, out originalString);
				if (this.NodeType != XmlNodeType.EndElement)
				{
					throw new XmlException("ReadElementContentAs() methods cannot be called on an element that has child elements.", string.Empty, this);
				}
			}
			xmlType = this.ElementXmlType;
			this.Read();
			return obj;
		}

		private object ReadTillEndElement()
		{
			if (this.atomicValue == null)
			{
				while (this.coreReader.Read())
				{
					if (!this.replayCache)
					{
						switch (this.coreReader.NodeType)
						{
						case XmlNodeType.Element:
							this.ProcessReaderEvent();
							goto IL_010B;
						case XmlNodeType.Text:
						case XmlNodeType.CDATA:
							this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
							break;
						case XmlNodeType.Whitespace:
						case XmlNodeType.SignificantWhitespace:
							this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
							break;
						case XmlNodeType.EndElement:
							this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
							this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
							if (this.manageNamespaces)
							{
								this.nsManager.PopScope();
								goto IL_010B;
							}
							goto IL_010B;
						}
					}
				}
			}
			else
			{
				if (this.atomicValue == this)
				{
					this.atomicValue = null;
				}
				this.SwitchReader();
			}
			IL_010B:
			return this.atomicValue;
		}

		private void SwitchReader()
		{
			XsdCachingReader xsdCachingReader = this.coreReader as XsdCachingReader;
			if (xsdCachingReader != null)
			{
				this.coreReader = xsdCachingReader.GetCoreReader();
			}
			this.replayCache = false;
		}

		private void ReadAheadForMemberType()
		{
			while (this.coreReader.Read())
			{
				switch (this.coreReader.NodeType)
				{
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
					break;
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
					break;
				case XmlNodeType.EndElement:
					this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
					this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
					if (this.atomicValue == null)
					{
						this.atomicValue = this;
						return;
					}
					if (this.xmlSchemaInfo.IsDefault)
					{
						this.cachingReader.SwitchTextNodeAndEndElement(this.xmlSchemaInfo.XmlType.ValueConverter.ToString(this.atomicValue), this.originalAtomicValueString);
						return;
					}
					return;
				}
			}
		}

		private void GetIsDefault()
		{
			if (!(this.coreReader is XsdCachingReader) && this.xmlSchemaInfo.HasDefaultValue)
			{
				this.coreReader = this.GetCachingReader();
				if (this.xmlSchemaInfo.IsUnionType && !this.xmlSchemaInfo.IsNil)
				{
					this.ReadAheadForMemberType();
				}
				else if (this.coreReader.Read())
				{
					switch (this.coreReader.NodeType)
					{
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
						break;
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
						break;
					case XmlNodeType.EndElement:
						this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
						this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
						if (this.xmlSchemaInfo.IsDefault)
						{
							this.cachingReader.SwitchTextNodeAndEndElement(this.xmlSchemaInfo.XmlType.ValueConverter.ToString(this.atomicValue), this.originalAtomicValueString);
						}
						break;
					}
				}
				this.cachingReader.SetToReplayMode();
				this.replayCache = true;
			}
		}

		private void GetMemberType()
		{
			if (this.xmlSchemaInfo.MemberType != null || this.atomicValue == this)
			{
				return;
			}
			if (!(this.coreReader is XsdCachingReader) && this.xmlSchemaInfo.IsUnionType && !this.xmlSchemaInfo.IsNil)
			{
				this.coreReader = this.GetCachingReader();
				this.ReadAheadForMemberType();
				this.cachingReader.SetToReplayMode();
				this.replayCache = true;
			}
		}

		private object ReturnBoxedValue(object typedValue, XmlSchemaType xmlType, bool unWrap)
		{
			if (typedValue != null)
			{
				if (unWrap && xmlType.Datatype.Variety == XmlSchemaDatatypeVariety.List && (xmlType.Datatype as Datatype_List).ItemType.Variety == XmlSchemaDatatypeVariety.Union)
				{
					typedValue = xmlType.ValueConverter.ChangeType(typedValue, xmlType.Datatype.ValueType, this.thisNSResolver);
				}
				return typedValue;
			}
			typedValue = this.validator.GetConcatenatedValue();
			return typedValue;
		}

		private XsdCachingReader GetCachingReader()
		{
			if (this.cachingReader == null)
			{
				this.cachingReader = new XsdCachingReader(this.coreReader, this.lineInfo, new CachingEventHandler(this.CachingCallBack));
			}
			else
			{
				this.cachingReader.Reset(this.coreReader);
			}
			this.lineInfo = this.cachingReader;
			return this.cachingReader;
		}

		internal ValidatingReaderNodeData CreateDummyTextNode(string attributeValue, int depth)
		{
			if (this.textNode == null)
			{
				this.textNode = new ValidatingReaderNodeData(XmlNodeType.Text);
			}
			this.textNode.Depth = depth;
			this.textNode.RawValue = attributeValue;
			return this.textNode;
		}

		internal void CachingCallBack(XsdCachingReader cachingReader)
		{
			this.coreReader = cachingReader.GetCoreReader();
			this.lineInfo = cachingReader.GetLineInfo();
			this.replayCache = false;
		}

		private string GetOriginalAtomicValueStringOfElement()
		{
			if (!this.xmlSchemaInfo.IsDefault)
			{
				return this.validator.GetConcatenatedValue();
			}
			XmlSchemaElement schemaElement = this.xmlSchemaInfo.SchemaElement;
			if (schemaElement == null)
			{
				return string.Empty;
			}
			if (schemaElement.DefaultValue == null)
			{
				return schemaElement.FixedValue;
			}
			return schemaElement.DefaultValue;
		}

		public override Task<string> GetValueAsync()
		{
			if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
			{
				return Task.FromResult<string>(this.cachedNode.RawValue);
			}
			return this.coreReader.GetValueAsync();
		}

		public override Task<object> ReadContentAsObjectAsync()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsObject");
			}
			return this.InternalReadContentAsObjectAsync(true);
		}

		public override async Task<string> ReadContentAsStringAsync()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsString");
			}
			object obj = await this.InternalReadContentAsObjectAsync().ConfigureAwait(false);
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			string text;
			try
			{
				if (xmlSchemaType != null)
				{
					text = xmlSchemaType.ValueConverter.ToString(obj);
				}
				else
				{
					text = obj as string;
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex3, this);
			}
			return text;
		}

		public override async Task<object> ReadContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAs");
			}
			object obj = await this.InternalReadContentAsObjectTupleAsync(false).ConfigureAwait(false);
			string item = obj.Item1;
			object obj2 = obj.Item2;
			XmlSchemaType xmlSchemaType = ((this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType);
			object obj3;
			try
			{
				if (xmlSchemaType != null)
				{
					if (returnType == typeof(DateTimeOffset) && xmlSchemaType.Datatype is Datatype_dateTimeBase)
					{
						obj2 = item;
					}
					obj3 = xmlSchemaType.ValueConverter.ChangeType(obj2, returnType);
				}
				else
				{
					obj3 = XmlUntypedConverter.Untyped.ChangeType(obj2, returnType, namespaceResolver);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex3, this);
			}
			return obj3;
		}

		public override async Task<object> ReadElementContentAsObjectAsync()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsObject");
			}
			return (await this.InternalReadElementContentAsObjectAsync(true).ConfigureAwait(false)).Item2;
		}

		public override async Task<string> ReadElementContentAsStringAsync()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsString");
			}
			object obj = await this.InternalReadElementContentAsObjectAsync().ConfigureAwait(false);
			XmlSchemaType item = obj.Item1;
			object item2 = obj.Item2;
			string text;
			try
			{
				if (item != null)
				{
					text = item.ValueConverter.ToString(item2);
				}
				else
				{
					text = item2 as string;
				}
			}
			catch (InvalidCastException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex, this);
			}
			catch (FormatException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", ex3, this);
			}
			return text;
		}

		public override async Task<object> ReadElementContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAs");
			}
			object obj = await this.InternalReadElementContentAsObjectTupleAsync(false).ConfigureAwait(false);
			XmlSchemaType item = obj.Item1;
			string item2 = obj.Item2;
			object obj2 = obj.Item3;
			object obj3;
			try
			{
				if (item != null)
				{
					if (returnType == typeof(DateTimeOffset) && item.Datatype is Datatype_dateTimeBase)
					{
						obj2 = item2;
					}
					obj3 = item.ValueConverter.ChangeType(obj2, returnType, namespaceResolver);
				}
				else
				{
					obj3 = XmlUntypedConverter.Untyped.ChangeType(obj2, returnType, namespaceResolver);
				}
			}
			catch (FormatException ex)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex, this);
			}
			catch (InvalidCastException ex2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex2, this);
			}
			catch (OverflowException ex3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), ex3, this);
			}
			return obj3;
		}

		private Task<bool> ReadAsync_Read(Task<bool> task)
		{
			if (!task.IsSuccess())
			{
				return this._ReadAsync_Read(task);
			}
			if (task.Result)
			{
				return this.ProcessReaderEventAsync().ReturnTaskBoolWhenFinish(true);
			}
			this.validator.EndValidation();
			if (this.coreReader.EOF)
			{
				this.validationState = XsdValidatingReader.ValidatingReaderState.EOF;
			}
			return AsyncHelper.DoneTaskFalse;
		}

		private async Task<bool> _ReadAsync_Read(Task<bool> task)
		{
			ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = task.ConfigureAwait(false).GetAwaiter();
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
				await this.ProcessReaderEventAsync().ConfigureAwait(false);
				flag = true;
			}
			else
			{
				this.validator.EndValidation();
				if (this.coreReader.EOF)
				{
					this.validationState = XsdValidatingReader.ValidatingReaderState.EOF;
				}
				flag = false;
			}
			return flag;
		}

		private Task<bool> ReadAsync_ReadAhead(Task task)
		{
			if (task.IsSuccess())
			{
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				return AsyncHelper.DoneTaskTrue;
			}
			return this._ReadAsync_ReadAhead(task);
		}

		private async Task<bool> _ReadAsync_ReadAhead(Task task)
		{
			await task.ConfigureAwait(false);
			this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
			return true;
		}

		public override Task<bool> ReadAsync()
		{
			switch (this.validationState)
			{
			case XsdValidatingReader.ValidatingReaderState.OnReadAttributeValue:
			case XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute:
			case XsdValidatingReader.ValidatingReaderState.OnAttribute:
			case XsdValidatingReader.ValidatingReaderState.ClearAttributes:
				this.ClearAttributesInfo();
				if (this.inlineSchemaParser != null)
				{
					this.validationState = XsdValidatingReader.ValidatingReaderState.ParseInlineSchema;
					goto IL_0059;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				break;
			case XsdValidatingReader.ValidatingReaderState.None:
				goto IL_00F0;
			case XsdValidatingReader.ValidatingReaderState.Init:
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				if (this.coreReader.ReadState == ReadState.Interactive)
				{
					return this.ProcessReaderEventAsync().ReturnTaskBoolWhenFinish(true);
				}
				break;
			case XsdValidatingReader.ValidatingReaderState.Read:
				break;
			case XsdValidatingReader.ValidatingReaderState.ParseInlineSchema:
				goto IL_0059;
			case XsdValidatingReader.ValidatingReaderState.ReadAhead:
			{
				this.ClearAttributesInfo();
				Task task = this.ProcessReaderEventAsync();
				return this.ReadAsync_ReadAhead(task);
			}
			case XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent:
				this.validationState = this.savedState;
				return this.readBinaryHelper.FinishAsync().CallBoolTaskFuncWhenFinish(new Func<Task<bool>>(this.ReadAsync));
			case XsdValidatingReader.ValidatingReaderState.ReaderClosed:
			case XsdValidatingReader.ValidatingReaderState.EOF:
				return AsyncHelper.DoneTaskFalse;
			default:
				goto IL_00F0;
			}
			Task<bool> task2 = this.coreReader.ReadAsync();
			return this.ReadAsync_Read(task2);
			IL_0059:
			return this.ProcessInlineSchemaAsync().ReturnTaskBoolWhenFinish(true);
			IL_00F0:
			return AsyncHelper.DoneTaskFalse;
		}

		public override async Task SkipAsync()
		{
			int depth = this.Depth;
			XmlNodeType nodeType = this.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					goto IL_0116;
				}
				this.MoveToElement();
			}
			if (!this.coreReader.IsEmptyElement)
			{
				bool callSkipToEndElem = true;
				if ((this.xmlSchemaInfo.IsUnionType || this.xmlSchemaInfo.IsDefault) && this.coreReader is XsdCachingReader)
				{
					callSkipToEndElem = false;
				}
				await this.coreReader.SkipAsync().ConfigureAwait(false);
				this.validationState = XsdValidatingReader.ValidatingReaderState.ReadAhead;
				if (callSkipToEndElem)
				{
					this.validator.SkipToEndElement(this.xmlSchemaInfo);
				}
			}
			IL_0116:
			await this.ReadAsync().ConfigureAwait(false);
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
				if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
				{
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
					this.savedState = this.validationState;
				}
				this.validationState = this.savedState;
				int num2 = await this.readBinaryHelper.ReadContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
				this.savedState = this.validationState;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
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
				if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
				{
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
					this.savedState = this.validationState;
				}
				this.validationState = this.savedState;
				int num2 = await this.readBinaryHelper.ReadContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
				this.savedState = this.validationState;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
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
				if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
				{
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
					this.savedState = this.validationState;
				}
				this.validationState = this.savedState;
				int num2 = await this.readBinaryHelper.ReadElementContentAsBase64Async(buffer, index, count).ConfigureAwait(false);
				this.savedState = this.validationState;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
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
				if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
				{
					this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
					this.savedState = this.validationState;
				}
				this.validationState = this.savedState;
				int num2 = await this.readBinaryHelper.ReadElementContentAsBinHexAsync(buffer, index, count).ConfigureAwait(false);
				this.savedState = this.validationState;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
				num = num2;
			}
			return num;
		}

		private Task ProcessReaderEventAsync()
		{
			if (this.replayCache)
			{
				return AsyncHelper.DoneTask;
			}
			switch (this.coreReader.NodeType)
			{
			case XmlNodeType.Element:
				return this.ProcessElementEventAsync();
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
				break;
			case XmlNodeType.EntityReference:
				throw new InvalidOperationException();
			case XmlNodeType.DocumentType:
				this.validator.SetDtdSchemaInfo(this.coreReader.DtdInfo);
				break;
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
				break;
			case XmlNodeType.EndElement:
				return this.ProcessEndElementEventAsync();
			}
			return AsyncHelper.DoneTask;
		}

		private async Task ProcessElementEventAsync()
		{
			if (this.processInlineSchema && this.IsXSDRoot(this.coreReader.LocalName, this.coreReader.NamespaceURI) && this.coreReader.Depth > 0)
			{
				this.xmlSchemaInfo.Clear();
				this.attributeCount = (this.coreReaderAttributeCount = this.coreReader.AttributeCount);
				if (!this.coreReader.IsEmptyElement)
				{
					this.inlineSchemaParser = new Parser(SchemaType.XSD, this.coreReaderNameTable, this.validator.SchemaSet.GetSchemaNames(this.coreReaderNameTable), this.validationEvent);
					await this.inlineSchemaParser.StartParsingAsync(this.coreReader, null).ConfigureAwait(false);
					this.inlineSchemaParser.ParseReaderNode();
					this.validationState = XsdValidatingReader.ValidatingReaderState.ParseInlineSchema;
				}
				else
				{
					this.validationState = XsdValidatingReader.ValidatingReaderState.ClearAttributes;
				}
			}
			else
			{
				this.atomicValue = null;
				this.originalAtomicValueString = null;
				this.xmlSchemaInfo.Clear();
				if (this.manageNamespaces)
				{
					this.nsManager.PushScope();
				}
				string text = null;
				string text2 = null;
				string text3 = null;
				string text4 = null;
				if (this.coreReader.MoveToFirstAttribute())
				{
					do
					{
						string namespaceURI = this.coreReader.NamespaceURI;
						string localName = this.coreReader.LocalName;
						if (Ref.Equal(namespaceURI, this.NsXsi))
						{
							if (Ref.Equal(localName, this.XsiSchemaLocation))
							{
								text = this.coreReader.Value;
							}
							else if (Ref.Equal(localName, this.XsiNoNamespaceSchemaLocation))
							{
								text2 = this.coreReader.Value;
							}
							else if (Ref.Equal(localName, this.XsiType))
							{
								text4 = this.coreReader.Value;
							}
							else if (Ref.Equal(localName, this.XsiNil))
							{
								text3 = this.coreReader.Value;
							}
						}
						if (this.manageNamespaces && Ref.Equal(this.coreReader.NamespaceURI, this.NsXmlNs))
						{
							this.nsManager.AddNamespace((this.coreReader.Prefix.Length == 0) ? string.Empty : this.coreReader.LocalName, this.coreReader.Value);
						}
					}
					while (this.coreReader.MoveToNextAttribute());
					this.coreReader.MoveToElement();
				}
				this.validator.ValidateElement(this.coreReader.LocalName, this.coreReader.NamespaceURI, this.xmlSchemaInfo, text4, text3, text, text2);
				this.ValidateAttributes();
				this.validator.ValidateEndOfAttributes(this.xmlSchemaInfo);
				if (this.coreReader.IsEmptyElement)
				{
					await this.ProcessEndElementEventAsync().ConfigureAwait(false);
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.ClearAttributes;
			}
		}

		private async Task ProcessEndElementEventAsync()
		{
			this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
			this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
			if (this.xmlSchemaInfo.IsDefault)
			{
				int depth = this.coreReader.Depth;
				this.coreReader = this.GetCachingReader();
				this.cachingReader.RecordTextNode(this.xmlSchemaInfo.XmlType.ValueConverter.ToString(this.atomicValue), this.originalAtomicValueString, depth + 1, 0, 0);
				this.cachingReader.RecordEndElementNode();
				await this.cachingReader.SetToReplayModeAsync().ConfigureAwait(false);
				this.replayCache = true;
			}
			else if (this.manageNamespaces)
			{
				this.nsManager.PopScope();
			}
		}

		private async Task ProcessInlineSchemaAsync()
		{
			ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.coreReader.ReadAsync().ConfigureAwait(false).GetAwaiter();
			if (!configuredTaskAwaiter.IsCompleted)
			{
				await configuredTaskAwaiter;
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
				configuredTaskAwaiter = configuredTaskAwaiter2;
				configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
			}
			if (configuredTaskAwaiter.GetResult())
			{
				if (this.coreReader.NodeType == XmlNodeType.Element)
				{
					int num = this.coreReader.AttributeCount;
					this.coreReaderAttributeCount = num;
					this.attributeCount = num;
				}
				else
				{
					this.ClearAttributesInfo();
				}
				if (!this.inlineSchemaParser.ParseReaderNode())
				{
					this.inlineSchemaParser.FinishParsing();
					XmlSchema xmlSchema = this.inlineSchemaParser.XmlSchema;
					this.validator.AddSchema(xmlSchema);
					this.inlineSchemaParser = null;
					this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				}
			}
		}

		private Task<object> InternalReadContentAsObjectAsync()
		{
			return this.InternalReadContentAsObjectAsync(false);
		}

		private async Task<object> InternalReadContentAsObjectAsync(bool unwrapTypedValue)
		{
			return (await this.InternalReadContentAsObjectTupleAsync(unwrapTypedValue).ConfigureAwait(false)).Item2;
		}

		private async Task<Tuple<string, object>> InternalReadContentAsObjectTupleAsync(bool unwrapTypedValue)
		{
			XmlNodeType nodeType = this.NodeType;
			Tuple<string, object> tuple;
			if (nodeType == XmlNodeType.Attribute)
			{
				string text = this.Value;
				if (this.attributePSVI != null && this.attributePSVI.typedAttributeValue != null)
				{
					if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute)
					{
						XmlSchemaAttribute schemaAttribute = this.attributePSVI.attributeSchemaInfo.SchemaAttribute;
						text = ((schemaAttribute.DefaultValue != null) ? schemaAttribute.DefaultValue : schemaAttribute.FixedValue);
					}
					tuple = new Tuple<string, object>(text, this.ReturnBoxedValue(this.attributePSVI.typedAttributeValue, this.AttributeSchemaInfo.XmlType, unwrapTypedValue));
				}
				else
				{
					tuple = new Tuple<string, object>(text, this.Value);
				}
			}
			else if (nodeType == XmlNodeType.EndElement)
			{
				if (this.atomicValue != null)
				{
					string text = this.originalAtomicValueString;
					tuple = new Tuple<string, object>(text, this.atomicValue);
				}
				else
				{
					string text = string.Empty;
					tuple = new Tuple<string, object>(text, string.Empty);
				}
			}
			else if (this.validator.CurrentContentType == XmlSchemaContentType.TextOnly)
			{
				object obj = await this.ReadTillEndElementAsync().ConfigureAwait(false);
				object obj2 = this.ReturnBoxedValue(obj, this.xmlSchemaInfo.XmlType, unwrapTypedValue);
				string text = this.originalAtomicValueString;
				tuple = new Tuple<string, object>(text, obj2);
			}
			else
			{
				XsdCachingReader xsdCachingReader = this.coreReader as XsdCachingReader;
				string text;
				if (xsdCachingReader != null)
				{
					text = xsdCachingReader.ReadOriginalContentAsString();
				}
				else
				{
					text = await base.InternalReadContentAsStringAsync().ConfigureAwait(false);
				}
				tuple = new Tuple<string, object>(text, text);
			}
			return tuple;
		}

		private Task<Tuple<XmlSchemaType, object>> InternalReadElementContentAsObjectAsync()
		{
			return this.InternalReadElementContentAsObjectAsync(false);
		}

		private async Task<Tuple<XmlSchemaType, object>> InternalReadElementContentAsObjectAsync(bool unwrapTypedValue)
		{
			Tuple<XmlSchemaType, string, object> tuple = await this.InternalReadElementContentAsObjectTupleAsync(unwrapTypedValue).ConfigureAwait(false);
			return new Tuple<XmlSchemaType, object>(tuple.Item1, tuple.Item3);
		}

		private async Task<Tuple<XmlSchemaType, string, object>> InternalReadElementContentAsObjectTupleAsync(bool unwrapTypedValue)
		{
			object typedValue = null;
			XmlSchemaType xmlType = null;
			Tuple<XmlSchemaType, string, object> tuple;
			if (this.IsEmptyElement)
			{
				if (this.xmlSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
				{
					typedValue = this.ReturnBoxedValue(this.atomicValue, this.xmlSchemaInfo.XmlType, unwrapTypedValue);
				}
				else
				{
					typedValue = this.atomicValue;
				}
				string originalString = this.originalAtomicValueString;
				xmlType = this.ElementXmlType;
				await this.ReadAsync().ConfigureAwait(false);
				tuple = new Tuple<XmlSchemaType, string, object>(xmlType, originalString, typedValue);
			}
			else
			{
				await this.ReadAsync().ConfigureAwait(false);
				string originalString;
				if (this.NodeType == XmlNodeType.EndElement)
				{
					if (this.xmlSchemaInfo.IsDefault)
					{
						if (this.xmlSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
						{
							typedValue = this.ReturnBoxedValue(this.atomicValue, this.xmlSchemaInfo.XmlType, unwrapTypedValue);
						}
						else
						{
							typedValue = this.atomicValue;
						}
						originalString = this.originalAtomicValueString;
					}
					else
					{
						typedValue = string.Empty;
						originalString = string.Empty;
					}
				}
				else
				{
					if (this.NodeType == XmlNodeType.Element)
					{
						throw new XmlException("ReadElementContentAs() methods cannot be called on an element that has child elements.", string.Empty, this);
					}
					Tuple<string, object> tuple2 = await this.InternalReadContentAsObjectTupleAsync(unwrapTypedValue).ConfigureAwait(false);
					originalString = tuple2.Item1;
					typedValue = tuple2.Item2;
					if (this.NodeType != XmlNodeType.EndElement)
					{
						throw new XmlException("ReadElementContentAs() methods cannot be called on an element that has child elements.", string.Empty, this);
					}
				}
				xmlType = this.ElementXmlType;
				await this.ReadAsync().ConfigureAwait(false);
				tuple = new Tuple<XmlSchemaType, string, object>(xmlType, originalString, typedValue);
			}
			return tuple;
		}

		private async Task<object> ReadTillEndElementAsync()
		{
			if (this.atomicValue == null)
			{
				for (;;)
				{
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.coreReader.ReadAsync().ConfigureAwait(false).GetAwaiter();
					if (!configuredTaskAwaiter.IsCompleted)
					{
						await configuredTaskAwaiter;
						ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
						configuredTaskAwaiter = configuredTaskAwaiter2;
						configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
					}
					if (!configuredTaskAwaiter.GetResult())
					{
						goto Block_6;
					}
					if (!this.replayCache)
					{
						switch (this.coreReader.NodeType)
						{
						case XmlNodeType.Element:
							goto IL_008B;
						case XmlNodeType.Text:
						case XmlNodeType.CDATA:
							this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
							break;
						case XmlNodeType.Whitespace:
						case XmlNodeType.SignificantWhitespace:
							this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
							break;
						case XmlNodeType.EndElement:
							goto IL_012A;
						}
					}
				}
				IL_008B:
				await this.ProcessReaderEventAsync().ConfigureAwait(false);
				goto IL_01F0;
				IL_012A:
				this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
				this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
				if (this.manageNamespaces)
				{
					this.nsManager.PopScope();
				}
				Block_6:;
			}
			else
			{
				if (this.atomicValue == this)
				{
					this.atomicValue = null;
				}
				this.SwitchReader();
			}
			IL_01F0:
			return this.atomicValue;
		}

		private XmlReader coreReader;

		private IXmlNamespaceResolver coreReaderNSResolver;

		private IXmlNamespaceResolver thisNSResolver;

		private XmlSchemaValidator validator;

		private XmlResolver xmlResolver;

		private ValidationEventHandler validationEvent;

		private XsdValidatingReader.ValidatingReaderState validationState;

		private XmlValueGetter valueGetter;

		private XmlNamespaceManager nsManager;

		private bool manageNamespaces;

		private bool processInlineSchema;

		private bool replayCache;

		private ValidatingReaderNodeData cachedNode;

		private AttributePSVIInfo attributePSVI;

		private int attributeCount;

		private int coreReaderAttributeCount;

		private int currentAttrIndex;

		private AttributePSVIInfo[] attributePSVINodes;

		private ArrayList defaultAttributes;

		private Parser inlineSchemaParser;

		private object atomicValue;

		private XmlSchemaInfo xmlSchemaInfo;

		private string originalAtomicValueString;

		private XmlNameTable coreReaderNameTable;

		private XsdCachingReader cachingReader;

		private ValidatingReaderNodeData textNode;

		private string NsXmlNs;

		private string NsXs;

		private string NsXsi;

		private string XsiType;

		private string XsiNil;

		private string XsdSchema;

		private string XsiSchemaLocation;

		private string XsiNoNamespaceSchemaLocation;

		private XmlCharType xmlCharType = XmlCharType.Instance;

		private IXmlLineInfo lineInfo;

		private ReadContentAsBinaryHelper readBinaryHelper;

		private XsdValidatingReader.ValidatingReaderState savedState;

		private const int InitialAttributeCount = 8;

		private static volatile Type TypeOfString;

		private enum ValidatingReaderState
		{
			None,
			Init,
			Read,
			OnDefaultAttribute = -1,
			OnReadAttributeValue = -2,
			OnAttribute = 3,
			ClearAttributes,
			ParseInlineSchema,
			ReadAhead,
			OnReadBinaryContent,
			ReaderClosed,
			EOF,
			Error
		}
	}
}
