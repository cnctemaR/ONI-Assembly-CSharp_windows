using System;

namespace UnityEngine.UIElements.Internal
{
	internal class MultiColumnHeaderColumnMovePreview : VisualElement
	{
		public MultiColumnHeaderColumnMovePreview()
		{
			base.AddToClassList(MultiColumnHeaderColumnMovePreview.ussClassName);
			base.pickingMode = PickingMode.Ignore;
		}

		public static readonly string ussClassName = MultiColumnHeaderColumn.ussClassName + "__move-preview";
	}
}
