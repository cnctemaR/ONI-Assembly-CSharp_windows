using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal class ScopeN : ScopeExpression
	{
		internal ScopeN(IReadOnlyList<ParameterExpression> variables, IReadOnlyList<Expression> body)
			: base(variables)
		{
			this._body = body;
		}

		internal override bool SameExpressions(ICollection<Expression> expressions)
		{
			return ExpressionUtils.SameElements<Expression>(expressions, this._body);
		}

		protected IReadOnlyList<Expression> Body
		{
			get
			{
				return this._body;
			}
		}

		internal override Expression GetExpression(int index)
		{
			return this._body[index];
		}

		internal override int ExpressionCount
		{
			get
			{
				return this._body.Count;
			}
		}

		internal override ReadOnlyCollection<Expression> GetOrMakeExpressions()
		{
			return ExpressionUtils.ReturnReadOnly<Expression>(ref this._body);
		}

		internal override BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
		{
			if (args == null)
			{
				Expression.ValidateVariables(variables, "variables");
				return new ScopeN(variables, this._body);
			}
			return new ScopeN(base.ReuseOrValidateVariables(variables), args);
		}

		private IReadOnlyList<Expression> _body;
	}
}
