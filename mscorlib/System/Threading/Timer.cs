using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
			long num = (long)((dueTime == uint.MaxValue) ? ulong.MaxValue : ((ulong)dueTime));
			long num2 = (long)((period == uint.MaxValue) ? ulong.MaxValue : ((ulong)period));
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
			long num = (long)((dueTime == uint.MaxValue) ? ulong.MaxValue : ((ulong)dueTime));
			long num2 = (long)((period == uint.MaxValue) ? ulong.MaxValue : ((ulong)period));
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
				throw new ArgumentOutOfRangeException("dueTime", "Due time too large");
			}
			if (period > (long)((ulong)(-2)))
			{
				throw new ArgumentOutOfRangeException("period", "Period too large");
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
				throw new ObjectDisposedException(null, Environment.GetResourceString("Cannot access a disposed object."));
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
				num = dueTime * 10000L + Timer.GetTimeMonotonic();
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
			NativeEventCalls.SetEvent(notifyObject.SafeWaitHandle);
			return true;
		}

		internal void KeepRootedWhileScheduled()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetTimeMonotonic();

		private static readonly Timer.Scheduler scheduler = Timer.Scheduler.Instance;

		private TimerCallback callback;

		private object state;

		private long due_time_ms;

		private long period_ms;

		private long next_run;

		private bool disposed;

		private const long MaxValue = 4294967294L;

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
					if (x != y)
					{
						return -1;
					}
					return 0;
				}
				else
				{
					if (num <= 0L)
					{
						return -1;
					}
					return 1;
				}
			}
		}

		private sealed class Scheduler
		{
			public static Timer.Scheduler Instance
			{
				get
				{
					return Timer.Scheduler.instance;
				}
			}

			private Scheduler()
			{
				this.changed = new ManualResetEvent(false);
				this.list = new SortedList(new Timer.TimerComparer(), 1024);
				new Thread(new ThreadStart(this.SchedulerThread))
				{
					IsBackground = true
				}.Start();
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
				bool flag = false;
				lock (this)
				{
					this.InternalRemove(timer);
					if (new_next_run == 9223372036854775807L)
					{
						timer.next_run = new_next_run;
						return;
					}
					if (!timer.disposed)
					{
						timer.next_run = new_next_run;
						this.Add(timer);
						flag = this.list.GetByIndex(0) == timer;
					}
				}
				if (flag)
				{
					this.changed.Set();
				}
			}

			private int FindByDueTime(long nr)
			{
				int i = 0;
				int num = this.list.Count - 1;
				if (num < 0)
				{
					return -1;
				}
				if (num < 20)
				{
					while (i <= num)
					{
						Timer timer = (Timer)this.list.GetByIndex(i);
						if (timer.next_run == nr)
						{
							return i;
						}
						if (timer.next_run > nr)
						{
							return -1;
						}
						i++;
					}
					return -1;
				}
				while (i <= num)
				{
					int num2 = i + (num - i >> 1);
					Timer timer2 = (Timer)this.list.GetByIndex(num2);
					if (nr == timer2.next_run)
					{
						return num2;
					}
					if (nr > timer2.next_run)
					{
						i = num2 + 1;
					}
					else
					{
						num = num2 - 1;
					}
				}
				return -1;
			}

			private void Add(Timer timer)
			{
				int num = this.FindByDueTime(timer.next_run);
				if (num != -1)
				{
					bool flag = long.MaxValue - timer.next_run > 20000L;
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
					}
					while (num < this.list.Count && ((Timer)this.list.GetByIndex(num)).next_run == timer.next_run);
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

			private static void TimerCB(object o)
			{
				Timer timer = (Timer)o;
				timer.callback(timer.state);
			}

			private void SchedulerThread()
			{
				Thread.CurrentThread.Name = "Timer-Scheduler";
				List<Timer> list = new List<Timer>(512);
				for (;;)
				{
					int num = -1;
					long timeMonotonic = Timer.GetTimeMonotonic();
					lock (this)
					{
						this.changed.Reset();
						int num2 = this.list.Count;
						for (int i = 0; i < num2; i++)
						{
							Timer timer = (Timer)this.list.GetByIndex(i);
							if (timer.next_run > timeMonotonic)
							{
								break;
							}
							this.list.RemoveAt(i);
							num2--;
							i--;
							ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(Timer.Scheduler.TimerCB), timer);
							long period_ms = timer.period_ms;
							long due_time_ms = timer.due_time_ms;
							if (period_ms == -1L || ((period_ms == 0L || period_ms == -1L) && due_time_ms != -1L))
							{
								timer.next_run = long.MaxValue;
							}
							else
							{
								timer.next_run = Timer.GetTimeMonotonic() + 10000L * timer.period_ms;
								list.Add(timer);
							}
						}
						num2 = list.Count;
						for (int i = 0; i < num2; i++)
						{
							Timer timer2 = list[i];
							this.Add(timer2);
						}
						list.Clear();
						this.ShrinkIfNeeded(list, 512);
						int capacity = this.list.Capacity;
						num2 = this.list.Count;
						if (capacity > 1024 && num2 > 0 && capacity / num2 > 3)
						{
							this.list.Capacity = num2 * 2;
						}
						long num3 = long.MaxValue;
						if (this.list.Count > 0)
						{
							num3 = ((Timer)this.list.GetByIndex(0)).next_run;
						}
						num = -1;
						if (num3 != 9223372036854775807L)
						{
							long num4 = (num3 - Timer.GetTimeMonotonic()) / 10000L;
							if (num4 > 2147483647L)
							{
								num = 2147483646;
							}
							else
							{
								num = (int)num4;
								if (num < 0)
								{
									num = 0;
								}
							}
						}
					}
					this.changed.WaitOne(num);
				}
			}

			private void ShrinkIfNeeded(List<Timer> list, int initial)
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

			private ManualResetEvent changed;
		}
	}
}
