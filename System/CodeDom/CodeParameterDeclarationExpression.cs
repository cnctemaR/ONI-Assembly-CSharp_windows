using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeParameterDeclarationExpression : CodeExpression
	{
		public CodeParameterDeclarationExpression()
		{
		}

		public CodeParameterDeclarationExpression(CodeTypeReference type, string name)
		{
			this.Type = type;
			this.Name = name;
		}

		public CodeParameterDeclarationExpression(string type, string name)
		{
			this.Type = new CodeTypeReference(type);
			this.Name = name;
		}

		public CodeParameterDeclarationExpression(Type type, string name)
		{
			this.Type = new CodeTypeReference(type);
			this.Name = name;
		}

		public CodeAttributeDeclarationCollection CustomAttributes
		{
			get
			{
				CodeAttributeDeclarationCollection codeAttributeDeclarationCollection;
				if ((codeAttributeDeclarationCollection = this._customAttributes) == null)
				{
					codeAttributeDeclarationCollection = (this._customAttributes = new CodeAttributeDeclarationCollection());
				}
				return codeAttributeDeclarationCollection;
			}
			set
			{
				this._customAttributes = value;
			}
		}

		public FieldDirection Direction { get; set; }

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

		private CodeTypeReference _type;

		private string _name;

		private CodeAttributeDeclarationCollection _customAttributes;
	}
}
