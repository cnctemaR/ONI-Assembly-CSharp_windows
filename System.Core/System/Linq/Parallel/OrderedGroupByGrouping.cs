using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class OrderedGroupByGrouping<TGroupKey, TOrderKey, TElement> : IGrouping<TGroupKey, TElement>, IEnumerable<TElement>, IEnumerable
	{
		internal OrderedGroupByGrouping(TGroupKey groupKey, IComparer<TOrderKey> orderComparer)
		{
			this._groupKey = groupKey;
			this._values = new GrowingArray<TElement>();
			this._orderKeys = new GrowingArray<TOrderKey>();
			this._orderComparer = orderComparer;
			this._wrappedComparer = new OrderedGroupByGrouping<TGroupKey, TOrderKey, TElement>.KeyAndValuesComparer(this._orderComparer);
		}

		TGroupKey IGrouping<TGroupKey, TElement>.Key
		{
			get
			{
				return this._groupKey;
			}
		}

		IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
		{
			int valueCount = this._values.Count;
			TElement[] valueArray = this._values.InternalArray;
			int num;
			for (int i = 0; i < valueCount; i = num + 1)
			{
				yield return valueArray[i];
				num = i;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TElement>)this).GetEnumerator();
		}

		internal void Add(TElement value, TOrderKey orderKey)
		{
			this._values.Add(value);
			this._orderKeys.Add(orderKey);
		}

		internal void DoneAdding()
		{
			List<KeyValuePair<TOrderKey, TElement>> list = new List<KeyValuePair<TOrderKey, TElement>>();
			for (int i = 0; i < this._orderKeys.InternalArray.Length; i++)
			{
				list.Add(new KeyValuePair<TOrderKey, TElement>(this._orderKeys.InternalArray[i], this._values.InternalArray[i]));
			}
			list.Sort(0, this._values.Count, this._wrappedComparer);
			for (int j = 0; j < this._values.InternalArray.Length; j++)
			{
				this._orderKeys.InternalArray[j] = list[j].Key;
				this._values.InternalArray[j] = list[j].Value;
			}
		}

		private TGroupKey _groupKey;

		private GrowingArray<TElement> _values;

		private GrowingArray<TOrderKey> _orderKeys;

		private IComparer<TOrderKey> _orderComparer;

		private OrderedGroupByGrouping<TGroupKey, TOrderKey, TElement>.KeyAndValuesComparer _wrappedComparer;

		private class KeyAndValuesComparer : IComparer<KeyValuePair<TOrderKey, TElement>>
		{
			public KeyAndValuesComparer(IComparer<TOrderKey> comparer)
			{
				this.myComparer = comparer;
			}

			public int Compare(KeyValuePair<TOrderKey, TElement> x, KeyValuePair<TOrderKey, TElement> y)
			{
				return this.myComparer.Compare(x.Key, y.Key);
			}

			private IComparer<TOrderKey> myComparer;
		}
	}
}
