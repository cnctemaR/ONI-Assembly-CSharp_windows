using System;
using System.Collections;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class In : UnaryExpression
	{
		public In(IExpression e, IList set)
			: base(e)
		{
			this.set = set;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is In))
			{
				return false;
			}
			In @in = (In)obj;
			if (@in.set.Count != this.set.Count)
			{
				return false;
			}
			int i = 0;
			int count = this.set.Count;
			while (i < count)
			{
				object obj2 = this.set[i];
				object obj3 = @in.set[i];
				if (obj2 == null && obj3 != null)
				{
					return false;
				}
				if (!obj2.Equals(obj3))
				{
					return false;
				}
				i++;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			int i = 0;
			int count = this.set.Count;
			while (i < count)
			{
				object obj = this.set[i];
				if (obj != null)
				{
					num ^= obj.GetHashCode();
				}
				i++;
			}
			return num;
		}

		public override object Eval(DataRow row)
		{
			object obj = this.expr.Eval(row);
			if (obj == DBNull.Value)
			{
				return obj;
			}
			IComparable comparable = obj as IComparable;
			if (comparable == null)
			{
				return false;
			}
			foreach (object obj2 in this.set)
			{
				IExpression expression = (IExpression)obj2;
				IComparable comparable2 = (IComparable)expression.Eval(row);
				if (comparable2 != null)
				{
					if (Comparison.Compare(comparable, comparable2, row.Table.CaseSensitive) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override bool EvalBoolean(DataRow row)
		{
			return (bool)this.Eval(row);
		}

		private IList set;
	}
}
