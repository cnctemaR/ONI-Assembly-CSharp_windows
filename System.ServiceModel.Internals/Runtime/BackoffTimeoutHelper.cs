using System;
using System.Threading;

namespace System.Runtime
{
	internal sealed class BackoffTimeoutHelper
	{
		internal BackoffTimeoutHelper(TimeSpan timeout)
			: this(timeout, BackoffTimeoutHelper.defaultMaxWaitTime)
		{
		}

		internal BackoffTimeoutHelper(TimeSpan timeout, TimeSpan maxWaitTime)
			: this(timeout, maxWaitTime, BackoffTimeoutHelper.defaultInitialWaitTime)
		{
		}

		internal BackoffTimeoutHelper(TimeSpan timeout, TimeSpan maxWaitTime, TimeSpan initialWaitTime)
		{
			this.random = new Random(this.GetHashCode());
			this.maxWaitTime = maxWaitTime;
			this.originalTimeout = timeout;
			this.Reset(timeout, initialWaitTime);
		}

		public TimeSpan OriginalTimeout
		{
			get
			{
				return this.originalTimeout;
			}
		}

		private void Reset(TimeSpan timeout, TimeSpan initialWaitTime)
		{
			if (timeout == TimeSpan.MaxValue)
			{
				this.deadline = DateTime.MaxValue;
			}
			else
			{
				this.deadline = DateTime.UtcNow + timeout;
			}
			this.waitTime = initialWaitTime;
		}

		public bool IsExpired()
		{
			return !(this.deadline == DateTime.MaxValue) && DateTime.UtcNow >= this.deadline;
		}

		public void WaitAndBackoff(Action<object> callback, object state)
		{
			if (this.backoffCallback != callback || this.backoffState != state)
			{
				if (this.backoffTimer != null)
				{
					this.backoffTimer.Cancel();
				}
				this.backoffCallback = callback;
				this.backoffState = state;
				this.backoffTimer = new IOThreadTimer(callback, state, false, BackoffTimeoutHelper.maxSkewMilliseconds);
			}
			TimeSpan timeSpan = this.WaitTimeWithDrift();
			this.Backoff();
			this.backoffTimer.Set(timeSpan);
		}

		public void WaitAndBackoff()
		{
			Thread.Sleep(this.WaitTimeWithDrift());
			this.Backoff();
		}

		private TimeSpan WaitTimeWithDrift()
		{
			return Ticks.ToTimeSpan(Math.Max(Ticks.FromTimeSpan(BackoffTimeoutHelper.defaultInitialWaitTime), Ticks.Add(Ticks.FromTimeSpan(this.waitTime), (long)((ulong)this.random.Next() % (ulong)(2L * BackoffTimeoutHelper.maxDriftTicks + 1L) - (ulong)BackoffTimeoutHelper.maxDriftTicks))));
		}

		private void Backoff()
		{
			if (this.waitTime.Ticks >= this.maxWaitTime.Ticks / 2L)
			{
				this.waitTime = this.maxWaitTime;
			}
			else
			{
				this.waitTime = TimeSpan.FromTicks(this.waitTime.Ticks * 2L);
			}
			if (this.deadline != DateTime.MaxValue)
			{
				TimeSpan timeSpan = this.deadline - DateTime.UtcNow;
				if (this.waitTime > timeSpan)
				{
					this.waitTime = timeSpan;
					if (this.waitTime < TimeSpan.Zero)
					{
						this.waitTime = TimeSpan.Zero;
					}
				}
			}
		}

		private static readonly int maxSkewMilliseconds = (int)(IOThreadTimer.SystemTimeResolutionTicks / 10000L);

		private static readonly long maxDriftTicks = IOThreadTimer.SystemTimeResolutionTicks * 2L;

		private static readonly TimeSpan defaultInitialWaitTime = TimeSpan.FromMilliseconds(1.0);

		private static readonly TimeSpan defaultMaxWaitTime = TimeSpan.FromMinutes(1.0);

		private DateTime deadline;

		private TimeSpan maxWaitTime;

		private TimeSpan waitTime;

		private IOThreadTimer backoffTimer;

		private Action<object> backoffCallback;

		private object backoffState;

		private Random random;

		private TimeSpan originalTimeout;
	}
}
