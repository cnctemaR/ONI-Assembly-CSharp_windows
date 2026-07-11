using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal sealed class Expression2<TDelegate> : Expression<TDelegate>
	{
		public Expression2(Expression body, ParameterExpression par0, ParameterExpression par1)
			: base(body)
		{
			this._par0 = par0;
			this._par1 = par1;
		}

		internal override int ParameterCount
		{
			get
			{
				return 2;
			}
		}

		internal override ParameterExpression GetParameter(int index)
		{
			if (index == 0)
			{
				return ExpressionUtils.ReturnObject<ParameterExpression>(this._par0);
			}
			if (index != 1)
			{
				throw Error.ArgumentOutOfRange("index");
			}
			return this._par1;
		}

		internal override bool SameParameters(ICollection<ParameterExpression> parameters)
		{
			if (parameters != null && parameters.Count == 2)
			{
				ReadOnlyCollection<ParameterExpression> readOnlyCollection = this._par0 as ReadOnlyCollection<ParameterExpression>;
				if (readOnlyCollection != null)
				{
					return ExpressionUtils.SameElements<ParameterExpression>(parameters, readOnlyCollection);
				}
				using (IEnumerator<ParameterExpression> enumerator = parameters.GetEnumerator())
				{
					enumerator.MoveNext();
					if (enumerator.Current == this._par0)
					{
						enumerator.MoveNext();
						return enumerator.Current == this._par1;
					}
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
			return Expression.Lambda<TDelegate>(body, new ParameterExpression[]
			{
				ExpressionUtils.ReturnObject<ParameterExpression>(this._par0),
				this._par1
			});
		}

		private object _par0;

		private readonly ParameterExpression _par1;
	}
}
