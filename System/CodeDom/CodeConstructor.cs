using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeConstructor : CodeMemberMethod
	{
		public CodeConstructor()
		{
			base.Name = ".ctor";
		}

		public CodeExpressionCollection BaseConstructorArgs
		{
			get
			{
				if (this.baseConstructorArgs == null)
				{
					this.baseConstructorArgs = new CodeExpressionCollection();
				}
				return this.baseConstructorArgs;
			}
		}

		public CodeExpressionCollection ChainedConstructorArgs
		{
			get
			{
				if (this.chainedConstructorArgs == null)
				{
					this.chainedConstructorArgs = new CodeExpressionCollection();
				}
				return this.chainedConstructorArgs;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeExpressionCollection baseConstructorArgs;

		private CodeExpressionCollection chainedConstructorArgs;
	}
}
