using System;

namespace System.Xml
{
	internal class XmlSimpleDictionaryWriter : XmlDictionaryWriter
	{
		public XmlSimpleDictionaryWriter(XmlWriter writer)
		{
			this.writer = writer;
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

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.writer.WriteBase64(buffer, index, count);
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			this.writer.WriteBinHex(buffer, index, count);
		}

		public override void WriteCData(string text)
		{
			this.writer.WriteCData(text);
		}

		public override void WriteCharEntity(char ch)
		{
			this.writer.WriteCharEntity(ch);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.writer.WriteChars(buffer, index, count);
		}

		public override void WriteComment(string text)
		{
			this.writer.WriteComment(text);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.writer.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteEndAttribute()
		{
			this.writer.WriteEndAttribute();
		}

		public override void WriteEndDocument()
		{
			this.writer.WriteEndDocument();
		}

		public override void WriteEndElement()
		{
			base.Depth--;
			this.writer.WriteEndElement();
		}

		public override void WriteEntityRef(string name)
		{
			this.writer.WriteEntityRef(name);
		}

		public override void WriteFullEndElement()
		{
			this.writer.WriteFullEndElement();
		}

		public override void WriteName(string name)
		{
			this.writer.WriteName(name);
		}

		public override void WriteNmToken(string name)
		{
			this.writer.WriteNmToken(name);
		}

		public override void WriteNode(XmlReader reader, bool defattr)
		{
			this.writer.WriteNode(reader, defattr);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.writer.WriteProcessingInstruction(name, text);
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			this.writer.WriteQualifiedName(localName, ns);
		}

		public override void WriteRaw(string data)
		{
			this.writer.WriteRaw(data);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.writer.WriteRaw(buffer, index, count);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.writer.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.writer.WriteStartDocument(standalone);
		}

		public override void WriteStartDocument()
		{
			this.writer.WriteStartDocument();
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			base.Depth++;
			this.writer.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteString(string text)
		{
			this.writer.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.writer.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteWhitespace(string ws)
		{
			this.writer.WriteWhitespace(ws);
		}

		public override WriteState WriteState
		{
			get
			{
				return this.writer.WriteState;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.writer.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.writer.XmlSpace;
			}
		}

		private XmlWriter writer;
	}
}
