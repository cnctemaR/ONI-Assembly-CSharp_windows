using System;

namespace UnityEngine
{
	internal enum TextSelectOp
	{
		SelectLeft,
		SelectRight,
		SelectUp,
		SelectDown,
		SelectTextStart,
		SelectTextEnd,
		SelectPageUp,
		SelectPageDown,
		ExpandSelectGraphicalLineStart,
		ExpandSelectGraphicalLineEnd,
		SelectGraphicalLineStart,
		SelectGraphicalLineEnd,
		SelectWordLeft,
		SelectWordRight,
		SelectToEndOfPreviousWord,
		SelectToStartOfNextWord,
		SelectParagraphBackward,
		SelectParagraphForward,
		Copy,
		SelectAll,
		SelectNone
	}
}
