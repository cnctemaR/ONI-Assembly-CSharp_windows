using System;
using System.IO;
using System.Text;

namespace System.Xml
{
	internal class XmlSigningNodeWriter : XmlNodeWriter
	{
		public XmlSigningNodeWriter(bool text)
		{
			this.text = text;
		}

		public void SetOutput(XmlNodeWriter writer, Stream stream, bool includeComments, string[] inclusivePrefixes)
		{
			this.writer = writer;
			if (this.signingWriter == null)
			{
				this.signingWriter = new XmlCanonicalWriter();
			}
			this.signingWriter.SetOutput(stream, includeComments, inclusivePrefixes);
			this.chars = new byte[64];
			this.base64Chars = null;
		}

		public XmlNodeWriter NodeWriter
		{
			get
			{
				return this.writer;
			}
			set
			{
				this.writer = value;
			}
		}

		public XmlCanonicalWriter CanonicalWriter
		{
			get
			{
				return this.signingWriter;
			}
		}

		public override void Flush()
		{
			this.writer.Flush();
			this.signingWriter.Flush();
		}

		public override void Close()
		{
			this.writer.Close();
			this.signingWriter.Close();
		}

		public override void WriteDeclaration()
		{
			this.writer.WriteDeclaration();
			this.signingWriter.WriteDeclaration();
		}

		public override void WriteComment(string text)
		{
			this.writer.WriteComment(text);
			this.signingWriter.WriteComment(text);
		}

		public override void WriteCData(string text)
		{
			this.writer.WriteCData(text);
			this.signingWriter.WriteEscapedText(text);
		}

		public override void WriteStartElement(string prefix, string localName)
		{
			this.writer.WriteStartElement(prefix, localName);
			this.signingWriter.WriteStartElement(prefix, localName);
		}

		public override void WriteStartElement(byte[] prefixBuffer, int prefixOffset, int prefixLength, byte[] localNameBuffer, int localNameOffset, int localNameLength)
		{
			this.writer.WriteStartElement(prefixBuffer, prefixOffset, prefixLength, localNameBuffer, localNameOffset, localNameLength);
			this.signingWriter.WriteStartElement(prefixBuffer, prefixOffset, prefixLength, localNameBuffer, localNameOffset, localNameLength);
		}

		public override void WriteStartElement(string prefix, XmlDictionaryString localName)
		{
			this.writer.WriteStartElement(prefix, localName);
			this.signingWriter.WriteStartElement(prefix, localName.Value);
		}

		public override void WriteEndStartElement(bool isEmpty)
		{
			this.writer.WriteEndStartElement(isEmpty);
			this.signingWriter.WriteEndStartElement(isEmpty);
		}

		public override void WriteEndElement(string prefix, string localName)
		{
			this.writer.WriteEndElement(prefix, localName);
			this.signingWriter.WriteEndElement(prefix, localName);
		}

		public override void WriteXmlnsAttribute(string prefix, string ns)
		{
			this.writer.WriteXmlnsAttribute(prefix, ns);
			this.signingWriter.WriteXmlnsAttribute(prefix, ns);
		}

		public override void WriteXmlnsAttribute(byte[] prefixBuffer, int prefixOffset, int prefixLength, byte[] nsBuffer, int nsOffset, int nsLength)
		{
			this.writer.WriteXmlnsAttribute(prefixBuffer, prefixOffset, prefixLength, nsBuffer, nsOffset, nsLength);
			this.signingWriter.WriteXmlnsAttribute(prefixBuffer, prefixOffset, prefixLength, nsBuffer, nsOffset, nsLength);
		}

		public override void WriteXmlnsAttribute(string prefix, XmlDictionaryString ns)
		{
			this.writer.WriteXmlnsAttribute(prefix, ns);
			this.signingWriter.WriteXmlnsAttribute(prefix, ns.Value);
		}

		public override void WriteStartAttribute(string prefix, string localName)
		{
			this.writer.WriteStartAttribute(prefix, localName);
			this.signingWriter.WriteStartAttribute(prefix, localName);
		}

