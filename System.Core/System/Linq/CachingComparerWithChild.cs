using System;
using System.Collections.Generic;

namespace System.Linq
{
	internal sealed class CachingComparerWithChild<TElement, TKey> : CachingComparer<TElement, TKey>
	{
		public CachingComparerWithChild(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending, CachingComparer<TElement> child)
			: base(keySelector, comparer, descending)
		{
			this._child = child;
		}

		internal override int Compare(TElement element, bool cacheLower)
		{
			TKey tkey = this._keySelector(element);
			int num = (this._descending ? this._comparer.Compare(this._lastKey, tkey) : this._comparer.Compare(tkey, this._lastKey));
			if (num == 0)
			{
				return this._child.Compare(element, cacheLower);
			}
			if (cacheLower == num < 0)
			{
				this._lastKey = tkey;
				this._child.SetElement(element);
			}
			return num;
		}

		internal override void SetElement(TElement element)
		{
			base.SetElement(element);
			this._child.SetElement(element);
		}

		private readonly CachingComparer<TElement> _child;
	}
}
