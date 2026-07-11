using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeExpressionStatement : CodeStatement
	{
		public CodeExpressionStatement()
		{
		}

		public CodeExpressionStatement(CodeExpression expression)
		{
			this.Expression = expression;
		}

		public CodeExpression Expression { get; set; }
	}
}
