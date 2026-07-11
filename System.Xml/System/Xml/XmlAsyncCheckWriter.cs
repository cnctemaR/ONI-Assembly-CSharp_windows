using System;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace System.Xml
{
	internal class XmlAsyncCheckWriter : XmlWriter
	{
		internal XmlWriter CoreWriter
		{
			get
			{
				return this.coreWriter;
			}
		}

		public XmlAsyncCheckWriter(XmlWriter writer)
		{
			this.coreWriter = writer;
		}

		private void CheckAsync()
		{
			if (!this.lastTask.IsCompleted)
			{
				throw new InvalidOperationException(Res.GetString("An asynchronous operation is already in progress."));
			}
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				XmlWriterSettings xmlWriterSettings = this.coreWriter.Settings;
				if (xmlWriterSettings != null)
				{
					xmlWriterSettings = xmlWriterSettings.Clone();
				}
				else
				{
					xmlWriterSettings = new XmlWriterSettings();
				}
				xmlWriterSettings.Async = true;
				xmlWriterSettings.ReadOnly = true;
				return xmlWriterSettings;
			}
		}

		public override void WriteStartDocument()
		{
			this.CheckAsync();
			this.coreWriter.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.CheckAsync();
			this.coreWriter.WriteStartDocument(standalone);
		}

		public override void WriteEndDocument()
		{
			this.CheckAsync();
			this.coreWriter.WriteEndDocument();
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.CheckAsync();
			this.coreWriter.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.CheckAsync();
			this.coreWriter.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteEndElement()
		{
			this.CheckAsync();
			this.coreWriter.WriteEndElement();
		}

		public override void WriteFullEndElement()
		{
			this.CheckAsync();
			this.coreWriter.WriteFullEndElement();
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.CheckAsync();
			this.coreWriter.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			this.CheckAsync();
			this.coreWriter.WriteEndAttribute();
		}

		public override void WriteCData(string text)
		{
			this.CheckAsync();
			this.coreWriter.WriteCData(text);
		}

		public override void WriteComment(string text)
		{
			this.CheckAsync();
			this.coreWriter.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.CheckAsync();
			this.coreWriter.WriteProcessingInstruction(name, text);
		}

		public override void WriteEntityRef(string name)
		{
			this.CheckAsync();
			this.coreWriter.WriteEntityRef(name);
		}

		public override void WriteCharEntity(char ch)
		{
			this.CheckAsync();
			this.coreWriter.WriteCharEntity(ch);
		}

		public override void WriteWhitespace(string ws)
		{
			this.CheckAsync();
			this.coreWriter.WriteWhitespace(ws);
		}

		public override void WriteString(string text)
		{
			this.CheckAsync();
			this.coreWriter.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.CheckAsync();
			this.coreWriter.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.CheckAsync();
			this.coreWriter.WriteChars(buffer, index, count);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.CheckAsync();
			this.coreWriter.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			this.CheckAsync();
			this.coreWriter.WriteRaw(data);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			this.coreWriter.WriteBase64(buffer, index, count);
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			this.coreWriter.WriteBinHex(buffer, index, count);
		}

		public override WriteState WriteState
		{
			get
			{
				this.CheckAsync();
				return this.coreWriter.WriteState;
			}
		}

		public override void Close()
		{
			this.CheckAsync();
			this.coreWriter.Close();
		}

		public override void Flush()
		{
			this.CheckAsync();
			this.coreWriter.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			this.CheckAsync();
			return this.coreWriter.LookupPrefix(ns);
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				this.CheckAsync();
				return this.coreWriter.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				this.CheckAsync();
				return this.coreWriter.XmlLang;
			}
		}

		public override void WriteNmToken(string name)
		{
			this.CheckAsync();
			this.coreWriter.WriteNmToken(name);
		}

		public override void WriteName(string name)
		{
			this.CheckAsync();
			this.coreWriter.WriteName(name);
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			this.CheckAsync();
			this.coreWriter.WriteQualifiedName(localName, ns);
		}

		public override void WriteValue(object value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteValue(string value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteValue(bool value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteValue(DateTime value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteValue(DateTimeOffset value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteValue(double value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteValue(float value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteValue(decimal value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteValue(int value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteValue(long value)
		{
			this.CheckAsync();
			this.coreWriter.WriteValue(value);
		}

		public override void WriteAttributes(XmlReader reader, bool defattr)
		{
			this.CheckAsync();
			this.coreWriter.WriteAttributes(reader, defattr);
		}

		public override void WriteNode(XmlReader reader, bool defattr)
		{
			this.CheckAsync();
			this.coreWriter.WriteNode(reader, defattr);
		}

		public override void WriteNode(XPathNavigator navigator, bool defattr)
		{
			this.CheckAsync();
			this.coreWriter.WriteNode(navigator, defattr);
		}

		protected override void Dispose(bool disposing)
		{
			this.CheckAsync();
			this.coreWriter.Dispose();
		}

		public override Task WriteStartDocumentAsync()
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteStartDocumentAsync();
			this.lastTask = task;
			return task;
		}

		public override Task WriteStartDocumentAsync(bool standalone)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteStartDocumentAsync(standalone);
			this.lastTask = task;
			return task;
		}

		public override Task WriteEndDocumentAsync()
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteEndDocumentAsync();
			this.lastTask = task;
			return task;
		}

		public override Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteDocTypeAsync(name, pubid, sysid, subset);
			this.lastTask = task;
			return task;
		}

		public override Task WriteStartElementAsync(string prefix, string localName, string ns)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteStartElementAsync(prefix, localName, ns);
			this.lastTask = task;
			return task;
		}

		public override Task WriteEndElementAsync()
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteEndElementAsync();
			this.lastTask = task;
			return task;
		}

		public override Task WriteFullEndElementAsync()
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteFullEndElementAsync();
			this.lastTask = task;
			return task;
		}

		protected internal override Task WriteStartAttributeAsync(string prefix, string localName, string ns)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteStartAttributeAsync(prefix, localName, ns);
			this.lastTask = task;
			return task;
		}

		protected internal override Task WriteEndAttributeAsync()
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteEndAttributeAsync();
			this.lastTask = task;
			return task;
		}

		public override Task WriteCDataAsync(string text)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteCDataAsync(text);
			this.lastTask = task;
			return task;
		}

		public override Task WriteCommentAsync(string text)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteCommentAsync(text);
			this.lastTask = task;
			return task;
		}

		public override Task WriteProcessingInstructionAsync(string name, string text)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteProcessingInstructionAsync(name, text);
			this.lastTask = task;
			return task;
		}

		public override Task WriteEntityRefAsync(string name)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteEntityRefAsync(name);
			this.lastTask = task;
			return task;
		}

		public override Task WriteCharEntityAsync(char ch)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteCharEntityAsync(ch);
			this.lastTask = task;
			return task;
		}

		public override Task WriteWhitespaceAsync(string ws)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteWhitespaceAsync(ws);
			this.lastTask = task;
			return task;
		}

		public override Task WriteStringAsync(string text)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteStringAsync(text);
			this.lastTask = task;
			return task;
		}

		public override Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteSurrogateCharEntityAsync(lowChar, highChar);
			this.lastTask = task;
			return task;
		}

		public override Task WriteCharsAsync(char[] buffer, int index, int count)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteCharsAsync(buffer, index, count);
			this.lastTask = task;
			return task;
		}

		public override Task WriteRawAsync(char[] buffer, int index, int count)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteRawAsync(buffer, index, count);
			this.lastTask = task;
			return task;
		}

		public override Task WriteRawAsync(string data)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteRawAsync(data);
			this.lastTask = task;
			return task;
		}

		public override Task WriteBase64Async(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteBase64Async(buffer, index, count);
			this.lastTask = task;
			return task;
		}

		public override Task WriteBinHexAsync(byte[] buffer, int index, int count)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteBinHexAsync(buffer, index, count);
			this.lastTask = task;
			return task;
		}

		public override Task FlushAsync()
		{
			this.CheckAsync();
			Task task = this.coreWriter.FlushAsync();
			this.lastTask = task;
			return task;
		}

		public override Task WriteNmTokenAsync(string name)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteNmTokenAsync(name);
			this.lastTask = task;
			return task;
		}

		public override Task WriteNameAsync(string name)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteNameAsync(name);
			this.lastTask = task;
			return task;
		}

		public override Task WriteQualifiedNameAsync(string localName, string ns)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteQualifiedNameAsync(localName, ns);
			this.lastTask = task;
			return task;
		}

		public override Task WriteAttributesAsync(XmlReader reader, bool defattr)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteAttributesAsync(reader, defattr);
			this.lastTask = task;
			return task;
		}

		public override Task WriteNodeAsync(XmlReader reader, bool defattr)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteNodeAsync(reader, defattr);
			this.lastTask = task;
			return task;
		}

		public override Task WriteNodeAsync(XPathNavigator navigator, bool defattr)
		{
			this.CheckAsync();
			Task task = this.coreWriter.WriteNodeAsync(navigator, defattr);
			this.lastTask = task;
			return task;
		}

		private readonly XmlWriter coreWriter;

		private Task lastTask = AsyncHelper.DoneTask;
	}
}