		public override void WriteStartAttribute(byte[] prefixBuffer, int prefixOffset, int prefixLength, byte[] localNameBuffer, int localNameOffset, int localNameLength)
		{
			this.writer.WriteStartAttribute(prefixBuffer, prefixOffset, prefixLength, localNameBuffer, localNameOffset, localNameLength);
			this.signingWriter.WriteStartAttribute(prefixBuffer, prefixOffset, prefixLength, localNameBuffer, localNameOffset, localNameLength);
		}

		public override void WriteStartAttribute(string prefix, XmlDictionaryString localName)
		{
			this.writer.WriteStartAttribute(prefix, localName);
			this.signingWriter.WriteStartAttribute(prefix, localName.Value);
		}

		public override void WriteEndAttribute()
		{
			this.writer.WriteEndAttribute();
			this.signingWriter.WriteEndAttribute();
		}

		public override void WriteCharEntity(int ch)
		{
			this.writer.WriteCharEntity(ch);
			this.signingWriter.WriteCharEntity(ch);
		}

		public override void WriteEscapedText(string value)
		{
			this.writer.WriteEscapedText(value);
			this.signingWriter.WriteEscapedText(value);
		}

		public override void WriteEscapedText(char[] chars, int offset, int count)
		{
			this.writer.WriteEscapedText(chars, offset, count);
			this.signingWriter.WriteEscapedText(chars, offset, count);
		}

		public override void WriteEscapedText(XmlDictionaryString value)
		{
			this.writer.WriteEscapedText(value);
			this.signingWriter.WriteEscapedText(value.Value);
		}

		public override void WriteEscapedText(byte[] chars, int offset, int count)
		{
			this.writer.WriteEscapedText(chars, offset, count);
			this.signingWriter.WriteEscapedText(chars, offset, count);
		}

		public override void WriteText(string value)
		{
			this.writer.WriteText(value);
			this.signingWriter.WriteText(value);
		}

		public override void WriteText(char[] chars, int offset, int count)
		{
			this.writer.WriteText(chars, offset, count);
			this.signingWriter.WriteText(chars, offset, count);
		}

		public override void WriteText(byte[] chars, int offset, int count)
		{
			this.writer.WriteText(chars, offset, count);
			this.signingWriter.WriteText(chars, offset, count);
		}

		public override void WriteText(XmlDictionaryString value)
		{
			this.writer.WriteText(value);
			this.signingWriter.WriteText(value.Value);
		}

		public override void WriteInt32Text(int value)
		{
			int num = XmlConverter.ToChars(value, this.chars, 0);
			if (this.text)
			{
				this.writer.WriteText(this.chars, 0, num);
			}
			else
			{
				this.writer.WriteInt32Text(value);
			}
			this.signingWriter.WriteText(this.chars, 0, num);
		}

		public override void WriteInt64Text(long value)
		{
			int num = XmlConverter.ToChars(value, this.chars, 0);
			if (this.text)
			{
				this.writer.WriteText(this.chars, 0, num);
			}
			else
			{
				this.writer.WriteInt64Text(value);
			}
			this.signingWriter.WriteText(this.chars, 0, num);
		}

		public override void WriteBoolText(bool value)
		{
			int num = XmlConverter.ToChars(value, this.chars, 0);
			if (this.text)
			{
				this.writer.WriteText(this.chars, 0, num);
			}
			else
			{
				this.writer.WriteBoolText(value);
			}
			this.signingWriter.WriteText(this.chars, 0, num);
		}

		public override void WriteUInt64Text(ulong value)
		{
			int num = XmlConverter.ToChars(value, this.chars, 0);
			if (this.text)
			{
				this.writer.WriteText(this.chars, 0, num);
			}
			else
			{
				this.writer.WriteUInt64Text(value);
			}
			this.signingWriter.WriteText(this.chars, 0, num);
		}

		public override void WriteFloatText(float value)
		{
			int num = XmlConverter.ToChars(value, this.chars, 0);
			if (this.text)
			{
				this.writer.WriteText(this.chars, 0, num);
			}
			else
			{
				this.writer.WriteFloatText(value);
			}
			this.signingWriter.WriteText(this.chars, 0, num);
		}

