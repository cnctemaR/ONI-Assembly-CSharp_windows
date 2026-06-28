using System;
using System.Collections.Generic;

namespace Satsuma
{
	public sealed class DisjointSet<T> : IDisjointSet<T>, IReadOnlyDisjointSet<T>, IClearable
	{
		public DisjointSet()
		{
			this.parent = new Dictionary<T, T>();
			this.next = new Dictionary<T, T>();
			this.last = new Dictionary<T, T>();
			this.tmpList = new List<T>();
		}

		public void Clear()
		{
			this.parent.Clear();
			this.next.Clear();
			this.last.Clear();
		}

		public DisjointSetSet<T> WhereIs(T element)
		{
			T t;
			while (this.parent.TryGetValue(element, out t))
			{
				this.tmpList.Add(element);
				element = t;
			}
			foreach (T t2 in this.tmpList)
			{
				this.parent[t2] = element;
			}
			this.tmpList.Clear();
			return new DisjointSetSet<T>(element);
		}

		private T GetLast(T x)
		{
			T t;
			if (this.last.TryGetValue(x, out t))
			{
				return t;
			}
			return x;
		}

		public DisjointSetSet<T> Union(DisjointSetSet<T> a, DisjointSetSet<T> b)
		{
			T representative = a.Representative;
			T representative2 = b.Representative;
			if (!representative.Equals(representative2))
			{
				this.parent[representative] = representative2;
				this.next[this.GetLast(representative2)] = representative;
				this.last[representative2] = this.GetLast(representative);
			}
			return b;
		}

		public IEnumerable<T> Elements(DisjointSetSet<T> aSet)
		{
			T element = aSet.Representative;
			do
			{
				yield return element;
			}
			while (this.next.TryGetValue(element, out element));
			yield break;
		}

		private readonly Dictionary<T, T> parent;

		private readonly Dictionary<T, T> next;

		private readonly Dictionary<T, T> last;

		private readonly List<T> tmpList;
	}
}
