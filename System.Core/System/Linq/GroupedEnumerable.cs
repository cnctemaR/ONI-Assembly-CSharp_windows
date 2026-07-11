using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	internal sealed class GroupedEnumerable<TSource, TKey, TElement> : IIListProvider<IGrouping<TKey, TElement>>, IEnumerable<IGrouping<TKey, TElement>>, IEnumerable
	{
		public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
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
		}

		public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
		{
			return Lookup<TKey, TElement>.Create<TSource>(this._source, this._keySelector, this._elementSelector, this._comparer).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IGrouping<TKey, TElement>[] ToArray()
		{
			return ((IIListProvider<IGrouping<TKey, TElement>>)Lookup<TKey, TElement>.Create<TSource>(this._source, this._keySelector, this._elementSelector, this._comparer)).ToArray();
		}

		public List<IGrouping<TKey, TElement>> ToList()
		{
			return ((IIListProvider<IGrouping<TKey, TElement>>)Lookup<TKey, TElement>.Create<TSource>(this._source, this._keySelector, this._elementSelector, this._comparer)).ToList();
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
	}
}
