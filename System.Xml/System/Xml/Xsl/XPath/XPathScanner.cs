using System;

namespace System.Xml.Xsl.XPath
{
	internal sealed class XPathScanner
	{
		public XPathScanner(string xpathExpr)
			: this(xpathExpr, 0)
		{
		}

		public XPathScanner(string xpathExpr, int startFrom)
		{
			this.xpathExpr = xpathExpr;
			this.kind = LexKind.Unknown;
			this.SetSourceIndex(startFrom);
			this.NextLex();
		}

		public string Source
		{
			get
			{
				return this.xpathExpr;
			}
		}

		public LexKind Kind
		{
			get
			{
				return this.kind;
			}
		}

		public int LexStart
		{
			get
			{
				return this.lexStart;
			}
		}

		public int LexSize
		{
			get
			{
				return this.curIndex - this.lexStart;
			}
		}

		public int PrevLexEnd
		{
			get
			{
				return this.prevLexEnd;
			}
		}

		private void SetSourceIndex(int index)
		{
			this.curIndex = index - 1;
			this.NextChar();
		}

		private void NextChar()
		{
			this.curIndex++;
			if (this.curIndex < this.xpathExpr.Length)
			{
				this.curChar = this.xpathExpr[this.curIndex];
				return;
			}
			this.curChar = '\0';
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Prefix
		{
			get
			{
				return this.prefix;
			}
		}

		public string RawValue
		{
			get
			{
				if (this.kind == LexKind.Eof)
				{
					return this.LexKindToString(this.kind);
				}
				return this.xpathExpr.Substring(this.lexStart, this.curIndex - this.lexStart);
			}
		}

		public string StringValue
		{
			get
			{
				return this.stringValue;
			}
		}

		public bool CanBeFunction
		{
			get
			{
				return this.canBeFunction;
			}
		}

		public XPathAxis Axis
		{
			get
			{
				return this.axis;
			}
		}

		private void SkipSpace()
		{
			while (this.xmlCharType.IsWhiteSpace(this.curChar))
			{
				this.NextChar();
			}
		}

		private static bool IsAsciiDigit(char ch)
		{
			return ch - '0' <= '\t';
		}

		public void NextLex()
		{
			this.prevLexEnd = this.curIndex;
			this.prevKind = this.kind;
			this.SkipSpace();
			this.lexStart = this.curIndex;
			char c = this.curChar;
			if (c <= '[')
			{
				if (c != '\0')
				{
					switch (c)
					{
					case '!':
						this.NextChar();
						if (this.curChar == '=')
						{
							this.kind = LexKind.Ne;
							this.NextChar();
							return;
						}
						this.kind = LexKind.Unknown;
						return;
					case '"':
					case '\'':
						this.kind = LexKind.String;
						this.ScanString();
						return;
					case '#':
					case '%':
					case '&':
					case ';':
					case '?':
						goto IL_027C;
					case '$':
					case '(':
					case ')':
					case ',':
					case '@':
						goto IL_00F2;
					case '*':
						this.kind = LexKind.Star;
						this.NextChar();
						this.CheckOperator(true);
						return;
					case '+':
						this.kind = LexKind.Plus;
						this.NextChar();
						return;
					case '-':
						this.kind = LexKind.Minus;
						this.NextChar();
						return;
					case '.':
						this.NextChar();
						if (this.curChar == '.')
						{
							this.kind = LexKind.DotDot;
							this.NextChar();
							return;
						}
						if (!XPathScanner.IsAsciiDigit(this.curChar))
						{
							this.kind = LexKind.Dot;
							return;
						}
						this.SetSourceIndex(this.lexStart);
						break;
					case '/':
						this.NextChar();
						if (this.curChar == '/')
						{
							this.kind = LexKind.SlashSlash;
							this.NextChar();
							return;
						}
						this.kind = LexKind.Slash;
						return;
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
						break;
					case ':':
						this.NextChar();
						if (this.curChar == ':')
						{
							this.kind = LexKind.ColonColon;
							this.NextChar();
							return;
						}
						this.kind = LexKind.Unknown;
						return;
					case '<':
						this.NextChar();
						if (this.curChar == '=')
						{
							this.kind = LexKind.Le;
							this.NextChar();
							return;
						}
						this.kind = LexKind.Lt;
						return;
					case '=':
						this.kind = LexKind.Eq;
						this.NextChar();
						return;
					case '>':
						this.NextChar();
						if (this.curChar == '=')
						{
							this.kind = LexKind.Ge;
							this.NextChar();
							return;
						}
						this.kind = LexKind.Gt;
						return;
					default:
						if (c != '[')
						{
							goto IL_027C;
						}
						goto IL_00F2;
					}
					this.kind = LexKind.Number;
					this.ScanNumber();
					return;
				}
				this.kind = LexKind.Eof;
				return;
			}
			else if (c != ']')
			{
				if (c == '|')
				{
					this.kind = LexKind.Union;
					this.NextChar();
					return;
				}
				if (c != '}')
				{
					goto IL_027C;
				}
			}
			IL_00F2:
			this.kind = (LexKind)this.curChar;
			this.NextChar();
			return;
			IL_027C:
			if (this.xmlCharType.IsStartNCNameSingleChar(this.curChar))
			{
				this.kind = LexKind.Name;
				this.name = this.ScanNCName();
				this.prefix = string.Empty;
				this.canBeFunction = false;
				this.axis = XPathAxis.Unknown;
				bool flag = false;
				int num = this.curIndex;
				if (this.curChar == ':')
				{
					this.NextChar();
					if (this.curChar == ':')
					{
						this.NextChar();
						flag = true;
						this.SetSourceIndex(num);
					}
					else if (this.curChar == '*')
					{
						this.NextChar();
						this.prefix = this.name;
						this.name = "*";
					}
					else if (this.xmlCharType.IsStartNCNameSingleChar(this.curChar))
					{
						this.prefix = this.name;
						this.name = this.ScanNCName();
						num = this.curIndex;
						this.SkipSpace();
						this.canBeFunction = this.curChar == '(';
						this.SetSourceIndex(num);
					}
					else
					{
						this.SetSourceIndex(num);
					}
				}
				else
				{
					this.SkipSpace();
					if (this.curChar == ':')
					{
						this.NextChar();
						if (this.curChar == ':')
						{
							this.NextChar();
							flag = true;
						}
						this.SetSourceIndex(num);
					}
					else
					{
						this.canBeFunction = this.curChar == '(';
					}
				}
				if (!this.CheckOperator(false) && flag)
				{
					this.axis = this.CheckAxis();
					return;
				}
			}
			else
			{
				this.kind = LexKind.Unknown;
				this.NextChar();
			}
		}

		private bool CheckOperator(bool star)
		{
			LexKind lexKind;
			if (star)
			{
				lexKind = LexKind.Multiply;
			}
			else
			{
				if (this.prefix.Length != 0 || this.name.Length > 3)
				{
					return false;
				}
				string text = this.name;
				if (!(text == "or"))
				{
					if (!(text == "and"))
					{
						if (!(text == "div"))
						{
							if (!(text == "mod"))
							{
								return false;
							}
							lexKind = LexKind.Modulo;
						}
						else
						{
							lexKind = LexKind.Divide;
						}
					}
					else
					{
						lexKind = LexKind.And;
					}
				}
				else
				{
					lexKind = LexKind.Or;
				}
			}
			if (this.prevKind <= LexKind.Union)
			{
				return false;
			}
			LexKind lexKind2 = this.prevKind;
			if (lexKind2 <= LexKind.LParens)
			{
				if (lexKind2 - LexKind.ColonColon > 1 && lexKind2 != LexKind.Dollar && lexKind2 != LexKind.LParens)
				{
					goto IL_00BE;
				}
			}
			else if (lexKind2 <= LexKind.Slash)
			{
				if (lexKind2 != LexKind.Comma && lexKind2 != LexKind.Slash)
				{
					goto IL_00BE;
				}
			}
			else if (lexKind2 != LexKind.At && lexKind2 != LexKind.LBracket)
			{
				goto IL_00BE;
			}
			return false;
			IL_00BE:
			this.kind = lexKind;
			return true;
		}

		private XPathAxis CheckAxis()
		{
			this.kind = LexKind.Axis;
			string text = this.name;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2535512472U)
			{
				if (num <= 1047347951U)
				{
					if (num != 21436113U)
					{
						if (num != 510973315U)
						{
							if (num == 1047347951U)
							{
								if (text == "attribute")
								{
									return XPathAxis.Attribute;
								}
							}
						}
						else if (text == "ancestor-or-self")
						{
							return XPathAxis.AncestorOrSelf;
						}
					}
					else if (text == "preceding-sibling")
					{
						return XPathAxis.PrecedingSibling;
					}
				}
				else if (num != 1683726967U)
				{
					if (num != 2452897184U)
					{
						if (num == 2535512472U)
						{
							if (text == "following")
							{
								return XPathAxis.Following;
							}
						}
					}
					else if (text == "ancestor")
					{
						return XPathAxis.Ancestor;
					}
				}
				else if (text == "self")
				{
					return XPathAxis.Self;
				}
			}
			else if (num <= 3726896370U)
			{
				if (num != 2944295921U)
				{
					if (num != 3402529440U)
					{
						if (num == 3726896370U)
						{
							if (text == "preceding")
							{
								return XPathAxis.Preceding;
							}
						}
					}
					else if (text == "namespace")
					{
						return XPathAxis.Namespace;
					}
				}
				else if (text == "descendant-or-self")
				{
					return XPathAxis.DescendantOrSelf;
				}
			}
			else if (num <= 3939368189U)
			{
				if (num != 3852476509U)
				{
					if (num == 3939368189U)
					{
						if (text == "parent")
						{
							return XPathAxis.Parent;
						}
					}
				}
				else if (text == "child")
				{
					return XPathAxis.Child;
				}
			}
			else if (num != 3998959382U)
			{
				if (num == 4042989175U)
				{
					if (text == "following-sibling")
					{
						return XPathAxis.FollowingSibling;
					}
				}
			}
			else if (text == "descendant")
			{
				return XPathAxis.Descendant;
			}
			this.kind = LexKind.Name;
			return XPathAxis.Unknown;
		}

