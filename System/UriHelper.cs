using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
	internal static class UriHelper
	{
		internal unsafe static bool TestForSubPath(char* pMe, ushort meLength, char* pShe, ushort sheLength, bool ignoreCase)
		{
			ushort num = 0;
			bool flag = true;
			while (num < meLength)
			{
				if (num >= sheLength)
				{
					break;
				}
				char c = pMe[num];
				char c2 = pShe[num];
				if (c == '?' || c == '#')
				{
					return true;
				}
				if (c == '/')
				{
					if (c2 != '/')
					{
						return false;
					}
					if (!flag)
					{
						return false;
					}
					flag = true;
				}
				else
				{
					if (c2 == '?' || c2 == '#')
					{
						break;
					}
					if (!ignoreCase)
					{
						if (c != c2)
						{
							flag = false;
						}
					}
					else if (char.ToLower(c, CultureInfo.InvariantCulture) != char.ToLower(c2, CultureInfo.InvariantCulture))
					{
						flag = false;
					}
				}
				num += 1;
			}
			while (num < meLength)
			{
				char c;
				if ((c = pMe[num]) == '?' || c == '#')
				{
					return true;
				}
				if (c == '/')
				{
					return false;
				}
				num += 1;
			}
			return true;
		}

		internal unsafe static char[] EscapeString(string input, int start, int end, char[] dest, ref int destPos, bool isUriString, char force1, char force2, char rsvd)
		{
			if (end - start >= 65520)
			{
				throw new UriFormatException(global::SR.GetString("Invalid URI: The Uri string is too long."));
			}
			int i = start;
			int num = start;
			byte* ptr = stackalloc byte[(UIntPtr)160];
			fixed (string text = input)
			{
				char* ptr2 = text;
				if (ptr2 != null)
				{
					ptr2 += RuntimeHelpers.OffsetToStringData / 2;
				}
				while (i < end)
				{
					char c = ptr2[i];
					if (c > '\u007f')
					{
						short num2 = (short)Math.Min(end - i, 39);
						short num3 = 1;
						while (num3 < num2 && ptr2[i + (int)num3] > '\u007f')
						{
							num3 += 1;
						}
						if (ptr2[i + (int)num3 - 1] >= '\ud800' && ptr2[i + (int)num3 - 1] <= '\udbff')
						{
							if (num3 == 1 || (int)num3 == end - i)
							{
								throw new UriFormatException(global::SR.GetString("Invalid URI: There is an invalid sequence in the string."));
							}
							num3 += 1;
						}
						dest = UriHelper.EnsureDestinationSize(ptr2, dest, i, num3 * 4 * 3, 480, ref destPos, num);
						short num4 = (short)Encoding.UTF8.GetBytes(ptr2 + i, (int)num3, ptr, 160);
						if (num4 == 0)
						{
							throw new UriFormatException(global::SR.GetString("Invalid URI: There is an invalid sequence in the string."));
						}
						i += (int)(num3 - 1);
						for (num3 = 0; num3 < num4; num3 += 1)
						{
							UriHelper.EscapeAsciiChar((char)ptr[num3], dest, ref destPos);
						}
						num = i + 1;
					}
					else if (c == '%' && rsvd == '%')
					{
						dest = UriHelper.EnsureDestinationSize(ptr2, dest, i, 3, 120, ref destPos, num);
						if (i + 2 < end && UriHelper.EscapedAscii(ptr2[i + 1], ptr2[i + 2]) != '\uffff')
						{
							char[] array = dest;
							int num5 = destPos;
							destPos = num5 + 1;
							array[num5] = 37;
							char[] array2 = dest;
							num5 = destPos;
							destPos = num5 + 1;
							array2[num5] = ptr2[i + 1];
							char[] array3 = dest;
							num5 = destPos;
							destPos = num5 + 1;
							array3[num5] = ptr2[i + 2];
							i += 2;
						}
						else
						{
							UriHelper.EscapeAsciiChar('%', dest, ref destPos);
						}
						num = i + 1;
					}
					else if (c == force1 || c == force2)
					{
						dest = UriHelper.EnsureDestinationSize(ptr2, dest, i, 3, 120, ref destPos, num);
						UriHelper.EscapeAsciiChar(c, dest, ref destPos);
						num = i + 1;
					}
					else if (c != rsvd && (isUriString ? (!UriHelper.IsReservedUnreservedOrHash(c)) : (!UriHelper.IsUnreserved(c))))
					{
						dest = UriHelper.EnsureDestinationSize(ptr2, dest, i, 3, 120, ref destPos, num);
						UriHelper.EscapeAsciiChar(c, dest, ref destPos);
						num = i + 1;
					}
					i++;
				}
				if (num != i && (num != start || dest != null))
				{
					dest = UriHelper.EnsureDestinationSize(ptr2, dest, i, 0, 0, ref destPos, num);
				}
			}
			return dest;
		}

		private unsafe static char[] EnsureDestinationSize(char* pStr, char[] dest, int currentInputPos, short charsToAdd, short minReallocateChars, ref int destPos, int prevInputPos)
		{
			if (dest == null || dest.Length < destPos + (currentInputPos - prevInputPos) + (int)charsToAdd)
			{
				char[] array = new char[destPos + (currentInputPos - prevInputPos) + (int)minReallocateChars];
				if (dest != null && destPos != 0)
				{
					Buffer.BlockCopy(dest, 0, array, 0, destPos << 1);
				}
				dest = array;
			}
			while (prevInputPos != currentInputPos)
			{
				char[] array2 = dest;
				int num = destPos;
				destPos = num + 1;
				array2[num] = pStr[prevInputPos++];
			}
			return dest;
		}

		internal unsafe static char[] UnescapeString(string input, int start, int end, char[] dest, ref int destPosition, char rsvd1, char rsvd2, char rsvd3, UnescapeMode unescapeMode, UriParser syntax, bool isQuery)
		{
			char* ptr = input;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return UriHelper.UnescapeString(ptr, start, end, dest, ref destPosition, rsvd1, rsvd2, rsvd3, unescapeMode, syntax, isQuery);
		}

		internal unsafe static char[] UnescapeString(char* pStr, int start, int end, char[] dest, ref int destPosition, char rsvd1, char rsvd2, char rsvd3, UnescapeMode unescapeMode, UriParser syntax, bool isQuery)
		{
			byte[] array = null;
			byte b = 0;
			bool flag = false;
			int i = start;
			bool flag2 = Uri.IriParsingStatic(syntax) && (unescapeMode & UnescapeMode.EscapeUnescape) == UnescapeMode.EscapeUnescape;
			for (;;)
			{
				try
				{
					char[] array2;
					char* ptr;
					if ((array2 = dest) == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					if ((unescapeMode & UnescapeMode.EscapeUnescape) == UnescapeMode.CopyOnly)
					{
						while (start < end)
						{
							ref short ptr2 = ref *(short*)ptr;
							int num = destPosition;
							destPosition = num + 1;
							*((ref ptr2) + (IntPtr)num * 2) = (short)pStr[start++];
						}
						return dest;
					}
					for (;;)
					{
						IL_0070:
						char c = '\0';
						while (i < end)
						{
							if ((c = pStr[i]) == '%')
							{
								if ((unescapeMode & UnescapeMode.Unescape) == UnescapeMode.CopyOnly)
								{
									flag = true;
								}
								else if (i + 2 < end)
								{
									c = UriHelper.EscapedAscii(pStr[i + 1], pStr[i + 2]);
									if (unescapeMode >= UnescapeMode.UnescapeAll)
									{
										if (c == '\uffff')
										{
											if (unescapeMode >= UnescapeMode.UnescapeAllOrThrow)
											{
												goto Block_12;
											}
											goto IL_01D9;
										}
									}
									else if (c == '\uffff')
									{
										if ((unescapeMode & UnescapeMode.Escape) == UnescapeMode.CopyOnly)
										{
											goto IL_01D9;
										}
										flag = true;
									}
									else
									{
										if (c == '%')
										{
											i += 2;
											goto IL_01D9;
										}
										if (c == rsvd1 || c == rsvd2 || c == rsvd3)
										{
											i += 2;
											goto IL_01D9;
										}
										if ((unescapeMode & UnescapeMode.V1ToStringFlag) == UnescapeMode.CopyOnly && UriHelper.IsNotSafeForUnescape(c))
										{
											i += 2;
											goto IL_01D9;
										}
										if (flag2 && ((c <= '\u009f' && UriHelper.IsNotSafeForUnescape(c)) || (c > '\u009f' && !IriHelper.CheckIriUnicodeRange(c, isQuery))))
										{
											i += 2;
											goto IL_01D9;
										}
									}
								}
								else if (unescapeMode >= UnescapeMode.UnescapeAll)
								{
									if (unescapeMode >= UnescapeMode.UnescapeAllOrThrow)
									{
										goto Block_24;
									}
									goto IL_01D9;
								}
								else
								{
									flag = true;
								}
							}
							else
							{
								if ((unescapeMode & (UnescapeMode.Unescape | UnescapeMode.UnescapeAll)) == (UnescapeMode.Unescape | UnescapeMode.UnescapeAll) || (unescapeMode & UnescapeMode.Escape) == UnescapeMode.CopyOnly)
								{
									goto IL_01D9;
								}
								if (c == rsvd1 || c == rsvd2 || c == rsvd3)
								{
									flag = true;
								}
								else
								{
									if ((unescapeMode & UnescapeMode.V1ToStringFlag) != UnescapeMode.CopyOnly || (c > '\u001f' && (c < '\u007f' || c > '\u009f')))
									{
										goto IL_01D9;
									}
									flag = true;
								}
							}
							IL_0207:
							while (start < i)
							{
								ref short ptr3 = ref *(short*)ptr;
								int num = destPosition;
								destPosition = num + 1;
								*((ref ptr3) + (IntPtr)num * 2) = (short)pStr[start++];
							}
							if (i != end)
							{
								if (flag)
								{
									if (b == 0)
									{
										goto Block_36;
									}
									b -= 1;
									UriHelper.EscapeAsciiChar(pStr[i], dest, ref destPosition);
									flag = false;
									i = (start = i + 1);
									goto IL_0070;
								}
								else
								{
									if (c <= '\u007f')
									{
										char[] array3 = dest;
										int num = destPosition;
										destPosition = num + 1;
										array3[num] = c;
										i += 3;
										start = i;
										goto IL_0070;
									}
									int num2 = 1;
									if (array == null)
									{
										array = new byte[end - i];
									}
									array[0] = (byte)c;
									i += 3;
									while (i < end && pStr[i] == '%' && i + 2 < end)
									{
										c = UriHelper.EscapedAscii(pStr[i + 1], pStr[i + 2]);
										if (c == '\uffff' || c < '\u0080')
										{
											break;
										}
										array[num2++] = (byte)c;
										i += 3;
									}
									Encoding encoding = (Encoding)Encoding.UTF8.Clone();
									encoding.EncoderFallback = new EncoderReplacementFallback("");
									encoding.DecoderFallback = new DecoderReplacementFallback("");
									char[] array4 = new char[array.Length];
									int chars = encoding.GetChars(array, 0, num2, array4, 0);
									start = i;
									UriHelper.MatchUTF8Sequence(ptr, dest, ref destPosition, array4, chars, array, num2, isQuery, flag2);
								}
							}
							if (i == end)
							{
								goto Block_45;
							}
							goto IL_0070;
							IL_01D9:
							i++;
						}
						goto IL_0207;
					}
					Block_12:
					throw new UriFormatException(global::SR.GetString("Invalid URI: There is an invalid sequence in the string."));
					Block_24:
					throw new UriFormatException(global::SR.GetString("Invalid URI: There is an invalid sequence in the string."));
					Block_36:
					b = 30;
					char[] array5 = new char[dest.Length + (int)(b * 3)];
					char[] array6;
					char* ptr4;
					if ((array6 = array5) == null || array6.Length == 0)
					{
						ptr4 = null;
					}
					else
					{
						ptr4 = &array6[0];
					}
					for (int j = 0; j < destPosition; j++)
					{
						ptr4[j] = ptr[j];
					}
					array6 = null;
					dest = array5;
					continue;
					Block_45:;
				}
				finally
				{
					char[] array2 = null;
				}
				break;
			}
			return dest;
		}

		internal unsafe static void MatchUTF8Sequence(char* pDest, char[] dest, ref int destOffset, char[] unescapedChars, int charCount, byte[] bytes, int byteCount, bool isQuery, bool iriParsing)
		{
			int i = 0;
			fixed (char[] array = unescapedChars)
			{
				char* ptr;
				if (unescapedChars == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				for (int j = 0; j < charCount; j++)
				{
					bool flag = char.IsHighSurrogate(ptr[j]);
					byte[] bytes2 = Encoding.UTF8.GetBytes(unescapedChars, j, flag ? 2 : 1);
					int num = bytes2.Length;
					bool flag2 = false;
					if (iriParsing)
					{
						if (!flag)
						{
							flag2 = IriHelper.CheckIriUnicodeRange(unescapedChars[j], isQuery);
						}
						else
						{
							bool flag3 = false;
							flag2 = IriHelper.CheckIriUnicodeRange(unescapedChars[j], unescapedChars[j + 1], ref flag3, isQuery);
						}
					}
					for (;;)
					{
						if (bytes[i] == bytes2[0])
						{
							bool flag4 = true;
							int k;
							for (k = 0; k < num; k++)
							{
								if (bytes[i + k] != bytes2[k])
								{
									flag4 = false;
									break;
								}
							}
							if (flag4)
							{
								break;
							}
							for (int l = 0; l < k; l++)
							{
								UriHelper.EscapeAsciiChar((char)bytes[i++], dest, ref destOffset);
							}
						}
						else
						{
							UriHelper.EscapeAsciiChar((char)bytes[i++], dest, ref destOffset);
						}
					}
					i += num;
					if (iriParsing)
					{
						if (!flag2)
						{
							for (int m = 0; m < bytes2.Length; m++)
							{
								UriHelper.EscapeAsciiChar((char)bytes2[m], dest, ref destOffset);
							}
						}
						else if (!Uri.IsBidiControlCharacter(ptr[j]))
						{
							int num2 = destOffset;
							destOffset = num2 + 1;
							pDest[num2] = ptr[j];
							if (flag)
							{
								num2 = destOffset;
								destOffset = num2 + 1;
								pDest[num2] = ptr[j + 1];
							}
						}
					}
					else
					{
						int num2 = destOffset;
						destOffset = num2 + 1;
						pDest[num2] = ptr[j];
						if (flag)
						{
							num2 = destOffset;
							destOffset = num2 + 1;
							pDest[num2] = ptr[j + 1];
						}
					}
					if (flag)
					{
						j++;
					}
				}
			}
			while (i < byteCount)
			{
				UriHelper.EscapeAsciiChar((char)bytes[i++], dest, ref destOffset);
			}
		}

		internal static void EscapeAsciiChar(char ch, char[] to, ref int pos)
		{
			int num = pos;
			pos = num + 1;
			to[num] = '%';
			num = pos;
			pos = num + 1;
			to[num] = UriHelper.HexUpperChars[(int)((ch & 'ð') >> 4)];
			num = pos;
			pos = num + 1;
			to[num] = UriHelper.HexUpperChars[(int)(ch & '\u000f')];
		}

		internal static char EscapedAscii(char digit, char next)
		{
			if ((digit < '0' || digit > '9') && (digit < 'A' || digit > 'F') && (digit < 'a' || digit > 'f'))
			{
				return char.MaxValue;
			}
			int num = (int)((digit <= '9') ? (digit - '0') : (((digit <= 'F') ? (digit - 'A') : (digit - 'a')) + '\n'));
			if ((next < '0' || next > '9') && (next < 'A' || next > 'F') && (next < 'a' || next > 'f'))
			{
				return char.MaxValue;
			}
			return (char)((num << 4) + (int)((next <= '9') ? (next - '0') : (((next <= 'F') ? (next - 'A') : (next - 'a')) + '\n')));
		}

		internal static bool IsNotSafeForUnescape(char ch)
		{
			return ch <= '\u001f' || (ch >= '\u007f' && ch <= '\u009f') || ((ch >= ';' && ch <= '@' && (ch | '\u0002') != '>') || (ch >= '#' && ch <= '&') || ch == '+' || ch == ',' || ch == '/' || ch == '\\');
		}

		private static bool IsReservedUnreservedOrHash(char c)
		{
			if (UriHelper.IsUnreserved(c))
			{
				return true;
			}
			if (UriParser.ShouldUseLegacyV2Quirks)
			{
				return ";/?:@&=+$,".IndexOf(c) >= 0 || c == '#';
			}
			return ":/?#[]@!$&'()*+,;=".IndexOf(c) >= 0;
		}

		internal static bool IsUnreserved(char c)
		{
			if (Uri.IsAsciiLetterOrDigit(c))
			{
				return true;
			}
			if (UriParser.ShouldUseLegacyV2Quirks)
			{
				return "-_.!~*'()".IndexOf(c) >= 0;
			}
			return "-._~".IndexOf(c) >= 0;
		}

		internal static bool Is3986Unreserved(char c)
		{
			return Uri.IsAsciiLetterOrDigit(c) || "-._~".IndexOf(c) >= 0;
		}

		private static readonly char[] HexUpperChars = new char[]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F'
		};

		private const short c_MaxAsciiCharsReallocate = 40;

		private const short c_MaxUnicodeCharsReallocate = 40;

		private const short c_MaxUTF_8BytesPerUnicodeChar = 4;

		private const short c_EncodedCharsPerByte = 3;

		private const string RFC2396ReservedMarks = ";/?:@&=+$,";

		private const string RFC3986ReservedMarks = ":/?#[]@!$&'()*+,;=";

		private const string RFC2396UnreservedMarks = "-_.!~*'()";

		private const string RFC3986UnreservedMarks = "-._~";
	}
}
