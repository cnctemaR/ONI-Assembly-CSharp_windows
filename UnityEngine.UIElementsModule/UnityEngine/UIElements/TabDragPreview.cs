using System;

namespace UnityEngine.UIElements
{
	internal class TabDragPreview : VisualElement
	{
		public TabDragPreview()
		{
			base.AddToClassList(TabDragPreview.ussClassName);
			base.pickingMode = PickingMode.Ignore;
		}

		public static readonly string ussClassName = TabView.ussClassName + "__drag-preview";
	}
}
