using System;
using UnityEngine;

public static class RectTransformExtensions
{
	public static RectTransform Fill(this RectTransform rectTransform)
	{
		rectTransform.anchorMin = new Vector2(0f, 0f);
		rectTransform.anchorMax = new Vector2(1f, 1f);
		rectTransform.anchoredPosition = new Vector2(0f, 0f);
		rectTransform.sizeDelta = new Vector2(0f, 0f);
		return rectTransform;
	}

	public static RectTransform Fill(this RectTransform rectTransform, Padding padding)
	{
		rectTransform.anchorMin = new Vector2(0f, 0f);
		rectTransform.anchorMax = new Vector2(1f, 1f);
		rectTransform.anchoredPosition = new Vector2(padding.left, padding.bottom);
		rectTransform.sizeDelta = new Vector2(-padding.right, -padding.top);
		return rectTransform;
	}

	public static RectTransform Pivot(this RectTransform rectTransform, float x, float y)
	{
		rectTransform.pivot = new Vector2(x, y);
		return rectTransform;
	}

	public static RectTransform Pivot(this RectTransform rectTransform, Vector2 pivot)
	{
		rectTransform.pivot = pivot;
		return rectTransform;
	}
}
