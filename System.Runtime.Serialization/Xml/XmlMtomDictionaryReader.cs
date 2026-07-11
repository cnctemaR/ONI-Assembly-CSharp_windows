using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Xml
{
	internal class XmlMtomDictionaryReader : XmlDictionaryReader
	{
		public XmlMtomDictionaryReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas)
		{
			this.stream = stream;
			this.encoding = encoding;
			this.quotas = quotas;
			this.Initialize();
		}

		public XmlMtomDictionaryReader(Stream stream, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize, OnXmlDictionaryReaderClose onClose)
		{
			this.stream = stream;
			this.encodings = encodings;
			this.content_type = ((contentType == null) ? null : this.CreateContentType(contentType));
			this.quotas = quotas;
			this.max_buffer_size = maxBufferSize;
			this.on_close = onClose;
			this.Initialize();
		}

		private void Initialize()
		{
			NameTable nameTable = new NameTable();
			this.initial_reader = new NonInteractiveStateXmlReader(string.Empty, nameTable, ReadState.Initial);
			this.eof_reader = new NonInteractiveStateXmlReader(string.Empty, nameTable, ReadState.EndOfFile);
			this.xml_reader = this.initial_reader;
		}

		private ContentType CreateContentType(string contentTypeString)
		{
			ContentType contentType = null;
			foreach (string text in contentTypeString.Split(new char[] { ';' }))
			{
				string text2 = text.Trim();
				if (contentType == null)
				{
					contentType = new ContentType(text2);
				}
				else
				{
					int num = text2.IndexOf('=');
					if (num < 0)
					{
						throw new XmlException("Invalid content type header");
					}
					string text3 = this.StripBraces(text2.Substring(num + 1));
					contentType.Parameters[text2.Substring(0, num)] = text3;
				}
			}
			return contentType;
		}

		private XmlReader Reader
		{
			get
			{
				return this.part_reader ?? this.xml_reader;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.Reader == this.eof_reader;
			}
		}

		public override void Close()
		{
			if (!this.EOF && this.on_close != null)
			{
				this.on_close(this);
			}
			this.xml_reader = this.eof_reader;
		}

		public override bool Read()
		{
			if (this.EOF)
			{
				return false;
			}
			if (this.Reader == this.initial_reader)
			{
				this.SetupPrimaryReader();
			}
			if (this.part_reader != null)
			{
				this.part_reader = null;
			}
			if (!this.Reader.Read())
			{
				this.xml_reader = this.eof_reader;
				return false;
			}
			if (this.Reader.LocalName == "Include" && this.Reader.NamespaceURI == "http://www.w3.org/2004/08/xop/include")
			{
				string text = this.Reader.GetAttribute("href");
				if (!text.StartsWith("cid:"))
				{
					throw new XmlException("Cannot resolve non-cid href attribute value in XOP Include element");
				}
				text = text.Substring(4);
				if (!this.readers.ContainsKey(text))
				{
					this.ReadToIdentifiedStream(text);
				}
				this.part_reader = new MultiPartedXmlReader(this.Reader, this.readers[text]);
			}
			return true;
		}

		private void SetupPrimaryReader()
		{
			this.ReadOptionalMimeHeaders();
			if (this.current_content_type != null)
			{
				this.content_type = this.current_content_type;
			}
			if (this.content_type == null)
			{
				throw new XmlException("Content-Type header for the MTOM message was not found");
			}
			if (this.content_type.Boundary == null)
			{
				throw new XmlException("Content-Type header for the MTOM message must contain 'boundary' parameter");
			}
			if (this.encoding == null && this.content_type.CharSet != null)
			{
				this.encoding = Encoding.GetEncoding(this.content_type.CharSet);
			}
			if (this.encoding == null && this.encodings == null)
			{
				throw new XmlException("Encoding specification is required either in the constructor argument or the content-type header");
			}
			string text = "--" + this.content_type.Boundary;
			string text2;
			for (;;)
			{
				text2 = this.ReadAsciiLine().Trim();
				if (text2 == null)
				{
					break;
				}
				if (text2.Length != 0)
				{
					goto Block_9;
				}
			}
			return;
			Block_9:
			if (!text2.StartsWith(text, StringComparison.Ordinal))
			{
				throw new XmlException(string.Format("Unexpected boundary line was found. Expected boundary is '{0}' but it was '{1}'", this.content_type.Boundary, text2));
			}
			string text3 = this.content_type.Parameters["start"];
			this.ReadToIdentifiedStream(text3);
			this.xml_reader = XmlReader.Create(this.readers[text3].CreateTextReader());
		}

		private void ReadToIdentifiedStream(string id)
		{
			while (this.ReadNextStream())
			{
				if (this.current_content_id == id || id == null)
				{
					return;
				}
			}
			throw new XmlException(string.Format("The stream '{0}' did not appear", id));
		}

		private bool ReadNextStream()
		{
			this.ReadOptionalMimeHeaders();
			string text = "--" + this.content_type.Boundary;
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				string text2 = this.ReadAsciiLine();
				if (text2 == null && stringBuilder.Length == 0)
				{
					break;
				}
				if (text2 == null || text2.StartsWith(text, StringComparison.Ordinal))
				{
					goto IL_004F;
				}
				stringBuilder.Append(text2);
			}
			return false;
			IL_004F:
			this.readers.Add(this.current_content_id, new MimeEncodedStream(this.current_content_id, this.current_content_encoding, stringBuilder.ToString()));
			return true;
		}

		private void ReadOptionalMimeHeaders()
		{
			this.peek_char = this.stream.ReadByte();
			if (this.peek_char == 45)
			{
				return;
			}
			this.ReadMimeHeaders();
		}

		private string ReadAllHeaderLines()
		{
			string text = string.Empty;
			for (;;)
			{
				string text2 = this.ReadAsciiLine();
				if (text2.Length == 0)
				{
					break;
				}
				text2 = text2.TrimEnd(new char[0]);
				text += text2;
				if (text2[text2.Length - 1] != ';')
				{
					text += '\n';
				}
			}
			return text;
		}

		private void ReadMimeHeaders()
		{
			foreach (string text in this.ReadAllHeaderLines().Split(new char[] { '\n' }))
			{
				if (text.Length != 0)
				{
					int num = text.IndexOf(':');
					if (num < 0)
					{
						throw new XmlException(string.Format("Unexpected header string: {0}", text));
					}
					string text2 = this.StripBraces(text.Substring(num + 1).Trim());
					string text3 = text.Substring(0, num).ToLower();
					switch (text3)
					{
					case "content-type":
						this.current_content_type = this.CreateContentType(text2);
						break;
					case "content-id":
						this.current_content_id = text2;
						break;
					case "content-transfer-encoding":
						this.current_content_encoding = text2;
						break;
					}
				}
			}
		}

		private string StripBraces(string s)
		{
			if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"')
			{
				s = s.Substring(1, s.Length - 2);
			}
			if (s.Length >= 2 && s[0] == '<' && s[s.Length - 1] == '>')
			{
				s = s.Substring(1, s.Length - 2);
			}
			return s;
		}

		private string ReadAsciiLine()
		{
			if (this.buffer == null)
			{
				this.buffer = new byte[1024];
			}
			int num = 0;
			int num2 = this.peek_char;
			bool flag = num2 >= 0;
			this.peek_char = -1;
			for (;;)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					num2 = this.stream.ReadByte();
				}
				if (num2 < 0)
				{
					break;
				}
				if (num2 == 13)
				{
					num2 = this.stream.ReadByte();
					if (num2 < 0)
					{
						goto Block_6;
					}
					if (num2 == 10)
					{
						goto Block_7;
					}
					this.buffer[num++] = 13;
					flag = true;
				}
				else
				{
					this.buffer[num++] = (byte)num2;
				}
				if (num == this.buffer.Length)
				{
					byte[] array = new byte[this.buffer.Length << 1];
					Array.Copy(this.buffer, 0, array, 0, this.buffer.Length);
					this.buffer = array;
				}
			}
			if (num > 0)
			{
				throw new XmlException("The stream ends without end of line");
			}
			return null;
			Block_6:
			this.buffer[num++] = 13;
			Block_7:
			return Encoding.ASCII.GetString(this.buffer, 0, num);
		}

		public override int AttributeCount
		{
			get
			{
				return this.Reader.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.Reader.BaseURI;
			}
		}

		public override int Depth
		{
			get
			{
				return this.Reader.Depth;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.Reader.HasValue;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.Reader.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.Reader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.Reader.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.Reader.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.Reader.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.Reader.Prefix;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return this.Reader.ReadState;
			}
		}

		public override string Value
		{
			get
			{
				return this.Reader.Value;
			}
		}

		public override bool MoveToElement()
		{
			return this.Reader.MoveToElement();
		}

		public override string GetAttribute(int index)
		{
			return this.Reader.GetAttribute(index);
		}

		public override string GetAttribute(string name)
		{
			return this.Reader.GetAttribute(name);
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			return this.Reader.GetAttribute(localName, namespaceURI);
		}

		public override void MoveToAttribute(int index)
		{
			this.Reader.MoveToAttribute(index);
		}

		public override bool MoveToAttribute(string name)
		{
			return this.Reader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			return this.Reader.MoveToAttribute(localName, namespaceURI);
		}

		public override bool MoveToFirstAttribute()
		{
			return this.Reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this.Reader.MoveToNextAttribute();
		}

		public override string LookupNamespace(string prefix)
		{
			return this.Reader.LookupNamespace(prefix);
		}

		public override bool ReadAttributeValue()
		{
			return this.Reader.ReadAttributeValue();
		}

		public override void ResolveEntity()
		{
			this.Reader.ResolveEntity();
		}

		private Stream stream;

		private Encoding encoding;

		private Encoding[] encodings;

		private ContentType content_type;

		private XmlDictionaryReaderQuotas quotas;

		private int max_buffer_size;

		private OnXmlDictionaryReaderClose on_close;

		private Dictionary<string, MimeEncodedStream> readers = new Dictionary<string, MimeEncodedStream>();

		private XmlReader xml_reader;

		private XmlReader initial_reader;

		private XmlReader eof_reader;

		private XmlReader part_reader;

		private int buffer_length;

		private byte[] buffer;

		private int peek_char;

		private ContentType current_content_type;

		private int content_index;

		private string current_content_id;

		private string current_content_encoding;
	}
}
