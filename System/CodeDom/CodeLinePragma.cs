using System;

namespace System.CodeDom
{
	[Serializable]
	public class CodeLinePragma
	{
		public CodeLinePragma()
		{
		}

		public CodeLinePragma(string fileName, int lineNumber)
		{
			this.FileName = fileName;
			this.LineNumber = lineNumber;
		}

		public string FileName
		{
			get
			{
				return this._fileName ?? string.Empty;
			}
			set
			{
				this._fileName = value;
			}
		}

		public int LineNumber { get; set; }

		private string _fileName;
	}
}
