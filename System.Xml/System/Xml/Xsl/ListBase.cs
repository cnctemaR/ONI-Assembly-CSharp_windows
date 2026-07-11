using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Xsl
{
	internal abstract class ListBase<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection
	{
		public abstract int Count { get; }

		public abstract T this[int index] { get; set; }

		public virtual bool Contains(T value)
		{
			return this.IndexOf(value) != -1;
		}

		public virtual int IndexOf(T value)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (value.Equals(this[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public virtual void CopyTo(T[] array, int index)
		{
			for (int i = 0; i < this.Count; i++)
			{
				array[index + i] = this[i];
			}
		}

		public virtual IListEnumerator<T> GetEnumerator()
		{
			return new IListEnumerator<T>(this);
		}

		public virtual bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public virtual void Add(T value)
		{
			this.Insert(this.Count, value);
		}

		public virtual void Insert(int index, T value)
		{
			throw new NotSupportedException();
		}

		public virtual bool Remove(T value)
		{
			int num = this.IndexOf(value);
			if (num >= 0)
			{
				this.RemoveAt(num);
				return true;
			}
			return false;
		}

		public virtual void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public virtual void Clear()
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				this.RemoveAt(i);
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new IListEnumerator<T>(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new IListEnumerator<T>(this);
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.IsReadOnly;
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
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index);
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!ListBase<T>.IsCompatibleType(value.GetType()))
				{
					throw new ArgumentException(Res.GetString("Type is incompatible."), "value");
				}
				this[index] = (T)((object)value);
			}
		}

		int IList.Add(object value)
		{
			if (!ListBase<T>.IsCompatibleType(value.GetType()))
			{
				throw new ArgumentException(Res.GetString("Type is incompatible."), "value");
			}
			this.Add((T)((object)value));
			return this.Count - 1;
		}

		void IList.Clear()
		{
			this.Clear();
		}

		bool IList.Contains(object value)
		{
			return ListBase<T>.IsCompatibleType(value.GetType()) && this.Contains((T)((object)value));
		}

		int IList.IndexOf(object value)
		{
			if (!ListBase<T>.IsCompatibleType(value.GetType()))
			{
				return -1;
			}
			return this.IndexOf((T)((object)value));
		}

		void IList.Insert(int index, object value)
		{
			if (!ListBase<T>.IsCompatibleType(value.GetType()))
			{
				throw new ArgumentException(Res.GetString("Type is incompatible."), "value");
			}
			this.Insert(index, (T)((object)value));
		}

		void IList.Remove(object value)
		{
			if (ListBase<T>.IsCompatibleType(value.GetType()))
			{
				this.Remove((T)((object)value));
			}
		}

		private static bool IsCompatibleType(object value)
		{
			return (value == null && !typeof(T).IsValueType) || value is T;
		}
	}
}
