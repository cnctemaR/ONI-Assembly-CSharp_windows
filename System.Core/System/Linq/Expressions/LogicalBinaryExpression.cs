using System;

namespace System.Linq.Expressions
{
	internal sealed class LogicalBinaryExpression : BinaryExpression
	{
		internal LogicalBinaryExpression(ExpressionType nodeType, Expression left, Expression right)
			: base(left, right)
		{
			this.NodeType = nodeType;
		}

		public sealed override Type Type
		{
			get
			{
				return typeof(bool);
			}
		}

		public sealed override ExpressionType NodeType { get; }
	}
}
