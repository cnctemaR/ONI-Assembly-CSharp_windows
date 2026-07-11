using System;
using System.Diagnostics;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.GotoExpressionProxy))]
	public sealed class GotoExpression : Expression
	{
		internal GotoExpression(GotoExpressionKind kind, LabelTarget target, Expression value, Type type)
		{
			this.Kind = kind;
			this.Value = value;
			this.Target = target;
			this.Type = type;
		}

		public sealed override Type Type { get; }

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Goto;
			}
		}

		public Expression Value { get; }

		public LabelTarget Target { get; }

		public GotoExpressionKind Kind { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitGoto(this);
		}

		public GotoExpression Update(LabelTarget target, Expression value)
		{
			if (target == this.Target && value == this.Value)
			{
				return this;
			}
			return Expression.MakeGoto(this.Kind, target, value, this.Type);
		}

		internal GotoExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
