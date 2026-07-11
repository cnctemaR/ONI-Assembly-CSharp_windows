using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
	internal sealed class EmptyPartition<TElement> : IPartition<TElement>, IIListProvider<TElement>, IEnumerable<TElement>, IEnumerable, IEnumerator<TElement>, IDisposable, IEnumerator
	{
		private EmptyPartition()
		{
		}

		public IEnumerator<TElement> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		public bool MoveNext()
		{
			return false;
		}

		[ExcludeFromCodeCoverage]
		public TElement Current
		{
			get
			{
				return default(TElement);
			}
		}

		[ExcludeFromCodeCoverage]
		object IEnumerator.Current
		{
			get
			{
				return default(TElement);
			}
		}

		void IEnumerator.Reset()
		{
			throw Error.NotSupported();
		}

		void IDisposable.Dispose()
		{
		}

		public IPartition<TElement> Skip(int count)
		{
			return this;
		}

		public IPartition<TElement> Take(int count)
		{
			return this;
		}

		public TElement TryGetElementAt(int index, out bool found)
		{
			found = false;
			return default(TElement);
		}

		public TElement TryGetFirst(out bool found)
		{
			found = false;
			return default(TElement);
		}

		public TElement TryGetLast(out bool found)
		{
			found = false;
			return default(TElement);
		}

		public TElement[] ToArray()
		{
			return Array.Empty<TElement>();
		}

		public List<TElement> ToList()
		{
			return new List<TElement>();
		}

		public int GetCount(bool onlyIfCheap)
		{
			return 0;
		}

		public static readonly IPartition<TElement> Instance = new EmptyPartition<TElement>();
	}
}
