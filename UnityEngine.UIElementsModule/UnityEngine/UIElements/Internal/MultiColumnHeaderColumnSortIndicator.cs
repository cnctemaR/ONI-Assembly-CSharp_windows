using System;

namespace UnityEngine.UIElements.Internal
{
	internal class MultiColumnHeaderColumnSortIndicator : VisualElement
	{
		public string sortOrderLabel
		{
			get
			{
				return this.m_IndexLabel.text;
			}
			set
			{
				this.m_IndexLabel.text = value;
			}
		}

		public MultiColumnHeaderColumnSortIndicator()
		{
			base.AddToClassList(MultiColumnHeaderColumnSortIndicator.ussClassName);
			base.pickingMode = PickingMode.Ignore;
			VisualElement visualElement = new VisualElement
			{
				pickingMode = PickingMode.Ignore
			};
			visualElement.AddToClassList(MultiColumnHeaderColumnSortIndicator.arrowUssClassName);
			base.Add(visualElement);
			this.m_IndexLabel = new Label
			{
				pickingMode = PickingMode.Ignore
			};
			this.m_IndexLabel.AddToClassList(MultiColumnHeaderColumnSortIndicator.indexLabelUssClassName);
			base.Add(this.m_IndexLabel);
		}

		public static readonly string ussClassName = MultiColumnHeaderColumn.ussClassName + "__sort-indicator";

		public static readonly string arrowUssClassName = MultiColumnHeaderColumnSortIndicator.ussClassName + "__arrow";

		public static readonly string indexLabelUssClassName = MultiColumnHeaderColumnSortIndicator.ussClassName + "__index-label";

		private Label m_IndexLabel;
	}
}
