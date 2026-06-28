using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeArrayCreateExpression : CodeExpression
	{
		public CodeArrayCreateExpression()
		{
		}

		public CodeArrayCreateExpression(CodeTypeReference createType, CodeExpression size)
		{
			this.createType = createType;
			this.sizeExpression = size;
		}

		public CodeArrayCreateExpression(CodeTypeReference createType, params CodeExpression[] initializers)
		{
			this.createType = createType;
			this.Initializers.AddRange(initializers);
		}

		public CodeArrayCreateExpression(CodeTypeReference createType, int size)
		{
			this.createType = createType;
			this.size = size;
		}

		public CodeArrayCreateExpression(string createType, CodeExpression size)
		{
			this.createType = new CodeTypeReference(createType);
			this.sizeExpression = size;
		}

		public CodeArrayCreateExpression(string createType, params CodeExpression[] initializers)
		{
			this.createType = new CodeTypeReference(createType);
			this.Initializers.AddRange(initializers);
		}

		public CodeArrayCreateExpression(string createType, int size)
		{
			this.createType = new CodeTypeReference(createType);
			this.size = size;
		}

		public CodeArrayCreateExpression(Type createType, CodeExpression size)
		{
			this.createType = new CodeTypeReference(createType);
			this.sizeExpression = size;
		}

		public CodeArrayCreateExpression(Type createType, params CodeExpression[] initializers)
		{
			this.createType = new CodeTypeReference(createType);
			this.Initializers.AddRange(initializers);
		}

		public CodeArrayCreateExpression(Type createType, int size)
		{
			this.createType = new CodeTypeReference(createType);
			this.size = size;
		}

		public CodeTypeReference CreateType
		{
			get
			{
				if (this.createType == null)
				{
					this.createType = new CodeTypeReference(typeof(void));
				}
				return this.createType;
			}
			set
			{
				this.createType = value;
			}
		}

		public CodeExpressionCollection Initializers
		{
			get
			{
				if (this.initializers == null)
				{
					this.initializers = new CodeExpressionCollection();
				}
				return this.initializers;
			}
		}

		public CodeExpression SizeExpression
		{
			get
			{
				return this.sizeExpression;
			}
			set
			{
				this.sizeExpression = value;
			}
		}

		public int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeTypeReference createType;

		private CodeExpressionCollection initializers;

		private CodeExpression sizeExpression;

		private int size;
	}
}
