using System;

namespace System.Linq.Expressions
{
	internal class AssignBinaryExpression : BinaryExpression
	{
		internal AssignBinaryExpression(Expression left, Expression right)
			: base(left, right)
		{
		}

		public static AssignBinaryExpression Make(Expression left, Expression right, bool byRef)
		{
			if (byRef)
			{
				return new ByRefAssignBinaryExpression(left, right);
			}
			return new AssignBinaryExpression(left, right);
		}

		internal virtual bool IsByRef
		{
			get
			{
				return false;
			}
		}

		public sealed override Type Type
		{
			get
			{
				return base.Left.Type;
			}
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Assign;
			}
		}
	}
}
