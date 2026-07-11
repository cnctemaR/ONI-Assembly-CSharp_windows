using System;

namespace System.Linq.Expressions
{
	internal class SimpleBinaryExpression : BinaryExpression
	{
		internal SimpleBinaryExpression(ExpressionType nodeType, Expression left, Expression right, Type type)
			: base(left, right)
		{
			this.NodeType = nodeType;
			this.Type = type;
		}

		public sealed override ExpressionType NodeType { get; }

		public sealed override Type Type { get; }
	}
}
