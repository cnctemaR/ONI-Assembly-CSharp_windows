using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class XPathScanner
	{
		public XPathScanner(string xpathExpr)
		{
			if (xpathExpr == null)
			{
				throw XPathException.Create("'{0}' is an invalid expression.", string.Empty);
			}
			this._xpathExpr = xpathExpr;
			this.NextChar();
			this.NextLex();
		}

		public string SourceText
		{
			get
			{
				return this._xpathExpr;
			}
		}

		private char CurrentChar
		{
			get
			{
				return this._currentChar;
			}
		}

		private bool NextChar()
		{
			if (this._xpathExprIndex < this._xpathExpr.Length)
			{
				string xpathExpr = this._xpathExpr;
				int xpathExprIndex = this._xpathExprIndex;
				this._xpathExprIndex = xpathExprIndex + 1;
				this._currentChar = xpathExpr[xpathExprIndex];
				return true;
			}
			this._currentChar = '\0';
			return false;
		}

		public XPathScanner.LexKind Kind
		{
			get
			{
				return this._kind;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public string Prefix
		{
			get
			{
				return this._prefix;
			}
		}

		public string StringValue
		{
			get
			{
				return this._stringValue;
			}
		}

		public double NumberValue
		{
			get
			{
				return this._numberValue;
			}
		}

		public bool CanBeFunction
		{
			get
			{
				return this._canBeFunction;
			}
		}

		private void SkipSpace()
		{
			while (this._xmlCharType.IsWhiteSpace(this.CurrentChar) && this.NextChar())
			{
			}
		}

		public bool NextLex()
		{
			this.SkipSpace();
			char currentChar = this.CurrentChar;
			if (currentChar <= '@')
			{
				if (currentChar == '\0')
				{
					this._kind = XPathScanner.LexKind.Eof;
					return false;
				}
				switch (currentChar)
				{
				case '!':
					this._kind = XPathScanner.LexKind.Bang;
					this.NextChar();
					if (this.CurrentChar == '=')
					{
						this._kind = XPathScanner.LexKind.Ne;
						this.NextChar();
						return true;
					}
					return true;
				case '"':
				case '\'':
					this._kind = XPathScanner.LexKind.String;
					this._stringValue = this.ScanString();
					return true;
				case '#':
				case '$':
				case '(':
				case ')':
				case '*':
				case '+':
				case ',':
				case '-':
				case '=':
				case '@':
					break;
				case '%':
				case '&':
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
				case ':':
				case ';':
				case '?':
					goto IL_021D;
				case '.':
					this._kind = XPathScanner.LexKind.Dot;
					this.NextChar();
					if (this.CurrentChar == '.')
					{
						this._kind = XPathScanner.LexKind.DotDot;
						this.NextChar();
						return true;
					}
					if (XmlCharType.IsDigit(this.CurrentChar))
					{
						this._kind = XPathScanner.LexKind.Number;
						this._numberValue = this.ScanFraction();
						return true;
					}
					return true;
				case '/':
					this._kind = XPathScanner.LexKind.Slash;
					this.NextChar();
					if (this.CurrentChar == '/')
					{
						this._kind = XPathScanner.LexKind.SlashSlash;
						this.NextChar();
						return true;
					}
					return true;
				case '<':
					this._kind = XPathScanner.LexKind.Lt;
					this.NextChar();
					if (this.CurrentChar == '=')
					{
						this._kind = XPathScanner.LexKind.Le;
						this.NextChar();
						return true;
					}
					return true;
				case '>':
					this._kind = XPathScanner.LexKind.Gt;
					this.NextChar();
					if (this.CurrentChar == '=')
					{
						this._kind = XPathScanner.LexKind.Ge;
						this.NextChar();
						return true;
					}
					return true;
				default:
					goto IL_021D;
				}
			}
			else if (currentChar != '[' && currentChar != ']' && currentChar != '|')
			{
				goto IL_021D;
			}
			this._kind = (XPathScanner.LexKind)Convert.ToInt32(this.CurrentChar, CultureInfo.InvariantCulture);
			this.NextChar();
			return true;
			IL_021D:
			if (XmlCharType.IsDigit(this.CurrentChar))
			{
				this._kind = XPathScanner.LexKind.Number;
				this._numberValue = this.ScanNumber();
			}
			else
			{
				if (!this._xmlCharType.IsStartNCNameSingleChar(this.CurrentChar))
				{
					throw XPathException.Create("'{0}' has an invalid token.", this.SourceText);
				}
				this._kind = XPathScanner.LexKind.Name;
				this._name = this.ScanName();
				this._prefix = string.Empty;
				if (this.CurrentChar == ':')
				{
					this.NextChar();
					if (this.CurrentChar == ':')
					{
						this.NextChar();
						this._kind = XPathScanner.LexKind.Axe;
					}
					else
					{
						this._prefix = this._name;
						if (this.CurrentChar == '*')
						{
							this.NextChar();
							this._name = "*";
						}
						else
						{
							if (!this._xmlCharType.IsStartNCNameSingleChar(this.CurrentChar))
							{
								throw XPathException.Create("'{0}' has an invalid qualified name.", this.SourceText);
							}
							this._name = this.ScanName();
						}
					}
				}
				else
				{
					this.SkipSpace();
					if (this.CurrentChar == ':')
					{
						this.NextChar();
						if (this.CurrentChar != ':')
						{
							throw XPathException.Create("'{0}' has an invalid qualified name.", this.SourceText);
						}
						this.NextChar();
						this._kind = XPathScanner.LexKind.Axe;
					}
				}
				this.SkipSpace();
				this._canBeFunction = this.CurrentChar == '(';
			}
			return true;
		}

		private double ScanNumber()
		{
			int num = this._xpathExprIndex - 1;
			int num2 = 0;
			while (XmlCharType.IsDigit(this.CurrentChar))
			{
				this.NextChar();
				num2++;
			}
			if (this.CurrentChar == '.')
			{
				this.NextChar();
				num2++;
				while (XmlCharType.IsDigit(this.CurrentChar))
				{
					this.NextChar();
					num2++;
				}
			}
			return XmlConvert.ToXPathDouble(this._xpathExpr.Substring(num, num2));
		}

		private double ScanFraction()
		{
			int num = this._xpathExprIndex - 2;
			int num2 = 1;
			while (XmlCharType.IsDigit(this.CurrentChar))
			{
				this.NextChar();
				num2++;
			}
			return XmlConvert.ToXPathDouble(this._xpathExpr.Substring(num, num2));
		}

		private string ScanString()
		{
			char currentChar = this.CurrentChar;
			this.NextChar();
			int num = this._xpathExprIndex - 1;
			int num2 = 0;
			while (this.CurrentChar != currentChar)
			{
				if (!this.NextChar())
				{
					throw XPathException.Create("This is an unclosed string.");
				}
				num2++;
			}
			this.NextChar();
			return this._xpathExpr.Substring(num, num2);
		}

		private string ScanName()
		{
			int num = this._xpathExprIndex - 1;
			int num2 = 0;
			while (this._xmlCharType.IsNCNameSingleChar(this.CurrentChar))
			{
				this.NextChar();
				num2++;
			}
			return this._xpathExpr.Substring(num, num2);
		}

		private string _xpathExpr;

		private int _xpathExprIndex;

		private XPathScanner.LexKind _kind;

		private char _currentChar;

		private string _name;

		private string _prefix;

		private string _stringValue;

		private double _numberValue = double.NaN;

		private bool _canBeFunction;

		private XmlCharType _xmlCharType = XmlCharType.Instance;

		public enum LexKind
		{
			Comma = 44,
			Slash = 47,
			At = 64,
			Dot = 46,
			LParens = 40,
			RParens,
			LBracket = 91,
			RBracket = 93,
			Star = 42,
			Plus,
			Minus = 45,
			Eq = 61,
			Lt = 60,
			Gt = 62,
			Bang = 33,
			Dollar = 36,
			Apos = 39,
			Quote = 34,
			Union = 124,
			Ne = 78,
			Le = 76,
			Ge = 71,
			And = 65,
			Or = 79,
			DotDot = 68,
			SlashSlash = 83,
			Name = 110,
			String = 115,
			Number = 100,
			Axe = 97,
			Eof = 69
		}
	}
}
