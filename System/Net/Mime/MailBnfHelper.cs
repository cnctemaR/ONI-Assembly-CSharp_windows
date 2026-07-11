using System;
using System.Collections.Generic;
using System.Text;

namespace System.Net.Mime
{
	internal static class MailBnfHelper
	{
		static MailBnfHelper()
		{
			MailBnfHelper.Whitespace.Add(MailBnfHelper.Tab);
			MailBnfHelper.Whitespace.Add(MailBnfHelper.Space);
			MailBnfHelper.Whitespace.Add(MailBnfHelper.CR);
			MailBnfHelper.Whitespace.Add(MailBnfHelper.LF);
			for (int i = 48; i <= 57; i++)
			{
				MailBnfHelper.Atext[i] = true;
			}
			for (int j = 65; j <= 90; j++)
			{
				MailBnfHelper.Atext[j] = true;
			}
			for (int k = 97; k <= 122; k++)
			{
				MailBnfHelper.Atext[k] = true;
			}
			MailBnfHelper.Atext[33] = true;
			MailBnfHelper.Atext[35] = true;
			MailBnfHelper.Atext[36] = true;
			MailBnfHelper.Atext[37] = true;
			MailBnfHelper.Atext[38] = true;
			MailBnfHelper.Atext[39] = true;
			MailBnfHelper.Atext[42] = true;
			MailBnfHelper.Atext[43] = true;
			MailBnfHelper.Atext[45] = true;
			MailBnfHelper.Atext[47] = true;
			MailBnfHelper.Atext[61] = true;
			MailBnfHelper.Atext[63] = true;
			MailBnfHelper.Atext[94] = true;
			MailBnfHelper.Atext[95] = true;
			MailBnfHelper.Atext[96] = true;
			MailBnfHelper.Atext[123] = true;
			MailBnfHelper.Atext[124] = true;
			MailBnfHelper.Atext[125] = true;
			MailBnfHelper.Atext[126] = true;
			for (int l = 1; l <= 9; l++)
			{
				MailBnfHelper.Qtext[l] = true;
			}
			MailBnfHelper.Qtext[11] = true;
			MailBnfHelper.Qtext[12] = true;
			for (int m = 14; m <= 33; m++)
			{
				MailBnfHelper.Qtext[m] = true;
			}
			for (int n = 35; n <= 91; n++)
			{
				MailBnfHelper.Qtext[n] = true;
			}
			for (int num = 93; num <= 127; num++)
			{
				MailBnfHelper.Qtext[num] = true;
			}
			for (int num2 = 1; num2 <= 8; num2++)
			{
				MailBnfHelper.Dtext[num2] = true;
			}
			MailBnfHelper.Dtext[11] = true;
			MailBnfHelper.Dtext[12] = true;
			for (int num3 = 14; num3 <= 31; num3++)
			{
				MailBnfHelper.Dtext[num3] = true;
			}
			for (int num4 = 33; num4 <= 90; num4++)
			{
				MailBnfHelper.Dtext[num4] = true;
			}
			for (int num5 = 94; num5 <= 127; num5++)
			{
				MailBnfHelper.Dtext[num5] = true;
			}
			for (int num6 = 33; num6 <= 57; num6++)
			{
				MailBnfHelper.Ftext[num6] = true;
			}
			for (int num7 = 59; num7 <= 126; num7++)
			{
				MailBnfHelper.Ftext[num7] = true;
			}
			for (int num8 = 33; num8 <= 126; num8++)
			{
				MailBnfHelper.Ttext[num8] = true;
			}
			MailBnfHelper.Ttext[40] = false;
			MailBnfHelper.Ttext[41] = false;
			MailBnfHelper.Ttext[60] = false;
			MailBnfHelper.Ttext[62] = false;
			MailBnfHelper.Ttext[64] = false;
			MailBnfHelper.Ttext[44] = false;
			MailBnfHelper.Ttext[59] = false;
			MailBnfHelper.Ttext[58] = false;
			MailBnfHelper.Ttext[92] = false;
			MailBnfHelper.Ttext[34] = false;
			MailBnfHelper.Ttext[47] = false;
			MailBnfHelper.Ttext[91] = false;
			MailBnfHelper.Ttext[93] = false;
			MailBnfHelper.Ttext[63] = false;
			MailBnfHelper.Ttext[61] = false;
			for (int num9 = 1; num9 <= 8; num9++)
			{
				MailBnfHelper.Ctext[num9] = true;
			}
			MailBnfHelper.Ctext[11] = true;
			MailBnfHelper.Ctext[12] = true;
			for (int num10 = 14; num10 <= 31; num10++)
			{
				MailBnfHelper.Ctext[num10] = true;
			}
			for (int num11 = 33; num11 <= 39; num11++)
			{
				MailBnfHelper.Ctext[num11] = true;
			}
			for (int num12 = 42; num12 <= 91; num12++)
			{
				MailBnfHelper.Ctext[num12] = true;
			}
			for (int num13 = 93; num13 <= 127; num13++)
			{
				MailBnfHelper.Ctext[num13] = true;
			}
		}

