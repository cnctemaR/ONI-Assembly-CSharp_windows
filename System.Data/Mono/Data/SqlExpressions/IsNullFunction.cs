using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class IsNullFunction : UnaryExpression
	{
		public IsNullFunction(IExpression e, IExpression defaultExpr)
			: base(e)
		{
			this.defaultExpr = defaultExpr;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is UnaryExpression))
			{
				return false;
			}
			IsNullFunction isNullFunction = (IsNullFunction)obj;
			return isNullFunction.defaultExpr.Equals(this.defaultExpr);
		}

		public override int GetHashCode()
		{
			return this.defaultExpr.GetHashCode() ^ base.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			object obj = this.expr.Eval(row);
			if (obj == null || obj == DBNull.Value)
			{
				return this.defaultExpr.Eval(row);
			}
			return obj;
		}

		private IExpression defaultExpr;
	}
}
