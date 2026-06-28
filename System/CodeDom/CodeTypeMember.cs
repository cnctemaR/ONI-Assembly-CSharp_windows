using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeTypeMember : CodeObject
	{
		public CodeTypeMember()
		{
			this.attributes = (MemberAttributes)20482;
		}

		public MemberAttributes Attributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = value;
			}
		}

		public CodeCommentStatementCollection Comments
		{
			get
			{
				if (this.comments == null)
				{
					this.comments = new CodeCommentStatementCollection();
				}
				return this.comments;
			}
		}

		public CodeAttributeDeclarationCollection CustomAttributes
		{
			get
			{
				if (this.customAttributes == null)
				{
					this.customAttributes = new CodeAttributeDeclarationCollection();
				}
				return this.customAttributes;
			}
			set
			{
				this.customAttributes = value;
			}
		}

		public CodeLinePragma LinePragma
		{
			get
			{
				return this.linePragma;
			}
			set
			{
				this.linePragma = value;
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

		public CodeDirectiveCollection EndDirectives
		{
			get
			{
				if (this.endDirectives == null)
				{
					this.endDirectives = new CodeDirectiveCollection();
				}
				return this.endDirectives;
			}
		}

		public CodeDirectiveCollection StartDirectives
		{
			get
			{
				if (this.startDirectives == null)
				{
					this.startDirectives = new CodeDirectiveCollection();
				}
				return this.startDirectives;
			}
		}

		private string name;

		private MemberAttributes attributes;

		private CodeCommentStatementCollection comments;

		private CodeAttributeDeclarationCollection customAttributes;

		private CodeLinePragma linePragma;

		private CodeDirectiveCollection endDirectives;

		private CodeDirectiveCollection startDirectives;
	}
}
