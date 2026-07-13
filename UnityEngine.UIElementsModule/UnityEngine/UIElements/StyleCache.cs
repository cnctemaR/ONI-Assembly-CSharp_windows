using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static class StyleCache
	{
		public static bool TryGetValue(long hash, out ComputedStyle data)
		{
			return StyleCache.s_ComputedStyleCache.TryGetValue(hash, out data);
		}

		public static void SetValue(long hash, ref ComputedStyle data)
		{
			StyleCache.s_ComputedStyleCache[hash] = data;
		}

		public static bool TryGetValue(int hash, out StyleVariableContext data)
		{
			return StyleCache.s_StyleVariableContextCache.TryGetValue(hash, out data);
		}

		public static void SetValue(int hash, StyleVariableContext data)
		{
			StyleCache.s_StyleVariableContextCache[hash] = data;
		}

		public static bool TryGetValue(int hash, out ComputedTransitionProperty[] data)
		{
			return StyleCache.s_ComputedTransitionsCache.TryGetValue(hash, out data);
		}

		public static void SetValue(int hash, ComputedTransitionProperty[] data)
		{
			StyleCache.s_ComputedTransitionsCache[hash] = data;
		}

		public static void ClearStyleCache()
		{
			foreach (KeyValuePair<long, ComputedStyle> keyValuePair in StyleCache.s_ComputedStyleCache)
			{
				keyValuePair.Value.Release();
			}
			StyleCache.s_ComputedStyleCache.Clear();
			StyleCache.s_StyleVariableContextCache.Clear();
			StyleCache.s_ComputedTransitionsCache.Clear();
		}

		private static Dictionary<long, ComputedStyle> s_ComputedStyleCache = new Dictionary<long, ComputedStyle>();

		private static Dictionary<int, StyleVariableContext> s_StyleVariableContextCache = new Dictionary<int, StyleVariableContext>();

		private static Dictionary<int, ComputedTransitionProperty[]> s_ComputedTransitionsCache = new Dictionary<int, ComputedTransitionProperty[]>();
	}
}
