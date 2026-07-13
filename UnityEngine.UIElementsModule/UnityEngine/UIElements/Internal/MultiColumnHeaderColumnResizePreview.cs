using System;

namespace UnityEngine.UIElements.Internal
{
	internal class MultiColumnHeaderColumnResizePreview : VisualElement
	{
		public MultiColumnHeaderColumnResizePreview()
		{
			base.AddToClassList(MultiColumnHeaderColumnResizePreview.ussClassName);
			base.pickingMode = PickingMode.Ignore;
			VisualElement visualElement = new VisualElement
			{
				pickingMode = PickingMode.Ignore
			};
			visualElement.AddToClassList(MultiColumnHeaderColumnResizePreview.visualUssClassName);
			base.Add(visualElement);
		}

		public static readonly string ussClassName = MultiColumnHeaderColumn.ussClassName + "__resize-preview";

		public static readonly string visualUssClassName = MultiColumnHeaderColumnResizePreview.ussClassName + "__visual";
	}
}
