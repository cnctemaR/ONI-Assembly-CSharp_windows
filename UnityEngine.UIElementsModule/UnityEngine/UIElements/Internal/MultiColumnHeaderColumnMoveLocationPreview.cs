using System;

namespace UnityEngine.UIElements.Internal
{
	internal class MultiColumnHeaderColumnMoveLocationPreview : VisualElement
	{
		public MultiColumnHeaderColumnMoveLocationPreview()
		{
			base.AddToClassList(MultiColumnHeaderColumnMoveLocationPreview.ussClassName);
			base.pickingMode = PickingMode.Ignore;
			VisualElement visualElement = new VisualElement();
			visualElement.AddToClassList(MultiColumnHeaderColumnMoveLocationPreview.visualUssClassName);
			visualElement.pickingMode = PickingMode.Ignore;
			base.Add(visualElement);
		}

		public static readonly string ussClassName = MultiColumnHeaderColumn.ussClassName + "__move-location-preview";

		public static readonly string visualUssClassName = MultiColumnHeaderColumnMoveLocationPreview.ussClassName + "__visual";
	}
}
