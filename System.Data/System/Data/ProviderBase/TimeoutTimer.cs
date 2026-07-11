using System;
using System.Data.Common;

namespace System.Data.ProviderBase
{
	internal class TimeoutTimer
	{
		internal static TimeoutTimer StartSecondsTimeout(int seconds)
		{
			TimeoutTimer timeoutTimer = new TimeoutTimer();
			timeoutTimer.SetTimeoutSeconds(seconds);
			return timeoutTimer;
		}

		internal static TimeoutTimer StartMillisecondsTimeout(long milliseconds)
		{
			return new TimeoutTimer
			{
				_timerExpire = checked(ADP.TimerCurrent() + milliseconds * 10000L),
				_isInfiniteTimeout = false
			};
		}

		internal void SetTimeoutSeconds(int seconds)
		{
			if (TimeoutTimer.InfiniteTimeout == (long)seconds)
			{
				this._isInfiniteTimeout = true;
				return;
			}
			this._timerExpire = checked(ADP.TimerCurrent() + ADP.TimerFromSeconds(seconds));
			this._isInfiniteTimeout = false;
		}

		internal bool IsExpired
		{
			get
			{
				return !this.IsInfinite && ADP.TimerHasExpired(this._timerExpire);
			}
		}

		internal bool IsInfinite
		{
			get
			{
				return this._isInfiniteTimeout;
			}
		}

		internal long LegacyTimerExpire
		{
			get
			{
				if (!this._isInfiniteTimeout)
				{
					return this._timerExpire;
				}
				return long.MaxValue;
			}
		}

		internal long MillisecondsRemaining
		{
			get
			{
				long num;
				if (this._isInfiniteTimeout)
				{
					num = long.MaxValue;
				}
				else
				{
					num = ADP.TimerRemainingMilliseconds(this._timerExpire);
					if (0L > num)
					{
						num = 0L;
					}
				}
				return num;
			}
		}

		private long _timerExpire;

		private bool _isInfiniteTimeout;

		internal static readonly long InfiniteTimeout;
	}
}
