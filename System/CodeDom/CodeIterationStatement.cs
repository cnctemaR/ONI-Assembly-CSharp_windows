using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeIterationStatement : CodeStatement
	{
		public CodeIterationStatement()
		{
		}

		public CodeIterationStatement(CodeStatement initStatement, CodeExpression testExpression, CodeStatement incrementStatement, params CodeStatement[] statements)
		{
			this.InitStatement = initStatement;
			this.TestExpression = testExpression;
			this.IncrementStatement = incrementStatement;
			this.Statements.AddRange(statements);
		}

		public CodeStatement InitStatement { get; set; }

		public CodeExpression TestExpression { get; set; }

		public CodeStatement IncrementStatement { get; set; }

		public CodeStatementCollection Statements { get; } = new CodeStatementCollection();
	}
}
