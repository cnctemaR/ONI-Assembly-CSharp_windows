using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class TimerEventScheduler
	{
		public TimerEventScheduler(BaseVisualElementPanel p)
		{
			this.panel = p;
		}

		public void Schedule(ScheduledItem item)
		{
			bool flag = item == null;
			if (!flag)
			{
				bool flag2 = item == null;
				if (flag2)
				{
					throw new NotSupportedException("Scheduled Item type is not supported by this scheduler");
				}
				bool transactionMode = this.m_TransactionMode;
				if (transactionMode)
				{
					bool flag3 = this.m_UnscheduleTransactions.Remove(item);
					if (!flag3)
					{
						bool flag4 = this.m_ScheduledItems.Contains(item) || this.m_ScheduleTransactions.Contains(item);
						if (flag4)
						{
							throw new ArgumentException("Cannot schedule function " + item + " more than once");
						}
						this.m_ScheduleTransactions.Add(item);
					}
				}
				else
				{
					bool flag5 = this.m_ScheduledItems.Contains(item);
					if (flag5)
					{
						throw new ArgumentException("Cannot schedule function " + item + " more than once");
					}
					this.m_ScheduledItems.Add(item);
				}
			}
		}

		public ScheduledItem ScheduleOnce(Action<TimerState> timerUpdateEvent, long delayMs)
		{
			TimerEventScheduler.TimerEventSchedulerItem timerEventSchedulerItem = new TimerEventScheduler.TimerEventSchedulerItem(this.panel.TimeSinceStartupMs(), timerUpdateEvent)
			{
				delayMs = delayMs
			};
			this.Schedule(timerEventSchedulerItem);
			return timerEventSchedulerItem;
		}

		public ScheduledItem ScheduleUntil(Action<TimerState> timerUpdateEvent, long delayMs, long intervalMs, Func<bool> stopCondition)
		{
			TimerEventScheduler.TimerEventSchedulerItem timerEventSchedulerItem = new TimerEventScheduler.TimerEventSchedulerItem(this.panel.TimeSinceStartupMs(), timerUpdateEvent)
			{
				delayMs = delayMs,
				intervalMs = intervalMs,
				timerUpdateStopCondition = stopCondition
			};
			this.Schedule(timerEventSchedulerItem);
			return timerEventSchedulerItem;
		}

		public ScheduledItem ScheduleForDuration(Action<TimerState> timerUpdateEvent, long delayMs, long intervalMs, long durationMs)
		{
			TimerEventScheduler.TimerEventSchedulerItem timerEventSchedulerItem = new TimerEventScheduler.TimerEventSchedulerItem(this.panel.TimeSinceStartupMs(), timerUpdateEvent)
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
			bool flag = index >= 0;
			bool flag3;
			if (flag)
			{
				bool flag2 = index <= this.m_LastUpdatedIndex;
				if (flag2)
				{
					this.m_LastUpdatedIndex--;
				}
				this.m_ScheduledItems.RemoveAt(index);
				flag3 = true;
			}
			else
			{
				flag3 = false;
			}
			return flag3;
		}

		public void Unschedule(ScheduledItem item)
		{
			bool flag = item != null;
			if (flag)
			{
				bool transactionMode = this.m_TransactionMode;
				if (transactionMode)
				{
					bool flag2 = this.m_UnscheduleTransactions.Contains(item);
					if (flag2)
					{
						throw new ArgumentException("Cannot unschedule scheduled function twice" + ((item != null) ? item.ToString() : null));
					}
					bool flag3 = this.m_ScheduleTransactions.Remove(item);
					if (!flag3)
					{
						bool flag4 = this.m_ScheduledItems.Contains(item);
						if (!flag4)
						{
							throw new ArgumentException("Cannot unschedule unknown scheduled function " + ((item != null) ? item.ToString() : null));
						}
						this.m_UnscheduleTransactions.Add(item);
					}
				}
				else
				{
					bool flag5 = !this.PrivateUnSchedule(item);
					if (flag5)
					{
						throw new ArgumentException("Cannot unschedule unknown scheduled function " + ((item != null) ? item.ToString() : null));
					}
				}
				item.OnItemUnscheduled();
			}
		}

		public void AdjustCurrentTime(double previousCurrentTime, double newCurrentTime)
		{
			bool flag = this.m_ScheduleTransactions.Count > 0 || this.m_UnscheduleTransactions.Count > 0;
			if (flag)
			{
				throw new InvalidOperationException("Adjusting time cannot be done while the scheduler is being updated");
			}
			long num = (long)((newCurrentTime - previousCurrentTime) * 1000.0);
			for (int i = 0; i < this.m_ScheduledItems.Count; i++)
			{
				this.m_ScheduledItems[i].OffsetBy(num);
			}
		}

		private bool PrivateUnSchedule(ScheduledItem sItem)
		{
			return this.m_ScheduleTransactions.Remove(sItem) || this.RemovedScheduledItemAt(this.m_ScheduledItems.IndexOf(sItem));
		}

		public long FrameCount
		{
			get
			{
				return this.frameCount;
			}
			set
			{
				this.frameCount = value;
			}
		}

		public void UpdateScheduledEvents()
		{
			bool flag = true;
			try
			{
				this.m_TransactionMode = true;
				long num = this.panel.TimeSinceStartupMs();
				int count = this.m_ScheduledItems.Count;
				int num2 = this.m_LastUpdatedIndex + 1;
				bool flag2 = num2 >= count;
				if (flag2)
				{
					num2 = 0;
				}
				for (int i = 0; i < count; i++)
				{
					int num3 = num2 + i;
					bool flag3 = num3 >= count;
					if (flag3)
					{
						num3 -= count;
					}
					ScheduledItem scheduledItem = this.m_ScheduledItems[num3];
					bool flag4 = false;
					bool flag5 = num - scheduledItem.delayMs >= scheduledItem.startMs;
					if (flag5)
					{
						TimerState timerState = new TimerState
						{
							start = scheduledItem.startMs,
							now = num
						};
						bool flag6 = !this.m_UnscheduleTransactions.Contains(scheduledItem);
						if (flag6)
						{
							scheduledItem.PerformTimerUpdate(timerState);
						}
						scheduledItem.startMs = num;
						scheduledItem.delayMs = scheduledItem.intervalMs;
						bool flag7 = scheduledItem.ShouldUnschedule();
						if (flag7)
						{
							flag4 = true;
						}
					}
					bool flag8 = flag4 || (scheduledItem.endTimeMs > 0L && num > scheduledItem.endTimeMs);
					if (flag8)
					{
						bool flag9 = !this.m_UnscheduleTransactions.Contains(scheduledItem);
						if (flag9)
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
				bool flag10 = flag;
				if (flag10)
				{
					long num4 = this.FrameCount + 1L;
					this.FrameCount = num4;
				}
			}
		}

		private readonly List<ScheduledItem> m_ScheduledItems = new List<ScheduledItem>();

		private bool m_TransactionMode;

		private readonly List<ScheduledItem> m_ScheduleTransactions = new List<ScheduledItem>();

		private readonly HashSet<ScheduledItem> m_UnscheduleTransactions = new HashSet<ScheduledItem>();

		internal bool disableThrottling = false;

		private int m_LastUpdatedIndex = -1;

		private BaseVisualElementPanel panel;

		private long frameCount;

		private class TimerEventSchedulerItem : ScheduledItem
		{
			public TimerEventSchedulerItem(long startMs, Action<TimerState> updateEvent)
				: base(startMs)
			{
				this.m_TimerUpdateEvent = updateEvent;
			}

			public override void PerformTimerUpdate(TimerState state)
			{
				Action<TimerState> timerUpdateEvent = this.m_TimerUpdateEvent;
				if (timerUpdateEvent != null)
				{
					timerUpdateEvent(state);
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
