using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeMethodReferenceExpression : CodeExpression
	{
		public CodeMethodReferenceExpression()
		{
		}

		public CodeMethodReferenceExpression(CodeExpression targetObject, string methodName)
		{
			this.TargetObject = targetObject;
			this.MethodName = methodName;
		}

		public CodeMethodReferenceExpression(CodeExpression targetObject, string methodName, params CodeTypeReference[] typeParameters)
		{
			this.TargetObject = targetObject;
			this.MethodName = methodName;
			if (typeParameters != null && typeParameters.Length != 0)
			{
				this.TypeArguments.AddRange(typeParameters);
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

		public CodeTypeReferenceCollection TypeArguments
		{
			get
			{
				CodeTypeReferenceCollection codeTypeReferenceCollection;
				if ((codeTypeReferenceCollection = this._typeArguments) == null)
				{
					codeTypeReferenceCollection = (this._typeArguments = new CodeTypeReferenceCollection());
				}
				return codeTypeReferenceCollection;
			}
		}

		private string _methodName;

		private CodeTypeReferenceCollection _typeArguments;
	}
}
