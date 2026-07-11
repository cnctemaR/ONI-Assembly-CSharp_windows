using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeDelegateInvokeExpression : CodeExpression
	{
		public CodeDelegateInvokeExpression()
		{
		}

		public CodeDelegateInvokeExpression(CodeExpression targetObject)
		{
			this.TargetObject = targetObject;
		}

		public CodeDelegateInvokeExpression(CodeExpression targetObject, params CodeExpression[] parameters)
		{
			this.TargetObject = targetObject;
			this.Parameters.AddRange(parameters);
		}

		public CodeExpression TargetObject { get; set; }

		public CodeExpressionCollection Parameters { get; } = new CodeExpressionCollection();
	}
}
