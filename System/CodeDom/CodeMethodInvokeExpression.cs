using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeMethodInvokeExpression : CodeExpression
	{
		public CodeMethodInvokeExpression()
		{
		}

		public CodeMethodInvokeExpression(CodeMethodReferenceExpression method, params CodeExpression[] parameters)
		{
			this._method = method;
			this.Parameters.AddRange(parameters);
		}

		public CodeMethodInvokeExpression(CodeExpression targetObject, string methodName, params CodeExpression[] parameters)
		{
			this._method = new CodeMethodReferenceExpression(targetObject, methodName);
			this.Parameters.AddRange(parameters);
		}

		public CodeMethodReferenceExpression Method
		{
			get
			{
				CodeMethodReferenceExpression codeMethodReferenceExpression;
				if ((codeMethodReferenceExpression = this._method) == null)
				{
					codeMethodReferenceExpression = (this._method = new CodeMethodReferenceExpression());
				}
				return codeMethodReferenceExpression;
			}
			set
			{
				this._method = value;
			}
		}

		public CodeExpressionCollection Parameters { get; } = new CodeExpressionCollection();

		private CodeMethodReferenceExpression _method;
	}
}
