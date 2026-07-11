using System;

namespace System.Linq.Expressions
{
	internal class ByRefAssignBinaryExpression : AssignBinaryExpression
	{
		internal ByRefAssignBinaryExpression(Expression left, Expression right)
			: base(left, right)
		{
		}

		internal override bool IsByRef
		{
			get
			{
				return true;
			}
		}
	}
}
