using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class QueryOpeningEnumerator<TOutput> : IEnumerator<TOutput>, IDisposable, IEnumerator
	{
		internal QueryOpeningEnumerator(QueryOperator<TOutput> queryOperator, ParallelMergeOptions? mergeOptions, bool suppressOrderPreservation)
		{
			this._queryOperator = queryOperator;
			this._mergeOptions = mergeOptions;
			this._suppressOrderPreservation = suppressOrderPreservation;
		}

		public TOutput Current
		{
			get
			{
				if (this._openedQueryEnumerator == null)
				{
					throw new InvalidOperationException("Enumeration has not started. MoveNext must be called to initiate enumeration.");
				}
				return this._openedQueryEnumerator.Current;
			}
		}

		public void Dispose()
		{
			this._topLevelDisposedFlag.Value = true;
			this._topLevelCancellationTokenSource.Cancel();
			if (this._openedQueryEnumerator != null)
			{
				this._openedQueryEnumerator.Dispose();
				this._querySettings.CleanStateAtQueryEnd();
			}
			QueryLifecycle.LogicalQueryExecutionEnd(this._querySettings.QueryId);
		}

		object IEnumerator.Current
		{
			get
			{
				return ((IEnumerator<TOutput>)this).Current;
			}
		}

		public bool MoveNext()
		{
			if (this._topLevelDisposedFlag.Value)
			{
				throw new ObjectDisposedException("enumerator", "The query enumerator has been disposed.");
			}
			if (this._openedQueryEnumerator == null)
			{
				this.OpenQuery();
			}
			bool flag = this._openedQueryEnumerator.MoveNext();
			if ((this._moveNextIteration & 63) == 0)
			{
				CancellationState.ThrowWithStandardMessageIfCanceled(this._querySettings.CancellationState.ExternalCancellationToken);
			}
			this._moveNextIteration++;
			return flag;
		}

		private void OpenQuery()
		{
			if (this._hasQueryOpeningFailed)
			{
				throw new InvalidOperationException("The query enumerator previously threw an exception.");
			}
			try
			{
				this._querySettings = this._queryOperator.SpecifiedQuerySettings.WithPerExecutionSettings(this._topLevelCancellationTokenSource, this._topLevelDisposedFlag).WithDefaults();
				QueryLifecycle.LogicalQueryExecutionBegin(this._querySettings.QueryId);
				this._openedQueryEnumerator = this._queryOperator.GetOpenedEnumerator(this._mergeOptions, this._suppressOrderPreservation, false, this._querySettings);
				CancellationState.ThrowWithStandardMessageIfCanceled(this._querySettings.CancellationState.ExternalCancellationToken);
			}
			catch
			{
				this._hasQueryOpeningFailed = true;
				throw;
			}
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		private readonly QueryOperator<TOutput> _queryOperator;

		private IEnumerator<TOutput> _openedQueryEnumerator;

		private QuerySettings _querySettings;

		private readonly ParallelMergeOptions? _mergeOptions;

		private readonly bool _suppressOrderPreservation;

		private int _moveNextIteration;

		private bool _hasQueryOpeningFailed;

		private readonly Shared<bool> _topLevelDisposedFlag = new Shared<bool>(false);

		private readonly CancellationTokenSource _topLevelCancellationTokenSource = new CancellationTokenSource();
	}
}
