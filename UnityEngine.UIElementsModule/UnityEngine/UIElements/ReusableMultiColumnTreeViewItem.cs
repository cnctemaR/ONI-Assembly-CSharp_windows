using System;

namespace UnityEngine.UIElements
{
	internal class ReusableMultiColumnTreeViewItem : ReusableTreeViewItem
	{
		public override VisualElement rootElement
		{
			get
			{
				return base.bindableElement;
			}
		}

		public override void Init(VisualElement item)
		{
		}

		public void Init(VisualElement container, Columns columns)
		{
			int num = 0;
			base.bindableElement = container;
			foreach (Column column in columns.visibleList)
			{
				bool flag = columns.IsPrimary(column);
				if (flag)
				{
					VisualElement visualElement = container[num];
					VisualElement visualElement2 = visualElement.GetProperty(MultiColumnController.bindableElementPropertyName) as VisualElement;
					base.InitExpandHierarchy(visualElement, visualElement2);
					break;
				}
				num++;
			}
		}
	}
}
