using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeMethodInvokeExpression : CodeExpression
	{
		public CodeMethodInvokeExpression()
		{
		}

		public CodeMethodInvokeExpression(CodeMethodReferenceExpression method, params CodeExpression[] parameters)
		{
			this.method = method;
			this.Parameters.AddRange(parameters);
		}

		public CodeMethodInvokeExpression(CodeExpression targetObject, string methodName, params CodeExpression[] parameters)
		{
			this.method = new CodeMethodReferenceExpression(targetObject, methodName);
			this.Parameters.AddRange(parameters);
		}

		public CodeMethodReferenceExpression Method
		{
			get
			{
				if (this.method == null)
				{
					this.method = new CodeMethodReferenceExpression();
				}
				return this.method;
			}
			set
			{
				this.method = value;
			}
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

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeMethodReferenceExpression method;

		private CodeExpressionCollection parameters;
	}
}
