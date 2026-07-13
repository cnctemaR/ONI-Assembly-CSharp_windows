using System;

namespace UnityEngine.TextCore.Text
{
	internal struct TextCacheEntry
	{
		public TextCacheEntry(TextHandle handle, TextInfo info, float time = 0f)
		{
			Debug.Assert(handle != null, "Internal Text Error : Creation of a not assigned to no handle");
			this.textHandle = handle;
			this.textInfo = info;
			this.lastTimeInCache = time;
		}

		public TextHandle textHandle;

		public TextInfo textInfo;

		public float lastTimeInCache;
	}
}
