using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	internal class XmlMtomReader : XmlDictionaryReader, IXmlLineInfo, IXmlMtomReaderInitializer
	{
		internal static void DecrementBufferQuota(int maxBuffer, ref int remaining, int size)
		{
			if (remaining - size <= 0)
			{
				remaining = 0;
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM buffer quota exceeded. The maximum size is {0}.", new object[] { maxBuffer })));
			}
			remaining -= size;
		}

		private void SetReadEncodings(Encoding[] encodings)
		{
			if (encodings == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encodings");
			}
			for (int i = 0; i < encodings.Length; i++)
			{
				if (encodings[i] == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(string.Format(CultureInfo.InvariantCulture, "encodings[{0}]", i));
				}
			}
			this.encodings = new Encoding[encodings.Length];
			encodings.CopyTo(this.encodings, 0);
		}

		private void CheckContentType(string contentType)
		{
			if (contentType != null && contentType.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("MTOM content type is invalid."), "contentType"));
			}
		}

		public void SetInput(byte[] buffer, int offset, int count, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize, OnXmlDictionaryReaderClose onClose)
		{
			this.SetInput(new MemoryStream(buffer, offset, count), encodings, contentType, quotas, maxBufferSize, onClose);
		}

		public void SetInput(Stream stream, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize, OnXmlDictionaryReaderClose onClose)
		{
			this.SetReadEncodings(encodings);
			this.CheckContentType(contentType);
			this.Initialize(stream, contentType, quotas, maxBufferSize);
			this.onClose = onClose;
		}

		private void Initialize(Stream stream, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			this.maxBufferSize = maxBufferSize;
			this.bufferRemaining = maxBufferSize;
			string text;
			string text2;
			string text3;
			if (contentType == null)
			{
				MimeMessageReader mimeMessageReader = new MimeMessageReader(stream);
				MimeHeaders mimeHeaders = mimeMessageReader.ReadHeaders(this.maxBufferSize, ref this.bufferRemaining);
				this.ReadMessageMimeVersionHeader(mimeHeaders.MimeVersion);
				this.ReadMessageContentTypeHeader(mimeHeaders.ContentType, out text, out text2, out text3);
				stream = mimeMessageReader.GetContentStream();
				if (stream == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM message content is invalid.")));
				}
			}
			else
			{
				this.ReadMessageContentTypeHeader(new ContentTypeHeader(contentType), out text, out text2, out text3);
			}
			this.mimeReader = new MimeReader(stream, text);
			this.mimeParts = null;
			this.readingBinaryElement = false;
			XmlMtomReader.MimePart mimePart = ((text2 == null) ? this.ReadRootMimePart() : this.ReadMimePart(this.GetStartUri(text2)));
			byte[] buffer = mimePart.GetBuffer(this.maxBufferSize, ref this.bufferRemaining);
			int num = (int)mimePart.Length;
			Encoding encoding = this.ReadRootContentTypeHeader(mimePart.Headers.ContentType, this.encodings, text3);
			this.CheckContentTransferEncodingOnRoot(mimePart.Headers.ContentTransferEncoding);
			IXmlTextReaderInitializer xmlTextReaderInitializer = this.xmlReader as IXmlTextReaderInitializer;
			if (xmlTextReaderInitializer != null)
			{
				xmlTextReaderInitializer.SetInput(buffer, 0, num, encoding, quotas, null);
				return;
			}
			this.xmlReader = XmlDictionaryReader.CreateTextReader(buffer, 0, num, encoding, quotas, null);
		}

		public override XmlDictionaryReaderQuotas Quotas
		{
			get
			{
				return this.xmlReader.Quotas;
			}
		}

		private void ReadMessageMimeVersionHeader(MimeVersionHeader header)
		{
			if (header != null && header.Version != MimeVersionHeader.Default.Version)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM message has invalid MIME version. Expected '{1}', got '{0}' instead.", new object[]
				{
					header.Version,
					MimeVersionHeader.Default.Version
				})));
			}
		}

		private void ReadMessageContentTypeHeader(ContentTypeHeader header, out string boundary, out string start, out string startInfo)
		{
			if (header == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM message content type was not found.")));
			}
			if (string.Compare(MtomGlobals.MediaType, header.MediaType, StringComparison.OrdinalIgnoreCase) != 0 || string.Compare(MtomGlobals.MediaSubtype, header.MediaSubtype, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM message is not multipart: media type should be '{0}', media subtype should be '{1}'.", new object[]
				{
					MtomGlobals.MediaType,
					MtomGlobals.MediaSubtype
				})));
			}
			string text;
			if (!header.Parameters.TryGetValue(MtomGlobals.TypeParam, out text) || MtomGlobals.XopType != text)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM msssage type is not '{0}'.", new object[] { MtomGlobals.XopType })));
			}
			if (!header.Parameters.TryGetValue(MtomGlobals.BoundaryParam, out boundary))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Required MTOM parameter '{0}' is not specified.", new object[] { MtomGlobals.BoundaryParam })));
			}
			if (!MailBnfHelper.IsValidMimeBoundary(boundary))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MIME boundary is invalid: '{0}'.", new object[] { boundary })));
			}
			if (!header.Parameters.TryGetValue(MtomGlobals.StartParam, out start))
			{
				start = null;
			}
			if (!header.Parameters.TryGetValue(MtomGlobals.StartInfoParam, out startInfo))
			{
				startInfo = null;
			}
		}

		private Encoding ReadRootContentTypeHeader(ContentTypeHeader header, Encoding[] expectedEncodings, string expectedType)
		{
			if (header == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM root content type is not found.")));
			}
			if (string.Compare(MtomGlobals.XopMediaType, header.MediaType, StringComparison.OrdinalIgnoreCase) != 0 || string.Compare(MtomGlobals.XopMediaSubtype, header.MediaSubtype, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM root should have media type '{0}' and subtype '{1}'.", new object[]
				{
					MtomGlobals.XopMediaType,
					MtomGlobals.XopMediaSubtype
				})));
			}
			string text;
			if (!header.Parameters.TryGetValue(MtomGlobals.CharsetParam, out text) || text == null || text.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Required MTOM root parameter '{0}' is not specified.", new object[] { MtomGlobals.CharsetParam })));
			}
			Encoding encoding = null;
			for (int i = 0; i < this.encodings.Length; i++)
			{
				if (string.Compare(text, expectedEncodings[i].WebName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					encoding = expectedEncodings[i];
					break;
				}
			}
			if (encoding == null)
			{
				if (string.Compare(text, "utf-16LE", StringComparison.OrdinalIgnoreCase) == 0)
				{
					for (int j = 0; j < this.encodings.Length; j++)
					{
						if (string.Compare(expectedEncodings[j].WebName, Encoding.Unicode.WebName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							encoding = expectedEncodings[j];
							break;
						}
					}
				}
				else if (string.Compare(text, "utf-16BE", StringComparison.OrdinalIgnoreCase) == 0)
				{
					for (int k = 0; k < this.encodings.Length; k++)
					{
						if (string.Compare(expectedEncodings[k].WebName, Encoding.BigEndianUnicode.WebName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							encoding = expectedEncodings[k];
							break;
						}
					}
				}
				if (encoding == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int l = 0; l < this.encodings.Length; l++)
					{
						if (stringBuilder.Length != 0)
						{
							stringBuilder.Append(" | ");
						}
						stringBuilder.Append(this.encodings[l].WebName);
					}
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Unexpected charset on MTOM root. Expected '{1}', got '{0}' instead.", new object[]
					{
						text,
						stringBuilder.ToString()
					})));
				}
			}
			if (expectedType != null)
			{
				string text2;
				if (!header.Parameters.TryGetValue(MtomGlobals.TypeParam, out text2) || text2 == null || text2.Length == 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Required MTOM root parameter '{0}' is not specified.", new object[] { MtomGlobals.TypeParam })));
				}
				if (text2 != expectedType)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Unexpected type on MTOM root. Expected '{1}', got '{0}' instead.", new object[] { text2, expectedType })));
				}
			}
			return encoding;
		}

		private void CheckContentTransferEncodingOnRoot(ContentTransferEncodingHeader header)
		{
			if (header != null && header.ContentTransferEncoding == ContentTransferEncoding.Other)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM content transfer encoding value is not supported. Raw value is '{0}', '{1}' in 7bit encoding, '{2}' in 8bit encoding, and '{3}' in binary.", new object[]
				{
					header.Value,
					ContentTransferEncodingHeader.SevenBit.ContentTransferEncodingValue,
					ContentTransferEncodingHeader.EightBit.ContentTransferEncodingValue,
					ContentTransferEncodingHeader.Binary.ContentTransferEncodingValue
				})));
			}
		}

		private void CheckContentTransferEncodingOnBinaryPart(ContentTransferEncodingHeader header)
		{
			if (header == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM content transfer encoding is not present. ContentTransferEncoding header is '{0}'.", new object[] { ContentTransferEncodingHeader.Binary.ContentTransferEncodingValue })));
			}
			if (header.ContentTransferEncoding != ContentTransferEncoding.Binary)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid transfer encoding for MIME part: '{0}', in binary: '{1}'.", new object[]
				{
					header.Value,
					ContentTransferEncodingHeader.Binary.ContentTransferEncodingValue
				})));
			}
		}

		private string GetStartUri(string startUri)
		{
			if (!startUri.StartsWith("<", StringComparison.Ordinal))
			{
				return string.Format(CultureInfo.InvariantCulture, "<{0}>", startUri);
			}
			if (startUri.EndsWith(">", StringComparison.Ordinal))
			{
				return startUri;
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid MTOM start URI: '{0}'.", new object[] { startUri })));
		}

		public override bool Read()
		{
			bool flag = this.xmlReader.Read();
			if (this.xmlReader.NodeType == XmlNodeType.Element)
			{
				XmlMtomReader.XopIncludeReader xopIncludeReader = null;
				if (this.xmlReader.IsStartElement(MtomGlobals.XopIncludeLocalName, MtomGlobals.XopIncludeNamespace))
				{
					string text = null;
					while (this.xmlReader.MoveToNextAttribute())
					{
						if (this.xmlReader.LocalName == MtomGlobals.XopIncludeHrefLocalName && this.xmlReader.NamespaceURI == MtomGlobals.XopIncludeHrefNamespace)
						{
							text = this.xmlReader.Value;
						}
						else if (this.xmlReader.NamespaceURI == MtomGlobals.XopIncludeNamespace)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("xop Include element has invalid attribute: '{0}' in '{1}' namespace.", new object[]
							{
								this.xmlReader.LocalName,
								MtomGlobals.XopIncludeNamespace
							})));
						}
					}
					if (text == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("xop Include element did not specify '{0}' attribute.", new object[] { MtomGlobals.XopIncludeHrefLocalName })));
					}
					XmlMtomReader.MimePart mimePart = this.ReadMimePart(text);
					this.CheckContentTransferEncodingOnBinaryPart(mimePart.Headers.ContentTransferEncoding);
					this.part = mimePart;
					xopIncludeReader = new XmlMtomReader.XopIncludeReader(mimePart, this.xmlReader);
					xopIncludeReader.Read();
					this.xmlReader.MoveToElement();
					if (this.xmlReader.IsEmptyElement)
					{
						this.xmlReader.Read();
					}
					else
					{
						int depth = this.xmlReader.Depth;
						this.xmlReader.ReadStartElement();
						while (this.xmlReader.Depth > depth)
						{
							if (this.xmlReader.IsStartElement() && this.xmlReader.NamespaceURI == MtomGlobals.XopIncludeNamespace)
							{
								throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("xop Include element has invalid element: '{0}' in '{1}' namespace.", new object[]
								{
									this.xmlReader.LocalName,
									MtomGlobals.XopIncludeNamespace
								})));
							}
							this.xmlReader.Skip();
						}
						this.xmlReader.ReadEndElement();
					}
				}
				if (xopIncludeReader != null)
				{
					this.xmlReader.MoveToContent();
					this.infosetReader = this.xmlReader;
					this.xmlReader = xopIncludeReader;
				}
			}
			if (this.xmlReader.ReadState == ReadState.EndOfFile && this.infosetReader != null)
			{
				if (!flag)
				{
					flag = this.infosetReader.Read();
				}
				this.part.Release(this.maxBufferSize, ref this.bufferRemaining);
				this.xmlReader = this.infosetReader;
				this.infosetReader = null;
			}
			return flag;
		}

		private XmlMtomReader.MimePart ReadMimePart(string uri)
		{
			XmlMtomReader.MimePart mimePart = null;
			if (uri == null || uri.Length == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("empty URI is invalid for MTOM MIME part.")));
			}
			string text = null;
			if (uri.StartsWith(MimeGlobals.ContentIDScheme, StringComparison.Ordinal))
			{
				text = string.Format(CultureInfo.InvariantCulture, "<{0}>", Uri.UnescapeDataString(uri.Substring(MimeGlobals.ContentIDScheme.Length)));
			}
			else if (uri.StartsWith("<", StringComparison.Ordinal))
			{
				text = uri;
			}
			if (text == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Invalid MTOM CID URI: '{0}'.", new object[] { uri })));
			}
			if (this.mimeParts != null && this.mimeParts.TryGetValue(text, out mimePart))
			{
				if (mimePart.ReferencedFromInfoset)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("Specified MIME part '{0}' is referenced more than once.", new object[] { text })));
				}
			}
			else
			{
				int maxMimeParts = AppSettings.MaxMimeParts;
				while (mimePart == null && this.mimeReader.ReadNextPart())
				{
					MimeHeaders mimeHeaders = this.mimeReader.ReadHeaders(this.maxBufferSize, ref this.bufferRemaining);
					Stream contentStream = this.mimeReader.GetContentStream();
					if (contentStream == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM message content in MIME part is invalid.")));
					}
					ContentIDHeader contentIDHeader = ((mimeHeaders == null) ? null : mimeHeaders.ContentID);
					if (contentIDHeader == null || contentIDHeader.Value == null)
					{
						int num = 256;
						byte[] array = new byte[num];
						int num2;
						do
						{
							num2 = contentStream.Read(array, 0, num);
						}
						while (num2 > 0);
					}
					else
					{
						string value = mimeHeaders.ContentID.Value;
						XmlMtomReader.MimePart mimePart2 = new XmlMtomReader.MimePart(contentStream, mimeHeaders);
						if (this.mimeParts == null)
						{
							this.mimeParts = new Dictionary<string, XmlMtomReader.MimePart>();
						}
						this.mimeParts.Add(value, mimePart2);
						if (this.mimeParts.Count > maxMimeParts)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MIME parts number exceeded the maximum settings. Must be less than {0}. Specified as '{1}'.", new object[] { maxMimeParts, "microsoft:xmldictionaryreader:maxmimeparts" })));
						}
						if (value.Equals(text))
						{
							mimePart = mimePart2;
						}
						else
						{
							mimePart2.GetBuffer(this.maxBufferSize, ref this.bufferRemaining);
						}
					}
				}
				if (mimePart == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM part with URI '{0}' is not found.", new object[] { uri })));
				}
			}
			mimePart.ReferencedFromInfoset = true;
			return mimePart;
		}

		private XmlMtomReader.MimePart ReadRootMimePart()
		{
			if (!this.mimeReader.ReadNextPart())
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM root part is not found.")));
			}
			MimeHeaders mimeHeaders = this.mimeReader.ReadHeaders(this.maxBufferSize, ref this.bufferRemaining);
			Stream contentStream = this.mimeReader.GetContentStream();
			if (contentStream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(global::System.Runtime.Serialization.SR.GetString("MTOM message content in MIME part is invalid.")));
			}
			return new XmlMtomReader.MimePart(contentStream, mimeHeaders);
		}

		private void AdvanceToContentOnElement()
		{
			if (this.NodeType != XmlNodeType.Attribute)
			{
				this.MoveToContent();
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.xmlReader.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.xmlReader.BaseURI;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return this.xmlReader.CanReadBinaryContent;
			}
		}

		public override bool CanReadValueChunk
		{
			get
			{
				return this.xmlReader.CanReadValueChunk;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return this.xmlReader.CanResolveEntity;
			}
		}

		public override void Close()
		{
			this.xmlReader.Close();
			this.mimeReader.Close();
			OnXmlDictionaryReaderClose onXmlDictionaryReaderClose = this.onClose;
			this.onClose = null;
			if (onXmlDictionaryReaderClose != null)
			{
				try
				{
					onXmlDictionaryReaderClose(this);
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(ex);
				}
			}
		}

		public override int Depth
		{
			get
			{
				return this.xmlReader.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.xmlReader.EOF;
			}
		}

		public override string GetAttribute(int index)
		{
			return this.xmlReader.GetAttribute(index);
		}

		public override string GetAttribute(string name)
		{
			return this.xmlReader.GetAttribute(name);
		}

		public override string GetAttribute(string name, string ns)
		{
			return this.xmlReader.GetAttribute(name, ns);
		}

		public override string GetAttribute(XmlDictionaryString localName, XmlDictionaryString ns)
		{
			return this.xmlReader.GetAttribute(localName, ns);
		}

		public override bool HasAttributes
		{
			get
			{
				return this.xmlReader.HasAttributes;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.xmlReader.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.xmlReader.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.xmlReader.IsEmptyElement;
			}
		}

		public override bool IsLocalName(string localName)
		{
			return this.xmlReader.IsLocalName(localName);
		}

		public override bool IsLocalName(XmlDictionaryString localName)
		{
			return this.xmlReader.IsLocalName(localName);
		}

		public override bool IsNamespaceUri(string ns)
		{
			return this.xmlReader.IsNamespaceUri(ns);
		}

		public override bool IsNamespaceUri(XmlDictionaryString ns)
		{
			return this.xmlReader.IsNamespaceUri(ns);
		}

		public override bool IsStartElement()
		{
			return this.xmlReader.IsStartElement();
		}

		public override bool IsStartElement(string localName)
		{
			return this.xmlReader.IsStartElement(localName);
		}

		public override bool IsStartElement(string localName, string ns)
		{
			return this.xmlReader.IsStartElement(localName, ns);
		}

		public override bool IsStartElement(XmlDictionaryString localName, XmlDictionaryString ns)
		{
			return this.xmlReader.IsStartElement(localName, ns);
		}

		public override string LocalName
		{
			get
			{
				return this.xmlReader.LocalName;
			}
		}

		public override string LookupNamespace(string ns)
		{
			return this.xmlReader.LookupNamespace(ns);
		}

		public override void MoveToAttribute(int index)
		{
			this.xmlReader.MoveToAttribute(index);
		}

		public override bool MoveToAttribute(string name)
		{
			return this.xmlReader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return this.xmlReader.MoveToAttribute(name, ns);
		}

		public override bool MoveToElement()
		{
			return this.xmlReader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return this.xmlReader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this.xmlReader.MoveToNextAttribute();
		}

		public override string Name
		{
			get
			{
				return this.xmlReader.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.xmlReader.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.xmlReader.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.xmlReader.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.xmlReader.Prefix;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.xmlReader.QuoteChar;
			}
		}

		public override bool ReadAttributeValue()
		{
			return this.xmlReader.ReadAttributeValue();
		}

		public override object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAs(returnType, namespaceResolver);
		}

		public override byte[] ReadContentAsBase64()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsBase64();
		}

		public override int ReadValueAsBase64(byte[] buffer, int offset, int count)
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadValueAsBase64(buffer, offset, count);
		}

		public override int ReadContentAsBase64(byte[] buffer, int offset, int count)
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsBase64(buffer, offset, count);
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int offset, int count)
		{
			if (!this.readingBinaryElement)
			{
				if (this.IsEmptyElement)
				{
					this.Read();
					return 0;
				}
				this.ReadStartElement();
				this.readingBinaryElement = true;
			}
			int num = this.ReadContentAsBase64(buffer, offset, count);
			if (num == 0)
			{
				this.ReadEndElement();
				this.readingBinaryElement = false;
			}
			return num;
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int offset, int count)
		{
			if (!this.readingBinaryElement)
			{
				if (this.IsEmptyElement)
				{
					this.Read();
					return 0;
				}
				this.ReadStartElement();
				this.readingBinaryElement = true;
			}
			int num = this.ReadContentAsBinHex(buffer, offset, count);
			if (num == 0)
			{
				this.ReadEndElement();
				this.readingBinaryElement = false;
			}
			return num;
		}

		public override int ReadContentAsBinHex(byte[] buffer, int offset, int count)
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsBinHex(buffer, offset, count);
		}

		public override bool ReadContentAsBoolean()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsBoolean();
		}

		public override int ReadContentAsChars(char[] chars, int index, int count)
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsChars(chars, index, count);
		}

		public override DateTime ReadContentAsDateTime()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsDateTime();
		}

		public override decimal ReadContentAsDecimal()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsDecimal();
		}

		public override double ReadContentAsDouble()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsDouble();
		}

		public override int ReadContentAsInt()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsInt();
		}

		public override long ReadContentAsLong()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsLong();
		}

		public override object ReadContentAsObject()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsObject();
		}

		public override float ReadContentAsFloat()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsFloat();
		}

		public override string ReadContentAsString()
		{
			this.AdvanceToContentOnElement();
			return this.xmlReader.ReadContentAsString();
		}

		public override string ReadInnerXml()
		{
			return this.xmlReader.ReadInnerXml();
		}

		public override string ReadOuterXml()
		{
			return this.xmlReader.ReadOuterXml();
		}

		public override ReadState ReadState
		{
			get
			{
				if (this.xmlReader.ReadState != ReadState.Interactive && this.infosetReader != null)
				{
					return this.infosetReader.ReadState;
				}
				return this.xmlReader.ReadState;
			}
		}

		public override int ReadValueChunk(char[] buffer, int index, int count)
		{
			return this.xmlReader.ReadValueChunk(buffer, index, count);
		}

		public override void ResolveEntity()
		{
			this.xmlReader.ResolveEntity();
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				return this.xmlReader.Settings;
			}
		}

		public override void Skip()
		{
			this.xmlReader.Skip();
		}

		public override string this[int index]
		{
			get
			{
				return this.xmlReader[index];
			}
		}

		public override string this[string name]
		{
			get
			{
				return this.xmlReader[name];
			}
		}

		public override string this[string name, string ns]
		{
			get
			{
				return this.xmlReader[name, ns];
			}
		}

		public override string Value
		{
			get
			{
				return this.xmlReader.Value;
			}
		}

		public override Type ValueType
		{
			get
			{
				return this.xmlReader.ValueType;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.xmlReader.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.xmlReader.XmlSpace;
			}
		}

		public bool HasLineInfo()
		{
			if (this.xmlReader.ReadState == ReadState.Closed)
			{
				return false;
			}
			IXmlLineInfo xmlLineInfo = this.xmlReader as IXmlLineInfo;
			return xmlLineInfo != null && xmlLineInfo.HasLineInfo();
		}

		public int LineNumber
		{
			get
			{
				if (this.xmlReader.ReadState == ReadState.Closed)
				{
					return 0;
				}
				IXmlLineInfo xmlLineInfo = this.xmlReader as IXmlLineInfo;
				if (xmlLineInfo == null)
				{
					return 0;
				}
				return xmlLineInfo.LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				if (this.xmlReader.ReadState == ReadState.Closed)
				{
					return 0;
				}
				IXmlLineInfo xmlLineInfo = this.xmlReader as IXmlLineInfo;
				if (xmlLineInfo == null)
				{
					return 0;
				}
				return xmlLineInfo.LinePosition;
			}
		}

		private Encoding[] encodings;

		private XmlDictionaryReader xmlReader;

		private XmlDictionaryReader infosetReader;

		private MimeReader mimeReader;

		private Dictionary<string, XmlMtomReader.MimePart> mimeParts;

		private OnXmlDictionaryReaderClose onClose;

		private bool readingBinaryElement;

		private int maxBufferSize;

		private int bufferRemaining;

		private XmlMtomReader.MimePart part;

		internal class MimePart
		{
			internal MimePart(Stream stream, MimeHeaders headers)
			{
				this.stream = stream;
				this.headers = headers;
			}

			internal Stream Stream
			{
				get
				{
					return this.stream;
				}
			}

			internal MimeHeaders Headers
			{
				get
				{
					return this.headers;
				}
			}

			internal bool ReferencedFromInfoset
			{
				get
				{
					return this.isReferencedFromInfoset;
				}
				set
				{
					this.isReferencedFromInfoset = value;
				}
			}

			internal long Length
			{
				get
				{
					if (!this.stream.CanSeek)
					{
						return 0L;
					}
					return this.stream.Length;
				}
			}

			internal byte[] GetBuffer(int maxBuffer, ref int remaining)
			{
				if (this.buffer == null)
				{
					MemoryStream memoryStream = (this.stream.CanSeek ? new MemoryStream((int)this.stream.Length) : new MemoryStream());
					int num = 256;
					byte[] array = new byte[num];
					int num2;
					do
					{
						num2 = this.stream.Read(array, 0, num);
						XmlMtomReader.DecrementBufferQuota(maxBuffer, ref remaining, num2);
						if (num2 > 0)
						{
							memoryStream.Write(array, 0, num2);
						}
					}
					while (num2 > 0);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					this.buffer = memoryStream.GetBuffer();
					this.stream = memoryStream;
				}
				return this.buffer;
			}

			internal void Release(int maxBuffer, ref int remaining)
			{
				remaining += (int)this.Length;
				this.headers.Release(ref remaining);
			}

			private Stream stream;

			private MimeHeaders headers;

			private byte[] buffer;

			private bool isReferencedFromInfoset;
		}

		internal class XopIncludeReader : XmlDictionaryReader, IXmlLineInfo
		{
			public XopIncludeReader(XmlMtomReader.MimePart part, XmlDictionaryReader reader)
			{
				if (part == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("part");
				}
				if (reader == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
				}
				this.part = part;
				this.parentReader = reader;
				this.readState = ReadState.Initial;
				this.nodeType = XmlNodeType.None;
				this.chunkSize = Math.Min(reader.Quotas.MaxBytesPerRead, this.chunkSize);
				this.bytesRemaining = this.chunkSize;
				this.finishedStream = false;
			}

			public override XmlDictionaryReaderQuotas Quotas
			{
				get
				{
					return this.parentReader.Quotas;
				}
			}

			public override XmlNodeType NodeType
			{
				get
				{
					if (this.readState != ReadState.Interactive)
					{
						return this.parentReader.NodeType;
					}
					return this.nodeType;
				}
			}

			public override bool Read()
			{
				bool flag = true;
				switch (this.readState)
				{
				case ReadState.Initial:
					this.readState = ReadState.Interactive;
					this.nodeType = XmlNodeType.Text;
					break;
				case ReadState.Interactive:
					if (this.finishedStream || (this.bytesRemaining == this.chunkSize && this.stringValue == null))
					{
						this.readState = ReadState.EndOfFile;
						this.nodeType = XmlNodeType.EndElement;
					}
					else
					{
						this.bytesRemaining = this.chunkSize;
					}
					break;
				case ReadState.EndOfFile:
					this.nodeType = XmlNodeType.None;
					flag = false;
					break;
				}
				this.stringValue = null;
				this.binHexStream = null;
				this.valueOffset = 0;
				this.valueCount = 0;
				this.stringOffset = 0;
				this.CloseStreams();
				return flag;
			}

			public override int ReadValueAsBase64(byte[] buffer, int offset, int count)
			{
				if (buffer == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
				}
				if (offset < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
				}
				if (offset > buffer.Length)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
				}
				if (count < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
				}
				if (count > buffer.Length - offset)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
				}
				if (this.stringValue != null)
				{
					count = Math.Min(count, this.valueCount);
					if (count > 0)
					{
						Buffer.BlockCopy(this.valueBuffer, this.valueOffset, buffer, offset, count);
						this.valueOffset += count;
						this.valueCount -= count;
					}
					return count;
				}
				if (this.bytesRemaining < count)
				{
					count = this.bytesRemaining;
				}
				int i = 0;
				if (this.readState == ReadState.Interactive)
				{
					while (i < count)
					{
						int num = this.part.Stream.Read(buffer, offset + i, count - i);
						if (num == 0)
						{
							this.finishedStream = true;
							break;
						}
						i += num;
					}
				}
				this.bytesRemaining -= i;
				return i;
			}

			public override int ReadContentAsBase64(byte[] buffer, int offset, int count)
			{
				if (buffer == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
				}
				if (offset < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
				}
				if (offset > buffer.Length)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
				}
				if (count < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
				}
				if (count > buffer.Length - offset)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
				}
				if (this.valueCount > 0)
				{
					count = Math.Min(count, this.valueCount);
					Buffer.BlockCopy(this.valueBuffer, this.valueOffset, buffer, offset, count);
					this.valueOffset += count;
					this.valueCount -= count;
					return count;
				}
				if (this.chunkSize < count)
				{
					count = this.chunkSize;
				}
				int i = 0;
				if (this.readState == ReadState.Interactive)
				{
					while (i < count)
					{
						int num = this.part.Stream.Read(buffer, offset + i, count - i);
						if (num == 0)
						{
							this.finishedStream = true;
							if (!this.Read())
							{
								break;
							}
						}
						i += num;
					}
				}
				this.bytesRemaining = this.chunkSize;
				return i;
			}

			public override int ReadContentAsBinHex(byte[] buffer, int offset, int count)
			{
				if (buffer == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
				}
				if (offset < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
				}
				if (offset > buffer.Length)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
				}
				if (count < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
				}
				if (count > buffer.Length - offset)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
				}
				if (this.chunkSize < count)
				{
					count = this.chunkSize;
				}
				int i = 0;
				int num = 0;
				while (i < count)
				{
					if (this.binHexStream == null)
					{
						try
						{
							this.binHexStream = new MemoryStream(new BinHexEncoding().GetBytes(this.Value));
						}
						catch (FormatException ex)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(ex.Message, ex));
						}
					}
					int num2 = this.binHexStream.Read(buffer, offset + i, count - i);
					if (num2 == 0)
					{
						this.finishedStream = true;
						if (!this.Read())
						{
							break;
						}
						num = 0;
					}
					i += num2;
					num += num2;
				}
				if (this.stringValue != null && num > 0)
				{
					this.stringValue = this.stringValue.Substring(num * 2);
					this.stringOffset = Math.Max(0, this.stringOffset - num * 2);
					this.bytesRemaining = this.chunkSize;
				}
				return i;
			}

			public override int ReadValueChunk(char[] chars, int offset, int count)
			{
				if (chars == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("chars");
				}
				if (offset < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
				}
				if (offset > chars.Length)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
				}
				if (count < 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
				}
				if (count > chars.Length - offset)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { chars.Length - offset })));
				}
				if (this.readState != ReadState.Interactive)
				{
					return 0;
				}
				string value = this.Value;
				count = Math.Min(this.stringValue.Length - this.stringOffset, count);
				if (count > 0)
				{
					this.stringValue.CopyTo(this.stringOffset, chars, offset, count);
					this.stringOffset += count;
				}
				return count;
			}

			public override string Value
			{
				get
				{
					if (this.readState != ReadState.Interactive)
					{
						return string.Empty;
					}
					if (this.stringValue == null)
					{
						int i = this.bytesRemaining;
						i -= i % 3;
						if (this.valueCount > 0 && this.valueOffset > 0)
						{
							Buffer.BlockCopy(this.valueBuffer, this.valueOffset, this.valueBuffer, 0, this.valueCount);
							this.valueOffset = 0;
						}
						i -= this.valueCount;
						if (this.valueBuffer == null)
						{
							this.valueBuffer = new byte[i];
						}
						else if (this.valueBuffer.Length < i)
						{
							Array.Resize<byte>(ref this.valueBuffer, i);
						}
						byte[] array = this.valueBuffer;
						int num = 0;
						while (i > 0)
						{
							int num2 = this.part.Stream.Read(array, num, i);
							if (num2 == 0)
							{
								this.finishedStream = true;
								break;
							}
							this.bytesRemaining -= num2;
							this.valueCount += num2;
							i -= num2;
							num += num2;
						}
						this.stringValue = Convert.ToBase64String(array, 0, this.valueCount);
					}
					return this.stringValue;
				}
			}

			public override string ReadContentAsString()
			{
				int num = this.Quotas.MaxStringContentLength;
				StringBuilder stringBuilder = new StringBuilder();
				do
				{
					string value = this.Value;
					if (value.Length > num)
					{
						XmlExceptionHelper.ThrowMaxStringContentLengthExceeded(this, this.Quotas.MaxStringContentLength);
					}
					num -= value.Length;
					stringBuilder.Append(value);
				}
				while (this.Read());
				return stringBuilder.ToString();
			}

			public override int AttributeCount
			{
				get
				{
					return 0;
				}
			}

			public override string BaseURI
			{
				get
				{
					return this.parentReader.BaseURI;
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
					return this.parentReader.CanResolveEntity;
				}
			}

			public override void Close()
			{
				this.CloseStreams();
				this.readState = ReadState.Closed;
			}

			private void CloseStreams()
			{
				if (this.binHexStream != null)
				{
					this.binHexStream.Close();
					this.binHexStream = null;
				}
			}

			public override int Depth
			{
				get
				{
					if (this.readState != ReadState.Interactive)
					{
						return this.parentReader.Depth;
					}
					return this.parentReader.Depth + 1;
				}
			}

			public override bool EOF
			{
				get
				{
					return this.readState == ReadState.EndOfFile;
				}
			}

			public override string GetAttribute(int index)
			{
				return null;
			}

			public override string GetAttribute(string name)
			{
				return null;
			}

			public override string GetAttribute(string name, string ns)
			{
				return null;
			}

			public override string GetAttribute(XmlDictionaryString localName, XmlDictionaryString ns)
			{
				return null;
			}

			public override bool HasAttributes
			{
				get
				{
					return false;
				}
			}

			public override bool HasValue
			{
				get
				{
					return this.readState == ReadState.Interactive;
				}
			}

			public override bool IsDefault
			{
				get
				{
					return false;
				}
			}

			public override bool IsEmptyElement
			{
				get
				{
					return false;
				}
			}

			public override bool IsLocalName(string localName)
			{
				return false;
			}

			public override bool IsLocalName(XmlDictionaryString localName)
			{
				return false;
			}

			public override bool IsNamespaceUri(string ns)
			{
				return false;
			}

			public override bool IsNamespaceUri(XmlDictionaryString ns)
			{
				return false;
			}

			public override bool IsStartElement()
			{
				return false;
			}

			public override bool IsStartElement(string localName)
			{
				return false;
			}

			public override bool IsStartElement(string localName, string ns)
			{
				return false;
			}

			public override bool IsStartElement(XmlDictionaryString localName, XmlDictionaryString ns)
			{
				return false;
			}

			public override string LocalName
			{
				get
				{
					if (this.readState != ReadState.Interactive)
					{
						return this.parentReader.LocalName;
					}
					return string.Empty;
				}
			}

			public override string LookupNamespace(string ns)
			{
				return this.parentReader.LookupNamespace(ns);
			}

			public override void MoveToAttribute(int index)
			{
			}

			public override bool MoveToAttribute(string name)
			{
				return false;
			}

			public override bool MoveToAttribute(string name, string ns)
			{
				return false;
			}

			public override bool MoveToElement()
			{
				return false;
			}

			public override bool MoveToFirstAttribute()
			{
				return false;
			}

			public override bool MoveToNextAttribute()
			{
				return false;
			}

			public override string Name
			{
				get
				{
					if (this.readState != ReadState.Interactive)
					{
						return this.parentReader.Name;
					}
					return string.Empty;
				}
			}

			public override string NamespaceURI
			{
				get
				{
					if (this.readState != ReadState.Interactive)
					{
						return this.parentReader.NamespaceURI;
					}
					return string.Empty;
				}
			}

			public override XmlNameTable NameTable
			{
				get
				{
					return this.parentReader.NameTable;
				}
			}

			public override string Prefix
			{
				get
				{
					if (this.readState != ReadState.Interactive)
					{
						return this.parentReader.Prefix;
					}
					return string.Empty;
				}
			}

			public override char QuoteChar
			{
				get
				{
					return this.parentReader.QuoteChar;
				}
			}

			public override bool ReadAttributeValue()
			{
				return false;
			}

			public override string ReadInnerXml()
			{
				return this.ReadContentAsString();
			}

			public override string ReadOuterXml()
			{
				return this.ReadContentAsString();
			}

			public override ReadState ReadState
			{
				get
				{
					return this.readState;
				}
			}

			public override void ResolveEntity()
			{
			}

			public override XmlReaderSettings Settings
			{
				get
				{
					return this.parentReader.Settings;
				}
			}

			public override void Skip()
			{
				this.Read();
			}

			public override string this[int index]
			{
				get
				{
					return null;
				}
			}

			public override string this[string name]
			{
				get
				{
					return null;
				}
			}

			public override string this[string name, string ns]
			{
				get
				{
					return null;
				}
			}

			public override string XmlLang
			{
				get
				{
					return this.parentReader.XmlLang;
				}
			}

			public override XmlSpace XmlSpace
			{
				get
				{
					return this.parentReader.XmlSpace;
				}
			}

			public override Type ValueType
			{
				get
				{
					if (this.readState != ReadState.Interactive)
					{
						return this.parentReader.ValueType;
					}
					return typeof(byte[]);
				}
			}

			bool IXmlLineInfo.HasLineInfo()
			{
				return ((IXmlLineInfo)this.parentReader).HasLineInfo();
			}

			int IXmlLineInfo.LineNumber
			{
				get
				{
					return ((IXmlLineInfo)this.parentReader).LineNumber;
				}
			}

			int IXmlLineInfo.LinePosition
			{
				get
				{
					return ((IXmlLineInfo)this.parentReader).LinePosition;
				}
			}

			private int chunkSize = 4096;

			private int bytesRemaining;

			private XmlMtomReader.MimePart part;

			private ReadState readState;

			private XmlDictionaryReader parentReader;

			private string stringValue;

			private int stringOffset;

			private XmlNodeType nodeType;

			private MemoryStream binHexStream;

			private byte[] valueBuffer;

			private int valueOffset;

			private int valueCount;

			private bool finishedStream;
		}
	}
}
