using System;

namespace System.Linq.Expressions
{
	internal sealed class FullConditionalExpressionWithType : FullConditionalExpression
	{
		internal FullConditionalExpressionWithType(Expression test, Expression ifTrue, Expression ifFalse, Type type)
			: base(test, ifTrue, ifFalse)
		{
			this.Type = type;
		}

		public sealed override Type Type { get; }
	}
}
