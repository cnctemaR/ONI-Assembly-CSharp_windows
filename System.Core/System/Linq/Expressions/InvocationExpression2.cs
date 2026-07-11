using System;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal sealed class InvocationExpression2 : InvocationExpression
	{
		public InvocationExpression2(Expression lambda, Type returnType, Expression arg0, Expression arg1)
			: base(lambda, returnType)
		{
			this._arg0 = arg0;
			this._arg1 = arg1;
		}

		internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
		{
			return ExpressionUtils.ReturnReadOnly(this, ref this._arg0);
		}

		public override Expression GetArgument(int index)
		{
			if (index == 0)
			{
				return ExpressionUtils.ReturnObject<Expression>(this._arg0);
			}
			if (index != 1)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this._arg1;
		}

		public override int ArgumentCount
		{
			get
			{
				return 2;
			}
		}

		internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
		{
			if (arguments != null)
			{
				return Expression.Invoke(lambda, arguments[0], arguments[1]);
			}
			return Expression.Invoke(lambda, ExpressionUtils.ReturnObject<Expression>(this._arg0), this._arg1);
		}

		private object _arg0;

		private readonly Expression _arg1;
	}
}
