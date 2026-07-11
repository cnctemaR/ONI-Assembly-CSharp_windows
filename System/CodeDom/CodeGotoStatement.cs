using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeGotoStatement : CodeStatement
	{
		public CodeGotoStatement()
		{
		}

		public CodeGotoStatement(string label)
		{
			this.Label = label;
		}

		public string Label
		{
			get
			{
				return this._label;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("value");
				}
				this._label = value;
			}
		}

		private string _label;
	}
}
