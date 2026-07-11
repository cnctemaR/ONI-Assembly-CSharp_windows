using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	internal sealed class Block3 : BlockExpression
	{
		internal Block3(Expression arg0, Expression arg1, Expression arg2)
		{
			this._arg0 = arg0;
			this._arg1 = arg1;
			this._arg2 = arg2;
		}

		internal override bool SameExpressions(ICollection<Expression> expressions)
		{
			if (expressions.Count == 3)
			{
				ReadOnlyCollection<Expression> readOnlyCollection = this._arg0 as ReadOnlyCollection<Expression>;
				if (readOnlyCollection != null)
				{
					return ExpressionUtils.SameElements<Expression>(expressions, readOnlyCollection);
				}
				using (IEnumerator<Expression> enumerator = expressions.GetEnumerator())
				{
					enumerator.MoveNext();
					if (enumerator.Current == this._arg0)
					{
						enumerator.MoveNext();
						if (enumerator.Current == this._arg1)
						{
							enumerator.MoveNext();
							return enumerator.Current == this._arg2;
						}
					}
				}
				return false;
			}
			return false;
		}

		internal override Expression GetExpression(int index)
		{
			switch (index)
			{
			case 0:
				return ExpressionUtils.ReturnObject<Expression>(this._arg0);
			case 1:
				return this._arg1;
			case 2:
				return this._arg2;
			default:
				throw Error.ArgumentOutOfRange("index");
			}
		}

		internal override int ExpressionCount
		{
			get
			{
				return 3;
			}
		}

		internal override ReadOnlyCollection<Expression> GetOrMakeExpressions()
		{
			return BlockExpression.ReturnReadOnlyExpressions(this, ref this._arg0);
		}

		internal override BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
		{
			return new Block3(args[0], args[1], args[2]);
		}

		private object _arg0;

		private readonly Expression _arg1;

		private readonly Expression _arg2;
	}
}
