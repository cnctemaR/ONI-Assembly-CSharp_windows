using System;

namespace UnityEngine.UIElements
{
	internal struct DragAndDropArgs : IListDragAndDropArgs
	{
		public object target { readonly get; set; }

		public int insertAtIndex { readonly get; set; }

		public int parentId { readonly get; set; }

		public int childIndex { readonly get; set; }

		public DragAndDropPosition dragAndDropPosition { readonly get; set; }

		public DragAndDropData dragAndDropData { readonly get; set; }
	}
}
