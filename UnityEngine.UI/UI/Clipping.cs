using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public static class Clipping
	{
		public static Rect FindCullAndClipWorldRect(List<RectMask2D> rectMaskParents, out bool validRect)
		{
			Rect rect;
			if (rectMaskParents.Count == 0)
			{
				validRect = false;
				rect = default(Rect);
			}
			else
			{
				Rect rect2 = rectMaskParents[0].canvasRect;
				for (int i = 0; i < rectMaskParents.Count; i++)
				{
					rect2 = Clipping.RectIntersect(rect2, rectMaskParents[i].canvasRect);
				}
				bool flag = rect2.width <= 0f || rect2.height <= 0f;
				if (flag)
				{
					validRect = false;
					rect = default(Rect);
				}
				else
				{
					Vector3 vector = new Vector3(rect2.x, rect2.y, 0f);
					Vector3 vector2 = new Vector3(rect2.x + rect2.width, rect2.y + rect2.height, 0f);
					validRect = true;
					rect = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
				}
			}
			return rect;
		}

		private static Rect RectIntersect(Rect a, Rect b)
		{
			float num = Mathf.Max(a.x, b.x);
			float num2 = Mathf.Min(a.x + a.width, b.x + b.width);
			float num3 = Mathf.Max(a.y, b.y);
			float num4 = Mathf.Min(a.y + a.height, b.y + b.height);
			Rect rect;
			if (num2 >= num && num4 >= num3)
			{
				rect = new Rect(num, num3, num2 - num, num4 - num3);
			}
			else
			{
				rect = new Rect(0f, 0f, 0f, 0f);
			}
			return rect;
		}
	}
}
