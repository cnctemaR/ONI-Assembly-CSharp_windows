using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeCommentStatement : CodeStatement
	{
		public CodeCommentStatement()
		{
		}

		public CodeCommentStatement(CodeComment comment)
		{
			this.Comment = comment;
		}

		public CodeCommentStatement(string text)
		{
			this.Comment = new CodeComment(text);
		}

		public CodeCommentStatement(string text, bool docComment)
		{
			this.Comment = new CodeComment(text, docComment);
		}

		public CodeComment Comment { get; set; }
	}
}
