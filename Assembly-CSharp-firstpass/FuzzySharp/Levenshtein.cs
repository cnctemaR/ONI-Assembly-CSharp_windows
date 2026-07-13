using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp.Edits;

namespace FuzzySharp
{
	public static class Levenshtein
	{
		private static EditOp[] GetEditOps<T>(T[] arr1, T[] arr2) where T : IEquatable<T>
		{
			return Levenshtein.GetEditOps<T>(arr1.Length, arr1, arr2.Length, arr2);
		}

		private static EditOp[] GetEditOps(string s1, string s2)
		{
			return Levenshtein.GetEditOps<char>(s1.Length, s1.ToCharArray(), s2.Length, s2.ToCharArray());
		}

		private static EditOp[] GetEditOps<T>(int len1, T[] c1, int len2, T[] c2) where T : IEquatable<T>
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (len1 > 0 && len2 > 0 && c1[num].Equals(c2[num2]))
			{
				len1--;
				len2--;
				num++;
				num2++;
				num3++;
			}
			int num4 = num3;
			while (len1 > 0 && len2 > 0 && c1[num + len1 - 1].Equals(c2[num2 + len2 - 1]))
			{
				len1--;
				len2--;
			}
			len1++;
			len2++;
			int[] array = new int[len2 * len1];
			for (int i = 0; i < len2; i++)
			{
				array[i] = i;
			}
			for (int i = 1; i < len1; i++)
			{
				array[len2 * i] = i;
			}
			for (int i = 1; i < len1; i++)
			{
				int num5 = (i - 1) * len2;
				int j = i * len2;
				int num6 = j + len2 - 1;
				T t = c1[num + i - 1];
				int num7 = num2;
				int num8 = i;
				j++;
				while (j <= num6)
				{
					int num9 = array[num5++] + ((!t.Equals(c2[num7++])) ? 1 : 0);
					num8++;
					if (num8 > num9)
					{
						num8 = num9;
					}
					num9 = array[num5] + 1;
					if (num8 > num9)
					{
						num8 = num9;
					}
					array[j++] = num8;
				}
			}
			return Levenshtein.EditOpsFromCostMatrix<T>(len1, c1, num, num3, len2, c2, num2, num4, array);
		}

		private static EditOp[] EditOpsFromCostMatrix<T>(int len1, T[] c1, int p1, int o1, int len2, T[] c2, int p2, int o2, int[] matrix) where T : IEquatable<T>
		{
			int num = 0;
			int num2 = matrix[len1 * len2 - 1];
			EditOp[] array = new EditOp[num2];
			int num3 = len1 - 1;
			int num4 = len2 - 1;
			int num5 = len1 * len2 - 1;
			while (num3 > 0 || num4 > 0)
			{
				if (num3 != 0 && num4 != 0 && matrix[num5] == matrix[num5 - len2 - 1] && c1[p1 + num3 - 1].Equals(c2[p2 + num4 - 1]))
				{
					num3--;
					num4--;
					num5 -= len2 + 1;
					num = 0;
				}
				else if (num < 0 && num4 != 0 && matrix[num5] == matrix[num5 - 1] + 1)
				{
					EditOp editOp = new EditOp();
					num2--;
					array[num2] = editOp;
					editOp.EditType = EditType.INSERT;
					editOp.SourcePos = num3 + o1;
					editOp.DestPos = --num4 + o2;
					num5--;
				}
				else if (num > 0 && num3 != 0 && matrix[num5] == matrix[num5 - len2] + 1)
				{
					EditOp editOp2 = new EditOp();
					num2--;
					array[num2] = editOp2;
					editOp2.EditType = EditType.DELETE;
					editOp2.SourcePos = --num3 + o1;
					editOp2.DestPos = num4 + o2;
					num5 -= len2;
				}
				else if (num3 != 0 && num4 != 0 && matrix[num5] == matrix[num5 - len2 - 1] + 1)
				{
					num2--;
					EditOp editOp3 = new EditOp();
					array[num2] = editOp3;
					editOp3.EditType = EditType.REPLACE;
					editOp3.SourcePos = --num3 + o1;
					editOp3.DestPos = --num4 + o2;
					num5 -= len2 + 1;
					num = 0;
				}
				else if (num == 0 && num4 != 0 && matrix[num5] == matrix[num5 - 1] + 1)
				{
					num2--;
					EditOp editOp4 = new EditOp();
					array[num2] = editOp4;
					editOp4.EditType = EditType.INSERT;
					editOp4.SourcePos = num3 + o1;
					editOp4.DestPos = --num4 + o2;
					num5--;
					num = -1;
				}
				else
				{
					if (num != 0 || num3 == 0 || matrix[num5] != matrix[num5 - len2] + 1)
					{
						throw new InvalidOperationException("Cant calculate edit op");
					}
					num2--;
					EditOp editOp5 = new EditOp();
					array[num2] = editOp5;
					editOp5.EditType = EditType.DELETE;
					editOp5.SourcePos = --num3 + o1;
					editOp5.DestPos = num4 + o2;
					num5 -= len2;
					num = 1;
				}
			}
			return array;
		}

