using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeSnippetCompileUnit : CodeCompileUnit
	{
		public CodeSnippetCompileUnit()
		{
		}

		public CodeSnippetCompileUnit(string value)
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

		public CodeLinePragma LinePragma { get; set; }

		private string _value;
	}
}
