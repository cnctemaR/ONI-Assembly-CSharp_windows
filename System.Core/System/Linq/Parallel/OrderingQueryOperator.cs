using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal sealed class OrderingQueryOperator<TSource> : QueryOperator<TSource>
	{
		public OrderingQueryOperator(QueryOperator<TSource> child, bool orderOn)
			: base(orderOn, child.SpecifiedQuerySettings)
		{
			this._child = child;
			this._ordinalIndexState = this._child.OrdinalIndexState;
		}

		internal override QueryResults<TSource> Open(QuerySettings settings, bool preferStriping)
		{
			return this._child.Open(settings, preferStriping);
		}

		internal override IEnumerator<TSource> GetEnumerator(ParallelMergeOptions? mergeOptions, bool suppressOrderPreservation)
		{
			ScanQueryOperator<TSource> scanQueryOperator = this._child as ScanQueryOperator<TSource>;
			if (scanQueryOperator != null)
			{
				return scanQueryOperator.Data.GetEnumerator();
			}
			return base.GetEnumerator(mergeOptions, suppressOrderPreservation);
		}

		internal override IEnumerable<TSource> AsSequentialQuery(CancellationToken token)
		{
			return this._child.AsSequentialQuery(token);
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return this._child.LimitsParallelism;
			}
		}

		internal override OrdinalIndexState OrdinalIndexState
		{
			get
			{
				return this._ordinalIndexState;
			}
		}

		private QueryOperator<TSource> _child;

		private OrdinalIndexState _ordinalIndexState;
	}
}
