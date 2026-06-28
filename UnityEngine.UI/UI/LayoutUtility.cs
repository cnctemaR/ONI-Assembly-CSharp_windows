using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public static class LayoutUtility
	{
		public static float GetMinSize(RectTransform rect, int axis)
		{
			float num;
			if (axis == 0)
			{
				num = LayoutUtility.GetMinWidth(rect);
			}
			else
			{
				num = LayoutUtility.GetMinHeight(rect);
			}
			return num;
		}

		public static float GetPreferredSize(RectTransform rect, int axis)
		{
			float num;
			if (axis == 0)
			{
				num = LayoutUtility.GetPreferredWidth(rect);
			}
			else
			{
				num = LayoutUtility.GetPreferredHeight(rect);
			}
			return num;
		}

		public static float GetFlexibleSize(RectTransform rect, int axis)
		{
			float num;
			if (axis == 0)
			{
				num = LayoutUtility.GetFlexibleWidth(rect);
			}
			else
			{
				num = LayoutUtility.GetFlexibleHeight(rect);
			}
			return num;
		}

		public static float GetMinWidth(RectTransform rect)
		{
			return LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.minWidth, 0f);
		}

		public static float GetPreferredWidth(RectTransform rect)
		{
			return Mathf.Max(LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.minWidth, 0f), LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.preferredWidth, 0f));
		}

		public static float GetFlexibleWidth(RectTransform rect)
		{
			return LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.flexibleWidth, 0f);
		}

		public static float GetMinHeight(RectTransform rect)
		{
			return LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.minHeight, 0f);
		}

		public static float GetPreferredHeight(RectTransform rect)
		{
			return Mathf.Max(LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.minHeight, 0f), LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.preferredHeight, 0f));
		}

		public static float GetFlexibleHeight(RectTransform rect)
		{
			return LayoutUtility.GetLayoutProperty(rect, (ILayoutElement e) => e.flexibleHeight, 0f);
		}

		public static float GetLayoutProperty(RectTransform rect, Func<ILayoutElement, float> property, float defaultValue)
		{
			ILayoutElement layoutElement;
			return LayoutUtility.GetLayoutProperty(rect, property, defaultValue, out layoutElement);
		}

		public static float GetLayoutProperty(RectTransform rect, Func<ILayoutElement, float> property, float defaultValue, out ILayoutElement source)
		{
			source = null;
			float num;
			if (rect == null)
			{
				num = 0f;
			}
			else
			{
				float num2 = defaultValue;
				int num3 = int.MinValue;
				List<Component> list = ListPool<Component>.Get();
				rect.GetComponents(typeof(ILayoutElement), list);
				for (int i = 0; i < list.Count; i++)
				{
					ILayoutElement layoutElement = list[i] as ILayoutElement;
					if (!(layoutElement is Behaviour) || ((Behaviour)layoutElement).isActiveAndEnabled)
					{
						int layoutPriority = layoutElement.layoutPriority;
						if (layoutPriority >= num3)
						{
							float num4 = property(layoutElement);
							if (num4 >= 0f)
							{
								if (layoutPriority > num3)
								{
									num2 = num4;
									num3 = layoutPriority;
									source = layoutElement;
								}
								else if (num4 > num2)
								{
									num2 = num4;
									source = layoutElement;
								}
							}
						}
					}
				}
				ListPool<Component>.Release(list);
				num = num2;
			}
			return num;
		}
	}
}
