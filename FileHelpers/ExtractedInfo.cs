using System;
using System.Diagnostics;

namespace FileHelpers
{
	[DebuggerDisplay("{ExtractedString()} [{ExtractedFrom}-{ExtractedTo}]")]
	internal struct ExtractedInfo
	{
		public string ExtractedString()
		{
			if (this.mCustomExtractedString == null)
			{
				return this.mLine.Substring(this.ExtractedFrom, this.ExtractedTo - this.ExtractedFrom + 1);
			}
			return this.mCustomExtractedString;
		}

		public int Length
		{
			get
			{
				return this.ExtractedTo - this.ExtractedFrom + 1;
			}
		}

		public ExtractedInfo(LineInfo line)
		{
			this.mLine = line;
			this.ExtractedFrom = line.mCurrentPos;
			this.ExtractedTo = line.mLineStr.Length - 1;
			this.mCustomExtractedString = null;
		}

		public ExtractedInfo(LineInfo line, int extractTo)
		{
			this.mLine = line;
			this.ExtractedFrom = line.mCurrentPos;
			this.ExtractedTo = extractTo - 1;
			this.mCustomExtractedString = null;
		}

		public ExtractedInfo(string customExtract)
		{
			this.mLine = null;
			this.ExtractedFrom = 0;
			this.ExtractedTo = 0;
			this.mCustomExtractedString = customExtract;
		}

		public bool HasOnlyThisChars(char[] sortedArray)
		{
			if (this.mCustomExtractedString != null)
			{
				for (int i = 0; i < this.mCustomExtractedString.Length; i++)
				{
					if (Array.BinarySearch<char>(sortedArray, this.mCustomExtractedString[i]) < 0)
					{
						return false;
					}
				}
				return true;
			}
			for (int j = this.ExtractedFrom; j <= this.ExtractedTo; j++)
			{
				if (Array.BinarySearch<char>(sortedArray, this.mLine.mLineStr[j]) < 0)
				{
					return false;
				}
			}
			return true;
		}

		internal string mCustomExtractedString;

		public LineInfo mLine;

		public int ExtractedFrom;

		public int ExtractedTo;

		internal static readonly ExtractedInfo Empty = new ExtractedInfo(string.Empty);
	}
}
