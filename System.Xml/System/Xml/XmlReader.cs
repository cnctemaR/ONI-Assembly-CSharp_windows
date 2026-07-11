using System;
using System.IO;
using System.Text;
using System.Xml.Schema;
using Mono.Xml;
using Mono.Xml.Schema;

namespace System.Xml
{
	public abstract class XmlReader : IDisposable
	{
		void IDisposable.Dispose()
		{
			this.Dispose(false);
		}

		public abstract int AttributeCount { get; }

		public abstract string BaseURI { get; }

		internal XmlReaderBinarySupport Binary
		{
			get
			{
				return this.binary;
			}
		}

		internal XmlReaderBinarySupport.CharGetter BinaryCharGetter
		{
			get
			{
				return (this.binary == null) ? null : this.binary.Getter;
			}
			set
			{
				if (this.binary == null)
				{
					this.binary = new XmlReaderBinarySupport(this);
				}
				this.binary.Getter = value;
			}
		}

		public virtual bool CanReadBinaryContent
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanReadValueChunk
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanResolveEntity
		{
			get
			{
				return false;
			}
		}

		public abstract int Depth { get; }

		public abstract bool EOF { get; }

		public virtual bool HasAttributes
		{
			get
			{
				return this.AttributeCount > 0;
			}
		}

		public abstract bool HasValue { get; }

		public abstract bool IsEmptyElement { get; }

		public virtual bool IsDefault
		{
			get
			{
				return false;
			}
		}

		public virtual string this[int i]
		{
			get
			{
				return this.GetAttribute(i);
			}
		}

		public virtual string this[string name]
		{
			get
			{
				return this.GetAttribute(name);
			}
		}

		public virtual string this[string name, string namespaceURI]
		{
			get
			{
				return this.GetAttribute(name, namespaceURI);
			}
		}

		public abstract string LocalName { get; }

		public virtual string Name
		{
			get
			{
				return (this.Prefix.Length <= 0) ? this.LocalName : (this.Prefix + ":" + this.LocalName);
			}
		}

		public abstract string NamespaceURI { get; }

		public abstract XmlNameTable NameTable { get; }

		public abstract XmlNodeType NodeType { get; }

		public abstract string Prefix { get; }

		public virtual char QuoteChar
		{
			get
			{
				return '"';
			}
		}

		public abstract ReadState ReadState { get; }

