using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeMethodReferenceExpression : CodeExpression
	{
		public CodeMethodReferenceExpression()
		{
		}

		public CodeMethodReferenceExpression(CodeExpression targetObject, string methodName)
		{
			this.targetObject = targetObject;
			this.methodName = methodName;
		}

		public CodeMethodReferenceExpression(CodeExpression targetObject, string methodName, params CodeTypeReference[] typeParameters)
			: this(targetObject, methodName)
		{
			if (typeParameters != null && typeParameters.Length > 0)
			{
				this.TypeArguments.AddRange(typeParameters);
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

		[ComVisible(false)]
		public CodeTypeReferenceCollection TypeArguments
		{
			get
			{
				if (this.typeArguments == null)
				{
					this.typeArguments = new CodeTypeReferenceCollection();
				}
				return this.typeArguments;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private string methodName;

		private CodeExpression targetObject;

		private CodeTypeReferenceCollection typeArguments;
	}
}
