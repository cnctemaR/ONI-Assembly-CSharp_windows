using System;
using System.Linq.Expressions;

namespace System.Linq
{
	public class EnumerableExecutor<T> : EnumerableExecutor
	{
		public EnumerableExecutor(Expression expression)
		{
			this._expression = expression;
		}

		internal override object ExecuteBoxed()
		{
			return this.Execute();
		}

		internal T Execute()
		{
			return Expression.Lambda<Func<T>>(new EnumerableRewriter().Visit(this._expression), null).Compile()();
		}

		private readonly Expression _expression;
	}
}
