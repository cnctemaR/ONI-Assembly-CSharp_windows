using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.ListInitExpressionProxy))]
	public sealed class ListInitExpression : Expression
	{
		internal ListInitExpression(NewExpression newExpression, ReadOnlyCollection<ElementInit> initializers)
		{
			this.NewExpression = newExpression;
			this.Initializers = initializers;
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.ListInit;
			}
		}

		public sealed override Type Type
		{
			get
			{
				return this.NewExpression.Type;
			}
		}

		public override bool CanReduce
		{
			get
			{
				return true;
			}
		}

		public NewExpression NewExpression { get; }

		public ReadOnlyCollection<ElementInit> Initializers { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitListInit(this);
		}

		public override Expression Reduce()
		{
			return MemberInitExpression.ReduceListInit(this.NewExpression, this.Initializers, true);
		}

		public ListInitExpression Update(NewExpression newExpression, IEnumerable<ElementInit> initializers)
		{
			if (((newExpression == this.NewExpression) & (initializers != null)) && ExpressionUtils.SameElements<ElementInit>(ref initializers, this.Initializers))
			{
				return this;
			}
			return Expression.ListInit(newExpression, initializers);
		}

		internal ListInitExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
