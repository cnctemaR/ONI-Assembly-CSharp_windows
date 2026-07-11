using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal sealed class Scope1 : ScopeExpression
	{
		internal Scope1(IReadOnlyList<ParameterExpression> variables, Expression body)
			: this(variables, body)
		{
		}

		private Scope1(IReadOnlyList<ParameterExpression> variables, object body)
			: base(variables)
		{
			this._body = body;
		}

		internal override bool SameExpressions(ICollection<Expression> expressions)
		{
			if (expressions.Count == 1)
			{
				ReadOnlyCollection<Expression> readOnlyCollection = this._body as ReadOnlyCollection<Expression>;
				if (readOnlyCollection != null)
				{
					return ExpressionUtils.SameElements<Expression>(expressions, readOnlyCollection);
				}
				using (IEnumerator<Expression> enumerator = expressions.GetEnumerator())
				{
					enumerator.MoveNext();
					return ExpressionUtils.ReturnObject<Expression>(this._body) == enumerator.Current;
				}
				return false;
			}
			return false;
		}

		internal override Expression GetExpression(int index)
		{
			if (index == 0)
			{
				return ExpressionUtils.ReturnObject<Expression>(this._body);
			}
			throw Error.ArgumentOutOfRange("index");
		}

		internal override int ExpressionCount
		{
			get
			{
				return 1;
			}
		}

		internal override ReadOnlyCollection<Expression> GetOrMakeExpressions()
		{
			return BlockExpression.ReturnReadOnlyExpressions(this, ref this._body);
		}

		internal override BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
		{
			if (args == null)
			{
				Expression.ValidateVariables(variables, "variables");
				return new Scope1(variables, this._body);
			}
			return new Scope1(base.ReuseOrValidateVariables(variables), args[0]);
		}

		private object _body;
	}
}
