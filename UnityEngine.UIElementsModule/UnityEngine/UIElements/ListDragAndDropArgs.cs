using System;

namespace UnityEngine.UIElements
{
	internal struct ListDragAndDropArgs : IListDragAndDropArgs
	{
		public object target { readonly get; set; }

		public int insertAtIndex { readonly get; set; }

		public DragAndDropPosition dragAndDropPosition { readonly get; set; }

		public IDragAndDropData dragAndDropData
		{
			get
			{
				return DragAndDropUtility.dragAndDrop.data;
			}
		}
	}
}
