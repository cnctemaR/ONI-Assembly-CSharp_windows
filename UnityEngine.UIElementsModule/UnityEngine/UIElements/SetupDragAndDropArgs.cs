using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public readonly struct SetupDragAndDropArgs
	{
		internal SetupDragAndDropArgs(VisualElement draggedElement, IEnumerable<int> selectedIds, StartDragArgs startDragArgs)
		{
			this.draggedElement = draggedElement;
			this.selectedIds = selectedIds;
			this.startDragArgs = startDragArgs;
		}

		public readonly VisualElement draggedElement;

		public readonly IEnumerable<int> selectedIds;

		public readonly StartDragArgs startDragArgs;
	}
}
