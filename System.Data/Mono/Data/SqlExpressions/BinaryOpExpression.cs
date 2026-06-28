using System;

namespace Mono.Data.SqlExpressions
{
	internal abstract class BinaryOpExpression : BinaryExpression
	{
		protected BinaryOpExpression(Operation op, IExpression e1, IExpression e2)
			: base(e1, e2)
		{
			this.op = op;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is BinaryOpExpression))
			{
				return false;
			}
			BinaryOpExpression binaryOpExpression = (BinaryOpExpression)obj;
			return binaryOpExpression.op == this.op;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.op.GetHashCode();
		}

		protected Operation op;
	}
}
