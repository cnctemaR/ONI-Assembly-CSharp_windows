using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements
{
	internal interface IDragAndDropController<in TArgs>
	{
		bool CanStartDrag(IEnumerable<int> itemIds);

		bool CanDrop()
		{
			return true;
		}

		StartDragArgs SetupDragAndDrop(IEnumerable<int> itemIds, bool skipText = false);

		DragVisualMode HandleDragAndDrop(TArgs args);

		void OnDrop(TArgs args);

		void DragCleanup()
		{
		}

		void HandleAutoExpand(ReusableCollectionItem item, Vector2 pointerPosition)
		{
		}

		IEnumerable<int> GetSortedSelectedIds()
		{
			return Enumerable.Empty<int>();
		}
	}
}
