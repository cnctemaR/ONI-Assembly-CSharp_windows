using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class PartitionerQueryOperator<TElement> : QueryOperator<TElement>
	{
		internal PartitionerQueryOperator(Partitioner<TElement> partitioner)
			: base(false, QuerySettings.Empty)
		{
			this._partitioner = partitioner;
		}

		internal bool Orderable
		{
			get
			{
				return this._partitioner is OrderablePartitioner<TElement>;
			}
		}

		internal override QueryResults<TElement> Open(QuerySettings settings, bool preferStriping)
		{
			return new PartitionerQueryOperator<TElement>.PartitionerQueryOperatorResults(this._partitioner, settings);
		}

		internal override IEnumerable<TElement> AsSequentialQuery(CancellationToken token)
		{
			using (IEnumerator<TElement> enumerator = this._partitioner.GetPartitions(1)[0])
			{
				while (enumerator.MoveNext())
				{
					TElement telement = enumerator.Current;
					yield return telement;
				}
			}
			IEnumerator<TElement> enumerator = null;
			yield break;
			yield break;
		}

		internal override OrdinalIndexState OrdinalIndexState
		{
			get
			{
				return PartitionerQueryOperator<TElement>.GetOrdinalIndexState(this._partitioner);
			}
		}

		internal static OrdinalIndexState GetOrdinalIndexState(Partitioner<TElement> partitioner)
		{
			OrderablePartitioner<TElement> orderablePartitioner = partitioner as OrderablePartitioner<TElement>;
			if (orderablePartitioner == null)
			{
				return OrdinalIndexState.Shuffled;
			}
			if (!orderablePartitioner.KeysOrderedInEachPartition)
			{
				return OrdinalIndexState.Shuffled;
			}
			if (orderablePartitioner.KeysNormalized)
			{
				return OrdinalIndexState.Correct;
			}
			return OrdinalIndexState.Increasing;
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private Partitioner<TElement> _partitioner;

		private class PartitionerQueryOperatorResults : QueryResults<TElement>
		{
			internal PartitionerQueryOperatorResults(Partitioner<TElement> partitioner, QuerySettings settings)
			{
				this._partitioner = partitioner;
				this._settings = settings;
			}

			internal override void GivePartitionedStream(IPartitionedStreamRecipient<TElement> recipient)
			{
				int value = this._settings.DegreeOfParallelism.Value;
				OrderablePartitioner<TElement> orderablePartitioner = this._partitioner as OrderablePartitioner<TElement>;
				OrdinalIndexState ordinalIndexState = ((orderablePartitioner != null) ? PartitionerQueryOperator<TElement>.GetOrdinalIndexState(orderablePartitioner) : OrdinalIndexState.Shuffled);
				PartitionedStream<TElement, int> partitionedStream = new PartitionedStream<TElement, int>(value, Util.GetDefaultComparer<int>(), ordinalIndexState);
				if (orderablePartitioner != null)
				{
					IList<IEnumerator<KeyValuePair<long, TElement>>> orderablePartitions = orderablePartitioner.GetOrderablePartitions(value);
					if (orderablePartitions == null)
					{
						throw new InvalidOperationException("Partitioner returned null instead of a list of partitions.");
					}
					if (orderablePartitions.Count != value)
					{
						throw new InvalidOperationException("Partitioner returned a wrong number of partitions.");
					}
					for (int i = 0; i < value; i++)
					{
						IEnumerator<KeyValuePair<long, TElement>> enumerator = orderablePartitions[i];
						if (enumerator == null)
						{
							throw new InvalidOperationException("Partitioner returned a null partition.");
						}
						partitionedStream[i] = new PartitionerQueryOperator<TElement>.OrderablePartitionerEnumerator(enumerator);
					}
				}
				else
				{
					IList<IEnumerator<TElement>> partitions = this._partitioner.GetPartitions(value);
					if (partitions == null)
					{
						throw new InvalidOperationException("Partitioner returned null instead of a list of partitions.");
					}
					if (partitions.Count != value)
					{
						throw new InvalidOperationException("Partitioner returned a wrong number of partitions.");
					}
					for (int j = 0; j < value; j++)
					{
						IEnumerator<TElement> enumerator2 = partitions[j];
						if (enumerator2 == null)
						{
							throw new InvalidOperationException("Partitioner returned a null partition.");
						}
						partitionedStream[j] = new PartitionerQueryOperator<TElement>.PartitionerEnumerator(enumerator2);
					}
				}
				recipient.Receive<int>(partitionedStream);
			}

			private Partitioner<TElement> _partitioner;

			private QuerySettings _settings;
		}

		private class OrderablePartitionerEnumerator : QueryOperatorEnumerator<TElement, int>
		{
			internal OrderablePartitionerEnumerator(IEnumerator<KeyValuePair<long, TElement>> sourceEnumerator)
			{
				this._sourceEnumerator = sourceEnumerator;
			}

			internal override bool MoveNext(ref TElement currentElement, ref int currentKey)
			{
				if (!this._sourceEnumerator.MoveNext())
				{
					return false;
				}
				KeyValuePair<long, TElement> keyValuePair = this._sourceEnumerator.Current;
				currentElement = keyValuePair.Value;
				currentKey = checked((int)keyValuePair.Key);
				return true;
			}

			protected override void Dispose(bool disposing)
			{
				this._sourceEnumerator.Dispose();
			}

			private IEnumerator<KeyValuePair<long, TElement>> _sourceEnumerator;
		}

		private class PartitionerEnumerator : QueryOperatorEnumerator<TElement, int>
		{
			internal PartitionerEnumerator(IEnumerator<TElement> sourceEnumerator)
			{
				this._sourceEnumerator = sourceEnumerator;
			}

			internal override bool MoveNext(ref TElement currentElement, ref int currentKey)
			{
				if (!this._sourceEnumerator.MoveNext())
				{
					return false;
				}
				currentElement = this._sourceEnumerator.Current;
				currentKey = 0;
				return true;
			}

			protected override void Dispose(bool disposing)
			{
				this._sourceEnumerator.Dispose();
			}

			private IEnumerator<TElement> _sourceEnumerator;
		}
	}
}
