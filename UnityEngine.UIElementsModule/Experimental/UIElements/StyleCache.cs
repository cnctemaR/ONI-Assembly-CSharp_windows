using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	internal static class StyleCache
	{
		public static bool TryGetValue(long hash, out VisualElementStylesData data)
		{
			return StyleCache.s_StyleCache.TryGetValue(hash, out data);
		}

		public static void SetValue(long hash, VisualElementStylesData data)
		{
			StyleCache.s_StyleCache[hash] = data;
		}

		public static void ClearStyleCache()
		{
			StyleCache.s_StyleCache.Clear();
		}

		private static Dictionary<long, VisualElementStylesData> s_StyleCache = new Dictionary<long, VisualElementStylesData>();
	}
}
