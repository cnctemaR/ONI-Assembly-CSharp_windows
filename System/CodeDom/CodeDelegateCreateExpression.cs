using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeDelegateCreateExpression : CodeExpression
	{
		public CodeDelegateCreateExpression()
		{
		}

		public CodeDelegateCreateExpression(CodeTypeReference delegateType, CodeExpression targetObject, string methodName)
		{
			this._delegateType = delegateType;
			this.TargetObject = targetObject;
			this._methodName = methodName;
		}

		public CodeTypeReference DelegateType
		{
			get
			{
				CodeTypeReference codeTypeReference;
				if ((codeTypeReference = this._delegateType) == null)
				{
					codeTypeReference = (this._delegateType = new CodeTypeReference(""));
				}
				return codeTypeReference;
			}
			set
			{
				this._delegateType = value;
			}
		}

		public CodeExpression TargetObject { get; set; }

		public string MethodName
		{
			get
			{
				return this._methodName ?? string.Empty;
			}
			set
			{
				this._methodName = value;
			}
		}

		private CodeTypeReference _delegateType;

		private string _methodName;
	}
}