		internal static bool SkipCFWS(string data, ref int offset)
		{
			int num = 0;
			while (offset < data.Length)
			{
				if (data[offset] > '\u007f')
				{
					throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'.", new object[] { data[offset] }));
				}
				if (data[offset] == '\\' && num > 0)
				{
					offset += 2;
				}
				else if (data[offset] == '(')
				{
					num++;
				}
				else if (data[offset] == ')')
				{
					num--;
				}
				else if (data[offset] != ' ' && data[offset] != '\t' && num == 0)
				{
					return true;
				}
				if (num < 0)
				{
					throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'.", new object[] { data[offset] }));
				}
				offset++;
			}
			return false;
		}

		internal static void ValidateHeaderName(string data)
		{
			int i;
			for (i = 0; i < data.Length; i++)
			{
				if ((int)data[i] > MailBnfHelper.Ftext.Length || !MailBnfHelper.Ftext[(int)data[i]])
				{
					throw new FormatException(global::SR.GetString("An invalid character was found in header name."));
				}
			}
			if (i == 0)
			{
				throw new FormatException(global::SR.GetString("An invalid character was found in header name."));
			}
		}

		internal static string ReadQuotedString(string data, ref int offset, StringBuilder builder)
		{
			return MailBnfHelper.ReadQuotedString(data, ref offset, builder, false, false);
		}

		internal static string ReadQuotedString(string data, ref int offset, StringBuilder builder, bool doesntRequireQuotes, bool permitUnicodeInDisplayName)
		{
			if (!doesntRequireQuotes)
			{
				offset++;
			}
			int num = offset;
			StringBuilder stringBuilder = ((builder != null) ? builder : new StringBuilder());
			while (offset < data.Length)
			{
				if (data[offset] == '\\')
				{
					stringBuilder.Append(data, num, offset - num);
					int num2 = offset + 1;
					offset = num2;
					num = num2;
				}
				else if (data[offset] == '"')
				{
					stringBuilder.Append(data, num, offset - num);
					offset++;
					if (builder == null)
					{
						return stringBuilder.ToString();
					}
					return null;
				}
				else if (data[offset] == '=' && data.Length > offset + 3 && data[offset + 1] == '\r' && data[offset + 2] == '\n' && (data[offset + 3] == ' ' || data[offset + 3] == '\t'))
				{
					offset += 3;
				}
				else if (permitUnicodeInDisplayName)
				{
					if ((int)data[offset] <= MailBnfHelper.Ascii7bitMaxValue && !MailBnfHelper.Qtext[(int)data[offset]])
					{
						throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'.", new object[] { data[offset] }));
					}
				}
				else if ((int)data[offset] > MailBnfHelper.Ascii7bitMaxValue || !MailBnfHelper.Qtext[(int)data[offset]])
				{
					throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'.", new object[] { data[offset] }));
				}
				offset++;
			}
			if (!doesntRequireQuotes)
			{
				throw new FormatException(global::SR.GetString("The mail header is malformed."));
			}
			stringBuilder.Append(data, num, offset - num);
			if (builder == null)
			{
				return stringBuilder.ToString();
			}
			return null;
		}

		internal static string ReadParameterAttribute(string data, ref int offset, StringBuilder builder)
		{
			if (!MailBnfHelper.SkipCFWS(data, ref offset))
			{
				return null;
			}
			return MailBnfHelper.ReadToken(data, ref offset, null);
		}

		internal static string ReadToken(string data, ref int offset, StringBuilder builder)
		{
			int num = offset;
			while (offset < data.Length)
			{
				if ((int)data[offset] > MailBnfHelper.Ascii7bitMaxValue)
				{
					throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'.", new object[] { data[offset] }));
				}
				if (!MailBnfHelper.Ttext[(int)data[offset]])
				{
					break;
				}
				offset++;
			}
			if (num == offset)
			{
				throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'.", new object[] { data[offset] }));
			}
			return data.Substring(num, offset - num);
		}

