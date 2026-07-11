using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class QueryExecutionOption<TSource> : QueryOperator<TSource>
	{
		internal QueryExecutionOption(QueryOperator<TSource> source, QuerySettings settings)
			: base(source.OutputOrdered, settings.Merge(source.SpecifiedQuerySettings))
		{
			this._child = source;
			this._indexState = this._child.OrdinalIndexState;
		}

		internal override QueryResults<TSource> Open(QuerySettings settings, bool preferStriping)
		{
			return this._child.Open(settings, preferStriping);
		}

		internal override IEnumerable<TSource> AsSequentialQuery(CancellationToken token)
		{
			return this._child.AsSequentialQuery(token);
		}

		internal override OrdinalIndexState OrdinalIndexState
		{
			get
			{
				return this._indexState;
			}
		}

		internal override bool LimitsParallelism
		{
			get
			{
				return this._child.LimitsParallelism;
			}
		}

		private QueryOperator<TSource> _child;

		private OrdinalIndexState _indexState;
	}
}
