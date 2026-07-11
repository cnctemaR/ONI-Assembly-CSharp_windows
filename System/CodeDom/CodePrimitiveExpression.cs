using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodePrimitiveExpression : CodeExpression
	{
		public CodePrimitiveExpression()
		{
		}

		public CodePrimitiveExpression(object value)
		{
			this.value = value;
		}

		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private object value;
	}
}
