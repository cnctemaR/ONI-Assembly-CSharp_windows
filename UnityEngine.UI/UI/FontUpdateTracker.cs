using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public static class FontUpdateTracker
	{
		public static void TrackText(Text t)
		{
			if (t.font == null)
			{
				return;
			}
			HashSet<Text> hashSet;
			FontUpdateTracker.m_Tracked.TryGetValue(t.font, out hashSet);
			if (hashSet == null)
			{
				if (FontUpdateTracker.m_Tracked.Count == 0)
				{
					Font.textureRebuilt += FontUpdateTracker.RebuildForFont;
				}
				hashSet = new HashSet<Text>();
				FontUpdateTracker.m_Tracked.Add(t.font, hashSet);
			}
			hashSet.Add(t);
		}

		private static void RebuildForFont(Font f)
		{
			HashSet<Text> hashSet;
			FontUpdateTracker.m_Tracked.TryGetValue(f, out hashSet);
			if (hashSet == null)
			{
				return;
			}
			foreach (Text text in hashSet)
			{
				text.FontTextureChanged();
			}
		}

		public static void UntrackText(Text t)
		{
			if (t.font == null)
			{
				return;
			}
			HashSet<Text> hashSet;
			FontUpdateTracker.m_Tracked.TryGetValue(t.font, out hashSet);
			if (hashSet == null)
			{
				return;
			}
			hashSet.Remove(t);
			if (hashSet.Count == 0)
			{
				FontUpdateTracker.m_Tracked.Remove(t.font);
				if (FontUpdateTracker.m_Tracked.Count == 0)
				{
					Font.textureRebuilt -= FontUpdateTracker.RebuildForFont;
				}
			}
		}

		private static Dictionary<Font, HashSet<Text>> m_Tracked = new Dictionary<Font, HashSet<Text>>();
	}
}
