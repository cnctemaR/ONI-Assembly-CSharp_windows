using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public sealed class Timer : MarshalByRefObject, IDisposable
	{
		public Timer(TimerCallback callback, object state, int dueTime, int period)
		{
			this.Init(callback, state, (long)dueTime, (long)period);
		}

		public Timer(TimerCallback callback, object state, long dueTime, long period)
		{
			this.Init(callback, state, dueTime, period);
		}

		public Timer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
		{
			this.Init(callback, state, (long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds);
		}

		[CLSCompliant(false)]
		public Timer(TimerCallback callback, object state, uint dueTime, uint period)
		{
			long num = (long)((dueTime != uint.MaxValue) ? ((ulong)dueTime) : ulong.MaxValue);
			long num2 = (long)((period != uint.MaxValue) ? ((ulong)period) : ulong.MaxValue);
			this.Init(callback, state, num, num2);
		}

		public Timer(TimerCallback callback)
		{
			this.Init(callback, this, -1L, -1L);
		}

		private void Init(TimerCallback callback, object state, long dueTime, long period)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			this.callback = callback;
			this.state = state;
			this.Change(dueTime, period, true);
		}

		public bool Change(int dueTime, int period)
		{
			return this.Change((long)dueTime, (long)period, false);
		}

		public bool Change(TimeSpan dueTime, TimeSpan period)
		{
			return this.Change((long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds, false);
		}

		[CLSCompliant(false)]
		public bool Change(uint dueTime, uint period)
		{
			long num = (long)((dueTime != uint.MaxValue) ? ((ulong)dueTime) : ulong.MaxValue);
			long num2 = (long)((period != uint.MaxValue) ? ((ulong)period) : ulong.MaxValue);
			return this.Change(num, num2, false);
		}

		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			Timer.scheduler.Remove(this);
		}

		public bool Change(long dueTime, long period)
		{
			return this.Change(dueTime, period, false);
		}

		private bool Change(long dueTime, long period, bool first)
		{
			if (dueTime > (long)((ulong)(-2)))
			{
				throw new ArgumentOutOfRangeException("Due time too large");
			}
			if (period > (long)((ulong)(-2)))
			{
				throw new ArgumentOutOfRangeException("Period too large");
			}
			if (dueTime < -1L)
			{
				throw new ArgumentOutOfRangeException("dueTime");
			}
			if (period < -1L)
			{
				throw new ArgumentOutOfRangeException("period");
			}
			if (this.disposed)
			{
				return false;
			}
			this.due_time_ms = dueTime;
			this.period_ms = period;
			long num;
			if (dueTime == 0L)
			{
				num = 0L;
			}
			else if (dueTime < 0L)
			{
				num = long.MaxValue;
				if (first)
				{
					this.next_run = num;
					return true;
				}
			}
			else
			{
				num = dueTime * 10000L + DateTime.GetTimeMonotonic();
			}
			Timer.scheduler.Change(this, num);
			return true;
		}

		public bool Dispose(WaitHandle notifyObject)
		{
			if (notifyObject == null)
			{
				throw new ArgumentNullException("notifyObject");
			}
			this.Dispose();
			NativeEventCalls.SetEvent_internal(notifyObject.Handle);
			return true;
		}

		private const long MaxValue = 4294967294L;

		private static Timer.Scheduler scheduler = Timer.Scheduler.Instance;

		private TimerCallback callback;

		private object state;

		private long due_time_ms;

		private long period_ms;

		private long next_run;

		private bool disposed;

		private sealed class TimerComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Timer timer = x as Timer;
				if (timer == null)
				{
					return -1;
				}
				Timer timer2 = y as Timer;
				if (timer2 == null)
				{
					return 1;
				}
				long num = timer.next_run - timer2.next_run;
				if (num == 0L)
				{
					return 0;
				}
				return (num <= 0L) ? (-1) : 1;
			}
		}

		private sealed class Scheduler
		{
			private Scheduler()
			{
				this.list = new SortedList(new Timer.TimerComparer(), 1024);
				new Thread(new ThreadStart(this.SchedulerThread))
				{
					IsBackground = true
				}.Start();
			}

			public static Timer.Scheduler Instance
			{
				get
				{
					return Timer.Scheduler.instance;
				}
			}

			public void Remove(Timer timer)
			{
				if (timer.next_run == 0L || timer.next_run == 9223372036854775807L)
				{
					return;
				}
				lock (this)
				{
					this.InternalRemove(timer);
				}
			}

			public void Change(Timer timer, long new_next_run)
			{
				lock (this)
				{
					this.InternalRemove(timer);
					if (new_next_run == 9223372036854775807L)
					{
						timer.next_run = new_next_run;
					}
					else if (!timer.disposed)
					{
						timer.next_run = new_next_run;
						this.Add(timer);
						if (this.list.GetByIndex(0) == timer)
						{
							Monitor.Pulse(this);
						}
					}
				}
			}

			private void Add(Timer timer)
			{
				int num = this.list.IndexOfKey(timer);
				if (num != -1)
				{
					bool flag = long.MaxValue - timer.next_run > 20000L;
					Timer timer2;
					do
					{
						num++;
						if (flag)
						{
							timer.next_run += 1L;
						}
						else
						{
							timer.next_run -= 1L;
						}
						if (num >= this.list.Count)
						{
							break;
						}
						timer2 = (Timer)this.list.GetByIndex(num);
					}
					while (timer2.next_run == timer.next_run);
				}
				this.list.Add(timer, timer);
			}

			private int InternalRemove(Timer timer)
			{
				int num = this.list.IndexOfKey(timer);
				if (num >= 0)
				{
					this.list.RemoveAt(num);
				}
				return num;
			}

			private void SchedulerThread()
			{
				Thread.CurrentThread.Name = "Timer-Scheduler";
				ArrayList arrayList = new ArrayList(512);
				for (;;)
				{
					long timeMonotonic = DateTime.GetTimeMonotonic();
					lock (this)
					{
						int num = this.list.Count;
						for (int i = 0; i < num; i++)
						{
							Timer timer = (Timer)this.list.GetByIndex(i);
							if (timer.next_run > timeMonotonic)
							{
								break;
							}
							this.list.RemoveAt(i);
							num--;
							i--;
							ThreadPool.QueueUserWorkItem(new WaitCallback(timer.callback.Invoke), timer.state);
							long period_ms = timer.period_ms;
							long due_time_ms = timer.due_time_ms;
							bool flag = period_ms == -1L || ((period_ms == 0L || period_ms == -1L) && due_time_ms != -1L);
							if (flag)
							{
								timer.next_run = long.MaxValue;
							}
							else
							{
								timer.next_run = DateTime.GetTimeMonotonic() + 10000L * timer.period_ms;
								arrayList.Add(timer);
							}
						}
						num = arrayList.Count;
						for (int i = 0; i < num; i++)
						{
							Timer timer2 = (Timer)arrayList[i];
							this.Add(timer2);
						}
						arrayList.Clear();
						this.ShrinkIfNeeded(arrayList, 512);
						int capacity = this.list.Capacity;
						num = this.list.Count;
						if (capacity > 1024 && num > 0 && capacity / num > 3)
						{
							this.list.Capacity = num * 2;
						}
						long num2 = long.MaxValue;
						if (this.list.Count > 0)
						{
							num2 = ((Timer)this.list.GetByIndex(0)).next_run;
						}
						int num3 = -1;
						if (num2 != 9223372036854775807L)
						{
							long num4 = num2 - DateTime.GetTimeMonotonic();
							num3 = (int)(num4 / 10000L);
							if (num3 < 0)
							{
								num3 = 0;
							}
						}
						Monitor.Wait(this, num3);
					}
				}
			}

			private void ShrinkIfNeeded(ArrayList list, int initial)
			{
				int capacity = list.Capacity;
				int count = list.Count;
				if (capacity > initial && count > 0 && capacity / count > 3)
				{
					list.Capacity = count * 2;
				}
			}

			private static Timer.Scheduler instance = new Timer.Scheduler();

			private SortedList list;
		}
	}
}
