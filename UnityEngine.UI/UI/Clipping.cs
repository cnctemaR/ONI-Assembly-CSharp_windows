using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public static class Clipping
	{
		public static Rect FindCullAndClipWorldRect(List<RectMask2D> rectMaskParents, out bool validRect)
		{
			if (rectMaskParents.Count == 0)
			{
				validRect = false;
				return default(Rect);
			}
			Rect rect = rectMaskParents[0].canvasRect;
			Vector4 vector = rectMaskParents[0].padding;
			float num = rect.xMin + vector.x;
			float num2 = rect.xMax - vector.z;
			float num3 = rect.yMin + vector.y;
			float num4 = rect.yMax - vector.w;
			int count = rectMaskParents.Count;
			for (int i = 1; i < count; i++)
			{
				rect = rectMaskParents[i].canvasRect;
				vector = rectMaskParents[i].padding;
				if (num < rect.xMin + vector.x)
				{
					num = rect.xMin + vector.x;
				}
				if (num3 < rect.yMin + vector.y)
				{
					num3 = rect.yMin + vector.y;
				}
				if (num2 > rect.xMax - vector.z)
				{
					num2 = rect.xMax - vector.z;
				}
				if (num4 > rect.yMax - vector.w)
				{
					num4 = rect.yMax - vector.w;
				}
			}
			validRect = num2 > num && num4 > num3;
			if (!validRect)
			{
				return default(Rect);
			}
			return new Rect(num, num3, num2 - num, num4 - num3);
		}
	}
}
