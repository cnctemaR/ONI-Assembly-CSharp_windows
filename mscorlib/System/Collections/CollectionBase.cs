using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Serializable]
	public abstract class CollectionBase : IList, ICollection, IEnumerable
	{
		protected CollectionBase()
		{
			this.list = new ArrayList();
		}

		protected CollectionBase(int capacity)
		{
			this.list = new ArrayList(capacity);
		}

		protected ArrayList InnerList
		{
			get
			{
				if (this.list == null)
				{
					this.list = new ArrayList();
				}
				return this.list;
			}
		}

		protected IList List
		{
			get
			{
				return this;
			}
		}

		[ComVisible(false)]
		public int Capacity
		{
			get
			{
				return this.InnerList.Capacity;
			}
			set
			{
				this.InnerList.Capacity = value;
			}
		}

		public int Count
		{
			get
			{
				if (this.list != null)
				{
					return this.list.Count;
				}
				return 0;
			}
		}

		public void Clear()
		{
			this.OnClear();
			this.InnerList.Clear();
			this.OnClearComplete();
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."));
			}
			object obj = this.InnerList[index];
			this.OnValidate(obj);
			this.OnRemove(index, obj);
			this.InnerList.RemoveAt(index);
			try
			{
				this.OnRemoveComplete(index, obj);
			}
			catch
			{
				this.InnerList.Insert(index, obj);
				throw;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.InnerList.IsReadOnly;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.InnerList.IsFixedSize;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.InnerList.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.InnerList.SyncRoot;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.InnerList.CopyTo(array, index);
		}

		object IList.this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."));
				}
				return this.InnerList[index];
			}
			set
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."));
				}
				this.OnValidate(value);
				object obj = this.InnerList[index];
				this.OnSet(index, obj, value);
				this.InnerList[index] = value;
				try
				{
					this.OnSetComplete(index, obj, value);
				}
				catch
				{
					this.InnerList[index] = obj;
					throw;
				}
			}
		}

		bool IList.Contains(object value)
		{
			return this.InnerList.Contains(value);
		}

		int IList.Add(object value)
		{
			this.OnValidate(value);
			this.OnInsert(this.InnerList.Count, value);
			int num = this.InnerList.Add(value);
			try
			{
				this.OnInsertComplete(num, value);
			}
			catch
			{
				this.InnerList.RemoveAt(num);
				throw;
			}
			return num;
		}

		void IList.Remove(object value)
		{
			this.OnValidate(value);
			int num = this.InnerList.IndexOf(value);
			if (num < 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Cannot remove the specified item because it was not found in the specified Collection."));
			}
			this.OnRemove(num, value);
			this.InnerList.RemoveAt(num);
			try
			{
				this.OnRemoveComplete(num, value);
			}
			catch
			{
				this.InnerList.Insert(num, value);
				throw;
			}
		}

		int IList.IndexOf(object value)
		{
			return this.InnerList.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			if (index < 0 || index > this.Count)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Index was out of range. Must be non-negative and less than the size of the collection."));
			}
			this.OnValidate(value);
			this.OnInsert(index, value);
			this.InnerList.Insert(index, value);
			try
			{
				this.OnInsertComplete(index, value);
			}
			catch
			{
				this.InnerList.RemoveAt(index);
				throw;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.InnerList.GetEnumerator();
		}

		protected virtual void OnSet(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnInsert(int index, object value)
		{
		}

		protected virtual void OnClear()
		{
		}

		protected virtual void OnRemove(int index, object value)
		{
		}

		protected virtual void OnValidate(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
		}

		protected virtual void OnSetComplete(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnInsertComplete(int index, object value)
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual void OnRemoveComplete(int index, object value)
		{
		}

		private ArrayList list;
	}
}