		public override void WriteDoubleText(double value)
		{
			int num = XmlConverter.ToChars(value, this.chars, 0);
			if (this.text)
			{
				this.writer.WriteText(this.chars, 0, num);
			}
			else
			{
				this.writer.WriteDoubleText(value);
			}
			this.signingWriter.WriteText(this.chars, 0, num);
		}

		public override void WriteDecimalText(decimal value)
		{
			int num = XmlConverter.ToChars(value, this.chars, 0);
			if (this.text)
			{
				this.writer.WriteText(this.chars, 0, num);
			}
			else
			{
				this.writer.WriteDecimalText(value);
			}
			this.signingWriter.WriteText(this.chars, 0, num);
		}

		public override void WriteDateTimeText(DateTime value)
		{
			int num = XmlConverter.ToChars(value, this.chars, 0);
			if (this.text)
			{
				this.writer.WriteText(this.chars, 0, num);
			}
			else
			{
				this.writer.WriteDateTimeText(value);
			}
			this.signingWriter.WriteText(this.chars, 0, num);
		}

		public override void WriteUniqueIdText(UniqueId value)
		{
			string text = XmlConverter.ToString(value);
			if (this.text)
			{
				this.writer.WriteText(text);
			}
			else
			{
				this.writer.WriteUniqueIdText(value);
			}
			this.signingWriter.WriteText(text);
		}

		public override void WriteTimeSpanText(TimeSpan value)
		{
			string text = XmlConverter.ToString(value);
			if (this.text)
			{
				this.writer.WriteText(text);
			}
			else
			{
				this.writer.WriteTimeSpanText(value);
			}
			this.signingWriter.WriteText(text);
		}

		public override void WriteGuidText(Guid value)
		{
			string text = XmlConverter.ToString(value);
			if (this.text)
			{
				this.writer.WriteText(text);
			}
			else
			{
				this.writer.WriteGuidText(value);
			}
			this.signingWriter.WriteText(text);
		}

		public override void WriteStartListText()
		{
			this.writer.WriteStartListText();
		}

		public override void WriteListSeparator()
		{
			this.writer.WriteListSeparator();
			this.signingWriter.WriteText(32);
		}

		public override void WriteEndListText()
		{
			this.writer.WriteEndListText();
		}

		public override void WriteBase64Text(byte[] trailBytes, int trailByteCount, byte[] buffer, int offset, int count)
		{
			if (trailByteCount > 0)
			{
				this.WriteBase64Text(trailBytes, 0, trailByteCount);
			}
			this.WriteBase64Text(buffer, offset, count);
			if (!this.text)
			{
				this.writer.WriteBase64Text(trailBytes, trailByteCount, buffer, offset, count);
			}
		}

		private void WriteBase64Text(byte[] buffer, int offset, int count)
		{
			if (this.base64Chars == null)
			{
				this.base64Chars = new byte[512];
			}
			Base64Encoding base64Encoding = XmlConverter.Base64Encoding;
			while (count >= 3)
			{
				int num = Math.Min(this.base64Chars.Length / 4 * 3, count - count % 3);
				int num2 = num / 3 * 4;
				base64Encoding.GetChars(buffer, offset, num, this.base64Chars, 0);
				this.signingWriter.WriteText(this.base64Chars, 0, num2);
				if (this.text)
				{
					this.writer.WriteText(this.base64Chars, 0, num2);
				}
				offset += num;
				count -= num;
			}
			if (count > 0)
			{
				base64Encoding.GetChars(buffer, offset, count, this.base64Chars, 0);
				this.signingWriter.WriteText(this.base64Chars, 0, 4);
				if (this.text)
				{
					this.writer.WriteText(this.base64Chars, 0, 4);
				}
			}
		}

		public override void WriteQualifiedName(string prefix, XmlDictionaryString localName)
		{
			this.writer.WriteQualifiedName(prefix, localName);
			if (prefix.Length != 0)
			{
				this.signingWriter.WriteText(prefix);
				this.signingWriter.WriteText(":");
			}
			this.signingWriter.WriteText(localName.Value);
		}

		private XmlNodeWriter writer;

		private XmlCanonicalWriter signingWriter;

		private byte[] chars;

		private byte[] base64Chars;

		private bool text;
	}
}
