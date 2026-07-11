using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal sealed class Expression1<TDelegate> : Expression<TDelegate>
	{
		public Expression1(Expression body, ParameterExpression par0)
			: base(body)
		{
			this._par0 = par0;
		}

		internal override int ParameterCount
		{
			get
			{
				return 1;
			}
		}

		internal override ParameterExpression GetParameter(int index)
		{
			if (index == 0)
			{
				return ExpressionUtils.ReturnObject<ParameterExpression>(this._par0);
			}
			throw Error.ArgumentOutOfRange("index");
		}

		internal override bool SameParameters(ICollection<ParameterExpression> parameters)
		{
			if (parameters != null && parameters.Count == 1)
			{
				using (IEnumerator<ParameterExpression> enumerator = parameters.GetEnumerator())
				{
					enumerator.MoveNext();
					return enumerator.Current == ExpressionUtils.ReturnObject<ParameterExpression>(this._par0);
				}
				return false;
			}
			return false;
		}

		internal override ReadOnlyCollection<ParameterExpression> GetOrMakeParameters()
		{
			return ExpressionUtils.ReturnReadOnly(this, ref this._par0);
		}

		internal override Expression<TDelegate> Rewrite(Expression body, ParameterExpression[] parameters)
		{
			if (parameters != null)
			{
				return Expression.Lambda<TDelegate>(body, parameters);
			}
			return Expression.Lambda<TDelegate>(body, new ParameterExpression[] { ExpressionUtils.ReturnObject<ParameterExpression>(this._par0) });
		}

		private object _par0;
	}
}
