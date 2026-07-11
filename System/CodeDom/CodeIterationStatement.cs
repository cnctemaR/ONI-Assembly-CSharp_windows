using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeIterationStatement : CodeStatement
	{
		public CodeIterationStatement()
		{
		}

		public CodeIterationStatement(CodeStatement initStatement, CodeExpression testExpression, CodeStatement incrementStatement, params CodeStatement[] statements)
		{
			this.initStatement = initStatement;
			this.testExpression = testExpression;
			this.incrementStatement = incrementStatement;
			this.Statements.AddRange(statements);
		}

		public CodeStatement IncrementStatement
		{
			get
			{
				return this.incrementStatement;
			}
			set
			{
				this.incrementStatement = value;
			}
		}

		public CodeStatement InitStatement
		{
			get
			{
				return this.initStatement;
			}
			set
			{
				this.initStatement = value;
			}
		}

		public CodeStatementCollection Statements
		{
			get
			{
				if (this.statements == null)
				{
					this.statements = new CodeStatementCollection();
				}
				return this.statements;
			}
		}

		public CodeExpression TestExpression
		{
			get
			{
				return this.testExpression;
			}
			set
			{
				this.testExpression = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeStatement incrementStatement;

		private CodeStatement initStatement;

		private CodeStatementCollection statements;

		private CodeExpression testExpression;
	}
}
