using System;

namespace System.Threading.Tasks
{
	public class ParallelOptions
	{
		public ParallelOptions()
		{
			this._scheduler = TaskScheduler.Default;
			this._maxDegreeOfParallelism = -1;
			this._cancellationToken = CancellationToken.None;
		}

		public TaskScheduler TaskScheduler
		{
			get
			{
				return this._scheduler;
			}
			set
			{
				this._scheduler = value;
			}
		}

		internal TaskScheduler EffectiveTaskScheduler
		{
			get
			{
				if (this._scheduler == null)
				{
					return TaskScheduler.Current;
				}
				return this._scheduler;
			}
		}

		public int MaxDegreeOfParallelism
		{
			get
			{
				return this._maxDegreeOfParallelism;
			}
			set
			{
				if (value == 0 || value < -1)
				{
					throw new ArgumentOutOfRangeException("MaxDegreeOfParallelism");
				}
				this._maxDegreeOfParallelism = value;
			}
		}

		public CancellationToken CancellationToken
		{
			get
			{
				return this._cancellationToken;
			}
			set
			{
				this._cancellationToken = value;
			}
		}

		internal int EffectiveMaxConcurrencyLevel
		{
			get
			{
				int num = this.MaxDegreeOfParallelism;
				int maximumConcurrencyLevel = this.EffectiveTaskScheduler.MaximumConcurrencyLevel;
				if (maximumConcurrencyLevel > 0 && maximumConcurrencyLevel != 2147483647)
				{
					num = ((num == -1) ? maximumConcurrencyLevel : Math.Min(maximumConcurrencyLevel, num));
				}
				return num;
			}
		}

		private TaskScheduler _scheduler;

		private int _maxDegreeOfParallelism;

		private CancellationToken _cancellationToken;
	}
}
