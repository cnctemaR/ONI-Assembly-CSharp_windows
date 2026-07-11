using System;
using System.Collections.Generic;

namespace System.Runtime
{
	internal class MruCache<TKey, TValue> where TKey : class where TValue : class
	{
		public MruCache(int watermark)
			: this(watermark * 4 / 5, watermark)
		{
		}

		public MruCache(int lowWatermark, int highWatermark)
			: this(lowWatermark, highWatermark, null)
		{
		}

		public MruCache(int lowWatermark, int highWatermark, IEqualityComparer<TKey> comparer)
		{
			this.lowWatermark = lowWatermark;
			this.highWatermark = highWatermark;
			this.mruList = new LinkedList<TKey>();
			if (comparer == null)
			{
				this.items = new Dictionary<TKey, MruCache<TKey, TValue>.CacheEntry>();
				return;
			}
			this.items = new Dictionary<TKey, MruCache<TKey, TValue>.CacheEntry>(comparer);
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public void Add(TKey key, TValue value)
		{
			bool flag = false;
			try
			{
				if (this.items.Count == this.highWatermark)
				{
					int num = this.highWatermark - this.lowWatermark;
					for (int i = 0; i < num; i++)
					{
						TKey value2 = this.mruList.Last.Value;
						this.mruList.RemoveLast();
						TValue value3 = this.items[value2].value;
						this.items.Remove(value2);
						this.OnSingleItemRemoved(value3);
						this.OnItemAgedOutOfCache(value3);
					}
				}
				MruCache<TKey, TValue>.CacheEntry cacheEntry;
				cacheEntry.node = this.mruList.AddFirst(key);
				cacheEntry.value = value;
				this.items.Add(key, cacheEntry);
				this.mruEntry = cacheEntry;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Clear();
				}
			}
		}

		public void Clear()
		{
			this.mruList.Clear();
			this.items.Clear();
			this.mruEntry.value = default(TValue);
			this.mruEntry.node = null;
		}

		public bool Remove(TKey key)
		{
			MruCache<TKey, TValue>.CacheEntry cacheEntry;
			if (this.items.TryGetValue(key, out cacheEntry))
			{
				this.items.Remove(key);
				this.OnSingleItemRemoved(cacheEntry.value);
				this.mruList.Remove(cacheEntry.node);
				if (this.mruEntry.node == cacheEntry.node)
				{
					this.mruEntry.value = default(TValue);
					this.mruEntry.node = null;
				}
				return true;
			}
			return false;
		}

		protected virtual void OnSingleItemRemoved(TValue item)
		{
		}

		protected virtual void OnItemAgedOutOfCache(TValue item)
		{
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			if (this.mruEntry.node != null && key != null && key.Equals(this.mruEntry.node.Value))
			{
				value = this.mruEntry.value;
				return true;
			}
			MruCache<TKey, TValue>.CacheEntry cacheEntry;
			bool flag = this.items.TryGetValue(key, out cacheEntry);
			value = cacheEntry.value;
			if (flag && this.mruList.Count > 1 && this.mruList.First != cacheEntry.node)
			{
				this.mruList.Remove(cacheEntry.node);
				this.mruList.AddFirst(cacheEntry.node);
				this.mruEntry = cacheEntry;
			}
			return flag;
		}

		private LinkedList<TKey> mruList;

		private Dictionary<TKey, MruCache<TKey, TValue>.CacheEntry> items;

		private int lowWatermark;

		private int highWatermark;

		private MruCache<TKey, TValue>.CacheEntry mruEntry;

		private struct CacheEntry
		{
			internal TValue value;

			internal LinkedListNode<TKey> node;
		}
	}
}
