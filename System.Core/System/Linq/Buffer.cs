using System;
using System.Collections.Generic;

namespace System.Linq
{
	internal readonly struct Buffer<TElement>
	{
		internal Buffer(IEnumerable<TElement> source)
		{
			IIListProvider<TElement> iilistProvider = source as IIListProvider<TElement>;
			if (iilistProvider != null)
			{
				TElement[] array = iilistProvider.ToArray();
				this._items = array;
				this._count = array.Length;
				return;
			}
			this._items = global::System.Collections.Generic.EnumerableHelpers.ToArray<TElement>(source, out this._count);
		}

		internal readonly TElement[] _items;

		internal readonly int _count;
	}
}
