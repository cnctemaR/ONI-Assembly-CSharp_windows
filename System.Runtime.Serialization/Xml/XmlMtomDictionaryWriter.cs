using System;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Xml
{
	internal class XmlMtomDictionaryWriter : XmlDictionaryWriter
	{
		public XmlMtomDictionaryWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo, string boundary, string startUri, bool writeMessageHeaders, bool ownsStream)
		{
			this.writer = new StreamWriter(stream, encoding);
			this.max_bytes = maxSizeInBytes;
			this.write_headers = writeMessageHeaders;
			this.owns_stream = ownsStream;
			this.xml_writer_settings = new XmlWriterSettings
			{
				Encoding = encoding,
				OmitXmlDeclaration = true
			};
			ContentType contentType = new ContentType("multipart/related");
			contentType.Parameters["type"] = "application/xop+xml";
			contentType.Boundary = boundary;
			contentType.Parameters["start"] = "<" + startUri + ">";
			contentType.Parameters["start-info"] = startInfo;
			this.content_type = contentType;
		}

		private XmlWriter CreateWriter()
		{
			return XmlWriter.Create(this.writer, this.xml_writer_settings);
		}

		public override void Close()
		{
			this.w.Close();
			if (this.owns_stream)
			{
				this.writer.Close();
			}
		}

		public override void Flush()
		{
			this.w.Flush();
		}

		public override string LookupPrefix(string namespaceUri)
		{
			return this.w.LookupPrefix(namespaceUri);
		}

		public override void WriteBase64(byte[] bytes, int start, int length)
		{
			this.CheckState();
			this.w.WriteBase64(bytes, start, length);
		}

		public override void WriteCData(string text)
		{
			this.CheckState();
			this.w.WriteCData(text);
		}

		public override void WriteCharEntity(char c)
		{
			this.CheckState();
			this.w.WriteCharEntity(c);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.CheckState();
			this.w.WriteChars(buffer, index, count);
		}

		public override void WriteComment(string comment)
		{
			this.CheckState();
			this.w.WriteComment(comment);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string intSubset)
		{
			throw new NotSupportedException();
		}

		public override void WriteEndAttribute()
		{
			this.w.WriteEndAttribute();
		}

		public override void WriteEndDocument()
		{
			this.w.WriteEndDocument();
		}

		public override void WriteEndElement()
		{
			this.w.WriteEndElement();
			if (--this.depth == 0)
			{
				this.WriteEndOfMimeSection();
			}
		}

		public override void WriteEntityRef(string name)
		{
			this.w.WriteEntityRef(name);
		}

		public override void WriteFullEndElement()
		{
			this.w.WriteFullEndElement();
			if (--this.depth == 0)
			{
				this.WriteEndOfMimeSection();
			}
		}

		public override void WriteProcessingInstruction(string name, string data)
		{
			throw new NotSupportedException();
		}

		public override void WriteRaw(string raw)
		{
			this.CheckState();
			this.w.WriteRaw(raw);
		}

		public override void WriteRaw(char[] chars, int index, int count)
		{
			this.CheckState();
			this.w.WriteRaw(chars, index, count);
		}

		public override void WriteStartAttribute(string prefix, string localName, string namespaceURI)
		{
			this.CheckState();
			this.w.WriteStartAttribute(prefix, localName, namespaceURI);
		}

		public override void WriteStartDocument()
		{
			this.CheckState();
			this.w.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.CheckState();
			this.w.WriteStartDocument(standalone);
		}

		public override void WriteStartElement(string prefix, string localName, string namespaceURI)
		{
			this.CheckState();
			if (this.depth == 0)
			{
				this.WriteStartOfMimeSection();
			}
			this.w.WriteStartElement(prefix, localName, namespaceURI);
			this.depth++;
		}

		public override WriteState WriteState
		{
			get
			{
				return this.w.WriteState;
			}
		}

		public override void WriteString(string text)
		{
			this.CheckState();
			int num = 0;
			for (;;)
			{
				int num2 = text.IndexOf('\r', num);
				if (num2 < 0)
				{
					break;
				}
				this.w.WriteString(text.Substring(num, num2 - num));
				this.WriteCharEntity('\r');
				num = num2 + 1;
			}
			this.w.WriteString(text.Substring(num));
		}

		public override void WriteSurrogateCharEntity(char low, char high)
		{
			this.CheckState();
			this.w.WriteSurrogateCharEntity(low, high);
		}

		public override void WriteWhitespace(string text)
		{
			this.CheckState();
			this.w.WriteWhitespace(text);
		}

		public override string XmlLang
		{
			get
			{
				return this.w.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.w.XmlSpace;
			}
		}

		private void CheckState()
		{
			if (this.w == null && this.write_headers)
			{
				this.WriteMimeHeaders();
			}
			if (this.w == null || this.w.WriteState == WriteState.Closed || this.w.WriteState == WriteState.Error)
			{
				this.w = this.CreateWriter();
			}
		}

		private void WriteMimeHeaders()
		{
			this.writer.Write("MIME-Version: 1.0\r\n");
			this.writer.Write("Content-Type: ");
			this.writer.Write(this.content_type.ToString());
			this.writer.Write("\r\n\r\n\r\n");
		}

		private void WriteStartOfMimeSection()
		{
			this.section_count++;
			if (this.section_count > 1)
			{
				return;
			}
			this.writer.Write("\r\n");
			this.writer.Write("--");
			this.writer.Write(this.content_type.Boundary);
			this.writer.Write("\r\n");
			this.writer.Write("Content-ID: ");
			this.writer.Write(this.content_type.Parameters["start"]);
			this.writer.Write("\r\n");
			this.writer.Write("Content-Transfer-Encoding: 8bit\r\n");
			this.writer.Write("Content-Type: application/xop+xml;charset=");
			this.writer.Write(this.xml_writer_settings.Encoding.HeaderName);
			this.writer.Write(";type=\"");
			this.writer.Write(this.content_type.Parameters["start-info"].Replace("\"", "\\\""));
			this.writer.Write("\"\r\n\r\n");
		}

		private void WriteEndOfMimeSection()
		{
			if (this.section_count > 1)
			{
				return;
			}
			this.writer.Write("\r\n");
			this.writer.Write("--");
			this.writer.Write(this.content_type.Boundary);
			this.writer.Write("--\r\n");
		}

		private TextWriter writer;

		private XmlWriterSettings xml_writer_settings;

		private Encoding encoding;

		private int max_bytes;

		private bool write_headers;

		private bool owns_stream;

		private ContentType content_type;

		private XmlWriter w;

		private int depth;

		private int section_count;
	}
}
