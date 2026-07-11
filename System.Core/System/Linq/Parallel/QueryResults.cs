using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal abstract class QueryResults<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		internal abstract void GivePartitionedStream(IPartitionedStreamRecipient<T> recipient);

		internal virtual bool IsIndexible
		{
			get
			{
				return false;
			}
		}

		internal virtual T GetElement(int index)
		{
			throw new NotSupportedException();
		}

		internal virtual int ElementsCount
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		int IList<T>.IndexOf(T item)
		{
			throw new NotSupportedException();
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public T this[int index]
		{
			get
			{
				return this.GetElement(index);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.Contains(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				return this.ElementsCount;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			int num;
			for (int index = 0; index < this.Count; index = num + 1)
			{
				yield return this[index];
				num = index;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}
	}
}
