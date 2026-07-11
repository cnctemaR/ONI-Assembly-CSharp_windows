using System;
using System.Security.Permissions;

namespace System.Threading
{
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public struct SpinWait
	{
		public int Count
		{
			get
			{
				return this.m_count;
			}
		}

		public bool NextSpinWillYield
		{
			get
			{
				return this.m_count > 10 || PlatformHelper.IsSingleProcessor;
			}
		}

		public void SpinOnce()
		{
			if (this.NextSpinWillYield)
			{
				int num = ((this.m_count >= 10) ? (this.m_count - 10) : this.m_count);
				if (num % 20 == 19)
				{
					Thread.Sleep(1);
				}
				else if (num % 5 == 4)
				{
					Thread.Sleep(0);
				}
				else
				{
					Thread.Yield();
				}
			}
			else
			{
				Thread.SpinWait(4 << this.m_count);
			}
			this.m_count = ((this.m_count == int.MaxValue) ? 10 : (this.m_count + 1));
		}

		public void Reset()
		{
			this.m_count = 0;
		}

		public static void SpinUntil(Func<bool> condition)
		{
			SpinWait.SpinUntil(condition, -1);
		}

		public static bool SpinUntil(Func<bool> condition, TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", timeout, Environment.GetResourceString("The timeout must represent a value between -1 and Int32.MaxValue, inclusive."));
			}
			return SpinWait.SpinUntil(condition, (int)timeout.TotalMilliseconds);
		}

		public static bool SpinUntil(Func<bool> condition, int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", millisecondsTimeout, Environment.GetResourceString("The timeout must represent a value between -1 and Int32.MaxValue, inclusive."));
			}
			if (condition == null)
			{
				throw new ArgumentNullException("condition", Environment.GetResourceString("The condition argument is null."));
			}
			uint num = 0U;
			if (millisecondsTimeout != 0 && millisecondsTimeout != -1)
			{
				num = TimeoutHelper.GetTime();
			}
			SpinWait spinWait = default(SpinWait);
			while (!condition())
			{
				if (millisecondsTimeout == 0)
				{
					return false;
				}
				spinWait.SpinOnce();
				if (millisecondsTimeout != -1 && spinWait.NextSpinWillYield && (long)millisecondsTimeout <= (long)((ulong)(TimeoutHelper.GetTime() - num)))
				{
					return false;
				}
			}
			return true;
		}

		internal const int YIELD_THRESHOLD = 10;

		internal const int SLEEP_0_EVERY_HOW_MANY_TIMES = 5;

		internal const int SLEEP_1_EVERY_HOW_MANY_TIMES = 20;

		private int m_count;
	}
}
