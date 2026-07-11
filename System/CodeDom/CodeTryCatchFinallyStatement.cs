using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
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

		public CodeStatementCollection FinallyStatements
		{
			get
			{
				if (this.finallyStatements == null)
				{
					this.finallyStatements = new CodeStatementCollection();
				}
				return this.finallyStatements;
			}
		}

		public CodeStatementCollection TryStatements
		{
			get
			{
				if (this.tryStatements == null)
				{
					this.tryStatements = new CodeStatementCollection();
				}
				return this.tryStatements;
			}
		}

		public CodeCatchClauseCollection CatchClauses
		{
			get
			{
				if (this.catchClauses == null)
				{
					this.catchClauses = new CodeCatchClauseCollection();
				}
				return this.catchClauses;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeStatementCollection tryStatements;

		private CodeStatementCollection finallyStatements;

		private CodeCatchClauseCollection catchClauses;
	}
}
