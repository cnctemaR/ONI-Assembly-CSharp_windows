using System;

namespace System.Threading.Tasks
{
	public class ParallelOptions
	{
		public ParallelOptions()
		{
			this.m_scheduler = TaskScheduler.Default;
			this.m_maxDegreeOfParallelism = -1;
			this.m_cancellationToken = CancellationToken.None;
		}

		public TaskScheduler TaskScheduler
		{
			get
			{
				return this.m_scheduler;
			}
			set
			{
				this.m_scheduler = value;
			}
		}

		internal TaskScheduler EffectiveTaskScheduler
		{
			get
			{
				if (this.m_scheduler == null)
				{
					return TaskScheduler.Current;
				}
				return this.m_scheduler;
			}
		}

		public int MaxDegreeOfParallelism
		{
			get
			{
				return this.m_maxDegreeOfParallelism;
			}
			set
			{
				if (value == 0 || value < -1)
				{
					throw new ArgumentOutOfRangeException("MaxDegreeOfParallelism");
				}
				this.m_maxDegreeOfParallelism = value;
			}
		}

		public CancellationToken CancellationToken
		{
			get
			{
				return this.m_cancellationToken;
			}
			set
			{
				this.m_cancellationToken = value;
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

		private TaskScheduler m_scheduler;

		private int m_maxDegreeOfParallelism;

		private CancellationToken m_cancellationToken;
	}
}
