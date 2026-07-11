using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal static class StyleCache
	{
		public static bool TryGetValue(long hash, out VisualElementStylesData data)
		{
			return StyleCache.s_StyleDataCache.TryGetValue(hash, out data);
		}

		public static void SetValue(long hash, VisualElementStylesData data)
		{
			StyleCache.s_StyleDataCache[hash] = data;
		}

		public static bool TryGetValue(int hash, out InheritedStylesData data)
		{
			return StyleCache.s_InheritedStyleDataCache.TryGetValue(hash, out data);
		}

		public static void SetValue(int hash, InheritedStylesData data)
		{
			StyleCache.s_InheritedStyleDataCache[hash] = data;
		}

		public static bool TryGetValue(int hash, out StyleVariableContext data)
		{
			return StyleCache.s_StyleVariableContextCache.TryGetValue(hash, out data);
		}

		public static void SetValue(int hash, StyleVariableContext data)
		{
			StyleCache.s_StyleVariableContextCache[hash] = data;
		}

		public static void ClearStyleCache()
		{
			StyleCache.s_StyleDataCache.Clear();
			StyleCache.s_InheritedStyleDataCache.Clear();
			StyleCache.s_StyleVariableContextCache.Clear();
		}

		private static Dictionary<long, VisualElementStylesData> s_StyleDataCache = new Dictionary<long, VisualElementStylesData>();

		private static Dictionary<int, InheritedStylesData> s_InheritedStyleDataCache = new Dictionary<int, InheritedStylesData>();

		private static Dictionary<int, StyleVariableContext> s_StyleVariableContextCache = new Dictionary<int, StyleVariableContext>();
	}
}
