using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal sealed class Expression0<TDelegate> : Expression<TDelegate>
	{
		public Expression0(Expression body)
			: base(body)
		{
		}

		internal override int ParameterCount
		{
			get
			{
				return 0;
			}
		}

		internal override bool SameParameters(ICollection<ParameterExpression> parameters)
		{
			return parameters == null || parameters.Count == 0;
		}

		internal override ParameterExpression GetParameter(int index)
		{
			throw Error.ArgumentOutOfRange("index");
		}

		internal override ReadOnlyCollection<ParameterExpression> GetOrMakeParameters()
		{
			return EmptyReadOnlyCollection<ParameterExpression>.Instance;
		}

		internal override Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
		{
			return Expression.Lambda<TDelegate>(body, parameters);
		}
	}
}
