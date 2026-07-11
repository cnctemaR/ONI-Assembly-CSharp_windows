using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeSnippetStatement : CodeStatement
	{
		public CodeSnippetStatement()
		{
		}

		public CodeSnippetStatement(string value)
		{
			this.Value = value;
		}

		public string Value
		{
			get
			{
				return this._value ?? string.Empty;
			}
			set
			{
				this._value = value;
			}
		}

		private string _value;
	}
}
