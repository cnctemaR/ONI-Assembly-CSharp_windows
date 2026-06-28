using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal abstract class UnaryExpression : BaseExpression
	{
		public UnaryExpression(IExpression e)
		{
			this.expr = e;
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
			UnaryExpression unaryExpression = (UnaryExpression)obj;
			return unaryExpression.expr.Equals(this.expr);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.expr.GetHashCode();
		}

		public override bool DependsOn(DataColumn other)
		{
			return this.expr.DependsOn(other);
		}

		public override bool EvalBoolean(DataRow row)
		{
			return (bool)this.Eval(row);
		}

		protected IExpression expr;
	}
}
