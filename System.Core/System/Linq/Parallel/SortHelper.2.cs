using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class SortHelper<TInputOutput, TKey> : SortHelper<TInputOutput>, IDisposable
	{
		private SortHelper(QueryOperatorEnumerator<TInputOutput, TKey> source, int partitionCount, int partitionIndex, QueryTaskGroupState groupState, int[][] sharedIndices, OrdinalIndexState indexState, IComparer<TKey> keyComparer, GrowingArray<TKey>[] sharedkeys, TInputOutput[][] sharedValues, Barrier[][] sharedBarriers)
		{
			this._source = source;
			this._partitionCount = partitionCount;
			this._partitionIndex = partitionIndex;
			this._groupState = groupState;
			this._sharedIndices = sharedIndices;
			this._indexState = indexState;
			this._keyComparer = keyComparer;
			this._sharedKeys = sharedkeys;
			this._sharedValues = sharedValues;
			this._sharedBarriers = sharedBarriers;
		}

		internal static SortHelper<TInputOutput, TKey>[] GenerateSortHelpers(PartitionedStream<TInputOutput, TKey> partitions, QueryTaskGroupState groupState)
		{
			int partitionCount = partitions.PartitionCount;
			SortHelper<TInputOutput, TKey>[] array = new SortHelper<TInputOutput, TKey>[partitionCount];
			int i = 1;
			int num = 0;
			while (i < partitionCount)
			{
				num++;
				i <<= 1;
			}
			int[][] array2 = new int[partitionCount][];
			GrowingArray<TKey>[] array3 = new GrowingArray<TKey>[partitionCount];
			TInputOutput[][] array4 = new TInputOutput[partitionCount][];
			Barrier[][] array5 = JaggedArray<Barrier>.Allocate(num, partitionCount);
			if (partitionCount > 1)
			{
				int num2 = 1;
				for (int j = 0; j < array5.Length; j++)
				{
					for (int k = 0; k < array5[j].Length; k++)
					{
						if (k % num2 == 0)
						{
							array5[j][k] = new Barrier(2);
						}
					}
					num2 *= 2;
				}
			}
			for (int l = 0; l < partitionCount; l++)
			{
				array[l] = new SortHelper<TInputOutput, TKey>(partitions[l], partitionCount, l, groupState, array2, partitions.OrdinalIndexState, partitions.KeyComparer, array3, array4, array5);
			}
			return array;
		}

		public void Dispose()
		{
			if (this._partitionIndex == 0)
			{
				for (int i = 0; i < this._sharedBarriers.Length; i++)
				{
					for (int j = 0; j < this._sharedBarriers[i].Length; j++)
					{
						Barrier barrier = this._sharedBarriers[i][j];
						if (barrier != null)
						{
							barrier.Dispose();
						}
					}
				}
			}
		}

		internal override TInputOutput[] Sort()
		{
			GrowingArray<TKey> growingArray = null;
			List<TInputOutput> list = null;
			this.BuildKeysFromSource(ref growingArray, ref list);
			this.QuickSortIndicesInPlace(growingArray, list, this._indexState);
			if (this._partitionCount > 1)
			{
				this.MergeSortCooperatively();
			}
			return this._sharedValues[this._partitionIndex];
		}

		private void BuildKeysFromSource(ref GrowingArray<TKey> keys, ref List<TInputOutput> values)
		{
			values = new List<TInputOutput>();
			CancellationToken mergedCancellationToken = this._groupState.CancellationState.MergedCancellationToken;
			try
			{
				TInputOutput tinputOutput = default(TInputOutput);
				TKey tkey = default(TKey);
				bool flag = this._source.MoveNext(ref tinputOutput, ref tkey);
				if (keys == null)
				{
					keys = new GrowingArray<TKey>();
				}
				if (flag)
				{
					int num = 0;
					do
					{
						if ((num++ & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(mergedCancellationToken);
						}
						keys.Add(tkey);
						values.Add(tinputOutput);
					}
					while (this._source.MoveNext(ref tinputOutput, ref tkey));
				}
			}
			finally
			{
				this._source.Dispose();
			}
		}

		private void QuickSortIndicesInPlace(GrowingArray<TKey> keys, List<TInputOutput> values, OrdinalIndexState ordinalIndexState)
		{
			int[] array = new int[values.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i;
			}
			if (array.Length > 1 && ordinalIndexState.IsWorseThan(OrdinalIndexState.Increasing))
			{
				this.QuickSort(0, array.Length - 1, keys.InternalArray, array, this._groupState.CancellationState.MergedCancellationToken);
			}
			if (this._partitionCount == 1)
			{
				TInputOutput[] array2 = new TInputOutput[values.Count];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = values[array[j]];
				}
				this._sharedValues[this._partitionIndex] = array2;
				return;
			}
			this._sharedIndices[this._partitionIndex] = array;
			this._sharedKeys[this._partitionIndex] = keys;
			this._sharedValues[this._partitionIndex] = new TInputOutput[values.Count];
			values.CopyTo(this._sharedValues[this._partitionIndex]);
		}

		private void MergeSortCooperatively()
		{
			CancellationToken mergedCancellationToken = this._groupState.CancellationState.MergedCancellationToken;
			int num = this._sharedBarriers.Length;
			for (int i = 0; i < num; i++)
			{
				bool flag = i == num - 1;
				int num2 = this.ComputePartnerIndex(i);
				if (num2 < this._partitionCount)
				{
					int[] array = this._sharedIndices[this._partitionIndex];
					GrowingArray<TKey> growingArray = this._sharedKeys[this._partitionIndex];
					TKey[] internalArray = growingArray.InternalArray;
					TInputOutput[] array2 = this._sharedValues[this._partitionIndex];
					this._sharedBarriers[i][Math.Min(this._partitionIndex, num2)].SignalAndWait(mergedCancellationToken);
					if (this._partitionIndex >= num2)
					{
						this._sharedBarriers[i][num2].SignalAndWait(mergedCancellationToken);
						int[] array3 = this._sharedIndices[this._partitionIndex];
						TKey[] internalArray2 = this._sharedKeys[this._partitionIndex].InternalArray;
						TInputOutput[] array4 = this._sharedValues[this._partitionIndex];
						int[] array5 = this._sharedIndices[num2];
						GrowingArray<TKey> growingArray2 = this._sharedKeys[num2];
						TInputOutput[] array6 = this._sharedValues[num2];
						int num3 = array4.Length;
						int num4 = array2.Length;
						int num5 = num3 + num4;
						int num6 = (num5 + 1) / 2;
						int j = num5 - 1;
						int num7 = num3 - 1;
						int num8 = num4 - 1;
						while (j >= num6)
						{
							if ((j & 63) == 0)
							{
								CancellationState.ThrowIfCanceled(mergedCancellationToken);
							}
							if (num7 >= 0 && (num8 < 0 || this._keyComparer.Compare(internalArray2[array3[num7]], internalArray[array[num8]]) > 0))
							{
								if (flag)
								{
									array6[j] = array4[array3[num7]];
								}
								else
								{
									array5[j] = array3[num7];
								}
								num7--;
							}
							else
							{
								if (flag)
								{
									array6[j] = array2[array[num8]];
								}
								else
								{
									array5[j] = num3 + array[num8];
								}
								num8--;
							}
							j--;
						}
						if (!flag && array2.Length != 0)
						{
							growingArray2.CopyFrom(internalArray, array2.Length);
							Array.Copy(array2, 0, array6, num3, array2.Length);
						}
						this._sharedBarriers[i][num2].SignalAndWait(mergedCancellationToken);
						return;
					}
					int[] array7 = this._sharedIndices[num2];
					TKey[] internalArray3 = this._sharedKeys[num2].InternalArray;
					TInputOutput[] array8 = this._sharedValues[num2];
					this._sharedIndices[num2] = array;
					this._sharedKeys[num2] = growingArray;
					this._sharedValues[num2] = array2;
					int num9 = array2.Length;
					int num10 = array8.Length;
					int num11 = num9 + num10;
					int[] array9 = null;
					TInputOutput[] array10 = new TInputOutput[num11];
					if (!flag)
					{
						array9 = new int[num11];
					}
					this._sharedIndices[this._partitionIndex] = array9;
					this._sharedKeys[this._partitionIndex] = growingArray;
					this._sharedValues[this._partitionIndex] = array10;
					this._sharedBarriers[i][this._partitionIndex].SignalAndWait(mergedCancellationToken);
					int num12 = (num11 + 1) / 2;
					int k = 0;
					int num13 = 0;
					int num14 = 0;
					while (k < num12)
					{
						if ((k & 63) == 0)
						{
							CancellationState.ThrowIfCanceled(mergedCancellationToken);
						}
						if (num13 < num9 && (num14 >= num10 || this._keyComparer.Compare(internalArray[array[num13]], internalArray3[array7[num14]]) <= 0))
						{
							if (flag)
							{
								array10[k] = array2[array[num13]];
							}
							else
							{
								array9[k] = array[num13];
							}
							num13++;
						}
						else
						{
							if (flag)
							{
								array10[k] = array8[array7[num14]];
							}
							else
							{
								array9[k] = num9 + array7[num14];
							}
							num14++;
						}
						k++;
					}
					if (!flag && num9 > 0)
					{
						Array.Copy(array2, 0, array10, 0, num9);
					}
					this._sharedBarriers[i][this._partitionIndex].SignalAndWait(mergedCancellationToken);
				}
			}
		}

		private int ComputePartnerIndex(int phase)
		{
			int num = 1 << phase;
			return this._partitionIndex + ((this._partitionIndex % (num * 2) == 0) ? num : (-num));
		}

		private void QuickSort(int left, int right, TKey[] keys, int[] indices, CancellationToken cancelToken)
		{
			if (right - left > 63)
			{
				CancellationState.ThrowIfCanceled(cancelToken);
			}
			do
			{
				int num = left;
				int num2 = right;
				int num3 = indices[num + (num2 - num >> 1)];
				TKey tkey = keys[num3];
				for (;;)
				{
					if (this._keyComparer.Compare(keys[indices[num]], tkey) >= 0)
					{
						while (this._keyComparer.Compare(keys[indices[num2]], tkey) > 0)
						{
							num2--;
						}
						if (num > num2)
						{
							break;
						}
						if (num < num2)
						{
							int num4 = indices[num];
							indices[num] = indices[num2];
							indices[num2] = num4;
						}
						num++;
						num2--;
						if (num > num2)
						{
							break;
						}
					}
					else
					{
						num++;
					}
				}
				if (num2 - left <= right - num)
				{
					if (left < num2)
					{
						this.QuickSort(left, num2, keys, indices, cancelToken);
					}
					left = num;
				}
				else
				{
					if (num < right)
					{
						this.QuickSort(num, right, keys, indices, cancelToken);
					}
					right = num2;
				}
			}
			while (left < right);
		}

		private QueryOperatorEnumerator<TInputOutput, TKey> _source;

		private int _partitionCount;

		private int _partitionIndex;

		private QueryTaskGroupState _groupState;

		private int[][] _sharedIndices;

		private GrowingArray<TKey>[] _sharedKeys;

		private TInputOutput[][] _sharedValues;

		private Barrier[][] _sharedBarriers;

		private OrdinalIndexState _indexState;

		private IComparer<TKey> _keyComparer;
	}
}
