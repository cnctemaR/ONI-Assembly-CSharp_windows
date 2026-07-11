using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal struct QuerySettings
	{
		internal CancellationState CancellationState
		{
			get
			{
				return this._cancellationState;
			}
			set
			{
				this._cancellationState = value;
			}
		}

		internal TaskScheduler TaskScheduler
		{
			get
			{
				return this._taskScheduler;
			}
			set
			{
				this._taskScheduler = value;
			}
		}

		internal int? DegreeOfParallelism
		{
			get
			{
				return this._degreeOfParallelism;
			}
			set
			{
				this._degreeOfParallelism = value;
			}
		}

		internal ParallelExecutionMode? ExecutionMode
		{
			get
			{
				return this._executionMode;
			}
			set
			{
				this._executionMode = value;
			}
		}

		internal ParallelMergeOptions? MergeOptions
		{
			get
			{
				return this._mergeOptions;
			}
			set
			{
				this._mergeOptions = value;
			}
		}

		internal int QueryId
		{
			get
			{
				return this._queryId;
			}
		}

		internal QuerySettings(TaskScheduler taskScheduler, int? degreeOfParallelism, CancellationToken externalCancellationToken, ParallelExecutionMode? executionMode, ParallelMergeOptions? mergeOptions)
		{
			this._taskScheduler = taskScheduler;
			this._degreeOfParallelism = degreeOfParallelism;
			this._cancellationState = new CancellationState(externalCancellationToken);
			this._executionMode = executionMode;
			this._mergeOptions = mergeOptions;
			this._queryId = -1;
		}

		internal QuerySettings Merge(QuerySettings settings2)
		{
			if (this.TaskScheduler != null && settings2.TaskScheduler != null)
			{
				throw new InvalidOperationException("The WithTaskScheduler operator may be used at most once in a query.");
			}
			if (this.DegreeOfParallelism != null && settings2.DegreeOfParallelism != null)
			{
				throw new InvalidOperationException("The WithDegreeOfParallelism operator may be used at most once in a query.");
			}
			if (this.CancellationState.ExternalCancellationToken.CanBeCanceled && settings2.CancellationState.ExternalCancellationToken.CanBeCanceled)
			{
				throw new InvalidOperationException("The WithCancellation operator may by used at most once in a query.");
			}
			if (this.ExecutionMode != null && settings2.ExecutionMode != null)
			{
				throw new InvalidOperationException("The WithExecutionMode operator may be used at most once in a query.");
			}
			if (this.MergeOptions != null && settings2.MergeOptions != null)
			{
				throw new InvalidOperationException("The WithMergeOptions operator may be used at most once in a query.");
			}
			TaskScheduler taskScheduler = ((this.TaskScheduler == null) ? settings2.TaskScheduler : this.TaskScheduler);
			int? num = ((this.DegreeOfParallelism != null) ? this.DegreeOfParallelism : settings2.DegreeOfParallelism);
			CancellationToken cancellationToken = (this.CancellationState.ExternalCancellationToken.CanBeCanceled ? this.CancellationState.ExternalCancellationToken : settings2.CancellationState.ExternalCancellationToken);
			ParallelExecutionMode? parallelExecutionMode = ((this.ExecutionMode != null) ? this.ExecutionMode : settings2.ExecutionMode);
			ParallelMergeOptions? parallelMergeOptions = ((this.MergeOptions != null) ? this.MergeOptions : settings2.MergeOptions);
			return new QuerySettings(taskScheduler, num, cancellationToken, parallelExecutionMode, parallelMergeOptions);
		}

		internal QuerySettings WithPerExecutionSettings()
		{
			return this.WithPerExecutionSettings(new CancellationTokenSource(), new Shared<bool>(false));
		}

		internal QuerySettings WithPerExecutionSettings(CancellationTokenSource topLevelCancellationTokenSource, Shared<bool> topLevelDisposedFlag)
		{
			QuerySettings querySettings = new QuerySettings(this.TaskScheduler, this.DegreeOfParallelism, this.CancellationState.ExternalCancellationToken, this.ExecutionMode, this.MergeOptions);
			querySettings.CancellationState.InternalCancellationTokenSource = topLevelCancellationTokenSource;
			querySettings.CancellationState.MergedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(querySettings.CancellationState.InternalCancellationTokenSource.Token, querySettings.CancellationState.ExternalCancellationToken);
			querySettings.CancellationState.TopLevelDisposedFlag = topLevelDisposedFlag;
			querySettings._queryId = PlinqEtwProvider.NextQueryId();
			return querySettings;
		}

		internal QuerySettings WithDefaults()
		{
			QuerySettings querySettings = this;
			if (querySettings.TaskScheduler == null)
			{
				querySettings.TaskScheduler = TaskScheduler.Default;
			}
			if (querySettings.DegreeOfParallelism == null)
			{
				querySettings.DegreeOfParallelism = new int?(Scheduling.GetDefaultDegreeOfParallelism());
			}
			if (querySettings.ExecutionMode == null)
			{
				querySettings.ExecutionMode = new ParallelExecutionMode?(ParallelExecutionMode.Default);
			}
			if (querySettings.MergeOptions == null)
			{
				querySettings.MergeOptions = new ParallelMergeOptions?(ParallelMergeOptions.Default);
			}
			if (querySettings.MergeOptions == ParallelMergeOptions.Default)
			{
				querySettings.MergeOptions = new ParallelMergeOptions?(ParallelMergeOptions.AutoBuffered);
			}
			return querySettings;
		}

		internal static QuerySettings Empty
		{
			get
			{
				return new QuerySettings(null, null, default(CancellationToken), null, null);
			}
		}

		public void CleanStateAtQueryEnd()
		{
			this._cancellationState.MergedCancellationTokenSource.Dispose();
		}

		private TaskScheduler _taskScheduler;

		private int? _degreeOfParallelism;

		private CancellationState _cancellationState;

		private ParallelExecutionMode? _executionMode;

		private ParallelMergeOptions? _mergeOptions;

		private int _queryId;
	}
}
