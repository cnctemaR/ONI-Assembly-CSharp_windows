using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal sealed class InvocationExpressionN : InvocationExpression
	{
		public InvocationExpressionN(Expression lambda, IReadOnlyList<Expression> arguments, Type returnType)
			: base(lambda, returnType)
		{
			this._arguments = arguments;
		}

		internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
		{
			return ExpressionUtils.ReturnReadOnly<Expression>(ref this._arguments);
		}

		public override Expression GetArgument(int index)
		{
			return this._arguments[index];
		}

		public override int ArgumentCount
		{
			get
			{
				return this._arguments.Count;
			}
		}

		internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
		{
			return Expression.Invoke(lambda, arguments ?? this._arguments);
		}

		private IReadOnlyList<Expression> _arguments;
	}
}
