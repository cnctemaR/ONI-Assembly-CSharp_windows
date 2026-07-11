using System;

namespace System.Linq.Expressions
{
	internal static class Utils
	{
		public static ConstantExpression Constant(bool value)
		{
			if (!value)
			{
				return Utils.s_false;
			}
			return Utils.s_true;
		}

		public static ConstantExpression Constant(int value)
		{
			switch (value)
			{
			case -1:
				return Utils.s_m1;
			case 0:
				return Utils.s_0;
			case 1:
				return Utils.s_1;
			case 2:
				return Utils.s_2;
			case 3:
				return Utils.s_3;
			default:
				return Expression.Constant(value);
			}
		}

		public static readonly object BoxedFalse = false;

		public static readonly object BoxedTrue = true;

		public static readonly object BoxedIntM1 = -1;

		public static readonly object BoxedInt0 = 0;

		public static readonly object BoxedInt1 = 1;

		public static readonly object BoxedInt2 = 2;

		public static readonly object BoxedInt3 = 3;

		public static readonly object BoxedDefaultSByte = 0;

		public static readonly object BoxedDefaultChar = '\0';

		public static readonly object BoxedDefaultInt16 = 0;

		public static readonly object BoxedDefaultInt64 = 0L;

		public static readonly object BoxedDefaultByte = 0;

		public static readonly object BoxedDefaultUInt16 = 0;

		public static readonly object BoxedDefaultUInt32 = 0U;

		public static readonly object BoxedDefaultUInt64 = 0UL;

		public static readonly object BoxedDefaultSingle = 0f;

		public static readonly object BoxedDefaultDouble = 0.0;

		public static readonly object BoxedDefaultDecimal = 0m;

		public static readonly object BoxedDefaultDateTime = default(DateTime);

		private static readonly ConstantExpression s_true = Expression.Constant(Utils.BoxedTrue);

		private static readonly ConstantExpression s_false = Expression.Constant(Utils.BoxedFalse);

		private static readonly ConstantExpression s_m1 = Expression.Constant(Utils.BoxedIntM1);

		private static readonly ConstantExpression s_0 = Expression.Constant(Utils.BoxedInt0);

		private static readonly ConstantExpression s_1 = Expression.Constant(Utils.BoxedInt1);

		private static readonly ConstantExpression s_2 = Expression.Constant(Utils.BoxedInt2);

		private static readonly ConstantExpression s_3 = Expression.Constant(Utils.BoxedInt3);

		public static readonly DefaultExpression Empty = Expression.Empty();

		public static readonly ConstantExpression Null = Expression.Constant(null);
	}
}
