using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal class TextHandleTemporaryCache
	{
		public void ClearTemporaryCache()
		{
			foreach (TextCacheEntry textCacheEntry in this.s_Cache)
			{
				this.ResetEntryState(textCacheEntry.textHandle);
			}
			this.s_Cache.Clear();
		}

		public void AddTextInfoToCache(TextHandle textHandle, int hashCode)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool isCachedPermanentTextCore = textHandle.IsCachedPermanentTextCore;
				if (isCachedPermanentTextCore)
				{
					return;
				}
				bool flag2 = !TextGenerator.IsExecutingJob;
				bool flag3 = flag2;
				if (flag3)
				{
					this.currentFrame = Time.frameCount;
				}
				bool flag4 = this.s_Cache.Count > 0 && ((float)this.currentFrame - this.s_Cache.Last.Value.lastTimeInCache < 0f || (float)this.currentFrame - this.s_Cache.First.Value.lastTimeInCache < 0f);
				if (flag4)
				{
					this.ClearTemporaryCache();
				}
				bool isCachedTemporary = textHandle.IsCachedTemporary;
				if (isCachedTemporary)
				{
					this.RefreshCaching(textHandle);
					return;
				}
				bool flag5 = this.s_Cache.Count > 0 && (float)this.currentFrame - this.s_Cache.Last.Value.lastTimeInCache > 2f;
				if (flag5)
				{
					this.RecycleTextInfoFromCache(textHandle);
				}
				else
				{
					TextInfo textInfo = new TextInfo();
					textHandle.TextInfoNode = new LinkedListNode<TextCacheEntry>(new TextCacheEntry(textHandle, textInfo, (float)this.currentFrame));
					this.s_Cache.AddFirst(textHandle.TextInfoNode);
				}
			}
			textHandle.IsCachedTemporary = true;
			textHandle.SetDirty();
			textHandle.UpdateWithHash(hashCode);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal void RemoveFromCache(TextHandle handle)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool flag2 = !handle.IsCachedTemporary;
				if (!flag2)
				{
					bool flag3 = handle.TextInfoNode != null;
					if (flag3)
					{
						this.s_Cache.Remove(handle.TextInfoNode);
						this.s_Cache.AddLast(handle.TextInfoNode);
					}
					this.ResetEntryState(handle);
				}
			}
		}

		internal void ResetEntryState(TextHandle handle)
		{
			bool flag = handle == null || !handle.IsCachedTemporary;
			if (!flag)
			{
				handle.IsCachedTemporary = false;
				handle.TextInfoNode.SetTime(0f);
				handle.TextInfoNode.SetTextHandle(null);
				handle.TextInfoNode = null;
			}
		}

		private void RefreshCaching(TextHandle textHandle)
		{
			bool flag = !TextGenerator.IsExecutingJob;
			if (flag)
			{
				this.currentFrame = Time.frameCount;
			}
			textHandle.TextInfoNode.SetTime((float)this.currentFrame);
			this.s_Cache.Remove(textHandle.TextInfoNode);
			this.s_Cache.AddFirst(textHandle.TextInfoNode);
		}

		private void RecycleTextInfoFromCache(TextHandle textHandle)
		{
			bool flag = !TextGenerator.IsExecutingJob;
			if (flag)
			{
				this.currentFrame = Time.frameCount;
			}
			textHandle.RemoveFromTemporaryCache();
			bool flag2 = this.s_Cache.Last.Value.textHandle != null;
			if (flag2)
			{
				this.s_Cache.Last.Value.textHandle.RemoveFromTemporaryCache();
			}
			textHandle.TextInfoNode = this.s_Cache.Last;
			textHandle.TextInfoNode.SetTextHandle(textHandle);
			textHandle.TextInfoNode.SetTime((float)this.currentFrame);
			textHandle.IsCachedTemporary = true;
			this.s_Cache.RemoveLast();
			this.s_Cache.AddFirst(textHandle.TextInfoNode);
		}

		public void UpdateCurrentFrame()
		{
			this.currentFrame = Time.frameCount;
		}

		internal LinkedList<TextCacheEntry> s_Cache = new LinkedList<TextCacheEntry>();

		internal const int s_MinFramesInCache = 2;

		internal int currentFrame;

		private object syncRoot = new object();
	}
}
