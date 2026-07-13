using System;

namespace UnityEngine.UIElements
{
	internal class TabDragLocationPreview : VisualElement
	{
		internal VisualElement preview
		{
			get
			{
				return this.m_Preview;
			}
		}

		public TabDragLocationPreview()
		{
			base.AddToClassList(TabDragLocationPreview.ussClassName);
			base.pickingMode = PickingMode.Ignore;
			this.m_Preview = new VisualElement();
			this.m_Preview.AddToClassList(TabDragLocationPreview.visualUssClassName);
			this.m_Preview.pickingMode = PickingMode.Ignore;
			base.Add(this.m_Preview);
		}

		public static readonly string ussClassName = TabView.ussClassName + "__drag-location-preview";

		public static readonly string visualUssClassName = TabDragLocationPreview.ussClassName + "__visual";

		public static readonly string verticalUssClassName = TabDragLocationPreview.ussClassName + "__vertical";

		public static readonly string horizontalUssClassName = TabDragLocationPreview.ussClassName + "__horizontal";

		private VisualElement m_Preview;
	}
}
