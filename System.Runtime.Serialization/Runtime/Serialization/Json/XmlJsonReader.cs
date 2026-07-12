using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class XmlJsonReader : XmlBaseReader, IXmlJsonReaderInitializer
	{
		public override bool CanCanonicalize
		{
			get
			{
				return false;
			}
		}

		public override string Value
		{
			get
			{
				if (this.IsAttributeValue && !this.IsLocalName("type"))
				{
					return this.UnescapeJsonString(base.Value);
				}
				return base.Value;
			}
		}

		private bool IsAttributeValue
		{
			get
			{
				return base.Node.NodeType == XmlNodeType.Attribute || base.Node is XmlBaseReader.XmlAttributeTextNode;
			}
		}

		private bool IsReadingCollection
		{
			get
			{
				return this.scopeDepth > 0 && this.scopes[this.scopeDepth] == JsonNodeType.Collection;
			}
		}

		private bool IsReadingComplexText
		{
			get
			{
				return !base.Node.IsAtomicValue && base.Node.NodeType == XmlNodeType.Text;
			}
		}

		public override void Close()
		{
			base.Close();
			OnXmlDictionaryReaderClose onXmlDictionaryReaderClose = this.onReaderClose;
			this.onReaderClose = null;
			this.ResetState();
			if (onXmlDictionaryReaderClose != null)
			{
				try
				{
					onXmlDictionaryReaderClose(this);
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(ex);
				}
			}
		}

		public override void EndCanonicalization()
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		public override string GetAttribute(int index)
		{
			return this.UnescapeJsonString(base.GetAttribute(index));
		}

		public override string GetAttribute(string localName, string namespaceUri)
		{
			if (localName != "type")
			{
				return this.UnescapeJsonString(base.GetAttribute(localName, namespaceUri));
			}
			return base.GetAttribute(localName, namespaceUri);
		}

		public override string GetAttribute(string name)
		{
			if (name != "type")
			{
				return this.UnescapeJsonString(base.GetAttribute(name));
			}
			return base.GetAttribute(name);
		}

		public override string GetAttribute(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (XmlDictionaryString.GetString(localName) != "type")
			{
				return this.UnescapeJsonString(base.GetAttribute(localName, namespaceUri));
			}
			return base.GetAttribute(localName, namespaceUri);
		}

		public override bool Read()
		{
			if (base.Node.CanMoveToElement)
			{
				this.MoveToElement();
			}
			if (base.Node.ReadState == ReadState.Closed)
			{
				return false;
			}
			if (base.Node.ExitScope)
			{
				base.ExitScope();
			}
			if (!this.buffered)
			{
				base.BufferReader.SetWindow(base.ElementNode.BufferOffset, this.maxBytesPerRead);
			}
			byte b;
			if (!this.IsReadingComplexText)
			{
				this.SkipWhitespaceInBufferReader();
				if (this.TryGetByte(out b) && (this.charactersToSkipOnNextRead[0] == b || this.charactersToSkipOnNextRead[1] == b))
				{
					base.BufferReader.SkipByte();
					this.charactersToSkipOnNextRead[0] = 0;
					this.charactersToSkipOnNextRead[1] = 0;
				}
				this.SkipWhitespaceInBufferReader();
				if (this.TryGetByte(out b) && b == 93 && this.IsReadingCollection)
				{
					base.BufferReader.SkipByte();
					this.SkipWhitespaceInBufferReader();
					this.ExitJsonScope();
				}
				if (base.BufferReader.EndOfFile)
				{
					if (this.scopeDepth > 0)
					{
						this.MoveToEndElement();
						return true;
					}
					base.MoveToEndOfFile();
					return false;
				}
			}
			b = base.BufferReader.GetByte();
			if (this.scopeDepth == 0)
			{
				this.ReadNonExistentElementName(StringHandleConstStringType.Root);
			}
			else if (this.IsReadingComplexText)
			{
				switch (this.complexTextMode)
				{
				case XmlJsonReader.JsonComplexTextMode.QuotedText:
					if (b == 92)
					{
						this.ReadEscapedCharacter(true);
					}
					else
					{
						this.ReadQuotedText(true);
					}
					break;
				case XmlJsonReader.JsonComplexTextMode.NumericalText:
					this.ReadNumericalText();
					break;
				case XmlJsonReader.JsonComplexTextMode.None:
					XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an unexpected character '{0}' in JSON.", new object[] { (char)b })));
					break;
				}
			}
			else if (this.IsReadingCollection)
			{
				this.ReadNonExistentElementName(StringHandleConstStringType.Item);
			}
			else if (b == 93)
			{
				base.BufferReader.SkipByte();
				this.MoveToEndElement();
				this.ExitJsonScope();
			}
			else if (b == 123)
			{
				base.BufferReader.SkipByte();
				this.SkipWhitespaceInBufferReader();
				b = base.BufferReader.GetByte();
				if (b == 125)
				{
					base.BufferReader.SkipByte();
					this.SkipWhitespaceInBufferReader();
					if (this.TryGetByte(out b))
					{
						if (b == 44)
						{
							base.BufferReader.SkipByte();
						}
					}
					else
					{
						this.charactersToSkipOnNextRead[0] = 44;
					}
					this.MoveToEndElement();
				}
				else
				{
					this.EnterJsonScope(JsonNodeType.Object);
					this.ParseStartElement();
				}
			}
			else if (b == 125)
			{
				base.BufferReader.SkipByte();
				if (this.expectingFirstElementInNonPrimitiveChild)
				{
					this.SkipWhitespaceInBufferReader();
					b = base.BufferReader.GetByte();
					if (b == 44 || b == 125)
					{
						base.BufferReader.SkipByte();
					}
					else
					{
						XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an unexpected character '{0}' in JSON.", new object[] { (char)b })));
					}
					this.expectingFirstElementInNonPrimitiveChild = false;
				}
				this.MoveToEndElement();
			}
			else if (b == 44)
			{
				base.BufferReader.SkipByte();
				this.MoveToEndElement();
			}
			else if (b == 34)
			{
				if (this.readServerTypeElement)
				{
					this.readServerTypeElement = false;
					this.EnterJsonScope(JsonNodeType.Object);
					this.ParseStartElement();
				}
				else if (base.Node.NodeType == XmlNodeType.Element)
				{
					if (this.expectingFirstElementInNonPrimitiveChild)
					{
						this.EnterJsonScope(JsonNodeType.Object);
						this.ParseStartElement();
					}
					else
					{
						base.BufferReader.SkipByte();
						this.ReadQuotedText(true);
					}
				}
				else if (base.Node.NodeType == XmlNodeType.EndElement)
				{
					this.EnterJsonScope(JsonNodeType.Element);
					this.ParseStartElement();
				}
				else
				{
					XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an unexpected character '{0}' in JSON.", new object[] { '"' })));
				}
			}
			else if (b == 102)
			{
				int num;
				byte[] buffer = base.BufferReader.GetBuffer(5, out num);
				if (buffer[num + 1] != 97 || buffer[num + 2] != 108 || buffer[num + 3] != 115 || buffer[num + 4] != 101)
				{
					XmlExceptionHelper.ThrowTokenExpected(this, "false", Encoding.UTF8.GetString(buffer, num, 5));
				}
				base.BufferReader.Advance(5);
				if (this.TryGetByte(out b) && !XmlJsonReader.IsWhitespace(b) && b != 44 && b != 125 && b != 93)
				{
					string text = "false";
					string @string = Encoding.UTF8.GetString(buffer, num, 4);
					char c = (char)b;
					XmlExceptionHelper.ThrowTokenExpected(this, text, @string + c.ToString());
				}
				base.MoveToAtomicText().Value.SetValue(ValueHandleType.UTF8, num, 5);
			}
			else if (b == 116)
			{
				int num2;
				byte[] buffer2 = base.BufferReader.GetBuffer(4, out num2);
				if (buffer2[num2 + 1] != 114 || buffer2[num2 + 2] != 117 || buffer2[num2 + 3] != 101)
				{
					XmlExceptionHelper.ThrowTokenExpected(this, "true", Encoding.UTF8.GetString(buffer2, num2, 4));
				}
				base.BufferReader.Advance(4);
				if (this.TryGetByte(out b) && !XmlJsonReader.IsWhitespace(b) && b != 44 && b != 125 && b != 93)
				{
					string text2 = "true";
					string string2 = Encoding.UTF8.GetString(buffer2, num2, 4);
					char c = (char)b;
					XmlExceptionHelper.ThrowTokenExpected(this, text2, string2 + c.ToString());
				}
				base.MoveToAtomicText().Value.SetValue(ValueHandleType.UTF8, num2, 4);
			}
			else if (b == 110)
			{
				int num3;
				byte[] buffer3 = base.BufferReader.GetBuffer(4, out num3);
				if (buffer3[num3 + 1] != 117 || buffer3[num3 + 2] != 108 || buffer3[num3 + 3] != 108)
				{
					XmlExceptionHelper.ThrowTokenExpected(this, "null", Encoding.UTF8.GetString(buffer3, num3, 4));
				}
				base.BufferReader.Advance(4);
				this.SkipWhitespaceInBufferReader();
				if (this.TryGetByte(out b))
				{
					if (b == 44 || b == 125)
					{
						base.BufferReader.SkipByte();
					}
					else if (b != 93)
					{
						string text3 = "null";
						string string3 = Encoding.UTF8.GetString(buffer3, num3, 4);
						char c = (char)b;
						XmlExceptionHelper.ThrowTokenExpected(this, text3, string3 + c.ToString());
					}
				}
				else
				{
					this.charactersToSkipOnNextRead[0] = 44;
					this.charactersToSkipOnNextRead[1] = 125;
				}
				this.MoveToEndElement();
			}
			else if (b == 45 || (48 <= b && b <= 57) || b == 73 || b == 78)
			{
				this.ReadNumericalText();
			}
			else
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an unexpected character '{0}' in JSON.", new object[] { (char)b })));
			}
			return true;
		}

		public override decimal ReadContentAsDecimal()
		{
			string text = this.ReadContentAsString();
			decimal num;
			try
			{
				num = decimal.Parse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "decimal", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "decimal", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "decimal", ex3));
			}
			return num;
		}

		public override int ReadContentAsInt()
		{
			return XmlJsonReader.ParseInt(this.ReadContentAsString(), NumberStyles.Float);
		}

		public override long ReadContentAsLong()
		{
			string text = this.ReadContentAsString();
			long num;
			try
			{
				num = long.Parse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Int64", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Int64", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(text, "Int64", ex3));
			}
			return num;
		}

		public override int ReadValueAsBase64(byte[] buffer, int offset, int count)
		{
			if (!this.IsAttributeValue)
			{
				return base.ReadValueAsBase64(buffer, offset, count);
			}
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > buffer.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { buffer.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { buffer.Length - offset })));
			}
			return 0;
		}

		public override int ReadValueChunk(char[] chars, int offset, int count)
		{
			if (!this.IsAttributeValue)
			{
				return base.ReadValueChunk(chars, offset, count);
			}
			if (chars == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("chars"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > chars.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { chars.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > chars.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { chars.Length - offset })));
			}
			string text = this.UnescapeJsonString(base.Node.ValueAsString);
			int num = Math.Min(count, text.Length);
			if (num > 0)
			{
				text.CopyTo(0, chars, offset, num);
				if (base.Node.QNameType == XmlBaseReader.QNameType.Xmlns)
				{
					base.Node.Namespace.Uri.SetValue(0, 0);
				}
				else
				{
					base.Node.Value.SetValue(ValueHandleType.UTF8, 0, 0);
				}
			}
			return num;
		}

		public void SetInput(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > buffer.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("On JSON writer, offset exceeded buffer size {0}.", new object[] { buffer.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > buffer.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("JSON size exceeded remaining buffer space, by {0} byte(s).", new object[] { buffer.Length - offset })));
			}
			this.MoveToInitial(quotas, onClose);
			ArraySegment<byte> arraySegment = JsonEncodingStreamWrapper.ProcessBuffer(buffer, offset, count, encoding);
			base.BufferReader.SetBuffer(arraySegment.Array, arraySegment.Offset, arraySegment.Count, null, null);
			this.buffered = true;
			this.ResetState();
		}

		public void SetInput(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
		{
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
			}
			this.MoveToInitial(quotas, onClose);
			stream = new JsonEncodingStreamWrapper(stream, encoding, true);
			base.BufferReader.SetBuffer(stream, null, null);
			this.buffered = false;
			this.ResetState();
		}

		public override void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		internal static void CheckArray(Array array, int offset, int count)
		{
			if (array == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("array"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > array.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", global::System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[] { array.Length })));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > array.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", global::System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[] { array.Length - offset })));
			}
		}

		protected override XmlSigningNodeWriter CreateSigningNodeWriter()
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(global::System.Runtime.Serialization.SR.GetString("Method {0} is not supported in JSON.", new object[] { "CreateSigningNodeWriter" })));
		}

		private static int BreakText(byte[] buffer, int offset, int length)
		{
			if (length > 0 && (buffer[offset + length - 1] & 128) == 128)
			{
				int num = length;
				do
				{
					length--;
				}
				while (length > 0 && (buffer[offset + length] & 192) != 192);
				if (length == 0)
				{
					return num;
				}
				byte b = (byte)(buffer[offset + length] << 2);
				int num2 = 2;
				while ((b & 128) == 128)
				{
					b = (byte)(b << 1);
					num2++;
					if (num2 > 4)
					{
						return num;
					}
				}
				if (length + num2 == num)
				{
					return num;
				}
				if (length == 0)
				{
					return num;
				}
			}
			return length;
		}

		private static int ComputeNumericalTextLength(byte[] buffer, int offset, int offsetMax)
		{
			int num = offset;
			while (offset < offsetMax)
			{
				byte b = buffer[offset];
				if (b == 44 || b == 125 || b == 93 || XmlJsonReader.IsWhitespace(b))
				{
					break;
				}
				offset++;
			}
			return offset - num;
		}

		private static int ComputeQuotedTextLengthUntilEndQuote(byte[] buffer, int offset, int offsetMax, out bool escaped)
		{
			int num = offset;
			escaped = false;
			while (offset < offsetMax)
			{
				byte b = buffer[offset];
				if (b < 32)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(global::System.Runtime.Serialization.SR.GetString("Encountered an invalid character '{0}'.", new object[] { (char)b })));
				}
				if (b == 92 || b == 239)
				{
					escaped = true;
					break;
				}
				if (b == 34)
				{
					break;
				}
				offset++;
			}
			return offset - num;
		}

		private static bool IsWhitespace(byte ch)
		{
			return ch == 32 || ch == 9 || ch == 10 || ch == 13;
		}

		private static char ParseChar(string value, NumberStyles style)
		{
			int num = XmlJsonReader.ParseInt(value, style);
			char c;
			try
			{
				c = Convert.ToChar(num);
			}
			catch (OverflowException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "char", ex));
			}
			return c;
		}

		private static int ParseInt(string value, NumberStyles style)
		{
			int num;
			try
			{
				num = int.Parse(value, style, NumberFormatInfo.InvariantInfo);
			}
			catch (ArgumentException ex)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Int32", ex));
			}
			catch (FormatException ex2)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Int32", ex2));
			}
			catch (OverflowException ex3)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlExceptionHelper.CreateConversionException(value, "Int32", ex3));
			}
			return num;
		}

		private void BufferElement()
		{
			int offset = base.BufferReader.Offset;
			bool flag = false;
			byte b = 0;
			while (!flag)
			{
				int num;
				int num2;
				byte[] buffer = base.BufferReader.GetBuffer(128, out num, out num2);
				if (num + 128 != num2)
				{
					break;
				}
				int num3 = num;
				while (num3 < num2 && !flag)
				{
					byte b2 = buffer[num3];
					if (b2 == 92)
					{
						num3++;
						if (num3 >= num2)
						{
							break;
						}
					}
					else if (b == 0)
					{
						if (b2 == 39 || b2 == 34)
						{
							b = b2;
						}
						if (b2 == 58)
						{
							flag = true;
						}
					}
					else if (b2 == b)
					{
						b = 0;
					}
					num3++;
				}
				base.BufferReader.Advance(128);
			}
			base.BufferReader.Offset = offset;
		}

		private void EnterJsonScope(JsonNodeType currentNodeType)
		{
			this.scopeDepth++;
			if (this.scopes == null)
			{
				this.scopes = new JsonNodeType[4];
			}
			else if (this.scopes.Length == this.scopeDepth)
			{
				JsonNodeType[] array = new JsonNodeType[this.scopeDepth * 2];
				Array.Copy(this.scopes, array, this.scopeDepth);
				this.scopes = array;
			}
			this.scopes[this.scopeDepth] = currentNodeType;
		}

		private JsonNodeType ExitJsonScope()
		{
			JsonNodeType jsonNodeType = this.scopes[this.scopeDepth];
			this.scopes[this.scopeDepth] = JsonNodeType.None;
			this.scopeDepth--;
			return jsonNodeType;
		}

		private new void MoveToEndElement()
		{
			this.ExitJsonScope();
			base.MoveToEndElement();
		}

		private void MoveToInitial(XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
		{
			base.MoveToInitial(quotas);
			this.maxBytesPerRead = quotas.MaxBytesPerRead;
			this.onReaderClose = onClose;
		}

		private void ParseAndSetLocalName()
		{
			XmlBaseReader.XmlElementNode xmlElementNode = base.EnterScope();
			xmlElementNode.NameOffset = base.BufferReader.Offset;
			do
			{
				if (base.BufferReader.GetByte() == 92)
				{
					this.ReadEscapedCharacter(false);
				}
				else
				{
					this.ReadQuotedText(false);
				}
			}
			while (this.complexTextMode == XmlJsonReader.JsonComplexTextMode.QuotedText);
			int num = base.BufferReader.Offset - 1;
			xmlElementNode.LocalName.SetValue(xmlElementNode.NameOffset, num - xmlElementNode.NameOffset);
			xmlElementNode.NameLength = num - xmlElementNode.NameOffset;
			xmlElementNode.Namespace.Uri.SetValue(xmlElementNode.NameOffset, 0);
			xmlElementNode.Prefix.SetValue(PrefixHandleType.Empty);
			xmlElementNode.IsEmptyElement = false;
			xmlElementNode.ExitScope = false;
			xmlElementNode.BufferOffset = num;
			int num2 = (int)base.BufferReader.GetByte(xmlElementNode.NameOffset);
			if ((XmlJsonReader.charType[num2] & 1) == 0)
			{
				this.SetJsonNameWithMapping(xmlElementNode);
				return;
			}
			int i = 0;
			int num3 = xmlElementNode.NameOffset;
			while (i < xmlElementNode.NameLength)
			{
				num2 = (int)base.BufferReader.GetByte(num3);
				if ((XmlJsonReader.charType[num2] & 2) == 0 || num2 >= 128)
				{
					this.SetJsonNameWithMapping(xmlElementNode);
					return;
				}
				i++;
				num3++;
			}
		}

		private void ParseStartElement()
		{
			if (!this.buffered)
			{
				this.BufferElement();
			}
			this.expectingFirstElementInNonPrimitiveChild = false;
			byte @byte = base.BufferReader.GetByte();
			if (@byte == 34)
			{
				base.BufferReader.SkipByte();
				this.ParseAndSetLocalName();
				this.SkipWhitespaceInBufferReader();
				this.SkipExpectedByteInBufferReader(58);
				this.SkipWhitespaceInBufferReader();
				if (base.BufferReader.GetByte() == 123)
				{
					base.BufferReader.SkipByte();
					this.expectingFirstElementInNonPrimitiveChild = true;
				}
				this.ReadAttributes();
				return;
			}
			XmlExceptionHelper.ThrowTokenExpected(this, "\"", (char)@byte);
		}

		private void ReadAttributes()
		{
			XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddAttribute();
			xmlAttributeNode.LocalName.SetConstantValue(StringHandleConstStringType.Type);
			xmlAttributeNode.Namespace.Uri.SetValue(0, 0);
			xmlAttributeNode.Prefix.SetValue(PrefixHandleType.Empty);
			this.SkipWhitespaceInBufferReader();
			byte @byte = base.BufferReader.GetByte();
			if (@byte <= 102)
			{
				if (@byte != 34)
				{
					if (@byte == 91)
					{
						xmlAttributeNode.Value.SetConstantValue(ValueHandleConstStringType.Array);
						base.BufferReader.SkipByte();
						this.EnterJsonScope(JsonNodeType.Collection);
						return;
					}
					if (@byte != 102)
					{
						goto IL_0132;
					}
				}
				else
				{
					if (!this.expectingFirstElementInNonPrimitiveChild)
					{
						xmlAttributeNode.Value.SetConstantValue(ValueHandleConstStringType.String);
						return;
					}
					xmlAttributeNode.Value.SetConstantValue(ValueHandleConstStringType.Object);
					this.ReadServerTypeAttribute(true);
					return;
				}
			}
			else if (@byte <= 116)
			{
				if (@byte == 110)
				{
					xmlAttributeNode.Value.SetConstantValue(ValueHandleConstStringType.Null);
					return;
				}
				if (@byte != 116)
				{
					goto IL_0132;
				}
			}
			else
			{
				if (@byte == 123)
				{
					xmlAttributeNode.Value.SetConstantValue(ValueHandleConstStringType.Object);
					this.ReadServerTypeAttribute(false);
					return;
				}
				if (@byte != 125)
				{
					goto IL_0132;
				}
				if (this.expectingFirstElementInNonPrimitiveChild)
				{
					xmlAttributeNode.Value.SetConstantValue(ValueHandleConstStringType.Object);
					return;
				}
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an unexpected character '{0}' in JSON.", new object[] { (char)@byte })));
				return;
			}
			xmlAttributeNode.Value.SetConstantValue(ValueHandleConstStringType.Boolean);
			return;
			IL_0132:
			if (@byte == 45 || (@byte <= 57 && @byte >= 48) || @byte == 78 || @byte == 73)
			{
				xmlAttributeNode.Value.SetConstantValue(ValueHandleConstStringType.Number);
				return;
			}
			XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an unexpected character '{0}' in JSON.", new object[] { (char)@byte })));
		}

		private void ReadEscapedCharacter(bool moveToText)
		{
			base.BufferReader.SkipByte();
			char c = (char)base.BufferReader.GetByte();
			if (c == 'u')
			{
				base.BufferReader.SkipByte();
				int num;
				byte[] array = base.BufferReader.GetBuffer(5, out num);
				string text = Encoding.UTF8.GetString(array, num, 4);
				base.BufferReader.Advance(4);
				int num2 = (int)XmlJsonReader.ParseChar(text, NumberStyles.HexNumber);
				if (char.IsHighSurrogate((char)num2) && base.BufferReader.GetByte() == 92)
				{
					base.BufferReader.SkipByte();
					this.SkipExpectedByteInBufferReader(117);
					array = base.BufferReader.GetBuffer(5, out num);
					text = Encoding.UTF8.GetString(array, num, 4);
					base.BufferReader.Advance(4);
					char c2 = XmlJsonReader.ParseChar(text, NumberStyles.HexNumber);
					if (!char.IsLowSurrogate(c2))
					{
						XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Low surrogate char '0x{0}' not valid. Low surrogate chars range from 0xDC00 to 0xDFFF.", new object[] { text })));
					}
					num2 = new SurrogateChar(c2, (char)num2).Char;
				}
				if (array[num + 4] == 34)
				{
					base.BufferReader.SkipByte();
					if (moveToText)
					{
						base.MoveToAtomicText().Value.SetCharValue(num2);
					}
					this.complexTextMode = XmlJsonReader.JsonComplexTextMode.None;
					return;
				}
				if (moveToText)
				{
					base.MoveToComplexText().Value.SetCharValue(num2);
				}
				this.complexTextMode = XmlJsonReader.JsonComplexTextMode.QuotedText;
				return;
			}
			else
			{
				if (c <= 'b')
				{
					if (c <= '/')
					{
						if (c == '"' || c == '/')
						{
							goto IL_01CE;
						}
					}
					else
					{
						if (c == '\\')
						{
							goto IL_01CE;
						}
						if (c == 'b')
						{
							c = '\b';
							goto IL_01CE;
						}
					}
				}
				else if (c <= 'n')
				{
					if (c == 'f')
					{
						c = '\f';
						goto IL_01CE;
					}
					if (c == 'n')
					{
						c = '\n';
						goto IL_01CE;
					}
				}
				else
				{
					if (c == 'r')
					{
						c = '\r';
						goto IL_01CE;
					}
					if (c == 't')
					{
						c = '\t';
						goto IL_01CE;
					}
				}
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an unexpected character '{0}' in JSON.", new object[] { c })));
				IL_01CE:
				base.BufferReader.SkipByte();
				if (base.BufferReader.GetByte() == 34)
				{
					base.BufferReader.SkipByte();
					if (moveToText)
					{
						base.MoveToAtomicText().Value.SetCharValue((int)c);
					}
					this.complexTextMode = XmlJsonReader.JsonComplexTextMode.None;
					return;
				}
				if (moveToText)
				{
					base.MoveToComplexText().Value.SetCharValue((int)c);
				}
				this.complexTextMode = XmlJsonReader.JsonComplexTextMode.QuotedText;
				return;
			}
		}

		private void ReadNonExistentElementName(StringHandleConstStringType elementName)
		{
			this.EnterJsonScope(JsonNodeType.Object);
			XmlBaseReader.XmlElementNode xmlElementNode = base.EnterScope();
			xmlElementNode.LocalName.SetConstantValue(elementName);
			xmlElementNode.Namespace.Uri.SetValue(xmlElementNode.NameOffset, 0);
			xmlElementNode.Prefix.SetValue(PrefixHandleType.Empty);
			xmlElementNode.BufferOffset = base.BufferReader.Offset;
			xmlElementNode.IsEmptyElement = false;
			xmlElementNode.ExitScope = false;
			this.ReadAttributes();
		}

		private int ReadNonFFFE()
		{
			int num;
			byte[] buffer = base.BufferReader.GetBuffer(3, out num);
			if (buffer[num + 1] == 191 && (buffer[num + 2] == 190 || buffer[num + 2] == 191))
			{
				XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("FFFE in JSON is invalid.")));
			}
			return 3;
		}

		private void ReadNumericalText()
		{
			int num2;
			int num3;
			int num;
			if (this.buffered)
			{
				num = XmlJsonReader.ComputeNumericalTextLength(base.BufferReader.GetBuffer(out num2, out num3), num2, num3);
			}
			else
			{
				byte[] buffer = base.BufferReader.GetBuffer(2048, out num2, out num3);
				num = XmlJsonReader.ComputeNumericalTextLength(buffer, num2, num3);
				num = XmlJsonReader.BreakText(buffer, num2, num);
			}
			base.BufferReader.Advance(num);
			if (num2 <= num3 - num)
			{
				base.MoveToAtomicText().Value.SetValue(ValueHandleType.UTF8, num2, num);
				this.complexTextMode = XmlJsonReader.JsonComplexTextMode.None;
				return;
			}
			base.MoveToComplexText().Value.SetValue(ValueHandleType.UTF8, num2, num);
			this.complexTextMode = XmlJsonReader.JsonComplexTextMode.NumericalText;
		}

		private void ReadQuotedText(bool moveToText)
		{
			int offset;
			bool flag;
			int num;
			bool flag2;
			if (this.buffered)
			{
				int num2;
				num = XmlJsonReader.ComputeQuotedTextLengthUntilEndQuote(base.BufferReader.GetBuffer(out offset, out num2), offset, num2, out flag);
				flag2 = offset < num2 - num;
			}
			else
			{
				int num2;
				byte[] buffer = base.BufferReader.GetBuffer(2048, out offset, out num2);
				num = XmlJsonReader.ComputeQuotedTextLengthUntilEndQuote(buffer, offset, num2, out flag);
				flag2 = offset < num2 - num;
				num = XmlJsonReader.BreakText(buffer, offset, num);
			}
			if (flag && base.BufferReader.GetByte() == 239)
			{
				offset = base.BufferReader.Offset;
				num = this.ReadNonFFFE();
			}
			base.BufferReader.Advance(num);
			if (!flag && flag2)
			{
				if (moveToText)
				{
					base.MoveToAtomicText().Value.SetValue(ValueHandleType.UTF8, offset, num);
				}
				this.SkipExpectedByteInBufferReader(34);
				this.complexTextMode = XmlJsonReader.JsonComplexTextMode.None;
				return;
			}
			if (num == 0 && flag)
			{
				this.ReadEscapedCharacter(moveToText);
				return;
			}
			if (moveToText)
			{
				base.MoveToComplexText().Value.SetValue(ValueHandleType.UTF8, offset, num);
			}
			this.complexTextMode = XmlJsonReader.JsonComplexTextMode.QuotedText;
		}

		private void ReadServerTypeAttribute(bool consumedObjectChar)
		{
			if (!consumedObjectChar)
			{
				this.SkipExpectedByteInBufferReader(123);
				this.SkipWhitespaceInBufferReader();
				byte @byte = base.BufferReader.GetByte();
				if (@byte != 34 && @byte != 125)
				{
					XmlExceptionHelper.ThrowTokenExpected(this, "\"", (char)@byte);
				}
			}
			else
			{
				this.SkipWhitespaceInBufferReader();
			}
			int num;
			int num2;
			byte[] array = base.BufferReader.GetBuffer(8, out num, out num2);
			if (num + 8 <= num2 && array[num] == 34 && array[num + 1] == 95 && array[num + 2] == 95 && array[num + 3] == 116 && array[num + 4] == 121 && array[num + 5] == 112 && array[num + 6] == 101 && array[num + 7] == 34)
			{
				XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddAttribute();
				xmlAttributeNode.LocalName.SetValue(num + 1, 6);
				xmlAttributeNode.Namespace.Uri.SetValue(0, 0);
				xmlAttributeNode.Prefix.SetValue(PrefixHandleType.Empty);
				base.BufferReader.Advance(8);
				if (!this.buffered)
				{
					this.BufferElement();
				}
				this.SkipWhitespaceInBufferReader();
				this.SkipExpectedByteInBufferReader(58);
				this.SkipWhitespaceInBufferReader();
				this.SkipExpectedByteInBufferReader(34);
				array = base.BufferReader.GetBuffer(out num, out num2);
				do
				{
					if (base.BufferReader.GetByte() == 92)
					{
						this.ReadEscapedCharacter(false);
					}
					else
					{
						this.ReadQuotedText(false);
					}
				}
				while (this.complexTextMode == XmlJsonReader.JsonComplexTextMode.QuotedText);
				xmlAttributeNode.Value.SetValue(ValueHandleType.UTF8, num, base.BufferReader.Offset - 1 - num);
				this.SkipWhitespaceInBufferReader();
				if (base.BufferReader.GetByte() == 44)
				{
					base.BufferReader.SkipByte();
					this.readServerTypeElement = true;
				}
			}
			if (base.BufferReader.GetByte() == 125)
			{
				base.BufferReader.SkipByte();
				this.readServerTypeElement = false;
				this.expectingFirstElementInNonPrimitiveChild = false;
				return;
			}
			this.readServerTypeElement = true;
		}

		private void ResetState()
		{
			this.complexTextMode = XmlJsonReader.JsonComplexTextMode.None;
			this.expectingFirstElementInNonPrimitiveChild = false;
			this.charactersToSkipOnNextRead = new byte[2];
			this.scopeDepth = 0;
			if (this.scopes != null && this.scopes.Length > 25)
			{
				this.scopes = null;
			}
		}

		private void SetJsonNameWithMapping(XmlBaseReader.XmlElementNode elementNode)
		{
			XmlBaseReader.Namespace @namespace = base.AddNamespace();
			@namespace.Prefix.SetValue(PrefixHandleType.A);
			@namespace.Uri.SetConstantValue(StringHandleConstStringType.Item);
			base.AddXmlnsAttribute(@namespace);
			XmlBaseReader.XmlAttributeNode xmlAttributeNode = base.AddAttribute();
			xmlAttributeNode.LocalName.SetConstantValue(StringHandleConstStringType.Item);
			xmlAttributeNode.Namespace.Uri.SetValue(0, 0);
			xmlAttributeNode.Prefix.SetValue(PrefixHandleType.Empty);
			xmlAttributeNode.Value.SetValue(ValueHandleType.UTF8, elementNode.NameOffset, elementNode.NameLength);
			elementNode.NameLength = 0;
			elementNode.Prefix.SetValue(PrefixHandleType.A);
			elementNode.LocalName.SetConstantValue(StringHandleConstStringType.Item);
			elementNode.Namespace = @namespace;
		}

		private void SkipExpectedByteInBufferReader(byte characterToSkip)
		{
			if (base.BufferReader.GetByte() != characterToSkip)
			{
				char c = (char)characterToSkip;
				XmlExceptionHelper.ThrowTokenExpected(this, c.ToString(), (char)base.BufferReader.GetByte());
			}
			base.BufferReader.SkipByte();
		}

		private void SkipWhitespaceInBufferReader()
		{
			byte b;
			while (this.TryGetByte(out b) && XmlJsonReader.IsWhitespace(b))
			{
				base.BufferReader.SkipByte();
			}
		}

		private bool TryGetByte(out byte ch)
		{
			int num;
			int num2;
			byte[] buffer = base.BufferReader.GetBuffer(1, out num, out num2);
			if (num < num2)
			{
				ch = buffer[num];
				return true;
			}
			ch = 0;
			return false;
		}

		private string UnescapeJsonString(string val)
		{
			if (val == null)
			{
				return null;
			}
			StringBuilder stringBuilder = null;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < val.Length; i++)
			{
				if (val[i] == '\\')
				{
					i++;
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					stringBuilder.Append(val, num, num2);
					if (i >= val.Length)
					{
						XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an unexpected character '{0}' in JSON.", new object[] { val[i] })));
					}
					char c = val[i];
					if (c <= '\\')
					{
						if (c <= '\'')
						{
							if (c != '"' && c != '\'')
							{
								goto IL_017D;
							}
						}
						else if (c != '/' && c != '\\')
						{
							goto IL_017D;
						}
						stringBuilder.Append(val[i]);
					}
					else if (c <= 'f')
					{
						if (c != 'b')
						{
							if (c == 'f')
							{
								stringBuilder.Append('\f');
							}
						}
						else
						{
							stringBuilder.Append('\b');
						}
					}
					else if (c != 'n')
					{
						switch (c)
						{
						case 'r':
							stringBuilder.Append('\r');
							break;
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'u':
							if (i + 3 >= val.Length)
							{
								XmlExceptionHelper.ThrowXmlException(this, new XmlException(global::System.Runtime.Serialization.SR.GetString("Encountered an unexpected character '{0}' in JSON.", new object[] { val[i] })));
							}
							stringBuilder.Append(XmlJsonReader.ParseChar(val.Substring(i + 1, 4), NumberStyles.HexNumber));
							i += 4;
							break;
						}
					}
					else
					{
						stringBuilder.Append('\n');
					}
					IL_017D:
					num = i + 1;
					num2 = 0;
				}
				else
				{
					num2++;
				}
			}
			if (stringBuilder == null)
			{
				return val;
			}
			if (num2 > 0)
			{
				stringBuilder.Append(val, num, num2);
			}
			return stringBuilder.ToString();
		}

		private const int MaxTextChunk = 2048;

		private static byte[] charType = new byte[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 2, 2, 0, 2, 2,
			2, 2, 2, 2, 2, 2, 2, 2, 0, 0,
			0, 0, 0, 0, 0, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 0, 0, 0, 0, 3, 0, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 0, 0, 0, 0, 0, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
			3, 3, 3, 3, 3, 3
		};

		private bool buffered;

		private byte[] charactersToSkipOnNextRead;

		private XmlJsonReader.JsonComplexTextMode complexTextMode = XmlJsonReader.JsonComplexTextMode.None;

		private bool expectingFirstElementInNonPrimitiveChild;

		private int maxBytesPerRead;

		private OnXmlDictionaryReaderClose onReaderClose;

		private bool readServerTypeElement;

		private int scopeDepth;

		private JsonNodeType[] scopes;

		private enum JsonComplexTextMode
		{
			QuotedText,
			NumericalText,
			None
		}

		private static class CharType
		{
			public const byte FirstName = 1;

			public const byte Name = 2;

			public const byte None = 0;
		}
	}
}
