using System;
using System.Diagnostics;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.LabelExpressionProxy))]
	public sealed class LabelExpression : Expression
	{
		internal LabelExpression(LabelTarget label, Expression defaultValue)
		{
			this.Target = label;
			this.DefaultValue = defaultValue;
		}

		public sealed override Type Type
		{
			get
			{
				return this.Target.Type;
			}
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Label;
			}
		}

		public LabelTarget Target { get; }

		public Expression DefaultValue { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitLabel(this);
		}

		public LabelExpression Update(LabelTarget target, Expression defaultValue)
		{
			if (target == this.Target && defaultValue == this.DefaultValue)
			{
				return this;
			}
			return Expression.Label(target, defaultValue);
		}
	}
}
