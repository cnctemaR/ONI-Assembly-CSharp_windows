using System;

namespace UnityEngine.UIElements
{
	public static class PointerType
	{
		internal static string GetPointerType(int pointerId)
		{
			bool flag = pointerId == PointerId.mousePointerId;
			string text;
			if (flag)
			{
				text = PointerType.mouse;
			}
			else
			{
				bool flag2 = pointerId == PointerId.penPointerIdBase;
				if (flag2)
				{
					text = PointerType.pen;
				}
				else
				{
					text = PointerType.touch;
				}
			}
			return text;
		}

		internal static bool IsDirectManipulationDevice(string pointerType)
		{
			return pointerType == PointerType.touch || pointerType == PointerType.pen;
		}

		public static readonly string mouse = "mouse";

		public static readonly string touch = "touch";

		public static readonly string pen = "pen";

		public static readonly string unknown = "";
	}
}
