using System;

namespace UnityEngine.UIElements
{
	internal abstract class ScheduledItem
	{
		public long startMs { get; set; }

		public long delayMs { get; set; }

		public long intervalMs { get; set; }

		public long endTimeMs { get; private set; }

		public ScheduledItem()
		{
			this.ResetStartTime();
			this.timerUpdateStopCondition = ScheduledItem.OnceCondition;
		}

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
			bool flag = this.timerUpdateStopCondition != null;
			return flag && this.timerUpdateStopCondition();
		}

		public Func<bool> timerUpdateStopCondition;

		public static readonly Func<bool> OnceCondition = () => true;

		public static readonly Func<bool> ForeverCondition = () => false;
	}
}
