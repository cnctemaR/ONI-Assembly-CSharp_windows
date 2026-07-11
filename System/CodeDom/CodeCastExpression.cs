using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeCastExpression : CodeExpression
	{
		public CodeCastExpression()
		{
		}

		public CodeCastExpression(CodeTypeReference targetType, CodeExpression expression)
		{
			this.targetType = targetType;
			this.expression = expression;
		}

		public CodeCastExpression(string targetType, CodeExpression expression)
		{
			this.targetType = new CodeTypeReference(targetType);
			this.expression = expression;
		}

		public CodeCastExpression(Type targetType, CodeExpression expression)
		{
			this.targetType = new CodeTypeReference(targetType);
			this.expression = expression;
		}

		public CodeExpression Expression
		{
			get
			{
				return this.expression;
			}
			set
			{
				this.expression = value;
			}
		}

		public CodeTypeReference TargetType
		{
			get
			{
				if (this.targetType == null)
				{
					this.targetType = new CodeTypeReference(string.Empty);
				}
				return this.targetType;
			}
			set
			{
				this.targetType = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeTypeReference targetType;

		private CodeExpression expression;
	}
}
