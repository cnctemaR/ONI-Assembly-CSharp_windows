using System;
using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexInterpreter : RegexRunner
	{
		public RegexInterpreter(RegexCode code, CultureInfo culture)
		{
			this._code = code;
			this._culture = culture;
		}

		protected override void InitTrackCount()
		{
			this.runtrackcount = this._code.TrackCount;
		}

		private void Advance(int i)
		{
			this._codepos += i + 1;
			this.SetOperator(this._code.Codes[this._codepos]);
		}

		private void Goto(int newpos)
		{
			if (newpos < this._codepos)
			{
				base.EnsureStorage();
			}
			this.SetOperator(this._code.Codes[newpos]);
			this._codepos = newpos;
		}

		private void Textto(int newpos)
		{
			this.runtextpos = newpos;
		}

		private void Trackto(int newpos)
		{
			this.runtrackpos = this.runtrack.Length - newpos;
		}

		private int Textstart()
		{
			return this.runtextstart;
		}

		private int Textpos()
		{
			return this.runtextpos;
		}

		private int Trackpos()
		{
			return this.runtrack.Length - this.runtrackpos;
		}

		private void TrackPush()
		{
			int[] runtrack = this.runtrack;
			int num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack[num] = this._codepos;
		}

		private void TrackPush(int I1)
		{
			int[] runtrack = this.runtrack;
			int num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack[num] = I1;
			int[] runtrack2 = this.runtrack;
			num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack2[num] = this._codepos;
		}

		private void TrackPush(int I1, int I2)
		{
			int[] runtrack = this.runtrack;
			int num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack[num] = I1;
			int[] runtrack2 = this.runtrack;
			num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack2[num] = I2;
			int[] runtrack3 = this.runtrack;
			num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack3[num] = this._codepos;
		}

		private void TrackPush(int I1, int I2, int I3)
		{
			int[] runtrack = this.runtrack;
			int num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack[num] = I1;
			int[] runtrack2 = this.runtrack;
			num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack2[num] = I2;
			int[] runtrack3 = this.runtrack;
			num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack3[num] = I3;
			int[] runtrack4 = this.runtrack;
			num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack4[num] = this._codepos;
		}

		private void TrackPush2(int I1)
		{
			int[] runtrack = this.runtrack;
			int num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack[num] = I1;
			int[] runtrack2 = this.runtrack;
			num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack2[num] = -this._codepos;
		}

		private void TrackPush2(int I1, int I2)
		{
			int[] runtrack = this.runtrack;
			int num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack[num] = I1;
			int[] runtrack2 = this.runtrack;
			num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack2[num] = I2;
			int[] runtrack3 = this.runtrack;
			num = this.runtrackpos - 1;
			this.runtrackpos = num;
			runtrack3[num] = -this._codepos;
		}

		private void Backtrack()
		{
			int[] runtrack = this.runtrack;
			int runtrackpos = this.runtrackpos;
			this.runtrackpos = runtrackpos + 1;
			int num = runtrack[runtrackpos];
			if (num < 0)
			{
				num = -num;
				this.SetOperator(this._code.Codes[num] | 256);
			}
			else
			{
				this.SetOperator(this._code.Codes[num] | 128);
			}
			if (num < this._codepos)
			{
				base.EnsureStorage();
			}
			this._codepos = num;
		}

		private void SetOperator(int op)
		{
			this._caseInsensitive = (op & 512) != 0;
			this._rightToLeft = (op & 64) != 0;
			this._operator = op & -577;
		}

		private void TrackPop()
		{
			this.runtrackpos++;
		}

		private void TrackPop(int framesize)
		{
			this.runtrackpos += framesize;
		}

		private int TrackPeek()
		{
			return this.runtrack[this.runtrackpos - 1];
		}

		private int TrackPeek(int i)
		{
			return this.runtrack[this.runtrackpos - i - 1];
		}

		private void StackPush(int I1)
		{
			int[] runstack = this.runstack;
			int num = this.runstackpos - 1;
			this.runstackpos = num;
			runstack[num] = I1;
		}

		private void StackPush(int I1, int I2)
		{
			int[] runstack = this.runstack;
			int num = this.runstackpos - 1;
			this.runstackpos = num;
			runstack[num] = I1;
			int[] runstack2 = this.runstack;
			num = this.runstackpos - 1;
			this.runstackpos = num;
			runstack2[num] = I2;
		}

		private void StackPop()
		{
			this.runstackpos++;
		}

		private void StackPop(int framesize)
		{
			this.runstackpos += framesize;
		}

		private int StackPeek()
		{
			return this.runstack[this.runstackpos - 1];
		}

		private int StackPeek(int i)
		{
			return this.runstack[this.runstackpos - i - 1];
		}

		private int Operator()
		{
			return this._operator;
		}

		private int Operand(int i)
		{
			return this._code.Codes[this._codepos + i + 1];
		}

		private int Leftchars()
		{
			return this.runtextpos - this.runtextbeg;
		}

		private int Rightchars()
		{
			return this.runtextend - this.runtextpos;
		}

		private int Bump()
		{
			if (!this._rightToLeft)
			{
				return 1;
			}
			return -1;
		}

		private int Forwardchars()
		{
			if (!this._rightToLeft)
			{
				return this.runtextend - this.runtextpos;
			}
			return this.runtextpos - this.runtextbeg;
		}

		private char Forwardcharnext()
		{
			char c;
			if (!this._rightToLeft)
			{
				string runtext = this.runtext;
				int num = this.runtextpos;
				this.runtextpos = num + 1;
				c = runtext[num];
			}
			else
			{
				string runtext2 = this.runtext;
				int num = this.runtextpos - 1;
				this.runtextpos = num;
				c = runtext2[num];
			}
			char c2 = c;
			if (!this._caseInsensitive)
			{
				return c2;
			}
			return this._culture.TextInfo.ToLower(c2);
		}

		private bool Stringmatch(string str)
		{
			int num;
			int num2;
			if (!this._rightToLeft)
			{
				if (this.runtextend - this.runtextpos < (num = str.Length))
				{
					return false;
				}
				num2 = this.runtextpos + num;
			}
			else
			{
				if (this.runtextpos - this.runtextbeg < (num = str.Length))
				{
					return false;
				}
				num2 = this.runtextpos;
			}
			if (!this._caseInsensitive)
			{
				while (num != 0)
				{
					if (str[--num] != this.runtext[--num2])
					{
						return false;
					}
				}
			}
			else
			{
				while (num != 0)
				{
					if (str[--num] != this._culture.TextInfo.ToLower(this.runtext[--num2]))
					{
						return false;
					}
				}
			}
			if (!this._rightToLeft)
			{
				num2 += str.Length;
			}
			this.runtextpos = num2;
			return true;
		}

		private bool Refmatch(int index, int len)
		{
			int num;
			if (!this._rightToLeft)
			{
				if (this.runtextend - this.runtextpos < len)
				{
					return false;
				}
				num = this.runtextpos + len;
			}
			else
			{
				if (this.runtextpos - this.runtextbeg < len)
				{
					return false;
				}
				num = this.runtextpos;
			}
			int num2 = index + len;
			int num3 = len;
			if (!this._caseInsensitive)
			{
				while (num3-- != 0)
				{
					if (this.runtext[--num2] != this.runtext[--num])
					{
						return false;
					}
				}
			}
			else
			{
				while (num3-- != 0)
				{
					if (this._culture.TextInfo.ToLower(this.runtext[--num2]) != this._culture.TextInfo.ToLower(this.runtext[--num]))
					{
						return false;
					}
				}
			}
			if (!this._rightToLeft)
			{
				num += len;
			}
			this.runtextpos = num;
			return true;
		}

		private void Backwardnext()
		{
			this.runtextpos += (this._rightToLeft ? 1 : (-1));
		}

		private char CharAt(int j)
		{
			return this.runtext[j];
		}

		protected override bool FindFirstChar()
		{
			if ((this._code.Anchors & 53) != 0)
			{
				if (!this._code.RightToLeft)
				{
					if (((this._code.Anchors & 1) != 0 && this.runtextpos > this.runtextbeg) || ((this._code.Anchors & 4) != 0 && this.runtextpos > this.runtextstart))
					{
						this.runtextpos = this.runtextend;
						return false;
					}
					if ((this._code.Anchors & 16) != 0 && this.runtextpos < this.runtextend - 1)
					{
						this.runtextpos = this.runtextend - 1;
					}
					else if ((this._code.Anchors & 32) != 0 && this.runtextpos < this.runtextend)
					{
						this.runtextpos = this.runtextend;
					}
				}
				else
				{
					if (((this._code.Anchors & 32) != 0 && this.runtextpos < this.runtextend) || ((this._code.Anchors & 16) != 0 && (this.runtextpos < this.runtextend - 1 || (this.runtextpos == this.runtextend - 1 && this.CharAt(this.runtextpos) != '\n'))) || ((this._code.Anchors & 4) != 0 && this.runtextpos < this.runtextstart))
					{
						this.runtextpos = this.runtextbeg;
						return false;
					}
					if ((this._code.Anchors & 1) != 0 && this.runtextpos > this.runtextbeg)
					{
						this.runtextpos = this.runtextbeg;
					}
				}
				return this._code.BMPrefix == null || this._code.BMPrefix.IsMatch(this.runtext, this.runtextpos, this.runtextbeg, this.runtextend);
			}
			if (this._code.BMPrefix != null)
			{
				this.runtextpos = this._code.BMPrefix.Scan(this.runtext, this.runtextpos, this.runtextbeg, this.runtextend);
				if (this.runtextpos == -1)
				{
					this.runtextpos = (this._code.RightToLeft ? this.runtextbeg : this.runtextend);
					return false;
				}
				return true;
			}
			else
			{
				if (this._code.FCPrefix == null)
				{
					return true;
				}
				this._rightToLeft = this._code.RightToLeft;
				this._caseInsensitive = this._code.FCPrefix.GetValueOrDefault().CaseInsensitive;
				string prefix = this._code.FCPrefix.GetValueOrDefault().Prefix;
				if (RegexCharClass.IsSingleton(prefix))
				{
					char c = RegexCharClass.SingletonChar(prefix);
					for (int i = this.Forwardchars(); i > 0; i--)
					{
						if (c == this.Forwardcharnext())
						{
							this.Backwardnext();
							return true;
						}
					}
				}
				else
				{
					for (int j = this.Forwardchars(); j > 0; j--)
					{
						if (RegexCharClass.CharInClass(this.Forwardcharnext(), prefix))
						{
							this.Backwardnext();
							return true;
						}
					}
				}
				return false;
			}
		}

		protected override void Go()
		{
			this.Goto(0);
			int num = -1;
			for (;;)
			{
				if (num >= 0)
				{
					this.Advance(num);
					num = -1;
				}
				base.CheckTimeout();
				int num2 = this.Operator();
				switch (num2)
				{
				case 0:
				{
					int num3 = this.Operand(1);
					if (this.Forwardchars() >= num3)
					{
						char c = (char)this.Operand(0);
						while (num3-- > 0)
						{
							if (this.Forwardcharnext() != c)
							{
								goto IL_0DA8;
							}
						}
						num = 2;
						continue;
					}
					break;
				}
				case 1:
				{
					int num4 = this.Operand(1);
					if (this.Forwardchars() >= num4)
					{
						char c2 = (char)this.Operand(0);
						while (num4-- > 0)
						{
							if (this.Forwardcharnext() == c2)
							{
								goto IL_0DA8;
							}
						}
						num = 2;
						continue;
					}
					break;
				}
				case 2:
				{
					int num5 = this.Operand(1);
					if (this.Forwardchars() >= num5)
					{
						string text = this._code.Strings[this.Operand(0)];
						while (num5-- > 0)
						{
							if (!RegexCharClass.CharInClass(this.Forwardcharnext(), text))
							{
								goto IL_0DA8;
							}
						}
						num = 2;
						continue;
					}
					break;
				}
				case 3:
				{
					int num6 = this.Operand(1);
					if (num6 > this.Forwardchars())
					{
						num6 = this.Forwardchars();
					}
					char c3 = (char)this.Operand(0);
					int i;
					for (i = num6; i > 0; i--)
					{
						if (this.Forwardcharnext() != c3)
						{
							this.Backwardnext();
							break;
						}
					}
					if (num6 > i)
					{
						this.TrackPush(num6 - i - 1, this.Textpos() - this.Bump());
					}
					num = 2;
					continue;
				}
				case 4:
				{
					int num7 = this.Operand(1);
					if (num7 > this.Forwardchars())
					{
						num7 = this.Forwardchars();
					}
					char c4 = (char)this.Operand(0);
					int j;
					for (j = num7; j > 0; j--)
					{
						if (this.Forwardcharnext() == c4)
						{
							this.Backwardnext();
							break;
						}
					}
					if (num7 > j)
					{
						this.TrackPush(num7 - j - 1, this.Textpos() - this.Bump());
					}
					num = 2;
					continue;
				}
				case 5:
				{
					int num8 = this.Operand(1);
					if (num8 > this.Forwardchars())
					{
						num8 = this.Forwardchars();
					}
					string text2 = this._code.Strings[this.Operand(0)];
					int k;
					for (k = num8; k > 0; k--)
					{
						if (!RegexCharClass.CharInClass(this.Forwardcharnext(), text2))
						{
							this.Backwardnext();
							break;
						}
					}
					if (num8 > k)
					{
						this.TrackPush(num8 - k - 1, this.Textpos() - this.Bump());
					}
					num = 2;
					continue;
				}
				case 6:
				case 7:
				{
					int num9 = this.Operand(1);
					if (num9 > this.Forwardchars())
					{
						num9 = this.Forwardchars();
					}
					if (num9 > 0)
					{
						this.TrackPush(num9 - 1, this.Textpos());
					}
					num = 2;
					continue;
				}
				case 8:
				{
					int num10 = this.Operand(1);
					if (num10 > this.Forwardchars())
					{
						num10 = this.Forwardchars();
					}
					if (num10 > 0)
					{
						this.TrackPush(num10 - 1, this.Textpos());
					}
					num = 2;
					continue;
				}
				case 9:
					if (this.Forwardchars() >= 1 && this.Forwardcharnext() == (char)this.Operand(0))
					{
						num = 1;
						continue;
					}
					break;
				case 10:
					if (this.Forwardchars() >= 1 && this.Forwardcharnext() != (char)this.Operand(0))
					{
						num = 1;
						continue;
					}
					break;
				case 11:
					if (this.Forwardchars() >= 1 && RegexCharClass.CharInClass(this.Forwardcharnext(), this._code.Strings[this.Operand(0)]))
					{
						num = 1;
						continue;
					}
					break;
				case 12:
					if (this.Stringmatch(this._code.Strings[this.Operand(0)]))
					{
						num = 1;
						continue;
					}
					break;
				case 13:
				{
					int num11 = this.Operand(0);
					if (base.IsMatched(num11))
					{
						if (!this.Refmatch(base.MatchIndex(num11), base.MatchLength(num11)))
						{
							break;
						}
					}
					else if ((this.runregex.roptions & RegexOptions.ECMAScript) == RegexOptions.None)
					{
						break;
					}
					num = 1;
					continue;
				}
				case 14:
					if (this.Leftchars() <= 0 || this.CharAt(this.Textpos() - 1) == '\n')
					{
						num = 0;
						continue;
					}
					break;
				case 15:
					if (this.Rightchars() <= 0 || this.CharAt(this.Textpos()) == '\n')
					{
						num = 0;
						continue;
					}
					break;
				case 16:
					if (base.IsBoundary(this.Textpos(), this.runtextbeg, this.runtextend))
					{
						num = 0;
						continue;
					}
					break;
				case 17:
					if (!base.IsBoundary(this.Textpos(), this.runtextbeg, this.runtextend))
					{
						num = 0;
						continue;
					}
					break;
				case 18:
					if (this.Leftchars() <= 0)
					{
						num = 0;
						continue;
					}
					break;
				case 19:
					if (this.Textpos() == this.Textstart())
					{
						num = 0;
						continue;
					}
					break;
				case 20:
					if (this.Rightchars() <= 1 && (this.Rightchars() != 1 || this.CharAt(this.Textpos()) == '\n'))
					{
						num = 0;
						continue;
					}
					break;
				case 21:
					if (this.Rightchars() <= 0)
					{
						num = 0;
						continue;
					}
					break;
				case 22:
					break;
				case 23:
					this.TrackPush(this.Textpos());
					num = 1;
					continue;
				case 24:
					this.StackPop();
					if (this.Textpos() - this.StackPeek() != 0)
					{
						this.TrackPush(this.StackPeek(), this.Textpos());
						this.StackPush(this.Textpos());
						this.Goto(this.Operand(0));
						continue;
					}
					this.TrackPush2(this.StackPeek());
					num = 1;
					continue;
				case 25:
				{
					this.StackPop();
					int num12 = this.StackPeek();
					if (this.Textpos() != num12)
					{
						if (num12 != -1)
						{
							this.TrackPush(num12, this.Textpos());
						}
						else
						{
							this.TrackPush(this.Textpos(), this.Textpos());
						}
					}
					else
					{
						this.StackPush(num12);
						this.TrackPush2(this.StackPeek());
					}
					num = 1;
					continue;
				}
				case 26:
					this.StackPush(-1, this.Operand(0));
					this.TrackPush();
					num = 1;
					continue;
				case 27:
					this.StackPush(this.Textpos(), this.Operand(0));
					this.TrackPush();
					num = 1;
					continue;
				case 28:
				{
					this.StackPop(2);
					int num13 = this.StackPeek();
					int num14 = this.StackPeek(1);
					int num15 = this.Textpos() - num13;
					if (num14 >= this.Operand(1) || (num15 == 0 && num14 >= 0))
					{
						this.TrackPush2(num13, num14);
						num = 2;
						continue;
					}
					this.TrackPush(num13);
					this.StackPush(this.Textpos(), num14 + 1);
					this.Goto(this.Operand(0));
					continue;
				}
				case 29:
				{
					this.StackPop(2);
					int num16 = this.StackPeek();
					int num17 = this.StackPeek(1);
					if (num17 < 0)
					{
						this.TrackPush2(num16);
						this.StackPush(this.Textpos(), num17 + 1);
						this.Goto(this.Operand(0));
						continue;
					}
					this.TrackPush(num16, num17, this.Textpos());
					num = 2;
					continue;
				}
				case 30:
					this.StackPush(-1);
					this.TrackPush();
					num = 0;
					continue;
				case 31:
					this.StackPush(this.Textpos());
					this.TrackPush();
					num = 0;
					continue;
				case 32:
					if (this.Operand(1) == -1 || base.IsMatched(this.Operand(1)))
					{
						this.StackPop();
						if (this.Operand(1) != -1)
						{
							base.TransferCapture(this.Operand(0), this.Operand(1), this.StackPeek(), this.Textpos());
						}
						else
						{
							base.Capture(this.Operand(0), this.StackPeek(), this.Textpos());
						}
						this.TrackPush(this.StackPeek());
						num = 2;
						continue;
					}
					break;
				case 33:
					this.StackPop();
					this.TrackPush(this.StackPeek());
					this.Textto(this.StackPeek());
					num = 0;
					continue;
				case 34:
					this.StackPush(this.Trackpos(), base.Crawlpos());
					this.TrackPush();
					num = 0;
					continue;
				case 35:
					this.StackPop(2);
					this.Trackto(this.StackPeek());
					while (base.Crawlpos() != this.StackPeek(1))
					{
						base.Uncapture();
					}
					break;
				case 36:
					this.StackPop(2);
					this.Trackto(this.StackPeek());
					this.TrackPush(this.StackPeek(1));
					num = 0;
					continue;
				case 37:
					if (base.IsMatched(this.Operand(0)))
					{
						num = 1;
						continue;
					}
					break;
				case 38:
					this.Goto(this.Operand(0));
					continue;
				case 39:
					goto IL_0D9D;
				case 40:
					return;
				case 41:
					if (base.IsECMABoundary(this.Textpos(), this.runtextbeg, this.runtextend))
					{
						num = 0;
						continue;
					}
					break;
				case 42:
					if (!base.IsECMABoundary(this.Textpos(), this.runtextbeg, this.runtextend))
					{
						num = 0;
						continue;
					}
					break;
				default:
					switch (num2)
					{
					case 131:
					case 132:
					{
						this.TrackPop(2);
						int num18 = this.TrackPeek();
						int num19 = this.TrackPeek(1);
						this.Textto(num19);
						if (num18 > 0)
						{
							this.TrackPush(num18 - 1, num19 - this.Bump());
						}
						num = 2;
						continue;
					}
					case 133:
					{
						this.TrackPop(2);
						int num20 = this.TrackPeek();
						int num21 = this.TrackPeek(1);
						this.Textto(num21);
						if (num20 > 0)
						{
							this.TrackPush(num20 - 1, num21 - this.Bump());
						}
						num = 2;
						continue;
					}
					case 134:
					{
						this.TrackPop(2);
						int num22 = this.TrackPeek(1);
						this.Textto(num22);
						if (this.Forwardcharnext() == (char)this.Operand(0))
						{
							int num23 = this.TrackPeek();
							if (num23 > 0)
							{
								this.TrackPush(num23 - 1, num22 + this.Bump());
							}
							num = 2;
							continue;
						}
						break;
					}
					case 135:
					{
						this.TrackPop(2);
						int num24 = this.TrackPeek(1);
						this.Textto(num24);
						if (this.Forwardcharnext() != (char)this.Operand(0))
						{
							int num25 = this.TrackPeek();
							if (num25 > 0)
							{
								this.TrackPush(num25 - 1, num24 + this.Bump());
							}
							num = 2;
							continue;
						}
						break;
					}
					case 136:
					{
						this.TrackPop(2);
						int num26 = this.TrackPeek(1);
						this.Textto(num26);
						if (RegexCharClass.CharInClass(this.Forwardcharnext(), this._code.Strings[this.Operand(0)]))
						{
							int num27 = this.TrackPeek();
							if (num27 > 0)
							{
								this.TrackPush(num27 - 1, num26 + this.Bump());
							}
							num = 2;
							continue;
						}
						break;
					}
					case 137:
					case 138:
					case 139:
					case 140:
					case 141:
					case 142:
					case 143:
					case 144:
					case 145:
					case 146:
					case 147:
					case 148:
					case 149:
					case 150:
					case 163:
						goto IL_0D9D;
					case 151:
						this.TrackPop();
						this.Textto(this.TrackPeek());
						this.Goto(this.Operand(0));
						continue;
					case 152:
						this.TrackPop(2);
						this.StackPop();
						this.Textto(this.TrackPeek(1));
						this.TrackPush2(this.TrackPeek());
						num = 1;
						continue;
					case 153:
					{
						this.TrackPop(2);
						int num28 = this.TrackPeek(1);
						this.TrackPush2(this.TrackPeek());
						this.StackPush(num28);
						this.Textto(num28);
						this.Goto(this.Operand(0));
						continue;
					}
					case 154:
						this.StackPop(2);
						break;
					case 155:
						this.StackPop(2);
						break;
					case 156:
						this.TrackPop();
						this.StackPop(2);
						if (this.StackPeek(1) > 0)
						{
							this.Textto(this.StackPeek());
							this.TrackPush2(this.TrackPeek(), this.StackPeek(1) - 1);
							num = 2;
							continue;
						}
						this.StackPush(this.TrackPeek(), this.StackPeek(1) - 1);
						break;
					case 157:
					{
						this.TrackPop(3);
						int num29 = this.TrackPeek();
						int num30 = this.TrackPeek(2);
						if (this.TrackPeek(1) < this.Operand(1) && num30 != num29)
						{
							this.Textto(num30);
							this.StackPush(num30, this.TrackPeek(1) + 1);
							this.TrackPush2(num29);
							this.Goto(this.Operand(0));
							continue;
						}
						this.StackPush(this.TrackPeek(), this.TrackPeek(1));
						break;
					}
					case 158:
					case 159:
						this.StackPop();
						break;
					case 160:
						this.TrackPop();
						this.StackPush(this.TrackPeek());
						base.Uncapture();
						if (this.Operand(0) != -1 && this.Operand(1) != -1)
						{
							base.Uncapture();
						}
						break;
					case 161:
						this.TrackPop();
						this.StackPush(this.TrackPeek());
						break;
					case 162:
						this.StackPop(2);
						break;
					case 164:
						this.TrackPop();
						while (base.Crawlpos() != this.TrackPeek())
						{
							base.Uncapture();
						}
						break;
					default:
						switch (num2)
						{
						case 280:
							this.TrackPop();
							this.StackPush(this.TrackPeek());
							goto IL_0DA8;
						case 281:
							this.StackPop();
							this.TrackPop();
							this.StackPush(this.TrackPeek());
							goto IL_0DA8;
						case 284:
							this.TrackPop(2);
							this.StackPush(this.TrackPeek(), this.TrackPeek(1));
							goto IL_0DA8;
						case 285:
							this.TrackPop();
							this.StackPop(2);
							this.StackPush(this.TrackPeek(), this.StackPeek(1) - 1);
							goto IL_0DA8;
						}
						goto Block_4;
					}
					break;
				}
				IL_0DA8:
				this.Backtrack();
			}
			Block_4:
			IL_0D9D:
			throw global::System.NotImplemented.ByDesignWithMessage("Unimplemented state.");
		}

		private readonly RegexCode _code;

		private readonly CultureInfo _culture;

		private int _operator;

		private int _codepos;

		private bool _rightToLeft;

		private bool _caseInsensitive;
	}
}
