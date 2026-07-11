using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public static class PointerId
	{
		internal static IEnumerable<int> hoveringPointers
		{
			get
			{
				yield return PointerId.mousePointerId;
				yield break;
			}
		}

		public static readonly int maxPointers = 32;

		public static readonly int invalidPointerId = -1;

		public static readonly int mousePointerId = 0;

		public static readonly int touchPointerIdBase = 1;

		public static readonly int touchPointerCount = 20;

		public static readonly int penPointerIdBase = PointerId.touchPointerIdBase + PointerId.touchPointerCount;

		public static readonly int penPointerCount = 2;
	}
}
