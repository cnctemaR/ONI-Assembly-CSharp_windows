using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeComment : CodeObject
	{
		public CodeComment()
		{
		}

		public CodeComment(string text)
		{
			this.Text = text;
		}

		public CodeComment(string text, bool docComment)
		{
			this.Text = text;
			this.DocComment = docComment;
		}

		public bool DocComment { get; set; }

		public string Text
		{
			get
			{
				return this._text ?? string.Empty;
			}
			set
			{
				this._text = value;
			}
		}

		private string _text;
	}
}
