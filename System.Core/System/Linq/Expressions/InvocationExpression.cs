using System;
using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
	public sealed class InvocationExpression : Expression
	{
		internal InvocationExpression(Expression expression, Type type, ReadOnlyCollection<Expression> arguments)
			: base(ExpressionType.Invoke, type)
		{
			this.expression = expression;
			this.arguments = arguments;
		}

		public Expression Expression
		{
			get
			{
				return this.expression;
			}
		}

		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		internal override void Emit(EmitContext ec)
		{
			ec.EmitCall(this.expression, this.arguments, this.expression.Type.GetInvokeMethod());
		}

		private Expression expression;

		private ReadOnlyCollection<Expression> arguments;
	}
}
