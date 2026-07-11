using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeSnippetExpression : CodeExpression
	{
		public CodeSnippetExpression()
		{
		}

		public CodeSnippetExpression(string value)
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
