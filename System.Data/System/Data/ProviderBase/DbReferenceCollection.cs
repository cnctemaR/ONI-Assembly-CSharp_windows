using System;
using System.Threading;

namespace System.Data.ProviderBase
{
	internal abstract class DbReferenceCollection
	{
		protected DbReferenceCollection()
		{
			this._items = new DbReferenceCollection.CollectionEntry[20];
			this._itemLock = new object();
			this._optimisticCount = 0;
			this._lastItemIndex = 0;
		}

		public abstract void Add(object value, int tag);

		protected void AddItem(object value, int tag)
		{
			bool flag = false;
			object itemLock = this._itemLock;
			lock (itemLock)
			{
				for (int i = 0; i <= this._lastItemIndex; i++)
				{
					if (this._items[i].Tag == 0)
					{
						this._items[i].NewTarget(tag, value);
						flag = true;
						break;
					}
				}
				if (!flag && this._lastItemIndex + 1 < this._items.Length)
				{
					this._lastItemIndex++;
					this._items[this._lastItemIndex].NewTarget(tag, value);
					flag = true;
				}
				if (!flag)
				{
					for (int j = 0; j <= this._lastItemIndex; j++)
					{
						if (!this._items[j].HasTarget)
						{
							this._items[j].NewTarget(tag, value);
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					Array.Resize<DbReferenceCollection.CollectionEntry>(ref this._items, this._items.Length * 2);
					this._lastItemIndex++;
					this._items[this._lastItemIndex].NewTarget(tag, value);
				}
				this._optimisticCount++;
			}
		}

		internal T FindItem<T>(int tag, Func<T, bool> filterMethod) where T : class
		{
			bool flag = false;
			try
			{
				this.TryEnterItemLock(ref flag);
				if (flag && this._optimisticCount > 0)
				{
					for (int i = 0; i <= this._lastItemIndex; i++)
					{
						if (this._items[i].Tag == tag)
						{
							object target = this._items[i].Target;
							if (target != null)
							{
								T t = target as T;
								if (t != null && filterMethod(t))
								{
									return t;
								}
							}
						}
					}
				}
			}
			finally
			{
				this.ExitItemLockIfNeeded(flag);
			}
			return default(T);
		}

		public void Notify(int message)
		{
			bool flag = false;
			try
			{
				this.TryEnterItemLock(ref flag);
				if (flag)
				{
					try
					{
						this._isNotifying = true;
						if (this._optimisticCount > 0)
						{
							for (int i = 0; i <= this._lastItemIndex; i++)
							{
								object target = this._items[i].Target;
								if (target != null)
								{
									this.NotifyItem(message, this._items[i].Tag, target);
									this._items[i].RemoveTarget();
								}
							}
							this._optimisticCount = 0;
						}
						if (this._items.Length > 100)
						{
							this._lastItemIndex = 0;
							this._items = new DbReferenceCollection.CollectionEntry[20];
						}
					}
					finally
					{
						this._isNotifying = false;
					}
				}
			}
			finally
			{
				this.ExitItemLockIfNeeded(flag);
			}
		}

		protected abstract void NotifyItem(int message, int tag, object value);

		public abstract void Remove(object value);

		protected void RemoveItem(object value)
		{
			bool flag = false;
			try
			{
				this.TryEnterItemLock(ref flag);
				if (flag && this._optimisticCount > 0)
				{
					for (int i = 0; i <= this._lastItemIndex; i++)
					{
						if (value == this._items[i].Target)
						{
							this._items[i].RemoveTarget();
							this._optimisticCount--;
							break;
						}
					}
				}
			}
			finally
			{
				this.ExitItemLockIfNeeded(flag);
			}
		}

		private void TryEnterItemLock(ref bool lockObtained)
		{
			lockObtained = false;
			while (!this._isNotifying && !lockObtained)
			{
				Monitor.TryEnter(this._itemLock, 100, ref lockObtained);
			}
		}

		private void ExitItemLockIfNeeded(bool lockObtained)
		{
			if (lockObtained)
			{
				Monitor.Exit(this._itemLock);
			}
		}

		private const int LockPollTime = 100;

		private const int DefaultCollectionSize = 20;

		private DbReferenceCollection.CollectionEntry[] _items;

		private readonly object _itemLock;

		private int _optimisticCount;

		private int _lastItemIndex;

		private volatile bool _isNotifying;

		private struct CollectionEntry
		{
			public void NewTarget(int tag, object target)
			{
				if (this._weak == null)
				{
					this._weak = new WeakReference(target, false);
				}
				else
				{
					this._weak.Target = target;
				}
				this._tag = tag;
			}

			public void RemoveTarget()
			{
				this._tag = 0;
			}

			public bool HasTarget
			{
				get
				{
					return this._tag != 0 && this._weak.IsAlive;
				}
			}

			public int Tag
			{
				get
				{
					return this._tag;
				}
			}

			public object Target
			{
				get
				{
					if (this._tag != 0)
					{
						return this._weak.Target;
					}
					return null;
				}
			}

			private int _tag;

			private WeakReference _weak;
		}
	}
}
