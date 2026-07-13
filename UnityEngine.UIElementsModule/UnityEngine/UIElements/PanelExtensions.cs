using System;

namespace UnityEngine.UIElements
{
	public static class PanelExtensions
	{
		public static AbstractGenericMenu CreateMenu(this IPanel panel)
		{
			BaseVisualElementPanel baseVisualElementPanel = panel as BaseVisualElementPanel;
			bool flag = baseVisualElementPanel != null;
			AbstractGenericMenu abstractGenericMenu;
			if (flag)
			{
				abstractGenericMenu = baseVisualElementPanel.CreateMenu();
			}
			else
			{
				abstractGenericMenu = null;
			}
			return abstractGenericMenu;
		}
	}
}
