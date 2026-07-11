using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.NewArrayExpressionProxy))]
	public class NewArrayExpression : Expression
	{
		internal NewArrayExpression(Type type, ReadOnlyCollection<Expression> expressions)
		{
			this.Expressions = expressions;
			this.Type = type;
		}

		internal static NewArrayExpression Make(ExpressionType nodeType, Type type, ReadOnlyCollection<Expression> expressions)
		{
			if (nodeType == ExpressionType.NewArrayInit)
			{
				return new NewArrayInitExpression(type, expressions);
			}
			return new NewArrayBoundsExpression(type, expressions);
		}

		public sealed override Type Type { get; }

		public ReadOnlyCollection<Expression> Expressions { get; }

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitNewArray(this);
		}

		public NewArrayExpression Update(IEnumerable<Expression> expressions)
		{
			ContractUtils.RequiresNotNull(expressions, "expressions");
			if (ExpressionUtils.SameElements<Expression>(ref expressions, this.Expressions))
			{
				return this;
			}
			if (this.NodeType != ExpressionType.NewArrayInit)
			{
				return Expression.NewArrayBounds(this.Type.GetElementType(), expressions);
			}
			return Expression.NewArrayInit(this.Type.GetElementType(), expressions);
		}
	}
}
