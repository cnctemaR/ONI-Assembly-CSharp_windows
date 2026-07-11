using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class XmlSerializableWriter : XmlWriter
	{
		internal void BeginWrite(XmlWriter xmlWriter, object obj)
		{
			this.depth = 0;
			this.xmlWriter = xmlWriter;
			this.obj = obj;
		}

		internal void EndWrite()
		{
			if (this.depth != 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("IXmlSerializable.WriteXml method of type '{0}' did not close all open tags. Verify that the IXmlSerializable implementation is correct.", new object[] { (this.obj == null) ? string.Empty : DataContract.GetClrTypeFullName(this.obj.GetType()) })));
			}
			this.obj = null;
		}

		public override void WriteStartDocument()
		{
			if (this.WriteState == WriteState.Start)
			{
				this.xmlWriter.WriteStartDocument();
			}
		}

		public override void WriteEndDocument()
		{
			this.xmlWriter.WriteEndDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			if (this.WriteState == WriteState.Start)
			{
				this.xmlWriter.WriteStartDocument(standalone);
			}
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.xmlWriter.WriteStartElement(prefix, localName, ns);
			this.depth++;
		}

		public override void WriteEndElement()
		{
			if (this.depth == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("IXmlSerializable.WriteXml method of type '{0}' attempted to close too many tags.  Verify that the IXmlSerializable implementation is correct.", new object[] { (this.obj == null) ? string.Empty : DataContract.GetClrTypeFullName(this.obj.GetType()) })));
			}
			this.xmlWriter.WriteEndElement();
			this.depth--;
		}

		public override void WriteFullEndElement()
		{
			if (this.depth == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("IXmlSerializable.WriteXml method of type '{0}' attempted to close too many tags.  Verify that the IXmlSerializable implementation is correct.", new object[] { (this.obj == null) ? string.Empty : DataContract.GetClrTypeFullName(this.obj.GetType()) })));
			}
			this.xmlWriter.WriteFullEndElement();
			this.depth--;
		}

		public override void Close()
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("This method cannot be called from IXmlSerializable implementations.")));
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.xmlWriter.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteEndAttribute()
		{
			this.xmlWriter.WriteEndAttribute();
		}

		public override void WriteCData(string text)
		{
			this.xmlWriter.WriteCData(text);
		}

		public override void WriteComment(string text)
		{
			this.xmlWriter.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.xmlWriter.WriteProcessingInstruction(name, text);
		}

		public override void WriteEntityRef(string name)
		{
			this.xmlWriter.WriteEntityRef(name);
		}

		public override void WriteCharEntity(char ch)
		{
			this.xmlWriter.WriteCharEntity(ch);
		}

		public override void WriteWhitespace(string ws)
		{
			this.xmlWriter.WriteWhitespace(ws);
		}

		public override void WriteString(string text)
		{
			this.xmlWriter.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.xmlWriter.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.xmlWriter.WriteChars(buffer, index, count);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.xmlWriter.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			this.xmlWriter.WriteRaw(data);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.xmlWriter.WriteBase64(buffer, index, count);
		}

		public override void WriteBinHex(byte[] buffer, int index, int count)
		{
			this.xmlWriter.WriteBinHex(buffer, index, count);
		}

		public override WriteState WriteState
		{
			get
			{
				return this.xmlWriter.WriteState;
			}
		}

		public override void Flush()
		{
			this.xmlWriter.Flush();
		}

		public override void WriteName(string name)
		{
			this.xmlWriter.WriteName(name);
		}

		public override void WriteQualifiedName(string localName, string ns)
		{
			this.xmlWriter.WriteQualifiedName(localName, ns);
		}

		public override string LookupPrefix(string ns)
		{
			return this.xmlWriter.LookupPrefix(ns);
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.xmlWriter.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.xmlWriter.XmlLang;
			}
		}

		public override void WriteNmToken(string name)
		{
			this.xmlWriter.WriteNmToken(name);
		}

		private XmlWriter xmlWriter;

		private int depth;

		private object obj;
	}
}
