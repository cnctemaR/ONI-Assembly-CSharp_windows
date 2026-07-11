using System;

namespace UnityEngine.Experimental.UIElements
{
	internal abstract class ScheduledItem : IScheduledItem
	{
		public ScheduledItem()
		{
			this.ResetStartTime();
			this.timerUpdateStopCondition = ScheduledItem.OnceCondition;
		}

		public long startMs { get; set; }

		public long delayMs { get; set; }

		public long intervalMs { get; set; }

		public long endTimeMs { get; private set; }

		protected void ResetStartTime()
		{
			this.startMs = Panel.TimeSinceStartupMs();
		}

		public void SetDuration(long durationMs)
		{
			this.endTimeMs = this.startMs + durationMs;
		}

		public abstract void PerformTimerUpdate(TimerState state);

		internal virtual void OnItemUnscheduled()
		{
		}

		public virtual bool ShouldUnschedule()
		{
			if (this.endTimeMs > 0L)
			{
				if (Panel.TimeSinceStartupMs() > this.endTimeMs)
				{
					return true;
				}
			}
			return this.timerUpdateStopCondition != null && this.timerUpdateStopCondition();
		}

		public Func<bool> timerUpdateStopCondition;

		public static readonly Func<bool> OnceCondition = () => true;

		public static readonly Func<bool> ForeverCondition = () => false;
	}
}
