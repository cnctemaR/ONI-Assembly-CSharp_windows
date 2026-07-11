using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeAssignStatement : CodeStatement
	{
		public CodeAssignStatement()
		{
		}

		public CodeAssignStatement(CodeExpression left, CodeExpression right)
		{
			this.left = left;
			this.right = right;
		}

		public CodeExpression Left
		{
			get
			{
				return this.left;
			}
			set
			{
				this.left = value;
			}
		}

		public CodeExpression Right
		{
			get
			{
				return this.right;
			}
			set
			{
				this.right = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeExpression left;

		private CodeExpression right;
	}
}
