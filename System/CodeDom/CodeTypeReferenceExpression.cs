using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeTypeReferenceExpression : CodeExpression
	{
		public CodeTypeReferenceExpression()
		{
		}

		public CodeTypeReferenceExpression(CodeTypeReference type)
		{
			this.type = type;
		}

		public CodeTypeReferenceExpression(string type)
		{
			this.type = new CodeTypeReference(type);
		}

		public CodeTypeReferenceExpression(Type type)
		{
			this.type = new CodeTypeReference(type);
		}

		public CodeTypeReference Type
		{
			get
			{
				if (this.type == null)
				{
					return new CodeTypeReference(string.Empty);
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
