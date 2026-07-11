using System;

namespace System.ComponentModel
{
	public enum MaskedTextResultHint
	{
		Unknown,
		CharacterEscaped,
		NoEffect,
		SideEffect,
		Success,
		AsciiCharacterExpected = -1,
		AlphanumericCharacterExpected = -2,
		DigitExpected = -3,
		LetterExpected = -4,
		SignedDigitExpected = -5,
		InvalidInput = -51,
		PromptCharNotAllowed = -52,
		UnavailableEditPosition = -53,
		NonEditPosition = -54,
		PositionOutOfRange = -55
	}
}
