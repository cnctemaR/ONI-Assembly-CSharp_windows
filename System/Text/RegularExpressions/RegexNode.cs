using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexNode
	{
		internal RegexNode(int type, RegexOptions options)
		{
			this._type = type;
			this._options = options;
		}

		internal RegexNode(int type, RegexOptions options, char ch)
		{
			this._type = type;
			this._options = options;
			this._ch = ch;
		}

		internal RegexNode(int type, RegexOptions options, string str)
		{
			this._type = type;
			this._options = options;
			this._str = str;
		}

		internal RegexNode(int type, RegexOptions options, int m)
		{
			this._type = type;
			this._options = options;
			this._m = m;
		}

		internal RegexNode(int type, RegexOptions options, int m, int n)
		{
			this._type = type;
			this._options = options;
			this._m = m;
			this._n = n;
		}

		internal bool UseOptionR()
		{
			return (this._options & RegexOptions.RightToLeft) > RegexOptions.None;
		}

		internal RegexNode ReverseLeft()
		{
			if (this.UseOptionR() && this._type == 25 && this._children != null)
			{
				this._children.Reverse(0, this._children.Count);
			}
			return this;
		}

		internal void MakeRep(int type, int min, int max)
		{
			this._type += type - 9;
			this._m = min;
			this._n = max;
		}

		internal RegexNode Reduce()
		{
			int num = this.Type();
			RegexNode regexNode;
			if (num != 5 && num != 11)
			{
				switch (num)
				{
				case 24:
					return this.ReduceAlternation();
				case 25:
					return this.ReduceConcatenation();
				case 26:
				case 27:
					return this.ReduceRep();
				case 29:
					return this.ReduceGroup();
				}
				regexNode = this;
			}
			else
			{
				regexNode = this.ReduceSet();
			}
			return regexNode;
		}

		internal RegexNode StripEnation(int emptyType)
		{
			int num = this.ChildCount();
			if (num == 0)
			{
				return new RegexNode(emptyType, this._options);
			}
			if (num != 1)
			{
				return this;
			}
			return this.Child(0);
		}

		internal RegexNode ReduceGroup()
		{
			RegexNode regexNode = this;
			while (regexNode.Type() == 29)
			{
				regexNode = regexNode.Child(0);
			}
			return regexNode;
		}

		internal RegexNode ReduceRep()
		{
			RegexNode regexNode = this;
			int num = this.Type();
			int num2 = this._m;
			int num3 = this._n;
			while (regexNode.ChildCount() != 0)
			{
				RegexNode regexNode2 = regexNode.Child(0);
				if (regexNode2.Type() != num)
				{
					int num4 = regexNode2.Type();
					if ((num4 < 3 || num4 > 5 || num != 26) && (num4 < 6 || num4 > 8 || num != 27))
					{
						break;
					}
				}
				if ((regexNode._m == 0 && regexNode2._m > 1) || regexNode2._n < regexNode2._m * 2)
				{
					break;
				}
				regexNode = regexNode2;
				if (regexNode._m > 0)
				{
					num2 = (regexNode._m = ((2147483646 / regexNode._m < num2) ? int.MaxValue : (regexNode._m * num2)));
				}
				if (regexNode._n > 0)
				{
					num3 = (regexNode._n = ((2147483646 / regexNode._n < num3) ? int.MaxValue : (regexNode._n * num3)));
				}
			}
			if (num2 != 2147483647)
			{
				return regexNode;
			}
			return new RegexNode(22, this._options);
		}

		internal RegexNode ReduceSet()
		{
			if (RegexCharClass.IsEmpty(this._str))
			{
				this._type = 22;
				this._str = null;
			}
			else if (RegexCharClass.IsSingleton(this._str))
			{
				this._ch = RegexCharClass.SingletonChar(this._str);
				this._str = null;
				this._type += -2;
			}
			else if (RegexCharClass.IsSingletonInverse(this._str))
			{
				this._ch = RegexCharClass.SingletonChar(this._str);
				this._str = null;
				this._type += -1;
			}
			return this;
		}

		internal RegexNode ReduceAlternation()
		{
			if (this._children == null)
			{
				return new RegexNode(22, this._options);
			}
			bool flag = false;
			bool flag2 = false;
			RegexOptions regexOptions = RegexOptions.None;
			int i = 0;
			int num = 0;
			while (i < this._children.Count)
			{
				RegexNode regexNode = this._children[i];
				if (num < i)
				{
					this._children[num] = regexNode;
				}
				if (regexNode._type == 24)
				{
					for (int j = 0; j < regexNode._children.Count; j++)
					{
						regexNode._children[j]._next = this;
					}
					this._children.InsertRange(i + 1, regexNode._children);
					num--;
				}
				else if (regexNode._type == 11 || regexNode._type == 9)
				{
					RegexOptions regexOptions2 = regexNode._options & (RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
					if (regexNode._type == 11)
					{
						if (!flag || regexOptions != regexOptions2 || flag2 || !RegexCharClass.IsMergeable(regexNode._str))
						{
							flag = true;
							flag2 = !RegexCharClass.IsMergeable(regexNode._str);
							regexOptions = regexOptions2;
							goto IL_01D0;
						}
					}
					else if (!flag || regexOptions != regexOptions2 || flag2)
					{
						flag = true;
						flag2 = false;
						regexOptions = regexOptions2;
						goto IL_01D0;
					}
					num--;
					RegexNode regexNode2 = this._children[num];
					RegexCharClass regexCharClass;
					if (regexNode2._type == 9)
					{
						regexCharClass = new RegexCharClass();
						regexCharClass.AddChar(regexNode2._ch);
					}
					else
					{
						regexCharClass = RegexCharClass.Parse(regexNode2._str);
					}
					if (regexNode._type == 9)
					{
						regexCharClass.AddChar(regexNode._ch);
					}
					else
					{
						RegexCharClass regexCharClass2 = RegexCharClass.Parse(regexNode._str);
						regexCharClass.AddCharClass(regexCharClass2);
					}
					regexNode2._type = 11;
					regexNode2._str = regexCharClass.ToStringClass();
				}
				else if (regexNode._type == 22)
				{
					num--;
				}
				else
				{
					flag = false;
					flag2 = false;
				}
				IL_01D0:
				i++;
				num++;
			}
			if (num < i)
			{
				this._children.RemoveRange(num, i - num);
			}
			return this.StripEnation(22);
		}

		internal RegexNode ReduceConcatenation()
		{
			if (this._children == null)
			{
				return new RegexNode(23, this._options);
			}
			bool flag = false;
			RegexOptions regexOptions = RegexOptions.None;
			int i = 0;
			int num = 0;
			while (i < this._children.Count)
			{
				RegexNode regexNode = this._children[i];
				if (num < i)
				{
					this._children[num] = regexNode;
				}
				if (regexNode._type == 25 && (regexNode._options & RegexOptions.RightToLeft) == (this._options & RegexOptions.RightToLeft))
				{
					for (int j = 0; j < regexNode._children.Count; j++)
					{
						regexNode._children[j]._next = this;
					}
					this._children.InsertRange(i + 1, regexNode._children);
					num--;
				}
				else if (regexNode._type == 12 || regexNode._type == 9)
				{
					RegexOptions regexOptions2 = regexNode._options & (RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
					if (!flag || regexOptions != regexOptions2)
					{
						flag = true;
						regexOptions = regexOptions2;
					}
					else
					{
						RegexNode regexNode2 = this._children[--num];
						if (regexNode2._type == 9)
						{
							regexNode2._type = 12;
							regexNode2._str = Convert.ToString(regexNode2._ch, CultureInfo.InvariantCulture);
						}
						if ((regexOptions2 & RegexOptions.RightToLeft) == RegexOptions.None)
						{
							if (regexNode._type == 9)
							{
								RegexNode regexNode3 = regexNode2;
								regexNode3._str += regexNode._ch.ToString();
							}
							else
							{
								RegexNode regexNode4 = regexNode2;
								regexNode4._str += regexNode._str;
							}
						}
						else if (regexNode._type == 9)
						{
							regexNode2._str = regexNode._ch.ToString() + regexNode2._str;
						}
						else
						{
							regexNode2._str = regexNode._str + regexNode2._str;
						}
					}
				}
				else if (regexNode._type == 23)
				{
					num--;
				}
				else
				{
					flag = false;
				}
				i++;
				num++;
			}
			if (num < i)
			{
				this._children.RemoveRange(num, i - num);
			}
			return this.StripEnation(23);
		}

		internal RegexNode MakeQuantifier(bool lazy, int min, int max)
		{
			if (min == 0 && max == 0)
			{
				return new RegexNode(23, this._options);
			}
			if (min == 1 && max == 1)
			{
				return this;
			}
			int type = this._type;
			if (type - 9 <= 2)
			{
				this.MakeRep(lazy ? 6 : 3, min, max);
				return this;
			}
			RegexNode regexNode = new RegexNode(lazy ? 27 : 26, this._options, min, max);
			regexNode.AddChild(this);
			return regexNode;
		}

		internal void AddChild(RegexNode newChild)
		{
			if (this._children == null)
			{
				this._children = new List<RegexNode>(4);
			}
			RegexNode regexNode = newChild.Reduce();
			this._children.Add(regexNode);
			regexNode._next = this;
		}

		internal RegexNode Child(int i)
		{
			return this._children[i];
		}

		internal int ChildCount()
		{
			if (this._children != null)
			{
				return this._children.Count;
			}
			return 0;
		}

		internal int Type()
		{
			return this._type;
		}

		internal const int Oneloop = 3;

		internal const int Notoneloop = 4;

		internal const int Setloop = 5;

		internal const int Onelazy = 6;

		internal const int Notonelazy = 7;

		internal const int Setlazy = 8;

		internal const int One = 9;

		internal const int Notone = 10;

		internal const int Set = 11;

		internal const int Multi = 12;

		internal const int Ref = 13;

		internal const int Bol = 14;

		internal const int Eol = 15;

		internal const int Boundary = 16;

		internal const int Nonboundary = 17;

		internal const int ECMABoundary = 41;

		internal const int NonECMABoundary = 42;

		internal const int Beginning = 18;

		internal const int Start = 19;

		internal const int EndZ = 20;

		internal const int End = 21;

		internal const int Nothing = 22;

		internal const int Empty = 23;

		internal const int Alternate = 24;

		internal const int Concatenate = 25;

		internal const int Loop = 26;

		internal const int Lazyloop = 27;

		internal const int Capture = 28;

		internal const int Group = 29;

		internal const int Require = 30;

		internal const int Prevent = 31;

		internal const int Greedy = 32;

		internal const int Testref = 33;

		internal const int Testgroup = 34;

		internal int _type;

		internal List<RegexNode> _children;

		internal string _str;

		internal char _ch;

		internal int _m;

		internal int _n;

		internal RegexOptions _options;

		internal RegexNode _next;
	}
}
