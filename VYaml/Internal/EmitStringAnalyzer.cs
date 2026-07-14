using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace VYaml.Internal
{
	internal static class EmitStringAnalyzer
	{
		public unsafe static EmitStringInfo Analyze(ReadOnlySpan<char> value)
		{
			if (value.Length <= 0)
			{
				return new EmitStringInfo(0, true, false);
			}
			bool flag = EmitStringAnalyzer.IsReservedWord(value);
			char c = (char)(*value[0]);
			char c2 = (char)(*value[value.Length - 1]);
			bool flag2 = flag || c == ' ' || c2 == ' ';
			if (!flag2)
			{
				if (c <= '&')
				{
					if (c != '!' && c != '%' && c != '&')
					{
						goto IL_00A5;
					}
				}
				else
				{
					switch (c)
					{
					case '*':
					case '-':
					case '.':
						break;
					case '+':
					case ',':
						goto IL_00A5;
					default:
						switch (c)
						{
						case '<':
						case '=':
						case '>':
						case '?':
						case '@':
							break;
						default:
							if (c != '|')
							{
								goto IL_00A5;
							}
							break;
						}
						break;
					}
				}
				bool flag3 = true;
				goto IL_00A8;
				IL_00A5:
				flag3 = false;
				IL_00A8:
				flag2 = flag3;
			}
			bool flag4 = flag2;
			int num = 0;
			int num2 = 1;
			ReadOnlySpan<char> readOnlySpan = value;
			int i = 0;
			while (i < readOnlySpan.Length)
			{
				char c3 = (char)(*readOnlySpan[i]);
				if (c3 <= '[')
				{
					if (c3 != '\n')
					{
						switch (c3)
						{
						case '"':
						case '#':
						case '\'':
						case ',':
						case ':':
							goto IL_016D;
						case '$':
						case '%':
						case '&':
						case '(':
						case ')':
						case '*':
						case '+':
						case '-':
						case '.':
						case '/':
							break;
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
							num++;
							break;
						default:
							if (c3 == '[')
							{
								goto IL_016D;
							}
							break;
						}
					}
					else
					{
						num2++;
					}
				}
				else if (c3 == ']' || c3 == '`' || c3 == '{')
				{
					goto IL_016D;
				}
				IL_0177:
				i++;
				continue;
				IL_016D:
				flag4 = true;
				goto IL_0177;
			}
			if (c2 == '\n')
			{
				num2--;
			}
			return new EmitStringInfo(num2, flag4 || num == value.Length, flag);
		}

		[return: Nullable(1)]
		internal unsafe static StringBuilder BuildLiteralScalar(ReadOnlySpan<char> originalValue, int indentCharCount)
		{
			char c = '\0';
			if (originalValue.Length > 0 && *originalValue[originalValue.Length - 1] == 10)
			{
				if (*originalValue[originalValue.Length - 2] == 10 || (*originalValue[originalValue.Length - 2] == 13 && *originalValue[originalValue.Length - 3] == 10))
				{
					c = '+';
				}
			}
			else
			{
				c = '-';
			}
			StringBuilder stringBuilder;
			if ((stringBuilder = EmitStringAnalyzer.stringBuilderThreadStatic) == null)
			{
				stringBuilder = (EmitStringAnalyzer.stringBuilderThreadStatic = new StringBuilder(1024));
			}
			StringBuilder stringBuilder2 = stringBuilder.Clear();
			stringBuilder2.Append('|');
			if (c > '\0')
			{
				stringBuilder2.Append(c);
			}
			stringBuilder2.Append('\n');
			EmitStringAnalyzer.AppendWhiteSpace(stringBuilder2, indentCharCount);
			for (int i = 0; i < originalValue.Length; i++)
			{
				char c2 = (char)(*originalValue[i]);
				stringBuilder2.Append(c2);
				if (c2 == '\n' && i < originalValue.Length - 1)
				{
					EmitStringAnalyzer.AppendWhiteSpace(stringBuilder2, indentCharCount);
				}
			}
			if (c == '-')
			{
				stringBuilder2.Append('\n');
			}
			return stringBuilder2;
		}

		[return: Nullable(1)]
		internal unsafe static StringBuilder BuildQuotedScalar(ReadOnlySpan<char> originalValue, bool doubleQuote = true)
		{
			StringBuilder stringBuilder = EmitStringAnalyzer.GetStringBuilder();
			char c = (doubleQuote ? '"' : '\'');
			stringBuilder.Append(c);
			ReadOnlySpan<char> readOnlySpan = originalValue;
			int i = 0;
			while (i < readOnlySpan.Length)
			{
				char c2 = (char)(*readOnlySpan[i]);
				char c3 = c2;
				if (c3 <= '\u007f')
				{
					switch (c3)
					{
					case '\0':
						stringBuilder.Append("\\0");
						break;
					case '\u0001':
						stringBuilder.Append("\\1");
						break;
					case '\u0002':
						stringBuilder.Append("\\2");
						break;
					case '\u0003':
						stringBuilder.Append("\\3");
						break;
					case '\u0004':
						stringBuilder.Append("\\4");
						break;
					case '\u0005':
						stringBuilder.Append("\\5");
						break;
					case '\u0006':
						stringBuilder.Append("\\6");
						break;
					case '\a':
						stringBuilder.Append("\\a");
						break;
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					case '\v':
						stringBuilder.Append("\\v");
						break;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					case '\u000e':
						stringBuilder.Append("\\r");
						break;
					case '\u000f':
						stringBuilder.Append("\\u000f");
						break;
					case '\u0010':
						stringBuilder.Append("\\u0010");
						break;
					case '\u0011':
						stringBuilder.Append("\\u0011");
						break;
					case '\u0012':
						stringBuilder.Append("\\u0012");
						break;
					case '\u0013':
						stringBuilder.Append("\\u0013");
						break;
					case '\u0014':
						stringBuilder.Append("\\u0014");
						break;
					case '\u0015':
						stringBuilder.Append("\\u0015");
						break;
					case '\u0016':
						stringBuilder.Append("\\u0016");
						break;
					case '\u0017':
						stringBuilder.Append("\\u0017");
						break;
					case '\u0018':
						stringBuilder.Append("\\u0018");
						break;
					case '\u0019':
						stringBuilder.Append("\\u0019");
						break;
					case '\u001a':
						stringBuilder.Append("\\u001a");
						break;
					case '\u001b':
						stringBuilder.Append("\\u001b");
						break;
					case '\u001c':
						stringBuilder.Append("\\u001c");
						break;
					case '\u001d':
						stringBuilder.Append("\\u001d");
						break;
					case '\u001e':
						stringBuilder.Append("\\u001e");
						break;
					case '\u001f':
						stringBuilder.Append("\\u001f");
						break;
					case ' ':
					case '!':
					case '#':
					case '$':
					case '%':
					case '&':
						goto IL_03D3;
					case '"':
						if (!doubleQuote)
						{
							goto IL_03D3;
						}
						stringBuilder.Append("\\\"");
						break;
					case '\'':
						if (doubleQuote)
						{
							goto IL_03D3;
						}
						stringBuilder.Append("\\'");
						break;
					default:
						if (c3 != '\\')
						{
							if (c3 != '\u007f')
							{
								goto IL_03D3;
							}
							stringBuilder.Append("\\u007F");
						}
						else
						{
							stringBuilder.Append("\\\\");
						}
						break;
					}
				}
				else if (c3 <= '\u00a0')
				{
					if (c3 != '\u0085')
					{
						if (c3 != '\u00a0')
						{
							goto IL_03D3;
						}
						stringBuilder.Append("\\_");
					}
					else
					{
						stringBuilder.Append("\\N");
					}
				}
				else if (c3 != '\u2028')
				{
					if (c3 != '\u2029')
					{
						goto IL_03D3;
					}
					stringBuilder.Append("\\P");
				}
				else
				{
					stringBuilder.Append("\\L");
				}
				IL_03DC:
				i++;
				continue;
				IL_03D3:
				stringBuilder.Append(c2);
				goto IL_03DC;
			}
			stringBuilder.Append(c);
			return stringBuilder;
		}

		private unsafe static bool IsReservedWord(ReadOnlySpan<char> value)
		{
			new StringBuilder().Append('\n');
			switch (value.Length)
			{
			case 1:
				if (value.Length == 1 && *value[0] == 126)
				{
					return true;
				}
				break;
			case 4:
				if (value.SequenceEqual<char>("null") || value.SequenceEqual<char>("null") || value.SequenceEqual<char>("Null") || value.SequenceEqual<char>("NULL") || value.SequenceEqual<char>("true") || value.SequenceEqual<char>("True") || value.SequenceEqual<char>("TRUE"))
				{
					return true;
				}
				break;
			case 5:
				if (value.SequenceEqual<char>("false") || value.SequenceEqual<char>("False") || value.SequenceEqual<char>("FALSE"))
				{
					return true;
				}
				break;
			}
			return false;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static StringBuilder GetStringBuilder()
		{
			StringBuilder stringBuilder;
			if ((stringBuilder = EmitStringAnalyzer.stringBuilderThreadStatic) == null)
			{
				stringBuilder = (EmitStringAnalyzer.stringBuilderThreadStatic = new StringBuilder(1024));
			}
			return stringBuilder.Clear();
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AppendWhiteSpace(StringBuilder stringBuilder, int length)
		{
			if (length > EmitStringAnalyzer.whiteSpaces.Length)
			{
				EmitStringAnalyzer.whiteSpaces = Enumerable.Repeat<char>(' ', length * 2).ToArray<char>();
			}
			stringBuilder.Append(EmitStringAnalyzer.whiteSpaces.AsSpan<char>(0, length));
		}

		[Nullable(2)]
		[ThreadStatic]
		private static StringBuilder stringBuilderThreadStatic;

		[Nullable(1)]
		private static char[] whiteSpaces = new char[]
		{
			' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
			' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
			' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
			' ', ' '
		};
	}
}
