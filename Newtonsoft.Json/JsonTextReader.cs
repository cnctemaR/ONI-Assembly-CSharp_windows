using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json
{
	public class JsonTextReader : JsonReader, IJsonLineInfo
	{
		public JsonTextReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this._reader = reader;
			this._lineNumber = 1;
			this._chars = new char[1025];
		}

		private StringBuffer GetBuffer()
		{
			if (this._buffer == null)
			{
				this._buffer = new StringBuffer(1025);
			}
			else
			{
				this._buffer.Position = 0;
			}
			return this._buffer;
		}

		private void OnNewLine(int pos)
		{
			this._lineNumber++;
			this._lineStartPos = pos - 1;
		}

		private void ParseString(char quote)
		{
			this._charPos++;
			this.ShiftBufferIfNeeded();
			this.ReadStringIntoBuffer(quote);
			base.SetPostValueState(true);
			if (this._readType == ReadType.ReadAsBytes)
			{
				byte[] array;
				Guid guid;
				if (this._stringReference.Length == 0)
				{
					array = new byte[0];
				}
				else if (this._stringReference.Length == 36 && ConvertUtils.TryConvertGuid(this._stringReference.ToString(), out guid))
				{
					array = guid.ToByteArray();
				}
				else
				{
					array = Convert.FromBase64CharArray(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
				}
				base.SetToken(JsonToken.Bytes, array, false);
				return;
			}
			if (this._readType == ReadType.ReadAsString)
			{
				string text = this._stringReference.ToString();
				base.SetToken(JsonToken.String, text, false);
				this._quoteChar = quote;
				return;
			}
			string text2 = this._stringReference.ToString();
			if (this._dateParseHandling != DateParseHandling.None)
			{
				DateParseHandling dateParseHandling;
				if (this._readType == ReadType.ReadAsDateTime)
				{
					dateParseHandling = DateParseHandling.DateTime;
				}
				else if (this._readType == ReadType.ReadAsDateTimeOffset)
				{
					dateParseHandling = DateParseHandling.DateTimeOffset;
				}
				else
				{
					dateParseHandling = this._dateParseHandling;
				}
				object obj;
				if (DateTimeUtils.TryParseDateTime(text2, dateParseHandling, base.DateTimeZoneHandling, base.DateFormatString, base.Culture, out obj))
				{
					base.SetToken(JsonToken.Date, obj, false);
					return;
				}
			}
			base.SetToken(JsonToken.String, text2, false);
			this._quoteChar = quote;
		}

		private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
		}

		private void ShiftBufferIfNeeded()
		{
			int num = this._chars.Length;
			if ((double)(num - this._charPos) <= (double)num * 0.1)
			{
				int num2 = this._charsUsed - this._charPos;
				if (num2 > 0)
				{
					JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
				}
				this._lineStartPos -= this._charPos;
				this._charPos = 0;
				this._charsUsed = num2;
				this._chars[this._charsUsed] = '\0';
			}
		}

		private int ReadData(bool append)
		{
			return this.ReadData(append, 0);
		}

		private int ReadData(bool append, int charsRequired)
		{
			if (this._isEndOfFile)
			{
				return 0;
			}
			if (this._charsUsed + charsRequired >= this._chars.Length - 1)
			{
				if (append)
				{
					int num = Math.Max(this._chars.Length * 2, this._charsUsed + charsRequired + 1);
					char[] array = new char[num];
					JsonTextReader.BlockCopyChars(this._chars, 0, array, 0, this._chars.Length);
					this._chars = array;
				}
				else
				{
					int num2 = this._charsUsed - this._charPos;
					if (num2 + charsRequired + 1 >= this._chars.Length)
					{
						char[] array2 = new char[num2 + charsRequired + 1];
						if (num2 > 0)
						{
							JsonTextReader.BlockCopyChars(this._chars, this._charPos, array2, 0, num2);
						}
						this._chars = array2;
					}
					else if (num2 > 0)
					{
						JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
					}
					this._lineStartPos -= this._charPos;
					this._charPos = 0;
					this._charsUsed = num2;
				}
			}
			int num3 = this._chars.Length - this._charsUsed - 1;
			int num4 = this._reader.Read(this._chars, this._charsUsed, num3);
			this._charsUsed += num4;
			if (num4 == 0)
			{
				this._isEndOfFile = true;
			}
			this._chars[this._charsUsed] = '\0';
			return num4;
		}

		private bool EnsureChars(int relativePosition, bool append)
		{
			return this._charPos + relativePosition < this._charsUsed || this.ReadChars(relativePosition, append);
		}

		private bool ReadChars(int relativePosition, bool append)
		{
			if (this._isEndOfFile)
			{
				return false;
			}
			int num = this._charPos + relativePosition - this._charsUsed + 1;
			int num2 = 0;
			do
			{
				int num3 = this.ReadData(append, num - num2);
				if (num3 == 0)
				{
					break;
				}
				num2 += num3;
			}
			while (num2 < num);
			return num2 >= num;
		}

		[DebuggerStepThrough]
		public override bool Read()
		{
			this._readType = ReadType.Read;
			if (!this.ReadInternal())
			{
				base.SetToken(JsonToken.None);
				return false;
			}
			return true;
		}

		public override byte[] ReadAsBytes()
		{
			return base.ReadAsBytesInternal();
		}

		public override decimal? ReadAsDecimal()
		{
			return base.ReadAsDecimalInternal();
		}

		public override int? ReadAsInt32()
		{
			return base.ReadAsInt32Internal();
		}

		public override string ReadAsString()
		{
			return base.ReadAsStringInternal();
		}

		public override DateTime? ReadAsDateTime()
		{
			return base.ReadAsDateTimeInternal();
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			return base.ReadAsDateTimeOffsetInternal();
		}

		internal override bool ReadInternal()
		{
			for (;;)
			{
				switch (this._currentState)
				{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
					goto IL_0043;
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
					goto IL_004A;
				case JsonReader.State.PostValue:
					if (this.ParsePostValue())
					{
						return true;
					}
					continue;
				case JsonReader.State.Finished:
					goto IL_005B;
				}
				break;
			}
			goto IL_00BA;
			IL_0043:
			return this.ParseValue();
			IL_004A:
			return this.ParseObject();
			IL_005B:
			if (!this.EnsureChars(0, false))
			{
				return false;
			}
			this.EatWhitespace(false);
			if (this._isEndOfFile)
			{
				return false;
			}
			if (this._chars[this._charPos] == '/')
			{
				this.ParseComment();
				return true;
			}
			throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			IL_00BA:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		private void ReadStringIntoBuffer(char quote)
		{
			int num = this._charPos;
			int charPos = this._charPos;
			int num2 = this._charPos;
			StringBuffer stringBuffer = null;
			char c2;
			for (;;)
			{
				char c = this._chars[num++];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						if (c != '\n')
						{
							if (c == '\r')
							{
								this._charPos = num - 1;
								this.ProcessCarriageReturn(true);
								num = this._charPos;
							}
						}
						else
						{
							this._charPos = num - 1;
							this.ProcessLineFeed();
							num = this._charPos;
						}
					}
					else if (this._charsUsed == num - 1)
					{
						num--;
						if (this.ReadData(true) == 0)
						{
							break;
						}
					}
				}
				else if (c != '"' && c != '\'')
				{
					if (c == '\\')
					{
						this._charPos = num;
						if (!this.EnsureChars(0, true))
						{
							goto Block_10;
						}
						int num3 = num - 1;
						c2 = this._chars[num];
						char c3 = c2;
						char c4;
						if (c3 <= '\\')
						{
							if (c3 <= '\'')
							{
								if (c3 != '"' && c3 != '\'')
								{
									goto Block_14;
								}
							}
							else if (c3 != '/')
							{
								if (c3 != '\\')
								{
									goto Block_16;
								}
								num++;
								c4 = '\\';
								goto IL_02BF;
							}
							c4 = c2;
							num++;
						}
						else if (c3 <= 'f')
						{
							if (c3 != 'b')
							{
								if (c3 != 'f')
								{
									goto Block_19;
								}
								num++;
								c4 = '\f';
							}
							else
							{
								num++;
								c4 = '\b';
							}
						}
						else
						{
							if (c3 != 'n')
							{
								switch (c3)
								{
								case 'r':
									num++;
									c4 = '\r';
									goto IL_02BF;
								case 't':
									num++;
									c4 = '\t';
									goto IL_02BF;
								case 'u':
									num++;
									this._charPos = num;
									c4 = this.ParseUnicode();
									if (StringUtils.IsLowSurrogate(c4))
									{
										c4 = '\ufffd';
									}
									else if (StringUtils.IsHighSurrogate(c4))
									{
										bool flag;
										do
										{
											flag = false;
											if (this.EnsureChars(2, true) && this._chars[this._charPos] == '\\' && this._chars[this._charPos + 1] == 'u')
											{
												char c5 = c4;
												this._charPos += 2;
												c4 = this.ParseUnicode();
												if (!StringUtils.IsLowSurrogate(c4))
												{
													if (StringUtils.IsHighSurrogate(c4))
													{
														c5 = '\ufffd';
														flag = true;
													}
													else
													{
														c5 = '\ufffd';
													}
												}
												if (stringBuffer == null)
												{
													stringBuffer = this.GetBuffer();
												}
												this.WriteCharToBuffer(stringBuffer, c5, num2, num3);
												num2 = this._charPos;
											}
											else
											{
												c4 = '\ufffd';
											}
										}
										while (flag);
									}
									num = this._charPos;
									goto IL_02BF;
								}
								goto Block_21;
							}
							num++;
							c4 = '\n';
						}
						IL_02BF:
						if (stringBuffer == null)
						{
							stringBuffer = this.GetBuffer();
						}
						this.WriteCharToBuffer(stringBuffer, c4, num2, num3);
						num2 = num;
					}
				}
				else if (this._chars[num - 1] == quote)
				{
					goto Block_30;
				}
			}
			this._charPos = num;
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_10:
			this._charPos = num;
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_14:
			Block_16:
			Block_19:
			Block_21:
			num++;
			this._charPos = num;
			throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, "\\" + c2));
			Block_30:
			num--;
			if (charPos == num2)
			{
				this._stringReference = new StringReference(this._chars, charPos, num - charPos);
			}
			else
			{
				if (stringBuffer == null)
				{
					stringBuffer = this.GetBuffer();
				}
				if (num > num2)
				{
					stringBuffer.Append(this._chars, num2, num - num2);
				}
				this._stringReference = new StringReference(stringBuffer.GetInternalBuffer(), 0, stringBuffer.Position);
			}
			num++;
			this._charPos = num;
		}

		private void WriteCharToBuffer(StringBuffer buffer, char writeChar, int lastWritePosition, int writeToPosition)
		{
			if (writeToPosition > lastWritePosition)
			{
				buffer.Append(this._chars, lastWritePosition, writeToPosition - lastWritePosition);
			}
			buffer.Append(writeChar);
		}

		private char ParseUnicode()
		{
			if (this.EnsureChars(4, true))
			{
				string text = new string(this._chars, this._charPos, 4);
				char c = Convert.ToChar(int.Parse(text, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo));
				char c2 = c;
				this._charPos += 4;
				return c2;
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing unicode character.");
		}

		private void ReadNumberIntoBuffer()
		{
			int num = this._charPos;
			for (;;)
			{
				char c = this._chars[num];
				if (c <= 'F')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '+':
						case '-':
						case '.':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
						case 'A':
						case 'B':
						case 'C':
						case 'D':
						case 'E':
						case 'F':
							goto IL_00E4;
						}
						break;
					}
					this._charPos = num;
					if (this._charsUsed != num || this.ReadData(true) == 0)
					{
						return;
					}
					continue;
				}
				else if (c != 'X')
				{
					switch (c)
					{
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
						break;
					default:
						if (c != 'x')
						{
							goto Block_6;
						}
						break;
					}
				}
				IL_00E4:
				num++;
			}
			Block_6:
			this._charPos = num;
			char c2 = this._chars[this._charPos];
			if (char.IsWhiteSpace(c2) || c2 == ',' || c2 == '}' || c2 == ']' || c2 == ')' || c2 == '/')
			{
				return;
			}
			throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, c2));
		}

		private void ClearRecentString()
		{
			if (this._buffer != null)
			{
				this._buffer.Position = 0;
			}
			this._stringReference = default(StringReference);
		}

		private bool ParsePostValue()
		{
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				char c2 = c;
				if (c2 <= ')')
				{
					if (c2 <= '\r')
					{
						if (c2 != '\0')
						{
							switch (c2)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_0145;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_0145;
							}
						}
						else
						{
							if (this._charsUsed != this._charPos)
							{
								this._charPos++;
								continue;
							}
							if (this.ReadData(false) == 0)
							{
								break;
							}
							continue;
						}
					}
					else if (c2 != ' ')
					{
						if (c2 != ')')
						{
							goto IL_0145;
						}
						goto IL_00E5;
					}
					this._charPos++;
					continue;
				}
				if (c2 <= '/')
				{
					if (c2 == ',')
					{
						goto IL_0105;
					}
					if (c2 == '/')
					{
						goto IL_00FD;
					}
				}
				else
				{
					if (c2 == ']')
					{
						goto IL_00CD;
					}
					if (c2 == '}')
					{
						goto IL_00B5;
					}
				}
				IL_0145:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_0160;
				}
				this._charPos++;
			}
			this._currentState = JsonReader.State.Finished;
			return false;
			IL_00B5:
			this._charPos++;
			base.SetToken(JsonToken.EndObject);
			return true;
			IL_00CD:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_00E5:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_00FD:
			this.ParseComment();
			return true;
			IL_0105:
			this._charPos++;
			base.SetStateBasedOnCurrent();
			return false;
			IL_0160:
			throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		private bool ParseObject()
		{
			for (;;)
			{
				char c = this._chars[this._charPos];
				char c2 = c;
				if (c2 <= '\r')
				{
					if (c2 != '\0')
					{
						switch (c2)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_00BF;
						case '\r':
							this.ProcessCarriageReturn(false);
							continue;
						default:
							goto IL_00BF;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(false) == 0)
						{
							break;
						}
						continue;
					}
				}
				else if (c2 != ' ')
				{
					if (c2 == '/')
					{
						goto IL_008D;
					}
					if (c2 != '}')
					{
						goto IL_00BF;
					}
					goto IL_0075;
				}
				this._charPos++;
				continue;
				IL_00BF:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_00DA;
				}
				this._charPos++;
			}
			return false;
			IL_0075:
			base.SetToken(JsonToken.EndObject);
			this._charPos++;
			return true;
			IL_008D:
			this.ParseComment();
			return true;
			IL_00DA:
			return this.ParseProperty();
		}

		private bool ParseProperty()
		{
			char c = this._chars[this._charPos];
			char c2;
			if (c == '"' || c == '\'')
			{
				this._charPos++;
				c2 = c;
				this.ShiftBufferIfNeeded();
				this.ReadStringIntoBuffer(c2);
			}
			else
			{
				if (!this.ValidIdentifierChar(c))
				{
					throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				c2 = '\0';
				this.ShiftBufferIfNeeded();
				this.ParseUnquotedProperty();
			}
			string text;
			if (this.NameTable != null)
			{
				text = this.NameTable.Get(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
				if (text == null)
				{
					text = this._stringReference.ToString();
				}
			}
			else
			{
				text = this._stringReference.ToString();
			}
			this.EatWhitespace(false);
			if (this._chars[this._charPos] != ':')
			{
				throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			base.SetToken(JsonToken.PropertyName, text);
			this._quoteChar = c2;
			this.ClearRecentString();
			return true;
		}

		private bool ValidIdentifierChar(char value)
		{
			return char.IsLetterOrDigit(value) || value == '_' || value == '$';
		}

		private void ParseUnquotedProperty()
		{
			int charPos = this._charPos;
			char c2;
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_003C;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					c2 = this._chars[this._charPos];
					if (!this.ValidIdentifierChar(c2))
					{
						goto IL_007E;
					}
					this._charPos++;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
			IL_003C:
			this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
			return;
			IL_007E:
			if (char.IsWhiteSpace(c2) || c2 == ':')
			{
				this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
				return;
			}
			throw JsonReaderException.Create(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, c2));
		}

		private bool ParseValue()
		{
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				char c2 = c;
				if (c2 <= 'I')
				{
					if (c2 <= '\r')
					{
						if (c2 != '\0')
						{
							switch (c2)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_0272;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_0272;
							}
						}
						else
						{
							if (this._charsUsed != this._charPos)
							{
								this._charPos++;
								continue;
							}
							if (this.ReadData(false) == 0)
							{
								break;
							}
							continue;
						}
					}
					else
					{
						switch (c2)
						{
						case ' ':
							break;
						case '!':
							goto IL_0272;
						case '"':
							goto IL_0110;
						default:
							switch (c2)
							{
							case '\'':
								goto IL_0110;
							case '(':
							case '*':
							case '+':
							case '.':
								goto IL_0272;
							case ')':
								goto IL_0230;
							case ',':
								goto IL_0226;
							case '-':
								goto IL_01A3;
							case '/':
								goto IL_01D0;
							default:
								if (c2 != 'I')
								{
									goto IL_0272;
								}
								goto IL_019B;
							}
							break;
						}
					}
					this._charPos++;
					continue;
				}
				if (c2 <= 'f')
				{
					if (c2 == 'N')
					{
						goto IL_0193;
					}
					switch (c2)
					{
					case '[':
						goto IL_01F7;
					case '\\':
						break;
					case ']':
						goto IL_020E;
					default:
						if (c2 == 'f')
						{
							goto IL_0121;
						}
						break;
					}
				}
				else
				{
					if (c2 == 'n')
					{
						goto IL_0129;
					}
					switch (c2)
					{
					case 't':
						goto IL_0119;
					case 'u':
						goto IL_01D8;
					default:
						if (c2 == '{')
						{
							goto IL_01E0;
						}
						break;
					}
				}
				IL_0272:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_028D;
				}
				this._charPos++;
			}
			return false;
			IL_0110:
			this.ParseString(c);
			return true;
			IL_0119:
			this.ParseTrue();
			return true;
			IL_0121:
			this.ParseFalse();
			return true;
			IL_0129:
			if (this.EnsureChars(1, true))
			{
				char c3 = this._chars[this._charPos + 1];
				if (c3 == 'u')
				{
					this.ParseNull();
				}
				else
				{
					if (c3 != 'e')
					{
						throw JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
					}
					this.ParseConstructor();
				}
				return true;
			}
			throw JsonReaderException.Create(this, "Unexpected end.");
			IL_0193:
			this.ParseNumberNaN();
			return true;
			IL_019B:
			this.ParseNumberPositiveInfinity();
			return true;
			IL_01A3:
			if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
			{
				this.ParseNumberNegativeInfinity();
			}
			else
			{
				this.ParseNumber();
			}
			return true;
			IL_01D0:
			this.ParseComment();
			return true;
			IL_01D8:
			this.ParseUndefined();
			return true;
			IL_01E0:
			this._charPos++;
			base.SetToken(JsonToken.StartObject);
			return true;
			IL_01F7:
			this._charPos++;
			base.SetToken(JsonToken.StartArray);
			return true;
			IL_020E:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_0226:
			base.SetToken(JsonToken.Undefined);
			return true;
			IL_0230:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_028D:
			if (char.IsNumber(c) || c == '-' || c == '.')
			{
				this.ParseNumber();
				return true;
			}
			throw JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		private void ProcessLineFeed()
		{
			this._charPos++;
			this.OnNewLine(this._charPos);
		}

		private void ProcessCarriageReturn(bool append)
		{
			this._charPos++;
			if (this.EnsureChars(1, append) && this._chars[this._charPos] == '\n')
			{
				this._charPos++;
			}
			this.OnNewLine(this._charPos);
		}

		private bool EatWhitespace(bool oneOrMore)
		{
			bool flag = false;
			bool flag2 = false;
			while (!flag)
			{
				char c = this._chars[this._charPos];
				char c2 = c;
				if (c2 != '\0')
				{
					if (c2 != '\n')
					{
						if (c2 != '\r')
						{
							if (c == ' ' || char.IsWhiteSpace(c))
							{
								flag2 = true;
								this._charPos++;
							}
							else
							{
								flag = true;
							}
						}
						else
						{
							this.ProcessCarriageReturn(false);
						}
					}
					else
					{
						this.ProcessLineFeed();
					}
				}
				else if (this._charsUsed == this._charPos)
				{
					if (this.ReadData(false) == 0)
					{
						flag = true;
					}
				}
				else
				{
					this._charPos++;
				}
			}
			return !oneOrMore || flag2;
		}

		private void ParseConstructor()
		{
			if (!this.MatchValueWithTrailingSeparator("new"))
			{
				throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
			}
			this.EatWhitespace(false);
			int charPos = this._charPos;
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_0053;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					if (!char.IsLetterOrDigit(c))
					{
						goto IL_0085;
					}
					this._charPos++;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
			IL_0053:
			int num = this._charPos;
			this._charPos++;
			goto IL_00F7;
			IL_0085:
			if (c == '\r')
			{
				num = this._charPos;
				this.ProcessCarriageReturn(true);
			}
			else if (c == '\n')
			{
				num = this._charPos;
				this.ProcessLineFeed();
			}
			else if (char.IsWhiteSpace(c))
			{
				num = this._charPos;
				this._charPos++;
			}
			else
			{
				if (c != '(')
				{
					throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
				}
				num = this._charPos;
			}
			IL_00F7:
			this._stringReference = new StringReference(this._chars, charPos, num - charPos);
			string text = this._stringReference.ToString();
			this.EatWhitespace(false);
			if (this._chars[this._charPos] != '(')
			{
				throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			this.ClearRecentString();
			base.SetToken(JsonToken.StartConstructor, text);
		}

		private void ParseNumber()
		{
			this.ShiftBufferIfNeeded();
			char c = this._chars[this._charPos];
			int charPos = this._charPos;
			this.ReadNumberIntoBuffer();
			base.SetPostValueState(true);
			this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
			bool flag = char.IsDigit(c) && this._stringReference.Length == 1;
			bool flag2 = c == '0' && this._stringReference.Length > 1 && this._stringReference.Chars[this._stringReference.StartIndex + 1] != '.' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'e' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'E';
			object obj;
			JsonToken jsonToken;
			if (this._readType == ReadType.ReadAsInt32)
			{
				if (flag)
				{
					obj = (int)(c - '0');
				}
				else
				{
					if (flag2)
					{
						string text = this._stringReference.ToString();
						try
						{
							int num = (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(text, 16) : Convert.ToInt32(text, 8));
							obj = num;
							goto IL_01DE;
						}
						catch (Exception ex)
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, text), ex);
						}
					}
					int num2;
					ParseResult parseResult = ConvertUtils.Int32TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num2);
					if (parseResult == ParseResult.Success)
					{
						obj = num2;
					}
					else
					{
						if (parseResult == ParseResult.Overflow)
						{
							throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int32.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
						}
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
				}
				IL_01DE:
				jsonToken = JsonToken.Integer;
			}
			else if (this._readType == ReadType.ReadAsDecimal)
			{
				if (flag)
				{
					obj = c - 48m;
				}
				else
				{
					if (flag2)
					{
						string text2 = this._stringReference.ToString();
						try
						{
							long num3 = (text2.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text2, 16) : Convert.ToInt64(text2, 8));
							obj = Convert.ToDecimal(num3);
							goto IL_02D1;
						}
						catch (Exception ex2)
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, text2), ex2);
						}
					}
					string text3 = this._stringReference.ToString();
					decimal num4;
					if (!decimal.TryParse(text3, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num4))
					{
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					obj = num4;
				}
				IL_02D1:
				jsonToken = JsonToken.Float;
			}
			else if (flag)
			{
				obj = (long)((ulong)c - 48UL);
				jsonToken = JsonToken.Integer;
			}
			else if (flag2)
			{
				string text4 = this._stringReference.ToString();
				try
				{
					obj = (text4.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text4, 16) : Convert.ToInt64(text4, 8));
				}
				catch (Exception ex3)
				{
					throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text4), ex3);
				}
				jsonToken = JsonToken.Integer;
			}
			else
			{
				long num5;
				ParseResult parseResult2 = ConvertUtils.Int64TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num5);
				if (parseResult2 == ParseResult.Success)
				{
					obj = num5;
					jsonToken = JsonToken.Integer;
				}
				else
				{
					if (parseResult2 == ParseResult.Overflow)
					{
						throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int64.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					string text5 = this._stringReference.ToString();
					if (this._floatParseHandling == FloatParseHandling.Decimal)
					{
						decimal num6;
						if (!decimal.TryParse(text5, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num6))
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, text5));
						}
						obj = num6;
					}
					else
					{
						double num7;
						if (!double.TryParse(text5, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num7))
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text5));
						}
						obj = num7;
					}
					jsonToken = JsonToken.Float;
				}
			}
			this.ClearRecentString();
			base.SetToken(jsonToken, obj, false);
		}

		private void ParseComment()
		{
			this._charPos++;
			if (!this.EnsureChars(1, false))
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			bool flag;
			if (this._chars[this._charPos] == '*')
			{
				flag = false;
			}
			else
			{
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				flag = true;
			}
			this._charPos++;
			int charPos = this._charPos;
			bool flag2 = false;
			while (!flag2)
			{
				char c = this._chars[this._charPos];
				if (c <= '\n')
				{
					if (c != '\0')
					{
						if (c == '\n')
						{
							if (flag)
							{
								this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
								flag2 = true;
							}
							this.ProcessLineFeed();
							continue;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(true) != 0)
						{
							continue;
						}
						if (!flag)
						{
							throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
						}
						this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
						flag2 = true;
						continue;
					}
				}
				else
				{
					if (c == '\r')
					{
						if (flag)
						{
							this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
							flag2 = true;
						}
						this.ProcessCarriageReturn(true);
						continue;
					}
					if (c == '*')
					{
						this._charPos++;
						if (!flag && this.EnsureChars(0, true) && this._chars[this._charPos] == '/')
						{
							this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos - 1);
							this._charPos++;
							flag2 = true;
							continue;
						}
						continue;
					}
				}
				this._charPos++;
			}
			base.SetToken(JsonToken.Comment, this._stringReference.ToString());
			this.ClearRecentString();
		}

		private bool MatchValue(string value)
		{
			if (!this.EnsureChars(value.Length - 1, true))
			{
				return false;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (this._chars[this._charPos + i] != value[i])
				{
					return false;
				}
			}
			this._charPos += value.Length;
			return true;
		}

		private bool MatchValueWithTrailingSeparator(string value)
		{
			return this.MatchValue(value) && (!this.EnsureChars(0, false) || this.IsSeparator(this._chars[this._charPos]) || this._chars[this._charPos] == '\0');
		}

		private bool IsSeparator(char c)
		{
			if (c <= ')')
			{
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
					break;
				case '\v':
				case '\f':
					goto IL_008E;
				default:
					if (c != ' ')
					{
						if (c != ')')
						{
							goto IL_008E;
						}
						if (base.CurrentState == JsonReader.State.Constructor || base.CurrentState == JsonReader.State.ConstructorStart)
						{
							return true;
						}
						return false;
					}
					break;
				}
				return true;
			}
			if (c <= '/')
			{
				if (c != ',')
				{
					if (c != '/')
					{
						goto IL_008E;
					}
					if (!this.EnsureChars(1, false))
					{
						return false;
					}
					char c2 = this._chars[this._charPos + 1];
					return c2 == '*' || c2 == '/';
				}
			}
			else if (c != ']' && c != '}')
			{
				goto IL_008E;
			}
			return true;
			IL_008E:
			if (char.IsWhiteSpace(c))
			{
				return true;
			}
			return false;
		}

		private void ParseTrue()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.True))
			{
				base.SetToken(JsonToken.Boolean, true);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		private void ParseNull()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.Null))
			{
				base.SetToken(JsonToken.Null);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing null value.");
		}

		private void ParseUndefined()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.Undefined))
			{
				base.SetToken(JsonToken.Undefined);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing undefined value.");
		}

		private void ParseFalse()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.False))
			{
				base.SetToken(JsonToken.Boolean, false);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		private void ParseNumberNegativeInfinity()
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.NegativeInfinity))
			{
				throw JsonReaderException.Create(this, "Error parsing negative infinity value.");
			}
			if (this._floatParseHandling == FloatParseHandling.Decimal)
			{
				throw new JsonReaderException("Cannot read -Infinity as a decimal.");
			}
			base.SetToken(JsonToken.Float, double.NegativeInfinity);
		}

		private void ParseNumberPositiveInfinity()
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.PositiveInfinity))
			{
				throw JsonReaderException.Create(this, "Error parsing positive infinity value.");
			}
			if (this._floatParseHandling == FloatParseHandling.Decimal)
			{
				throw new JsonReaderException("Cannot read Infinity as a decimal.");
			}
			base.SetToken(JsonToken.Float, double.PositiveInfinity);
		}

		private void ParseNumberNaN()
		{
			if (!this.MatchValueWithTrailingSeparator(JsonConvert.NaN))
			{
				throw JsonReaderException.Create(this, "Error parsing NaN value.");
			}
			if (this._floatParseHandling == FloatParseHandling.Decimal)
			{
				throw new JsonReaderException("Cannot read NaN as a decimal.");
			}
			base.SetToken(JsonToken.Float, double.NaN);
		}

		public override void Close()
		{
			base.Close();
			if (base.CloseInput && this._reader != null)
			{
				this._reader.Close();
			}
			if (this._buffer != null)
			{
				this._buffer.Clear();
			}
		}

		public bool HasLineInfo()
		{
			return true;
		}

		public int LineNumber
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start && this.LinePosition == 0)
				{
					return 0;
				}
				return this._lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this._charPos - this._lineStartPos;
			}
		}

		private const char UnicodeReplacementChar = '\ufffd';

		private const int MaximumJavascriptIntegerCharacterLength = 380;

		private readonly TextReader _reader;

		private char[] _chars;

		private int _charsUsed;

		private int _charPos;

		private int _lineStartPos;

		private int _lineNumber;

		private bool _isEndOfFile;

		private StringBuffer _buffer;

		private StringReference _stringReference;

		internal PropertyNameTable NameTable;
	}
}
