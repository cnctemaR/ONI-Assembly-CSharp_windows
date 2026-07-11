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
			this.xpathExpr = xpathExpr;
			this.NextChar();
			this.NextLex();
		}

		public string SourceText
		{
			get
			{
				return this.xpathExpr;
			}
		}

		private char CurerntChar
		{
			get
			{
				return this.currentChar;
			}
		}

		private bool NextChar()
		{
			if (this.xpathExprIndex < this.xpathExpr.Length)
			{
				string text = this.xpathExpr;
				int num = this.xpathExprIndex;
				this.xpathExprIndex = num + 1;
				this.currentChar = text[num];
				return true;
			}
			this.currentChar = '\0';
			return false;
		}

		public XPathScanner.LexKind Kind
		{
			get
			{
				return this.kind;
			}
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

		public string StringValue
		{
			get
			{
				return this.stringValue;
			}
		}

		public double NumberValue
		{
			get
			{
				return this.numberValue;
			}
		}

		public bool CanBeFunction
		{
			get
			{
				return this.canBeFunction;
			}
		}

		private void SkipSpace()
		{
			while (this.xmlCharType.IsWhiteSpace(this.CurerntChar) && this.NextChar())
			{
			}
		}

		public bool NextLex()
		{
			this.SkipSpace();
			char curerntChar = this.CurerntChar;
			if (curerntChar <= '@')
			{
				if (curerntChar == '\0')
				{
					this.kind = XPathScanner.LexKind.Eof;
					return false;
				}
				switch (curerntChar)
				{
				case '!':
					this.kind = XPathScanner.LexKind.Bang;
					this.NextChar();
					if (this.CurerntChar == '=')
					{
						this.kind = XPathScanner.LexKind.Ne;
						this.NextChar();
						return true;
					}
					return true;
				case '"':
				case '\'':
					this.kind = XPathScanner.LexKind.String;
					this.stringValue = this.ScanString();
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
					this.kind = XPathScanner.LexKind.Dot;
					this.NextChar();
					if (this.CurerntChar == '.')
					{
						this.kind = XPathScanner.LexKind.DotDot;
						this.NextChar();
						return true;
					}
					if (XmlCharType.IsDigit(this.CurerntChar))
					{
						this.kind = XPathScanner.LexKind.Number;
						this.numberValue = this.ScanFraction();
						return true;
					}
					return true;
				case '/':
					this.kind = XPathScanner.LexKind.Slash;
					this.NextChar();
					if (this.CurerntChar == '/')
					{
						this.kind = XPathScanner.LexKind.SlashSlash;
						this.NextChar();
						return true;
					}
					return true;
				case '<':
					this.kind = XPathScanner.LexKind.Lt;
					this.NextChar();
					if (this.CurerntChar == '=')
					{
						this.kind = XPathScanner.LexKind.Le;
						this.NextChar();
						return true;
					}
					return true;
				case '>':
					this.kind = XPathScanner.LexKind.Gt;
					this.NextChar();
					if (this.CurerntChar == '=')
					{
						this.kind = XPathScanner.LexKind.Ge;
						this.NextChar();
						return true;
					}
					return true;
				default:
					goto IL_021D;
				}
			}
			else if (curerntChar != '[' && curerntChar != ']' && curerntChar != '|')
			{
				goto IL_021D;
			}
			this.kind = (XPathScanner.LexKind)Convert.ToInt32(this.CurerntChar, CultureInfo.InvariantCulture);
			this.NextChar();
			return true;
			IL_021D:
			if (XmlCharType.IsDigit(this.CurerntChar))
			{
				this.kind = XPathScanner.LexKind.Number;
				this.numberValue = this.ScanNumber();
			}
			else
			{
				if (!this.xmlCharType.IsStartNCNameSingleChar(this.CurerntChar))
				{
					throw XPathException.Create("'{0}' has an invalid token.", this.SourceText);
				}
				this.kind = XPathScanner.LexKind.Name;
				this.name = this.ScanName();
				this.prefix = string.Empty;
				if (this.CurerntChar == ':')
				{
					this.NextChar();
					if (this.CurerntChar == ':')
					{
						this.NextChar();
						this.kind = XPathScanner.LexKind.Axe;
					}
					else
					{
						this.prefix = this.name;
						if (this.CurerntChar == '*')
						{
							this.NextChar();
							this.name = "*";
						}
						else
						{
							if (!this.xmlCharType.IsStartNCNameSingleChar(this.CurerntChar))
							{
								throw XPathException.Create("'{0}' has an invalid qualified name.", this.SourceText);
							}
							this.name = this.ScanName();
						}
					}
				}
				else
				{
					this.SkipSpace();
					if (this.CurerntChar == ':')
					{
						this.NextChar();
						if (this.CurerntChar != ':')
						{
							throw XPathException.Create("'{0}' has an invalid qualified name.", this.SourceText);
						}
						this.NextChar();
						this.kind = XPathScanner.LexKind.Axe;
					}
				}
				this.SkipSpace();
				this.canBeFunction = this.CurerntChar == '(';
			}
			return true;
		}

		private double ScanNumber()
		{
			int num = this.xpathExprIndex - 1;
			int num2 = 0;
			while (XmlCharType.IsDigit(this.CurerntChar))
			{
				this.NextChar();
				num2++;
			}
			if (this.CurerntChar == '.')
			{
				this.NextChar();
				num2++;
				while (XmlCharType.IsDigit(this.CurerntChar))
				{
					this.NextChar();
					num2++;
				}
			}
			return XmlConvert.ToXPathDouble(this.xpathExpr.Substring(num, num2));
		}

		private double ScanFraction()
		{
			int num = this.xpathExprIndex - 2;
			int num2 = 1;
			while (XmlCharType.IsDigit(this.CurerntChar))
			{
				this.NextChar();
				num2++;
			}
			return XmlConvert.ToXPathDouble(this.xpathExpr.Substring(num, num2));
		}

		private string ScanString()
		{
			char curerntChar = this.CurerntChar;
			this.NextChar();
			int num = this.xpathExprIndex - 1;
			int num2 = 0;
			while (this.CurerntChar != curerntChar)
			{
				if (!this.NextChar())
				{
					throw XPathException.Create("This is an unclosed string.");
				}
				num2++;
			}
			this.NextChar();
			return this.xpathExpr.Substring(num, num2);
		}

		private string ScanName()
		{
			int num = this.xpathExprIndex - 1;
			int num2 = 0;
			while (this.xmlCharType.IsNCNameSingleChar(this.CurerntChar))
			{
				this.NextChar();
				num2++;
			}
			return this.xpathExpr.Substring(num, num2);
		}

		private string xpathExpr;

		private int xpathExprIndex;

		private XPathScanner.LexKind kind;

		private char currentChar;

		private string name;

		private string prefix;

		private string stringValue;

		private double numberValue = double.NaN;

		private bool canBeFunction;

		private XmlCharType xmlCharType = XmlCharType.Instance;

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
