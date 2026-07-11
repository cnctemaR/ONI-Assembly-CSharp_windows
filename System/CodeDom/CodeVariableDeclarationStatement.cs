using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeVariableDeclarationStatement : CodeStatement
	{
		public CodeVariableDeclarationStatement()
		{
		}

		public CodeVariableDeclarationStatement(CodeTypeReference type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public CodeVariableDeclarationStatement(string type, string name)
		{
			this.Type = new CodeTypeReference(type);
			this.Name = name;
		}

		public CodeVariableDeclarationStatement(Type type, string name)
		{
			this.Type = new CodeTypeReference(type);
			this.Name = name;
		}

		public CodeVariableDeclarationStatement(CodeTypeReference type, string name, CodeExpression initExpression)
		{
			this.Type = type;
			this.Name = name;
			this.InitExpression = initExpression;
		}

		public CodeVariableDeclarationStatement(string type, string name, CodeExpression initExpression)
		{
			this.Type = new CodeTypeReference(type);
			this.Name = name;
			this.InitExpression = initExpression;
		}

		public CodeVariableDeclarationStatement(Type type, string name, CodeExpression initExpression)
		{
			this.Type = new CodeTypeReference(type);
			this.Name = name;
			this.InitExpression = initExpression;
		}

		public CodeExpression InitExpression { get; set; }

		public string Name
		{
			get
			{
				return this._name ?? string.Empty;
			}
			set
			{
				this._name = value;
			}
		}

		public CodeTypeReference Type
		{
			get
			{
				CodeTypeReference codeTypeReference;
				if ((codeTypeReference = this._type) == null)
				{
					codeTypeReference = (this._type = new CodeTypeReference(""));
				}
				return codeTypeReference;
			}
			set
			{
				this._type = value;
			}
		}

		private CodeTypeReference _type;

		private string _name;
	}
}
