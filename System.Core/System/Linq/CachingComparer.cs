using System;

namespace System.Linq
{
	internal abstract class CachingComparer<TElement>
	{
		internal abstract int Compare(TElement element, bool cacheLower);

		internal abstract void SetElement(TElement element);
	}
}
