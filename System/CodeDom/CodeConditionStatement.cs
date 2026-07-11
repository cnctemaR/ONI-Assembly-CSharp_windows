using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeConditionStatement : CodeStatement
	{
		public CodeConditionStatement()
		{
		}

		public CodeConditionStatement(CodeExpression condition, params CodeStatement[] trueStatements)
		{
			this.condition = condition;
			this.TrueStatements.AddRange(trueStatements);
		}

		public CodeConditionStatement(CodeExpression condition, CodeStatement[] trueStatements, CodeStatement[] falseStatements)
		{
			this.condition = condition;
			this.TrueStatements.AddRange(trueStatements);
			this.FalseStatements.AddRange(falseStatements);
		}

		public CodeExpression Condition
		{
			get
			{
				return this.condition;
			}
			set
			{
				this.condition = value;
			}
		}

		public CodeStatementCollection FalseStatements
		{
			get
			{
				if (this.falseStatements == null)
				{
					this.falseStatements = new CodeStatementCollection();
				}
				return this.falseStatements;
			}
		}

		public CodeStatementCollection TrueStatements
		{
			get
			{
				if (this.trueStatements == null)
				{
					this.trueStatements = new CodeStatementCollection();
				}
				return this.trueStatements;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeExpression condition;

		private CodeStatementCollection trueStatements;

		private CodeStatementCollection falseStatements;
	}
}
