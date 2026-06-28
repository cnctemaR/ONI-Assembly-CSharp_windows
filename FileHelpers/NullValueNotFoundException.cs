using System;

namespace FileHelpers
{
	[Serializable]
	public class NullValueNotFoundException : BadUsageException
	{
		protected internal NullValueNotFoundException(string message)
			: base(message)
		{
		}

		protected internal NullValueNotFoundException(int line, int column, string message)
			: base(line, column, message)
		{
		}

		internal NullValueNotFoundException(LineInfo line, string message)
			: base(line, message)
		{
		}
	}
}
