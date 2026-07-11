using System;

namespace UnityEngine
{
	public struct DrivenRectTransformTracker
	{
		internal static bool CanRecordModifications()
		{
			return true;
		}

		public void Add(Object driver, RectTransform rectTransform, DrivenTransformProperties drivenProperties)
		{
		}

		[Obsolete("revertValues parameter is ignored. Please use Clear() instead.")]
		public void Clear(bool revertValues)
		{
			this.Clear();
		}

		public void Clear()
		{
		}
	}
}
