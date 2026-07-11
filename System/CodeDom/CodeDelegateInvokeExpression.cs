using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeDelegateInvokeExpression : CodeExpression
	{
		public CodeDelegateInvokeExpression()
		{
		}

		public CodeDelegateInvokeExpression(CodeExpression targetObject)
		{
			this.targetObject = targetObject;
		}

		public CodeDelegateInvokeExpression(CodeExpression targetObject, params CodeExpression[] parameters)
		{
			this.targetObject = targetObject;
			this.Parameters.AddRange(parameters);
		}

		public CodeExpressionCollection Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = new CodeExpressionCollection();
				}
				return this.parameters;
			}
		}

		public CodeExpression TargetObject
		{
			get
			{
				return this.targetObject;
			}
			set
			{
				this.targetObject = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeExpressionCollection parameters;

		private CodeExpression targetObject;
	}
}
