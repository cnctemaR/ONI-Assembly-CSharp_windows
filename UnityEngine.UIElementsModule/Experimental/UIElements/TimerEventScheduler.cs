using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class TimerEventScheduler : IScheduler
	{
		public void Schedule(IScheduledItem item)
		{
			if (item != null)
			{
				ScheduledItem scheduledItem = item as ScheduledItem;
				if (scheduledItem == null)
				{
					throw new NotSupportedException("Scheduled Item type is not supported by this scheduler");
				}
				if (this.m_TransactionMode)
				{
					if (!this.m_UnscheduleTransactions.Remove(scheduledItem))
					{
						if (this.m_ScheduledItems.Contains(scheduledItem) || this.m_ScheduleTransactions.Contains(scheduledItem))
						{
							throw new ArgumentException("Cannot schedule function " + scheduledItem + " more than once");
						}
						this.m_ScheduleTransactions.Add(scheduledItem);
					}
				}
				else
				{
					if (this.m_ScheduledItems.Contains(scheduledItem))
					{
						throw new ArgumentException("Cannot schedule function " + scheduledItem + " more than once");
					}
					this.m_ScheduledItems.Add(scheduledItem);
				}
			}
		}

		public IScheduledItem ScheduleOnce(Action<TimerState> timerUpdateEvent, long delayMs)
		{
			TimerEventScheduler.TimerEventSchedulerItem timerEventSchedulerItem = new TimerEventScheduler.TimerEventSchedulerItem(timerUpdateEvent)
			{
				delayMs = delayMs
			};
			this.Schedule(timerEventSchedulerItem);
			return timerEventSchedulerItem;
		}

		public IScheduledItem ScheduleUntil(Action<TimerState> timerUpdateEvent, long delayMs, long intervalMs, Func<bool> stopCondition)
		{
			TimerEventScheduler.TimerEventSchedulerItem timerEventSchedulerItem = new TimerEventScheduler.TimerEventSchedulerItem(timerUpdateEvent)
			{
				delayMs = delayMs,
				intervalMs = intervalMs,
				timerUpdateStopCondition = stopCondition
			};
			this.Schedule(timerEventSchedulerItem);
			return timerEventSchedulerItem;
		}

		public IScheduledItem ScheduleForDuration(Action<TimerState> timerUpdateEvent, long delayMs, long intervalMs, long durationMs)
		{
			TimerEventScheduler.TimerEventSchedulerItem timerEventSchedulerItem = new TimerEventScheduler.TimerEventSchedulerItem(timerUpdateEvent)
			{
				delayMs = delayMs,
				intervalMs = intervalMs,
				timerUpdateStopCondition = null
			};
			timerEventSchedulerItem.SetDuration(durationMs);
			this.Schedule(timerEventSchedulerItem);
			return timerEventSchedulerItem;
		}

		private bool RemovedScheduledItemAt(int index)
		{
			bool flag;
			if (index >= 0)
			{
				ScheduledItem scheduledItem = this.m_ScheduledItems[index];
				this.m_ScheduledItems.RemoveAt(index);
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public void Unschedule(IScheduledItem item)
		{
			ScheduledItem scheduledItem = item as ScheduledItem;
			if (scheduledItem != null)
			{
				if (this.m_TransactionMode)
				{
					if (this.m_UnscheduleTransactions.Contains(scheduledItem))
					{
						throw new ArgumentException("Cannot unschedule scheduled function twice" + scheduledItem);
					}
					if (!this.m_ScheduleTransactions.Remove(scheduledItem))
					{
						if (!this.m_ScheduledItems.Contains(scheduledItem))
						{
							throw new ArgumentException("Cannot unschedule unknown scheduled function " + scheduledItem);
						}
						this.m_UnscheduleTransactions.Add(scheduledItem);
					}
				}
				else if (!this.PrivateUnSchedule(scheduledItem))
				{
					throw new ArgumentException("Cannot unschedule unknown scheduled function " + scheduledItem);
				}
				scheduledItem.OnItemUnscheduled();
			}
		}

		private bool PrivateUnSchedule(ScheduledItem sItem)
		{
			return this.m_ScheduleTransactions.Remove(sItem) || this.RemovedScheduledItemAt(this.m_ScheduledItems.IndexOf(sItem));
		}

		public void UpdateScheduledEvents()
		{
			try
			{
				this.m_TransactionMode = true;
				long num = Panel.TimeSinceStartupMs();
				int count = this.m_ScheduledItems.Count;
				int num2 = this.m_LastUpdatedIndex + 1;
				if (num2 >= count)
				{
					num2 = 0;
				}
				for (int i = 0; i < count; i++)
				{
					int num3 = num2 + i;
					if (num3 >= count)
					{
						num3 -= count;
					}
					ScheduledItem scheduledItem = this.m_ScheduledItems[num3];
					if (num - scheduledItem.delayMs >= scheduledItem.startMs)
					{
						TimerState timerState = new TimerState
						{
							start = scheduledItem.startMs,
							now = num
						};
						if (!this.m_UnscheduleTransactions.Contains(scheduledItem))
						{
							scheduledItem.PerformTimerUpdate(timerState);
						}
						scheduledItem.startMs = num;
						scheduledItem.delayMs = scheduledItem.intervalMs;
						if (scheduledItem.ShouldUnschedule() && !this.m_UnscheduleTransactions.Contains(scheduledItem))
						{
							this.Unschedule(scheduledItem);
						}
					}
					this.m_LastUpdatedIndex = num3;
				}
			}
			finally
			{
				this.m_TransactionMode = false;
				foreach (ScheduledItem scheduledItem2 in this.m_UnscheduleTransactions)
				{
					this.PrivateUnSchedule(scheduledItem2);
				}
				this.m_UnscheduleTransactions.Clear();
				foreach (ScheduledItem scheduledItem3 in this.m_ScheduleTransactions)
				{
					this.Schedule(scheduledItem3);
				}
				this.m_ScheduleTransactions.Clear();
			}
		}

		private readonly List<ScheduledItem> m_ScheduledItems = new List<ScheduledItem>();

		private bool m_TransactionMode;

		private readonly List<ScheduledItem> m_ScheduleTransactions = new List<ScheduledItem>();

		private readonly HashSet<ScheduledItem> m_UnscheduleTransactions = new HashSet<ScheduledItem>();

		internal bool disableThrottling = false;

		private int m_LastUpdatedIndex = -1;

		private class TimerEventSchedulerItem : ScheduledItem
		{
			public TimerEventSchedulerItem(Action<TimerState> updateEvent)
			{
				this.m_TimerUpdateEvent = updateEvent;
			}

			public override void PerformTimerUpdate(TimerState state)
			{
				if (this.m_TimerUpdateEvent != null)
				{
					this.m_TimerUpdateEvent(state);
				}
			}

			public override string ToString()
			{
				return this.m_TimerUpdateEvent.ToString();
			}

			private readonly Action<TimerState> m_TimerUpdateEvent;
		}
	}
}
