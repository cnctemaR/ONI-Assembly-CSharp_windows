using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace FileHelpers
{
	[DebuggerDisplay("{ErrorsDescription()}. ErrorMode: {ErrorMode.ToString()}")]
	public sealed class ErrorManager : IEnumerable
	{
		public ErrorManager()
		{
		}

		public ErrorManager(ErrorMode mode)
		{
			this.mErrorMode = mode;
		}

		public int ErrorLimit
		{
			get
			{
				return this.mErrorLimit;
			}
			set
			{
				this.mErrorLimit = value;
			}
		}

		private string ErrorsDescription()
		{
			if (this.ErrorCount == 1)
			{
				return this.ErrorCount.ToString() + " Error";
			}
			if (this.ErrorCount == 0)
			{
				return "No Errors";
			}
			return this.ErrorCount.ToString() + " Errors";
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public ErrorInfo[] Errors
		{
			get
			{
				return this.mErrorsArray.ToArray();
			}
		}

		public ErrorMode ErrorMode
		{
			get
			{
				return this.mErrorMode;
			}
			set
			{
				this.mErrorMode = value;
			}
		}

		public int ErrorCount
		{
			get
			{
				return this.mErrorsArray.Count;
			}
		}

		public bool HasErrors
		{
			get
			{
				return this.mErrorsArray.Count > 0;
			}
		}

		public void ClearErrors()
		{
			this.mErrorsArray.Clear();
		}

		internal void AddError(ErrorInfo error)
		{
			if (this.mErrorsArray.Count <= this.mErrorLimit)
			{
				this.mErrorsArray.Add(error);
			}
		}

		internal void AddErrors(ErrorManager errors)
		{
			if (this.mErrorsArray.Count <= this.mErrorLimit)
			{
				this.mErrorsArray.AddRange(errors.mErrorsArray);
			}
		}

		public void SaveErrors(string fileName)
		{
			string text;
			if (this.ErrorCount > 0)
			{
				text = "FileHelpers - Errors Saved ";
			}
			else
			{
				text = "FileHelpers - NO Errors Found ";
			}
			string text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"at ",
				DateTime.Now.ToLongDateString(),
				" ",
				DateTime.Now.ToLongTimeString()
			});
			text = text + StringHelper.NewLine + "LineNumber | LineString |ErrorDescription";
			this.SaveErrors(fileName, text);
		}

		public void SaveErrors(string fileName, string header)
		{
			FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(ErrorInfo));
			if (header.IndexOf(StringHelper.NewLine) == header.LastIndexOf(StringHelper.NewLine))
			{
				header += StringHelper.NewLine;
			}
			fileHelperEngine.HeaderText = header;
			fileHelperEngine.WriteFile(fileName, this.Errors);
		}

		public static ErrorInfo[] LoadErrors(string fileName)
		{
			FileHelperEngine fileHelperEngine = new FileHelperEngine(typeof(ErrorInfo));
			return (ErrorInfo[])fileHelperEngine.ReadFile(fileName);
		}

		public IEnumerator GetEnumerator()
		{
			return this.mErrorsArray.GetEnumerator();
		}

		private int mErrorLimit = 10000;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private List<ErrorInfo> mErrorsArray = new List<ErrorInfo>();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ErrorMode mErrorMode;
	}
}
