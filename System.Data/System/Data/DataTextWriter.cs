using System;
using System.IO;
using System.Xml;

namespace System.Data
{
	internal sealed class DataTextWriter : XmlWriter
	{
		internal static XmlWriter CreateWriter(XmlWriter xw)
		{
			return new DataTextWriter(xw);
		}

		private DataTextWriter(XmlWriter w)
		{
			this._xmltextWriter = w;
		}

		internal Stream BaseStream
		{
			get
			{
				XmlTextWriter xmlTextWriter = this._xmltextWriter as XmlTextWriter;
				if (xmlTextWriter != null)
				{
					return xmlTextWriter.BaseStream;
				}
				return null;
			}
		}

		public override void WriteStartDocument()
		{
			this._xmltextWriter.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			this._xmltextWriter.WriteStartDocument(standalone);
		}

		public override void WriteEndDocument()
		{
			this._xmltextWriter.WriteEndDocument();
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this._xmltextWriter.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this._xmltextWriter.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteEndElement()
		{
			this._xmltextWriter.WriteEndElement();
		}

		public override void WriteFullEndElement()
		{
			this._xmltextWriter.WriteFullEndElement();
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this._xmltextWriter.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			this._xmltextWriter.WriteEndAttribute();
		}

		public override void WriteCData(string text)
		{
			this._xmltextWriter.WriteCData(text);
		}

		public override void WriteComment(string text)
		{
			this._xmltextWriter.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this._xmltextWriter.WriteProcessingInstruction(name, text);
		}

		public override void WriteEntityRef(string name)
		{
			this._xmltextWriter.WriteEntityRef(name);
		}

		public override void WriteCharEntity(char ch)
		{
			this._xmltextWriter.WriteCharEntity(ch);
		}

		public override void WriteWhitespace(string ws)
		{
			this._xmltextWriter.WriteWhitespace(ws);
		}

		public override void WriteString(string text)
		{
			this._xmltextWriter.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this._xmltextWriter.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this._xmltextWriter.WriteChars(buffer, index, count);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this._xmltextWriter.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			this._xmltextWriter.WriteRaw(data);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this._xmltextWriter.WriteBase64(buffer, index, count);
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			this._xmltextWriter.WriteBinHex(buffer, index, count);
		}

		public override WriteState WriteState
		{
			get
			{
				return this._xmltextWriter.WriteState;
			}
		}

		public override void Close()
		{
			this._xmltextWriter.Close();
		}

		public override void Flush()
		{
			this._xmltextWriter.Flush();
		}

		public override void WriteName(string name)
		{
			this._xmltextWriter.WriteName(name);
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			this._xmltextWriter.WriteQualifiedName(localName, ns);
		}

		public override string LookupPrefix(string ns)
		{
			return this._xmltextWriter.LookupPrefix(ns);
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this._xmltextWriter.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this._xmltextWriter.XmlLang;
			}
		}

		public override void WriteNmToken(string name)
		{
			this._xmltextWriter.WriteNmToken(name);
		}

		private XmlWriter _xmltextWriter;
	}
}
