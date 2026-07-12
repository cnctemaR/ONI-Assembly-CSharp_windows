using System;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

namespace System.Xml
{
	internal class XmlUTF8NodeWriter : XmlStreamNodeWriter
	{
		public XmlUTF8NodeWriter()
			: this(XmlUTF8NodeWriter.defaultIsEscapedAttributeChar, XmlUTF8NodeWriter.defaultIsEscapedElementChar)
		{
		}

		public XmlUTF8NodeWriter(bool[] isEscapedAttributeChar, bool[] isEscapedElementChar)
		{
			this.isEscapedAttributeChar = isEscapedAttributeChar;
			this.isEscapedElementChar = isEscapedElementChar;
			this.inAttribute = false;
		}

		public new void SetOutput(Stream stream, bool ownsStream, Encoding encoding)
		{
			Encoding encoding2 = null;
			if (encoding != null && encoding.CodePage == Encoding.UTF8.CodePage)
			{
				encoding2 = encoding;
				encoding = null;
			}
			base.SetOutput(stream, ownsStream, encoding2);
			this.encoding = encoding;
			this.inAttribute = false;
		}

		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
		}

		private byte[] GetCharEntityBuffer()
		{
			if (this.entityChars == null)
			{
				this.entityChars = new byte[32];
			}
			return this.entityChars;
		}

		private char[] GetCharBuffer(int charCount)
		{
			if (charCount >= 256)
			{
				return new char[charCount];
			}
			if (this.chars == null || this.chars.Length < charCount)
			{
				this.chars = new char[charCount];
			}
			return this.chars;
		}

		public override void WriteDeclaration()
		{
			if (this.encoding == null)
			{
				base.WriteUTF8Chars(XmlUTF8NodeWriter.utf8Decl, 0, XmlUTF8NodeWriter.utf8Decl.Length);
				return;
			}
			base.WriteUTF8Chars(XmlUTF8NodeWriter.startDecl, 0, XmlUTF8NodeWriter.startDecl.Length);
			if (this.encoding.WebName == Encoding.BigEndianUnicode.WebName)
			{
				base.WriteUTF8Chars("utf-16BE");
			}
			else
			{
				base.WriteUTF8Chars(this.encoding.WebName);
			}
			base.WriteUTF8Chars(XmlUTF8NodeWriter.endDecl, 0, XmlUTF8NodeWriter.endDecl.Length);
		}

		public override void WriteCData(string text)
		{
			int num;
			byte[] buffer = base.GetBuffer(9, out num);
			buffer[num] = 60;
			buffer[num + 1] = 33;
			buffer[num + 2] = 91;
			buffer[num + 3] = 67;
			buffer[num + 4] = 68;
			buffer[num + 5] = 65;
			buffer[num + 6] = 84;
			buffer[num + 7] = 65;
			buffer[num + 8] = 91;
			base.Advance(9);
			base.WriteUTF8Chars(text);
			byte[] buffer2 = base.GetBuffer(3, out num);
			buffer2[num] = 93;
			buffer2[num + 1] = 93;
			buffer2[num + 2] = 62;
			base.Advance(3);
		}

		private void WriteStartComment()
		{
			int num;
			byte[] buffer = base.GetBuffer(4, out num);
			buffer[num] = 60;
			buffer[num + 1] = 33;
			buffer[num + 2] = 45;
			buffer[num + 3] = 45;
			base.Advance(4);
		}

		private void WriteEndComment()
		{
			int num;
			byte[] buffer = base.GetBuffer(3, out num);
			buffer[num] = 45;
			buffer[num + 1] = 45;
			buffer[num + 2] = 62;
			base.Advance(3);
		}

		public override void WriteComment(string text)
		{
			this.WriteStartComment();
			base.WriteUTF8Chars(text);
			this.WriteEndComment();
		}

		public override void WriteStartElement(string prefix, string localName)
		{
			base.WriteByte('<');
			if (prefix.Length != 0)
			{
				this.WritePrefix(prefix);
				base.WriteByte(':');
			}
			this.WriteLocalName(localName);
		}

		public override void WriteStartElement(string prefix, XmlDictionaryString localName)
		{
			this.WriteStartElement(prefix, localName.Value);
		}

		public override void WriteStartElement(byte[] prefixBuffer, int prefixOffset, int prefixLength, byte[] localNameBuffer, int localNameOffset, int localNameLength)
		{
			base.WriteByte('<');
			if (prefixLength != 0)
			{
				this.WritePrefix(prefixBuffer, prefixOffset, prefixLength);
				base.WriteByte(':');
			}
			this.WriteLocalName(localNameBuffer, localNameOffset, localNameLength);
		}

