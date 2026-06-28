using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeTypeOfExpression : CodeExpression
	{
		public CodeTypeOfExpression()
		{
		}

		public CodeTypeOfExpression(CodeTypeReference type)
		{
			this.type = type;
		}

		public CodeTypeOfExpression(string type)
		{
			this.type = new CodeTypeReference(type);
		}

		public CodeTypeOfExpression(Type type)
		{
			this.type = new CodeTypeReference(type);
		}

		public CodeTypeReference Type
		{
			get
			{
				if (this.type == null)
				{
					this.type = new CodeTypeReference(string.Empty);
				}
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeTypeReference type;
	}
}
