using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeConditionStatement : CodeStatement
	{
		public CodeConditionStatement()
		{
		}

		public CodeConditionStatement(CodeExpression condition, params CodeStatement[] trueStatements)
		{
			this.Condition = condition;
			this.TrueStatements.AddRange(trueStatements);
		}

		public CodeConditionStatement(CodeExpression condition, CodeStatement[] trueStatements, CodeStatement[] falseStatements)
		{
			this.Condition = condition;
			this.TrueStatements.AddRange(trueStatements);
			this.FalseStatements.AddRange(falseStatements);
		}

		public CodeExpression Condition { get; set; }

		public CodeStatementCollection TrueStatements { get; } = new CodeStatementCollection();

		public CodeStatementCollection FalseStatements { get; } = new CodeStatementCollection();
	}
}
