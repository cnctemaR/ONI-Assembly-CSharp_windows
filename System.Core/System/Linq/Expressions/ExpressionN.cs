using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal class ExpressionN<TDelegate> : Expression<TDelegate>
	{
		public ExpressionN(Expression body, IReadOnlyList<ParameterExpression> parameters)
			: base(body)
		{
			this._parameters = parameters;
		}

		internal override int ParameterCount
		{
			get
			{
				return this._parameters.Count;
			}
		}

		internal override ParameterExpression GetParameter(int index)
		{
			return this._parameters[index];
		}

		internal override bool SameParameters(ICollection<ParameterExpression> parameters)
		{
			return ExpressionUtils.SameElements<ParameterExpression>(parameters, this._parameters);
		}

		internal override ReadOnlyCollection<ParameterExpression> GetOrMakeParameters()
		{
			return ExpressionUtils.ReturnReadOnly<ParameterExpression>(ref this._parameters);
		}

		internal override Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
		{
			return Expression.Lambda<TDelegate>(body, base.Name, base.TailCall, parameters ?? this._parameters);
		}

		private IReadOnlyList<ParameterExpression> _parameters;
	}
}
