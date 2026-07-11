using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class ScanQueryOperator<TElement> : QueryOperator<TElement>
	{
		internal ScanQueryOperator(IEnumerable<TElement> data)
			: base(false, QuerySettings.Empty)
		{
			ParallelEnumerableWrapper<TElement> parallelEnumerableWrapper = data as ParallelEnumerableWrapper<TElement>;
			if (parallelEnumerableWrapper != null)
			{
				data = parallelEnumerableWrapper.WrappedEnumerable;
			}
			this._data = data;
		}

		public IEnumerable<TElement> Data
		{
			get
			{
				return this._data;
			}
		}

		internal override QueryResults<TElement> Open(QuerySettings settings, bool preferStriping)
		{
			IList<TElement> list = this._data as IList<TElement>;
			if (list != null)
			{
				return new ListQueryResults<TElement>(list, settings.DegreeOfParallelism.GetValueOrDefault(), preferStriping);
			}
			return new ScanQueryOperator<TElement>.ScanEnumerableQueryOperatorResults(this._data, settings);
		}

		internal override IEnumerator<TElement> GetEnumerator(ParallelMergeOptions? mergeOptions, bool suppressOrderPreservation)
		{
			return this._data.GetEnumerator();
		}

		internal override IEnumerable<TElement> AsSequentialQuery(CancellationToken token)
		{
			return this._data;
		}

		internal override OrdinalIndexState OrdinalIndexState
		{
			get
			{
				if (!(this._data is IList<TElement>))
				{
					return OrdinalIndexState.Correct;
				}
				return OrdinalIndexState.Indexable;
			}
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return false;
			}
		}

		private readonly IEnumerable<TElement> _data;

		private class ScanEnumerableQueryOperatorResults : QueryResults<TElement>
		{
			internal ScanEnumerableQueryOperatorResults(IEnumerable<TElement> data, QuerySettings settings)
			{
				this._data = data;
				this._settings = settings;
			}

			internal override void GivePartitionedStream(IPartitionedStreamRecipient<TElement> recipient)
			{
				PartitionedStream<TElement, int> partitionedStream = ExchangeUtilities.PartitionDataSource<TElement>(this._data, this._settings.DegreeOfParallelism.Value, false);
				recipient.Receive<int>(partitionedStream);
			}

			private IEnumerable<TElement> _data;

			private QuerySettings _settings;
		}
	}
}
