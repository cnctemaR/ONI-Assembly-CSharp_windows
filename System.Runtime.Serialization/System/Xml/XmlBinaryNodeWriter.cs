using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace System.Xml
{
	internal class XmlBinaryNodeWriter : XmlStreamNodeWriter
	{
		public void SetOutput(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session, bool ownsStream)
		{
			this.dictionary = dictionary;
			this.session = session;
			this.inAttribute = false;
			this.inList = false;
			this.attributeValue.Clear();
			this.textNodeOffset = -1;
			base.SetOutput(stream, ownsStream, null);
		}

		private void WriteNode(XmlBinaryNodeType nodeType)
		{
			base.WriteByte((byte)nodeType);
			this.textNodeOffset = -1;
		}

		private void WroteAttributeValue()
		{
			if (this.wroteAttributeValue && !this.inList)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Only a single typed value may be written inside an attribute or content.")));
			}
			this.wroteAttributeValue = true;
		}

		private void WriteTextNode(XmlBinaryNodeType nodeType)
		{
			if (this.inAttribute)
			{
				this.WroteAttributeValue();
			}
			base.WriteByte((byte)nodeType);
			this.textNodeOffset = base.BufferOffset - 1;
		}

		private byte[] GetTextNodeBuffer(int size, out int offset)
		{
			if (this.inAttribute)
			{
				this.WroteAttributeValue();
			}
			byte[] buffer = base.GetBuffer(size, out offset);
			this.textNodeOffset = offset;
			return buffer;
		}

		private void WriteTextNodeWithLength(XmlBinaryNodeType nodeType, int length)
		{
			int num;
			byte[] textNodeBuffer = this.GetTextNodeBuffer(5, out num);
			if (length < 256)
			{
				textNodeBuffer[num] = (byte)nodeType;
				textNodeBuffer[num + 1] = (byte)length;
				base.Advance(2);
				return;
			}
			if (length < 65536)
			{
				textNodeBuffer[num] = (byte)(nodeType + 2);
				textNodeBuffer[num + 1] = (byte)length;
				length >>= 8;
				textNodeBuffer[num + 2] = (byte)length;
				base.Advance(3);
				return;
			}
			textNodeBuffer[num] = (byte)(nodeType + 4);
			textNodeBuffer[num + 1] = (byte)length;
			length >>= 8;
			textNodeBuffer[num + 2] = (byte)length;
			length >>= 8;
			textNodeBuffer[num + 3] = (byte)length;
			length >>= 8;
			textNodeBuffer[num + 4] = (byte)length;
			base.Advance(5);
		}

		private void WriteTextNodeWithInt64(XmlBinaryNodeType nodeType, long value)
		{
			int num;
			byte[] textNodeBuffer = this.GetTextNodeBuffer(9, out num);
			textNodeBuffer[num] = (byte)nodeType;
			textNodeBuffer[num + 1] = (byte)value;
			value >>= 8;
			textNodeBuffer[num + 2] = (byte)value;
			value >>= 8;
			textNodeBuffer[num + 3] = (byte)value;
			value >>= 8;
			textNodeBuffer[num + 4] = (byte)value;
			value >>= 8;
			textNodeBuffer[num + 5] = (byte)value;
			value >>= 8;
			textNodeBuffer[num + 6] = (byte)value;
			value >>= 8;
			textNodeBuffer[num + 7] = (byte)value;
			value >>= 8;
			textNodeBuffer[num + 8] = (byte)value;
			base.Advance(9);
		}

		public override void WriteDeclaration()
		{
		}

		public override void WriteStartElement(string prefix, string localName)
		{
			if (prefix.Length == 0)
			{
				this.WriteNode(XmlBinaryNodeType.MinElement);
				this.WriteName(localName);
				return;
			}
			char c = prefix[0];
			if (prefix.Length == 1 && c >= 'a' && c <= 'z')
			{
				this.WritePrefixNode(XmlBinaryNodeType.PrefixElementA, (int)(c - 'a'));
				this.WriteName(localName);
				return;
			}
			this.WriteNode(XmlBinaryNodeType.Element);
			this.WriteName(prefix);
			this.WriteName(localName);
		}

		private void WritePrefixNode(XmlBinaryNodeType nodeType, int ch)
		{
			this.WriteNode(nodeType + ch);
		}

		public override void WriteStartElement(string prefix, XmlDictionaryString localName)
		{
			int num;
			if (!this.TryGetKey(localName, out num))
			{
				this.WriteStartElement(prefix, localName.Value);
				return;
			}
			if (prefix.Length == 0)
			{
				this.WriteNode(XmlBinaryNodeType.ShortDictionaryElement);
				this.WriteDictionaryString(localName, num);
				return;
			}
			char c = prefix[0];
			if (prefix.Length == 1 && c >= 'a' && c <= 'z')
			{
				this.WritePrefixNode(XmlBinaryNodeType.PrefixDictionaryElementA, (int)(c - 'a'));
				this.WriteDictionaryString(localName, num);
				return;
			}
			this.WriteNode(XmlBinaryNodeType.DictionaryElement);
			this.WriteName(prefix);
			this.WriteDictionaryString(localName, num);
		}

		public override void WriteEndStartElement(bool isEmpty)
		{
			if (isEmpty)
			{
				this.WriteEndElement();
			}
		}

		public override void WriteEndElement(string prefix, string localName)
		{
			this.WriteEndElement();
		}

		private void WriteEndElement()
		{
			if (this.textNodeOffset != -1)
			{
				byte[] streamBuffer = base.StreamBuffer;
				XmlBinaryNodeType xmlBinaryNodeType = (XmlBinaryNodeType)streamBuffer[this.textNodeOffset];
				streamBuffer[this.textNodeOffset] = (byte)(xmlBinaryNodeType + 1);
				this.textNodeOffset = -1;
				return;
			}
			this.WriteNode(XmlBinaryNodeType.EndElement);
		}

		public override void WriteStartAttribute(string prefix, string localName)
		{
			if (prefix.Length == 0)
			{
				this.WriteNode(XmlBinaryNodeType.MinAttribute);
				this.WriteName(localName);
			}
			else
			{
				char c = prefix[0];
				if (prefix.Length == 1 && c >= 'a' && c <= 'z')
				{
					this.WritePrefixNode(XmlBinaryNodeType.PrefixAttributeA, (int)(c - 'a'));
					this.WriteName(localName);
				}
				else
				{
					this.WriteNode(XmlBinaryNodeType.Attribute);
					this.WriteName(prefix);
					this.WriteName(localName);
				}
			}
			this.inAttribute = true;
			this.wroteAttributeValue = false;
		}

		public override void WriteStartAttribute(string prefix, XmlDictionaryString localName)
		{
			int num;
			if (!this.TryGetKey(localName, out num))
			{
				this.WriteStartAttribute(prefix, localName.Value);
				return;
			}
			if (prefix.Length == 0)
			{
				this.WriteNode(XmlBinaryNodeType.ShortDictionaryAttribute);
				this.WriteDictionaryString(localName, num);
			}
			else
			{
				char c = prefix[0];
				if (prefix.Length == 1 && c >= 'a' && c <= 'z')
				{
					this.WritePrefixNode(XmlBinaryNodeType.PrefixDictionaryAttributeA, (int)(c - 'a'));
					this.WriteDictionaryString(localName, num);
				}
				else
				{
					this.WriteNode(XmlBinaryNodeType.DictionaryAttribute);
					this.WriteName(prefix);
					this.WriteDictionaryString(localName, num);
				}
			}
			this.inAttribute = true;
			this.wroteAttributeValue = false;
		}

		public override void WriteEndAttribute()
		{
			this.inAttribute = false;
			if (!this.wroteAttributeValue)
			{
				this.attributeValue.WriteTo(this);
			}
			this.textNodeOffset = -1;
		}

		public override void WriteXmlnsAttribute(string prefix, string ns)
		{
			if (prefix.Length == 0)
			{
				this.WriteNode(XmlBinaryNodeType.ShortXmlnsAttribute);
				this.WriteName(ns);
				return;
			}
			this.WriteNode(XmlBinaryNodeType.XmlnsAttribute);
			this.WriteName(prefix);
			this.WriteName(ns);
		}

		public override void WriteXmlnsAttribute(string prefix, XmlDictionaryString ns)
		{
			int num;
			if (!this.TryGetKey(ns, out num))
			{
				this.WriteXmlnsAttribute(prefix, ns.Value);
				return;
			}
			if (prefix.Length == 0)
			{
				this.WriteNode(XmlBinaryNodeType.ShortDictionaryXmlnsAttribute);
				this.WriteDictionaryString(ns, num);
				return;
			}
			this.WriteNode(XmlBinaryNodeType.DictionaryXmlnsAttribute);
			this.WriteName(prefix);
			this.WriteDictionaryString(ns, num);
		}

		private bool TryGetKey(XmlDictionaryString s, out int key)
		{
			key = -1;
			if (s.Dictionary == this.dictionary)
			{
				key = s.Key * 2;
				return true;
			}
			XmlDictionaryString xmlDictionaryString;
			if (this.dictionary != null && this.dictionary.TryLookup(s, out xmlDictionaryString))
			{
				key = xmlDictionaryString.Key * 2;
				return true;
			}
			if (this.session == null)
			{
				return false;
			}
			int num;
			if (!this.session.TryLookup(s, out num) && !this.session.TryAdd(s, out num))
			{
				return false;
			}
			key = num * 2 + 1;
			return true;
		}

		private void WriteDictionaryString(XmlDictionaryString s, int key)
		{
			this.WriteMultiByteInt32(key);
		}

		[SecuritySafeCritical]
		private unsafe void WriteName(string s)
		{
			int length = s.Length;
			if (length == 0)
			{
				base.WriteByte(0);
				return;
			}
			fixed (string text = s)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				this.UnsafeWriteName(ptr, length);
			}
		}

		[SecurityCritical]
		private unsafe void UnsafeWriteName(char* chars, int charCount)
		{
			if (charCount < 42)
			{
				int num;
				byte[] buffer = base.GetBuffer(1 + charCount * 3, out num);
				int num2 = base.UnsafeGetUTF8Chars(chars, charCount, buffer, num + 1);
				buffer[num] = (byte)num2;
				base.Advance(1 + num2);
				return;
			}
			int num3 = base.UnsafeGetUTF8Length(chars, charCount);
			this.WriteMultiByteInt32(num3);
			base.UnsafeWriteUTF8Chars(chars, charCount);
		}

		private void WriteMultiByteInt32(int i)
		{
			int num;
			byte[] buffer = base.GetBuffer(5, out num);
			int num2 = num;
			while (((long)i & (long)((ulong)(-128))) != 0L)
			{
				buffer[num++] = (byte)((i & 127) | 128);
				i >>= 7;
			}
			buffer[num++] = (byte)i;
			base.Advance(num - num2);
		}

		public override void WriteComment(string value)
		{
			this.WriteNode(XmlBinaryNodeType.Comment);
			this.WriteName(value);
		}

		public override void WriteCData(string value)
		{
			this.WriteText(value);
		}

		private void WriteEmptyText()
		{
			this.WriteTextNode(XmlBinaryNodeType.EmptyText);
		}

		public override void WriteBoolText(bool value)
		{
			if (value)
			{
				this.WriteTextNode(XmlBinaryNodeType.TrueText);
				return;
			}
			this.WriteTextNode(XmlBinaryNodeType.FalseText);
		}

		public override void WriteInt32Text(int value)
		{
			if (value >= -128 && value < 128)
			{
				if (value == 0)
				{
					this.WriteTextNode(XmlBinaryNodeType.MinText);
					return;
				}
				if (value == 1)
				{
					this.WriteTextNode(XmlBinaryNodeType.OneText);
					return;
				}
				int num;
				byte[] textNodeBuffer = this.GetTextNodeBuffer(2, out num);
				textNodeBuffer[num] = 136;
				textNodeBuffer[num + 1] = (byte)value;
				base.Advance(2);
				return;
			}
			else
			{
				if (value >= -32768 && value < 32768)
				{
					int num2;
					byte[] textNodeBuffer2 = this.GetTextNodeBuffer(3, out num2);
					textNodeBuffer2[num2] = 138;
					textNodeBuffer2[num2 + 1] = (byte)value;
					value >>= 8;
					textNodeBuffer2[num2 + 2] = (byte)value;
					base.Advance(3);
					return;
				}
				int num3;
				byte[] textNodeBuffer3 = this.GetTextNodeBuffer(5, out num3);
				textNodeBuffer3[num3] = 140;
				textNodeBuffer3[num3 + 1] = (byte)value;
				value >>= 8;
				textNodeBuffer3[num3 + 2] = (byte)value;
				value >>= 8;
				textNodeBuffer3[num3 + 3] = (byte)value;
				value >>= 8;
				textNodeBuffer3[num3 + 4] = (byte)value;
				base.Advance(5);
				return;
			}
		}

		public override void WriteInt64Text(long value)
		{
			if (value >= -2147483648L && value <= 2147483647L)
			{
				this.WriteInt32Text((int)value);
				return;
			}
			this.WriteTextNodeWithInt64(XmlBinaryNodeType.Int64Text, value);
		}

		public override void WriteUInt64Text(ulong value)
		{
			if (value <= 9223372036854775807UL)
			{
				this.WriteInt64Text((long)value);
				return;
			}
			this.WriteTextNodeWithInt64(XmlBinaryNodeType.UInt64Text, (long)value);
		}

		private void WriteInt64(long value)
		{
			int num;
			byte[] buffer = base.GetBuffer(8, out num);
			buffer[num] = (byte)value;
			value >>= 8;
			buffer[num + 1] = (byte)value;
			value >>= 8;
			buffer[num + 2] = (byte)value;
			value >>= 8;
			buffer[num + 3] = (byte)value;
			value >>= 8;
			buffer[num + 4] = (byte)value;
			value >>= 8;
			buffer[num + 5] = (byte)value;
			value >>= 8;
			buffer[num + 6] = (byte)value;
			value >>= 8;
			buffer[num + 7] = (byte)value;
			base.Advance(8);
		}

		public override void WriteBase64Text(byte[] trailBytes, int trailByteCount, byte[] base64Buffer, int base64Offset, int base64Count)
		{
			if (this.inAttribute)
			{
				this.attributeValue.WriteBase64Text(trailBytes, trailByteCount, base64Buffer, base64Offset, base64Count);
				return;
			}
			int num = trailByteCount + base64Count;
			if (num > 0)
			{
				this.WriteTextNodeWithLength(XmlBinaryNodeType.Bytes8Text, num);
				if (trailByteCount > 0)
				{
					int num2;
					byte[] buffer = base.GetBuffer(trailByteCount, out num2);
					for (int i = 0; i < trailByteCount; i++)
					{
						buffer[num2 + i] = trailBytes[i];
					}
					base.Advance(trailByteCount);
				}
				if (base64Count > 0)
				{
					base.WriteBytes(base64Buffer, base64Offset, base64Count);
					return;
				}
			}
			else
			{
				this.WriteEmptyText();
			}
		}

		public override void WriteText(XmlDictionaryString value)
		{
			if (this.inAttribute)
			{
				this.attributeValue.WriteText(value);
				return;
			}
			int num;
			if (!this.TryGetKey(value, out num))
			{
				this.WriteText(value.Value);
				return;
			}
			this.WriteTextNode(XmlBinaryNodeType.DictionaryText);
			this.WriteDictionaryString(value, num);
		}

		[SecuritySafeCritical]
		public unsafe override void WriteText(string value)
		{
			if (this.inAttribute)
			{
				this.attributeValue.WriteText(value);
				return;
			}
			if (value.Length > 0)
			{
				fixed (string text = value)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					this.UnsafeWriteText(ptr, value.Length);
				}
				return;
			}
			this.WriteEmptyText();
		}

		[SecuritySafeCritical]
		public unsafe override void WriteText(char[] chars, int offset, int count)
		{
			if (this.inAttribute)
			{
				this.attributeValue.WriteText(new string(chars, offset, count));
				return;
			}
			if (count > 0)
			{
				fixed (char* ptr = &chars[offset])
				{
					char* ptr2 = ptr;
					this.UnsafeWriteText(ptr2, count);
				}
				return;
			}
			this.WriteEmptyText();
		}

		public override void WriteText(byte[] chars, int charOffset, int charCount)
		{
			this.WriteTextNodeWithLength(XmlBinaryNodeType.Chars8Text, charCount);
			base.WriteBytes(chars, charOffset, charCount);
		}

		[SecurityCritical]
		private unsafe void UnsafeWriteText(char* chars, int charCount)
		{
			if (charCount == 1)
			{
				char c = *chars;
				if (c == '0')
				{
					this.WriteTextNode(XmlBinaryNodeType.MinText);
					return;
				}
				if (c == '1')
				{
					this.WriteTextNode(XmlBinaryNodeType.OneText);
					return;
				}
			}
			if (charCount <= 85)
			{
				int num;
				byte[] buffer = base.GetBuffer(2 + charCount * 3, out num);
				int num2 = base.UnsafeGetUTF8Chars(chars, charCount, buffer, num + 2);
				if (num2 / 2 <= charCount)
				{
					buffer[num] = 152;
				}
				else
				{
					buffer[num] = 182;
					num2 = base.UnsafeGetUnicodeChars(chars, charCount, buffer, num + 2);
				}
				this.textNodeOffset = num;
				buffer[num + 1] = (byte)num2;
				base.Advance(2 + num2);
				return;
			}
			int num3 = base.UnsafeGetUTF8Length(chars, charCount);
			if (num3 / 2 > charCount)
			{
				this.WriteTextNodeWithLength(XmlBinaryNodeType.UnicodeChars8Text, charCount * 2);
				base.UnsafeWriteUnicodeChars(chars, charCount);
				return;
			}
			this.WriteTextNodeWithLength(XmlBinaryNodeType.Chars8Text, num3);
			base.UnsafeWriteUTF8Chars(chars, charCount);
		}

		public override void WriteEscapedText(string value)
		{
			this.WriteText(value);
		}

		public override void WriteEscapedText(XmlDictionaryString value)
		{
			this.WriteText(value);
		}

		public override void WriteEscapedText(char[] chars, int offset, int count)
		{
			this.WriteText(chars, offset, count);
		}

		public override void WriteEscapedText(byte[] chars, int offset, int count)
		{
			this.WriteText(chars, offset, count);
		}

		public override void WriteCharEntity(int ch)
		{
			if (ch > 65535)
			{
				SurrogateChar surrogateChar = new SurrogateChar(ch);
				char[] array = new char[] { surrogateChar.HighChar, surrogateChar.LowChar };
				this.WriteText(array, 0, 2);
				return;
			}
			char[] array2 = new char[] { (char)ch };
			this.WriteText(array2, 0, 1);
		}

		[SecuritySafeCritical]
		public unsafe override void WriteFloatText(float f)
		{
			long num;
			if (f >= -9.223372E+18f && f <= 9.223372E+18f && (float)(num = (long)f) == f)
			{
				this.WriteInt64Text(num);
				return;
			}
			int num2;
			byte[] textNodeBuffer = this.GetTextNodeBuffer(5, out num2);
			byte* ptr = (byte*)(&f);
			textNodeBuffer[num2] = 144;
			textNodeBuffer[num2 + 1] = *ptr;
			textNodeBuffer[num2 + 2] = ptr[1];
			textNodeBuffer[num2 + 3] = ptr[2];
			textNodeBuffer[num2 + 4] = ptr[3];
			base.Advance(5);
		}

		[SecuritySafeCritical]
		public unsafe override void WriteDoubleText(double d)
		{
			float num;
			if (d >= -3.4028234663852886E+38 && d <= 3.4028234663852886E+38 && (double)(num = (float)d) == d)
			{
				this.WriteFloatText(num);
				return;
			}
			int num2;
			byte[] textNodeBuffer = this.GetTextNodeBuffer(9, out num2);
			byte* ptr = (byte*)(&d);
			textNodeBuffer[num2] = 146;
			textNodeBuffer[num2 + 1] = *ptr;
			textNodeBuffer[num2 + 2] = ptr[1];
			textNodeBuffer[num2 + 3] = ptr[2];
			textNodeBuffer[num2 + 4] = ptr[3];
			textNodeBuffer[num2 + 5] = ptr[4];
			textNodeBuffer[num2 + 6] = ptr[5];
			textNodeBuffer[num2 + 7] = ptr[6];
			textNodeBuffer[num2 + 8] = ptr[7];
			base.Advance(9);
		}

		[SecuritySafeCritical]
		public unsafe override void WriteDecimalText(decimal d)
		{
			int num;
			byte[] textNodeBuffer = this.GetTextNodeBuffer(17, out num);
			byte* ptr = (byte*)(&d);
			textNodeBuffer[num++] = 148;
			for (int i = 0; i < 16; i++)
			{
				textNodeBuffer[num++] = ptr[i];
			}
			base.Advance(17);
		}

		public override void WriteDateTimeText(DateTime dt)
		{
			this.WriteTextNodeWithInt64(XmlBinaryNodeType.DateTimeText, dt.ToBinary());
		}

		public override void WriteUniqueIdText(UniqueId value)
		{
			if (value.IsGuid)
			{
				int num;
				byte[] textNodeBuffer = this.GetTextNodeBuffer(17, out num);
				textNodeBuffer[num] = 172;
				value.TryGetGuid(textNodeBuffer, num + 1);
				base.Advance(17);
				return;
			}
			this.WriteText(value.ToString());
		}

		public override void WriteGuidText(Guid guid)
		{
			int num;
			byte[] textNodeBuffer = this.GetTextNodeBuffer(17, out num);
			textNodeBuffer[num] = 176;
			Buffer.BlockCopy(guid.ToByteArray(), 0, textNodeBuffer, num + 1, 16);
			base.Advance(17);
		}

		public override void WriteTimeSpanText(TimeSpan value)
		{
			this.WriteTextNodeWithInt64(XmlBinaryNodeType.TimeSpanText, value.Ticks);
		}

		public override void WriteStartListText()
		{
			this.inList = true;
			this.WriteNode(XmlBinaryNodeType.StartListText);
		}

		public override void WriteListSeparator()
		{
		}

		public override void WriteEndListText()
		{
			this.inList = false;
			this.wroteAttributeValue = true;
			this.WriteNode(XmlBinaryNodeType.EndListText);
		}

		public void WriteArrayNode()
		{
			this.WriteNode(XmlBinaryNodeType.Array);
		}

		private void WriteArrayInfo(XmlBinaryNodeType nodeType, int count)
		{
			this.WriteNode(nodeType);
			this.WriteMultiByteInt32(count);
		}

		[SecurityCritical]
		public unsafe void UnsafeWriteArray(XmlBinaryNodeType nodeType, int count, byte* array, byte* arrayMax)
		{
			this.WriteArrayInfo(nodeType, count);
			this.UnsafeWriteArray(array, (int)((long)(arrayMax - array)));
		}

		[SecurityCritical]
		private unsafe void UnsafeWriteArray(byte* array, int byteCount)
		{
			base.UnsafeWriteBytes(array, byteCount);
		}

		public void WriteDateTimeArray(DateTime[] array, int offset, int count)
		{
			this.WriteArrayInfo(XmlBinaryNodeType.DateTimeTextWithEndElement, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteInt64(array[offset + i].ToBinary());
			}
		}

		public void WriteGuidArray(Guid[] array, int offset, int count)
		{
			this.WriteArrayInfo(XmlBinaryNodeType.GuidTextWithEndElement, count);
			for (int i = 0; i < count; i++)
			{
				byte[] array2 = array[offset + i].ToByteArray();
				base.WriteBytes(array2, 0, 16);
			}
		}

		public void WriteTimeSpanArray(TimeSpan[] array, int offset, int count)
		{
			this.WriteArrayInfo(XmlBinaryNodeType.TimeSpanTextWithEndElement, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteInt64(array[offset + i].Ticks);
			}
		}

		public override void WriteQualifiedName(string prefix, XmlDictionaryString localName)
		{
			if (prefix.Length == 0)
			{
				this.WriteText(localName);
				return;
			}
			char c = prefix[0];
			int num;
			if (prefix.Length == 1 && c >= 'a' && c <= 'z' && this.TryGetKey(localName, out num))
			{
				this.WriteTextNode(XmlBinaryNodeType.QNameDictionaryText);
				base.WriteByte((byte)(c - 'a'));
				this.WriteDictionaryString(localName, num);
				return;
			}
			this.WriteText(prefix);
			this.WriteText(":");
			this.WriteText(localName);
		}

		protected override void FlushBuffer()
		{
			base.FlushBuffer();
			this.textNodeOffset = -1;
		}

		public override void Close()
		{
			base.Close();
			this.attributeValue.Clear();
		}

		private IXmlDictionary dictionary;

		private XmlBinaryWriterSession session;

		private bool inAttribute;

		private bool inList;

		private bool wroteAttributeValue;

		private XmlBinaryNodeWriter.AttributeValue attributeValue;

		private const int maxBytesPerChar = 3;

		private int textNodeOffset;

		private struct AttributeValue
		{
			public void Clear()
			{
				this.captureText = null;
				this.captureXText = null;
				this.captureStream = null;
			}

			public void WriteText(string s)
			{
				if (this.captureStream != null)
				{
					this.captureText = XmlConverter.Base64Encoding.GetString(this.captureStream.GetBuffer(), 0, (int)this.captureStream.Length);
					this.captureStream = null;
				}
				if (this.captureXText != null)
				{
					this.captureText = this.captureXText.Value;
					this.captureXText = null;
				}
				if (this.captureText == null || this.captureText.Length == 0)
				{
					this.captureText = s;
					return;
				}
				this.captureText += s;
			}

			public void WriteText(XmlDictionaryString s)
			{
				if (this.captureText != null || this.captureStream != null)
				{
					this.WriteText(s.Value);
					return;
				}
				this.captureXText = s;
			}

			public void WriteBase64Text(byte[] trailBytes, int trailByteCount, byte[] buffer, int offset, int count)
			{
				if (this.captureText != null || this.captureXText != null)
				{
					if (trailByteCount > 0)
					{
						this.WriteText(XmlConverter.Base64Encoding.GetString(trailBytes, 0, trailByteCount));
					}
					this.WriteText(XmlConverter.Base64Encoding.GetString(buffer, offset, count));
					return;
				}
				if (this.captureStream == null)
				{
					this.captureStream = new MemoryStream();
				}
				if (trailByteCount > 0)
				{
					this.captureStream.Write(trailBytes, 0, trailByteCount);
				}
				this.captureStream.Write(buffer, offset, count);
			}

			public void WriteTo(XmlBinaryNodeWriter writer)
			{
				if (this.captureText != null)
				{
					writer.WriteText(this.captureText);
					this.captureText = null;
					return;
				}
				if (this.captureXText != null)
				{
					writer.WriteText(this.captureXText);
					this.captureXText = null;
					return;
				}
				if (this.captureStream != null)
				{
					writer.WriteBase64Text(null, 0, this.captureStream.GetBuffer(), 0, (int)this.captureStream.Length);
					this.captureStream = null;
					return;
				}
				writer.WriteEmptyText();
			}

			private string captureText;

			private XmlDictionaryString captureXText;

			private MemoryStream captureStream;
		}
	}
}
