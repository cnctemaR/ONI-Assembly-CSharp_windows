using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Serializable]
	public abstract class CollectionBase : IEnumerable, ICollection, IList
	{
		protected CollectionBase()
		{
		}

		protected CollectionBase(int capacity)
		{
			this.list = new ArrayList(capacity);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.InnerList.CopyTo(array, index);
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.InnerList.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.InnerList.IsSynchronized;
			}
		}

		int IList.Add(object value)
		{
			this.OnValidate(value);
			int count = this.InnerList.Count;
			this.OnInsert(count, value);
			this.InnerList.Add(value);
			try
			{
				this.OnInsertComplete(count, value);
			}
			catch
			{
				this.InnerList.RemoveAt(count);
				throw;
			}
			return count;
		}

		bool IList.Contains(object value)
		{
			return this.InnerList.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this.InnerList.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
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

		void IList.Remove(object value)
		{
			this.OnValidate(value);
			int num = this.InnerList.IndexOf(value);
			if (num == -1)
			{
				throw new ArgumentException("The element cannot be found.", "value");
			}
			this.OnRemove(num, value);
			this.InnerList.Remove(value);
			this.OnRemoveComplete(num, value);
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.InnerList.IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.InnerList.IsReadOnly;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.InnerList[index];
			}
			set
			{
				if (index < 0 || index >= this.InnerList.Count)
				{
					throw new ArgumentOutOfRangeException("index");
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

		public int Count
		{
			get
			{
				return this.InnerList.Count;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.InnerList.GetEnumerator();
		}

		public void Clear()
		{
			this.OnClear();
			this.InnerList.Clear();
			this.OnClearComplete();
		}

		public void RemoveAt(int index)
		{
			object obj = this.InnerList[index];
			this.OnValidate(obj);
			this.OnRemove(index, obj);
			this.InnerList.RemoveAt(index);
			this.OnRemoveComplete(index, obj);
		}

		[ComVisible(false)]
		public int Capacity
		{
			get
			{
				if (this.list == null)
				{
					this.list = new ArrayList();
				}
				return this.list.Capacity;
			}
			set
			{
				if (this.list == null)
				{
					this.list = new ArrayList();
				}
				this.list.Capacity = value;
			}
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

		protected virtual void OnClear()
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual void OnInsert(int index, object value)
		{
		}

		protected virtual void OnInsertComplete(int index, object value)
		{
		}

		protected virtual void OnRemove(int index, object value)
		{
		}

		protected virtual void OnRemoveComplete(int index, object value)
		{
		}

		protected virtual void OnSet(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnSetComplete(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnValidate(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("CollectionBase.OnValidate: Invalid parameter value passed to method: null");
			}
		}

		private ArrayList list;
	}
}
