using System;

namespace System.Data
{
	internal sealed class Operators
	{
		private Operators()
		{
		}

		internal static bool IsArithmetical(int op)
		{
			return op == 15 || op == 16 || op == 17 || op == 18 || op == 20;
		}

		internal static bool IsLogical(int op)
		{
			return op == 26 || op == 27 || op == 3 || op == 13 || op == 39;
		}

		internal static bool IsRelational(int op)
		{
			return 7 <= op && op <= 12;
		}

		internal static int Priority(int op)
		{
			if (op > Operators.s_priority.Length)
			{
				return 24;
			}
			return Operators.s_priority[op];
		}

		internal static string ToString(int op)
		{
			string text;
			if (op <= Operators.s_looks.Length)
			{
				text = Operators.s_looks[op];
			}
			else
			{
				text = "Unknown op";
			}
			return text;
		}

		internal const int Noop = 0;

		internal const int Negative = 1;

		internal const int UnaryPlus = 2;

		internal const int Not = 3;

		internal const int BetweenAnd = 4;

		internal const int In = 5;

		internal const int Between = 6;

		internal const int EqualTo = 7;

		internal const int GreaterThen = 8;

		internal const int LessThen = 9;

		internal const int GreaterOrEqual = 10;

		internal const int LessOrEqual = 11;

		internal const int NotEqual = 12;

		internal const int Is = 13;

		internal const int Like = 14;

		internal const int Plus = 15;

		internal const int Minus = 16;

		internal const int Multiply = 17;

		internal const int Divide = 18;

		internal const int Modulo = 20;

		internal const int BitwiseAnd = 22;

		internal const int BitwiseOr = 23;

		internal const int BitwiseXor = 24;

		internal const int BitwiseNot = 25;

		internal const int And = 26;

		internal const int Or = 27;

		internal const int Proc = 28;

		internal const int Iff = 29;

		internal const int Qual = 30;

		internal const int Dot = 31;

		internal const int Null = 32;

		internal const int True = 33;

		internal const int False = 34;

		internal const int Date = 35;

		internal const int GenUniqueId = 36;

		internal const int GenGUID = 37;

		internal const int GUID = 38;

		internal const int IsNot = 39;

		internal const int priStart = 0;

		internal const int priSubstr = 1;

		internal const int priParen = 2;

		internal const int priLow = 3;

		internal const int priImp = 4;

		internal const int priEqv = 5;

		internal const int priXor = 6;

		internal const int priOr = 7;

		internal const int priAnd = 8;

		internal const int priNot = 9;

		internal const int priIs = 10;

		internal const int priBetweenInLike = 11;

		internal const int priBetweenAnd = 12;

		internal const int priRelOp = 13;

		internal const int priConcat = 14;

		internal const int priContains = 15;

		internal const int priPlusMinus = 16;

		internal const int priMod = 17;

		internal const int priIDiv = 18;

		internal const int priMulDiv = 19;

		internal const int priNeg = 20;

		internal const int priExp = 21;

		internal const int priProc = 22;

		internal const int priDot = 23;

		internal const int priMax = 24;

		private static readonly int[] s_priority = new int[]
		{
			0, 20, 20, 9, 12, 11, 11, 13, 13, 13,
			13, 13, 13, 10, 11, 16, 16, 19, 19, 18,
			17, 21, 8, 7, 6, 9, 8, 7, 2, 22,
			23, 23, 24, 24, 24, 24, 24, 24, 24, 24,
			24, 24, 24, 24
		};

		private static readonly string[] s_looks = new string[]
		{
			"", "-", "+", "Not", "BetweenAnd", "In", "Between", "=", ">", "<",
			">=", "<=", "<>", "Is", "Like", "+", "-", "*", "/", "\\",
			"Mod", "**", "&", "|", "^", "~", "And", "Or", "Proc", "Iff",
			".", ".", "Null", "True", "False", "Date", "GenUniqueId()", "GenGuid()", "Guid {..}", "Is Not"
		};
	}
}
