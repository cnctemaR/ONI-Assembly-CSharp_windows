using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	internal sealed class GroupedResultEnumerable<TSource, TKey, TElement, TResult> : IIListProvider<TResult>, IEnumerable<TResult>, IEnumerable
	{
		public GroupedResultEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
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
			if (elementSelector == null)
			{
				throw Error.ArgumentNull("elementSelector");
			}
			this._elementSelector = elementSelector;
			this._comparer = comparer;
			if (resultSelector == null)
			{
				throw Error.ArgumentNull("resultSelector");
			}
			this._resultSelector = resultSelector;
		}

		public IEnumerator<TResult> GetEnumerator()
		{
			return Lookup<TKey, TElement>.Create<TSource>(this._source, this._keySelector, this._elementSelector, this._comparer).ApplyResultSelector<TResult>(this._resultSelector).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public TResult[] ToArray()
		{
			return Lookup<TKey, TElement>.Create<TSource>(this._source, this._keySelector, this._elementSelector, this._comparer).ToArray<TResult>(this._resultSelector);
		}

		public List<TResult> ToList()
		{
			return Lookup<TKey, TElement>.Create<TSource>(this._source, this._keySelector, this._elementSelector, this._comparer).ToList<TResult>(this._resultSelector);
		}

		public int GetCount(bool onlyIfCheap)
		{
			if (!onlyIfCheap)
			{
				return Lookup<TKey, TElement>.Create<TSource>(this._source, this._keySelector, this._elementSelector, this._comparer).Count;
			}
			return -1;
		}

		private readonly IEnumerable<TSource> _source;

		private readonly Func<TSource, TKey> _keySelector;

		private readonly Func<TSource, TElement> _elementSelector;

		private readonly IEqualityComparer<TKey> _comparer;

		private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;
	}
}