		private void ScanNumber()
		{
			while (XPathScanner.IsAsciiDigit(this.curChar))
			{
				this.NextChar();
			}
			if (this.curChar == '.')
			{
				this.NextChar();
				while (XPathScanner.IsAsciiDigit(this.curChar))
				{
					this.NextChar();
				}
			}
			if (((int)this.curChar & -33) == 69)
			{
				this.NextChar();
				if (this.curChar == '+' || this.curChar == '-')
				{
					this.NextChar();
				}
				while (XPathScanner.IsAsciiDigit(this.curChar))
				{
					this.NextChar();
				}
				throw this.CreateException("Scientific notation is not allowed.", Array.Empty<string>());
			}
		}

		private void ScanString()
		{
			int num = this.curIndex + 1;
			int num2 = this.xpathExpr.IndexOf(this.curChar, num);
			if (num2 < 0)
			{
				this.SetSourceIndex(this.xpathExpr.Length);
				throw this.CreateException("String literal was not closed.", Array.Empty<string>());
			}
			this.stringValue = this.xpathExpr.Substring(num, num2 - num);
			this.SetSourceIndex(num2 + 1);
		}

		private string ScanNCName()
		{
			int num = this.curIndex;
			while (this.xmlCharType.IsNCNameSingleChar(this.curChar))
			{
				this.NextChar();
			}
			return this.xpathExpr.Substring(num, this.curIndex - num);
		}

