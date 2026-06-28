using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class BoolOperation : BinaryOpExpression
	{
		public BoolOperation(Operation op, IExpression e1, IExpression e2)
			: base(op, e1, e2)
		{
		}

		public override object Eval(DataRow row)
		{
			return this.EvalBoolean(row);
		}

		public override bool EvalBoolean(DataRow row)
		{
			if (this.op == Operation.OR)
			{
				return this.expr1.EvalBoolean(row) || this.expr2.EvalBoolean(row);
			}
			return this.op == Operation.AND && this.expr1.EvalBoolean(row) && this.expr2.EvalBoolean(row);
		}
	}
}
