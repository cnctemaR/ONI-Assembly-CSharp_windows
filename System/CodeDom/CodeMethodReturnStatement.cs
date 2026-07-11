using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeMethodReturnStatement : CodeStatement
	{
		public CodeMethodReturnStatement()
		{
		}

		public CodeMethodReturnStatement(CodeExpression expression)
		{
			this.Expression = expression;
		}

		public CodeExpression Expression { get; set; }
	}
}
