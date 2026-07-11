using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeThrowExceptionStatement : CodeStatement
	{
		public CodeThrowExceptionStatement()
		{
		}

		public CodeThrowExceptionStatement(CodeExpression toThrow)
		{
			this.toThrow = toThrow;
		}

		public CodeExpression ToThrow
		{
			get
			{
				return this.toThrow;
			}
			set
			{
				this.toThrow = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeExpression toThrow;
	}
}
