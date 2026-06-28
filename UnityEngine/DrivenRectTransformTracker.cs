using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public struct DrivenRectTransformTracker
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CanRecordModifications();

		public void Add(Object driver, RectTransform rectTransform, DrivenTransformProperties drivenProperties)
		{
		}

		public void Clear()
		{
		}
	}
}