		public static MatchingBlock[] GetMatchingBlocks<T>(T[] s1, T[] s2) where T : IEquatable<T>
		{
			return Levenshtein.GetMatchingBlocks(s1.Length, s2.Length, Levenshtein.GetEditOps<T>(s1, s2));
		}

		public static MatchingBlock[] GetMatchingBlocks(string s1, string s2)
		{
			return Levenshtein.GetMatchingBlocks(s1.Length, s2.Length, Levenshtein.GetEditOps(s1, s2));
		}

		public static MatchingBlock[] GetMatchingBlocks(int len1, int len2, OpCode[] ops)
		{
			int num = ops.Length;
			int num2 = 0;
			int num3 = 0;
			int num4 = num;
			while (num4-- != 0)
			{
				if (ops[num2].EditType == EditType.KEEP)
				{
					num3++;
					while (num4 != 0 && ops[num2].EditType == EditType.KEEP)
					{
						num4--;
						num2++;
					}
					if (num4 == 0)
					{
						break;
					}
				}
				num2++;
			}
			MatchingBlock[] array = new MatchingBlock[num3 + 1];
			int num5 = 0;
			num2 = 0;
			array[num5] = new MatchingBlock();
			num4 = num;
			while (num4 != 0)
			{
				if (ops[num2].EditType == EditType.KEEP)
				{
					array[num5].SourcePos = ops[num2].SourceBegin;
					array[num5].DestPos = ops[num2].DestBegin;
					while (num4 != 0 && ops[num2].EditType == EditType.KEEP)
					{
						num4--;
						num2++;
					}
					if (num4 == 0)
					{
						array[num5].Length = len1 - array[num5].SourcePos;
						num5++;
						break;
					}
					array[num5].Length = ops[num2].SourceBegin - array[num5].SourcePos;
					num5++;
					array[num5] = new MatchingBlock();
				}
				num4--;
				num2++;
			}
			Debug.Assert(num5 != num3);
			MatchingBlock matchingBlock = new MatchingBlock
			{
				SourcePos = len1,
				DestPos = len2,
				Length = 0
			};
			array[num5] = matchingBlock;
			return array;
		}