		public override void WriteEndStartElement(bool isEmpty)
		{
			if (!isEmpty)
			{
				base.WriteByte('>');
				return;
			}
			base.WriteBytes('/', '>');
		}

		public override void WriteEndElement(string prefix, string localName)
		{
			base.WriteBytes('<', '/');
			if (prefix.Length != 0)
			{
				this.WritePrefix(prefix);
				base.WriteByte(':');
			}
			this.WriteLocalName(localName);
			base.WriteByte('>');
		}

		public override void WriteEndElement(byte[] prefixBuffer, int prefixOffset, int prefixLength, byte[] localNameBuffer, int localNameOffset, int localNameLength)
		{
			base.WriteBytes('<', '/');
			if (prefixLength != 0)
			{
				this.WritePrefix(prefixBuffer, prefixOffset, prefixLength);
				base.WriteByte(':');
			}
			this.WriteLocalName(localNameBuffer, localNameOffset, localNameLength);
			base.WriteByte('>');
		}

		private void WriteStartXmlnsAttribute()
		{
			int num;
			byte[] buffer = base.GetBuffer(6, out num);
			buffer[num] = 32;
			buffer[num + 1] = 120;
			buffer[num + 2] = 109;
			buffer[num + 3] = 108;
			buffer[num + 4] = 110;
			buffer[num + 5] = 115;
			base.Advance(6);
			this.inAttribute = true;
		}

		public override void WriteXmlnsAttribute(string prefix, string ns)
		{
			this.WriteStartXmlnsAttribute();
			if (prefix.Length != 0)
			{
				base.WriteByte(':');
				this.WritePrefix(prefix);
			}
			base.WriteBytes('=', '"');
			this.WriteEscapedText(ns);
			this.WriteEndAttribute();
		}

		public override void WriteXmlnsAttribute(string prefix, XmlDictionaryString ns)
		{
			this.WriteXmlnsAttribute(prefix, ns.Value);
		}

		public override void WriteXmlnsAttribute(byte[] prefixBuffer, int prefixOffset, int prefixLength, byte[] nsBuffer, int nsOffset, int nsLength)
		{
			this.WriteStartXmlnsAttribute();
			if (prefixLength != 0)
			{
				base.WriteByte(':');
				this.WritePrefix(prefixBuffer, prefixOffset, prefixLength);
			}
			base.WriteBytes('=', '"');
			this.WriteEscapedText(nsBuffer, nsOffset, nsLength);
			this.WriteEndAttribute();
		}

		public override void WriteStartAttribute(string prefix, string localName)
		{
			base.WriteByte(' ');
			if (prefix.Length != 0)
			{
				this.WritePrefix(prefix);
				base.WriteByte(':');
			}
			this.WriteLocalName(localName);
			base.WriteBytes('=', '"');
			this.inAttribute = true;
		}

		public override void WriteStartAttribute(string prefix, XmlDictionaryString localName)
		{
			this.WriteStartAttribute(prefix, localName.Value);
		}

		public override void WriteStartAttribute(byte[] prefixBuffer, int prefixOffset, int prefixLength, byte[] localNameBuffer, int localNameOffset, int localNameLength)
		{
			base.WriteByte(' ');
			if (prefixLength != 0)
			{
				this.WritePrefix(prefixBuffer, prefixOffset, prefixLength);
				base.WriteByte(':');
			}
			this.WriteLocalName(localNameBuffer, localNameOffset, localNameLength);
			base.WriteBytes('=', '"');
			this.inAttribute = true;
		}

		public override void WriteEndAttribute()
		{
			base.WriteByte('"');
			this.inAttribute = false;
		}

		private void WritePrefix(string prefix)
		{
			if (prefix.Length == 1)
			{
				base.WriteUTF8Char((int)prefix[0]);
				return;
			}
			base.WriteUTF8Chars(prefix);
		}

		private void WritePrefix(byte[] prefixBuffer, int prefixOffset, int prefixLength)
		{
			if (prefixLength == 1)
			{
				base.WriteUTF8Char((int)prefixBuffer[prefixOffset]);
				return;
			}
			base.WriteUTF8Chars(prefixBuffer, prefixOffset, prefixLength);
		}

		private void WriteLocalName(string localName)
		{
			base.WriteUTF8Chars(localName);
		}

