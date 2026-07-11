using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeTypeParameter : CodeObject
	{
		public CodeTypeParameter()
		{
		}

		public CodeTypeParameter(string name)
		{
			this._name = name;
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

		public CodeTypeReferenceCollection Constraints
		{
			get
			{
				CodeTypeReferenceCollection codeTypeReferenceCollection;
				if ((codeTypeReferenceCollection = this._constraints) == null)
				{
					codeTypeReferenceCollection = (this._constraints = new CodeTypeReferenceCollection());
				}
				return codeTypeReferenceCollection;
			}
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
		}

		public bool HasConstructorConstraint { get; set; }

		private string _name;

		private CodeAttributeDeclarationCollection _customAttributes;

		private CodeTypeReferenceCollection _constraints;
	}
}
