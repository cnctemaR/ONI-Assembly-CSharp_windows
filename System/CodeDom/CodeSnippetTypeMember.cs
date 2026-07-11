using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeSnippetTypeMember : CodeTypeMember
	{
		public CodeSnippetTypeMember()
		{
		}

		public CodeSnippetTypeMember(string text)
		{
			this.Text = text;
		}

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
