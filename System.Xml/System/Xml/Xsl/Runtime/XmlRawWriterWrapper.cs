using System;

namespace System.Xml.Xsl.Runtime
{
	internal sealed class XmlRawWriterWrapper : XmlRawWriter
	{
		public XmlRawWriterWrapper(XmlWriter writer)
		{
			this.wrapped = writer;
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return this.wrapped.Settings;
			}
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.wrapped.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.wrapped.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.wrapped.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			this.wrapped.WriteEndAttribute();
		}

		public override void WriteCData(string text)
		{
			this.wrapped.WriteCData(text);
		}

		public override void WriteComment(string text)
		{
			this.wrapped.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.wrapped.WriteProcessingInstruction(name, text);
		}

		public override void WriteWhitespace(string ws)
		{
			this.wrapped.WriteWhitespace(ws);
		}

		public override void WriteString(string text)
		{
			this.wrapped.WriteString(text);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.wrapped.WriteChars(buffer, index, count);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.wrapped.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			this.wrapped.WriteRaw(data);
		}

		public override void WriteEntityRef(string name)
		{
			this.wrapped.WriteEntityRef(name);
		}

		public override void WriteCharEntity(char ch)
		{
			this.wrapped.WriteCharEntity(ch);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.wrapped.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void Close()
		{
			this.wrapped.Close();
		}

		public override void Flush()
		{
			this.wrapped.Flush();
		}

		public override void WriteValue(object value)
		{
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(string value)
		{
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(bool value)
		{
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(DateTime value)
		{
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(float value)
		{
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(decimal value)
		{
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(double value)
		{
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(int value)
		{
			this.wrapped.WriteValue(value);
		}

		public override void WriteValue(long value)
		{
			this.wrapped.WriteValue(value);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					((IDisposable)this.wrapped).Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		internal override void WriteXmlDeclaration(XmlStandalone standalone)
		{
		}

		internal override void WriteXmlDeclaration(string xmldecl)
		{
		}

		internal override void StartElementContent()
		{
		}

		internal override void WriteEndElement(string prefix, string localName, string ns)
		{
			this.wrapped.WriteEndElement();
		}

		internal override void WriteFullEndElement(string prefix, string localName, string ns)
		{
			this.wrapped.WriteFullEndElement();
		}

		internal override void WriteNamespaceDeclaration(string prefix, string ns)
		{
			if (prefix.Length == 0)
			{
				this.wrapped.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", ns);
				return;
			}
			this.wrapped.WriteAttributeString("xmlns", prefix, "http://www.w3.org/2000/xmlns/", ns);
		}

		private XmlWriter wrapped;
	}
}
