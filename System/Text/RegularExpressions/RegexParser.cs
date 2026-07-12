using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexParser
	{
		public static RegexTree Parse(string re, RegexOptions op)
		{
			RegexParser regexParser = new RegexParser(((op & RegexOptions.CultureInvariant) != RegexOptions.None) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture);
			regexParser._options = op;
			regexParser.SetPattern(re);
			regexParser.CountCaptures();
			regexParser.Reset(op);
			RegexNode regexNode = regexParser.ScanRegex();
			string[] array;
			if (regexParser._capnamelist == null)
			{
				array = null;
			}
			else
			{
				array = regexParser._capnamelist.ToArray();
			}
			return new RegexTree(regexNode, regexParser._caps, regexParser._capnumlist, regexParser._captop, regexParser._capnames, array, op);
		}

		public static RegexReplacement ParseReplacement(string rep, Hashtable caps, int capsize, Hashtable capnames, RegexOptions op)
		{
			RegexParser regexParser = new RegexParser(((op & RegexOptions.CultureInvariant) != RegexOptions.None) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture);
			regexParser._options = op;
			regexParser.NoteCaptures(caps, capsize, capnames);
			regexParser.SetPattern(rep);
			RegexNode regexNode = regexParser.ScanReplacement();
			return new RegexReplacement(rep, regexNode, caps);
		}

		public static string Escape(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (RegexParser.IsMetachar(input[i]))
				{
					StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
					char c = input[i];
					stringBuilder.Append(input, 0, i);
					do
					{
						stringBuilder.Append('\\');
						switch (c)
						{
						case '\t':
							c = 't';
							break;
						case '\n':
							c = 'n';
							break;
						case '\f':
							c = 'f';
							break;
						case '\r':
							c = 'r';
							break;
						}
						stringBuilder.Append(c);
						i++;
						int num = i;
						while (i < input.Length)
						{
							c = input[i];
							if (RegexParser.IsMetachar(c))
							{
								break;
							}
							i++;
						}
						stringBuilder.Append(input, num, i - num);
					}
					while (i < input.Length);
					return StringBuilderCache.GetStringAndRelease(stringBuilder);
				}
			}
			return input;
		}

		public static string Unescape(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == '\\')
				{
					StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
					RegexParser regexParser = new RegexParser(CultureInfo.InvariantCulture);
					regexParser.SetPattern(input);
					stringBuilder.Append(input, 0, i);
					do
					{
						i++;
						regexParser.Textto(i);
						if (i < input.Length)
						{
							stringBuilder.Append(regexParser.ScanCharEscape());
						}
						i = regexParser.Textpos();
						int num = i;
						while (i < input.Length && input[i] != '\\')
						{
							i++;
						}
						stringBuilder.Append(input, num, i - num);
					}
					while (i < input.Length);
					return StringBuilderCache.GetStringAndRelease(stringBuilder);
				}
			}
			return input;
		}

		private RegexParser(CultureInfo culture)
		{
			this._culture = culture;
			this._optionsStack = new List<RegexOptions>();
			this._caps = new Hashtable();
		}

		private void SetPattern(string Re)
		{
			if (Re == null)
			{
				Re = string.Empty;
			}
			this._pattern = Re;
			this._currentPos = 0;
		}

		private void Reset(RegexOptions topopts)
		{
			this._currentPos = 0;
			this._autocap = 1;
			this._ignoreNextParen = false;
			if (this._optionsStack.Count > 0)
			{
				this._optionsStack.RemoveRange(0, this._optionsStack.Count - 1);
			}
			this._options = topopts;
			this._stack = null;
		}

		private RegexNode ScanRegex()
		{
			bool flag = false;
			this.StartGroup(new RegexNode(28, this._options, 0, -1));
			while (this.CharsRight() > 0)
			{
				bool flag2 = flag;
				flag = false;
				this.ScanBlank();
				int num = this.Textpos();
				char c;
				if (this.UseOptionX())
				{
					while (this.CharsRight() > 0)
					{
						if (RegexParser.IsStopperX(c = this.RightChar()))
						{
							if (c != '{')
							{
								break;
							}
							if (this.IsTrueQuantifier())
							{
								break;
							}
						}
						this.MoveRight();
					}
				}
				else
				{
					while (this.CharsRight() > 0 && (!RegexParser.IsSpecial(c = this.RightChar()) || (c == '{' && !this.IsTrueQuantifier())))
					{
						this.MoveRight();
					}
				}
				int num2 = this.Textpos();
				this.ScanBlank();
				if (this.CharsRight() == 0)
				{
					c = '!';
				}
				else if (RegexParser.IsSpecial(c = this.RightChar()))
				{
					flag = RegexParser.IsQuantifier(c);
					this.MoveRight();
				}
				else
				{
					c = ' ';
				}
				if (num < num2)
				{
					int num3 = num2 - num - (flag ? 1 : 0);
					flag2 = false;
					if (num3 > 0)
					{
						this.AddConcatenate(num, num3, false);
					}
					if (flag)
					{
						this.AddUnitOne(this.CharAt(num2 - 1));
					}
				}
				if (c <= '?')
				{
					switch (c)
					{
					case ' ':
						continue;
					case '!':
						goto IL_0414;
					case '"':
					case '#':
					case '%':
					case '&':
					case '\'':
					case ',':
					case '-':
						goto IL_02A3;
					case '$':
						this.AddUnitType(this.UseOptionM() ? 15 : 20);
						break;
					case '(':
					{
						this.PushOptions();
						RegexNode regexNode;
						if ((regexNode = this.ScanGroupOpen()) == null)
						{
							this.PopKeepOptions();
							continue;
						}
						this.PushGroup();
						this.StartGroup(regexNode);
						continue;
					}
					case ')':
						if (this.EmptyStack())
						{
							throw this.MakeException("Too many )'s.");
						}
						this.AddGroup();
						this.PopGroup();
						this.PopOptions();
						if (this.Unit() == null)
						{
							continue;
						}
						break;
					case '*':
					case '+':
						goto IL_0271;
					case '.':
						if (this.UseOptionS())
						{
							this.AddUnitSet("\0\u0001\0\0");
						}
						else
						{
							this.AddUnitNotone('\n');
						}
						break;
					default:
						if (c != '?')
						{
							goto IL_02A3;
						}
						goto IL_0271;
					}
				}
				else
				{
					switch (c)
					{
					case '[':
						this.AddUnitSet(this.ScanCharClass(this.UseOptionI(), false).ToStringClass());
						break;
					case '\\':
						this.AddUnitNode(this.ScanBackslash(false));
						break;
					case ']':
						goto IL_02A3;
					case '^':
						this.AddUnitType(this.UseOptionM() ? 14 : 18);
						break;
					default:
						if (c == '{')
						{
							goto IL_0271;
						}
						if (c != '|')
						{
							goto IL_02A3;
						}
						this.AddAlternate();
						continue;
					}
				}
				IL_02AF:
				this.ScanBlank();
				if (this.CharsRight() == 0 || !(flag = this.IsTrueQuantifier()))
				{
					this.AddConcatenate();
					continue;
				}
				c = this.RightCharMoveRight();
				while (this.Unit() != null)
				{
					int num4;
					int num5;
					if (c <= '+')
					{
						if (c != '*')
						{
							if (c != '+')
							{
								goto IL_03AD;
							}
							num4 = 1;
							num5 = int.MaxValue;
						}
						else
						{
							num4 = 0;
							num5 = int.MaxValue;
						}
					}
					else if (c != '?')
					{
						if (c != '{')
						{
							goto IL_03AD;
						}
						num = this.Textpos();
						num4 = (num5 = this.ScanDecimal());
						if (num < this.Textpos() && this.CharsRight() > 0 && this.RightChar() == ',')
						{
							this.MoveRight();
							if (this.CharsRight() == 0 || this.RightChar() == '}')
							{
								num5 = int.MaxValue;
							}
							else
							{
								num5 = this.ScanDecimal();
							}
						}
						if (num == this.Textpos() || this.CharsRight() == 0 || this.RightCharMoveRight() != '}')
						{
							this.AddConcatenate();
							this.Textto(num - 1);
							break;
						}
					}
					else
					{
						num4 = 0;
						num5 = 1;
					}
					this.ScanBlank();
					bool flag3;
					if (this.CharsRight() == 0 || this.RightChar() != '?')
					{
						flag3 = false;
					}
					else
					{
						this.MoveRight();
						flag3 = true;
					}
					if (num4 > num5)
					{
						throw this.MakeException("Illegal {x,y} with x > y.");
					}
					this.AddConcatenate(flag3, num4, num5);
					continue;
					IL_03AD:
					throw this.MakeException("Internal error in ScanRegex.");
				}
				continue;
				IL_0271:
				if (this.Unit() == null)
				{
					throw this.MakeException(flag2 ? SR.Format("Nested quantifier {0}.", c.ToString()) : "Quantifier {x,y} following nothing.");
				}
				this.MoveLeft();
				goto IL_02AF;
				IL_02A3:
				throw this.MakeException("Internal error in ScanRegex.");
			}
			IL_0414:
			if (!this.EmptyStack())
			{
				throw this.MakeException("Not enough )'s.");
			}
			this.AddGroup();
			return this.Unit();
		}

		private RegexNode ScanReplacement()
		{
			this._concatenation = new RegexNode(25, this._options);
			for (;;)
			{
				int num = this.CharsRight();
				if (num == 0)
				{
					break;
				}
				int num2 = this.Textpos();
				while (num > 0 && this.RightChar() != '$')
				{
					this.MoveRight();
					num--;
				}
				this.AddConcatenate(num2, this.Textpos() - num2, true);
				if (num > 0)
				{
					if (this.RightCharMoveRight() == '$')
					{
						this.AddUnitNode(this.ScanDollar());
					}
					this.AddConcatenate();
				}
			}
			return this._concatenation;
		}

		private RegexCharClass ScanCharClass(bool caseInsensitive, bool scanOnly)
		{
			char c = '\0';
			bool flag = false;
			bool flag2 = true;
			bool flag3 = false;
			RegexCharClass regexCharClass = (scanOnly ? null : new RegexCharClass());
			if (this.CharsRight() > 0 && this.RightChar() == '^')
			{
				this.MoveRight();
				if (!scanOnly)
				{
					regexCharClass.Negate = true;
				}
			}
			while (this.CharsRight() > 0)
			{
				bool flag4 = false;
				char c2 = this.RightCharMoveRight();
				if (c2 == ']')
				{
					if (!flag2)
					{
						flag3 = true;
						break;
					}
					goto IL_0261;
				}
				else
				{
					if (c2 == '\\' && this.CharsRight() > 0)
					{
						char c3;
						c2 = (c3 = this.RightCharMoveRight());
						if (c3 <= 'S')
						{
							if (c3 <= 'D')
							{
								if (c3 != '-')
								{
									if (c3 != 'D')
									{
										goto IL_01FA;
									}
								}
								else
								{
									if (!scanOnly)
									{
										regexCharClass.AddRange(c2, c2);
										goto IL_0371;
									}
									goto IL_0371;
								}
							}
							else
							{
								if (c3 == 'P')
								{
									goto IL_019B;
								}
								if (c3 != 'S')
								{
									goto IL_01FA;
								}
								goto IL_012B;
							}
						}
						else
						{
							if (c3 <= 'd')
							{
								if (c3 != 'W')
								{
									if (c3 != 'd')
									{
										goto IL_01FA;
									}
									goto IL_00ED;
								}
							}
							else
							{
								if (c3 == 'p')
								{
									goto IL_019B;
								}
								if (c3 == 's')
								{
									goto IL_012B;
								}
								if (c3 != 'w')
								{
									goto IL_01FA;
								}
							}
							if (scanOnly)
							{
								goto IL_0371;
							}
							if (flag)
							{
								throw this.MakeException(SR.Format("Cannot include class \\{0} in character range.", c2.ToString()));
							}
							regexCharClass.AddWord(this.UseOptionE(), c2 == 'W');
							goto IL_0371;
						}
						IL_00ED:
						if (scanOnly)
						{
							goto IL_0371;
						}
						if (flag)
						{
							throw this.MakeException(SR.Format("Cannot include class \\{0} in character range.", c2.ToString()));
						}
						regexCharClass.AddDigit(this.UseOptionE(), c2 == 'D', this._pattern);
						goto IL_0371;
						IL_012B:
						if (scanOnly)
						{
							goto IL_0371;
						}
						if (flag)
						{
							throw this.MakeException(SR.Format("Cannot include class \\{0} in character range.", c2.ToString()));
						}
						regexCharClass.AddSpace(this.UseOptionE(), c2 == 'S');
						goto IL_0371;
						IL_019B:
						if (scanOnly)
						{
							this.ParseProperty();
							goto IL_0371;
						}
						if (flag)
						{
							throw this.MakeException(SR.Format("Cannot include class \\{0} in character range.", c2.ToString()));
						}
						regexCharClass.AddCategoryFromName(this.ParseProperty(), c2 != 'p', caseInsensitive, this._pattern);
						goto IL_0371;
						IL_01FA:
						this.MoveLeft();
						c2 = this.ScanCharEscape();
						flag4 = true;
						goto IL_0261;
					}
					if (c2 != '[' || this.CharsRight() <= 0 || this.RightChar() != ':' || flag)
					{
						goto IL_0261;
					}
					int num = this.Textpos();
					this.MoveRight();
					this.ScanCapname();
					if (this.CharsRight() < 2 || this.RightCharMoveRight() != ':' || this.RightCharMoveRight() != ']')
					{
						this.Textto(num);
						goto IL_0261;
					}
					goto IL_0261;
				}
				IL_0371:
				flag2 = false;
				continue;
				IL_0261:
				if (flag)
				{
					flag = false;
					if (scanOnly)
					{
						goto IL_0371;
					}
					if (c2 == '[' && !flag4 && !flag2)
					{
						regexCharClass.AddChar(c);
						regexCharClass.AddSubtraction(this.ScanCharClass(caseInsensitive, scanOnly));
						if (this.CharsRight() > 0 && this.RightChar() != ']')
						{
							throw this.MakeException("A subtraction must be the last element in a character class.");
						}
						goto IL_0371;
					}
					else
					{
						if (c > c2)
						{
							throw this.MakeException("[x-y] range in reverse order.");
						}
						regexCharClass.AddRange(c, c2);
						goto IL_0371;
					}
				}
				else
				{
					if (this.CharsRight() >= 2 && this.RightChar() == '-' && this.RightChar(1) != ']')
					{
						c = c2;
						flag = true;
						this.MoveRight();
						goto IL_0371;
					}
					if (this.CharsRight() >= 1 && c2 == '-' && !flag4 && this.RightChar() == '[' && !flag2)
					{
						if (scanOnly)
						{
							this.MoveRight(1);
							this.ScanCharClass(caseInsensitive, scanOnly);
							goto IL_0371;
						}
						this.MoveRight(1);
						regexCharClass.AddSubtraction(this.ScanCharClass(caseInsensitive, scanOnly));
						if (this.CharsRight() > 0 && this.RightChar() != ']')
						{
							throw this.MakeException("A subtraction must be the last element in a character class.");
						}
						goto IL_0371;
					}
					else
					{
						if (!scanOnly)
						{
							regexCharClass.AddRange(c2, c2);
							goto IL_0371;
						}
						goto IL_0371;
					}
				}
			}
			if (!flag3)
			{
				throw this.MakeException("Unterminated [] set.");
			}
			if (!scanOnly && caseInsensitive)
			{
				regexCharClass.AddLowercase(this._culture);
			}
			return regexCharClass;
		}

		private RegexNode ScanGroupOpen()
		{
			char c = '>';
			if (this.CharsRight() != 0 && this.RightChar() == '?' && (this.RightChar() != '?' || this.CharsRight() <= 1 || this.RightChar(1) != ')'))
			{
				this.MoveRight();
				if (this.CharsRight() != 0)
				{
					char c2 = this.RightCharMoveRight();
					int num;
					char c3;
					if (c2 <= '\'')
					{
						if (c2 == '!')
						{
							this._options &= ~RegexOptions.RightToLeft;
							num = 31;
							goto IL_04FA;
						}
						if (c2 != '\'')
						{
							goto IL_04C1;
						}
						c = '\'';
					}
					else if (c2 != '(')
					{
						switch (c2)
						{
						case ':':
							num = 29;
							goto IL_04FA;
						case ';':
							goto IL_04C1;
						case '<':
							break;
						case '=':
							this._options &= ~RegexOptions.RightToLeft;
							num = 30;
							goto IL_04FA;
						case '>':
							num = 32;
							goto IL_04FA;
						default:
							goto IL_04C1;
						}
					}
					else
					{
						int num2 = this.Textpos();
						if (this.CharsRight() > 0)
						{
							c3 = this.RightChar();
							if (c3 >= '0' && c3 <= '9')
							{
								int num3 = this.ScanDecimal();
								if (this.CharsRight() <= 0 || this.RightCharMoveRight() != ')')
								{
									throw this.MakeException(SR.Format("(?({0}) ) malformed.", num3.ToString(CultureInfo.CurrentCulture)));
								}
								if (this.IsCaptureSlot(num3))
								{
									return new RegexNode(33, this._options, num3);
								}
								throw this.MakeException(SR.Format("(?({0}) ) reference to undefined group.", num3.ToString(CultureInfo.CurrentCulture)));
							}
							else if (RegexCharClass.IsWordChar(c3))
							{
								string text = this.ScanCapname();
								if (this.IsCaptureName(text) && this.CharsRight() > 0 && this.RightCharMoveRight() == ')')
								{
									return new RegexNode(33, this._options, this.CaptureSlotFromName(text));
								}
							}
						}
						num = 34;
						this.Textto(num2 - 1);
						this._ignoreNextParen = true;
						int num4 = this.CharsRight();
						if (num4 < 3 || this.RightChar(1) != '?')
						{
							goto IL_04FA;
						}
						char c4 = this.RightChar(2);
						if (c4 == '#')
						{
							throw this.MakeException("Alternation conditions cannot be comments.");
						}
						if (c4 == '\'')
						{
							throw this.MakeException("Alternation conditions do not capture and cannot be named.");
						}
						if (num4 >= 4 && c4 == '<' && this.RightChar(3) != '!' && this.RightChar(3) != '=')
						{
							throw this.MakeException("Alternation conditions do not capture and cannot be named.");
						}
						goto IL_04FA;
					}
					if (this.CharsRight() == 0)
					{
						goto IL_0507;
					}
					char c5;
					c3 = (c5 = this.RightCharMoveRight());
					if (c5 != '!')
					{
						if (c5 == '=')
						{
							if (c != '\'')
							{
								this._options |= RegexOptions.RightToLeft;
								num = 30;
								goto IL_04FA;
							}
							goto IL_0507;
						}
						else
						{
							this.MoveLeft();
							int num5 = -1;
							int num6 = -1;
							bool flag = false;
							if (c3 >= '0' && c3 <= '9')
							{
								num5 = this.ScanDecimal();
								if (!this.IsCaptureSlot(num5))
								{
									num5 = -1;
								}
								if (this.CharsRight() > 0 && this.RightChar() != c && this.RightChar() != '-')
								{
									throw this.MakeException("Invalid group name: Group names must begin with a word character.");
								}
								if (num5 == 0)
								{
									throw this.MakeException("Capture number cannot be zero.");
								}
							}
							else if (RegexCharClass.IsWordChar(c3))
							{
								string text2 = this.ScanCapname();
								if (this.IsCaptureName(text2))
								{
									num5 = this.CaptureSlotFromName(text2);
								}
								if (this.CharsRight() > 0 && this.RightChar() != c && this.RightChar() != '-')
								{
									throw this.MakeException("Invalid group name: Group names must begin with a word character.");
								}
							}
							else
							{
								if (c3 != '-')
								{
									throw this.MakeException("Invalid group name: Group names must begin with a word character.");
								}
								flag = true;
							}
							if ((num5 != -1 || flag) && this.CharsRight() > 1 && this.RightChar() == '-')
							{
								this.MoveRight();
								c3 = this.RightChar();
								if (c3 >= '0' && c3 <= '9')
								{
									num6 = this.ScanDecimal();
									if (!this.IsCaptureSlot(num6))
									{
										throw this.MakeException(SR.Format("Reference to undefined group number {0}.", num6));
									}
									if (this.CharsRight() > 0 && this.RightChar() != c)
									{
										throw this.MakeException("Invalid group name: Group names must begin with a word character.");
									}
								}
								else
								{
									if (!RegexCharClass.IsWordChar(c3))
									{
										throw this.MakeException("Invalid group name: Group names must begin with a word character.");
									}
									string text3 = this.ScanCapname();
									if (!this.IsCaptureName(text3))
									{
										throw this.MakeException(SR.Format("Reference to undefined group name {0}.", text3));
									}
									num6 = this.CaptureSlotFromName(text3);
									if (this.CharsRight() > 0 && this.RightChar() != c)
									{
										throw this.MakeException("Invalid group name: Group names must begin with a word character.");
									}
								}
							}
							if ((num5 != -1 || num6 != -1) && this.CharsRight() > 0 && this.RightCharMoveRight() == c)
							{
								return new RegexNode(28, this._options, num5, num6);
							}
							goto IL_0507;
						}
					}
					else
					{
						if (c != '\'')
						{
							this._options |= RegexOptions.RightToLeft;
							num = 31;
							goto IL_04FA;
						}
						goto IL_0507;
					}
					IL_04C1:
					this.MoveLeft();
					num = 29;
					if (this._group.NType != 34)
					{
						this.ScanOptions();
					}
					if (this.CharsRight() == 0)
					{
						goto IL_0507;
					}
					if ((c3 = this.RightCharMoveRight()) == ')')
					{
						return null;
					}
					if (c3 != ':')
					{
						goto IL_0507;
					}
					IL_04FA:
					return new RegexNode(num, this._options);
				}
				IL_0507:
				throw this.MakeException("Unrecognized grouping construct.");
			}
			if (this.UseOptionN() || this._ignoreNextParen)
			{
				this._ignoreNextParen = false;
				return new RegexNode(29, this._options);
			}
			int num7 = 28;
			RegexOptions options = this._options;
			int autocap = this._autocap;
			this._autocap = autocap + 1;
			return new RegexNode(num7, options, autocap, -1);
		}

		private void ScanBlank()
		{
			if (this.UseOptionX())
			{
				for (;;)
				{
					if (this.CharsRight() <= 0 || !RegexParser.IsSpace(this.RightChar()))
					{
						if (this.CharsRight() == 0)
						{
							return;
						}
						if (this.RightChar() == '#')
						{
							while (this.CharsRight() > 0)
							{
								if (this.RightChar() == '\n')
								{
									break;
								}
								this.MoveRight();
							}
						}
						else
						{
							if (this.CharsRight() < 3 || this.RightChar(2) != '#' || this.RightChar(1) != '?' || this.RightChar() != '(')
							{
								return;
							}
							while (this.CharsRight() > 0 && this.RightChar() != ')')
							{
								this.MoveRight();
							}
							if (this.CharsRight() == 0)
							{
								break;
							}
							this.MoveRight();
						}
					}
					else
					{
						this.MoveRight();
					}
				}
				throw this.MakeException("Unterminated (?#...) comment.");
			}
			while (this.CharsRight() >= 3 && this.RightChar(2) == '#' && this.RightChar(1) == '?' && this.RightChar() == '(')
			{
				while (this.CharsRight() > 0 && this.RightChar() != ')')
				{
					this.MoveRight();
				}
				if (this.CharsRight() == 0)
				{
					throw this.MakeException("Unterminated (?#...) comment.");
				}
				this.MoveRight();
			}
		}

		private RegexNode ScanBackslash(bool scanOnly)
		{
			if (this.CharsRight() == 0)
			{
				throw this.MakeException("Illegal \\ at end of pattern.");
			}
			char c2;
			char c = (c2 = this.RightChar());
			if (c2 <= 'Z')
			{
				if (c2 <= 'P')
				{
					switch (c2)
					{
					case 'A':
					case 'B':
					case 'G':
						break;
					case 'C':
					case 'E':
					case 'F':
						goto IL_0274;
					case 'D':
						this.MoveRight();
						if (scanOnly)
						{
							return null;
						}
						if (this.UseOptionE())
						{
							return new RegexNode(11, this._options, "\u0001\u0002\00:");
						}
						return new RegexNode(11, this._options, RegexCharClass.NotDigitClass);
					default:
						if (c2 != 'P')
						{
							goto IL_0274;
						}
						goto IL_021B;
					}
				}
				else if (c2 != 'S')
				{
					if (c2 != 'W')
					{
						if (c2 != 'Z')
						{
							goto IL_0274;
						}
					}
					else
					{
						this.MoveRight();
						if (scanOnly)
						{
							return null;
						}
						if (this.UseOptionE())
						{
							return new RegexNode(11, this._options, "\u0001\n\00:A[_`a{İı");
						}
						return new RegexNode(11, this._options, RegexCharClass.NotWordClass);
					}
				}
				else
				{
					this.MoveRight();
					if (scanOnly)
					{
						return null;
					}
					if (this.UseOptionE())
					{
						return new RegexNode(11, this._options, "\u0001\u0004\0\t\u000e !");
					}
					return new RegexNode(11, this._options, RegexCharClass.NotSpaceClass);
				}
			}
			else if (c2 <= 'p')
			{
				if (c2 != 'b')
				{
					if (c2 != 'd')
					{
						if (c2 != 'p')
						{
							goto IL_0274;
						}
						goto IL_021B;
					}
					else
					{
						this.MoveRight();
						if (scanOnly)
						{
							return null;
						}
						if (this.UseOptionE())
						{
							return new RegexNode(11, this._options, "\0\u0002\00:");
						}
						return new RegexNode(11, this._options, RegexCharClass.DigitClass);
					}
				}
			}
			else if (c2 != 's')
			{
				if (c2 != 'w')
				{
					if (c2 != 'z')
					{
						goto IL_0274;
					}
				}
				else
				{
					this.MoveRight();
					if (scanOnly)
					{
						return null;
					}
					if (this.UseOptionE())
					{
						return new RegexNode(11, this._options, "\0\n\00:A[_`a{İı");
					}
					return new RegexNode(11, this._options, RegexCharClass.WordClass);
				}
			}
			else
			{
				this.MoveRight();
				if (scanOnly)
				{
					return null;
				}
				if (this.UseOptionE())
				{
					return new RegexNode(11, this._options, "\0\u0004\0\t\u000e !");
				}
				return new RegexNode(11, this._options, RegexCharClass.SpaceClass);
			}
			this.MoveRight();
			if (scanOnly)
			{
				return null;
			}
			return new RegexNode(this.TypeFromCode(c), this._options);
			IL_021B:
			this.MoveRight();
			if (scanOnly)
			{
				return null;
			}
			RegexCharClass regexCharClass = new RegexCharClass();
			regexCharClass.AddCategoryFromName(this.ParseProperty(), c != 'p', this.UseOptionI(), this._pattern);
			if (this.UseOptionI())
			{
				regexCharClass.AddLowercase(this._culture);
			}
			return new RegexNode(11, this._options, regexCharClass.ToStringClass());
			IL_0274:
			return this.ScanBasicBackslash(scanOnly);
		}

		private RegexNode ScanBasicBackslash(bool scanOnly)
		{
			if (this.CharsRight() == 0)
			{
				throw this.MakeException("Illegal \\ at end of pattern.");
			}
			bool flag = false;
			char c = '\0';
			int num = this.Textpos();
			char c2 = this.RightChar();
			if (c2 == 'k')
			{
				if (this.CharsRight() >= 2)
				{
					this.MoveRight();
					c2 = this.RightCharMoveRight();
					if (c2 == '<' || c2 == '\'')
					{
						flag = true;
						c = ((c2 == '\'') ? '\'' : '>');
					}
				}
				if (!flag || this.CharsRight() <= 0)
				{
					throw this.MakeException("Malformed \\k<...> named back reference.");
				}
				c2 = this.RightChar();
			}
			else if ((c2 == '<' || c2 == '\'') && this.CharsRight() > 1)
			{
				flag = true;
				c = ((c2 == '\'') ? '\'' : '>');
				this.MoveRight();
				c2 = this.RightChar();
			}
			if (flag && c2 >= '0' && c2 <= '9')
			{
				int num2 = this.ScanDecimal();
				if (this.CharsRight() > 0 && this.RightCharMoveRight() == c)
				{
					if (scanOnly)
					{
						return null;
					}
					if (this.IsCaptureSlot(num2))
					{
						return new RegexNode(13, this._options, num2);
					}
					throw this.MakeException(SR.Format("Reference to undefined group number {0}.", num2.ToString(CultureInfo.CurrentCulture)));
				}
			}
			else if (!flag && c2 >= '1' && c2 <= '9')
			{
				if (this.UseOptionE())
				{
					int num3 = -1;
					int i = (int)(c2 - '0');
					int num4 = this.Textpos() - 1;
					while (i <= this._captop)
					{
						if (this.IsCaptureSlot(i) && (this._caps == null || (int)this._caps[i] < num4))
						{
							num3 = i;
						}
						this.MoveRight();
						if (this.CharsRight() == 0 || (c2 = this.RightChar()) < '0' || c2 > '9')
						{
							break;
						}
						i = i * 10 + (int)(c2 - '0');
					}
					if (num3 >= 0)
					{
						if (!scanOnly)
						{
							return new RegexNode(13, this._options, num3);
						}
						return null;
					}
				}
				else
				{
					int num5 = this.ScanDecimal();
					if (scanOnly)
					{
						return null;
					}
					if (this.IsCaptureSlot(num5))
					{
						return new RegexNode(13, this._options, num5);
					}
					if (num5 <= 9)
					{
						throw this.MakeException(SR.Format("Reference to undefined group number {0}.", num5.ToString(CultureInfo.CurrentCulture)));
					}
				}
			}
			else if (flag && RegexCharClass.IsWordChar(c2))
			{
				string text = this.ScanCapname();
				if (this.CharsRight() > 0 && this.RightCharMoveRight() == c)
				{
					if (scanOnly)
					{
						return null;
					}
					if (this.IsCaptureName(text))
					{
						return new RegexNode(13, this._options, this.CaptureSlotFromName(text));
					}
					throw this.MakeException(SR.Format("Reference to undefined group name {0}.", text));
				}
			}
			this.Textto(num);
			c2 = this.ScanCharEscape();
			if (this.UseOptionI())
			{
				c2 = this._culture.TextInfo.ToLower(c2);
			}
			if (!scanOnly)
			{
				return new RegexNode(9, this._options, c2);
			}
			return null;
		}

		private RegexNode ScanDollar()
		{
			if (this.CharsRight() == 0)
			{
				return new RegexNode(9, this._options, '$');
			}
			char c = this.RightChar();
			int num = this.Textpos();
			int num2 = num;
			bool flag;
			if (c == '{' && this.CharsRight() > 1)
			{
				flag = true;
				this.MoveRight();
				c = this.RightChar();
			}
			else
			{
				flag = false;
			}
			if (c >= '0' && c <= '9')
			{
				if (!flag && this.UseOptionE())
				{
					int num3 = -1;
					int num4 = (int)(c - '0');
					this.MoveRight();
					if (this.IsCaptureSlot(num4))
					{
						num3 = num4;
						num2 = this.Textpos();
					}
					while (this.CharsRight() > 0 && (c = this.RightChar()) >= '0' && c <= '9')
					{
						int num5 = (int)(c - '0');
						if (num4 > 214748364 || (num4 == 214748364 && num5 > 7))
						{
							throw this.MakeException("Capture group numbers must be less than or equal to Int32.MaxValue.");
						}
						num4 = num4 * 10 + num5;
						this.MoveRight();
						if (this.IsCaptureSlot(num4))
						{
							num3 = num4;
							num2 = this.Textpos();
						}
					}
					this.Textto(num2);
					if (num3 >= 0)
					{
						return new RegexNode(13, this._options, num3);
					}
				}
				else
				{
					int num6 = this.ScanDecimal();
					if ((!flag || (this.CharsRight() > 0 && this.RightCharMoveRight() == '}')) && this.IsCaptureSlot(num6))
					{
						return new RegexNode(13, this._options, num6);
					}
				}
			}
			else if (flag && RegexCharClass.IsWordChar(c))
			{
				string text = this.ScanCapname();
				if (this.CharsRight() > 0 && this.RightCharMoveRight() == '}' && this.IsCaptureName(text))
				{
					return new RegexNode(13, this._options, this.CaptureSlotFromName(text));
				}
			}
			else if (!flag)
			{
				int num7 = 1;
				if (c <= '+')
				{
					switch (c)
					{
					case '$':
						this.MoveRight();
						return new RegexNode(9, this._options, '$');
					case '%':
						break;
					case '&':
						num7 = 0;
						break;
					case '\'':
						num7 = -2;
						break;
					default:
						if (c == '+')
						{
							num7 = -3;
						}
						break;
					}
				}
				else if (c != '_')
				{
					if (c == '`')
					{
						num7 = -1;
					}
				}
				else
				{
					num7 = -4;
				}
				if (num7 != 1)
				{
					this.MoveRight();
					return new RegexNode(13, this._options, num7);
				}
			}
			this.Textto(num);
			return new RegexNode(9, this._options, '$');
		}

		private string ScanCapname()
		{
			int num = this.Textpos();
			while (this.CharsRight() > 0)
			{
				if (!RegexCharClass.IsWordChar(this.RightCharMoveRight()))
				{
					this.MoveLeft();
					break;
				}
			}
			return this._pattern.Substring(num, this.Textpos() - num);
		}

		private char ScanOctal()
		{
			int num = 3;
			if (num > this.CharsRight())
			{
				num = this.CharsRight();
			}
			int num2 = 0;
			int num3;
			while (num > 0 && (num3 = (int)(this.RightChar() - '0')) <= 7)
			{
				this.MoveRight();
				num2 *= 8;
				num2 += num3;
				if (this.UseOptionE() && num2 >= 32)
				{
					break;
				}
				num--;
			}
			num2 &= 255;
			return (char)num2;
		}

		private int ScanDecimal()
		{
			int num = 0;
			int num2;
			while (this.CharsRight() > 0 && (num2 = (int)((ushort)(this.RightChar() - '0'))) <= 9)
			{
				this.MoveRight();
				if (num > 214748364 || (num == 214748364 && num2 > 7))
				{
					throw this.MakeException("Capture group numbers must be less than or equal to Int32.MaxValue.");
				}
				num *= 10;
				num += num2;
			}
			return num;
		}

		private char ScanHex(int c)
		{
			int num = 0;
			if (this.CharsRight() >= c)
			{
				int num2;
				while (c > 0 && (num2 = RegexParser.HexDigit(this.RightCharMoveRight())) >= 0)
				{
					num *= 16;
					num += num2;
					c--;
				}
			}
			if (c > 0)
			{
				throw this.MakeException("Insufficient hexadecimal digits.");
			}
			return (char)num;
		}

		private static int HexDigit(char ch)
		{
			int num;
			if ((num = (int)(ch - '0')) <= 9)
			{
				return num;
			}
			if ((num = (int)(ch - 'a')) <= 5)
			{
				return num + 10;
			}
			if ((num = (int)(ch - 'A')) <= 5)
			{
				return num + 10;
			}
			return -1;
		}

		private char ScanControl()
		{
			if (this.CharsRight() <= 0)
			{
				throw this.MakeException("Missing control character.");
			}
			char c = this.RightCharMoveRight();
			if (c >= 'a' && c <= 'z')
			{
				c -= ' ';
			}
			if ((c -= '@') < ' ')
			{
				return c;
			}
			throw this.MakeException("Unrecognized control character.");
		}

		private bool IsOnlyTopOption(RegexOptions option)
		{
			return option == RegexOptions.RightToLeft || option == RegexOptions.CultureInvariant || option == RegexOptions.ECMAScript;
		}

		private void ScanOptions()
		{
			bool flag = false;
			while (this.CharsRight() > 0)
			{
				char c = this.RightChar();
				if (c == '-')
				{
					flag = true;
				}
				else if (c == '+')
				{
					flag = false;
				}
				else
				{
					RegexOptions regexOptions = RegexParser.OptionFromCode(c);
					if (regexOptions == RegexOptions.None || this.IsOnlyTopOption(regexOptions))
					{
						return;
					}
					if (flag)
					{
						this._options &= ~regexOptions;
					}
					else
					{
						this._options |= regexOptions;
					}
				}
				this.MoveRight();
			}
		}

		private char ScanCharEscape()
		{
			char c = this.RightCharMoveRight();
			if (c >= '0' && c <= '7')
			{
				this.MoveLeft();
				return this.ScanOctal();
			}
			switch (c)
			{
			case 'a':
				return '\a';
			case 'b':
				return '\b';
			case 'c':
				return this.ScanControl();
			case 'd':
				break;
			case 'e':
				return '\u001b';
			case 'f':
				return '\f';
			default:
				switch (c)
				{
				case 'n':
					return '\n';
				case 'r':
					return '\r';
				case 't':
					return '\t';
				case 'u':
					return this.ScanHex(4);
				case 'v':
					return '\v';
				case 'x':
					return this.ScanHex(2);
				}
				break;
			}
			if (!this.UseOptionE() && RegexCharClass.IsWordChar(c))
			{
				throw this.MakeException(SR.Format("Unrecognized escape sequence \\{0}.", c.ToString()));
			}
			return c;
		}

		private string ParseProperty()
		{
			if (this.CharsRight() < 3)
			{
				throw this.MakeException("Incomplete \\p{X} character escape.");
			}
			char c = this.RightCharMoveRight();
			if (c != '{')
			{
				throw this.MakeException("Malformed \\p{X} character escape.");
			}
			int num = this.Textpos();
			while (this.CharsRight() > 0)
			{
				c = this.RightCharMoveRight();
				if (!RegexCharClass.IsWordChar(c) && c != '-')
				{
					this.MoveLeft();
					break;
				}
			}
			string text = this._pattern.Substring(num, this.Textpos() - num);
			if (this.CharsRight() == 0 || this.RightCharMoveRight() != '}')
			{
				throw this.MakeException("Incomplete \\p{X} character escape.");
			}
			return text;
		}

		private int TypeFromCode(char ch)
		{
			if (ch <= 'G')
			{
				if (ch == 'A')
				{
					return 18;
				}
				if (ch != 'B')
				{
					if (ch == 'G')
					{
						return 19;
					}
				}
				else
				{
					if (!this.UseOptionE())
					{
						return 17;
					}
					return 42;
				}
			}
			else
			{
				if (ch == 'Z')
				{
					return 20;
				}
				if (ch != 'b')
				{
					if (ch == 'z')
					{
						return 21;
					}
				}
				else
				{
					if (!this.UseOptionE())
					{
						return 16;
					}
					return 41;
				}
			}
			return 22;
		}

		private static RegexOptions OptionFromCode(char ch)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				ch += ' ';
			}
			if (ch <= 'i')
			{
				if (ch == 'e')
				{
					return RegexOptions.ECMAScript;
				}
				if (ch == 'i')
				{
					return RegexOptions.IgnoreCase;
				}
			}
			else
			{
				switch (ch)
				{
				case 'm':
					return RegexOptions.Multiline;
				case 'n':
					return RegexOptions.ExplicitCapture;
				case 'o':
				case 'p':
				case 'q':
					break;
				case 'r':
					return RegexOptions.RightToLeft;
				case 's':
					return RegexOptions.Singleline;
				default:
					if (ch == 'x')
					{
						return RegexOptions.IgnorePatternWhitespace;
					}
					break;
				}
			}
			return RegexOptions.None;
		}

		private void CountCaptures()
		{
			this.NoteCaptureSlot(0, 0);
			this._autocap = 1;
			while (this.CharsRight() > 0)
			{
				int num = this.Textpos();
				char c = this.RightCharMoveRight();
				if (c <= '(')
				{
					if (c != '#')
					{
						if (c == '(')
						{
							if (this.CharsRight() >= 2 && this.RightChar(1) == '#' && this.RightChar() == '?')
							{
								this.MoveLeft();
								this.ScanBlank();
							}
							else
							{
								this.PushOptions();
								if (this.CharsRight() > 0 && this.RightChar() == '?')
								{
									this.MoveRight();
									if (this.CharsRight() > 1 && (this.RightChar() == '<' || this.RightChar() == '\''))
									{
										this.MoveRight();
										c = this.RightChar();
										if (c != '0' && RegexCharClass.IsWordChar(c))
										{
											if (c >= '1' && c <= '9')
											{
												this.NoteCaptureSlot(this.ScanDecimal(), num);
											}
											else
											{
												this.NoteCaptureName(this.ScanCapname(), num);
											}
										}
									}
									else
									{
										this.ScanOptions();
										if (this.CharsRight() > 0)
										{
											if (this.RightChar() == ')')
											{
												this.MoveRight();
												this.PopKeepOptions();
											}
											else if (this.RightChar() == '(')
											{
												this._ignoreNextParen = true;
												continue;
											}
										}
									}
								}
								else if (!this.UseOptionN() && !this._ignoreNextParen)
								{
									int autocap = this._autocap;
									this._autocap = autocap + 1;
									this.NoteCaptureSlot(autocap, num);
								}
							}
							this._ignoreNextParen = false;
						}
					}
					else if (this.UseOptionX())
					{
						this.MoveLeft();
						this.ScanBlank();
					}
				}
				else if (c != ')')
				{
					if (c != '[')
					{
						if (c == '\\' && this.CharsRight() > 0)
						{
							this.ScanBackslash(true);
						}
					}
					else
					{
						this.ScanCharClass(false, true);
					}
				}
				else if (!this.EmptyOptionsStack())
				{
					this.PopOptions();
				}
			}
			this.AssignNameSlots();
		}

		private void NoteCaptureSlot(int i, int pos)
		{
			if (!this._caps.ContainsKey(i))
			{
				this._caps.Add(i, pos);
				this._capcount++;
				if (this._captop <= i)
				{
					if (i == 2147483647)
					{
						this._captop = i;
						return;
					}
					this._captop = i + 1;
				}
			}
		}

		private void NoteCaptureName(string name, int pos)
		{
			if (this._capnames == null)
			{
				this._capnames = new Hashtable();
				this._capnamelist = new List<string>();
			}
			if (!this._capnames.ContainsKey(name))
			{
				this._capnames.Add(name, pos);
				this._capnamelist.Add(name);
			}
		}

		private void NoteCaptures(Hashtable caps, int capsize, Hashtable capnames)
		{
			this._caps = caps;
			this._capsize = capsize;
			this._capnames = capnames;
		}

		private void AssignNameSlots()
		{
			if (this._capnames != null)
			{
				for (int i = 0; i < this._capnamelist.Count; i++)
				{
					while (this.IsCaptureSlot(this._autocap))
					{
						this._autocap++;
					}
					string text = this._capnamelist[i];
					int num = (int)this._capnames[text];
					this._capnames[text] = this._autocap;
					this.NoteCaptureSlot(this._autocap, num);
					this._autocap++;
				}
			}
			if (this._capcount < this._captop)
			{
				this._capnumlist = new int[this._capcount];
				int num2 = 0;
				IDictionaryEnumerator enumerator = this._caps.GetEnumerator();
				while (enumerator.MoveNext())
				{
					this._capnumlist[num2++] = (int)enumerator.Key;
				}
				Array.Sort<int>(this._capnumlist, Comparer<int>.Default);
			}
			if (this._capnames != null || this._capnumlist != null)
			{
				int num3 = 0;
				List<string> list;
				int num4;
				if (this._capnames == null)
				{
					list = null;
					this._capnames = new Hashtable();
					this._capnamelist = new List<string>();
					num4 = -1;
				}
				else
				{
					list = this._capnamelist;
					this._capnamelist = new List<string>();
					num4 = (int)this._capnames[list[0]];
				}
				for (int j = 0; j < this._capcount; j++)
				{
					int num5 = ((this._capnumlist == null) ? j : this._capnumlist[j]);
					if (num4 == num5)
					{
						this._capnamelist.Add(list[num3++]);
						num4 = ((num3 == list.Count) ? (-1) : ((int)this._capnames[list[num3]]));
					}
					else
					{
						string text2 = Convert.ToString(num5, this._culture);
						this._capnamelist.Add(text2);
						this._capnames[text2] = num5;
					}
				}
			}
		}

		private int CaptureSlotFromName(string capname)
		{
			return (int)this._capnames[capname];
		}

		private bool IsCaptureSlot(int i)
		{
			if (this._caps != null)
			{
				return this._caps.ContainsKey(i);
			}
			return i >= 0 && i < this._capsize;
		}

		private bool IsCaptureName(string capname)
		{
			return this._capnames != null && this._capnames.ContainsKey(capname);
		}

		private bool UseOptionN()
		{
			return (this._options & RegexOptions.ExplicitCapture) > RegexOptions.None;
		}

		private bool UseOptionI()
		{
			return (this._options & RegexOptions.IgnoreCase) > RegexOptions.None;
		}

		private bool UseOptionM()
		{
			return (this._options & RegexOptions.Multiline) > RegexOptions.None;
		}

		private bool UseOptionS()
		{
			return (this._options & RegexOptions.Singleline) > RegexOptions.None;
		}

		private bool UseOptionX()
		{
			return (this._options & RegexOptions.IgnorePatternWhitespace) > RegexOptions.None;
		}

		private bool UseOptionE()
		{
			return (this._options & RegexOptions.ECMAScript) > RegexOptions.None;
		}

		private static bool IsSpecial(char ch)
		{
			return ch <= '|' && RegexParser.s_category[(int)ch] >= 4;
		}

		private static bool IsStopperX(char ch)
		{
			return ch <= '|' && RegexParser.s_category[(int)ch] >= 2;
		}

		private static bool IsQuantifier(char ch)
		{
			return ch <= '{' && RegexParser.s_category[(int)ch] >= 5;
		}

		private bool IsTrueQuantifier()
		{
			int num = this.CharsRight();
			if (num == 0)
			{
				return false;
			}
			int num2 = this.Textpos();
			char c = this.CharAt(num2);
			if (c != '{')
			{
				return c <= '{' && RegexParser.s_category[(int)c] >= 5;
			}
			int num3 = num2;
			while (--num > 0 && (c = this.CharAt(++num3)) >= '0' && c <= '9')
			{
			}
			if (num == 0 || num3 - num2 == 1)
			{
				return false;
			}
			if (c == '}')
			{
				return true;
			}
			if (c != ',')
			{
				return false;
			}
			while (--num > 0 && (c = this.CharAt(++num3)) >= '0' && c <= '9')
			{
			}
			return num > 0 && c == '}';
		}

		private static bool IsSpace(char ch)
		{
			return ch <= ' ' && RegexParser.s_category[(int)ch] == 2;
		}

		private static bool IsMetachar(char ch)
		{
			return ch <= '|' && RegexParser.s_category[(int)ch] >= 1;
		}

		private void AddConcatenate(int pos, int cch, bool isReplacement)
		{
			if (cch == 0)
			{
				return;
			}
			RegexNode regexNode;
			if (cch > 1)
			{
				string text = this._pattern.Substring(pos, cch);
				if (this.UseOptionI() && !isReplacement)
				{
					StringBuilder stringBuilder = StringBuilderCache.Acquire(text.Length);
					for (int i = 0; i < text.Length; i++)
					{
						stringBuilder.Append(this._culture.TextInfo.ToLower(text[i]));
					}
					text = StringBuilderCache.GetStringAndRelease(stringBuilder);
				}
				regexNode = new RegexNode(12, this._options, text);
			}
			else
			{
				char c = this._pattern[pos];
				if (this.UseOptionI() && !isReplacement)
				{
					c = this._culture.TextInfo.ToLower(c);
				}
				regexNode = new RegexNode(9, this._options, c);
			}
			this._concatenation.AddChild(regexNode);
		}

		private void PushGroup()
		{
			this._group.Next = this._stack;
			this._alternation.Next = this._group;
			this._concatenation.Next = this._alternation;
			this._stack = this._concatenation;
		}

		private void PopGroup()
		{
			this._concatenation = this._stack;
			this._alternation = this._concatenation.Next;
			this._group = this._alternation.Next;
			this._stack = this._group.Next;
			if (this._group.Type() == 34 && this._group.ChildCount() == 0)
			{
				if (this._unit == null)
				{
					throw this.MakeException("Illegal conditional (?(...)) expression.");
				}
				this._group.AddChild(this._unit);
				this._unit = null;
			}
		}

		private bool EmptyStack()
		{
			return this._stack == null;
		}

		private void StartGroup(RegexNode openGroup)
		{
			this._group = openGroup;
			this._alternation = new RegexNode(24, this._options);
			this._concatenation = new RegexNode(25, this._options);
		}

		private void AddAlternate()
		{
			if (this._group.Type() == 34 || this._group.Type() == 33)
			{
				this._group.AddChild(this._concatenation.ReverseLeft());
			}
			else
			{
				this._alternation.AddChild(this._concatenation.ReverseLeft());
			}
			this._concatenation = new RegexNode(25, this._options);
		}

		private void AddConcatenate()
		{
			this._concatenation.AddChild(this._unit);
			this._unit = null;
		}

		private void AddConcatenate(bool lazy, int min, int max)
		{
			this._concatenation.AddChild(this._unit.MakeQuantifier(lazy, min, max));
			this._unit = null;
		}

		private RegexNode Unit()
		{
			return this._unit;
		}

		private void AddUnitOne(char ch)
		{
			if (this.UseOptionI())
			{
				ch = this._culture.TextInfo.ToLower(ch);
			}
			this._unit = new RegexNode(9, this._options, ch);
		}

		private void AddUnitNotone(char ch)
		{
			if (this.UseOptionI())
			{
				ch = this._culture.TextInfo.ToLower(ch);
			}
			this._unit = new RegexNode(10, this._options, ch);
		}

		private void AddUnitSet(string cc)
		{
			this._unit = new RegexNode(11, this._options, cc);
		}

		private void AddUnitNode(RegexNode node)
		{
			this._unit = node;
		}

		private void AddUnitType(int type)
		{
			this._unit = new RegexNode(type, this._options);
		}

		private void AddGroup()
		{
			if (this._group.Type() == 34 || this._group.Type() == 33)
			{
				this._group.AddChild(this._concatenation.ReverseLeft());
				if ((this._group.Type() == 33 && this._group.ChildCount() > 2) || this._group.ChildCount() > 3)
				{
					throw this.MakeException("Too many | in (?()|).");
				}
			}
			else
			{
				this._alternation.AddChild(this._concatenation.ReverseLeft());
				this._group.AddChild(this._alternation);
			}
			this._unit = this._group;
		}

		private void PushOptions()
		{
			this._optionsStack.Add(this._options);
		}

		private void PopOptions()
		{
			this._options = this._optionsStack[this._optionsStack.Count - 1];
			this._optionsStack.RemoveAt(this._optionsStack.Count - 1);
		}

		private bool EmptyOptionsStack()
		{
			return this._optionsStack.Count == 0;
		}

		private void PopKeepOptions()
		{
			this._optionsStack.RemoveAt(this._optionsStack.Count - 1);
		}

		private ArgumentException MakeException(string message)
		{
			return new ArgumentException(SR.Format("parsing \"{0}\" - {1}", this._pattern, message));
		}

		private int Textpos()
		{
			return this._currentPos;
		}

		private void Textto(int pos)
		{
			this._currentPos = pos;
		}

		private char RightCharMoveRight()
		{
			string pattern = this._pattern;
			int currentPos = this._currentPos;
			this._currentPos = currentPos + 1;
			return pattern[currentPos];
		}

		private void MoveRight()
		{
			this.MoveRight(1);
		}

		private void MoveRight(int i)
		{
			this._currentPos += i;
		}

		private void MoveLeft()
		{
			this._currentPos--;
		}

		private char CharAt(int i)
		{
			return this._pattern[i];
		}

		internal char RightChar()
		{
			return this._pattern[this._currentPos];
		}

		private char RightChar(int i)
		{
			return this._pattern[this._currentPos + i];
		}

		private int CharsRight()
		{
			return this._pattern.Length - this._currentPos;
		}

		private const int MaxValueDiv10 = 214748364;

		private const int MaxValueMod10 = 7;

		private RegexNode _stack;

		private RegexNode _group;

		private RegexNode _alternation;

		private RegexNode _concatenation;

		private RegexNode _unit;

		private string _pattern;

		private int _currentPos;

		private CultureInfo _culture;

		private int _autocap;

		private int _capcount;

		private int _captop;

		private int _capsize;

		private Hashtable _caps;

		private Hashtable _capnames;

		private int[] _capnumlist;

		private List<string> _capnamelist;

		private RegexOptions _options;

		private List<RegexOptions> _optionsStack;

		private bool _ignoreNextParen;

		private const byte Q = 5;

		private const byte S = 4;

		private const byte Z = 3;

		private const byte X = 2;

		private const byte E = 1;

		private static readonly byte[] s_category = new byte[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 2,
			2, 0, 2, 2, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 2, 0, 0, 3, 4, 0, 0, 0,
			4, 4, 5, 5, 0, 0, 4, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 5, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 4, 4, 0, 4, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 5, 4, 0, 0, 0
		};
	}
}
