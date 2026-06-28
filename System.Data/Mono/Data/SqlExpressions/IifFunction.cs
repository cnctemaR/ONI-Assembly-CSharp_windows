using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class IifFunction : UnaryExpression
	{
		public IifFunction(IExpression e, IExpression trueExpr, IExpression falseExpr)
			: base(e)
		{
			this.trueExpr = trueExpr;
			this.falseExpr = falseExpr;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is IifFunction))
			{
				return false;
			}
			IifFunction iifFunction = (IifFunction)obj;
			return iifFunction.falseExpr.Equals(this.falseExpr) && iifFunction.trueExpr.Equals(this.trueExpr);
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			num ^= this.falseExpr.GetHashCode();
			return num ^ this.trueExpr.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			object obj = this.expr.Eval(row);
			if (obj == DBNull.Value)
			{
				return obj;
			}
			bool flag = Convert.ToBoolean(obj);
			return (!flag) ? this.falseExpr.Eval(row) : this.trueExpr.Eval(row);
		}

		private IExpression trueExpr;

		private IExpression falseExpr;
	}
}
