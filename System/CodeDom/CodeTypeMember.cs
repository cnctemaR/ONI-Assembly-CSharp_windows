using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeTypeMember : CodeObject
	{
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

		public MemberAttributes Attributes { get; set; } = (MemberAttributes)20482;

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

		public CodeLinePragma LinePragma { get; set; }

		public CodeCommentStatementCollection Comments { get; } = new CodeCommentStatementCollection();

		public CodeDirectiveCollection StartDirectives
		{
			get
			{
				CodeDirectiveCollection codeDirectiveCollection;
				if ((codeDirectiveCollection = this._startDirectives) == null)
				{
					codeDirectiveCollection = (this._startDirectives = new CodeDirectiveCollection());
				}
				return codeDirectiveCollection;
			}
		}

		public CodeDirectiveCollection EndDirectives
		{
			get
			{
				CodeDirectiveCollection codeDirectiveCollection;
				if ((codeDirectiveCollection = this._endDirectives) == null)
				{
					codeDirectiveCollection = (this._endDirectives = new CodeDirectiveCollection());
				}
				return codeDirectiveCollection;
			}
		}

		private string _name;

		private CodeAttributeDeclarationCollection _customAttributes;

		private CodeDirectiveCollection _startDirectives;

		private CodeDirectiveCollection _endDirectives;
	}
}
