using System;

namespace UnityEngine.UIElements
{
	public interface ITextSelection
	{
		bool isSelectable { get; set; }

		Color cursorColor { get; set; }

		int cursorIndex { get; set; }

		bool doubleClickSelectsWord { get; set; }

		int selectIndex { get; set; }

		Color selectionColor { get; set; }

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

		void MoveTextEnd();
	}
}
