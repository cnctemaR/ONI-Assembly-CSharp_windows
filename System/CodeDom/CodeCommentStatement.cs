using System;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeCommentStatement : CodeStatement
	{
		public CodeCommentStatement()
		{
		}

		public CodeCommentStatement(CodeComment comment)
		{
			this.comment = comment;
		}

		public CodeCommentStatement(string text)
		{
			this.comment = new CodeComment(text);
		}

		public CodeCommentStatement(string text, bool docComment)
		{
			this.comment = new CodeComment(text, docComment);
		}

		public CodeComment Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = value;
			}
		}

		internal override void Accept(ICodeDomVisitor visitor)
		{
			visitor.Visit(this);
		}

		private CodeComment comment;
	}
}
