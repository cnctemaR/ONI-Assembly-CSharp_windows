using System;

namespace UnityEngine.UIElements
{
	public static class AlignmentUtils
	{
		internal static float RoundToPixelGrid(float v, float pixelsPerPoint, float offset = 0.02f)
		{
			return Mathf.Floor(v * pixelsPerPoint + 0.5f + offset) / pixelsPerPoint;
		}

		internal static float CeilToPixelGrid(float v, float pixelsPerPoint, float offset = -0.02f)
		{
			return Mathf.Ceil(v * pixelsPerPoint + offset) / pixelsPerPoint;
		}

		internal static float FloorToPixelGrid(float v, float pixelsPerPoint, float offset = 0.02f)
		{
			return Mathf.Floor(v * pixelsPerPoint + offset) / pixelsPerPoint;
		}

		public static float RoundToPanelPixelSize(this VisualElement ve, float v)
		{
			return AlignmentUtils.RoundToPixelGrid(v, ve.scaledPixelsPerPoint, 0.02f);
		}

		public static float CeilToPanelPixelSize(this VisualElement ve, float v)
		{
			return AlignmentUtils.CeilToPixelGrid(v, ve.scaledPixelsPerPoint, -0.02f);
		}

		public static float FloorToPanelPixelSize(this VisualElement ve, float v)
		{
			return AlignmentUtils.FloorToPixelGrid(v, ve.scaledPixelsPerPoint, 0.02f);
		}
	}
}