		public void PassToken(LexKind t)
		{
			this.CheckToken(t);
			this.NextLex();
		}

		public void CheckToken(LexKind t)
		{
			if (this.kind == t)
			{
				return;
			}
			if (t == LexKind.Eof)
			{
				throw this.CreateException("Expected end of the expression, found '{0}'.", new string[] { this.RawValue });
			}
			throw this.CreateException("Expected token '{0}', found '{1}'.", new string[]
			{
				this.LexKindToString(t),
				this.RawValue
			});
		}

		private string LexKindToString(LexKind t)
		{
			if (LexKind.Eof < t)
			{
				return new string((char)t, 1);
			}
			switch (t)
			{
			case LexKind.Name:
				return "<name>";
			case LexKind.String:
				return "<string literal>";
			case LexKind.Eof:
				return "<eof>";
			default:
				return string.Empty;
			}
		}

		public XPathCompileException CreateException(string resId, params string[] args)
		{
			return new XPathCompileException(this.xpathExpr, this.lexStart, this.curIndex, resId, args);
		}

		private string xpathExpr;

		private int curIndex;

		private char curChar;

		private LexKind kind;

		private string name;

		private string prefix;

		private string stringValue;

		private bool canBeFunction;

		private int lexStart;

		private int prevLexEnd;

		private LexKind prevKind;

		private XPathAxis axis;

		private XmlCharType xmlCharType = XmlCharType.Instance;
	}
}
