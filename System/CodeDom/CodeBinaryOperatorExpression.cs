using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeBinaryOperatorExpression : CodeExpression
	{
		public CodeBinaryOperatorExpression()
		{
		}

		public CodeBinaryOperatorExpression(CodeExpression left, CodeBinaryOperatorType op, CodeExpression right)
		{
			this.Right = right;
			this.Operator = op;
			this.Left = left;
		}

		public CodeExpression Right { get; set; }

		public CodeExpression Left { get; set; }

		public CodeBinaryOperatorType Operator { get; set; }
	}
}
