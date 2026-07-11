using System;

namespace System.ComponentModel
{
	public enum MaskedTextResultHint
	{
		PositionOutOfRange = -55,
		NonEditPosition,
		UnavailableEditPosition,
		PromptCharNotAllowed,
		InvalidInput,
		SignedDigitExpected = -5,
		LetterExpected,
		DigitExpected,
		AlphanumericCharacterExpected,
		AsciiCharacterExpected,
		Unknown,
		CharacterEscaped,
		NoEffect,
		SideEffect,
		Success
	}
}
