using System;
using System.Collections.Generic;

namespace System.Linq
{
	internal class CachingComparer<TElement, TKey> : CachingComparer<TElement>
	{
		public CachingComparer(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
		{
			this._keySelector = keySelector;
			this._comparer = comparer;
			this._descending = descending;
		}

		internal override int Compare(TElement element, bool cacheLower)
		{
			TKey tkey = this._keySelector(element);
			int num = (this._descending ? this._comparer.Compare(this._lastKey, tkey) : this._comparer.Compare(tkey, this._lastKey));
			if (cacheLower == num < 0)
			{
				this._lastKey = tkey;
			}
			return num;
		}

		internal override void SetElement(TElement element)
		{
			this._lastKey = this._keySelector(element);
		}

		protected readonly Func<TElement, TKey> _keySelector;

		protected readonly IComparer<TKey> _comparer;

		protected readonly bool _descending;

		protected TKey _lastKey;
	}
}
