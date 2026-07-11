using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal static class ExchangeUtilities
	{
		internal static PartitionedStream<T, int> PartitionDataSource<T>(IEnumerable<T> source, int partitionCount, bool useStriping)
		{
			IParallelPartitionable<T> parallelPartitionable = source as IParallelPartitionable<T>;
			PartitionedStream<T, int> partitionedStream2;
			if (parallelPartitionable != null)
			{
				QueryOperatorEnumerator<T, int>[] partitions = parallelPartitionable.GetPartitions(partitionCount);
				if (partitions == null)
				{
					throw new InvalidOperationException("The return value must not be null.");
				}
				if (partitions.Length != partitionCount)
				{
					throw new InvalidOperationException("The returned array's length must equal the number of partitions requested.");
				}
				PartitionedStream<T, int> partitionedStream = new PartitionedStream<T, int>(partitionCount, Util.GetDefaultComparer<int>(), OrdinalIndexState.Correct);
				for (int i = 0; i < partitionCount; i++)
				{
					QueryOperatorEnumerator<T, int> queryOperatorEnumerator = partitions[i];
					if (queryOperatorEnumerator == null)
					{
						throw new InvalidOperationException("Elements returned must not be null.");
					}
					partitionedStream[i] = queryOperatorEnumerator;
				}
				partitionedStream2 = partitionedStream;
			}
			else
			{
				partitionedStream2 = new PartitionedDataSource<T>(source, partitionCount, useStriping);
			}
			return partitionedStream2;
		}

		internal static PartitionedStream<Pair<TElement, THashKey>, int> HashRepartition<TElement, THashKey, TIgnoreKey>(PartitionedStream<TElement, TIgnoreKey> source, Func<TElement, THashKey> keySelector, IEqualityComparer<THashKey> keyComparer, IEqualityComparer<TElement> elementComparer, CancellationToken cancellationToken)
		{
			return new UnorderedHashRepartitionStream<TElement, THashKey, TIgnoreKey>(source, keySelector, keyComparer, elementComparer, cancellationToken);
		}

		internal static PartitionedStream<Pair<TElement, THashKey>, TOrderKey> HashRepartitionOrdered<TElement, THashKey, TOrderKey>(PartitionedStream<TElement, TOrderKey> source, Func<TElement, THashKey> keySelector, IEqualityComparer<THashKey> keyComparer, IEqualityComparer<TElement> elementComparer, CancellationToken cancellationToken)
		{
			return new OrderedHashRepartitionStream<TElement, THashKey, TOrderKey>(source, keySelector, keyComparer, elementComparer, cancellationToken);
		}

		internal static OrdinalIndexState Worse(this OrdinalIndexState state1, OrdinalIndexState state2)
		{
			if (state1 <= state2)
			{
				return state2;
			}
			return state1;
		}

		internal static bool IsWorseThan(this OrdinalIndexState state1, OrdinalIndexState state2)
		{
			return state1 > state2;
		}
	}
}
