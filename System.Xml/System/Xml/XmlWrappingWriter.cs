using System;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class XmlWrappingWriter : XmlWriter
	{
		internal XmlWrappingWriter(XmlWriter baseWriter)
		{
			this.writer = baseWriter;
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return this.writer.Settings;
			}
		}

		public override WriteState WriteState
		{
			get
			{
				return this.writer.WriteState;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.writer.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.writer.XmlLang;
			}
		}

		public override void WriteStartDocument()
		{
			this.writer.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.writer.WriteStartDocument(standalone);
		}

		public override void WriteEndDocument()
		{
			this.writer.WriteEndDocument();
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.writer.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.writer.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteEndElement()
		{
			this.writer.WriteEndElement();
		}

		public override void WriteFullEndElement()
		{
			this.writer.WriteFullEndElement();
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.writer.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			this.writer.WriteEndAttribute();
		}

		public override void WriteCData(string text)
		{
			this.writer.WriteCData(text);
		}

		public override void WriteComment(string text)
		{
			this.writer.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.writer.WriteProcessingInstruction(name, text);
		}

		public override void WriteEntityRef(string name)
		{
			this.writer.WriteEntityRef(name);
		}

		public override void WriteCharEntity(char ch)
		{
			this.writer.WriteCharEntity(ch);
		}

		public override void WriteWhitespace(string ws)
		{
			this.writer.WriteWhitespace(ws);
		}

		public override void WriteString(string text)
		{
			this.writer.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.writer.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.writer.WriteChars(buffer, index, count);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.writer.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			this.writer.WriteRaw(data);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.writer.WriteBase64(buffer, index, count);
		}

		public override void Close()
		{
			this.writer.Close();
		}

		public override void Flush()
		{
			this.writer.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			return this.writer.LookupPrefix(ns);
		}

		public override void WriteValue(object value)
		{
			this.writer.WriteValue(value);
		}

		public override void WriteValue(string value)
		{
			this.writer.WriteValue(value);
		}

		public override void WriteValue(bool value)
		{
			this.writer.WriteValue(value);
		}

		public override void WriteValue(DateTime value)
		{
			this.writer.WriteValue(value);
		}

		public override void WriteValue(DateTimeOffset value)
		{
			this.writer.WriteValue(value);
		}

		public override void WriteValue(double value)
		{
			this.writer.WriteValue(value);
		}

		public override void WriteValue(float value)
		{
			this.writer.WriteValue(value);
		}

		public override void WriteValue(decimal value)
		{
			this.writer.WriteValue(value);
		}

		public override void WriteValue(int value)
		{
			this.writer.WriteValue(value);
		}

		public override void WriteValue(long value)
		{
			this.writer.WriteValue(value);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				((IDisposable)this.writer).Dispose();
			}
		}

		public override Task WriteStartDocumentAsync()
		{
			return this.writer.WriteStartDocumentAsync();
		}

		public override Task WriteStartDocumentAsync(bool standalone)
		{
			return this.writer.WriteStartDocumentAsync(standalone);
		}

		public override Task WriteEndDocumentAsync()
		{
			return this.writer.WriteEndDocumentAsync();
		}

		public override Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
		{
			return this.writer.WriteDocTypeAsync(name, pubid, sysid, subset);
		}

		public override Task WriteStartElementAsync(string prefix, string localName, string ns)
		{
			return this.writer.WriteStartElementAsync(prefix, localName, ns);
		}

		public override Task WriteEndElementAsync()
		{
			return this.writer.WriteEndElementAsync();
		}

		public override Task WriteFullEndElementAsync()
		{
			return this.writer.WriteFullEndElementAsync();
		}

		protected internal override Task WriteStartAttributeAsync(string prefix, string localName, string ns)
		{
			return this.writer.WriteStartAttributeAsync(prefix, localName, ns);
		}

		protected internal override Task WriteEndAttributeAsync()
		{
			return this.writer.WriteEndAttributeAsync();
		}

		public override Task WriteCDataAsync(string text)
		{
			return this.writer.WriteCDataAsync(text);
		}

		public override Task WriteCommentAsync(string text)
		{
			return this.writer.WriteCommentAsync(text);
		}

		public override Task WriteProcessingInstructionAsync(string name, string text)
		{
			return this.writer.WriteProcessingInstructionAsync(name, text);
		}

		public override Task WriteEntityRefAsync(string name)
		{
			return this.writer.WriteEntityRefAsync(name);
		}

		public override Task WriteCharEntityAsync(char ch)
		{
			return this.writer.WriteCharEntityAsync(ch);
		}

		public override Task WriteWhitespaceAsync(string ws)
		{
			return this.writer.WriteWhitespaceAsync(ws);
		}

		public override Task WriteStringAsync(string text)
		{
			return this.writer.WriteStringAsync(text);
		}

		public override Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
		{
			return this.writer.WriteSurrogateCharEntityAsync(lowChar, highChar);
		}

		public override Task WriteCharsAsync(char[] buffer, int index, int count)
		{
			return this.writer.WriteCharsAsync(buffer, index, count);
		}

		public override Task WriteRawAsync(char[] buffer, int index, int count)
		{
			return this.writer.WriteRawAsync(buffer, index, count);
		}

		public override Task WriteRawAsync(string data)
		{
			return this.writer.WriteRawAsync(data);
		}

		public override Task WriteBase64Async(byte[] buffer, int index, int count)
		{
			return this.writer.WriteBase64Async(buffer, index, count);
		}

		public override Task FlushAsync()
		{
			return this.writer.FlushAsync();
		}

		protected XmlWriter writer;
	}
}
