using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeDirectionExpression : CodeExpression
	{
		public CodeDirectionExpression()
		{
		}

		public CodeDirectionExpression(FieldDirection direction, CodeExpression expression)
		{
			this.direction = direction;
			this.expression = expression;
		}

		public FieldDirection Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
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

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private FieldDirection direction;

		private CodeExpression expression;
	}
}
