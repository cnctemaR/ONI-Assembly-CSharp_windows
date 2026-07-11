using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class XmlJsonWriter : XmlDictionaryWriter, IXmlJsonWriterInitializer
	{
		public XmlJsonWriter()
			: this(false, null)
		{
		}

		public XmlJsonWriter(bool indent, string indentChars)
		{
			this.indent = indent;
			if (indent)
			{
				if (indentChars == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("indentChars");
				}
				this.indentChars = indentChars;
			}
			this.InitializeWriter();
			if (XmlJsonWriter.CharacterAbbrevs == null)
			{
				XmlJsonWriter.CharacterAbbrevs = XmlJsonWriter.GetCharacterAbbrevs();
			}
		}

		private static char[] GetCharacterAbbrevs()
		{
			char[] array = new char[32];
			for (int i = 0; i < 32; i++)
			{
				char c;
				if (!LocalAppContextSwitches.DoNotUseEcmaScriptV6EscapeControlCharacter && XmlJsonWriter.TryEscapeControlCharacter((char)i, out c))
				{
					array[i] = c;
				}
				else
				{
					array[i] = '\0';
				}
			}
			return array;
		}

		private static bool TryEscapeControlCharacter(char ch, out char abbrev)
		{
			switch (ch)
			{
			case '\b':
				abbrev = 'b';
				return true;
			case '\t':
				abbrev = 't';
				return true;
			case '\n':
				abbrev = 'n';
				return true;
			case '\f':
				abbrev = 'f';
				return true;
			case '\r':
				abbrev = 'r';
				return true;
			}
			abbrev = ' ';
			return false;
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return null;
			}
		}

		public override WriteState WriteState
		{
			get
			{
				if (this.writeState == WriteState.Closed)
				{
					return WriteState.Closed;
				}
				if (this.HasOpenAttribute)
				{
					return WriteState.Attribute;
				}
				switch (this.nodeType)
				{
				case JsonNodeType.None:
					return WriteState.Start;
				case JsonNodeType.Element:
					return WriteState.Element;
				case JsonNodeType.EndElement:
				case JsonNodeType.QuotedText:
				case JsonNodeType.StandaloneText:
					return WriteState.Content;
				}
				return WriteState.Error;
			}
		}

		public override string XmlLang
		{
			get
			{
				return null;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		private static BinHexEncoding BinHexEncoding
		{
			[SecuritySafeCritical]
			get
			{
				if (XmlJsonWriter.binHexEncoding == null)
				{
					XmlJsonWriter.binHexEncoding = new BinHexEncoding();
				}
				return XmlJsonWriter.binHexEncoding;
			}
		}

		private bool HasOpenAttribute
		{
			get
			{
				return this.isWritingDataTypeAttribute || this.isWritingServerTypeAttribute || this.IsWritingNameAttribute || this.isWritingXmlnsAttribute;
			}
		}

		private bool IsClosed
		{
			get
			{
				return this.WriteState == WriteState.Closed;
			}
		}

		private bool IsWritingCollection
		{
			get
			{
				return this.depth > 0 && this.scopes[this.depth] == JsonNodeType.Collection;
			}
		}

		private bool IsWritingNameAttribute
		{
			get
			{
				return (this.nameState & XmlJsonWriter.NameState.IsWritingNameAttribute) == XmlJsonWriter.NameState.IsWritingNameAttribute;
			}
		}

		private bool IsWritingNameWithMapping
		{
			get
			{
				return (this.nameState & XmlJsonWriter.NameState.IsWritingNameWithMapping) == XmlJsonWriter.NameState.IsWritingNameWithMapping;
			}
		}

		private bool WrittenNameWithMapping
		{
			get
			{
				return (this.nameState & XmlJsonWriter.NameState.WrittenNameWithMapping) == XmlJsonWriter.NameState.WrittenNameWithMapping;
			}
		}

		public override void Close()
		{
			if (!this.IsClosed)
			{
				try
				{
					this.WriteEndDocument();
				}
				finally
				{
					try
					{
						this.nodeWriter.Flush();
						this.nodeWriter.Close();
					}
					finally
					{
						this.writeState = WriteState.Closed;
						if (this.depth != 0)
						{
							this.depth = 0;
						}
					}
				}
			}
		}

		public override void Flush()
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			this.nodeWriter.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			if (ns == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("ns");
			}
			if (ns == "http://www.w3.org/2000/xmlns/")
			{
				return "xmlns";
			}
			if (ns == "http://www.w3.org/XML/1998/namespace")
			{
				return "xml";
			}
			if (ns == string.Empty)
			{
				return string.Empty;
			}
			return null;
		}

		public void SetOutput(Stream stream, Encoding encoding, bool ownsStream)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			if (encoding == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encoding");
			}
			if (encoding.WebName != Encoding.UTF8.WebName)
			{
				stream = new JsonEncodingStreamWrapper(stream, encoding, false);
			}
			else
			{
				encoding = null;
			}
			if (this.nodeWriter == null)
			{
				this.nodeWriter = new XmlJsonWriter.JsonNodeWriter();
			}
			this.nodeWriter.SetOutput(stream, ownsStream, encoding);
			this.InitializeWriter();
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, bool[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, short[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, int[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, long[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, float[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, double[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, decimal[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, Guid[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int count)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("JSON WriteArray is not supported.")));
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			if (index < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - index)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("JSON size exceeded remaining buffer space, by {0} byte(s).", new object[] { buffer.Length - index })));
			}
			this.StartText();
			this.nodeWriter.WriteBase64Text(buffer, 0, buffer, index, count);
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			if (index < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - index)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("JSON size exceeded remaining buffer space, by {0} byte(s).", new object[] { buffer.Length - index })));
			}
			this.StartText();
			this.WriteEscapedJsonString(XmlJsonWriter.BinHexEncoding.GetString(buffer, index, count));
		}

		public override void WriteCData(string text)
		{
			this.WriteString(text);
		}

		public override void WriteCharEntity(char ch)
		{
			this.WriteString(ch.ToString());
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			if (index < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - index)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("JSON size exceeded remaining buffer space, by {0} byte(s).", new object[] { buffer.Length - index })));
			}
			this.WriteString(new string(buffer, index, count));
		}

		public override void WriteComment(string text)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Method {0} is not supported in JSON.", new object[] { "WriteComment" })));
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Method {0} is not supported in JSON.", new object[] { "WriteDocType" })));
		}

		public override void WriteEndAttribute()
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (!this.HasOpenAttribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("WriteEndAttribute was called while there is no open attribute.")));
			}
			if (this.isWritingDataTypeAttribute)
			{
				string text = this.attributeText;
				if (!(text == "number"))
				{
					if (!(text == "string"))
					{
						if (!(text == "array"))
						{
							if (!(text == "object"))
							{
								if (!(text == "null"))
								{
									if (!(text == "boolean"))
									{
										throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Unexpected attribute value '{0}'.", new object[] { this.attributeText })));
									}
									this.ThrowIfServerTypeWritten("boolean");
									this.dataType = XmlJsonWriter.JsonDataType.Boolean;
								}
								else
								{
									this.ThrowIfServerTypeWritten("null");
									this.dataType = XmlJsonWriter.JsonDataType.Null;
								}
							}
							else
							{
								this.dataType = XmlJsonWriter.JsonDataType.Object;
							}
						}
						else
						{
							this.ThrowIfServerTypeWritten("array");
							this.dataType = XmlJsonWriter.JsonDataType.Array;
						}
					}
					else
					{
						this.ThrowIfServerTypeWritten("string");
						this.dataType = XmlJsonWriter.JsonDataType.String;
					}
				}
				else
				{
					this.ThrowIfServerTypeWritten("number");
					this.dataType = XmlJsonWriter.JsonDataType.Number;
				}
				this.attributeText = null;
				this.isWritingDataTypeAttribute = false;
				if (!this.IsWritingNameWithMapping || this.WrittenNameWithMapping)
				{
					this.WriteDataTypeServerType();
					return;
				}
			}
			else if (this.isWritingServerTypeAttribute)
			{
				this.serverTypeValue = this.attributeText;
				this.attributeText = null;
				this.isWritingServerTypeAttribute = false;
				if ((!this.IsWritingNameWithMapping || this.WrittenNameWithMapping) && this.dataType == XmlJsonWriter.JsonDataType.Object)
				{
					this.WriteServerTypeAttribute();
					return;
				}
			}
			else
			{
				if (this.IsWritingNameAttribute)
				{
					this.WriteJsonElementName(this.attributeText);
					this.attributeText = null;
					this.nameState = XmlJsonWriter.NameState.IsWritingNameWithMapping | XmlJsonWriter.NameState.WrittenNameWithMapping;
					this.WriteDataTypeServerType();
					return;
				}
				if (this.isWritingXmlnsAttribute)
				{
					if (!string.IsNullOrEmpty(this.attributeText) && this.isWritingXmlnsAttributeDefaultNs)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("ns", global::System.Runtime.Serialization.SR.GetString("JSON namespace is specified as '{0}' but it must be empty.", new object[] { this.attributeText }));
					}
					this.attributeText = null;
					this.isWritingXmlnsAttribute = false;
					this.isWritingXmlnsAttributeDefaultNs = false;
				}
			}
		}

		public override void WriteEndDocument()
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (this.nodeType != JsonNodeType.None)
			{
				while (this.depth > 0)
				{
					this.WriteEndElement();
				}
			}
		}

		public override void WriteEndElement()
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (this.depth == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an end element while there was no open element in JSON writer.")));
			}
			if (this.HasOpenAttribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON attribute must be closed first before calling {0} method.", new object[] { "WriteEndElement" })));
			}
			this.endElementBuffer = false;
			JsonNodeType jsonNodeType = this.ExitScope();
			if (jsonNodeType == JsonNodeType.Collection)
			{
				this.indentLevel--;
				if (this.indent)
				{
					if (this.nodeType == JsonNodeType.Element)
					{
						this.nodeWriter.WriteText(32);
					}
					else
					{
						this.WriteNewLine();
						this.WriteIndent();
					}
				}
				this.nodeWriter.WriteText(93);
				jsonNodeType = this.ExitScope();
			}
			else if (this.nodeType == JsonNodeType.QuotedText)
			{
				this.WriteJsonQuote();
			}
			else if (this.nodeType == JsonNodeType.Element)
			{
				if (this.dataType == XmlJsonWriter.JsonDataType.None && this.serverTypeValue != null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("On JSON writer data type '{0}' must be specified. Object string is '{1}', server type string is '{2}'.", new object[] { "type", "object", "__type" })));
				}
				if (this.IsWritingNameWithMapping && !this.WrittenNameWithMapping)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("On JSON writer data type '{0}' must be specified. Object string is '{1}', server type string is '{2}'.", new object[]
					{
						"item",
						string.Empty,
						"item"
					})));
				}
				if (this.dataType == XmlJsonWriter.JsonDataType.None || this.dataType == XmlJsonWriter.JsonDataType.String)
				{
					this.nodeWriter.WriteText(34);
					this.nodeWriter.WriteText(34);
				}
			}
			if (this.depth != 0)
			{
				if (jsonNodeType == JsonNodeType.Element)
				{
					this.endElementBuffer = true;
				}
				else if (jsonNodeType == JsonNodeType.Object)
				{
					this.indentLevel--;
					if (this.indent)
					{
						if (this.nodeType == JsonNodeType.Element)
						{
							this.nodeWriter.WriteText(32);
						}
						else
						{
							this.WriteNewLine();
							this.WriteIndent();
						}
					}
					this.nodeWriter.WriteText(125);
					if (this.depth > 0 && this.scopes[this.depth] == JsonNodeType.Element)
					{
						this.ExitScope();
						this.endElementBuffer = true;
					}
				}
			}
			this.dataType = XmlJsonWriter.JsonDataType.None;
			this.nodeType = JsonNodeType.EndElement;
			this.nameState = XmlJsonWriter.NameState.None;
			this.wroteServerTypeAttribute = false;
		}

		public override void WriteEntityRef(string name)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Method {0} is not supported in JSON.", new object[] { "WriteEntityRef" })));
		}

		public override void WriteFullEndElement()
		{
			this.WriteEndElement();
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (!name.Equals("xml", StringComparison.OrdinalIgnoreCase))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("processing instruction is not supported in JSON writer."), "name"));
			}
			if (this.WriteState != WriteState.Start)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Attempt to write invalid XML declration.")));
			}
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localName");
			}
			if (localName.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("localName", global::System.Runtime.Serialization.SR.GetString("Empty string is invalid as a local name."));
			}
			if (ns == null)
			{
				ns = string.Empty;
			}
			base.WriteQualifiedName(localName, ns);
		}

		public override void WriteRaw(string data)
		{
			this.WriteString(data);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			if (index < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - index)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("JSON size exceeded remaining buffer space, by {0} byte(s).", new object[] { buffer.Length - index })));
			}
			this.WriteString(new string(buffer, index, count));
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (!string.IsNullOrEmpty(prefix))
			{
				if (!this.IsWritingNameWithMapping || !(prefix == "xmlns"))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("prefix", global::System.Runtime.Serialization.SR.GetString("JSON prefix must be null or empty. '{0}' is specified instead.", new object[] { prefix }));
				}
				if (ns != null && ns != "http://www.w3.org/2000/xmlns/")
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("The prefix '{0}' is bound to the namespace '{1}' and cannot be changed to '{2}'.", new object[] { "xmlns", "http://www.w3.org/2000/xmlns/", ns }), "ns"));
				}
			}
			else if (this.IsWritingNameWithMapping && ns == "http://www.w3.org/2000/xmlns/" && localName != "xmlns")
			{
				prefix = "xmlns";
			}
			if (!string.IsNullOrEmpty(ns))
			{
				if (this.IsWritingNameWithMapping && ns == "http://www.w3.org/2000/xmlns/")
				{
					prefix = "xmlns";
				}
				else
				{
					if (!string.IsNullOrEmpty(prefix) || !(localName == "xmlns") || !(ns == "http://www.w3.org/2000/xmlns/"))
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("ns", global::System.Runtime.Serialization.SR.GetString("JSON namespace is specified as '{0}' but it must be empty.", new object[] { ns }));
					}
					prefix = "xmlns";
					this.isWritingXmlnsAttributeDefaultNs = true;
				}
			}
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localName");
			}
			if (localName.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("localName", global::System.Runtime.Serialization.SR.GetString("Empty string is invalid as a local name."));
			}
			if (this.nodeType != JsonNodeType.Element && !this.wroteServerTypeAttribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON attribute must have an owner element.")));
			}
			if (this.HasOpenAttribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON attribute must be closed first before calling {0} method.", new object[] { "WriteStartAttribute" })));
			}
			if (prefix == "xmlns")
			{
				this.isWritingXmlnsAttribute = true;
				return;
			}
			if (localName == "type")
			{
				if (this.dataType != XmlJsonWriter.JsonDataType.None)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON attribute '{0}' is already written.", new object[] { "type" })));
				}
				this.isWritingDataTypeAttribute = true;
				return;
			}
			else if (localName == "__type")
			{
				if (this.serverTypeValue != null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON attribute '{0}' is already written.", new object[] { "__type" })));
				}
				if (this.dataType != XmlJsonWriter.JsonDataType.None && this.dataType != XmlJsonWriter.JsonDataType.Object)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Server type is specified for invalid data type in JSON. Server type: '{0}', type: '{1}', dataType: '{2}', object: '{3}'.", new object[]
					{
						"__type",
						"type",
						this.dataType.ToString().ToLowerInvariant(),
						"object"
					})));
				}
				this.isWritingServerTypeAttribute = true;
				return;
			}
			else
			{
				if (!(localName == "item"))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("localName", global::System.Runtime.Serialization.SR.GetString("Unexpected attribute local name '{0}'.", new object[] { localName }));
				}
				if (this.WrittenNameWithMapping)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON attribute '{0}' is already written.", new object[] { "item" })));
				}
				if (!this.IsWritingNameWithMapping)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an end element while there was no open element in JSON writer.")));
				}
				this.nameState |= XmlJsonWriter.NameState.IsWritingNameAttribute;
				return;
			}
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.WriteStartDocument();
		}

		public override void WriteStartDocument()
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (this.WriteState != WriteState.Start)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid write state {1} for '{0}' method.", new object[]
				{
					"WriteStartDocument",
					this.WriteState.ToString()
				})));
			}
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localName");
			}
			if (localName.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("localName", global::System.Runtime.Serialization.SR.GetString("Empty string is invalid as a local name."));
			}
			if (!string.IsNullOrEmpty(prefix) && (string.IsNullOrEmpty(ns) || !this.TrySetWritingNameWithMapping(localName, ns)))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("prefix", global::System.Runtime.Serialization.SR.GetString("JSON prefix must be null or empty. '{0}' is specified instead.", new object[] { prefix }));
			}
			if (!string.IsNullOrEmpty(ns) && !this.TrySetWritingNameWithMapping(localName, ns))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("ns", global::System.Runtime.Serialization.SR.GetString("JSON namespace is specified as '{0}' but it must be empty.", new object[] { ns }));
			}
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (this.HasOpenAttribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON attribute must be closed first before calling {0} method.", new object[] { "WriteStartElement" })));
			}
			if (this.nodeType != JsonNodeType.None && this.depth == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Multiple root element is not allowed on JSON writer.")));
			}
			switch (this.nodeType)
			{
			case JsonNodeType.None:
				if (!localName.Equals("root"))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid root element name '{0}' (root element is '{1}' in JSON).", new object[] { localName, "root" })));
				}
				this.EnterScope(JsonNodeType.Element);
				goto IL_027E;
			case JsonNodeType.Element:
				if (this.dataType != XmlJsonWriter.JsonDataType.Array && this.dataType != XmlJsonWriter.JsonDataType.Object)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Either Object or Array of JSON node type must be specified.")));
				}
				if (this.indent)
				{
					this.WriteNewLine();
					this.WriteIndent();
				}
				if (!this.IsWritingCollection)
				{
					if (this.nameState != XmlJsonWriter.NameState.IsWritingNameWithMapping)
					{
						this.WriteJsonElementName(localName);
					}
				}
				else if (!localName.Equals("item"))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid JSON item name '{0}' for array element (item element is '{1}' in JSON).", new object[] { localName, "item" })));
				}
				this.EnterScope(JsonNodeType.Element);
				goto IL_027E;
			case JsonNodeType.EndElement:
				if (this.endElementBuffer)
				{
					this.nodeWriter.WriteText(44);
				}
				if (this.indent)
				{
					this.WriteNewLine();
					this.WriteIndent();
				}
				if (!this.IsWritingCollection)
				{
					if (this.nameState != XmlJsonWriter.NameState.IsWritingNameWithMapping)
					{
						this.WriteJsonElementName(localName);
					}
				}
				else if (!localName.Equals("item"))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid JSON item name '{0}' for array element (item element is '{1}' in JSON).", new object[] { localName, "item" })));
				}
				this.EnterScope(JsonNodeType.Element);
				goto IL_027E;
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid call to JSON WriteStartElement method.")));
			IL_027E:
			this.isWritingDataTypeAttribute = false;
			this.isWritingServerTypeAttribute = false;
			this.isWritingXmlnsAttribute = false;
			this.wroteServerTypeAttribute = false;
			this.serverTypeValue = null;
			this.dataType = XmlJsonWriter.JsonDataType.None;
			this.nodeType = JsonNodeType.Element;
		}

		public override void WriteString(string text)
		{
			if (this.HasOpenAttribute && text != null)
			{
				this.attributeText += text;
				return;
			}
			if (text == null)
			{
				text = string.Empty;
			}
			if ((this.dataType != XmlJsonWriter.JsonDataType.Array && this.dataType != XmlJsonWriter.JsonDataType.Object && this.nodeType != JsonNodeType.EndElement) || !XmlConverter.IsWhitespace(text))
			{
				this.StartText();
				this.WriteEscapedJsonString(text);
			}
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.WriteString(highChar + lowChar);
		}

		public override void WriteValue(bool value)
		{
			this.StartText();
			this.nodeWriter.WriteBoolText(value);
		}

		public override void WriteValue(decimal value)
		{
			this.StartText();
			this.nodeWriter.WriteDecimalText(value);
		}

		public override void WriteValue(double value)
		{
			this.StartText();
			this.nodeWriter.WriteDoubleText(value);
		}

		public override void WriteValue(float value)
		{
			this.StartText();
			this.nodeWriter.WriteFloatText(value);
		}

		public override void WriteValue(int value)
		{
			this.StartText();
			this.nodeWriter.WriteInt32Text(value);
		}

		public override void WriteValue(long value)
		{
			this.StartText();
			this.nodeWriter.WriteInt64Text(value);
		}

		public override void WriteValue(Guid value)
		{
			this.StartText();
			this.nodeWriter.WriteGuidText(value);
		}

		public override void WriteValue(DateTime value)
		{
			this.StartText();
			this.nodeWriter.WriteDateTimeText(value);
		}

		public override void WriteValue(string value)
		{
			this.WriteString(value);
		}

		public override void WriteValue(TimeSpan value)
		{
			this.StartText();
			this.nodeWriter.WriteTimeSpanText(value);
		}

		public override void WriteValue(UniqueId value)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			this.StartText();
			this.nodeWriter.WriteUniqueIdText(value);
		}

		public override void WriteValue(object value)
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			if (value is Array)
			{
				this.WriteValue((Array)value);
				return;
			}
			if (value is IStreamProvider)
			{
				this.WriteValue((IStreamProvider)value);
				return;
			}
			this.WritePrimitiveValue(value);
		}

		public override void WriteWhitespace(string ws)
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (ws == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("ws");
			}
			foreach (char c in ws)
			{
				if (c != ' ' && c != '\t' && c != '\n' && c != '\r')
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("ws", global::System.Runtime.Serialization.SR.GetString("Only whitespace characters are allowed for {1} method. The specified value is '{0}'", new object[]
					{
						c.ToString(),
						"WriteWhitespace"
					}));
				}
			}
			this.WriteString(ws);
		}

		public override void WriteXmlAttribute(string localName, string value)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Method {0} is not supported in JSON.", new object[] { "WriteXmlAttribute" })));
		}

		public override void WriteXmlAttribute(XmlDictionaryString localName, XmlDictionaryString value)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Method {0} is not supported in JSON.", new object[] { "WriteXmlAttribute" })));
		}

		public override void WriteXmlnsAttribute(string prefix, string namespaceUri)
		{
			if (!this.IsWritingNameWithMapping)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Method {0} is not supported in JSON.", new object[] { "WriteXmlnsAttribute" })));
			}
		}

		public override void WriteXmlnsAttribute(string prefix, XmlDictionaryString namespaceUri)
		{
			if (!this.IsWritingNameWithMapping)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Method {0} is not supported in JSON.", new object[] { "WriteXmlnsAttribute" })));
			}
		}

		internal static bool CharacterNeedsEscaping(char ch)
		{
			return ch == '/' || ch == '"' || ch < ' ' || ch == '\\' || (ch >= '\ud800' && (ch <= '\udfff' || ch >= '\ufffe'));
		}

		private static void ThrowClosed()
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("JSON writer is already closed.")));
		}

		private void CheckText(JsonNodeType nextNodeType)
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (this.depth == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Text cannot be written outside the root element.")));
			}
			if (nextNodeType == JsonNodeType.StandaloneText && this.nodeType == JsonNodeType.QuotedText)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON writer cannot write standalone text after quoted text.")));
			}
		}

		private void EnterScope(JsonNodeType currentNodeType)
		{
			this.depth++;
			if (this.scopes == null)
			{
				this.scopes = new JsonNodeType[4];
			}
			else if (this.scopes.Length == this.depth)
			{
				JsonNodeType[] array = new JsonNodeType[this.depth * 2];
				Array.Copy(this.scopes, array, this.depth);
				this.scopes = array;
			}
			this.scopes[this.depth] = currentNodeType;
		}

		private JsonNodeType ExitScope()
		{
			JsonNodeType jsonNodeType = this.scopes[this.depth];
			this.scopes[this.depth] = JsonNodeType.None;
			this.depth--;
			return jsonNodeType;
		}

		private void InitializeWriter()
		{
			this.nodeType = JsonNodeType.None;
			this.dataType = XmlJsonWriter.JsonDataType.None;
			this.isWritingDataTypeAttribute = false;
			this.wroteServerTypeAttribute = false;
			this.isWritingServerTypeAttribute = false;
			this.serverTypeValue = null;
			this.attributeText = null;
			if (this.depth != 0)
			{
				this.depth = 0;
			}
			if (this.scopes != null && this.scopes.Length > 25)
			{
				this.scopes = null;
			}
			this.writeState = WriteState.Start;
			this.endElementBuffer = false;
			this.indentLevel = 0;
		}

		private static bool IsUnicodeNewlineCharacter(char c)
		{
			return c == '\u0085' || c == '\u2028' || c == '\u2029';
		}

		private void StartText()
		{
			if (this.HasOpenAttribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("On JSON writer WriteString must be used for writing attribute values.")));
			}
			if (this.dataType == XmlJsonWriter.JsonDataType.None && this.serverTypeValue != null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("On JSON writer data type '{0}' must be specified. Object string is '{1}', server type string is '{2}'.", new object[] { "type", "object", "__type" })));
			}
			if (this.IsWritingNameWithMapping && !this.WrittenNameWithMapping)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("On JSON writer data type '{0}' must be specified. Object string is '{1}', server type string is '{2}'.", new object[]
				{
					"item",
					string.Empty,
					"item"
				})));
			}
			if (this.dataType == XmlJsonWriter.JsonDataType.String || this.dataType == XmlJsonWriter.JsonDataType.None)
			{
				this.CheckText(JsonNodeType.QuotedText);
				if (this.nodeType != JsonNodeType.QuotedText)
				{
					this.WriteJsonQuote();
				}
				this.nodeType = JsonNodeType.QuotedText;
				return;
			}
			if (this.dataType == XmlJsonWriter.JsonDataType.Number || this.dataType == XmlJsonWriter.JsonDataType.Boolean)
			{
				this.CheckText(JsonNodeType.StandaloneText);
				this.nodeType = JsonNodeType.StandaloneText;
				return;
			}
			this.ThrowInvalidAttributeContent();
		}

		private void ThrowIfServerTypeWritten(string dataTypeSpecified)
		{
			if (this.serverTypeValue != null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("The specified data type is invalid for server type. Type: '{0}', specified data type: '{1}', server type: '{2}', object '{3}'.", new object[] { "type", dataTypeSpecified, "__type", "object" })));
			}
		}

		private void ThrowInvalidAttributeContent()
		{
			if (this.HasOpenAttribute)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid method call state between start and end attribute.")));
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("JSON writer cannot write text after non-text attribute. Data type is '{0}'.", new object[] { this.dataType.ToString().ToLowerInvariant() })));
		}

		private bool TrySetWritingNameWithMapping(string localName, string ns)
		{
			if (localName.Equals("item") && ns.Equals("item"))
			{
				this.nameState = XmlJsonWriter.NameState.IsWritingNameWithMapping;
				return true;
			}
			return false;
		}

		private void WriteDataTypeServerType()
		{
			if (this.dataType != XmlJsonWriter.JsonDataType.None)
			{
				XmlJsonWriter.JsonDataType jsonDataType = this.dataType;
				if (jsonDataType != XmlJsonWriter.JsonDataType.Null)
				{
					if (jsonDataType != XmlJsonWriter.JsonDataType.Object)
					{
						if (jsonDataType == XmlJsonWriter.JsonDataType.Array)
						{
							this.EnterScope(JsonNodeType.Collection);
							this.nodeWriter.WriteText(91);
							this.indentLevel++;
						}
					}
					else
					{
						this.EnterScope(JsonNodeType.Object);
						this.nodeWriter.WriteText(123);
						this.indentLevel++;
					}
				}
				else
				{
					this.nodeWriter.WriteText("null");
				}
				if (this.serverTypeValue != null)
				{
					this.WriteServerTypeAttribute();
				}
			}
		}

		[SecuritySafeCritical]
		private unsafe void WriteEscapedJsonString(string str)
		{
			fixed (string text = str)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				int num = 0;
				int i;
				for (i = 0; i < str.Length; i++)
				{
					char c = ptr[i];
					if (c <= '/')
					{
						if (c == '/' || c == '"')
						{
							this.nodeWriter.WriteChars(ptr + num, i - num);
							this.nodeWriter.WriteText(92);
							this.nodeWriter.WriteText((int)c);
							num = i + 1;
						}
						else if (c < ' ')
						{
							this.nodeWriter.WriteChars(ptr + num, i - num);
							this.nodeWriter.WriteText(92);
							if (XmlJsonWriter.CharacterAbbrevs[(int)c] == '\0')
							{
								this.nodeWriter.WriteText(117);
								this.nodeWriter.WriteText(string.Format(CultureInfo.InvariantCulture, "{0:x4}", (int)c));
								num = i + 1;
							}
							else
							{
								this.nodeWriter.WriteText((int)XmlJsonWriter.CharacterAbbrevs[(int)c]);
								num = i + 1;
							}
						}
					}
					else if (c == '\\')
					{
						this.nodeWriter.WriteChars(ptr + num, i - num);
						this.nodeWriter.WriteText(92);
						this.nodeWriter.WriteText((int)c);
						num = i + 1;
					}
					else if ((c >= '\ud800' && (c <= '\udfff' || c >= '\ufffe')) || XmlJsonWriter.IsUnicodeNewlineCharacter(c))
					{
						this.nodeWriter.WriteChars(ptr + num, i - num);
						this.nodeWriter.WriteText(92);
						this.nodeWriter.WriteText(117);
						this.nodeWriter.WriteText(string.Format(CultureInfo.InvariantCulture, "{0:x4}", (int)c));
						num = i + 1;
					}
				}
				if (num < i)
				{
					this.nodeWriter.WriteChars(ptr + num, i - num);
				}
			}
		}

		private void WriteIndent()
		{
			for (int i = 0; i < this.indentLevel; i++)
			{
				this.nodeWriter.WriteText(this.indentChars);
			}
		}

		private void WriteNewLine()
		{
			this.nodeWriter.WriteText(13);
			this.nodeWriter.WriteText(10);
		}

		private void WriteJsonElementName(string localName)
		{
			this.WriteJsonQuote();
			this.WriteEscapedJsonString(localName);
			this.WriteJsonQuote();
			this.nodeWriter.WriteText(58);
			if (this.indent)
			{
				this.nodeWriter.WriteText(32);
			}
		}

		private void WriteJsonQuote()
		{
			this.nodeWriter.WriteText(34);
		}

		private void WritePrimitiveValue(object value)
		{
			if (this.IsClosed)
			{
				XmlJsonWriter.ThrowClosed();
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (value is ulong)
			{
				this.WriteValue((ulong)value);
				return;
			}
			if (value is string)
			{
				this.WriteValue((string)value);
				return;
			}
			if (value is int)
			{
				this.WriteValue((int)value);
				return;
			}
			if (value is long)
			{
				this.WriteValue((long)value);
				return;
			}
			if (value is bool)
			{
				this.WriteValue((bool)value);
				return;
			}
			if (value is double)
			{
				this.WriteValue((double)value);
				return;
			}
			if (value is DateTime)
			{
				this.WriteValue((DateTime)value);
				return;
			}
			if (value is float)
			{
				this.WriteValue((float)value);
				return;
			}
			if (value is decimal)
			{
				this.WriteValue((decimal)value);
				return;
			}
			if (value is XmlDictionaryString)
			{
				this.WriteValue((XmlDictionaryString)value);
				return;
			}
			if (value is UniqueId)
			{
				this.WriteValue((UniqueId)value);
				return;
			}
			if (value is Guid)
			{
				this.WriteValue((Guid)value);
				return;
			}
			if (value is TimeSpan)
			{
				this.WriteValue((TimeSpan)value);
				return;
			}
			if (value.GetType().IsArray)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Nested array is not supported in JSON: '{0}'"), "value"));
			}
			base.WriteValue(value);
		}

		private void WriteServerTypeAttribute()
		{
			string text = this.serverTypeValue;
			XmlJsonWriter.JsonDataType jsonDataType = this.dataType;
			XmlJsonWriter.NameState nameState = this.nameState;
			base.WriteStartElement("__type");
			this.WriteValue(text);
			this.WriteEndElement();
			this.dataType = jsonDataType;
			this.nameState = nameState;
			this.wroteServerTypeAttribute = true;
		}

		private void WriteValue(ulong value)
		{
			this.StartText();
			this.nodeWriter.WriteUInt64Text(value);
		}

		private void WriteValue(Array array)
		{
			XmlJsonWriter.JsonDataType jsonDataType = this.dataType;
			this.dataType = XmlJsonWriter.JsonDataType.String;
			this.StartText();
			for (int i = 0; i < array.Length; i++)
			{
				if (i != 0)
				{
					this.nodeWriter.WriteText(32);
				}
				this.WritePrimitiveValue(array.GetValue(i));
			}
			this.dataType = jsonDataType;
		}

		private const char BACK_SLASH = '\\';

		private const char FORWARD_SLASH = '/';

		private const char HIGH_SURROGATE_START = '\ud800';

		private const char LOW_SURROGATE_END = '\udfff';

		private const char MAX_CHAR = '\ufffe';

		private const char WHITESPACE = ' ';

		private const char CARRIAGE_RETURN = '\r';

		private const char NEWLINE = '\n';

		private const char BACKSPACE = '\b';

		private const char FORM_FEED = '\f';

		private const char HORIZONTAL_TABULATION = '\t';

		private const string xmlNamespace = "http://www.w3.org/XML/1998/namespace";

		private const string xmlnsNamespace = "http://www.w3.org/2000/xmlns/";

		[SecurityCritical]
		private static BinHexEncoding binHexEncoding;

		private static char[] CharacterAbbrevs;

		private string attributeText;

		private XmlJsonWriter.JsonDataType dataType;

		private int depth;

		private bool endElementBuffer;

		private bool isWritingDataTypeAttribute;

		private bool isWritingServerTypeAttribute;

		private bool isWritingXmlnsAttribute;

		private bool isWritingXmlnsAttributeDefaultNs;

		private XmlJsonWriter.NameState nameState;

		private JsonNodeType nodeType;

		private XmlJsonWriter.JsonNodeWriter nodeWriter;

		private JsonNodeType[] scopes;

		private string serverTypeValue;

		private WriteState writeState;

		private bool wroteServerTypeAttribute;

		private bool indent;

		private string indentChars;

		private int indentLevel;

		private enum JsonDataType
		{
			None,
			Null,
			Boolean,
			Number,
			String,
			Object,
			Array
		}

		[Flags]
		private enum NameState
		{
			None = 0,
			IsWritingNameWithMapping = 1,
			IsWritingNameAttribute = 2,
			WrittenNameWithMapping = 4
		}

		private class JsonNodeWriter : XmlUTF8NodeWriter
		{
			[SecurityCritical]
			internal unsafe void WriteChars(char* chars, int charCount)
			{
				base.UnsafeWriteUTF8Chars(chars, charCount);
			}
		}
	}
}
