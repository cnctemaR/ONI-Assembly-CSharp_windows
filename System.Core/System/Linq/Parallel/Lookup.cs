using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class Lookup<TKey, TElement> : ILookup<TKey, TElement>, IEnumerable<IGrouping<TKey, TElement>>, IEnumerable
	{
		internal Lookup(IEqualityComparer<TKey> comparer)
		{
			this._comparer = comparer;
			this._dict = new Dictionary<TKey, IGrouping<TKey, TElement>>(this._comparer);
		}

		public int Count
		{
			get
			{
				int num = this._dict.Count;
				if (this._defaultKeyGrouping != null)
				{
					num++;
				}
				return num;
			}
		}

		public IEnumerable<TElement> this[TKey key]
		{
			get
			{
				if (this._comparer.Equals(key, default(TKey)))
				{
					if (this._defaultKeyGrouping != null)
					{
						return this._defaultKeyGrouping;
					}
					return Enumerable.Empty<TElement>();
				}
				else
				{
					IGrouping<TKey, TElement> grouping;
					if (this._dict.TryGetValue(key, out grouping))
					{
						return grouping;
					}
					return Enumerable.Empty<TElement>();
				}
			}
		}

		public bool Contains(TKey key)
		{
			if (this._comparer.Equals(key, default(TKey)))
			{
				return this._defaultKeyGrouping != null;
			}
			return this._dict.ContainsKey(key);
		}

		internal void Add(IGrouping<TKey, TElement> grouping)
		{
			if (this._comparer.Equals(grouping.Key, default(TKey)))
			{
				this._defaultKeyGrouping = grouping;
				return;
			}
			this._dict.Add(grouping.Key, grouping);
		}

		public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
		{
			foreach (IGrouping<TKey, TElement> grouping in this._dict.Values)
			{
				yield return grouping;
			}
			IEnumerator<IGrouping<TKey, TElement>> enumerator = null;
			if (this._defaultKeyGrouping != null)
			{
				yield return this._defaultKeyGrouping;
			}
			yield break;
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<IGrouping<TKey, TElement>>)this).GetEnumerator();
		}

		private IDictionary<TKey, IGrouping<TKey, TElement>> _dict;

		private IEqualityComparer<TKey> _comparer;

		private IGrouping<TKey, TElement> _defaultKeyGrouping;
	}
}
