using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeThrowExceptionStatement : CodeStatement
	{
		public CodeThrowExceptionStatement()
		{
		}

		public CodeThrowExceptionStatement(CodeExpression toThrow)
		{
			this.ToThrow = toThrow;
		}

		public CodeExpression ToThrow { get; set; }
	}
}
