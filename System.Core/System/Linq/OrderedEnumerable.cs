using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
	internal abstract class OrderedEnumerable<TElement> : IOrderedEnumerable<TElement>, IEnumerable<TElement>, IEnumerable, IPartition<TElement>, IIListProvider<TElement>
	{
		private int[] SortedMap(Buffer<TElement> buffer)
		{
			return this.GetEnumerableSorter().Sort(buffer._items, buffer._count);
		}

		private int[] SortedMap(Buffer<TElement> buffer, int minIdx, int maxIdx)
		{
			return this.GetEnumerableSorter().Sort(buffer._items, buffer._count, minIdx, maxIdx);
		}

		public IEnumerator<TElement> GetEnumerator()
		{
			Buffer<TElement> buffer = new Buffer<TElement>(this._source);
			if (buffer._count > 0)
			{
				int[] map = this.SortedMap(buffer);
				int num;
				for (int i = 0; i < buffer._count; i = num + 1)
				{
					yield return buffer._items[map[i]];
					num = i;
				}
				map = null;
			}
			yield break;
		}

		public TElement[] ToArray()
		{
			Buffer<TElement> buffer = new Buffer<TElement>(this._source);
			int count = buffer._count;
			if (count == 0)
			{
				return buffer._items;
			}
			TElement[] array = new TElement[count];
			int[] array2 = this.SortedMap(buffer);
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = buffer._items[array2[num]];
			}
			return array;
		}

		public List<TElement> ToList()
		{
			Buffer<TElement> buffer = new Buffer<TElement>(this._source);
			int count = buffer._count;
			List<TElement> list = new List<TElement>(count);
			if (count > 0)
			{
				int[] array = this.SortedMap(buffer);
				for (int num = 0; num != count; num++)
				{
					list.Add(buffer._items[array[num]]);
				}
			}
			return list;
		}

		public int GetCount(bool onlyIfCheap)
		{
			IIListProvider<TElement> iilistProvider;
			if ((iilistProvider = this._source as IIListProvider<TElement>) != null)
			{
				return iilistProvider.GetCount(onlyIfCheap);
			}
			if (onlyIfCheap && !(this._source is ICollection<TElement>) && !(this._source is ICollection))
			{
				return -1;
			}
			return this._source.Count<TElement>();
		}

		internal IEnumerator<TElement> GetEnumerator(int minIdx, int maxIdx)
		{
			Buffer<TElement> buffer = new Buffer<TElement>(this._source);
			int count = buffer._count;
			if (count > minIdx)
			{
				if (count <= maxIdx)
				{
					maxIdx = count - 1;
				}
				if (minIdx == maxIdx)
				{
					yield return this.GetEnumerableSorter().ElementAt(buffer._items, count, minIdx);
				}
				else
				{
					int[] map = this.SortedMap(buffer, minIdx, maxIdx);
					while (minIdx <= maxIdx)
					{
						yield return buffer._items[map[minIdx]];
						int num = minIdx + 1;
						minIdx = num;
					}
					map = null;
				}
			}
			yield break;
		}

		internal TElement[] ToArray(int minIdx, int maxIdx)
		{
			Buffer<TElement> buffer = new Buffer<TElement>(this._source);
			int count = buffer._count;
			if (count <= minIdx)
			{
				return Array.Empty<TElement>();
			}
			if (count <= maxIdx)
			{
				maxIdx = count - 1;
			}
			if (minIdx == maxIdx)
			{
				return new TElement[] { this.GetEnumerableSorter().ElementAt(buffer._items, count, minIdx) };
			}
			int[] array = this.SortedMap(buffer, minIdx, maxIdx);
			TElement[] array2 = new TElement[maxIdx - minIdx + 1];
			int num = 0;
			while (minIdx <= maxIdx)
			{
				array2[num] = buffer._items[array[minIdx]];
				num++;
				minIdx++;
			}
			return array2;
		}

		internal List<TElement> ToList(int minIdx, int maxIdx)
		{
			Buffer<TElement> buffer = new Buffer<TElement>(this._source);
			int count = buffer._count;
			if (count <= minIdx)
			{
				return new List<TElement>();
			}
			if (count <= maxIdx)
			{
				maxIdx = count - 1;
			}
			if (minIdx == maxIdx)
			{
				return new List<TElement>(1) { this.GetEnumerableSorter().ElementAt(buffer._items, count, minIdx) };
			}
			int[] array = this.SortedMap(buffer, minIdx, maxIdx);
			List<TElement> list = new List<TElement>(maxIdx - minIdx + 1);
			while (minIdx <= maxIdx)
			{
				list.Add(buffer._items[array[minIdx]]);
				minIdx++;
			}
			return list;
		}

		internal int GetCount(int minIdx, int maxIdx, bool onlyIfCheap)
		{
			int count = this.GetCount(onlyIfCheap);
			if (count <= 0)
			{
				return count;
			}
			if (count <= minIdx)
			{
				return 0;
			}
			return ((count <= maxIdx) ? count : (maxIdx + 1)) - minIdx;
		}

		private EnumerableSorter<TElement> GetEnumerableSorter()
		{
			return this.GetEnumerableSorter(null);
		}

		internal abstract EnumerableSorter<TElement> GetEnumerableSorter(EnumerableSorter<TElement> next);

		private CachingComparer<TElement> GetComparer()
		{
			return this.GetComparer(null);
		}

		internal abstract CachingComparer<TElement> GetComparer(CachingComparer<TElement> childComparer);

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IOrderedEnumerable<TElement> IOrderedEnumerable<TElement>.CreateOrderedEnumerable<TKey>(Func<TElement, TKey> keySelector, IComparer<TKey> comparer, bool descending)
		{
			return new OrderedEnumerable<TElement, TKey>(this._source, keySelector, comparer, descending, this);
		}

		public IPartition<TElement> Skip(int count)
		{
			return new OrderedPartition<TElement>(this, count, int.MaxValue);
		}

		public IPartition<TElement> Take(int count)
		{
			return new OrderedPartition<TElement>(this, 0, count - 1);
		}

		public TElement TryGetElementAt(int index, out bool found)
		{
			if (index == 0)
			{
				return this.TryGetFirst(out found);
			}
			if (index > 0)
			{
				Buffer<TElement> buffer = new Buffer<TElement>(this._source);
				int count = buffer._count;
				if (index < count)
				{
					found = true;
					return this.GetEnumerableSorter().ElementAt(buffer._items, count, index);
				}
			}
			found = false;
			return default(TElement);
		}

		public TElement TryGetFirst(out bool found)
		{
			CachingComparer<TElement> comparer = this.GetComparer();
			TElement telement;
			using (IEnumerator<TElement> enumerator = this._source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					found = false;
					telement = default(TElement);
					telement = telement;
				}
				else
				{
					TElement telement2 = enumerator.Current;
					comparer.SetElement(telement2);
					while (enumerator.MoveNext())
					{
						TElement telement3 = enumerator.Current;
						if (comparer.Compare(telement3, true) < 0)
						{
							telement2 = telement3;
						}
					}
					found = true;
					telement = telement2;
				}
			}
			return telement;
		}

		public TElement TryGetFirst(Func<TElement, bool> predicate, out bool found)
		{
			CachingComparer<TElement> comparer = this.GetComparer();
			TElement telement3;
			using (IEnumerator<TElement> enumerator = this._source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TElement telement = enumerator.Current;
					if (predicate(telement))
					{
						comparer.SetElement(telement);
						while (enumerator.MoveNext())
						{
							TElement telement2 = enumerator.Current;
							if (predicate(telement2) && comparer.Compare(telement2, true) < 0)
							{
								telement = telement2;
							}
						}
						found = true;
						return telement;
					}
				}
				found = false;
				telement3 = default(TElement);
				telement3 = telement3;
			}
			return telement3;
		}

		public TElement TryGetLast(out bool found)
		{
			TElement telement;
			using (IEnumerator<TElement> enumerator = this._source.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					found = false;
					telement = default(TElement);
					telement = telement;
				}
				else
				{
					CachingComparer<TElement> comparer = this.GetComparer();
					TElement telement2 = enumerator.Current;
					comparer.SetElement(telement2);
					while (enumerator.MoveNext())
					{
						TElement telement3 = enumerator.Current;
						if (comparer.Compare(telement3, false) >= 0)
						{
							telement2 = telement3;
						}
					}
					found = true;
					telement = telement2;
				}
			}
			return telement;
		}

		public TElement TryGetLast(int minIdx, int maxIdx, out bool found)
		{
			Buffer<TElement> buffer = new Buffer<TElement>(this._source);
			int count = buffer._count;
			if (minIdx >= count)
			{
				found = false;
				return default(TElement);
			}
			found = true;
			if (maxIdx >= count - 1)
			{
				return this.Last(buffer);
			}
			return this.GetEnumerableSorter().ElementAt(buffer._items, count, maxIdx);
		}

		private TElement Last(Buffer<TElement> buffer)
		{
			CachingComparer<TElement> comparer = this.GetComparer();
			TElement[] items = buffer._items;
			int count = buffer._count;
			TElement telement = items[0];
			comparer.SetElement(telement);
			for (int num = 1; num != count; num++)
			{
				TElement telement2 = items[num];
				if (comparer.Compare(telement2, false) >= 0)
				{
					telement = telement2;
				}
			}
			return telement;
		}

		public TElement TryGetLast(Func<TElement, bool> predicate, out bool found)
		{
			CachingComparer<TElement> comparer = this.GetComparer();
			TElement telement3;
			using (IEnumerator<TElement> enumerator = this._source.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TElement telement = enumerator.Current;
					if (predicate(telement))
					{
						comparer.SetElement(telement);
						while (enumerator.MoveNext())
						{
							TElement telement2 = enumerator.Current;
							if (predicate(telement2) && comparer.Compare(telement2, false) >= 0)
							{
								telement = telement2;
							}
						}
						found = true;
						return telement;
					}
				}
				found = false;
				telement3 = default(TElement);
				telement3 = telement3;
			}
			return telement3;
		}

		internal IEnumerable<TElement> _source;
	}
}
