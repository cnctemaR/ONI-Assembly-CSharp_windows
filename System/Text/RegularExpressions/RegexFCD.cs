using System;
using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexFCD
	{
		internal static RegexPrefix FirstChars(RegexTree t)
		{
			RegexFC regexFC = new RegexFCD().RegexFCFromRegexTree(t);
			if (regexFC == null || regexFC._nullable)
			{
				return null;
			}
			CultureInfo cultureInfo = (((t._options & RegexOptions.CultureInvariant) != RegexOptions.None) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture);
			return new RegexPrefix(regexFC.GetFirstChars(cultureInfo), regexFC.IsCaseInsensitive());
		}

		internal static RegexPrefix Prefix(RegexTree tree)
		{
			RegexNode regexNode = null;
			int num = 0;
			RegexNode regexNode2 = tree._root;
			for (;;)
			{
				int type = regexNode2._type;
				switch (type)
				{
				case 3:
				case 6:
					goto IL_00C3;
				case 4:
				case 5:
				case 7:
				case 8:
				case 10:
				case 11:
				case 13:
				case 17:
				case 22:
				case 24:
				case 26:
				case 27:
				case 29:
					goto IL_0131;
				case 9:
					goto IL_00F9;
				case 12:
					goto IL_011A;
				case 14:
				case 15:
				case 16:
				case 18:
				case 19:
				case 20:
				case 21:
				case 23:
				case 30:
				case 31:
					break;
				case 25:
					if (regexNode2.ChildCount() > 0)
					{
						regexNode = regexNode2;
						num = 0;
					}
					break;
				case 28:
				case 32:
					regexNode2 = regexNode2.Child(0);
					regexNode = null;
					continue;
				default:
					if (type != 41)
					{
						goto Block_2;
					}
					break;
				}
				if (regexNode == null || num >= regexNode.ChildCount())
				{
					goto IL_0143;
				}
				regexNode2 = regexNode.Child(num++);
			}
			Block_2:
			goto IL_0131;
			IL_00C3:
			if (regexNode2._m > 0)
			{
				return new RegexPrefix(string.Empty.PadRight(regexNode2._m, regexNode2._ch), (regexNode2._options & RegexOptions.IgnoreCase) > RegexOptions.None);
			}
			return RegexPrefix.Empty;
			IL_00F9:
			return new RegexPrefix(regexNode2._ch.ToString(CultureInfo.InvariantCulture), (regexNode2._options & RegexOptions.IgnoreCase) > RegexOptions.None);
			IL_011A:
			return new RegexPrefix(regexNode2._str, (regexNode2._options & RegexOptions.IgnoreCase) > RegexOptions.None);
			IL_0131:
			return RegexPrefix.Empty;
			IL_0143:
			return RegexPrefix.Empty;
		}

		internal static int Anchors(RegexTree tree)
		{
			RegexNode regexNode = null;
			int num = 0;
			int num2 = 0;
			RegexNode regexNode2 = tree._root;
			int type;
			for (;;)
			{
				type = regexNode2._type;
				switch (type)
				{
				case 14:
				case 15:
				case 16:
				case 18:
				case 19:
				case 20:
				case 21:
					goto IL_0091;
				case 17:
				case 22:
				case 24:
				case 26:
				case 27:
				case 29:
					return num2;
				case 23:
				case 30:
				case 31:
					goto IL_00A1;
				case 25:
					if (regexNode2.ChildCount() > 0)
					{
						regexNode = regexNode2;
						num = 0;
						goto IL_00A1;
					}
					goto IL_00A1;
				case 28:
				case 32:
					regexNode2 = regexNode2.Child(0);
					regexNode = null;
					continue;
				}
				break;
				IL_00A1:
				if (regexNode == null || num >= regexNode.ChildCount())
				{
					return num2;
				}
				regexNode2 = regexNode.Child(num++);
			}
			if (type != 41)
			{
				return num2;
			}
			IL_0091:
			return num2 | RegexFCD.AnchorFromType(regexNode2._type);
		}

		private static int AnchorFromType(int type)
		{
			switch (type)
			{
			case 14:
				return 2;
			case 15:
				return 8;
			case 16:
				return 64;
			case 17:
				break;
			case 18:
				return 1;
			case 19:
				return 4;
			case 20:
				return 16;
			case 21:
				return 32;
			default:
				if (type == 41)
				{
					return 128;
				}
				break;
			}
			return 0;
		}

		private RegexFCD()
		{
			this._fcStack = new RegexFC[32];
			this._intStack = new int[32];
		}

		private void PushInt(int I)
		{
			if (this._intDepth >= this._intStack.Length)
			{
				int[] array = new int[this._intDepth * 2];
				Array.Copy(this._intStack, 0, array, 0, this._intDepth);
				this._intStack = array;
			}
			int[] intStack = this._intStack;
			int intDepth = this._intDepth;
			this._intDepth = intDepth + 1;
			intStack[intDepth] = I;
		}

		private bool IntIsEmpty()
		{
			return this._intDepth == 0;
		}

		private int PopInt()
		{
			int[] intStack = this._intStack;
			int num = this._intDepth - 1;
			this._intDepth = num;
			return intStack[num];
		}

		private void PushFC(RegexFC fc)
		{
			if (this._fcDepth >= this._fcStack.Length)
			{
				RegexFC[] array = new RegexFC[this._fcDepth * 2];
				Array.Copy(this._fcStack, 0, array, 0, this._fcDepth);
				this._fcStack = array;
			}
			RegexFC[] fcStack = this._fcStack;
			int fcDepth = this._fcDepth;
			this._fcDepth = fcDepth + 1;
			fcStack[fcDepth] = fc;
		}

		private bool FCIsEmpty()
		{
			return this._fcDepth == 0;
		}

		private RegexFC PopFC()
		{
			RegexFC[] fcStack = this._fcStack;
			int num = this._fcDepth - 1;
			this._fcDepth = num;
			return fcStack[num];
		}

		private RegexFC TopFC()
		{
			return this._fcStack[this._fcDepth - 1];
		}

		private RegexFC RegexFCFromRegexTree(RegexTree tree)
		{
			RegexNode regexNode = tree._root;
			int num = 0;
			for (;;)
			{
				if (regexNode._children == null)
				{
					this.CalculateFC(regexNode._type, regexNode, 0);
				}
				else if (num < regexNode._children.Count && !this._skipAllChildren)
				{
					this.CalculateFC(regexNode._type | 64, regexNode, num);
					if (!this._skipchild)
					{
						regexNode = regexNode._children[num];
						this.PushInt(num);
						num = 0;
						continue;
					}
					num++;
					this._skipchild = false;
					continue;
				}
				this._skipAllChildren = false;
				if (this.IntIsEmpty())
				{
					goto IL_00B9;
				}
				num = this.PopInt();
				regexNode = regexNode._next;
				this.CalculateFC(regexNode._type | 128, regexNode, num);
				if (this._failed)
				{
					break;
				}
				num++;
			}
			return null;
			IL_00B9:
			if (this.FCIsEmpty())
			{
				return null;
			}
			return this.PopFC();
		}

		private void SkipChild()
		{
			this._skipchild = true;
		}

		private void CalculateFC(int NodeType, RegexNode node, int CurIndex)
		{
			bool flag = false;
			bool flag2 = false;
			if (NodeType <= 13)
			{
				if ((node._options & RegexOptions.IgnoreCase) != RegexOptions.None)
				{
					flag = true;
				}
				if ((node._options & RegexOptions.RightToLeft) != RegexOptions.None)
				{
					flag2 = true;
				}
			}
			switch (NodeType)
			{
			case 3:
			case 6:
				this.PushFC(new RegexFC(node._ch, false, node._m == 0, flag));
				return;
			case 4:
			case 7:
				this.PushFC(new RegexFC(node._ch, true, node._m == 0, flag));
				return;
			case 5:
			case 8:
				this.PushFC(new RegexFC(node._str, node._m == 0, flag));
				return;
			case 9:
			case 10:
				this.PushFC(new RegexFC(node._ch, NodeType == 10, false, flag));
				return;
			case 11:
				this.PushFC(new RegexFC(node._str, false, flag));
				return;
			case 12:
				if (node._str.Length == 0)
				{
					this.PushFC(new RegexFC(true));
					return;
				}
				if (!flag2)
				{
					this.PushFC(new RegexFC(node._str[0], false, false, flag));
					return;
				}
				this.PushFC(new RegexFC(node._str[node._str.Length - 1], false, false, flag));
				return;
			case 13:
				this.PushFC(new RegexFC("\0\u0001\0\0", true, false));
				return;
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 41:
			case 42:
				this.PushFC(new RegexFC(true));
				return;
			case 23:
				this.PushFC(new RegexFC(true));
				return;
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
			case 40:
				break;
			default:
				switch (NodeType)
				{
				case 88:
				case 89:
				case 90:
				case 91:
				case 92:
				case 93:
				case 96:
				case 97:
					break;
				case 94:
				case 95:
					this.SkipChild();
					this.PushFC(new RegexFC(true));
					return;
				case 98:
					if (CurIndex == 0)
					{
						this.SkipChild();
						return;
					}
					break;
				default:
					switch (NodeType)
					{
					case 152:
					case 161:
						if (CurIndex != 0)
						{
							RegexFC regexFC = this.PopFC();
							RegexFC regexFC2 = this.TopFC();
							this._failed = !regexFC2.AddFC(regexFC, false);
							return;
						}
						break;
					case 153:
						if (CurIndex != 0)
						{
							RegexFC regexFC3 = this.PopFC();
							RegexFC regexFC4 = this.TopFC();
							this._failed = !regexFC4.AddFC(regexFC3, true);
						}
						if (!this.TopFC()._nullable)
						{
							this._skipAllChildren = true;
							return;
						}
						break;
					case 154:
					case 155:
						if (node._m == 0)
						{
							this.TopFC()._nullable = true;
							return;
						}
						break;
					case 156:
					case 157:
					case 158:
					case 159:
					case 160:
						break;
					case 162:
						if (CurIndex > 1)
						{
							RegexFC regexFC5 = this.PopFC();
							RegexFC regexFC6 = this.TopFC();
							this._failed = !regexFC6.AddFC(regexFC5, false);
							return;
						}
						break;
					default:
						goto IL_0312;
					}
					break;
				}
				return;
			}
			IL_0312:
			throw new ArgumentException(global::SR.GetString("Unexpected opcode in regular expression generation: {0}.", new object[] { NodeType.ToString(CultureInfo.CurrentCulture) }));
		}

		private int[] _intStack;

		private int _intDepth;

		private RegexFC[] _fcStack;

		private int _fcDepth;

		private bool _skipAllChildren;

		private bool _skipchild;

		private bool _failed;

		private const int BeforeChild = 64;

		private const int AfterChild = 128;

		internal const int Beginning = 1;

		internal const int Bol = 2;

		internal const int Start = 4;

		internal const int Eol = 8;

		internal const int EndZ = 16;

		internal const int End = 32;

		internal const int Boundary = 64;

		internal const int ECMABoundary = 128;
	}
}
