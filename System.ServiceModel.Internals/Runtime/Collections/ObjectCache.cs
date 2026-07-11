using System;
using System.Collections.Generic;

namespace System.Runtime.Collections
{
	internal class ObjectCache<TKey, TValue> where TValue : class
	{
		public ObjectCache(ObjectCacheSettings settings)
			: this(settings, null)
		{
		}

		public ObjectCache(ObjectCacheSettings settings, IEqualityComparer<TKey> comparer)
		{
			this.settings = settings.Clone();
			this.cacheItems = new Dictionary<TKey, ObjectCache<TKey, TValue>.Item>(comparer);
			this.idleTimeoutEnabled = settings.IdleTimeout != TimeSpan.MaxValue;
			this.leaseTimeoutEnabled = settings.LeaseTimeout != TimeSpan.MaxValue;
		}

		private object ThisLock
		{
			get
			{
				return this;
			}
		}

		public Action<TValue> DisposeItemCallback { get; set; }

		public int Count
		{
			get
			{
				return this.cacheItems.Count;
			}
		}

		public ObjectCacheItem<TValue> Add(TKey key, TValue value)
		{
			object thisLock = this.ThisLock;
			ObjectCacheItem<TValue> objectCacheItem;
			lock (thisLock)
			{
				if (this.Count >= this.settings.CacheLimit || this.cacheItems.ContainsKey(key))
				{
					objectCacheItem = new ObjectCache<TKey, TValue>.Item(key, value, this.DisposeItemCallback);
				}
				else
				{
					objectCacheItem = this.InternalAdd(key, value);
				}
			}
			return objectCacheItem;
		}

		public ObjectCacheItem<TValue> Take(TKey key)
		{
			return this.Take(key, null);
		}

