using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeLabeledStatement : CodeStatement
	{
		public CodeLabeledStatement()
		{
		}

		public CodeLabeledStatement(string label)
		{
			this._label = label;
		}

		public CodeLabeledStatement(string label, CodeStatement statement)
		{
			this._label = label;
			this.Statement = statement;
		}

		public string Label
		{
			get
			{
				return this._label ?? string.Empty;
			}
			set
			{
				this._label = value;
			}
		}

		public CodeStatement Statement { get; set; }

		private string _label;
	}
}
