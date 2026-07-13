using System;

namespace UnityEngine.UIElements.Internal
{
	internal class MultiColumnHeaderColumnResizeHandle : VisualElement
	{
		public VisualElement dragArea { get; }

		public MultiColumnHeaderColumnResizeHandle()
		{
			base.AddToClassList(MultiColumnHeaderColumnResizeHandle.ussClassName);
			this.dragArea = new VisualElement
			{
				focusable = true,
				tabIndex = -1
			};
			this.dragArea.AddToClassList(MultiColumnHeaderColumnResizeHandle.dragAreaUssClassName);
			base.Add(this.dragArea);
		}

		public static readonly string ussClassName = MultiColumnCollectionHeader.ussClassName + "__column-resize-handle";

		public static readonly string dragAreaUssClassName = MultiColumnHeaderColumnResizeHandle.ussClassName + "__drag-area";
	}
}
