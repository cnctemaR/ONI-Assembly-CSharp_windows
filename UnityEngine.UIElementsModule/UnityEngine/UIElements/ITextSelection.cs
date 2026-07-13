using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public interface ITextSelection
	{
		bool isSelectable { get; set; }

		[Obsolete("cursorColor is deprecated. Please use the corresponding USS property (--unity-cursor-color) instead.")]
		Color cursorColor { get; set; }

		[Obsolete("selectionColor is deprecated. Please use the corresponding USS property (--unity-selection-color) instead.")]
		Color selectionColor { get; set; }

		int cursorIndex { get; set; }

		bool doubleClickSelectsWord { get; set; }

		int selectIndex { get; set; }

		bool tripleClickSelectsLine { get; set; }

		bool HasSelection();

		void SelectAll();

		void SelectNone();

		void SelectRange(int cursorIndex, int selectionIndex);

		bool selectAllOnFocus { get; set; }

		bool selectAllOnMouseUp { get; set; }

		Vector2 cursorPosition { get; }

		float lineHeightAtCursorPosition { get; }

		float cursorWidth { get; set; }

		[VisibleToOtherModules(new string[] { "UnityEditor.QuickSearchModule" })]
		void MoveTextEnd();

		Vector2 GetCursorPositionFromStringIndex(int stringIndex);

		void MoveForward();

		void MoveBackward();

		void MoveToParagraphEnd();

		void MoveToParagraphStart();

		void MoveToEndOfPreviousWord();

		void MoveToStartOfNextWord();

		void MoveWordBackward();

		void MoveWordForward();

		event Action OnCursorIndexChange;

		event Action OnSelectIndexChange;
	}
}
