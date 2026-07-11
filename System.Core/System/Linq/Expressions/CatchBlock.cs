using System;
using System.Diagnostics;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.CatchBlockProxy))]
	public sealed class CatchBlock
	{
		internal CatchBlock(Type test, ParameterExpression variable, Expression body, Expression filter)
		{
			this.Test = test;
			this.Variable = variable;
			this.Body = body;
			this.Filter = filter;
		}

		public ParameterExpression Variable { get; }

		public Type Test { get; }

		public Expression Body { get; }

		public Expression Filter { get; }

		public override string ToString()
		{
			return ExpressionStringBuilder.CatchBlockToString(this);
		}

		public CatchBlock Update(ParameterExpression variable, Expression filter, Expression body)
		{
			if (variable == this.Variable && filter == this.Filter && body == this.Body)
			{
				return this;
			}
			return Expression.MakeCatchBlock(this.Test, variable, body, filter);
		}
	}
}
