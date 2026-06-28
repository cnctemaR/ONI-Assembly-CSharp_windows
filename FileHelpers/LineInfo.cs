using System;
using System.Diagnostics;
using System.Globalization;

namespace FileHelpers
{
	[DebuggerDisplay("{DebuggerDisplayStr()}")]
	internal sealed class LineInfo
	{
		public LineInfo(string line)
		{
			this.mReader = null;
			this.mLineStr = line;
			this.mCurrentPos = 0;
		}

		public string Substring(int from, int count)
		{
			return this.mLineStr.Substring(from, count);
		}

		private string DebuggerDisplayStr()
		{
			if (this.IsEOL())
			{
				return "<EOL>";
			}
			return this.CurrentString;
		}

		public string CurrentString
		{
			get
			{
				return this.mLineStr.Substring(this.mCurrentPos, this.mLineStr.Length - this.mCurrentPos);
			}
		}

		public bool IsEOL()
		{
			return this.mCurrentPos >= this.mLineStr.Length;
		}

		public int CurrentLength
		{
			get
			{
				return this.mLineStr.Length - this.mCurrentPos;
			}
		}

		public bool EmptyFromPos()
		{
			int length = this.mLineStr.Length;
			int num = this.mCurrentPos;
			while (num < length && Array.BinarySearch<char>(LineInfo.mWhitespaceChars, this.mLineStr[num]) >= 0)
			{
				num++;
			}
			return num >= length;
		}

		public void TrimStart()
		{
			this.TrimStartSorted(LineInfo.mWhitespaceChars);
		}

		public void TrimStart(char[] toTrim)
		{
			Array.Sort<char>(toTrim);
			this.TrimStartSorted(toTrim);
		}

		private void TrimStartSorted(char[] toTrim)
		{
			int length = this.mLineStr.Length;
			while (this.mCurrentPos < length && Array.BinarySearch<char>(toTrim, this.mLineStr[this.mCurrentPos]) >= 0)
			{
				this.mCurrentPos++;
			}
		}

		public bool StartsWith(string str)
		{
			return this.mCurrentPos < this.mLineStr.Length && LineInfo.mCompare.Compare(this.mLineStr, this.mCurrentPos, str.Length, str, 0, str.Length, CompareOptions.OrdinalIgnoreCase) == 0;
		}

		public bool StartsWithTrim(string str)
		{
			int length = this.mLineStr.Length;
			int num = this.mCurrentPos;
			while (num < length && Array.BinarySearch<char>(LineInfo.mWhitespaceChars, this.mLineStr[num]) >= 0)
			{
				num++;
			}
			return LineInfo.mCompare.Compare(this.mLineStr, num, str, 0, CompareOptions.OrdinalIgnoreCase) == 0;
		}

		public void ReadNextLine()
		{
			this.mLineStr = this.mReader.ReadNextLine();
			this.mCurrentPos = 0;
		}

		public int IndexOf(string foundThis)
		{
			return LineInfo.mCompare.IndexOf(this.mLineStr, foundThis, this.mCurrentPos, CompareOptions.Ordinal);
		}

		internal void ReLoad(string line)
		{
			this.mLineStr = line;
			this.mCurrentPos = 0;
		}

		internal string mLineStr;

		internal ForwardReader mReader;

		internal int mCurrentPos;

		private static readonly char[] mWhitespaceChars = new char[]
		{
			'\t', '\n', '\v', '\f', '\r', ' ', '\u00a0', '\u2000', '\u2001', '\u2002',
			'\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008', '\u2009', '\u200a', '\u200b', '\u3000',
			'\ufeff'
		};

		private static readonly CompareInfo mCompare = StringHelper.CreateComparer();
	}
}
