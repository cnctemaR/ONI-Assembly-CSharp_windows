using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class XmlQuerySequence<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
	{
		public static XmlQuerySequence<T> CreateOrReuse(XmlQuerySequence<T> seq)
		{
			if (seq != null)
			{
				seq.Clear();
				return seq;
			}
			return new XmlQuerySequence<T>();
		}

		public static XmlQuerySequence<T> CreateOrReuse(XmlQuerySequence<T> seq, T item)
		{
			if (seq != null)
			{
				seq.Clear();
				seq.Add(item);
				return seq;
			}
			return new XmlQuerySequence<T>(item);
		}

		public XmlQuerySequence()
		{
			this.items = new T[16];
		}

		public XmlQuerySequence(int capacity)
		{
			this.items = new T[capacity];
		}

		public XmlQuerySequence(T[] array, int size)
		{
			this.items = array;
			this.size = size;
		}

		public XmlQuerySequence(T value)
		{
			this.items = new T[1];
			this.items[0] = value;
			this.size = 1;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new IListEnumerator<T>(this);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new IListEnumerator<T>(this);
		}

		public int Count
		{
			get
			{
				return this.size;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (this.size == 0)
			{
				return;
			}
			Array.Copy(this.items, 0, array, index, this.size);
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<T>.Add(T value)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(T value)
		{
			return this.IndexOf(value) != -1;
		}

		public void CopyTo(T[] array, int index)
		{
			for (int i = 0; i < this.Count; i++)
			{
				array[index + i] = this[i];
			}
		}

		bool ICollection<T>.Remove(T value)
		{
			throw new NotSupportedException();
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		object IList.this[int index]
		{
			get
			{
				if (index >= this.size)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.items[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		bool IList.Contains(object value)
		{
			return this.Contains((T)((object)value));
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((T)((object)value));
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public T this[int index]
		{
			get
			{
				if (index >= this.size)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.items[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int IndexOf(T value)
		{
			int num = Array.IndexOf<T>(this.items, value);
			if (num >= this.size)
			{
				return -1;
			}
			return num;
		}

		void IList<T>.Insert(int index, T value)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			this.size = 0;
			this.OnItemsChanged();
		}

		public void Add(T value)
		{
			this.EnsureCache();
			T[] array = this.items;
			int num = this.size;
			this.size = num + 1;
			array[num] = value;
			this.OnItemsChanged();
		}

		public void SortByKeys(Array keys)
		{
			if (this.size <= 1)
			{
				return;
			}
			Array.Sort(keys, this.items, 0, this.size);
			this.OnItemsChanged();
		}

		private void EnsureCache()
		{
			if (this.size >= this.items.Length)
			{
				T[] array = new T[this.size * 2];
				this.CopyTo(array, 0);
				this.items = array;
			}
		}

		protected virtual void OnItemsChanged()
		{
		}

		public static readonly XmlQuerySequence<T> Empty = new XmlQuerySequence<T>();

		private static readonly Type XPathItemType = typeof(XPathItem);

		private T[] items;

		private int size;

		private const int DefaultCacheSize = 16;
	}
}