		private void WriteLocalName(byte[] localNameBuffer, int localNameOffset, int localNameLength)
		{
			base.WriteUTF8Chars(localNameBuffer, localNameOffset, localNameLength);
		}

		public override void WriteEscapedText(XmlDictionaryString s)
		{
			this.WriteEscapedText(s.Value);
		}

		[SecuritySafeCritical]
		public unsafe override void WriteEscapedText(string s)
		{
			int length = s.Length;
			if (length > 0)
			{
				fixed (string text = s)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					this.UnsafeWriteEscapedText(ptr, length);
				}
			}
		}

		[SecuritySafeCritical]
		public unsafe override void WriteEscapedText(char[] s, int offset, int count)
		{
			if (count > 0)
			{
				fixed (char* ptr = &s[offset])
				{
					char* ptr2 = ptr;
					this.UnsafeWriteEscapedText(ptr2, count);
				}
			}
		}

		[SecurityCritical]
		private unsafe void UnsafeWriteEscapedText(char* chars, int count)
		{
			bool[] array = (this.inAttribute ? this.isEscapedAttributeChar : this.isEscapedElementChar);
			int num = array.Length;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				char c = chars[i];
				if (((int)c < num && array[(int)c]) || c >= '\ufffe')
				{
					base.UnsafeWriteUTF8Chars(chars + num2, i - num2);
					this.WriteCharEntity((int)c);
					num2 = i + 1;
				}
			}
			base.UnsafeWriteUTF8Chars(chars + num2, count - num2);
		}

		public override void WriteEscapedText(byte[] chars, int offset, int count)
		{
			bool[] array = (this.inAttribute ? this.isEscapedAttributeChar : this.isEscapedElementChar);
			int num = array.Length;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				byte b = chars[offset + i];
				if ((int)b < num && array[(int)b])
				{
					base.WriteUTF8Chars(chars, offset + num2, i - num2);
					this.WriteCharEntity((int)b);
					num2 = i + 1;
				}
				else if (b == 239 && offset + i + 2 < count)
				{
					int num3 = (int)chars[offset + i + 1];
					byte b2 = chars[offset + i + 2];
					if (num3 == 191 && (b2 == 190 || b2 == 191))
					{
						base.WriteUTF8Chars(chars, offset + num2, i - num2);
						this.WriteCharEntity((b2 == 190) ? 65534 : 65535);
						num2 = i + 3;
					}
				}
			}
			base.WriteUTF8Chars(chars, offset + num2, count - num2);
		}

		public void WriteText(int ch)
		{
			base.WriteUTF8Char(ch);
		}

		public override void WriteText(byte[] chars, int offset, int count)
		{
			base.WriteUTF8Chars(chars, offset, count);
		}

		[SecuritySafeCritical]
		public unsafe override void WriteText(char[] chars, int offset, int count)
		{
			if (count > 0)
			{
				fixed (char* ptr = &chars[offset])
				{
					char* ptr2 = ptr;
					base.UnsafeWriteUTF8Chars(ptr2, count);
				}
			}
		}

		public override void WriteText(string value)
		{
			base.WriteUTF8Chars(value);
		}

		public override void WriteText(XmlDictionaryString value)
		{
			base.WriteUTF8Chars(value.Value);
		}

		public void WriteLessThanCharEntity()
		{
			int num;
			byte[] buffer = base.GetBuffer(4, out num);
			buffer[num] = 38;
			buffer[num + 1] = 108;
			buffer[num + 2] = 116;
			buffer[num + 3] = 59;
			base.Advance(4);
		}

		public void WriteGreaterThanCharEntity()
		{
			int num;
			byte[] buffer = base.GetBuffer(4, out num);
			buffer[num] = 38;
			buffer[num + 1] = 103;
			buffer[num + 2] = 116;
			buffer[num + 3] = 59;
			base.Advance(4);
		}

		public void WriteAmpersandCharEntity()
		{
			int num;
			byte[] buffer = base.GetBuffer(5, out num);
			buffer[num] = 38;
			buffer[num + 1] = 97;
			buffer[num + 2] = 109;
			buffer[num + 3] = 112;
			buffer[num + 4] = 59;
			base.Advance(5);
		}

		public void WriteApostropheCharEntity()
		{
			int num;
			byte[] buffer = base.GetBuffer(6, out num);
			buffer[num] = 38;
			buffer[num + 1] = 97;
			buffer[num + 2] = 112;
			buffer[num + 3] = 111;
			buffer[num + 4] = 115;
			buffer[num + 5] = 59;
			base.Advance(6);
		}

		public void WriteQuoteCharEntity()
		{
			int num;
			byte[] buffer = base.GetBuffer(6, out num);
			buffer[num] = 38;
			buffer[num + 1] = 113;
			buffer[num + 2] = 117;
			buffer[num + 3] = 111;
			buffer[num + 4] = 116;
			buffer[num + 5] = 59;
			base.Advance(6);
		}

		private void WriteHexCharEntity(int ch)
		{
			byte[] charEntityBuffer = this.GetCharEntityBuffer();
			int num = 32;
			charEntityBuffer[--num] = 59;
			num -= this.ToBase16(charEntityBuffer, num, (uint)ch);
			charEntityBuffer[--num] = 120;
			charEntityBuffer[--num] = 35;
			charEntityBuffer[--num] = 38;
			base.WriteUTF8Chars(charEntityBuffer, num, 32 - num);
		}

		public override void WriteCharEntity(int ch)
		{
			if (ch <= 38)
			{
				if (ch == 34)
				{
					this.WriteQuoteCharEntity();
					return;
				}
				if (ch == 38)
				{
					this.WriteAmpersandCharEntity();
					return;
				}
			}
			else
			{
				if (ch == 39)
				{
					this.WriteApostropheCharEntity();
					return;
				}
				if (ch == 60)
				{
					this.WriteLessThanCharEntity();
					return;
				}
				if (ch == 62)
				{
					this.WriteGreaterThanCharEntity();
					return;
				}
			}
			this.WriteHexCharEntity(ch);
		}

		private int ToBase16(byte[] chars, int offset, uint value)
		{
			int num = 0;
			do
			{
				num++;
				chars[--offset] = XmlUTF8NodeWriter.digits[(int)(value & 15U)];
				value /= 16U;
			}
			while (value != 0U);
			return num;
		}

		public override void WriteBoolText(bool value)
		{
			int num;
			byte[] buffer = base.GetBuffer(5, out num);
			base.Advance(XmlConverter.ToChars(value, buffer, num));
		}

		public override void WriteDecimalText(decimal value)
		{
			int num;
			byte[] buffer = base.GetBuffer(40, out num);
			base.Advance(XmlConverter.ToChars(value, buffer, num));
		}

		public override void WriteDoubleText(double value)
		{
			int num;
			byte[] buffer = base.GetBuffer(32, out num);
			base.Advance(XmlConverter.ToChars(value, buffer, num));
		}

		public override void WriteFloatText(float value)
		{
			int num;
			byte[] buffer = base.GetBuffer(16, out num);
			base.Advance(XmlConverter.ToChars(value, buffer, num));
		}

		public override void WriteDateTimeText(DateTime value)
		{
			int num;
			byte[] buffer = base.GetBuffer(64, out num);
			base.Advance(XmlConverter.ToChars(value, buffer, num));
		}

		public override void WriteUniqueIdText(UniqueId value)
		{
			if (value.IsGuid)
			{
				int charArrayLength = value.CharArrayLength;
				char[] charBuffer = this.GetCharBuffer(charArrayLength);
				value.ToCharArray(charBuffer, 0);
				this.WriteText(charBuffer, 0, charArrayLength);
				return;
			}
			this.WriteEscapedText(value.ToString());
		}

		public override void WriteInt32Text(int value)
		{
			int num;
			byte[] buffer = base.GetBuffer(16, out num);
			base.Advance(XmlConverter.ToChars(value, buffer, num));
		}

		public override void WriteInt64Text(long value)
		{
			int num;
			byte[] buffer = base.GetBuffer(32, out num);
			base.Advance(XmlConverter.ToChars(value, buffer, num));
		}

		public override void WriteUInt64Text(ulong value)
		{
			int num;
			byte[] buffer = base.GetBuffer(32, out num);
			base.Advance(XmlConverter.ToChars(value, buffer, num));
		}

		public override void WriteGuidText(Guid value)
		{
			this.WriteText(value.ToString());
		}

		public override void WriteBase64Text(byte[] trailBytes, int trailByteCount, byte[] buffer, int offset, int count)
		{
			if (trailByteCount > 0)
			{
				this.InternalWriteBase64Text(trailBytes, 0, trailByteCount);
			}
			this.InternalWriteBase64Text(buffer, offset, count);
		}

		private void InternalWriteBase64Text(byte[] buffer, int offset, int count)
		{
			Base64Encoding base64Encoding = XmlConverter.Base64Encoding;
			while (count >= 3)
			{
				int num = Math.Min(384, count - count % 3);
				int num2 = num / 3 * 4;
				int num3;
				byte[] buffer2 = base.GetBuffer(num2, out num3);
				base.Advance(base64Encoding.GetChars(buffer, offset, num, buffer2, num3));
				offset += num;
				count -= num;
			}
			if (count > 0)
			{
				int num4;
				byte[] buffer3 = base.GetBuffer(4, out num4);
				base.Advance(base64Encoding.GetChars(buffer, offset, count, buffer3, num4));
			}
		}

		internal override AsyncCompletionResult WriteBase64TextAsync(AsyncEventArgs<XmlNodeWriterWriteBase64TextArgs> xmlNodeWriterState)
		{
			if (this.internalWriteBase64TextAsyncWriter == null)
			{
				this.internalWriteBase64TextAsyncWriter = new XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter(this);
			}
			return this.internalWriteBase64TextAsyncWriter.StartAsync(xmlNodeWriterState);
		}

		public override IAsyncResult BeginWriteBase64Text(byte[] trailBytes, int trailByteCount, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return new XmlUTF8NodeWriter.WriteBase64TextAsyncResult(trailBytes, trailByteCount, buffer, offset, count, this, callback, state);
		}

		public override void EndWriteBase64Text(IAsyncResult result)
		{
			XmlUTF8NodeWriter.WriteBase64TextAsyncResult.End(result);
		}

		private IAsyncResult BeginInternalWriteBase64Text(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return new XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult(buffer, offset, count, this, callback, state);
		}

		private void EndInternalWriteBase64Text(IAsyncResult result)
		{
			XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult.End(result);
		}

		public override void WriteTimeSpanText(TimeSpan value)
		{
			this.WriteText(XmlConvert.ToString(value));
		}

		public override void WriteStartListText()
		{
		}

		public override void WriteListSeparator()
		{
			base.WriteByte(' ');
		}

		public override void WriteEndListText()
		{
		}

		public override void WriteQualifiedName(string prefix, XmlDictionaryString localName)
		{
			if (prefix.Length != 0)
			{
				this.WritePrefix(prefix);
				base.WriteByte(':');
			}
			this.WriteText(localName);
		}

		private byte[] entityChars;

		private bool[] isEscapedAttributeChar;

		private bool[] isEscapedElementChar;

		private bool inAttribute;

		private const int bufferLength = 512;

		private const int maxEntityLength = 32;

		private const int maxBytesPerChar = 3;

		private Encoding encoding;

		private char[] chars;

		private XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter internalWriteBase64TextAsyncWriter;

		private static readonly byte[] startDecl = new byte[]
		{
			60, 63, 120, 109, 108, 32, 118, 101, 114, 115,
			105, 111, 110, 61, 34, 49, 46, 48, 34, 32,
			101, 110, 99, 111, 100, 105, 110, 103, 61, 34
		};

		private static readonly byte[] endDecl = new byte[] { 34, 63, 62 };

		private static readonly byte[] utf8Decl = new byte[]
		{
			60, 63, 120, 109, 108, 32, 118, 101, 114, 115,
			105, 111, 110, 61, 34, 49, 46, 48, 34, 32,
			101, 110, 99, 111, 100, 105, 110, 103, 61, 34,
			117, 116, 102, 45, 56, 34, 63, 62
		};

		private static readonly byte[] digits = new byte[]
		{
			48, 49, 50, 51, 52, 53, 54, 55, 56, 57,
			65, 66, 67, 68, 69, 70
		};

		private static readonly bool[] defaultIsEscapedAttributeChar = new bool[]
		{
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, false, false, true, false, false, false, true, false,
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, false, false,
			true, false, true, false
		};

		private static readonly bool[] defaultIsEscapedElementChar = new bool[]
		{
			true, true, true, true, true, true, true, true, true, false,
			false, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, false, false, false, false, false, false, true, false,
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, false, false,
			true, false, true, false
		};

		private class InternalWriteBase64TextAsyncWriter
		{
			public InternalWriteBase64TextAsyncWriter(XmlUTF8NodeWriter writer)
			{
				this.writer = writer;
				this.writerState = new AsyncEventArgs<XmlWriteBase64AsyncArguments>();
				this.writerArgs = new XmlWriteBase64AsyncArguments();
			}

			internal AsyncCompletionResult StartAsync(AsyncEventArgs<XmlNodeWriterWriteBase64TextArgs> xmlNodeWriterState)
			{
				this.nodeState = xmlNodeWriterState;
				XmlNodeWriterWriteBase64TextArgs arguments = xmlNodeWriterState.Arguments;
				if (arguments.TrailCount > 0)
				{
					this.writerArgs.Buffer = arguments.TrailBuffer;
					this.writerArgs.Offset = 0;
					this.writerArgs.Count = arguments.TrailCount;
					this.writerState.Set(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter.onTrailByteComplete, this.writerArgs, this);
					if (this.InternalWriteBase64TextAsync(this.writerState) != AsyncCompletionResult.Completed)
					{
						return AsyncCompletionResult.Queued;
					}
					this.writerState.Complete(true);
				}
				if (this.WriteBufferAsync() == AsyncCompletionResult.Completed)
				{
					this.nodeState = null;
					return AsyncCompletionResult.Completed;
				}
				return AsyncCompletionResult.Queued;
			}

			private static void OnTrailBytesComplete(IAsyncEventArgs eventArgs)
			{
				XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter internalWriteBase64TextAsyncWriter = (XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter)eventArgs.AsyncState;
				bool flag = false;
				try
				{
					if (eventArgs.Exception != null)
					{
						Exception exception = eventArgs.Exception;
						flag = true;
					}
					else if (internalWriteBase64TextAsyncWriter.WriteBufferAsync() == AsyncCompletionResult.Completed)
					{
						flag = true;
					}
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					flag = true;
				}
				if (flag)
				{
					AsyncEventArgs<XmlNodeWriterWriteBase64TextArgs> asyncEventArgs = internalWriteBase64TextAsyncWriter.nodeState;
					internalWriteBase64TextAsyncWriter.nodeState = null;
					asyncEventArgs.Complete(false, eventArgs.Exception);
				}
			}

			private AsyncCompletionResult WriteBufferAsync()
			{
				this.writerArgs.Buffer = this.nodeState.Arguments.Buffer;
				this.writerArgs.Offset = this.nodeState.Arguments.Offset;
				this.writerArgs.Count = this.nodeState.Arguments.Count;
				this.writerState.Set(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter.onWriteComplete, this.writerArgs, this);
				if (this.InternalWriteBase64TextAsync(this.writerState) == AsyncCompletionResult.Completed)
				{
					this.writerState.Complete(true);
					return AsyncCompletionResult.Completed;
				}
				return AsyncCompletionResult.Queued;
			}

			private static void OnWriteComplete(IAsyncEventArgs eventArgs)
			{
				XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter internalWriteBase64TextAsyncWriter = (XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter)eventArgs.AsyncState;
				AsyncEventArgs<XmlNodeWriterWriteBase64TextArgs> asyncEventArgs = internalWriteBase64TextAsyncWriter.nodeState;
				internalWriteBase64TextAsyncWriter.nodeState = null;
				asyncEventArgs.Complete(false, eventArgs.Exception);
			}

			private AsyncCompletionResult InternalWriteBase64TextAsync(AsyncEventArgs<XmlWriteBase64AsyncArguments> writerState)
			{
				XmlStreamNodeWriter.GetBufferAsyncEventArgs e = this.getBufferState;
				XmlStreamNodeWriter.GetBufferArgs getBufferArgs = this.getBufferArgs;
				XmlWriteBase64AsyncArguments arguments = writerState.Arguments;
				if (e == null)
				{
					e = new XmlStreamNodeWriter.GetBufferAsyncEventArgs();
					getBufferArgs = new XmlStreamNodeWriter.GetBufferArgs();
					this.getBufferState = e;
					this.getBufferArgs = getBufferArgs;
				}
				Base64Encoding base64Encoding = XmlConverter.Base64Encoding;
				while (arguments.Count >= 3)
				{
					int num = Math.Min(384, arguments.Count - arguments.Count % 3);
					int num2 = num / 3 * 4;
					getBufferArgs.Count = num2;
					e.Set(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter.onGetBufferComplete, getBufferArgs, this);
					if (this.writer.GetBufferAsync(e) != AsyncCompletionResult.Completed)
					{
						return AsyncCompletionResult.Queued;
					}
					XmlStreamNodeWriter.GetBufferEventResult result = e.Result;
					e.Complete(true);
					this.writer.Advance(base64Encoding.GetChars(arguments.Buffer, arguments.Offset, num, result.Buffer, result.Offset));
					arguments.Offset += num;
					arguments.Count -= num;
				}
				if (arguments.Count > 0)
				{
					getBufferArgs.Count = 4;
					e.Set(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter.onGetBufferComplete, getBufferArgs, this);
					if (this.writer.GetBufferAsync(e) != AsyncCompletionResult.Completed)
					{
						return AsyncCompletionResult.Queued;
					}
					XmlStreamNodeWriter.GetBufferEventResult result2 = e.Result;
					e.Complete(true);
					this.writer.Advance(base64Encoding.GetChars(arguments.Buffer, arguments.Offset, arguments.Count, result2.Buffer, result2.Offset));
				}
				return AsyncCompletionResult.Completed;
			}

			private static void OnGetBufferComplete(IAsyncEventArgs state)
			{
				XmlStreamNodeWriter.GetBufferEventResult result = ((XmlStreamNodeWriter.GetBufferAsyncEventArgs)state).Result;
				XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter internalWriteBase64TextAsyncWriter = (XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter)state.AsyncState;
				XmlWriteBase64AsyncArguments arguments = internalWriteBase64TextAsyncWriter.writerState.Arguments;
				Exception ex = null;
				bool flag = false;
				try
				{
					if (state.Exception != null)
					{
						ex = state.Exception;
						flag = true;
					}
					else
					{
						byte[] buffer = result.Buffer;
						int offset = result.Offset;
						Base64Encoding base64Encoding = XmlConverter.Base64Encoding;
						int num = Math.Min(384, arguments.Count - arguments.Count % 3);
						int num2 = num / 3;
						internalWriteBase64TextAsyncWriter.writer.Advance(base64Encoding.GetChars(arguments.Buffer, arguments.Offset, num, buffer, offset));
						if (num >= 3)
						{
							arguments.Offset += num;
							arguments.Count -= num;
						}
						if (internalWriteBase64TextAsyncWriter.InternalWriteBase64TextAsync(internalWriteBase64TextAsyncWriter.writerState) == AsyncCompletionResult.Completed)
						{
							flag = true;
						}
					}
				}
				catch (Exception ex2)
				{
					if (Fx.IsFatal(ex2))
					{
						throw;
					}
					ex = ex2;
					flag = true;
				}
				if (flag)
				{
					internalWriteBase64TextAsyncWriter.writerState.Complete(false, ex);
				}
			}

			private AsyncEventArgs<XmlNodeWriterWriteBase64TextArgs> nodeState;

			private AsyncEventArgs<XmlWriteBase64AsyncArguments> writerState;

			private XmlWriteBase64AsyncArguments writerArgs;

			private XmlUTF8NodeWriter writer;

			private XmlStreamNodeWriter.GetBufferAsyncEventArgs getBufferState;

			private XmlStreamNodeWriter.GetBufferArgs getBufferArgs;

			private static AsyncEventArgsCallback onTrailByteComplete = new AsyncEventArgsCallback(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter.OnTrailBytesComplete);

			private static AsyncEventArgsCallback onWriteComplete = new AsyncEventArgsCallback(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter.OnWriteComplete);

			private static AsyncEventArgsCallback onGetBufferComplete = new AsyncEventArgsCallback(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncWriter.OnGetBufferComplete);
		}

		private class WriteBase64TextAsyncResult : AsyncResult
		{
			public WriteBase64TextAsyncResult(byte[] trailBytes, int trailByteCount, byte[] buffer, int offset, int count, XmlUTF8NodeWriter writer, AsyncCallback callback, object state)
				: base(callback, state)
			{
				this.writer = writer;
				this.trailBytes = trailBytes;
				this.trailByteCount = trailByteCount;
				this.buffer = buffer;
				this.offset = offset;
				this.count = count;
				if (this.HandleWriteTrailBytes(null))
				{
					base.Complete(true);
				}
			}

			private static bool OnTrailBytesComplete(IAsyncResult result)
			{
				return ((XmlUTF8NodeWriter.WriteBase64TextAsyncResult)result.AsyncState).HandleWriteTrailBytes(result);
			}

			private static bool OnComplete(IAsyncResult result)
			{
				return ((XmlUTF8NodeWriter.WriteBase64TextAsyncResult)result.AsyncState).HandleWriteBase64Text(result);
			}

			private bool HandleWriteTrailBytes(IAsyncResult result)
			{
				if (this.trailByteCount > 0)
				{
					if (result == null)
					{
						result = this.writer.BeginInternalWriteBase64Text(this.trailBytes, 0, this.trailByteCount, base.PrepareAsyncCompletion(XmlUTF8NodeWriter.WriteBase64TextAsyncResult.onTrailBytesComplete), this);
						if (!result.CompletedSynchronously)
						{
							return false;
						}
					}
					this.writer.EndInternalWriteBase64Text(result);
				}
				return this.HandleWriteBase64Text(null);
			}

			private bool HandleWriteBase64Text(IAsyncResult result)
			{
				if (result == null)
				{
					result = this.writer.BeginInternalWriteBase64Text(this.buffer, this.offset, this.count, base.PrepareAsyncCompletion(XmlUTF8NodeWriter.WriteBase64TextAsyncResult.onComplete), this);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				this.writer.EndInternalWriteBase64Text(result);
				return true;
			}

			public static void End(IAsyncResult result)
			{
				AsyncResult.End<XmlUTF8NodeWriter.WriteBase64TextAsyncResult>(result);
			}

			private static AsyncResult.AsyncCompletion onTrailBytesComplete = new AsyncResult.AsyncCompletion(XmlUTF8NodeWriter.WriteBase64TextAsyncResult.OnTrailBytesComplete);

			private static AsyncResult.AsyncCompletion onComplete = new AsyncResult.AsyncCompletion(XmlUTF8NodeWriter.WriteBase64TextAsyncResult.OnComplete);

			private byte[] trailBytes;

			private int trailByteCount;

			private byte[] buffer;

			private int offset;

			private int count;

			private XmlUTF8NodeWriter writer;
		}

		private class InternalWriteBase64TextAsyncResult : AsyncResult
		{
			public InternalWriteBase64TextAsyncResult(byte[] buffer, int offset, int count, XmlUTF8NodeWriter writer, AsyncCallback callback, object state)
				: base(callback, state)
			{
				this.buffer = buffer;
				this.offset = offset;
				this.count = count;
				this.writer = writer;
				this.encoding = XmlConverter.Base64Encoding;
				if (this.ContinueWork())
				{
					base.Complete(true);
				}
			}

			private static bool OnWriteTrailingCharacters(IAsyncResult result)
			{
				return ((XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult)result.AsyncState).HandleWriteTrailingCharacters(result);
			}

			private bool ContinueWork()
			{
				while (this.count >= 3)
				{
					if (!this.HandleWriteCharacters(null))
					{
						return false;
					}
				}
				return this.count <= 0 || this.HandleWriteTrailingCharacters(null);
			}

			private bool HandleWriteCharacters(IAsyncResult result)
			{
				int num = Math.Min(384, this.count - this.count % 3);
				int num2 = num / 3 * 4;
				if (result == null)
				{
					result = this.writer.BeginGetBuffer(num2, XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult.onWriteCharacters, this);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				int num3;
				byte[] array = this.writer.EndGetBuffer(result, out num3);
				this.writer.Advance(this.encoding.GetChars(this.buffer, this.offset, num, array, num3));
				this.offset += num;
				this.count -= num;
				return true;
			}

			private bool HandleWriteTrailingCharacters(IAsyncResult result)
			{
				if (result == null)
				{
					result = this.writer.BeginGetBuffer(4, base.PrepareAsyncCompletion(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult.onWriteTrailingCharacters), this);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				int num;
				byte[] array = this.writer.EndGetBuffer(result, out num);
				this.writer.Advance(this.encoding.GetChars(this.buffer, this.offset, this.count, array, num));
				return true;
			}

			private static void OnWriteCharacters(IAsyncResult result)
			{
				if (result.CompletedSynchronously)
				{
					return;
				}
				XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult internalWriteBase64TextAsyncResult = (XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult)result.AsyncState;
				Exception ex = null;
				bool flag = false;
				try
				{
					internalWriteBase64TextAsyncResult.HandleWriteCharacters(result);
					flag = internalWriteBase64TextAsyncResult.ContinueWork();
				}
				catch (Exception ex2)
				{
					if (Fx.IsFatal(ex2))
					{
						throw;
					}
					flag = true;
					ex = ex2;
				}
				if (flag)
				{
					internalWriteBase64TextAsyncResult.Complete(false, ex);
				}
			}

			public static void End(IAsyncResult result)
			{
				AsyncResult.End<XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult>(result);
			}

			private byte[] buffer;

			private int offset;

			private int count;

			private Base64Encoding encoding;

			private XmlUTF8NodeWriter writer;

			private static AsyncCallback onWriteCharacters = Fx.ThunkCallback(new AsyncCallback(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult.OnWriteCharacters));

			private static AsyncResult.AsyncCompletion onWriteTrailingCharacters = new AsyncResult.AsyncCompletion(XmlUTF8NodeWriter.InternalWriteBase64TextAsyncResult.OnWriteTrailingCharacters);
		}
	}
}
