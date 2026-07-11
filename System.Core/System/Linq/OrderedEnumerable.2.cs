using System;
using System.Collections.Generic;

namespace System.Linq
{
	internal sealed class OrderedEnumerable<TElement, TKey> : OrderedEnumerable<TElement>
	{
		internal OrderedEnumerable(IEnumerable<TElement> source, Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, OrderedEnumerable<TElement> parent)
		{
			if (source == null)
			{
				throw Error.ArgumentNull("source");
			}
			this._source = source;
			this._parent = parent;
			if (keySelector == null)
			{
				throw Error.ArgumentNull("keySelector");
			}
			this._keySelector = keySelector;
			this._comparer = comparer ?? Comparer<TKey>.Default;
			this._descending = descending;
		}

		internal override EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next)
		{
			EnumerableSorter<TElement> enumerableSorter = new EnumerableSorter<TElement, TKey>(this._keySelector, this._comparer, this._descending, next);
			if (this._parent != null)
			{
				enumerableSorter = this._parent.GetEnumerableSorter(enumerableSorter);
			}
			return enumerableSorter;
		}

		internal override CachingComparer<TElement> GetComparer(CachingComparer<TElement> childComparer)
		{
			CachingComparer<TElement> cachingComparer = ((childComparer == null) ? new CachingComparer<TElement, TKey>(this._keySelector, this._comparer, this._descending) : new CachingComparerWithChild<TElement, TKey>(this._keySelector, this._comparer, this._descending, childComparer));
			if (this._parent == null)
			{
				return cachingComparer;
			}
			return this._parent.GetComparer(cachingComparer);
		}

		private readonly OrderedEnumerable<TElement> _parent;

		private readonly Func<TElement, TKey> _keySelector;

		private readonly IComparer<TKey> _comparer;

		private readonly bool _descending;
	}
}
