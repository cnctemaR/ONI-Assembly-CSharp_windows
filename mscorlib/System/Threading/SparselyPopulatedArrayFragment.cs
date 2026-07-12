using System;

namespace System.Threading
{
	internal class SparselyPopulatedArrayFragment<T> where T : class
	{
		internal SparselyPopulatedArrayFragment(int size)
			: this(size, null)
		{
		}

		internal SparselyPopulatedArrayFragment(int size, SparselyPopulatedArrayFragment<T> prev)
		{
			this._elements = new T[size];
			this._freeCount = size;
			this._prev = prev;
		}

		internal T this[int index]
		{
			get
			{
				return Volatile.Read<T>(ref this._elements[index]);
			}
		}

		internal int Length
		{
			get
			{
				return this._elements.Length;
			}
		}

		internal SparselyPopulatedArrayFragment<T> Prev
		{
			get
			{
				return this._prev;
			}
		}

		internal T SafeAtomicRemove(int index, T expectedElement)
		{
			T t = Interlocked.CompareExchange<T>(ref this._elements[index], default(T), expectedElement);
			if (t != null)
			{
				this._freeCount++;
			}
			return t;
		}

		internal readonly T[] _elements;

		internal volatile int _freeCount;

		internal volatile SparselyPopulatedArrayFragment<T> _next;

		internal volatile SparselyPopulatedArrayFragment<T> _prev;
	}
}