		public ObjectCacheItem<TValue> Take(TKey key, Func<TValue> initializerDelegate)
		{
			ObjectCache<TKey, TValue>.Item item = null;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.cacheItems.TryGetValue(key, out item))
				{
					item.InternalAddReference();
				}
				else
				{
					if (initializerDelegate == null)
					{
						return null;
					}
					TValue tvalue = initializerDelegate();
					if (this.Count >= this.settings.CacheLimit)
					{
						return new ObjectCache<TKey, TValue>.Item(key, tvalue, this.DisposeItemCallback);
					}
					item = this.InternalAdd(key, tvalue);
				}
			}
			return item;
		}

		private ObjectCache<TKey, TValue>.Item InternalAdd(TKey key, TValue value)
		{
			ObjectCache<TKey, TValue>.Item item = new ObjectCache<TKey, TValue>.Item(key, value, this);
			if (this.leaseTimeoutEnabled)
			{
				item.CreationTime = DateTime.UtcNow;
			}
			this.cacheItems.Add(key, item);
			this.StartTimerIfNecessary();
			return item;
		}

		private bool Return(TKey key, ObjectCache<TKey, TValue>.Item cacheItem)
		{
			bool flag = false;
			if (this.disposed)
			{
				flag = true;
			}
			else
			{
				cacheItem.InternalReleaseReference();
				DateTime utcNow = DateTime.UtcNow;
				if (this.idleTimeoutEnabled)
				{
					cacheItem.LastUsage = utcNow;
				}
				if (this.ShouldPurgeItem(cacheItem, utcNow))
				{
					this.cacheItems.Remove(key);
					cacheItem.LockedDispose();
					flag = true;
				}
			}
			return flag;
		}

		private void StartTimerIfNecessary()
		{
			if (this.idleTimeoutEnabled && this.Count > 1)
			{
				if (this.idleTimer == null)
				{
					if (ObjectCache<TKey, TValue>.onIdle == null)
					{
						ObjectCache<TKey, TValue>.onIdle = new Action<object>(ObjectCache<TKey, TValue>.OnIdle);
					}
					this.idleTimer = new IOThreadTimer(ObjectCache<TKey, TValue>.onIdle, this, false);
				}
				this.idleTimer.Set(this.settings.IdleTimeout);
			}
		}

		private static void OnIdle(object state)
		{
			((ObjectCache<TKey, TValue>)state).PurgeCache(true);
		}

		private static void Add<T>(ref List<T> list, T item)
		{
			if (list == null)
			{
				list = new List<T>();
			}
			list.Add(item);
		}

		private bool ShouldPurgeItem(ObjectCache<TKey, TValue>.Item cacheItem, DateTime now)
		{
			return cacheItem.ReferenceCount <= 0 && ((this.idleTimeoutEnabled && now >= cacheItem.LastUsage + this.settings.IdleTimeout) || (this.leaseTimeoutEnabled && now - cacheItem.CreationTime >= this.settings.LeaseTimeout));
		}

		private void GatherExpiredItems(ref List<KeyValuePair<TKey, ObjectCache<TKey, TValue>.Item>> expiredItems, bool calledFromTimer)
		{
			if (this.Count == 0)
			{
				return;
			}
			if (!this.leaseTimeoutEnabled && !this.idleTimeoutEnabled)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			bool flag = false;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				foreach (KeyValuePair<TKey, ObjectCache<TKey, TValue>.Item> keyValuePair in this.cacheItems)
				{
					if (this.ShouldPurgeItem(keyValuePair.Value, utcNow))
					{
						keyValuePair.Value.LockedDispose();
						ObjectCache<TKey, TValue>.Add<KeyValuePair<TKey, ObjectCache<TKey, TValue>.Item>>(ref expiredItems, keyValuePair);
					}
				}
				if (expiredItems != null)
				{
					for (int i = 0; i < expiredItems.Count; i++)
					{
						this.cacheItems.Remove(expiredItems[i].Key);
					}
				}
				flag = calledFromTimer && this.Count > 0;
			}
			if (flag)
			{
				this.idleTimer.Set(this.settings.IdleTimeout);
			}
		}

		private void PurgeCache(bool calledFromTimer)
		{
			List<KeyValuePair<TKey, ObjectCache<TKey, TValue>.Item>> list = null;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				this.GatherExpiredItems(ref list, calledFromTimer);
			}
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].Value.LocalDispose();
				}
			}
		}

		public void Dispose()
		{
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				foreach (ObjectCache<TKey, TValue>.Item item in this.cacheItems.Values)
				{
					if (item != null)
					{
						item.Dispose();
					}
				}
				this.cacheItems.Clear();
				this.settings.CacheLimit = 0;
				this.disposed = true;
				if (this.idleTimer != null)
				{
					this.idleTimer.Cancel();
					this.idleTimer = null;
				}
			}
		}

		private const int timerThreshold = 1;

		private ObjectCacheSettings settings;

		private Dictionary<TKey, ObjectCache<TKey, TValue>.Item> cacheItems;

		private bool idleTimeoutEnabled;

		private bool leaseTimeoutEnabled;

		private IOThreadTimer idleTimer;

		private static Action<object> onIdle;

		private bool disposed;

		private class Item : ObjectCacheItem<TValue>
		{
			public Item(TKey key, TValue value, Action<TValue> disposeItemCallback)
				: this(key, value)
			{
				this.disposeItemCallback = disposeItemCallback;
			}

			public Item(TKey key, TValue value, ObjectCache<TKey, TValue> parent)
				: this(key, value)
			{
				this.parent = parent;
			}

			private Item(TKey key, TValue value)
			{
				this.key = key;
				this.value = value;
				this.referenceCount = 1;
			}

			public int ReferenceCount
			{
				get
				{
					return this.referenceCount;
				}
			}

			public override TValue Value
			{
				get
				{
					return this.value;
				}
			}

			public DateTime CreationTime { get; set; }

			public DateTime LastUsage { get; set; }

			public override bool TryAddReference()
			{
				bool flag;
				if (this.parent == null || this.referenceCount == -1)
				{
					flag = false;
				}
				else
				{
					bool flag2 = false;
					object thisLock = this.parent.ThisLock;
					lock (thisLock)
					{
						if (this.referenceCount == -1)
						{
							flag = false;
						}
						else if (this.referenceCount == 0 && this.parent.ShouldPurgeItem(this, DateTime.UtcNow))
						{
							this.LockedDispose();
							flag2 = true;
							flag = false;
							this.parent.cacheItems.Remove(this.key);
						}
						else
						{
							this.referenceCount++;
							flag = true;
						}
					}
					if (flag2)
					{
						this.LocalDispose();
					}
				}
				return flag;
			}

			public override void ReleaseReference()
			{
				bool flag;
				if (this.parent == null)
				{
					this.referenceCount = -1;
					flag = true;
				}
				else
				{
					object thisLock = this.parent.ThisLock;
					lock (thisLock)
					{
						if (this.referenceCount > 1)
						{
							this.InternalReleaseReference();
							flag = false;
						}
						else
						{
							flag = this.parent.Return(this.key, this);
						}
					}
				}
				if (flag)
				{
					this.LocalDispose();
				}
			}

			internal void InternalAddReference()
			{
				this.referenceCount++;
			}

			internal void InternalReleaseReference()
			{
				this.referenceCount--;
			}

			public void LockedDispose()
			{
				this.referenceCount = -1;
			}

			public void Dispose()
			{
				if (this.Value != null)
				{
					Action<TValue> action = this.disposeItemCallback;
					if (this.parent != null)
					{
						action = this.parent.DisposeItemCallback;
					}
					if (action != null)
					{
						action(this.Value);
					}
					else if (this.Value is IDisposable)
					{
						((IDisposable)((object)this.Value)).Dispose();
					}
				}
				this.value = default(TValue);
				this.referenceCount = -1;
			}

			public void LocalDispose()
			{
				this.Dispose();
			}

			private readonly ObjectCache<TKey, TValue> parent;

			private readonly TKey key;

			private readonly Action<TValue> disposeItemCallback;

			private TValue value;

			private int referenceCount;
		}
	}
}
