using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq
{
	public class EnumerableQuery<T> : EnumerableQuery, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>, IEnumerable, IQueryable, IOrderedQueryable, IQueryProvider
	{
		IQueryProvider IQueryable.Provider
		{
			get
			{
				return this;
			}
		}

		public EnumerableQuery(IEnumerable<T> enumerable)
		{
			this._enumerable = enumerable;
			this._expression = Expression.Constant(this);
		}

		public EnumerableQuery(Expression expression)
		{
			this._expression = expression;
		}

		internal override Expression Expression
		{
			get
			{
				return this._expression;
			}
		}

		internal override IEnumerable Enumerable
		{
			get
			{
				return this._enumerable;
			}
		}

		Expression IQueryable.Expression
		{
			get
			{
				return this._expression;
			}
		}

		Type IQueryable.ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		IQueryable IQueryProvider.CreateQuery(Expression expression)
		{
			if (expression == null)
			{
				throw Error.ArgumentNull("expression");
			}
			Type type = TypeHelper.FindGenericType(typeof(IQueryable<>), expression.Type);
			if (type == null)
			{
				throw Error.ArgumentNotValid("expression");
			}
			return EnumerableQuery.Create(type.GetGenericArguments()[0], expression);
		}

		IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
		{
			if (expression == null)
			{
				throw Error.ArgumentNull("expression");
			}
			if (!typeof(IQueryable<TElement>).IsAssignableFrom(expression.Type))
			{
				throw Error.ArgumentNotValid("expression");
			}
			return new EnumerableQuery<TElement>(expression);
		}

		object IQueryProvider.Execute(Expression expression)
		{
			if (expression == null)
			{
				throw Error.ArgumentNull("expression");
			}
			return EnumerableExecutor.Create(expression).ExecuteBoxed();
		}

		TElement IQueryProvider.Execute<TElement>(Expression expression)
		{
			if (expression == null)
			{
				throw Error.ArgumentNull("expression");
			}
			if (!typeof(TElement).IsAssignableFrom(expression.Type))
			{
				throw Error.ArgumentNotValid("expression");
			}
			return new EnumerableExecutor<TElement>(expression).Execute();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private IEnumerator<T> GetEnumerator()
		{
			if (this._enumerable == null)
			{
				IEnumerable<T> enumerable = Expression.Lambda<Func<IEnumerable<T>>>(new EnumerableRewriter().Visit(this._expression), null).Compile()();
				if (enumerable == this)
				{
					throw Error.EnumeratingNullEnumerableExpression();
				}
				this._enumerable = enumerable;
			}
			return this._enumerable.GetEnumerator();
		}

		public override string ToString()
		{
			ConstantExpression constantExpression = this._expression as ConstantExpression;
			if (constantExpression == null || constantExpression.Value != this)
			{
				return this._expression.ToString();
			}
			if (this._enumerable != null)
			{
				return this._enumerable.ToString();
			}
			return "null";
		}

		private readonly Expression _expression;

		private IEnumerable<T> _enumerable;
	}
}
