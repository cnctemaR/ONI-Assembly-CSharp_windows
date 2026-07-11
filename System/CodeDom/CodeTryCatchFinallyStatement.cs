using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeTryCatchFinallyStatement : CodeStatement
	{
		public CodeTryCatchFinallyStatement()
		{
		}

		public CodeTryCatchFinallyStatement(CodeStatement[] tryStatements, CodeCatchClause[] catchClauses)
		{
			this.TryStatements.AddRange(tryStatements);
			this.CatchClauses.AddRange(catchClauses);
		}

		public CodeTryCatchFinallyStatement(CodeStatement[] tryStatements, CodeCatchClause[] catchClauses, CodeStatement[] finallyStatements)
		{
			this.TryStatements.AddRange(tryStatements);
			this.CatchClauses.AddRange(catchClauses);
			this.FinallyStatements.AddRange(finallyStatements);
		}

		public CodeStatementCollection TryStatements { get; } = new CodeStatementCollection();

		public CodeCatchClauseCollection CatchClauses { get; } = new CodeCatchClauseCollection();

		public CodeStatementCollection FinallyStatements { get; } = new CodeStatementCollection();
	}
}
