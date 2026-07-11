using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.IndexExpressionProxy))]
	public sealed class IndexExpression : Expression, IArgumentProvider
	{
		internal IndexExpression(Expression instance, PropertyInfo indexer, IReadOnlyList<Expression> arguments)
		{
			indexer == null;
			this.Object = instance;
			this.Indexer = indexer;
			this._arguments = arguments;
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Index;
			}
		}

		public sealed override Type Type
		{
			get
			{
				if (this.Indexer != null)
				{
					return this.Indexer.PropertyType;
				}
				return this.Object.Type.GetElementType();
			}
		}

		public Expression Object { get; }

		public PropertyInfo Indexer { get; }

		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return ExpressionUtils.ReturnReadOnly<Expression>(ref this._arguments);
			}
		}

		public IndexExpression Update(Expression @object, IEnumerable<Expression> arguments)
		{
			if (((@object == this.Object) & (arguments != null)) && ExpressionUtils.SameElements<Expression>(ref arguments, this.Arguments))
			{
				return this;
			}
			return Expression.MakeIndex(@object, this.Indexer, arguments);
		}

		public Expression GetArgument(int index)
		{
			return this._arguments[index];
		}

		public int ArgumentCount
		{
			get
			{
				return this._arguments.Count;
			}
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitIndex(this);
		}

		internal Expression Rewrite(Expression instance, Expression[] arguments)
		{
			return Expression.MakeIndex(instance, this.Indexer, arguments ?? this._arguments);
		}

		internal IndexExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private IReadOnlyList<Expression> _arguments;
	}
}
