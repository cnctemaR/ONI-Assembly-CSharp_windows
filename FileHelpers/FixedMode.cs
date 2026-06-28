using System;

namespace FileHelpers
{
	public enum FixedMode
	{
		ExactLength,
		AllowMoreChars,
		AllowLessChars,
		AllowVariableLength
	}
}
