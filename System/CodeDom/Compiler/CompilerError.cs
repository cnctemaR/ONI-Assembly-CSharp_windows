using System;
using System.Globalization;

namespace System.CodeDom.Compiler
{
	[Serializable]
	public class CompilerError
	{
		public CompilerError()
			: this(string.Empty, 0, 0, string.Empty, string.Empty)
		{
		}

		public CompilerError(string fileName, int line, int column, string errorNumber, string errorText)
		{
			this.Line = line;
			this.Column = column;
			this.ErrorNumber = errorNumber;
			this.ErrorText = errorText;
			this.FileName = fileName;
		}

		public int Line { get; set; }

		public int Column { get; set; }

		public string ErrorNumber { get; set; }

		public string ErrorText { get; set; }

		public bool IsWarning { get; set; }

		public string FileName { get; set; }

		public override string ToString()
		{
			if (this.FileName.Length <= 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} {1}: {2}", this.WarningString, this.ErrorNumber, this.ErrorText);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}({1},{2}) : {3} {4}: {5}", new object[] { this.FileName, this.Line, this.Column, this.WarningString, this.ErrorNumber, this.ErrorText });
		}

		private string WarningString
		{
			get
			{
				if (!this.IsWarning)
				{
					return "error";
				}
				return "warning";
			}
		}
	}
}
