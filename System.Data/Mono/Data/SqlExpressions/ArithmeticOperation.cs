using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class ArithmeticOperation : BinaryOpExpression
	{
		public ArithmeticOperation(Operation op, IExpression e1, IExpression e2)
			: base(op, e1, e2)
		{
		}

		public override object Eval(DataRow row)
		{
			object obj = this.expr1.Eval(row);
			if (obj == DBNull.Value || obj == null)
			{
				return obj;
			}
			object obj2 = this.expr2.Eval(row);
			if (obj2 == DBNull.Value || obj2 == null)
			{
				return obj2;
			}
			if (this.op == Operation.ADD && (obj is string || obj2 is string))
			{
				return obj.ToString() + obj2.ToString();
			}
			IConvertible convertible = (IConvertible)obj;
			IConvertible convertible2 = (IConvertible)obj2;
			switch (this.op)
			{
			case Operation.ADD:
				return Numeric.Add(convertible, convertible2);
			case Operation.SUB:
				return Numeric.Subtract(convertible, convertible2);
			case Operation.MUL:
				return Numeric.Multiply(convertible, convertible2);
			case Operation.DIV:
				return Numeric.Divide(convertible, convertible2);
			case Operation.MOD:
				return Numeric.Modulo(convertible, convertible2);
			default:
				return 0;
			}
		}
	}
}
