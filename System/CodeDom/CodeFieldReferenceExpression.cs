using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeFieldReferenceExpression : CodeExpression
	{
		public CodeFieldReferenceExpression()
		{
			this.fieldName = string.Empty;
		}

		public CodeFieldReferenceExpression(CodeExpression targetObject, string fieldName)
		{
			this.targetObject = targetObject;
			this.fieldName = fieldName;
		}

		public string FieldName
		{
			get
			{
				return this.fieldName;
			}
			set
			{
				this.fieldName = value;
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

		private CodeExpression targetObject;

		private string fieldName;
	}
}
