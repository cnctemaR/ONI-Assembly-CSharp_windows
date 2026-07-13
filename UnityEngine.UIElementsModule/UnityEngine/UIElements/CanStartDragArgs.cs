using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public readonly struct CanStartDragArgs
	{
		internal CanStartDragArgs(VisualElement draggedElement, int id, IEnumerable<int> selectedIds, EventModifiers modifiers)
		{
			this.draggedElement = draggedElement;
			this.id = id;
			this.selectedIds = selectedIds;
			this.modifiers = modifiers;
		}

		public readonly VisualElement draggedElement;

		public readonly int id;

		public readonly IEnumerable<int> selectedIds;

		internal readonly EventModifiers modifiers;
	}
}
