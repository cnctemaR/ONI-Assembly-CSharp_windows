using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal class BlockN : BlockExpression
	{
		internal BlockN(IReadOnlyList<Expression> expressions)
		{
			this._expressions = expressions;
		}

		internal override bool SameExpressions(ICollection<Expression> expressions)
		{
			return ExpressionUtils.SameElements<Expression>(expressions, this._expressions);
		}

		internal override Expression GetExpression(int index)
		{
			return this._expressions[index];
		}

		internal override int ExpressionCount
		{
			get
			{
				return this._expressions.Count;
			}
		}

		internal override ReadOnlyCollection<Expression> GetOrMakeExpressions()
		{
			return ExpressionUtils.ReturnReadOnly<Expression>(ref this._expressions);
		}

		internal override BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
		{
			return new BlockN(args);
		}

		private IReadOnlyList<Expression> _expressions;
	}
}
