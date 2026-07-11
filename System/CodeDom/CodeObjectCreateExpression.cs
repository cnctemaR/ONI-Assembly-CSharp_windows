using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeObjectCreateExpression : CodeExpression
	{
		public CodeObjectCreateExpression()
		{
		}

		public CodeObjectCreateExpression(CodeTypeReference createType, params CodeExpression[] parameters)
		{
			this.createType = createType;
			this.Parameters.AddRange(parameters);
		}

		public CodeObjectCreateExpression(string createType, params CodeExpression[] parameters)
		{
			this.createType = new CodeTypeReference(createType);
			this.Parameters.AddRange(parameters);
		}

		public CodeObjectCreateExpression(Type createType, params CodeExpression[] parameters)
		{
			this.createType = new CodeTypeReference(createType);
			this.Parameters.AddRange(parameters);
		}

		public CodeTypeReference CreateType
		{
			get
			{
				if (this.createType == null)
				{
					this.createType = new CodeTypeReference(string.Empty);
				}
				return this.createType;
			}
			set
			{
				this.createType = value;
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

		private CodeTypeReference createType;

		private CodeExpressionCollection parameters;
	}
}
