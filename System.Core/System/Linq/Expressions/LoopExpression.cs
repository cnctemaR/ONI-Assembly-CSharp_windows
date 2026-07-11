using System;
using System.Diagnostics;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.LoopExpressionProxy))]
	public sealed class LoopExpression : Expression
	{
		internal LoopExpression(Expression body, LabelTarget @break, LabelTarget @continue)
		{
			this.Body = body;
			this.BreakLabel = @break;
			this.ContinueLabel = @continue;
		}

		public sealed override Type Type
		{
			get
			{
				if (this.BreakLabel != null)
				{
					return this.BreakLabel.Type;
				}
				return typeof(void);
			}
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Loop;
			}
		}

		public Expression Body { get; }

		public LabelTarget BreakLabel { get; }

		public LabelTarget ContinueLabel { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitLoop(this);
		}

		public LoopExpression Update(LabelTarget breakLabel, LabelTarget continueLabel, Expression body)
		{
			if (breakLabel == this.BreakLabel && continueLabel == this.ContinueLabel && body == this.Body)
			{
				return this;
			}
			return Expression.Loop(body, breakLabel, continueLabel);
		}

		internal LoopExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
