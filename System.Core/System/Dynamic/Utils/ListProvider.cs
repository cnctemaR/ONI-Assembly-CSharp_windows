using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
	internal abstract class ListProvider<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : class
	{
		protected abstract T First { get; }

		protected abstract int ElementCount { get; }

		protected abstract T GetElement(int index);

		public int IndexOf(T item)
		{
			if (this.First == item)
			{
				return 0;
			}
			int i = 1;
			int elementCount = this.ElementCount;
			while (i < elementCount)
			{
				if (this.GetElement(i) == item)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		[ExcludeFromCodeCoverage]
		public void Insert(int index, T item)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		public void RemoveAt(int index)
		{
			throw ContractUtils.Unreachable;
		}

		public T this[int index]
		{
			get
			{
				if (index == 0)
				{
					return this.First;
				}
				return this.GetElement(index);
			}
			[ExcludeFromCodeCoverage]
			set
			{
				throw ContractUtils.Unreachable;
			}
		}

		[ExcludeFromCodeCoverage]
		public void Add(T item)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		public void Clear()
		{
			throw ContractUtils.Unreachable;
		}

		public bool Contains(T item)
		{
			return this.IndexOf(item) != -1;
		}

		public void CopyTo(T[] array, int index)
		{
			ContractUtils.RequiresNotNull(array, "array");
			if (index < 0)
			{
				throw Error.ArgumentOutOfRange("index");
			}
			int elementCount = this.ElementCount;
			if (index + elementCount > array.Length)
			{
				throw new ArgumentException();
			}
			array[index++] = this.First;
			for (int i = 1; i < elementCount; i++)
			{
				array[index++] = this.GetElement(i);
			}
		}

		public int Count
		{
			get
			{
				return this.ElementCount;
			}
		}

		[ExcludeFromCodeCoverage]
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		[ExcludeFromCodeCoverage]
		public bool Remove(T item)
		{
			throw ContractUtils.Unreachable;
		}

		public IEnumerator<T> GetEnumerator()
		{
			yield return this.First;
			int i = 1;
			int j = this.ElementCount;
			while (i < j)
			{
				yield return this.GetElement(i);
				int num = i;
				i = num + 1;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
