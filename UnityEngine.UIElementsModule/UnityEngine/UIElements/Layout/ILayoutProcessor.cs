using System;

namespace UnityEngine.UIElements.Layout
{
	internal interface ILayoutProcessor
	{
		void CalculateLayout(LayoutNode node, float parentWidth, float parentHeight, LayoutDirection parentDirection);
	}
}