		private static MatchingBlock[] GetMatchingBlocks(int len1, int len2, EditOp[] ops)
		{
			int num = ops.Length;
			int num2 = 0;
			int num3 = 0;
			int num5;
			int num4 = (num5 = 0);
			int num6 = num;
			while (num6 != 0)
			{
				while (ops[num3].EditType == EditType.KEEP && --num6 != 0)
				{
					num3++;
				}
				if (num6 == 0)
				{
					break;
				}
				if (num5 < ops[num3].SourcePos || num4 < ops[num3].DestPos)
				{
					num2++;
					num5 = ops[num3].SourcePos;
					num4 = ops[num3].DestPos;
				}
				EditType editType = ops[num3].EditType;
				switch (editType)
				{
				case EditType.DELETE:
					do
					{
						num5++;
						num6--;
						num3++;
						if (num6 == 0 || ops[num3].EditType != editType || num5 != ops[num3].SourcePos)
						{
							break;
						}
					}
					while (num4 == ops[num3].DestPos);
					break;
				case EditType.INSERT:
					do
					{
						num4++;
						num6--;
						num3++;
					}
					while (num6 != 0 && ops[num3].EditType == editType && num5 == ops[num3].SourcePos && num4 == ops[num3].DestPos);
					break;
				case EditType.REPLACE:
					do
					{
						num5++;
						num4++;
						num6--;
						num3++;
						if (num6 == 0 || ops[num3].EditType != editType || num5 != ops[num3].SourcePos)
						{
							break;
						}
					}
					while (num4 == ops[num3].DestPos);
					break;
				}
			}
			if (num5 < len1 || num4 < len2)
			{
				num2++;
			}
			MatchingBlock[] array = new MatchingBlock[num2 + 1];
			num3 = 0;
			num4 = (num5 = 0);
			int num7 = 0;
			num6 = num;
			while (num6 != 0)
			{
				while (ops[num3].EditType == EditType.KEEP && --num6 != 0)
				{
					num3++;
				}
				if (num6 == 0)
				{
					break;
				}
				if (num5 < ops[num3].SourcePos || num4 < ops[num3].DestPos)
				{
					MatchingBlock matchingBlock = new MatchingBlock();
					matchingBlock.SourcePos = num5;
					matchingBlock.DestPos = num4;
					matchingBlock.Length = ops[num3].SourcePos - num5;
					num5 = ops[num3].SourcePos;
					num4 = ops[num3].DestPos;
					array[num7++] = matchingBlock;
				}
				EditType editType = ops[num3].EditType;
				switch (editType)
				{
				case EditType.DELETE:
					do
					{
						num5++;
						num6--;
						num3++;
						if (num6 == 0 || ops[num3].EditType != editType || num5 != ops[num3].SourcePos)
						{
							break;
						}
					}
					while (num4 == ops[num3].DestPos);
					break;
				case EditType.INSERT:
					do
					{
						num4++;
						num6--;
						num3++;
					}
					while (num6 != 0 && ops[num3].EditType == editType && num5 == ops[num3].SourcePos && num4 == ops[num3].DestPos);
					break;
				case EditType.REPLACE:
					do
					{
						num5++;
						num4++;
						num6--;
						num3++;
						if (num6 == 0 || ops[num3].EditType != editType || num5 != ops[num3].SourcePos)
						{
							break;
						}
					}
					while (num4 == ops[num3].DestPos);
					break;
				}
			}
			if (num5 < len1 || num4 < len2)
			{
				Debug.Assert(len1 - num5 == len2 - num4);
				MatchingBlock matchingBlock2 = new MatchingBlock();
				matchingBlock2.SourcePos = num5;
				matchingBlock2.DestPos = num4;
				matchingBlock2.Length = len1 - num5;
				array[num7++] = matchingBlock2;
			}
			Debug.Assert(num2 == num7);
			array[num7] = new MatchingBlock
			{
				SourcePos = len1,
				DestPos = len2,
				Length = 0
			};
			return array;
		}