		public virtual IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return null;
			}
		}

		public virtual XmlReaderSettings Settings
		{
			get
			{
				return this.settings;
			}
		}

		public abstract string Value { get; }

		public virtual string XmlLang
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		public abstract void Close();

		private static XmlNameTable PopulateNameTable(XmlReaderSettings settings)
		{
			XmlNameTable xmlNameTable = settings.NameTable;
			if (xmlNameTable == null)
			{
				xmlNameTable = new NameTable();
			}
			return xmlNameTable;
		}

		private static XmlParserContext PopulateParserContext(XmlReaderSettings settings, string baseUri)
		{
			XmlNameTable xmlNameTable = XmlReader.PopulateNameTable(settings);
			return new XmlParserContext(xmlNameTable, new XmlNamespaceManager(xmlNameTable), null, null, null, null, baseUri, null, XmlSpace.None, null);
		}

		private static XmlNodeType GetNodeType(XmlReaderSettings settings)
		{
			ConformanceLevel conformanceLevel = ((settings == null) ? ConformanceLevel.Auto : settings.ConformanceLevel);
			return (conformanceLevel != ConformanceLevel.Fragment) ? XmlNodeType.Document : XmlNodeType.Element;
		}

		public static XmlReader Create(Stream stream)
		{
			return XmlReader.Create(stream, null);
		}

		public static XmlReader Create(string url)
		{
			return XmlReader.Create(url, null);
		}

		public static XmlReader Create(TextReader reader)
		{
			return XmlReader.Create(reader, null);
		}

		public static XmlReader Create(string url, XmlReaderSettings settings)
		{
			return XmlReader.Create(url, settings, null);
		}

		public static XmlReader Create(Stream stream, XmlReaderSettings settings)
		{
			return XmlReader.Create(stream, settings, string.Empty);
		}

		public static XmlReader Create(TextReader reader, XmlReaderSettings settings)
		{
			return XmlReader.Create(reader, settings, string.Empty);
		}

		private static XmlReaderSettings PopulateSettings(XmlReaderSettings src)
		{
			if (src == null)
			{
				return new XmlReaderSettings();
			}
			return src.Clone();
		}

		public static XmlReader Create(Stream stream, XmlReaderSettings settings, string baseUri)
		{
			settings = XmlReader.PopulateSettings(settings);
			return XmlReader.Create(stream, settings, XmlReader.PopulateParserContext(settings, baseUri));
		}

		public static XmlReader Create(TextReader reader, XmlReaderSettings settings, string baseUri)
		{
			settings = XmlReader.PopulateSettings(settings);
			return XmlReader.Create(reader, settings, XmlReader.PopulateParserContext(settings, baseUri));
		}

		public static XmlReader Create(XmlReader reader, XmlReaderSettings settings)
		{
			settings = XmlReader.PopulateSettings(settings);
			XmlReader xmlReader = XmlReader.CreateFilteredXmlReader(reader, settings);
			xmlReader.settings = settings;
			return xmlReader;
		}

		public static XmlReader Create(string url, XmlReaderSettings settings, XmlParserContext context)
		{
			settings = XmlReader.PopulateSettings(settings);
			bool closeInput = settings.CloseInput;
			XmlReader xmlReader2;
			try
			{
				settings.CloseInput = true;
				if (context == null)
				{
					context = XmlReader.PopulateParserContext(settings, url);
				}
				XmlTextReader xmlTextReader = new XmlTextReader(false, settings.XmlResolver, url, XmlReader.GetNodeType(settings), context);
				XmlReader xmlReader = XmlReader.CreateCustomizedTextReader(xmlTextReader, settings);
				xmlReader2 = xmlReader;
			}
			finally
			{
				settings.CloseInput = closeInput;
			}
			return xmlReader2;
		}

		public static XmlReader Create(Stream stream, XmlReaderSettings settings, XmlParserContext context)
		{
			settings = XmlReader.PopulateSettings(settings);
			if (context == null)
			{
				context = XmlReader.PopulateParserContext(settings, string.Empty);
			}
			return XmlReader.CreateCustomizedTextReader(new XmlTextReader(stream, XmlReader.GetNodeType(settings), context), settings);
		}

		public static XmlReader Create(TextReader reader, XmlReaderSettings settings, XmlParserContext context)
		{
			settings = XmlReader.PopulateSettings(settings);
			if (context == null)
			{
				context = XmlReader.PopulateParserContext(settings, string.Empty);
			}
			return XmlReader.CreateCustomizedTextReader(new XmlTextReader(context.BaseURI, reader, XmlReader.GetNodeType(settings), context), settings);
		}

		private static XmlReader CreateCustomizedTextReader(XmlTextReader reader, XmlReaderSettings settings)
		{
			reader.XmlResolver = settings.XmlResolver;
			reader.Normalization = true;
			reader.EntityHandling = EntityHandling.ExpandEntities;
			if (settings.ProhibitDtd)
			{
				reader.ProhibitDtd = true;
			}
			if (!settings.CheckCharacters)
			{
				reader.CharacterChecking = false;
			}
			reader.CloseInput = settings.CloseInput;
			reader.Conformance = settings.ConformanceLevel;
			reader.AdjustLineInfoOffset(settings.LineNumberOffset, settings.LinePositionOffset);
			if (settings.NameTable != null)
			{
				reader.SetNameTable(settings.NameTable);
			}
			XmlReader xmlReader = XmlReader.CreateFilteredXmlReader(reader, settings);
			xmlReader.settings = settings;
			return xmlReader;
		}

		private static XmlReader CreateFilteredXmlReader(XmlReader reader, XmlReaderSettings settings)
		{
			ConformanceLevel conformanceLevel;
			if (reader is XmlTextReader)
			{
				conformanceLevel = ((XmlTextReader)reader).Conformance;
			}
			else if (reader.Settings != null)
			{
				conformanceLevel = reader.Settings.ConformanceLevel;
			}
			else
			{
				conformanceLevel = settings.ConformanceLevel;
			}
			if (settings.ConformanceLevel != ConformanceLevel.Auto && conformanceLevel != settings.ConformanceLevel)
			{
				throw new InvalidOperationException(string.Format("ConformanceLevel cannot be overwritten by a wrapping XmlReader. The source reader has {0}, while {1} is specified.", conformanceLevel, settings.ConformanceLevel));
			}
			settings.ConformanceLevel = conformanceLevel;
			reader = XmlReader.CreateValidatingXmlReader(reader, settings);
			if (settings.IgnoreComments || settings.IgnoreProcessingInstructions || settings.IgnoreWhitespace)
			{
				return new XmlFilterReader(reader, settings);
			}
			reader.settings = settings;
			return reader;
		}

		private static XmlReader CreateValidatingXmlReader(XmlReader reader, XmlReaderSettings settings)
		{
			switch (settings.ValidationType)
			{
			case ValidationType.DTD:
			{
				XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(reader);
				xmlValidatingReader.XmlResolver = settings.XmlResolver;
				xmlValidatingReader.ValidationType = ValidationType.DTD;
				if ((settings.ValidationFlags & XmlSchemaValidationFlags.ProcessIdentityConstraints) == XmlSchemaValidationFlags.None)
				{
					throw new NotImplementedException();
				}
				return (xmlValidatingReader == null) ? reader : xmlValidatingReader;
			}
			default:
				return reader;
			case ValidationType.Schema:
				return new XmlSchemaValidatingReader(reader, settings);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.ReadState != ReadState.Closed)
			{
				this.Close();
			}
		}

		public abstract string GetAttribute(int i);

		public abstract string GetAttribute(string name);

		public abstract string GetAttribute(string localName, string namespaceName);

		public static bool IsName(string s)
		{
			return s != null && XmlChar.IsName(s);
		}

		public static bool IsNameToken(string s)
		{
			return s != null && XmlChar.IsNmToken(s);
		}

		public virtual bool IsStartElement()
		{
			return this.MoveToContent() == XmlNodeType.Element;
		}

		public virtual bool IsStartElement(string name)
		{
			return this.IsStartElement() && this.Name == name;
		}

		public virtual bool IsStartElement(string localName, string namespaceName)
		{
			return this.IsStartElement() && this.LocalName == localName && this.NamespaceURI == namespaceName;
		}

		public abstract string LookupNamespace(string prefix);

		public virtual void MoveToAttribute(int i)
		{
			if (i >= this.AttributeCount)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.MoveToFirstAttribute();
			for (int j = 0; j < i; j++)
			{
				this.MoveToNextAttribute();
			}
		}

		public abstract bool MoveToAttribute(string name);

		public abstract bool MoveToAttribute(string localName, string namespaceName);

		private bool IsContent(XmlNodeType nodeType)
		{
			switch (nodeType)
			{
			case XmlNodeType.Element:
				return true;
			default:
				return nodeType == XmlNodeType.EndElement || nodeType == XmlNodeType.EndEntity;
			case XmlNodeType.Text:
				return true;
			case XmlNodeType.CDATA:
				return true;
			case XmlNodeType.EntityReference:
				return true;
			}
		}

		public virtual XmlNodeType MoveToContent()
		{
			ReadState readState = this.ReadState;
			if (readState != ReadState.Initial && readState != ReadState.Interactive)
			{
				return this.NodeType;
			}
			if (this.NodeType == XmlNodeType.Attribute)
			{
				this.MoveToElement();
			}
			while (!this.IsContent(this.NodeType))
			{
				this.Read();
				if (this.EOF)
				{
					return XmlNodeType.None;
				}
			}
			return this.NodeType;
		}

		public abstract bool MoveToElement();

		public abstract bool MoveToFirstAttribute();

		public abstract bool MoveToNextAttribute();

		public abstract bool Read();

		public abstract bool ReadAttributeValue();

		public virtual string ReadElementString()
		{
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				string text = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
				throw this.XmlError(text);
			}
			string text2 = string.Empty;
			if (!this.IsEmptyElement)
			{
				this.Read();
				text2 = this.ReadString();
				if (this.NodeType != XmlNodeType.EndElement)
				{
					string text3 = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
					throw this.XmlError(text3);
				}
			}
			this.Read();
			return text2;
		}

		public virtual string ReadElementString(string name)
		{
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				string text = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
				throw this.XmlError(text);
			}
			if (name != this.Name)
			{
				string text2 = string.Format("The {0} tag from namespace {1} is expected.", this.Name, this.NamespaceURI);
				throw this.XmlError(text2);
			}
			string text3 = string.Empty;
			if (!this.IsEmptyElement)
			{
				this.Read();
				text3 = this.ReadString();
				if (this.NodeType != XmlNodeType.EndElement)
				{
					string text4 = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
					throw this.XmlError(text4);
				}
			}
			this.Read();
			return text3;
		}

		public virtual string ReadElementString(string localName, string namespaceName)
		{
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				string text = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
				throw this.XmlError(text);
			}
			if (localName != this.LocalName || this.NamespaceURI != namespaceName)
			{
				string text2 = string.Format("The {0} tag from namespace {1} is expected.", this.LocalName, this.NamespaceURI);
				throw this.XmlError(text2);
			}
			string text3 = string.Empty;
			if (!this.IsEmptyElement)
			{
				this.Read();
				text3 = this.ReadString();
				if (this.NodeType != XmlNodeType.EndElement)
				{
					string text4 = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
					throw this.XmlError(text4);
				}
			}
			this.Read();
			return text3;
		}

		public virtual void ReadEndElement()
		{
			if (this.MoveToContent() != XmlNodeType.EndElement)
			{
				string text = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
				throw this.XmlError(text);
			}
			this.Read();
		}

		public virtual string ReadInnerXml()
		{
			if (this.ReadState != ReadState.Interactive || this.NodeType == XmlNodeType.EndElement)
			{
				return string.Empty;
			}
			if (this.IsEmptyElement)
			{
				this.Read();
				return string.Empty;
			}
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			if (this.NodeType == XmlNodeType.Element)
			{
				int i = this.Depth;
				this.Read();
				while (i < this.Depth)
				{
					if (this.ReadState != ReadState.Interactive)
					{
						throw this.XmlError("Unexpected end of the XML reader.");
					}
					xmlTextWriter.WriteNode(this, false);
				}
				this.Read();
			}
			else
			{
				xmlTextWriter.WriteNode(this, false);
			}
			return stringWriter.ToString();
		}

		public virtual string ReadOuterXml()
		{
			if (this.ReadState != ReadState.Interactive || this.NodeType == XmlNodeType.EndElement)
			{
				return string.Empty;
			}
			XmlNodeType nodeType = this.NodeType;
			if (nodeType != XmlNodeType.Element && nodeType != XmlNodeType.Attribute)
			{
				this.Skip();
				return string.Empty;
			}
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.WriteNode(this, false);
			return stringWriter.ToString();
		}

		public virtual void ReadStartElement()
		{
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				string text = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
				throw this.XmlError(text);
			}
			this.Read();
		}

		public virtual void ReadStartElement(string name)
		{
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				string text = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
				throw this.XmlError(text);
			}
			if (name != this.Name)
			{
				string text2 = string.Format("The {0} tag from namespace {1} is expected.", this.Name, this.NamespaceURI);
				throw this.XmlError(text2);
			}
			this.Read();
		}

		public virtual void ReadStartElement(string localName, string namespaceName)
		{
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				string text = string.Format("'{0}' is an invalid node type.", this.NodeType.ToString());
				throw this.XmlError(text);
			}
			if (localName != this.LocalName || this.NamespaceURI != namespaceName)
			{
				string text2 = string.Format("Expecting {0} tag from namespace {1}, got {2} and {3} instead", new object[] { localName, namespaceName, this.LocalName, this.NamespaceURI });
				throw this.XmlError(text2);
			}
			this.Read();
		}

		public virtual string ReadString()
		{
			if (this.readStringBuffer == null)
			{
				this.readStringBuffer = new StringBuilder();
			}
			this.readStringBuffer.Length = 0;
			this.MoveToElement();
			XmlNodeType nodeType = this.NodeType;
			switch (nodeType)
			{
			case XmlNodeType.Element:
				if (this.IsEmptyElement)
				{
					return string.Empty;
				}
				for (;;)
				{
					this.Read();
					XmlNodeType xmlNodeType = this.NodeType;
					if (xmlNodeType != XmlNodeType.Text && xmlNodeType != XmlNodeType.CDATA && xmlNodeType != XmlNodeType.Whitespace && xmlNodeType != XmlNodeType.SignificantWhitespace)
					{
						break;
					}
					this.readStringBuffer.Append(this.Value);
				}
				goto IL_0122;
			default:
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.SignificantWhitespace)
				{
					return string.Empty;
				}
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				break;
			}
			for (;;)
			{
				XmlNodeType xmlNodeType = this.NodeType;
				if (xmlNodeType != XmlNodeType.Text && xmlNodeType != XmlNodeType.CDATA && xmlNodeType != XmlNodeType.Whitespace && xmlNodeType != XmlNodeType.SignificantWhitespace)
				{
					break;
				}
				this.readStringBuffer.Append(this.Value);
				this.Read();
			}
			IL_0122:
			string text = this.readStringBuffer.ToString();
			this.readStringBuffer.Length = 0;
			return text;
		}

		public virtual Type ValueType
		{
			get
			{
				return typeof(string);
			}
		}

		public virtual bool ReadToDescendant(string name)
		{
			if (this.ReadState == ReadState.Initial)
			{
				this.MoveToContent();
				if (this.IsStartElement(name))
				{
					return true;
				}
			}
			if (this.NodeType != XmlNodeType.Element || this.IsEmptyElement)
			{
				return false;
			}
			int i = this.Depth;
			this.Read();
			while (i < this.Depth)
			{
				if (this.NodeType == XmlNodeType.Element && name == this.Name)
				{
					return true;
				}
				this.Read();
			}
			return false;
		}

		public virtual bool ReadToDescendant(string localName, string namespaceURI)
		{
			if (this.ReadState == ReadState.Initial)
			{
				this.MoveToContent();
				if (this.IsStartElement(localName, namespaceURI))
				{
					return true;
				}
			}
			if (this.NodeType != XmlNodeType.Element || this.IsEmptyElement)
			{
				return false;
			}
			int i = this.Depth;
			this.Read();
			while (i < this.Depth)
			{
				if (this.NodeType == XmlNodeType.Element && localName == this.LocalName && namespaceURI == this.NamespaceURI)
				{
					return true;
				}
				this.Read();
			}
			return false;
		}

		public virtual bool ReadToFollowing(string name)
		{
			while (this.Read())
			{
				if (this.NodeType == XmlNodeType.Element && name == this.Name)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool ReadToFollowing(string localName, string namespaceURI)
		{
			while (this.Read())
			{
				if (this.NodeType == XmlNodeType.Element && localName == this.LocalName && namespaceURI == this.NamespaceURI)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool ReadToNextSibling(string name)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return false;
			}
			int depth = this.Depth;
			this.Skip();
			while (!this.EOF && depth <= this.Depth)
			{
				if (this.NodeType == XmlNodeType.Element && name == this.Name)
				{
					return true;
				}
				this.Skip();
			}
			return false;
		}

		public virtual bool ReadToNextSibling(string localName, string namespaceURI)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return false;
			}
			int depth = this.Depth;
			this.Skip();
			while (!this.EOF && depth <= this.Depth)
			{
				if (this.NodeType == XmlNodeType.Element && localName == this.LocalName && namespaceURI == this.NamespaceURI)
				{
					return true;
				}
				this.Skip();
			}
			return false;
		}

		public virtual XmlReader ReadSubtree()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw new InvalidOperationException(string.Format("ReadSubtree() can be invoked only when the reader is positioned on an element. Current node is {0}. {1}", this.NodeType, this.GetLocation()));
			}
			return new SubtreeXmlReader(this);
		}

		private string ReadContentString()
		{
			if (this.NodeType == XmlNodeType.Attribute || (this.NodeType != XmlNodeType.Element && this.HasAttributes))
			{
				return this.Value;
			}
			return this.ReadContentString(true);
		}

		private string ReadContentString(bool isText)
		{
			if (isText)
			{
				XmlNodeType xmlNodeType = this.NodeType;
				switch (xmlNodeType)
				{
				case XmlNodeType.Element:
					throw new InvalidOperationException(string.Format("Node type {0} is not supported in this operation.{1}", this.NodeType, this.GetLocation()));
				default:
					if (xmlNodeType != XmlNodeType.Whitespace && xmlNodeType != XmlNodeType.SignificantWhitespace)
					{
						return string.Empty;
					}
					break;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					break;
				}
			}
			string text = string.Empty;
			for (;;)
			{
				XmlNodeType xmlNodeType = this.NodeType;
				switch (xmlNodeType)
				{
				case XmlNodeType.Element:
					goto IL_00A5;
				default:
					switch (xmlNodeType)
					{
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						goto IL_00BB;
					case XmlNodeType.EndElement:
						return text;
					}
					break;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					goto IL_00BB;
				}
				IL_00CD:
				if (!this.Read())
				{
					goto Block_6;
				}
				continue;
				IL_00BB:
				text += this.Value;
				goto IL_00CD;
			}
			IL_00A5:
			if (isText)
			{
				return text;
			}
			throw this.XmlError("Child element is not expected in this operation.");
			Block_6:
			throw this.XmlError("Unexpected end of document.");
		}

		private string GetLocation()
		{
			IXmlLineInfo xmlLineInfo = this as IXmlLineInfo;
			return (xmlLineInfo == null || !xmlLineInfo.HasLineInfo()) ? string.Empty : string.Format(" {0} (line {1}, column {2})", this.BaseURI, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
		}

		[MonoTODO]
		public virtual object ReadElementContentAsObject()
		{
			return this.ReadElementContentAs(this.ValueType, null);
		}

		[MonoTODO]
		public virtual object ReadElementContentAsObject(string localName, string namespaceURI)
		{
			return this.ReadElementContentAs(this.ValueType, null, localName, namespaceURI);
		}

		[MonoTODO]
		public virtual object ReadContentAsObject()
		{
			return this.ReadContentAs(this.ValueType, null);
		}

		public virtual object ReadElementContentAs(Type type, IXmlNamespaceResolver resolver)
		{
			bool isEmptyElement = this.IsEmptyElement;
			this.ReadStartElement();
			object obj = this.ValueAs((!isEmptyElement) ? this.ReadContentString(false) : string.Empty, type, resolver);
			if (!isEmptyElement)
			{
				this.ReadEndElement();
			}
			return obj;
		}

		public virtual object ReadElementContentAs(Type type, IXmlNamespaceResolver resolver, string localName, string namespaceURI)
		{
			this.ReadStartElement(localName, namespaceURI);
			object obj = this.ReadContentAs(type, resolver);
			this.ReadEndElement();
			return obj;
		}

		public virtual object ReadContentAs(Type type, IXmlNamespaceResolver resolver)
		{
			return this.ValueAs(this.ReadContentString(), type, resolver);
		}

		private object ValueAs(string text, Type type, IXmlNamespaceResolver resolver)
		{
			try
			{
				if (type == typeof(object))
				{
					return text;
				}
				if (type == typeof(XmlQualifiedName))
				{
					if (resolver != null)
					{
						return XmlQualifiedName.Parse(text, resolver);
					}
					return XmlQualifiedName.Parse(text, this);
				}
				else
				{
					if (type == typeof(DateTimeOffset))
					{
						return XmlConvert.ToDateTimeOffset(text);
					}
					switch (Type.GetTypeCode(type))
					{
					case TypeCode.Boolean:
						return XQueryConvert.StringToBoolean(text);
					case TypeCode.Int32:
						return XQueryConvert.StringToInt(text);
					case TypeCode.Int64:
						return XQueryConvert.StringToInteger(text);
					case TypeCode.Single:
						return XQueryConvert.StringToFloat(text);
					case TypeCode.Double:
						return XQueryConvert.StringToDouble(text);
					case TypeCode.Decimal:
						return XQueryConvert.StringToDecimal(text);
					case TypeCode.DateTime:
						return XQueryConvert.StringToDateTime(text);
					case TypeCode.String:
						return text;
					}
				}
			}
			catch (Exception ex)
			{
				throw this.XmlError(string.Format("Current text value '{0}' is not acceptable for specified type '{1}'. {2}", text, type, (ex == null) ? string.Empty : ex.Message), ex);
			}
			throw new ArgumentException(string.Format("Specified type '{0}' is not supported.", type));
		}

		public virtual bool ReadElementContentAsBoolean()
		{
			bool flag;
			try
			{
				flag = XQueryConvert.StringToBoolean(this.ReadElementContentAsString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return flag;
		}

		public virtual DateTime ReadElementContentAsDateTime()
		{
			DateTime dateTime;
			try
			{
				dateTime = XQueryConvert.StringToDateTime(this.ReadElementContentAsString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return dateTime;
		}

		public virtual decimal ReadElementContentAsDecimal()
		{
			decimal num;
			try
			{
				num = XQueryConvert.StringToDecimal(this.ReadElementContentAsString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual double ReadElementContentAsDouble()
		{
			double num;
			try
			{
				num = XQueryConvert.StringToDouble(this.ReadElementContentAsString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual float ReadElementContentAsFloat()
		{
			float num;
			try
			{
				num = XQueryConvert.StringToFloat(this.ReadElementContentAsString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual int ReadElementContentAsInt()
		{
			int num;
			try
			{
				num = XQueryConvert.StringToInt(this.ReadElementContentAsString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual long ReadElementContentAsLong()
		{
			long num;
			try
			{
				num = XQueryConvert.StringToInteger(this.ReadElementContentAsString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual string ReadElementContentAsString()
		{
			bool isEmptyElement = this.IsEmptyElement;
			if (this.NodeType != XmlNodeType.Element)
			{
				throw new InvalidOperationException(string.Format("'{0}' is an element node.", this.NodeType));
			}
			this.ReadStartElement();
			if (isEmptyElement)
			{
				return string.Empty;
			}
			string text = this.ReadContentString(false);
			this.ReadEndElement();
			return text;
		}

		public virtual bool ReadElementContentAsBoolean(string localName, string namespaceURI)
		{
			bool flag;
			try
			{
				flag = XQueryConvert.StringToBoolean(this.ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return flag;
		}

		public virtual DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
		{
			DateTime dateTime;
			try
			{
				dateTime = XQueryConvert.StringToDateTime(this.ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return dateTime;
		}

		public virtual decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
		{
			decimal num;
			try
			{
				num = XQueryConvert.StringToDecimal(this.ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual double ReadElementContentAsDouble(string localName, string namespaceURI)
		{
			double num;
			try
			{
				num = XQueryConvert.StringToDouble(this.ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual float ReadElementContentAsFloat(string localName, string namespaceURI)
		{
			float num;
			try
			{
				num = XQueryConvert.StringToFloat(this.ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual int ReadElementContentAsInt(string localName, string namespaceURI)
		{
			int num;
			try
			{
				num = XQueryConvert.StringToInt(this.ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual long ReadElementContentAsLong(string localName, string namespaceURI)
		{
			long num;
			try
			{
				num = XQueryConvert.StringToInteger(this.ReadElementContentAsString(localName, namespaceURI));
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual string ReadElementContentAsString(string localName, string namespaceURI)
		{
			bool isEmptyElement = this.IsEmptyElement;
			if (this.NodeType != XmlNodeType.Element)
			{
				throw new InvalidOperationException(string.Format("'{0}' is an element node.", this.NodeType));
			}
			this.ReadStartElement(localName, namespaceURI);
			if (isEmptyElement)
			{
				return string.Empty;
			}
			string text = this.ReadContentString(false);
			this.ReadEndElement();
			return text;
		}

		public virtual bool ReadContentAsBoolean()
		{
			bool flag;
			try
			{
				flag = XQueryConvert.StringToBoolean(this.ReadContentString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return flag;
		}

		public virtual DateTime ReadContentAsDateTime()
		{
			DateTime dateTime;
			try
			{
				dateTime = XQueryConvert.StringToDateTime(this.ReadContentString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return dateTime;
		}

		public virtual decimal ReadContentAsDecimal()
		{
			decimal num;
			try
			{
				num = XQueryConvert.StringToDecimal(this.ReadContentString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual double ReadContentAsDouble()
		{
			double num;
			try
			{
				num = XQueryConvert.StringToDouble(this.ReadContentString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual float ReadContentAsFloat()
		{
			float num;
			try
			{
				num = XQueryConvert.StringToFloat(this.ReadContentString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual int ReadContentAsInt()
		{
			int num;
			try
			{
				num = XQueryConvert.StringToInt(this.ReadContentString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual long ReadContentAsLong()
		{
			long num;
			try
			{
				num = XQueryConvert.StringToInteger(this.ReadContentString());
			}
			catch (FormatException ex)
			{
				throw this.XmlError("Typed value is invalid.", ex);
			}
			return num;
		}

		public virtual string ReadContentAsString()
		{
			return this.ReadContentString();
		}

		public virtual int ReadContentAsBase64(byte[] buffer, int offset, int length)
		{
			this.CheckSupport();
			return this.binary.ReadContentAsBase64(buffer, offset, length);
		}

		public virtual int ReadContentAsBinHex(byte[] buffer, int offset, int length)
		{
			this.CheckSupport();
			return this.binary.ReadContentAsBinHex(buffer, offset, length);
		}

		public virtual int ReadElementContentAsBase64(byte[] buffer, int offset, int length)
		{
			this.CheckSupport();
			return this.binary.ReadElementContentAsBase64(buffer, offset, length);
		}

		public virtual int ReadElementContentAsBinHex(byte[] buffer, int offset, int length)
		{
			this.CheckSupport();
			return this.binary.ReadElementContentAsBinHex(buffer, offset, length);
		}

		private void CheckSupport()
		{
			if (!this.CanReadBinaryContent || !this.CanReadValueChunk)
			{
				throw new NotSupportedException();
			}
			if (this.binary == null)
			{
				this.binary = new XmlReaderBinarySupport(this);
			}
		}

		public virtual int ReadValueChunk(char[] buffer, int offset, int length)
		{
			if (!this.CanReadValueChunk)
			{
				throw new NotSupportedException();
			}
			if (this.binary == null)
			{
				this.binary = new XmlReaderBinarySupport(this);
			}
			return this.binary.ReadValueChunk(buffer, offset, length);
		}

		public abstract void ResolveEntity();

		public virtual void Skip()
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return;
			}
			this.MoveToElement();
			if (this.NodeType != XmlNodeType.Element || this.IsEmptyElement)
			{
				this.Read();
				return;
			}
			int depth = this.Depth;
			while (this.Read() && depth < this.Depth)
			{
			}
			if (this.NodeType == XmlNodeType.EndElement)
			{
				this.Read();
			}
		}

		private XmlException XmlError(string message)
		{
			return new XmlException(this as IXmlLineInfo, this.BaseURI, message);
		}

		private XmlException XmlError(string message, Exception innerException)
		{
			return new XmlException(this as IXmlLineInfo, this.BaseURI, message);
		}

		private StringBuilder readStringBuffer;

		private XmlReaderBinarySupport binary;

		private XmlReaderSettings settings;
	}
}
