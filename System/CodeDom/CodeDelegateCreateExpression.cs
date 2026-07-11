using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeDelegateCreateExpression : CodeExpression
	{
		public CodeDelegateCreateExpression()
		{
		}

		public CodeDelegateCreateExpression(CodeTypeReference delegateType, CodeExpression targetObject, string methodName)
		{
			this.delegateType = delegateType;
			this.targetObject = targetObject;
			this.methodName = methodName;
		}

		public CodeTypeReference DelegateType
		{
			get
			{
				if (this.delegateType == null)
				{
					this.delegateType = new CodeTypeReference(string.Empty);
				}
				return this.delegateType;
			}
			set
			{
				this.delegateType = value;
			}
		}

		public string MethodName
		{
			get
			{
				if (this.methodName == null)
				{
					return string.Empty;
				}
				return this.methodName;
			}
			set
			{
				this.methodName = value;
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

		private CodeTypeReference delegateType;

		private string methodName;

		private CodeExpression targetObject;
	}
}