		private static OpCode[] EditOpsToOpCodes(EditOp[] ops, int len1, int len2)
		{
			int num = ops.Length;
			int num2 = 0;
			int num3 = 0;
			int num5;
			int num4 = (num5 = 0);
			int num6 = num;
			while (num6 != 0)
			{
				while (ops[num2].EditType == EditType.KEEP && --num6 != 0)
				{
					num2++;
				}
				if (num6 == 0)
				{
					break;
				}
				if (num5 < ops[num2].SourcePos || num4 < ops[num2].DestPos)
				{
					num3++;
					num5 = ops[num2].SourcePos;
					num4 = ops[num2].DestPos;
				}
				num3++;
				EditType editType = ops[num2].EditType;
				switch (editType)
				{
				case EditType.DELETE:
					do
					{
						num5++;
						num6--;
						num2++;
						if (num6 == 0 || ops[num2].EditType != editType || num5 != ops[num2].SourcePos)
						{
							break;
						}
					}
					while (num4 == ops[num2].DestPos);
					break;
				case EditType.INSERT:
					do
					{
						num4++;
						num6--;
						num2++;
					}
					while (num6 != 0 && ops[num2].EditType == editType && num5 == ops[num2].SourcePos && num4 == ops[num2].DestPos);
					break;
				case EditType.REPLACE:
					do
					{
						num5++;
						num4++;
						num6--;
						num2++;
						if (num6 == 0 || ops[num2].EditType != editType || num5 != ops[num2].SourcePos)
						{
							break;
						}
					}
					while (num4 == ops[num2].DestPos);
					break;
				}
			}
			if (num5 < len1 || num4 < len2)
			{
				num3++;
			}
			OpCode[] array = new OpCode[num3];
			num2 = 0;
			num4 = (num5 = 0);
			int num7 = 0;
			num6 = num;
			while (num6 != 0)
			{
				while (ops[num2].EditType == EditType.KEEP && --num6 != 0)
				{
					num2++;
				}
				if (num6 == 0)
				{
					break;
				}
				OpCode opCode = new OpCode();
				array[num7] = opCode;
				opCode.SourceBegin = num5;
				opCode.DestBegin = num4;
				if (num5 < ops[num2].SourcePos || num4 < ops[num2].DestPos)
				{
					opCode.EditType = EditType.KEEP;
					num5 = (opCode.SourceEnd = ops[num2].SourcePos);
					num4 = (opCode.DestEnd = ops[num2].DestPos);
					num7++;
					OpCode opCode2 = new OpCode();
					array[num7] = opCode2;
					opCode2.SourceBegin = num5;
					opCode2.DestBegin = num4;
				}
				EditType editType = ops[num2].EditType;
				switch (editType)
				{
				case EditType.DELETE:
					do
					{
						num5++;
						num6--;
						num2++;
						if (num6 == 0 || ops[num2].EditType != editType || num5 != ops[num2].SourcePos)
						{
							break;
						}
					}
					while (num4 == ops[num2].DestPos);
					break;
				case EditType.INSERT:
					do
					{
						num4++;
						num6--;
						num2++;
					}
					while (num6 != 0 && ops[num2].EditType == editType && num5 == ops[num2].SourcePos && num4 == ops[num2].DestPos);
					break;
				case EditType.REPLACE:
					do
					{
						num5++;
						num4++;
						num6--;
						num2++;
						if (num6 == 0 || ops[num2].EditType != editType || num5 != ops[num2].SourcePos)
						{
							break;
						}
					}
					while (num4 == ops[num2].DestPos);
					break;
				}
				array[num7].EditType = editType;
				array[num7].SourceEnd = num5;
				array[num7].DestEnd = num4;
				num7++;
			}
			if (num5 < len1 || num4 < len2)
			{
				Debug.Assert(len1 - num5 == len2 - num4);
				if (array[num7] == null)
				{
					array[num7] = new OpCode();
				}
				array[num7].EditType = EditType.KEEP;
				array[num7].SourceBegin = num5;
				array[num7].DestBegin = num4;
				array[num7].SourceEnd = len1;
				array[num7].DestEnd = len2;
				num7++;
			}
			Debug.Assert(num7 == num3);
			return array;
		}

		public static int EditDistance(string s1, string s2, int xcost = 0)
		{
			return Levenshtein.EditDistance<char>(s1.ToCharArray(), s2.ToCharArray(), xcost);
		}

