using System;
using UnityEngine;

public static class UIUtil
{
	public static float worldHeight(this RectTransform rt)
	{
		rt.GetWorldCorners(UIUtil.corners);
		return UIUtil.corners[2].y - UIUtil.corners[0].y;
	}

	public static Vector3[] corners = new Vector3[4];
}
