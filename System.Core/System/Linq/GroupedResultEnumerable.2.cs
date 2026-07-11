using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	internal sealed class GroupedResultEnumerable<TSource, TKey, TResult> : IIListProvider<TResult>, IEnumerable<TResult>, IEnumerable
	{
		public GroupedResultEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
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
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			this._resultSelector = resultSelector;
			this._comparer = comparer;
		}

		public IEnumerator<TResult> GetEnumerator()
		{
			return Lookup<TKey, TSource>.Create(this._source, this._keySelector, this._comparer).ApplyResultSelector<TResult>(this._resultSelector).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public TResult[] ToArray()
		{
			return Lookup<TKey, TSource>.Create(this._source, this._keySelector, this._comparer).ToArray<TResult>(this._resultSelector);
		}

		public List<TResult> ToList()
		{
			return Lookup<TKey, TSource>.Create(this._source, this._keySelector, this._comparer).ToList<TResult>(this._resultSelector);
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

		private readonly Func<TKey, IEnumerable<TSource>, TResult> _resultSelector;
	}
}
