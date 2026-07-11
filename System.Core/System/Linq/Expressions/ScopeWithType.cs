using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
	internal sealed class ScopeWithType : ScopeN
	{
		internal ScopeWithType(IReadOnlyList<ParameterExpression> variables, IReadOnlyList<Expression> expressions, Type type)
			: base(variables, expressions)
		{
			this.Type = type;
		}

		public sealed override Type Type { get; }

		internal override BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
		{
			if (args == null)
			{
				Expression.ValidateVariables(variables, "variables");
				return new ScopeWithType(variables, base.Body, this.Type);
			}
			return new ScopeWithType(base.ReuseOrValidateVariables(variables), args, this.Type);
		}
	}
}
