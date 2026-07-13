using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class TabLayout
	{
		public TabLayout(TabView tabView, bool isVertical)
		{
			this.m_TabView = tabView;
			this.m_TabHeaders = tabView.tabHeaders;
			this.m_IsVertical = isVertical;
		}

		public static float GetHeight(VisualElement t)
		{
			return t.boundingBox.height;
		}

		public static float GetWidth(VisualElement t)
		{
			return t.boundingBox.width;
		}

		public float GetTabOffset(VisualElement tab)
		{
			bool flag = !tab.visible;
			float num;
			if (flag)
			{
				num = float.NaN;
			}
			else
			{
				float num2 = 0f;
				int num3 = this.m_TabHeaders.IndexOf(tab);
				for (int i = 0; i < num3; i++)
				{
					VisualElement visualElement = this.m_TabHeaders[i];
					float num4 = (this.m_IsVertical ? TabLayout.GetHeight(visualElement) : TabLayout.GetWidth(visualElement));
					bool flag2 = float.IsNaN(num4);
					if (!flag2)
					{
						num2 += num4;
					}
				}
				num = num2;
			}
			return num;
		}

		private void InitOrderTabs()
		{
			if (this.m_TabHeaders == null)
			{
				this.m_TabHeaders = new List<VisualElement>();
			}
		}

		public void ReorderDisplay(int from, int to)
		{
			this.InitOrderTabs();
			this.m_TabView.ReorderTab(from, to);
		}

		private TabView m_TabView;

		private List<VisualElement> m_TabHeaders;

		private bool m_IsVertical;
	}
}
