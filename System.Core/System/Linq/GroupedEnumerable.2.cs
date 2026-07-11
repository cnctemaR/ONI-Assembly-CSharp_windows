using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	internal sealed class GroupedEnumerable<TSource, TKey> : IIListProvider<IGrouping<TKey, TSource>>, IEnumerable<IGrouping<TKey, TSource>>, IEnumerable
	{
		public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			this._source = source;
			if (keySelector == null)
			{
				throw Error.ArgumentNull("keySelector");
			}
			this._keySelector = keySelector;
			this._comparer = comparer;
		}

		public IEnumerator<IGrouping<TKey, TSource>> GetEnumerator()
		{
			return Lookup<TKey, TSource>.Create(this._source, this._keySelector, this._comparer).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IGrouping<TKey, TSource>[] ToArray()
		{
			return ((IIListProvider<IGrouping<TKey, TSource>>)Lookup<TKey, TSource>.Create(this._source, this._keySelector, this._comparer)).ToArray();
		}

		public List<IGrouping<TKey, TSource>> ToList()
		{
			return ((IIListProvider<IGrouping<TKey, TSource>>)Lookup<TKey, TSource>.Create(this._source, this._keySelector, this._comparer)).ToList();
		}

		public int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				return Lookup<TKey, TSource>.Create(this._source, this._keySelector, this._comparer).Count;
			}
			return -1;
		}

		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, TKey> _keySelector;

		private readonly IEqualityComparer<TKey> _comparer;
	}
}
