using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.XPath;

namespace System.Xml
{
	internal class XmlMtomWriter : XmlDictionaryWriter, IXmlMtomWriterInitializer
	{
		public void SetOutput(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo, string boundary, string startUri, bool writeMessageHeaders, bool ownsStream)
		{
			if (encoding == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encoding");
			}
			if (maxSizeInBytes < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxSizeInBytes", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			this.maxSizeInBytes = maxSizeInBytes;
			this.encoding = encoding;
			this.isUTF8 = XmlMtomWriter.IsUTF8Encoding(encoding);
			this.Initialize(stream, startInfo, boundary, startUri, writeMessageHeaders, ownsStream);
		}

		private XmlDictionaryWriter Writer
		{
			get
			{
				if (!this.IsInitialized)
				{
					this.Initialize();
				}
				return this.writer;
			}
		}

		private bool IsInitialized
		{
			get
			{
				return this.initialContentTypeForRootPart == null;
			}
		}

		private void Initialize(Stream stream, string startInfo, string boundary, string startUri, bool writeMessageHeaders, bool ownsStream)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			if (startInfo == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("startInfo");
			}
			if (boundary == null)
			{
				boundary = XmlMtomWriter.GetBoundaryString();
			}
			if (startUri == null)
			{
				startUri = XmlMtomWriter.GenerateUriForMimePart(0);
			}
			if (!MailBnfHelper.IsValidMimeBoundary(boundary))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("MIME boundary is invalid: '{0}'.", new object[] { boundary }), "boundary"));
			}
			this.ownsStream = ownsStream;
			this.isClosed = false;
			this.depth = 0;
			this.totalSizeOfMimeParts = 0;
			this.sizeOfBufferedBinaryData = 0;
			this.binaryDataChunks = null;
			this.contentType = null;
			this.contentTypeStream = null;
			this.contentID = startUri;
			if (this.mimeParts != null)
			{
				this.mimeParts.Clear();
			}
			this.mimeWriter = new MimeWriter(stream, boundary);
			this.initialContentTypeForRootPart = XmlMtomWriter.GetContentTypeForRootMimePart(this.encoding, startInfo);
			if (writeMessageHeaders)
			{
				this.initialContentTypeForMimeMessage = XmlMtomWriter.GetContentTypeForMimeMessage(boundary, startUri, startInfo);
			}
		}

		private void Initialize()
		{
			if (this.isClosed)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("The XmlWriter is closed.")));
			}
			if (this.initialContentTypeForRootPart != null)
			{
				if (this.initialContentTypeForMimeMessage != null)
				{
					this.mimeWriter.StartPreface();
					this.mimeWriter.WriteHeader(MimeGlobals.MimeVersionHeader, MimeGlobals.DefaultVersion);
					this.mimeWriter.WriteHeader(MimeGlobals.ContentTypeHeader, this.initialContentTypeForMimeMessage);
					this.initialContentTypeForMimeMessage = null;
				}
				this.WriteMimeHeaders(this.contentID, this.initialContentTypeForRootPart, this.isUTF8 ? MimeGlobals.Encoding8bit : MimeGlobals.EncodingBinary);
				Stream contentStream = this.mimeWriter.GetContentStream();
				IXmlTextWriterInitializer xmlTextWriterInitializer = this.writer as IXmlTextWriterInitializer;
				if (xmlTextWriterInitializer == null)
				{
					this.writer = XmlDictionaryWriter.CreateTextWriter(contentStream, this.encoding, this.ownsStream);
				}
				else
				{
					xmlTextWriterInitializer.SetOutput(contentStream, this.encoding, this.ownsStream);
				}
				this.contentID = null;
				this.initialContentTypeForRootPart = null;
			}
		}

		private static string GetBoundaryString()
		{
			return XmlMtomWriter.MimeBoundaryGenerator.Next();
		}

		internal static bool IsUTF8Encoding(Encoding encoding)
		{
			return encoding.WebName == "utf-8";
		}

		private static string GetContentTypeForMimeMessage(string boundary, string startUri, string startInfo)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "{0}/{1};{2}=\"{3}\";{4}=\"{5}\"", new object[]
			{
				MtomGlobals.MediaType,
				MtomGlobals.MediaSubtype,
				MtomGlobals.TypeParam,
				MtomGlobals.XopType,
				MtomGlobals.BoundaryParam,
				boundary
			}));
			if (startUri != null && startUri.Length > 0)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ";{0}=\"<{1}>\"", MtomGlobals.StartParam, startUri);
			}
			if (startInfo != null && startInfo.Length > 0)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, ";{0}=\"{1}\"", MtomGlobals.StartInfoParam, startInfo);
			}
			return stringBuilder.ToString();
		}

		private static string GetContentTypeForRootMimePart(Encoding encoding, string startInfo)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0};{1}={2}", MtomGlobals.XopType, MtomGlobals.CharsetParam, XmlMtomWriter.CharSet(encoding));
			if (startInfo != null)
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0};{1}=\"{2}\"", text, MtomGlobals.TypeParam, startInfo);
			}
			return text;
		}

		private static string CharSet(Encoding enc)
		{
			string webName = enc.WebName;
			if (string.Compare(webName, Encoding.UTF8.WebName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return webName;
			}
			if (string.Compare(webName, Encoding.Unicode.WebName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return "utf-16LE";
			}
			if (string.Compare(webName, Encoding.BigEndianUnicode.WebName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return "utf-16BE";
			}
			return webName;
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.WriteBase64InlineIfPresent();
			this.ThrowIfElementIsXOPInclude(prefix, localName, ns);
			this.Writer.WriteStartElement(prefix, localName, ns);
			this.depth++;
		}

		public override void WriteStartElement(string prefix, XmlDictionaryString localName, XmlDictionaryString ns)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("localName");
			}
			this.WriteBase64InlineIfPresent();
			this.ThrowIfElementIsXOPInclude(prefix, localName.Value, (ns == null) ? null : ns.Value);
			this.Writer.WriteStartElement(prefix, localName, ns);
			this.depth++;
		}

		private void ThrowIfElementIsXOPInclude(string prefix, string localName, string ns)
		{
			if (ns == null)
			{
				XmlBaseWriter xmlBaseWriter = this.Writer as XmlBaseWriter;
				if (xmlBaseWriter != null)
				{
					ns = xmlBaseWriter.LookupNamespace(prefix);
				}
			}
			if (localName == MtomGlobals.XopIncludeLocalName && ns == MtomGlobals.XopIncludeNamespace)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM data must not contain xop:Include element. '{0}' element in '{1}' namespace.", new object[]
				{
					MtomGlobals.XopIncludeLocalName,
					MtomGlobals.XopIncludeNamespace
				})));
			}
		}

		public override void WriteEndElement()
		{
			this.WriteXOPInclude();
			this.Writer.WriteEndElement();
			this.depth--;
			this.WriteXOPBinaryParts();
		}

		public override void WriteFullEndElement()
		{
			this.WriteXOPInclude();
			this.Writer.WriteFullEndElement();
			this.depth--;
			this.WriteXOPBinaryParts();
		}

		public override void WriteValue(IStreamProvider value)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (this.Writer.WriteState == WriteState.Element)
			{
				if (this.binaryDataChunks == null)
				{
					this.binaryDataChunks = new List<MtomBinaryData>();
					this.contentID = XmlMtomWriter.GenerateUriForMimePart((this.mimeParts == null) ? 1 : (this.mimeParts.Count + 1));
				}
				this.binaryDataChunks.Add(new MtomBinaryData(value));
				return;
			}
			this.Writer.WriteValue(value);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			if (this.Writer.WriteState != WriteState.Element)
			{
				this.Writer.WriteBase64(buffer, index, count);
				return;
			}
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
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
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - index })));
			}
			if (this.binaryDataChunks == null)
			{
				this.binaryDataChunks = new List<MtomBinaryData>();
				this.contentID = XmlMtomWriter.GenerateUriForMimePart((this.mimeParts == null) ? 1 : (this.mimeParts.Count + 1));
			}
			int num = XmlMtomWriter.ValidateSizeOfMessage(this.maxSizeInBytes, 0, this.totalSizeOfMimeParts);
			num += XmlMtomWriter.ValidateSizeOfMessage(this.maxSizeInBytes, num, this.sizeOfBufferedBinaryData);
			num += XmlMtomWriter.ValidateSizeOfMessage(this.maxSizeInBytes, num, count);
			this.sizeOfBufferedBinaryData += count;
			this.binaryDataChunks.Add(new MtomBinaryData(buffer, index, count));
		}

		internal static int ValidateSizeOfMessage(int maxSize, int offset, int size)
		{
			if (size > maxSize - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM exceeded max size in bytes. The maximum size is {0}.", new object[] { maxSize })));
			}
			return size;
		}

		private void WriteBase64InlineIfPresent()
		{
			if (this.binaryDataChunks != null)
			{
				this.WriteBase64Inline();
			}
		}

		private void WriteBase64Inline()
		{
			foreach (MtomBinaryData mtomBinaryData in this.binaryDataChunks)
			{
				if (mtomBinaryData.type == MtomBinaryDataType.Provider)
				{
					this.Writer.WriteValue(mtomBinaryData.provider);
				}
				else
				{
					this.Writer.WriteBase64(mtomBinaryData.chunk, 0, mtomBinaryData.chunk.Length);
				}
			}
			this.sizeOfBufferedBinaryData = 0;
			this.binaryDataChunks = null;
			this.contentType = null;
			this.contentID = null;
		}

		private void WriteXOPInclude()
		{
			if (this.binaryDataChunks == null)
			{
				return;
			}
			bool flag = true;
			long num = 0L;
			foreach (MtomBinaryData mtomBinaryData in this.binaryDataChunks)
			{
				long length = mtomBinaryData.Length;
				if (length < 0L || length > 767L - num)
				{
					flag = false;
					break;
				}
				num += length;
			}
			if (flag)
			{
				this.WriteBase64Inline();
				return;
			}
			if (this.mimeParts == null)
			{
				this.mimeParts = new List<XmlMtomWriter.MimePart>();
			}
			XmlMtomWriter.MimePart mimePart = new XmlMtomWriter.MimePart(this.binaryDataChunks, this.contentID, this.contentType, MimeGlobals.EncodingBinary, this.sizeOfBufferedBinaryData, this.maxSizeInBytes);
			this.mimeParts.Add(mimePart);
			this.totalSizeOfMimeParts += XmlMtomWriter.ValidateSizeOfMessage(this.maxSizeInBytes, this.totalSizeOfMimeParts, mimePart.sizeInBytes);
			this.totalSizeOfMimeParts += XmlMtomWriter.ValidateSizeOfMessage(this.maxSizeInBytes, this.totalSizeOfMimeParts, this.mimeWriter.GetBoundarySize());
			this.Writer.WriteStartElement(MtomGlobals.XopIncludePrefix, MtomGlobals.XopIncludeLocalName, MtomGlobals.XopIncludeNamespace);
			this.Writer.WriteStartAttribute(MtomGlobals.XopIncludeHrefLocalName, MtomGlobals.XopIncludeHrefNamespace);
			this.Writer.WriteValue(string.Format(CultureInfo.InvariantCulture, "{0}{1}", MimeGlobals.ContentIDScheme, this.contentID));
			this.Writer.WriteEndAttribute();
			this.Writer.WriteEndElement();
			this.binaryDataChunks = null;
			this.sizeOfBufferedBinaryData = 0;
			this.contentType = null;
			this.contentID = null;
		}

		public static string GenerateUriForMimePart(int index)
		{
			return string.Format(CultureInfo.InvariantCulture, "http://tempuri.org/{0}/{1}", index, DateTime.Now.Ticks);
		}

		private void WriteXOPBinaryParts()
		{
			if (this.depth > 0 || this.mimeWriter.WriteState == MimeWriterState.Closed)
			{
				return;
			}
			if (this.Writer.WriteState != WriteState.Closed)
			{
				this.Writer.Flush();
			}
			if (this.mimeParts != null)
			{
				foreach (XmlMtomWriter.MimePart mimePart in this.mimeParts)
				{
					this.WriteMimeHeaders(mimePart.contentID, mimePart.contentType, mimePart.contentTransferEncoding);
					Stream contentStream = this.mimeWriter.GetContentStream();
					int num = 256;
					byte[] array = new byte[num];
					foreach (MtomBinaryData mtomBinaryData in mimePart.binaryData)
					{
						if (mtomBinaryData.type == MtomBinaryDataType.Provider)
						{
							Stream stream = mtomBinaryData.provider.GetStream();
							if (stream == null)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Stream returned by IStreamProvider cannot be null.")));
							}
							for (;;)
							{
								int num2 = stream.Read(array, 0, num);
								if (num2 <= 0)
								{
									break;
								}
								contentStream.Write(array, 0, num2);
								if (num < 65536 && num2 == num)
								{
									num *= 16;
									array = new byte[num];
								}
							}
							mtomBinaryData.provider.ReleaseStream(stream);
						}
						else
						{
							contentStream.Write(mtomBinaryData.chunk, 0, mtomBinaryData.chunk.Length);
						}
					}
				}
				this.mimeParts.Clear();
			}
			this.mimeWriter.Close();
		}

		private void WriteMimeHeaders(string contentID, string contentType, string contentTransferEncoding)
		{
			this.mimeWriter.StartPart();
			if (contentID != null)
			{
				this.mimeWriter.WriteHeader(MimeGlobals.ContentIDHeader, string.Format(CultureInfo.InvariantCulture, "<{0}>", contentID));
			}
			if (contentTransferEncoding != null)
			{
				this.mimeWriter.WriteHeader(MimeGlobals.ContentTransferEncodingHeader, contentTransferEncoding);
			}
			if (contentType != null)
			{
				this.mimeWriter.WriteHeader(MimeGlobals.ContentTypeHeader, contentType);
			}
		}

		public override void Close()
		{
			if (!this.isClosed)
			{
				this.isClosed = true;
				if (this.IsInitialized)
				{
					this.WriteXOPInclude();
					if (this.Writer.WriteState == WriteState.Element || this.Writer.WriteState == WriteState.Attribute || this.Writer.WriteState == WriteState.Content)
					{
						this.Writer.WriteEndDocument();
					}
					this.Writer.Flush();
					this.depth = 0;
					this.WriteXOPBinaryParts();
					this.Writer.Close();
				}
			}
		}

		private void CheckIfStartContentTypeAttribute(string localName, string ns)
		{
			if (localName != null && localName == MtomGlobals.MimeContentTypeLocalName && ns != null && (ns == MtomGlobals.MimeContentTypeNamespace200406 || ns == MtomGlobals.MimeContentTypeNamespace200505))
			{
				this.contentTypeStream = new MemoryStream();
				this.infosetWriter = this.Writer;
				this.writer = XmlDictionaryWriter.CreateBinaryWriter(this.contentTypeStream);
				this.Writer.WriteStartElement("Wrapper");
				this.Writer.WriteStartAttribute(localName, ns);
			}
		}

		private void CheckIfEndContentTypeAttribute()
		{
			if (this.contentTypeStream != null)
			{
				this.Writer.WriteEndAttribute();
				this.Writer.WriteEndElement();
				this.Writer.Flush();
				this.contentTypeStream.Position = 0L;
				XmlReader xmlReader = XmlDictionaryReader.CreateBinaryReader(this.contentTypeStream, null, XmlDictionaryReaderQuotas.Max, null, null);
				while (xmlReader.Read())
				{
					if (xmlReader.IsStartElement("Wrapper"))
					{
						this.contentType = xmlReader.GetAttribute(MtomGlobals.MimeContentTypeLocalName, MtomGlobals.MimeContentTypeNamespace200406);
						if (this.contentType == null)
						{
							this.contentType = xmlReader.GetAttribute(MtomGlobals.MimeContentTypeLocalName, MtomGlobals.MimeContentTypeNamespace200505);
							break;
						}
						break;
					}
				}
				this.writer = this.infosetWriter;
				this.infosetWriter = null;
				this.contentTypeStream = null;
				if (this.contentType != null)
				{
					this.Writer.WriteString(this.contentType);
				}
			}
		}

		public override void Flush()
		{
			if (this.IsInitialized)
			{
				this.Writer.Flush();
			}
		}

		public override string LookupPrefix(string ns)
		{
			return this.Writer.LookupPrefix(ns);
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return this.Writer.Settings;
			}
		}

		public override void WriteAttributes(XmlReader reader, bool defattr)
		{
			this.Writer.WriteAttributes(reader, defattr);
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteBinHex(buffer, index, count);
		}

		public override void WriteCData(string text)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteCData(text);
		}

		public override void WriteCharEntity(char ch)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteCharEntity(ch);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteChars(buffer, index, count);
		}

		public override void WriteComment(string text)
		{
			if (this.depth == 0 && this.mimeWriter.WriteState == MimeWriterState.Closed)
			{
				return;
			}
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteComment(text);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteEndAttribute()
		{
			this.CheckIfEndContentTypeAttribute();
			this.Writer.WriteEndAttribute();
		}

		public override void WriteEndDocument()
		{
			this.WriteXOPInclude();
			this.Writer.WriteEndDocument();
			this.depth = 0;
			this.WriteXOPBinaryParts();
		}

		public override void WriteEntityRef(string name)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteEntityRef(name);
		}

		public override void WriteName(string name)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteName(name);
		}

		public override void WriteNmToken(string name)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteNmToken(name);
		}

		protected override void WriteTextNode(XmlDictionaryReader reader, bool attribute)
		{
			Type valueType = reader.ValueType;
			if (valueType == typeof(string))
			{
				if (reader.CanReadValueChunk)
				{
					if (this.chars == null)
					{
						this.chars = new char[256];
					}
					int num;
					while ((num = reader.ReadValueChunk(this.chars, 0, this.chars.Length)) > 0)
					{
						this.WriteChars(this.chars, 0, num);
					}
				}
				else
				{
					this.WriteString(reader.Value);
				}
				if (!attribute)
				{
					reader.Read();
					return;
				}
			}
			else if (valueType == typeof(byte[]))
			{
				if (reader.CanReadBinaryContent)
				{
					if (this.bytes == null)
					{
						this.bytes = new byte[384];
					}
					int num2;
					while ((num2 = reader.ReadValueAsBase64(this.bytes, 0, this.bytes.Length)) > 0)
					{
						this.WriteBase64(this.bytes, 0, num2);
					}
				}
				else
				{
					this.WriteString(reader.Value);
				}
				if (!attribute)
				{
					reader.Read();
					return;
				}
			}
			else
			{
				base.WriteTextNode(reader, attribute);
			}
		}

		public override void WriteNode(XPathNavigator navigator, bool defattr)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteNode(navigator, defattr);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteProcessingInstruction(name, text);
		}

		public override void WriteQualifiedName(string localName, string namespaceUri)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteQualifiedName(localName, namespaceUri);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteRaw(data);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.Writer.WriteStartAttribute(prefix, localName, ns);
			this.CheckIfStartContentTypeAttribute(localName, ns);
		}

		public override void WriteStartAttribute(string prefix, XmlDictionaryString localName, XmlDictionaryString ns)
		{
			this.Writer.WriteStartAttribute(prefix, localName, ns);
			if (localName != null && ns != null)
			{
				this.CheckIfStartContentTypeAttribute(localName.Value, ns.Value);
			}
		}

		public override void WriteStartDocument()
		{
			this.Writer.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.Writer.WriteStartDocument(standalone);
		}

		public override WriteState WriteState
		{
			get
			{
				return this.Writer.WriteState;
			}
		}

		public override void WriteString(string text)
		{
			if (this.depth == 0 && this.mimeWriter.WriteState == MimeWriterState.Closed && XmlConverter.IsWhitespace(text))
			{
				return;
			}
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteString(text);
		}

		public override void WriteString(XmlDictionaryString value)
		{
			if (this.depth == 0 && this.mimeWriter.WriteState == MimeWriterState.Closed && XmlConverter.IsWhitespace(value.Value))
			{
				return;
			}
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteString(value);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteWhitespace(string whitespace)
		{
			if (this.depth == 0 && this.mimeWriter.WriteState == MimeWriterState.Closed)
			{
				return;
			}
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteWhitespace(whitespace);
		}

		public override void WriteValue(object value)
		{
			IStreamProvider streamProvider = value as IStreamProvider;
			if (streamProvider != null)
			{
				this.WriteValue(streamProvider);
				return;
			}
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(string value)
		{
			if (this.depth == 0 && this.mimeWriter.WriteState == MimeWriterState.Closed && XmlConverter.IsWhitespace(value))
			{
				return;
			}
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(bool value)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(DateTime value)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(double value)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(int value)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(long value)
		{
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(XmlDictionaryString value)
		{
			if (this.depth == 0 && this.mimeWriter.WriteState == MimeWriterState.Closed && XmlConverter.IsWhitespace(value.Value))
			{
				return;
			}
			this.WriteBase64InlineIfPresent();
			this.Writer.WriteValue(value);
		}

		public override void WriteXmlnsAttribute(string prefix, string ns)
		{
			this.Writer.WriteXmlnsAttribute(prefix, ns);
		}

		public override void WriteXmlnsAttribute(string prefix, XmlDictionaryString ns)
		{
			this.Writer.WriteXmlnsAttribute(prefix, ns);
		}

		public override string XmlLang
		{
			get
			{
				return this.Writer.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.Writer.XmlSpace;
			}
		}

		private const int MaxInlinedBytes = 767;

		private int maxSizeInBytes;

		private XmlDictionaryWriter writer;

		private XmlDictionaryWriter infosetWriter;

		private MimeWriter mimeWriter;

		private Encoding encoding;

		private bool isUTF8;

		private string contentID;

		private string contentType;

		private string initialContentTypeForRootPart;

		private string initialContentTypeForMimeMessage;

		private MemoryStream contentTypeStream;

		private List<XmlMtomWriter.MimePart> mimeParts;

		private IList<MtomBinaryData> binaryDataChunks;

		private int depth;

		private int totalSizeOfMimeParts;

		private int sizeOfBufferedBinaryData;

		private char[] chars;

		private byte[] bytes;

		private bool isClosed;

		private bool ownsStream;

		private static class MimeBoundaryGenerator
		{
			internal static string Next()
			{
				long num = Interlocked.Increment(ref XmlMtomWriter.MimeBoundaryGenerator.id);
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}", XmlMtomWriter.MimeBoundaryGenerator.prefix, num);
			}

			private static long id;

			private static string prefix = Guid.NewGuid().ToString() + "+id=";
		}

		private class MimePart
		{
			internal MimePart(IList<MtomBinaryData> binaryData, string contentID, string contentType, string contentTransferEncoding, int sizeOfBufferedBinaryData, int maxSizeInBytes)
			{
				this.binaryData = binaryData;
				this.contentID = contentID;
				this.contentType = contentType ?? MtomGlobals.DefaultContentTypeForBinary;
				this.contentTransferEncoding = contentTransferEncoding;
				this.sizeInBytes = XmlMtomWriter.MimePart.GetSize(contentID, contentType, contentTransferEncoding, sizeOfBufferedBinaryData, maxSizeInBytes);
			}

			private static int GetSize(string contentID, string contentType, string contentTransferEncoding, int sizeOfBufferedBinaryData, int maxSizeInBytes)
			{
				int num = XmlMtomWriter.ValidateSizeOfMessage(maxSizeInBytes, 0, MimeGlobals.CRLF.Length * 3);
				if (contentTransferEncoding != null)
				{
					num += XmlMtomWriter.ValidateSizeOfMessage(maxSizeInBytes, num, MimeWriter.GetHeaderSize(MimeGlobals.ContentTransferEncodingHeader, contentTransferEncoding, maxSizeInBytes));
				}
				if (contentType != null)
				{
					num += XmlMtomWriter.ValidateSizeOfMessage(maxSizeInBytes, num, MimeWriter.GetHeaderSize(MimeGlobals.ContentTypeHeader, contentType, maxSizeInBytes));
				}
				if (contentID != null)
				{
					num += XmlMtomWriter.ValidateSizeOfMessage(maxSizeInBytes, num, MimeWriter.GetHeaderSize(MimeGlobals.ContentIDHeader, contentID, maxSizeInBytes));
					num += XmlMtomWriter.ValidateSizeOfMessage(maxSizeInBytes, num, 2);
				}
				return num + XmlMtomWriter.ValidateSizeOfMessage(maxSizeInBytes, num, sizeOfBufferedBinaryData);
			}

			internal IList<MtomBinaryData> binaryData;

			internal string contentID;

			internal string contentType;

			internal string contentTransferEncoding;

			internal int sizeInBytes;
		}
	}
}
