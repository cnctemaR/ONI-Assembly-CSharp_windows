using System;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal sealed class InvocationExpression0 : InvocationExpression
	{
		public InvocationExpression0(Expression lambda, Type returnType)
			: base(lambda, returnType)
		{
		}

		internal override ReadOnlyCollection<Expression> GetOrMakeArguments()
		{
			return EmptyReadOnlyCollection<Expression>.Instance;
		}

		public override Expression GetArgument(int index)
		{
			throw new ArgumentOutOfRangeException("index");
		}

		public override int ArgumentCount
		{
			get
			{
				return 0;
			}
		}

		internal override InvocationExpression Rewrite(Expression lambda, Expression[] arguments)
		{
			return Expression.Invoke(lambda);
		}
	}
}
