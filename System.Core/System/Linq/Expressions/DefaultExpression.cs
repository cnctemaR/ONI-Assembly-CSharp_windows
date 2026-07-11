using System;
using System.Diagnostics;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.DefaultExpressionProxy))]
	public sealed class DefaultExpression : Expression
	{
		internal DefaultExpression(Type type)
		{
			this.Type = type;
		}

		public sealed override Type Type { get; }

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Default;
			}
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitDefault(this);
		}

		internal DefaultExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
