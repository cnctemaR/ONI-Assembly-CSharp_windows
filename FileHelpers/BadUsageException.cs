using System;

namespace FileHelpers
{
	[Serializable]
	public class BadUsageException : FileHelpersException
	{
		protected internal BadUsageException(string message)
			: base(message)
		{
		}

		protected internal BadUsageException(int line, int column, string message)
			: base(line, column, message)
		{
		}

		internal BadUsageException(LineInfo line, string message)
			: this(line.mReader.LineNumber, line.mCurrentPos, message)
		{
		}
	}
}
