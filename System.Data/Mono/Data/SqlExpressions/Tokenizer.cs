using System;
using System.Collections;
using System.Data;
using System.Text;
using Mono.Data.SqlExpressions.yyParser;

namespace Mono.Data.SqlExpressions
{
	internal class Tokenizer : yyInput
	{
		public Tokenizer(string strInput)
		{
			this.input = strInput.ToCharArray();
			this.pos = 0;
		}

		static Tokenizer()
		{
			for (int i = 0; i < Tokenizer.tokens.Length; i += 2)
			{
				Tokenizer.tokenMap.Add(Tokenizer.tokens[i + 1], Tokenizer.tokens[i]);
			}
		}

		private char Current()
		{
			return this.input[this.pos];
		}

		private char Next()
		{
			if (this.pos + 1 >= this.input.Length)
			{
				return '\0';
			}
			return this.input[this.pos + 1];
		}

		private bool MoveNext()
		{
			this.pos++;
			return this.pos < this.input.Length;
		}

		private bool SkipWhiteSpace()
		{
			if (this.pos >= this.input.Length)
			{
				return false;
			}
			while (char.IsWhiteSpace(this.Current()))
			{
				if (!this.MoveNext())
				{
					return false;
				}
			}
			return true;
		}

		private object ReadNumber()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Current());
			char c;
			while (char.IsDigit(c = this.Next()) || c == '.')
			{
				stringBuilder.Append(c);
				if (!this.MoveNext())
				{
					break;
				}
			}
			string text = stringBuilder.ToString();
			if (text.IndexOf(".") == -1)
			{
				return long.Parse(text);
			}
			return double.Parse(text);
		}

		private char ProcessEscapes(char c)
		{
			if (c == '\\')
			{
				if (this.MoveNext())
				{
					c = this.Current();
				}
				else
				{
					c = '\0';
				}
				char c2 = c;
				switch (c2)
				{
				case 'r':
					c = '\r';
					break;
				default:
					if (c2 != '\\')
					{
						if (c2 != 'n')
						{
							throw new SyntaxErrorException(string.Format("Invalid escape sequence: '\\{0}'.", c));
						}
						c = '\n';
					}
					else
					{
						c = '\\';
					}
					break;
				case 't':
					c = '\t';
					break;
				}
			}
			return c;
		}

		private string ReadString(char terminator)
		{
			return this.ReadString(terminator, false);
		}

		private string ReadString(char terminator, bool canEscape)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			while (this.MoveNext())
			{
				if (this.Current() == terminator)
				{
					if (this.Next() != terminator)
					{
						flag = true;
						break;
					}
					stringBuilder.Append(this.ProcessEscapes(this.Current()));
					this.MoveNext();
				}
				else
				{
					stringBuilder.Append(this.ProcessEscapes(this.Current()));
				}
			}
			if (!flag)
			{
				throw new SyntaxErrorException(string.Format("invalid string at {0}{1}<--", terminator, stringBuilder.ToString()));
			}
			return stringBuilder.ToString();
		}

		private string ReadIdentifier()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Current());
			char c;
			while ((c = this.Next()) == '_' || char.IsLetterOrDigit(c) || c == '\\')
			{
				stringBuilder.Append(this.ProcessEscapes(c));
				if (!this.MoveNext())
				{
					break;
				}
			}
			string text = stringBuilder.ToString();
			if (string.Compare(text, "not", StringComparison.OrdinalIgnoreCase) == 0)
			{
				int num = this.pos;
				while (char.IsWhiteSpace(this.Next()))
				{
					if (!this.MoveNext())
					{
						this.pos = num;
						return text;
					}
				}
				this.MoveNext();
				char c2 = this.Current();
				switch (c2)
				{
				case 'I':
					break;
				default:
					switch (c2)
					{
					case 'i':
						goto IL_00EA;
					case 'l':
						goto IL_00F6;
					}
					this.pos = num;
					return text;
				case 'L':
					goto IL_00F6;
				}
				IL_00EA:
				string text2 = "in";
				goto IL_010B;
				IL_00F6:
				text2 = "like";
				IL_010B:
				int length = text2.Length;
				int num2 = 1;
				while (length-- > 0 && char.IsLetter(c = this.Next()))
				{
					char c3 = char.ToLowerInvariant(c);
					if (text2[num2++] != c3)
					{
						this.pos = num;
						return text;
					}
					this.MoveNext();
				}
				stringBuilder.Append(' ');
				stringBuilder.Append(text2);
				text = stringBuilder.ToString();
			}
			return text;
		}

		private int ParseIdentifier()
		{
			string text = this.ReadIdentifier();
			object obj = Tokenizer.tokenMap[text.ToLower()];
			if (obj != null)
			{
				return (int)obj;
			}
			this.val = text;
			return 298;
		}

		private int ParseToken()
		{
			char c2;
			char c = (c2 = this.Current());
			switch (c2)
			{
			case '"':
			case '\'':
				this.val = this.ReadString(c, true);
				return 295;
			case '#':
			{
				string text = this.ReadString('#');
				this.val = DateTime.Parse(text);
				return 297;
			}
			default:
				if (c2 == '[')
				{
					this.val = this.ReadString(']');
					return 298;
				}
				if (char.IsDigit(c))
				{
					this.val = this.ReadNumber();
					return 296;
				}
				if (char.IsLetter(c) || c == '_')
				{
					return this.ParseIdentifier();
				}
				throw new SyntaxErrorException("invalid token: '" + c + "'");
			case '%':
				return 274;
			case '(':
				return 257;
			case ')':
				return 258;
			case '*':
				return 272;
			case '+':
				return 270;
			case ',':
				return 276;
			case '-':
				return 271;
			case '.':
				return 275;
			case '/':
				return 273;
			case '<':
				return 268;
			case '=':
				return 267;
			case '>':
				return 269;
			}
		}

		public bool advance()
		{
			if (!this.SkipWhiteSpace())
			{
				return false;
			}
			this.tok = this.ParseToken();
			this.MoveNext();
			return true;
		}

		public int token()
		{
			return this.tok;
		}

		public object value()
		{
			return this.val;
		}

		private static readonly IDictionary tokenMap = new Hashtable();

		private static readonly object[] tokens = new object[]
		{
			259, "and", 260, "or", 261, "not", 262, "true", 263, "false",
			264, "null", 265, "parent", 266, "child", 277, "is", 278, "in",
			279, "not in", 280, "like", 281, "not like", 282, "count", 283, "sum",
			284, "avg", 285, "max", 286, "min", 287, "stdev", 288, "var",
			289, "iif", 290, "substring", 291, "isnull", 292, "len", 293, "trim",
			294, "convert"
		};

		private char[] input;

		private int pos;

		private int tok;

		private object val;
	}
}
