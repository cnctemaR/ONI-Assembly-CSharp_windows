using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class GroupByGrouping<TGroupKey, TElement> : IGrouping<TGroupKey, TElement>, IEnumerable<TElement>, IEnumerable
	{
		internal GroupByGrouping(KeyValuePair<Wrapper<TGroupKey>, ListChunk<TElement>> keyValues)
		{
			this._keyValues = keyValues;
		}

		TGroupKey IGrouping<TGroupKey, TElement>.Key
		{
			get
			{
				return this._keyValues.Key.Value;
			}
		}

		IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
		{
			return this._keyValues.Value.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TElement>)this).GetEnumerator();
		}

		private KeyValuePair<Wrapper<TGroupKey>, ListChunk<TElement>> _keyValues;
	}
}
