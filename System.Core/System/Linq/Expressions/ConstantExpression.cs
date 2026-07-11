using System;
using System.Diagnostics;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.ConstantExpressionProxy))]
	public class ConstantExpression : Expression
	{
		internal ConstantExpression(object value)
		{
			this.Value = value;
		}

		public override Type Type
		{
			get
			{
				if (this.Value == null)
				{
					return typeof(object);
				}
				return this.Value.GetType();
			}
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Constant;
			}
		}

		public object Value { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitConstant(this);
		}
	}
}
