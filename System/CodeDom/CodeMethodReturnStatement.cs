using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeMethodReturnStatement : CodeStatement
	{
		public CodeMethodReturnStatement()
		{
		}

		public CodeMethodReturnStatement(CodeExpression expression)
		{
			this.expression = expression;
		}

		public CodeExpression Expression
		{
			get
			{
				return this.expression;
			}
			set
			{
				this.expression = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeExpression expression;
	}
}
