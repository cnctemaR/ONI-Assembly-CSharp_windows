using System;
using System.Collections.Generic;

namespace Microsoft.Internal.Collections
{
	internal class WeakReferenceCollection<T> where T : class
	{
		public void Add(T item)
		{
			if (this._items.Capacity == this._items.Count)
			{
				this.CleanupDeadReferences();
			}
			this._items.Add(new WeakReference(item));
		}

		public void Remove(T item)
		{
			int num = this.IndexOf(item);
			if (num != -1)
			{
				this._items.RemoveAt(num);
			}
		}

		public bool Contains(T item)
		{
			return this.IndexOf(item) >= 0;
		}

		public void Clear()
		{
			this._items.Clear();
		}

		private int IndexOf(T item)
		{
			int count = this._items.Count;
			for (int i = 0; i < count; i++)
			{
				if (this._items[i].Target == item)
				{
					return i;
				}
			}
			return -1;
		}

		private void CleanupDeadReferences()
		{
			this._items.RemoveAll((WeakReference w) => !w.IsAlive);
		}

		public List<T> AliveItemsToList()
		{
			List<T> list = new List<T>();
			foreach (WeakReference weakReference in this._items)
			{
				T t = weakReference.Target as T;
				if (t != null)
				{
					list.Add(t);
				}
			}
			return list;
		}

		private readonly List<WeakReference> _items = new List<WeakReference>();
	}
}
