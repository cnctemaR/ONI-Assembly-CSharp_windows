using System;

namespace UnityEngine
{
	internal enum TextEditOp
	{
		MoveLeft,
		MoveRight,
		MoveUp,
		MoveDown,
		MoveLineStart,
		MoveLineEnd,
		MoveTextStart,
		MoveTextEnd,
		MovePageUp,
		MovePageDown,
		MoveGraphicalLineStart,
		MoveGraphicalLineEnd,
		MoveWordLeft,
		MoveWordRight,
		MoveParagraphForward,
		MoveParagraphBackward,
		MoveToStartOfNextWord,
		MoveToEndOfPreviousWord,
		Delete,
		Backspace,
		DeleteWordBack,
		DeleteWordForward,
		DeleteLineBack,
		Cut,
		Paste,
		ScrollStart,
		ScrollEnd,
		ScrollPageUp,
		ScrollPageDown
	}
}
