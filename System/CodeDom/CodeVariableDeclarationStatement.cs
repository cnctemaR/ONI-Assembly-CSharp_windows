using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeVariableDeclarationStatement : CodeStatement
	{
		public CodeVariableDeclarationStatement()
		{
		}

		public CodeVariableDeclarationStatement(CodeTypeReference type, string name)
		{
			this.type = type;
			this.name = name;
		}

		public CodeVariableDeclarationStatement(string type, string name)
		{
			this.type = new CodeTypeReference(type);
			this.name = name;
		}

		public CodeVariableDeclarationStatement(Type type, string name)
		{
			this.type = new CodeTypeReference(type);
			this.name = name;
		}

		public CodeVariableDeclarationStatement(CodeTypeReference type, string name, CodeExpression initExpression)
		{
			this.type = type;
			this.name = name;
			this.initExpression = initExpression;
		}

		public CodeVariableDeclarationStatement(string type, string name, CodeExpression initExpression)
		{
			this.type = new CodeTypeReference(type);
			this.name = name;
			this.initExpression = initExpression;
		}

		public CodeVariableDeclarationStatement(Type type, string name, CodeExpression initExpression)
		{
			this.type = new CodeTypeReference(type);
			this.name = name;
			this.initExpression = initExpression;
		}

		public CodeExpression InitExpression
		{
			get
			{
				return this.initExpression;
			}
			set
			{
				this.initExpression = value;
			}
		}

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					return string.Empty;
				}
				return this.name;
			}
			set
			{
				this.name = value;
			}
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

		private CodeExpression initExpression;

		private CodeTypeReference type;

		private string name;
	}
}
