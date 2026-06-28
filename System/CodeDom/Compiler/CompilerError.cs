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
			this.fileName = fileName;
			this.line = line;
			this.column = column;
			this.errorNumber = errorNumber;
			this.errorText = errorText;
		}

		public override string ToString()
		{
			string text = ((!this.isWarning) ? "error" : "warning");
			return string.Format(CultureInfo.InvariantCulture, "{0}({1},{2}) : {3} {4}: {5}", new object[] { this.fileName, this.line, this.column, text, this.errorNumber, this.errorText });
		}

		public int Line
		{
			get
			{
				return this.line;
			}
			set
			{
				this.line = value;
			}
		}

		public int Column
		{
			get
			{
				return this.column;
			}
			set
			{
				this.column = value;
			}
		}

		public string ErrorNumber
		{
			get
			{
				return this.errorNumber;
			}
			set
			{
				this.errorNumber = value;
			}
		}

		public string ErrorText
		{
			get
			{
				return this.errorText;
			}
			set
			{
				this.errorText = value;
			}
		}

		public bool IsWarning
		{
			get
			{
				return this.isWarning;
			}
			set
			{
				this.isWarning = value;
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		private string fileName;

		private int line;

		private int column;

		private string errorNumber;

		private string errorText;

		private bool isWarning;
	}
}
