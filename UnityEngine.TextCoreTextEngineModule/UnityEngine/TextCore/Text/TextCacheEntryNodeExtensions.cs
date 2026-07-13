using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text
{
	internal static class TextCacheEntryNodeExtensions
	{
		public static void SetTime(this LinkedListNode<TextCacheEntry> node, float newTime)
		{
			TextCacheEntry value = node.Value;
			value.lastTimeInCache = newTime;
			node.Value = value;
		}

		public static void SetTextHandle(this LinkedListNode<TextCacheEntry> node, TextHandle newTextHandle)
		{
			TextCacheEntry value = node.Value;
			Debug.Assert((value.textHandle == null) ^ (newTextHandle == null), "Internal Text Error : changing the TextCore caching node while the other is assigned. It might indicate the previous is still in cache");
			value.textHandle = newTextHandle;
			node.Value = value;
		}
	}
}
