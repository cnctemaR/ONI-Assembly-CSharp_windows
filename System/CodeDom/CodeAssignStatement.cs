using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeAssignStatement : CodeStatement
	{
		public CodeAssignStatement()
		{
		}

		public CodeAssignStatement(CodeExpression left, CodeExpression right)
		{
			this.Left = left;
			this.Right = right;
		}

		public CodeExpression Left { get; set; }

		public CodeExpression Right { get; set; }
	}
}
