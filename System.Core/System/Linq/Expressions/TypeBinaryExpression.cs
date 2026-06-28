using System;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class TypeBinaryExpression : Expression
	{
		internal TypeBinaryExpression(ExpressionType node_type, Expression expression, Type type_operand, Type type)
			: base(node_type, type)
		{
			this.expression = expression;
			this.type_operand = type_operand;
		}

		public Expression Expression
		{
			get
			{
				return this.expression;
			}
		}

		public Type TypeOperand
		{
			get
			{
				return this.type_operand;
			}
		}

		internal override void Emit(EmitContext ec)
		{
			if (this.expression.Type == typeof(void))
			{
				ec.ig.Emit(OpCodes.Ldc_I4_0);
				return;
			}
			ec.EmitIsInst(this.expression, this.type_operand);
			ec.ig.Emit(OpCodes.Ldnull);
			ec.ig.Emit(OpCodes.Cgt_Un);
		}

		private Expression expression;

		private Type type_operand;
	}
}
