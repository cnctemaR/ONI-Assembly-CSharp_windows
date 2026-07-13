using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class WaitWhile : CustomYieldInstruction
	{
		public override bool keepWaiting
		{
			get
			{
				bool flag = this.m_MaxExecutionTime == -1.0;
				bool flag2;
				if (flag)
				{
					flag2 = this.m_Predicate();
				}
				else
				{
					bool flag3 = this.GetTime() > this.m_MaxExecutionTime;
					if (flag3)
					{
						this.m_TimeoutCallback();
						flag2 = false;
					}
					else
					{
						flag2 = this.m_Predicate();
					}
				}
				return flag2;
			}
		}

		public WaitWhile(Func<bool> predicate)
		{
			this.m_Predicate = predicate;
		}

		public WaitWhile(Func<bool> predicate, TimeSpan timeout, Action onTimeout, WaitTimeoutMode timeoutMode = WaitTimeoutMode.Realtime)
			: this(predicate)
		{
			bool flag = timeout <= TimeSpan.Zero;
			if (flag)
			{
				throw new ArgumentException("Timeout must be greater than zero", "timeout");
			}
			if (onTimeout == null)
			{
				throw new ArgumentNullException("onTimeout", "Timeout callback must be specified");
			}
			this.m_TimeoutCallback = onTimeout;
			this.m_TimeoutMode = timeoutMode;
			this.m_MaxExecutionTime = this.GetTime() + timeout.TotalSeconds;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double GetTime()
		{
			return (this.m_TimeoutMode == WaitTimeoutMode.InGameTime) ? Time.timeAsDouble : Time.realtimeSinceStartupAsDouble;
		}

		private readonly Func<bool> m_Predicate;

		private readonly Action m_TimeoutCallback;

		private readonly WaitTimeoutMode m_TimeoutMode;

		private readonly double m_MaxExecutionTime = -1.0;
	}
}
