using System;
using System.Globalization;

namespace System.Net.Http.Headers
{
	internal class Lexer
	{
		public Lexer(string stream)
		{
			this.s = stream;
		}

		public int Position
		{
			get
			{
				return this.pos;
			}
			set
			{
				this.pos = value;
			}
		}

		public string GetStringValue(Token token)
		{
			return this.s.Substring(token.StartPosition, token.EndPosition - token.StartPosition);
		}

		public string GetStringValue(Token start, Token end)
		{
			return this.s.Substring(start.StartPosition, end.EndPosition - start.StartPosition);
		}

		public string GetQuotedStringValue(Token start)
		{
			return this.s.Substring(start.StartPosition + 1, start.EndPosition - start.StartPosition - 2);
		}

		public string GetRemainingStringValue(int position)
		{
			if (position <= this.s.Length)
			{
				return this.s.Substring(position);
			}
			return null;
		}

		public bool IsStarStringValue(Token token)
		{
			return token.EndPosition - token.StartPosition == 1 && this.s[token.StartPosition] == '*';
		}

		public bool TryGetNumericValue(Token token, out int value)
		{
			return int.TryParse(this.GetStringValue(token), NumberStyles.None, CultureInfo.InvariantCulture, out value);
		}

		public bool TryGetNumericValue(Token token, out long value)
		{
			return long.TryParse(this.GetStringValue(token), NumberStyles.None, CultureInfo.InvariantCulture, out value);
		}

		public TimeSpan? TryGetTimeSpanValue(Token token)
		{
			int num;
			if (this.TryGetNumericValue(token, out num))
			{
				return new TimeSpan?(TimeSpan.FromSeconds((double)num));
			}
			return null;
		}

		public bool TryGetDateValue(Token token, out DateTimeOffset value)
		{
			return Lexer.TryGetDateValue((token == Token.Type.QuotedString) ? this.s.Substring(token.StartPosition + 1, token.EndPosition - token.StartPosition - 2) : this.GetStringValue(token), out value);
		}

		public static bool TryGetDateValue(string text, out DateTimeOffset value)
		{
			return DateTimeOffset.TryParseExact(text, Lexer.dt_formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.AssumeUniversal, out value);
		}

		public bool TryGetDoubleValue(Token token, out double value)
		{
			return double.TryParse(this.GetStringValue(token), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value);
		}

		public static bool IsValidToken(string input)
		{
			int i;
			for (i = 0; i < input.Length; i++)
			{
				if (!Lexer.IsValidCharacter(input[i]))
				{
					return false;
				}
			}
			return i > 0;
		}

		public static bool IsValidCharacter(char input)
		{
			return (int)input < Lexer.last_token_char && Lexer.token_chars[(int)input];
		}

		public void EatChar()
		{
			this.pos++;
		}

		public int PeekChar()
		{
			if (this.pos >= this.s.Length)
			{
				return -1;
			}
			return (int)this.s[this.pos];
		}

		public bool ScanCommentOptional(out string value)
		{
			Token token;
			return this.ScanCommentOptional(out value, out token) || token == Token.Type.End;
		}

		public bool ScanCommentOptional(out string value, out Token readToken)
		{
			readToken = this.Scan(false);
			if (readToken != Token.Type.OpenParens)
			{
				value = null;
				return false;
			}
			int num = 1;
			while (this.pos < this.s.Length)
			{
				char c = this.s[this.pos];
				if (c == '(')
				{
					num++;
					this.pos++;
				}
				else if (c == ')')
				{
					this.pos++;
					if (--num <= 0)
					{
						int startPosition = readToken.StartPosition;
						value = this.s.Substring(startPosition, this.pos - startPosition);
						return true;
					}
				}
				else
				{
					if (c < ' ' || c > '~')
					{
						break;
					}
					this.pos++;
				}
			}
			value = null;
			return false;
		}

		public Token Scan(bool recognizeDash = false)
		{
			int num = this.pos;
			if (this.s == null)
			{
				return new Token(Token.Type.Error, 0, 0);
			}
			Token.Type type;
			if (this.pos >= this.s.Length)
			{
				type = Token.Type.End;
			}
			else
			{
				type = Token.Type.Error;
				char c;
				for (;;)
				{
					string text = this.s;
					int num2 = this.pos;
					this.pos = num2 + 1;
					c = text[num2];
					if (c > '"')
					{
						goto IL_006D;
					}
					if (c != '\t' && c != ' ')
					{
						break;
					}
					if (this.pos == this.s.Length)
					{
						goto Block_12;
					}
				}
				if (c != '"')
				{
					goto IL_0171;
				}
				num = this.pos - 1;
				while (this.pos < this.s.Length)
				{
					string text2 = this.s;
					int num2 = this.pos;
					this.pos = num2 + 1;
					c = text2[num2];
					if (c == '\\')
					{
						if (this.pos + 1 >= this.s.Length)
						{
							break;
						}
						this.pos++;
					}
					else if (c == '"')
					{
						type = Token.Type.QuotedString;
						break;
					}
				}
				goto IL_01D3;
				IL_006D:
				if (c <= '/')
				{
					if (c == '(')
					{
						num = this.pos - 1;
						type = Token.Type.OpenParens;
						goto IL_01D3;
					}
					switch (c)
					{
					case ',':
						type = Token.Type.SeparatorComma;
						goto IL_01D3;
					case '-':
						if (recognizeDash)
						{
							type = Token.Type.SeparatorDash;
							goto IL_01D3;
						}
						goto IL_0171;
					case '.':
						goto IL_0171;
					case '/':
						type = Token.Type.SeparatorSlash;
						goto IL_01D3;
					default:
						goto IL_0171;
					}
				}
				else
				{
					if (c == ';')
					{
						type = Token.Type.SeparatorSemicolon;
						goto IL_01D3;
					}
					if (c != '=')
					{
						goto IL_0171;
					}
					type = Token.Type.SeparatorEqual;
					goto IL_01D3;
				}
				Block_12:
				type = Token.Type.End;
				goto IL_01D3;
				IL_0171:
				if ((int)c < Lexer.last_token_char && Lexer.token_chars[(int)c])
				{
					num = this.pos - 1;
					type = Token.Type.Token;
					while (this.pos < this.s.Length)
					{
						c = this.s[this.pos];
						if ((int)c >= Lexer.last_token_char || !Lexer.token_chars[(int)c])
						{
							break;
						}
						this.pos++;
					}
				}
			}
			IL_01D3:
			return new Token(type, num, this.pos);
		}

		private static readonly bool[] token_chars = new bool[]
		{
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, true, false, true, true, true, true, true,
			false, false, true, true, false, true, true, false, true, true,
			true, true, true, true, true, true, true, true, false, false,
			false, false, false, false, false, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, false, false, false, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, false, true, false, true
		};

		private static readonly int last_token_char = Lexer.token_chars.Length;

		private static readonly string[] dt_formats = new string[] { "r", "dddd, dd'-'MMM'-'yy HH:mm:ss 'GMT'", "ddd MMM d HH:mm:ss yyyy", "d MMM yy H:m:s", "ddd, d MMM yyyy H:m:s zzz" };

		private readonly string s;

		private int pos;
	}
}
