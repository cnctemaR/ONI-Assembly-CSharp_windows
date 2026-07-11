using System;
using System.Collections.Generic;

namespace System.Linq.Expressions
{
	internal sealed class FullExpression<TDelegate> : ExpressionN<TDelegate>
	{
		public FullExpression(Expression body, string name, bool tailCall, IReadOnlyList<ParameterExpression> parameters)
			: base(body, parameters)
		{
			this.NameCore = name;
			this.TailCallCore = tailCall;
		}

		internal override string NameCore { get; }

		internal override bool TailCallCore { get; }
	}
}