		public static int EditDistance<T>(T[] c1, T[] c2, int xcost = 0) where T : IEquatable<T>
		{
			int num = 0;
			int num2 = 0;
			int num3 = c1.Length;
			int num4 = c2.Length;
			while (num3 > 0 && num4 > 0)
			{
				if (!c1[num].Equals(c2[num2]))
				{
					break;
				}
				num3--;
				num4--;
				num++;
				num2++;
			}
			while (num3 > 0 && num4 > 0 && c1[num + num3 - 1].Equals(c2[num2 + num4 - 1]))
			{
				num3--;
				num4--;
			}
			if (num3 == 0)
			{
				return num4;
			}
			if (num4 == 0)
			{
				return num3;
			}
			if (num3 > num4)
			{
				int num5 = num3;
				int num6 = num;
				num3 = num4;
				num4 = num5;
				num = num2;
				num2 = num6;
				T[] array = c2;
				c2 = c1;
				c1 = array;
			}
			if (num3 != 1)
			{
				num3++;
				num4++;
				int num7 = num3 >> 1;
				int[] array2 = new int[num4];
				int num8 = num4 - 1;
				for (int i = 0; i < num4 - ((xcost != 0) ? 0 : num7); i++)
				{
					array2[i] = i;
				}
				if (xcost != 0)
				{
					for (int i = 1; i < num3; i++)
					{
						int j = 1;
						T t = c1[num + i - 1];
						int num9 = num2;
						int num10 = i;
						int num11 = i;
						while (j <= num8)
						{
							if (t.Equals(c2[num9++]))
							{
								num11 = num10 - 1;
							}
							else
							{
								num11++;
							}
							num10 = array2[j];
							num10++;
							if (num11 > num10)
							{
								num11 = num10;
							}
							array2[j++] = num11;
						}
					}
				}
				else
				{
					array2[0] = num3 - num7 - 1;
					for (int i = 1; i < num3; i++)
					{
						T t2 = c1[num + i - 1];
						int num13;
						int k;
						int num15;
						int num16;
						if (i >= num3 - num7)
						{
							int num12 = i - (num3 - num7);
							num13 = num2 + num12;
							k = num12;
							int num14 = array2[k++] + ((!t2.Equals(c2[num13++])) ? 1 : 0);
							num15 = array2[k];
							num15++;
							num16 = num15;
							if (num15 > num14)
							{
								num15 = num14;
							}
							array2[k++] = num15;
						}
						else
						{
							k = 1;
							num13 = num2;
							num15 = (num16 = i);
						}
						if (i <= num7 + 1)
						{
							num8 = num4 + i - num7 - 2;
						}
						while (k <= num8)
						{
							int num17 = num16 - 1 + ((!t2.Equals(c2[num13++])) ? 1 : 0);
							num15++;
							if (num15 > num17)
							{
								num15 = num17;
							}
							num16 = array2[k];
							num16++;
							if (num15 > num16)
							{
								num15 = num16;
							}
							array2[k++] = num15;
						}
						if (i <= num7)
						{
							int num18 = num16 - 1 + ((!t2.Equals(c2[num13])) ? 1 : 0);
							num15++;
							if (num15 > num18)
							{
								num15 = num18;
							}
							array2[k] = num15;
						}
					}
				}
				return array2[num8];
			}
			if (xcost != 0)
			{
				return num4 + 1 - 2 * Levenshtein.Memchr<T>(c2, num2, c1[num], num4);
			}
			return num4 - Levenshtein.Memchr<T>(c2, num2, c1[num], num4);
		}

		private static int Memchr<T>(T[] haystack, int offset, T needle, int num) where T : IEquatable<T>
		{
			if (num != 0)
			{
				int num2 = 0;
				while (!haystack[offset + num2].Equals(needle))
				{
					num2++;
					if (--num == 0)
					{
						return 0;
					}
				}
				return 1;
			}
			return 0;
		}

		public static double GetRatio<T>(T[] input1, T[] input2) where T : IEquatable<T>
		{
			int num = input1.Length;
			int num2 = input2.Length;
			int num3 = num + num2;
			int num4 = Levenshtein.EditDistance<T>(input1, input2, 1);
			if (num4 != 0)
			{
				return (double)(num3 - num4) / (double)num3;
			}
			return 1.0;
		}

		public static double GetRatio<T>(IEnumerable<T> input1, IEnumerable<T> input2) where T : IEquatable<T>
		{
			T[] array = input1.ToArray<T>();
			T[] array2 = input2.ToArray<T>();
			int num = array.Length;
			int num2 = array2.Length;
			int num3 = num + num2;
			int num4 = Levenshtein.EditDistance<T>(array, array2, 1);
			if (num4 != 0)
			{
				return (double)(num3 - num4) / (double)num3;
			}
			return 1.0;
		}

		public static double GetRatio(string s1, string s2)
		{
			return Levenshtein.GetRatio<char>(s1.ToCharArray(), s2.ToCharArray());
		}
	}
}
