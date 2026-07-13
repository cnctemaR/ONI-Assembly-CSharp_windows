using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal class TextHandlePermanentCache
	{
		public void AddToCache(TextHandle textHandle)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool isCachedPermanentTextCore = textHandle.IsCachedPermanentTextCore;
				if (isCachedPermanentTextCore)
				{
					return;
				}
				bool isCachedTemporary = textHandle.IsCachedTemporary;
				if (isCachedTemporary)
				{
					textHandle.RemoveFromTemporaryCache();
				}
				bool flag2 = this.s_Cache.Count > 0;
				if (flag2)
				{
					textHandle.TextInfoNode = this.s_Cache.Last;
					textHandle.TextInfoNode.SetTextHandle(textHandle);
					this.s_Cache.RemoveLast();
				}
				else
				{
					textHandle.TextInfoNode = new LinkedListNode<TextCacheEntry>(new TextCacheEntry(textHandle, new TextInfo(), 0f));
				}
			}
			textHandle.IsCachedPermanentTextCore = true;
			textHandle.SetDirty();
			textHandle.Update();
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		public void RemoveFromCache(TextHandle textHandle)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool flag2 = !textHandle.IsCachedPermanentTextCore;
				if (!flag2)
				{
					bool flag3 = textHandle.TextInfoNode != null;
					if (flag3)
					{
						this.s_Cache.AddFirst(textHandle.TextInfoNode);
						this.ResetEntryState(textHandle);
					}
					textHandle.IsCachedPermanentTextCore = false;
				}
			}
		}

		private void ResetEntryState(TextHandle handle)
		{
			handle.TextInfoNode.SetTime(0f);
			handle.TextInfoNode.SetTextHandle(null);
			handle.TextInfoNode = null;
		}

		internal LinkedList<TextCacheEntry> s_Cache = new LinkedList<TextCacheEntry>();

		private object syncRoot = new object();
	}
}
