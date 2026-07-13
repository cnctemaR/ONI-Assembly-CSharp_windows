using System;

namespace UnityEngine.UIElements
{
	internal abstract class ScheduledItem
	{
		public long startMs { get; set; }

		public long delayMs { get; set; }

		public long intervalMs { get; set; }

		public long endTimeMs { get; private set; }

		public ScheduledItem(long startMs)
		{
			this.ResetStartTime(startMs);
			this.timerUpdateStopCondition = ScheduledItem.OnceCondition;
		}

		protected void ResetStartTime(long startMs)
		{
			this.startMs = startMs;
		}

		public void SetDuration(long durationMs)
		{
			this.endTimeMs = this.startMs + durationMs;
		}

		public void OffsetBy(long deltaMs)
		{
			bool flag = this.endTimeMs > 0L;
			if (flag)
			{
				this.endTimeMs += deltaMs;
			}
			this.startMs += deltaMs;
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
