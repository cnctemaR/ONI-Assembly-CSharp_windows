using System;

namespace System.Linq
{
	internal abstract class EnumerableSorter<TElement>
	{
		internal abstract void ComputeKeys(TElement[] elements, int count);

		internal abstract int CompareAnyKeys(int index1, int index2);

		private int[] ComputeMap(TElement[] elements, int count)
		{
			this.ComputeKeys(elements, count);
			int[] array = new int[count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i;
			}
			return array;
		}

		internal int[] Sort(TElement[] elements, int count)
		{
			int[] array = this.ComputeMap(elements, count);
			this.QuickSort(array, 0, count - 1);
			return array;
		}

		internal int[] Sort(TElement[] elements, int count, int minIdx, int maxIdx)
		{
			int[] array = this.ComputeMap(elements, count);
			this.PartialQuickSort(array, 0, count - 1, minIdx, maxIdx);
			return array;
		}

		internal TElement ElementAt(TElement[] elements, int count, int idx)
		{
			return elements[this.QuickSelect(this.ComputeMap(elements, count), count - 1, idx)];
		}

		protected abstract void QuickSort(int[] map, int left, int right);

		protected abstract void PartialQuickSort(int[] map, int left, int right, int minIdx, int maxIdx);

		protected abstract int QuickSelect(int[] map, int right, int idx);
	}
}
