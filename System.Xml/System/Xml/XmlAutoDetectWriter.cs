using System;
using System.IO;

namespace System.Xml
{
	internal class XmlAutoDetectWriter : XmlRawWriter, IRemovableWriter
	{
		private XmlAutoDetectWriter(XmlWriterSettings writerSettings)
		{
			this.writerSettings = writerSettings.Clone();
			this.writerSettings.ReadOnly = true;
			this.eventCache = new XmlEventCache(string.Empty, true);
		}

		public XmlAutoDetectWriter(TextWriter textWriter, XmlWriterSettings writerSettings)
			: this(writerSettings)
		{
			this.textWriter = textWriter;
		}

		public XmlAutoDetectWriter(Stream strm, XmlWriterSettings writerSettings)
			: this(writerSettings)
		{
			this.strm = strm;
		}

		public OnRemoveWriter OnRemoveWriterEvent
		{
			get
			{
				return this.onRemove;
			}
			set
			{
				this.onRemove = value;
			}
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return this.writerSettings;
			}
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			if (this.wrapped == null)
			{
				if (ns.Length == 0 && XmlAutoDetectWriter.IsHtmlTag(localName))
				{
					this.CreateWrappedWriter(XmlOutputMethod.Html);
				}
				else
				{
					this.CreateWrappedWriter(XmlOutputMethod.Xml);
				}
			}
			this.wrapped.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			this.wrapped.WriteEndAttribute();
		}

		public override void WriteCData(string text)
		{
			if (this.TextBlockCreatesWriter(text))
			{
				this.wrapped.WriteCData(text);
				return;
			}
			this.eventCache.WriteCData(text);
		}

		public override void WriteComment(string text)
		{
			if (this.wrapped == null)
			{
				this.eventCache.WriteComment(text);
				return;
			}
			this.wrapped.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			if (this.wrapped == null)
			{
				this.eventCache.WriteProcessingInstruction(name, text);
				return;
			}
			this.wrapped.WriteProcessingInstruction(name, text);
		}

		public override void WriteWhitespace(string ws)
		{
			if (this.wrapped == null)
			{
				this.eventCache.WriteWhitespace(ws);
				return;
			}
			this.wrapped.WriteWhitespace(ws);
		}

		public override void WriteString(string text)
		{
			if (this.TextBlockCreatesWriter(text))
			{
				this.wrapped.WriteString(text);
				return;
			}
			this.eventCache.WriteString(text);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.WriteString(new string(buffer, index, count));
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.WriteRaw(new string(buffer, index, count));
		}

		public override void WriteRaw(string data)
		{
			if (this.TextBlockCreatesWriter(data))
			{
				this.wrapped.WriteRaw(data);
				return;
			}
			this.eventCache.WriteRaw(data);
		}

		public override void WriteEntityRef(string name)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteEntityRef(name);
		}

		public override void WriteCharEntity(char ch)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteCharEntity(ch);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteBase64(buffer, index, count);
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteBinHex(buffer, index, count);
		}

		public override void Close()
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.Close();
		}

		public override void Flush()
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.Flush();
		}

		public override void WriteValue(object value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(string value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(bool value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(DateTime value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(DateTimeOffset value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(double value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(float value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(decimal value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(int value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(long value)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteValue(value);
		}

		internal override IXmlNamespaceResolver NamespaceResolver
		{
			get
			{
				return this.resolver;
			}
			set
			{
				this.resolver = value;
				if (this.wrapped == null)
				{
					this.eventCache.NamespaceResolver = value;
					return;
				}
				this.wrapped.NamespaceResolver = value;
			}
		}

		internal override void WriteXmlDeclaration(XmlStandalone standalone)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteXmlDeclaration(standalone);
		}

		internal override void WriteXmlDeclaration(string xmldecl)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteXmlDeclaration(xmldecl);
		}

		internal override void StartElementContent()
		{
			this.wrapped.StartElementContent();
		}

		internal override void WriteEndElement(string prefix, string localName, string ns)
		{
			this.wrapped.WriteEndElement(prefix, localName, ns);
		}

		internal override void WriteFullEndElement(string prefix, string localName, string ns)
		{
			this.wrapped.WriteFullEndElement(prefix, localName, ns);
		}

		internal override void WriteNamespaceDeclaration(string prefix, string ns)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteNamespaceDeclaration(prefix, ns);
		}

		internal override bool SupportsNamespaceDeclarationInChunks
		{
			get
			{
				return this.wrapped.SupportsNamespaceDeclarationInChunks;
			}
		}

		internal override void WriteStartNamespaceDeclaration(string prefix)
		{
			this.EnsureWrappedWriter(XmlOutputMethod.Xml);
			this.wrapped.WriteStartNamespaceDeclaration(prefix);
		}

		internal override void WriteEndNamespaceDeclaration()
		{
			this.wrapped.WriteEndNamespaceDeclaration();
		}

		private static bool IsHtmlTag(string tagName)
		{
			return tagName.Length == 4 && (tagName[0] == 'H' || tagName[0] == 'h') && (tagName[1] == 'T' || tagName[1] == 't') && (tagName[2] == 'M' || tagName[2] == 'm') && (tagName[3] == 'L' || tagName[3] == 'l');
		}

		private void EnsureWrappedWriter(XmlOutputMethod outMethod)
		{
			if (this.wrapped == null)
			{
				this.CreateWrappedWriter(outMethod);
			}
		}

		private bool TextBlockCreatesWriter(string textBlock)
		{
			if (this.wrapped == null)
			{
				if (XmlCharType.Instance.IsOnlyWhitespace(textBlock))
				{
					return false;
				}
				this.CreateWrappedWriter(XmlOutputMethod.Xml);
			}
			return true;
		}

		private void CreateWrappedWriter(XmlOutputMethod outMethod)
		{
			this.writerSettings.ReadOnly = false;
			this.writerSettings.OutputMethod = outMethod;
			if (outMethod == XmlOutputMethod.Html && this.writerSettings.IndentInternal == TriState.Unknown)
			{
				this.writerSettings.Indent = true;
			}
			this.writerSettings.ReadOnly = true;
			if (this.textWriter != null)
			{
				this.wrapped = ((XmlWellFormedWriter)XmlWriter.Create(this.textWriter, this.writerSettings)).RawWriter;
			}
			else
			{
				this.wrapped = ((XmlWellFormedWriter)XmlWriter.Create(this.strm, this.writerSettings)).RawWriter;
			}
			this.eventCache.EndEvents();
			this.eventCache.EventsToWriter(this.wrapped);
			if (this.onRemove != null)
			{
				this.onRemove(this.wrapped);
			}
		}

		private XmlRawWriter wrapped;

		private OnRemoveWriter onRemove;

		private XmlWriterSettings writerSettings;

		private XmlEventCache eventCache;

		private TextWriter textWriter;

		private Stream strm;
	}
}
