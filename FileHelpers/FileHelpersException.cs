using System;

namespace FileHelpers
{
	[Serializable]
	public class FileHelpersException : Exception
	{
		public FileHelpersException(string message)
			: base(message)
		{
		}

		public FileHelpersException(string message, Exception innerEx)
			: base(message, innerEx)
		{
		}

		public FileHelpersException(int line, int column, string message)
			: base(string.Concat(new string[]
			{
				"Line: ",
				line.ToString(),
				" Column: ",
				column.ToString(),
				". ",
				message
			}))
		{
		}
	}
}
