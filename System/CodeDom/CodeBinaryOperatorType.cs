using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[Serializable]
	public enum CodeBinaryOperatorType
	{
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
		Assign,
		IdentityInequality,
		IdentityEquality,
		ValueEquality,
		BitwiseOr,
		BitwiseAnd,
		BooleanOr,
		BooleanAnd,
		LessThan,
		LessThanOrEqual,
		GreaterThan,
		GreaterThanOrEqual
	}
}