		internal static string GetDateTimeString(DateTime value, StringBuilder builder)
		{
			StringBuilder stringBuilder = ((builder != null) ? builder : new StringBuilder());
			stringBuilder.Append(value.Day);
			stringBuilder.Append(' ');
			stringBuilder.Append(MailBnfHelper.s_months[value.Month]);
			stringBuilder.Append(' ');
			stringBuilder.Append(value.Year);
			stringBuilder.Append(' ');
			if (value.Hour <= 9)
			{
				stringBuilder.Append('0');
			}
			stringBuilder.Append(value.Hour);
			stringBuilder.Append(':');
			if (value.Minute <= 9)
			{
				stringBuilder.Append('0');
			}
			stringBuilder.Append(value.Minute);
			stringBuilder.Append(':');
			if (value.Second <= 9)
			{
				stringBuilder.Append('0');
			}
			stringBuilder.Append(value.Second);
			string text = TimeZone.CurrentTimeZone.GetUtcOffset(value).ToString();
			if (text[0] != '-')
			{
				stringBuilder.Append(" +");
			}
			else
			{
				stringBuilder.Append(" ");
			}
			string[] array = text.Split(new char[] { ':' });
			stringBuilder.Append(array[0]);
			stringBuilder.Append(array[1]);
			if (builder == null)
			{
				return stringBuilder.ToString();
			}
			return null;
		}

		internal static void GetTokenOrQuotedString(string data, StringBuilder builder, bool allowUnicode)
		{
			int i = 0;
			int num = 0;
			while (i < data.Length)
			{
				if (!MailBnfHelper.CheckForUnicode(data[i], allowUnicode) && (!MailBnfHelper.Ttext[(int)data[i]] || data[i] == ' '))
				{
					builder.Append('"');
					while (i < data.Length)
					{
						if (!MailBnfHelper.CheckForUnicode(data[i], allowUnicode))
						{
							if (MailBnfHelper.IsFWSAt(data, i))
							{
								i++;
								i++;
							}
							else if (!MailBnfHelper.Qtext[(int)data[i]])
							{
								builder.Append(data, num, i - num);
								builder.Append('\\');
								num = i;
							}
						}
						i++;
					}
					builder.Append(data, num, i - num);
					builder.Append('"');
					return;
				}
				i++;
			}
			if (data.Length == 0)
			{
				builder.Append("\"\"");
			}
			builder.Append(data);
		}

		private static bool CheckForUnicode(char ch, bool allowUnicode)
		{
			if ((int)ch < MailBnfHelper.Ascii7bitMaxValue)
			{
				return false;
			}
			if (!allowUnicode)
			{
				throw new FormatException(global::SR.GetString("An invalid character was found in the mail header: '{0}'.", new object[] { ch }));
			}
			return true;
		}

		internal static bool HasCROrLF(string data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] == '\r' || data[i] == '\n')
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsFWSAt(string data, int index)
		{
			return data[index] == MailBnfHelper.CR && index + 2 < data.Length && data[index + 1] == MailBnfHelper.LF && (data[index + 2] == MailBnfHelper.Space || data[index + 2] == MailBnfHelper.Tab);
		}

		internal static bool[] Atext = new bool[128];

		internal static bool[] Qtext = new bool[128];

		internal static bool[] Dtext = new bool[128];

		internal static bool[] Ftext = new bool[128];

		internal static bool[] Ttext = new bool[128];

		internal static bool[] Ctext = new bool[128];

		internal static readonly int Ascii7bitMaxValue = 127;

		internal static readonly char Quote = '"';

		internal static readonly char Space = ' ';

		internal static readonly char Tab = '\t';

		internal static readonly char CR = '\r';

		internal static readonly char LF = '\n';

		internal static readonly char StartComment = '(';

		internal static readonly char EndComment = ')';

		internal static readonly char Backslash = '\\';

		internal static readonly char At = '@';

		internal static readonly char EndAngleBracket = '>';

		internal static readonly char StartAngleBracket = '<';

		internal static readonly char StartSquareBracket = '[';

		internal static readonly char EndSquareBracket = ']';

		internal static readonly char Comma = ',';

		internal static readonly char Dot = '.';

		internal static readonly IList<char> Whitespace = new List<char>();

		private static string[] s_months = new string[]
		{
			null, "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep",
			"Oct", "Nov", "Dec"
		};
	}
}
