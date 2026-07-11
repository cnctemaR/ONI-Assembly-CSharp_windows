using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal class ScopeExpression : BlockExpression
	{
		internal ScopeExpression(IReadOnlyList<ParameterExpression> variables)
		{
			this._variables = variables;
		}

		internal override bool SameVariables(ICollection<ParameterExpression> variables)
		{
			return ExpressionUtils.SameElements<ParameterExpression>(variables, this._variables);
		}

		internal override ReadOnlyCollection<ParameterExpression> GetOrMakeVariables()
		{
			return ExpressionUtils.ReturnReadOnly<ParameterExpression>(ref this._variables);
		}

		protected IReadOnlyList<ParameterExpression> VariablesList
		{
			get
			{
				return this._variables;
			}
		}

		internal IReadOnlyList<ParameterExpression> ReuseOrValidateVariables(ReadOnlyCollection<ParameterExpression> variables)
		{
			if (variables != null && variables != this.VariablesList)
			{
				Expression.ValidateVariables(variables, "variables");
				return variables;
			}
			return this.VariablesList;
		}

		private IReadOnlyList<ParameterExpression> _variables;
	}
}
