using System;
using System.ComponentModel;
using UnityEngine;

namespace TMPro
{
	public static class ObjectUtilsBridge
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void MarkDirty(this global::UnityEngine.Object obj)
		{
			obj.MarkDirty();
		}
	}
}
