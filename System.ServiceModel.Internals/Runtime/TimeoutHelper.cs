using System;
using System.Threading;

namespace System.Runtime
{
	internal struct TimeoutHelper
	{
		public TimeoutHelper(TimeSpan timeout)
		{
			this.originalTimeout = timeout;
			this.deadline = DateTime.MaxValue;
			this.deadlineSet = timeout == TimeSpan.MaxValue;
		}

		public TimeSpan OriginalTimeout
		{
			get
			{
				return this.originalTimeout;
			}
		}

		public static bool IsTooLarge(TimeSpan timeout)
		{
			return timeout > TimeoutHelper.MaxWait && timeout != TimeSpan.MaxValue;
		}

		public static TimeSpan FromMilliseconds(int milliseconds)
		{
			if (milliseconds == -1)
			{
				return TimeSpan.MaxValue;
			}
			return TimeSpan.FromMilliseconds((double)milliseconds);
		}

		public static int ToMilliseconds(TimeSpan timeout)
		{
			if (timeout == TimeSpan.MaxValue)
			{
				return -1;
			}
			long num = Ticks.FromTimeSpan(timeout);
			if (num / 10000L > 2147483647L)
			{
				return int.MaxValue;
			}
			return Ticks.ToMilliseconds(num);
		}

		public static TimeSpan Min(TimeSpan val1, TimeSpan val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			return val1;
		}

		public static TimeSpan Add(TimeSpan timeout1, TimeSpan timeout2)
		{
			return Ticks.ToTimeSpan(Ticks.Add(Ticks.FromTimeSpan(timeout1), Ticks.FromTimeSpan(timeout2)));
		}

		public static DateTime Add(DateTime time, TimeSpan timeout)
		{
			if (timeout >= TimeSpan.Zero && DateTime.MaxValue - time <= timeout)
			{
				return DateTime.MaxValue;
			}
			if (timeout <= TimeSpan.Zero && DateTime.MinValue - time >= timeout)
			{
				return DateTime.MinValue;
			}
			return time + timeout;
		}

		public static DateTime Subtract(DateTime time, TimeSpan timeout)
		{
			return TimeoutHelper.Add(time, TimeSpan.Zero - timeout);
		}

		public static TimeSpan Divide(TimeSpan timeout, int factor)
		{
			if (timeout == TimeSpan.MaxValue)
			{
				return TimeSpan.MaxValue;
			}
			return Ticks.ToTimeSpan(Ticks.FromTimeSpan(timeout) / (long)factor + 1L);
		}

		public TimeSpan RemainingTime()
		{
			if (!this.deadlineSet)
			{
				this.SetDeadline();
				return this.originalTimeout;
			}
			if (this.deadline == DateTime.MaxValue)
			{
				return TimeSpan.MaxValue;
			}
			TimeSpan timeSpan = this.deadline - DateTime.UtcNow;
			if (timeSpan <= TimeSpan.Zero)
			{
				return TimeSpan.Zero;
			}
			return timeSpan;
		}

		public TimeSpan ElapsedTime()
		{
			return this.originalTimeout - this.RemainingTime();
		}

		private void SetDeadline()
		{
			this.deadline = DateTime.UtcNow + this.originalTimeout;
			this.deadlineSet = true;
		}

		public static void ThrowIfNegativeArgument(TimeSpan timeout)
		{
			TimeoutHelper.ThrowIfNegativeArgument(timeout, "timeout");
		}

		public static void ThrowIfNegativeArgument(TimeSpan timeout, string argumentName)
		{
			if (timeout < TimeSpan.Zero)
			{
				throw Fx.Exception.ArgumentOutOfRange(argumentName, timeout, InternalSR.TimeoutMustBeNonNegative(argumentName, timeout));
			}
		}

		public static void ThrowIfNonPositiveArgument(TimeSpan timeout)
		{
			TimeoutHelper.ThrowIfNonPositiveArgument(timeout, "timeout");
		}

		public static void ThrowIfNonPositiveArgument(TimeSpan timeout, string argumentName)
		{
			if (timeout <= TimeSpan.Zero)
			{
				throw Fx.Exception.ArgumentOutOfRange(argumentName, timeout, InternalSR.TimeoutMustBePositive(argumentName, timeout));
			}
		}

		public static bool WaitOne(WaitHandle waitHandle, TimeSpan timeout)
		{
			TimeoutHelper.ThrowIfNegativeArgument(timeout);
			if (timeout == TimeSpan.MaxValue)
			{
				waitHandle.WaitOne();
				return true;
			}
			return waitHandle.WaitOne(timeout, false);
		}

		private DateTime deadline;

		private bool deadlineSet;

		private TimeSpan originalTimeout;

		public static readonly TimeSpan MaxWait = TimeSpan.FromMilliseconds(2147483647.0);
	}
}
