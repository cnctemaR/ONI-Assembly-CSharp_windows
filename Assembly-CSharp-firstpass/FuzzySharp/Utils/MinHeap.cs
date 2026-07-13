using System;
using System.Collections.Generic;

namespace FuzzySharp.Utils
{
	public class MinHeap<T> : Heap<T>
	{
		public MinHeap()
			: this(Comparer<T>.Default)
		{
		}

		public MinHeap(Comparer<T> comparer)
			: base(comparer)
		{
		}

		public MinHeap(IEnumerable<T> collection)
			: base(collection)
		{
		}

		public MinHeap(IEnumerable<T> collection, Comparer<T> comparer)
			: base(collection, comparer)
		{
		}

		protected override bool Dominates(T x, T y)
		{
			return base.Comparer.Compare(x, y) <= 0;
		}
	}
}
