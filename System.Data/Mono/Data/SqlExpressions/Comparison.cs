using System;
using System.Data;
using System.Globalization;
using System.Threading;

namespace Mono.Data.SqlExpressions
{
	internal class Comparison : BinaryOpExpression
	{
		public Comparison(Operation op, IExpression e1, IExpression e2)
			: base(op, e1, e2)
		{
		}

		public override object Eval(DataRow row)
		{
			return this.EvalBoolean(row);
		}

		public override bool EvalBoolean(DataRow row)
		{
			IComparable comparable = this.expr1.Eval(row) as IComparable;
			IComparable comparable2 = this.expr2.Eval(row) as IComparable;
			if (comparable == null || comparable2 == null)
			{
				if (comparable == null && comparable2 == null)
				{
					return this.op == Operation.EQ;
				}
				return this.op == Operation.NE;
			}
			else
			{
				int num = Comparison.Compare(comparable, comparable2, row.Table.CaseSensitive);
				if (num < 0)
				{
					return this.op == Operation.NE || this.op == Operation.LE || this.op == Operation.LT;
				}
				if (num > 0)
				{
					return this.op == Operation.NE || this.op == Operation.GE || this.op == Operation.GT;
				}
				return this.op == Operation.EQ || this.op == Operation.LE || this.op == Operation.GE;
			}
		}

		internal static int Compare(IComparable o1, IComparable o2, bool caseSensitive)
		{
			try
			{
				if (o1 is string && Numeric.IsNumeric(o2))
				{
					o1 = (IComparable)Convert.ChangeType(o1, o2.GetType());
				}
				else if (o2 is string && Numeric.IsNumeric(o1))
				{
					o2 = (IComparable)Convert.ChangeType(o2, o1.GetType());
				}
				else if (o1 is string && o2 is Guid)
				{
					o2 = o2.ToString();
				}
				else if (o2 is string && o1 is Guid)
				{
					o1 = o1.ToString();
				}
			}
			catch (FormatException)
			{
				throw new EvaluateException(string.Format("Cannot perform compare operation on {0} and {1}.", o1.GetType(), o2.GetType()));
			}
			if (o1 is string && o2 is string)
			{
				o1 = ((string)o1).TrimEnd(Comparison.IgnoredTrailingChars);
				o2 = ((string)o2).TrimEnd(Comparison.IgnoredTrailingChars);
				if (!caseSensitive)
				{
					o1 = ((string)o1).ToLower();
					o2 = ((string)o2).ToLower();
				}
			}
			if (o1 is DateTime && o2 is string && Thread.CurrentThread.CurrentCulture != CultureInfo.InvariantCulture)
			{
				o2 = DateTime.Parse((string)o2, CultureInfo.InvariantCulture);
			}
			else if (o2 is DateTime && o1 is string && Thread.CurrentThread.CurrentCulture != CultureInfo.InvariantCulture)
			{
				o1 = DateTime.Parse((string)o1, CultureInfo.InvariantCulture);
			}
			else if (o2 is DateTime && o1 is string && Thread.CurrentThread.CurrentCulture != CultureInfo.InvariantCulture)
			{
				o1 = DateTime.Parse((string)o1, CultureInfo.InvariantCulture);
			}
			if (o1.GetType() != o2.GetType())
			{
				o2 = (IComparable)Convert.ChangeType(o2, o1.GetType());
			}
			return o1.CompareTo(o2);
		}

		private static readonly char[] IgnoredTrailingChars = new char[] { ' ', '\u3000', '\ufeff' };
	}
}
