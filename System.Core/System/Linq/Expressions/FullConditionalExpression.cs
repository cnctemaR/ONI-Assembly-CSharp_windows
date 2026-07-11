using System;

namespace System.Linq.Expressions
{
	internal class FullConditionalExpression : ConditionalExpression
	{
		internal FullConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse)
			: base(test, ifTrue)
		{
			this._false = ifFalse;
		}

		internal override Expression GetFalse()
		{
			return this._false;
		}

		private readonly Expression _false;
	}
}
