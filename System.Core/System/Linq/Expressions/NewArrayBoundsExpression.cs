using System;
using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
	internal sealed class NewArrayBoundsExpression : NewArrayExpression
	{
		internal NewArrayBoundsExpression(Type type, ReadOnlyCollection<Expression> expressions)
			: base(type, expressions)
		{
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.NewArrayBounds;
			}
		}
	}
}
