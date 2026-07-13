using System;

namespace UnityEngine.UIElements
{
	internal interface IListDragAndDropArgs
	{
		object target { get; }

		int insertAtIndex { get; }

		int parentId { get; }

		int childIndex { get; }

		DragAndDropData dragAndDropData { get; }

		DragAndDropPosition dragAndDropPosition { get; }
	}
}
