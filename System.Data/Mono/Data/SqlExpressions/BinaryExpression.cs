using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal abstract class BinaryExpression : BaseExpression
	{
		protected BinaryExpression(IExpression e1, IExpression e2)
		{
			this.expr1 = e1;
			this.expr2 = e2;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is BinaryExpression))
			{
				return false;
			}
			BinaryExpression binaryExpression = (BinaryExpression)obj;
			return binaryExpression.expr1.Equals(this.expr1) && binaryExpression.expr2.Equals(this.expr2);
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			num ^= this.expr1.GetHashCode();
			return num ^ this.expr2.GetHashCode();
		}

		public override bool DependsOn(DataColumn other)
		{
			return this.expr1.DependsOn(other) || this.expr2.DependsOn(other);
		}

		public override void ResetExpression()
		{
			this.expr1.ResetExpression();
			this.expr2.ResetExpression();
		}

		protected IExpression expr1;

		protected IExpression expr2;
	}
}
