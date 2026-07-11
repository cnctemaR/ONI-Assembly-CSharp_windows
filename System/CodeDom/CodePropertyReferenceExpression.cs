using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodePropertyReferenceExpression : CodeExpression
	{
		public CodePropertyReferenceExpression()
		{
		}

		public CodePropertyReferenceExpression(CodeExpression targetObject, string propertyName)
		{
			this.targetObject = targetObject;
			this.propertyName = propertyName;
		}

		public string PropertyName
		{
			get
			{
				if (this.propertyName == null)
				{
					return string.Empty;
				}
				return this.propertyName;
			}
			set
			{
				this.propertyName = value;
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

		private string propertyName;
	}
}
