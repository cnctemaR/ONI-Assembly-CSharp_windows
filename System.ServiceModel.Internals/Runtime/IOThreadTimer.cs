using System;
using System.ComponentModel;
using System.Runtime.Interop;
using System.Security;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.Runtime
{
	internal class IOThreadTimer
	{
		public IOThreadTimer(Action<object> callback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet)
			: this(callback, callbackState, isTypicallyCanceledShortlyAfterBeingSet, 100)
		{
		}

		public IOThreadTimer(Action<object> callback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet, int maxSkewInMilliseconds)
		{
			this.callback = callback;
			this.callbackState = callbackState;
			this.maxSkew = Ticks.FromMilliseconds(maxSkewInMilliseconds);
			this.timerGroup = (isTypicallyCanceledShortlyAfterBeingSet ? IOThreadTimer.TimerManager.Value.VolatileTimerGroup : IOThreadTimer.TimerManager.Value.StableTimerGroup);
		}

		public static long SystemTimeResolutionTicks
		{
			get
			{
				if (IOThreadTimer.systemTimeResolutionTicks == -1L)
				{
					IOThreadTimer.systemTimeResolutionTicks = IOThreadTimer.GetSystemTimeResolution();
				}
				return IOThreadTimer.systemTimeResolutionTicks;
			}
		}

		[SecuritySafeCritical]
		private static long GetSystemTimeResolution()
		{
			int num;
			uint num2;
			uint num3;
			if (UnsafeNativeMethods.GetSystemTimeAdjustment(out num, out num2, out num3) != 0U)
			{
				return (long)((ulong)num2);
			}
			return 150000L;
		}

		public bool Cancel()
		{
			return IOThreadTimer.TimerManager.Value.Cancel(this);
		}

		public void Set(TimeSpan timeFromNow)
		{
			if (timeFromNow != TimeSpan.MaxValue)
			{
				this.SetAt(Ticks.Add(Ticks.Now, Ticks.FromTimeSpan(timeFromNow)));
			}
		}

		public void Set(int millisecondsFromNow)
		{
			this.SetAt(Ticks.Add(Ticks.Now, Ticks.FromMilliseconds(millisecondsFromNow)));
		}

		public void SetAt(long dueTime)
		{
			IOThreadTimer.TimerManager.Value.Set(this, dueTime);
		}

		private const int maxSkewInMillisecondsDefault = 100;

		private static long systemTimeResolutionTicks = -1L;

		private Action<object> callback;

		private object callbackState;

		private long dueTime;

		private int index;

		private long maxSkew;

		private IOThreadTimer.TimerGroup timerGroup;

		private class TimerManager
		{
			public TimerManager()
			{
				this.onWaitCallback = new Action<object>(this.OnWaitCallback);
				this.stableTimerGroup = new IOThreadTimer.TimerGroup();
				this.volatileTimerGroup = new IOThreadTimer.TimerGroup();
				this.waitableTimers = new IOThreadTimer.WaitableTimer[]
				{
					this.stableTimerGroup.WaitableTimer,
					this.volatileTimerGroup.WaitableTimer
				};
			}

			private object ThisLock
			{
				get
				{
					return this;
				}
			}

			public static IOThreadTimer.TimerManager Value
			{
				get
				{
					return IOThreadTimer.TimerManager.value;
				}
			}

			public IOThreadTimer.TimerGroup StableTimerGroup
			{
				get
				{
					return this.stableTimerGroup;
				}
			}

			public IOThreadTimer.TimerGroup VolatileTimerGroup
			{
				get
				{
					return this.volatileTimerGroup;
				}
			}

			public void Set(IOThreadTimer timer, long dueTime)
			{
				long num = dueTime - timer.dueTime;
				if (num < 0L)
				{
					num = -num;
				}
				if (num > timer.maxSkew)
				{
					object thisLock = this.ThisLock;
					lock (thisLock)
					{
						IOThreadTimer.TimerGroup timerGroup = timer.timerGroup;
						IOThreadTimer.TimerQueue timerQueue = timerGroup.TimerQueue;
						if (timer.index > 0)
						{
							if (timerQueue.UpdateTimer(timer, dueTime))
							{
								this.UpdateWaitableTimer(timerGroup);
							}
						}
						else if (timerQueue.InsertTimer(timer, dueTime))
						{
							this.UpdateWaitableTimer(timerGroup);
							if (timerQueue.Count == 1)
							{
								this.EnsureWaitScheduled();
							}
						}
					}
				}
			}

			public bool Cancel(IOThreadTimer timer)
			{
				object thisLock = this.ThisLock;
				bool flag2;
				lock (thisLock)
				{
					if (timer.index > 0)
					{
						IOThreadTimer.TimerGroup timerGroup = timer.timerGroup;
						IOThreadTimer.TimerQueue timerQueue = timerGroup.TimerQueue;
						timerQueue.DeleteTimer(timer);
						if (timerQueue.Count > 0)
						{
							this.UpdateWaitableTimer(timerGroup);
						}
						else
						{
							IOThreadTimer.TimerGroup otherTimerGroup = this.GetOtherTimerGroup(timerGroup);
							if (otherTimerGroup.TimerQueue.Count == 0)
							{
								long now = Ticks.Now;
								long num = timerGroup.WaitableTimer.DueTime - now;
								long num2 = otherTimerGroup.WaitableTimer.DueTime - now;
								if (num > 10000000L && num2 > 10000000L)
								{
									timerGroup.WaitableTimer.Set(Ticks.Add(now, 10000000L));
								}
							}
						}
						flag2 = true;
					}
					else
					{
						flag2 = false;
					}
				}
				return flag2;
			}

			private void EnsureWaitScheduled()
			{
				if (!this.waitScheduled)
				{
					this.ScheduleWait();
				}
			}

			private IOThreadTimer.TimerGroup GetOtherTimerGroup(IOThreadTimer.TimerGroup timerGroup)
			{
				if (timerGroup == this.volatileTimerGroup)
				{
					return this.stableTimerGroup;
				}
				return this.volatileTimerGroup;
			}

			private void OnWaitCallback(object state)
			{
				WaitHandle[] array = this.waitableTimers;
				WaitHandle.WaitAny(array);
				long now = Ticks.Now;
				object thisLock = this.ThisLock;
				lock (thisLock)
				{
					this.waitScheduled = false;
					this.ScheduleElapsedTimers(now);
					this.ReactivateWaitableTimers();
					this.ScheduleWaitIfAnyTimersLeft();
				}
			}

			private void ReactivateWaitableTimers()
			{
				this.ReactivateWaitableTimer(this.stableTimerGroup);
				this.ReactivateWaitableTimer(this.volatileTimerGroup);
			}

			private void ReactivateWaitableTimer(IOThreadTimer.TimerGroup timerGroup)
			{
				IOThreadTimer.TimerQueue timerQueue = timerGroup.TimerQueue;
				if (timerQueue.Count > 0)
				{
					timerGroup.WaitableTimer.Set(timerQueue.MinTimer.dueTime);
					return;
				}
				timerGroup.WaitableTimer.Set(long.MaxValue);
			}

			private void ScheduleElapsedTimers(long now)
			{
				this.ScheduleElapsedTimers(this.stableTimerGroup, now);
				this.ScheduleElapsedTimers(this.volatileTimerGroup, now);
			}

			private void ScheduleElapsedTimers(IOThreadTimer.TimerGroup timerGroup, long now)
			{
				IOThreadTimer.TimerQueue timerQueue = timerGroup.TimerQueue;
				while (timerQueue.Count > 0)
				{
					IOThreadTimer minTimer = timerQueue.MinTimer;
					if (minTimer.dueTime - now > minTimer.maxSkew)
					{
						break;
					}
					timerQueue.DeleteMinTimer();
					ActionItem.Schedule(minTimer.callback, minTimer.callbackState);
				}
			}

			private void ScheduleWait()
			{
				ActionItem.Schedule(this.onWaitCallback, null);
				this.waitScheduled = true;
			}

			private void ScheduleWaitIfAnyTimersLeft()
			{
				if (this.stableTimerGroup.TimerQueue.Count > 0 || this.volatileTimerGroup.TimerQueue.Count > 0)
				{
					this.ScheduleWait();
				}
			}

			private void UpdateWaitableTimer(IOThreadTimer.TimerGroup timerGroup)
			{
				IOThreadTimer.WaitableTimer waitableTimer = timerGroup.WaitableTimer;
				IOThreadTimer minTimer = timerGroup.TimerQueue.MinTimer;
				long num = waitableTimer.DueTime - minTimer.dueTime;
				if (num < 0L)
				{
					num = -num;
				}
				if (num > minTimer.maxSkew)
				{
					waitableTimer.Set(minTimer.dueTime);
				}
			}

			private const long maxTimeToWaitForMoreTimers = 10000000L;

			private static IOThreadTimer.TimerManager value = new IOThreadTimer.TimerManager();

			private Action<object> onWaitCallback;

			private IOThreadTimer.TimerGroup stableTimerGroup;

			private IOThreadTimer.TimerGroup volatileTimerGroup;

			private IOThreadTimer.WaitableTimer[] waitableTimers;

			private bool waitScheduled;
		}

		private class TimerGroup
		{
			public TimerGroup()
			{
				this.waitableTimer = new IOThreadTimer.WaitableTimer();
				this.waitableTimer.Set(long.MaxValue);
				this.timerQueue = new IOThreadTimer.TimerQueue();
			}

			public IOThreadTimer.TimerQueue TimerQueue
			{
				get
				{
					return this.timerQueue;
				}
			}

			public IOThreadTimer.WaitableTimer WaitableTimer
			{
				get
				{
					return this.waitableTimer;
				}
			}

			private IOThreadTimer.TimerQueue timerQueue;

			private IOThreadTimer.WaitableTimer waitableTimer;
		}

		private class TimerQueue
		{
			public TimerQueue()
			{
				this.timers = new IOThreadTimer[4];
			}

			public int Count
			{
				get
				{
					return this.count;
				}
			}

			public IOThreadTimer MinTimer
			{
				get
				{
					return this.timers[1];
				}
			}

			public void DeleteMinTimer()
			{
				IOThreadTimer minTimer = this.MinTimer;
				this.DeleteMinTimerCore();
				minTimer.index = 0;
				minTimer.dueTime = 0L;
			}

			public void DeleteTimer(IOThreadTimer timer)
			{
				int num = timer.index;
				IOThreadTimer[] array = this.timers;
				for (;;)
				{
					int num2 = num / 2;
					if (num2 < 1)
					{
						break;
					}
					IOThreadTimer iothreadTimer = array[num2];
					array[num] = iothreadTimer;
					iothreadTimer.index = num;
					num = num2;
				}
				timer.index = 0;
				timer.dueTime = 0L;
				array[1] = null;
				this.DeleteMinTimerCore();
			}

			public bool InsertTimer(IOThreadTimer timer, long dueTime)
			{
				IOThreadTimer[] array = this.timers;
				int num = this.count + 1;
				if (num == array.Length)
				{
					array = new IOThreadTimer[array.Length * 2];
					Array.Copy(this.timers, array, this.timers.Length);
					this.timers = array;
				}
				this.count = num;
				if (num > 1)
				{
					for (;;)
					{
						int num2 = num / 2;
						if (num2 == 0)
						{
							break;
						}
						IOThreadTimer iothreadTimer = array[num2];
						if (iothreadTimer.dueTime <= dueTime)
						{
							break;
						}
						array[num] = iothreadTimer;
						iothreadTimer.index = num;
						num = num2;
					}
				}
				array[num] = timer;
				timer.index = num;
				timer.dueTime = dueTime;
				return num == 1;
			}

			public bool UpdateTimer(IOThreadTimer timer, long dueTime)
			{
				int index = timer.index;
				IOThreadTimer[] array = this.timers;
				int num = this.count;
				int num2 = index / 2;
				if (num2 == 0 || array[num2].dueTime <= dueTime)
				{
					int num3 = index * 2;
					if (num3 > num || array[num3].dueTime >= dueTime)
					{
						int num4 = num3 + 1;
						if (num4 > num || array[num4].dueTime >= dueTime)
						{
							timer.dueTime = dueTime;
							return index == 1;
						}
					}
				}
				this.DeleteTimer(timer);
				this.InsertTimer(timer, dueTime);
				return true;
			}

			private void DeleteMinTimerCore()
			{
				int num = this.count;
				if (num == 1)
				{
					this.count = 0;
					this.timers[1] = null;
					return;
				}
				IOThreadTimer[] array = this.timers;
				IOThreadTimer iothreadTimer = array[num];
				num = (this.count = num - 1);
				int num2 = 1;
				int num3;
				do
				{
					num3 = num2 * 2;
					if (num3 > num)
					{
						break;
					}
					IOThreadTimer iothreadTimer4;
					int num5;
					if (num3 < num)
					{
						IOThreadTimer iothreadTimer2 = array[num3];
						int num4 = num3 + 1;
						IOThreadTimer iothreadTimer3 = array[num4];
						if (iothreadTimer3.dueTime < iothreadTimer2.dueTime)
						{
							iothreadTimer4 = iothreadTimer3;
							num5 = num4;
						}
						else
						{
							iothreadTimer4 = iothreadTimer2;
							num5 = num3;
						}
					}
					else
					{
						num5 = num3;
						iothreadTimer4 = array[num5];
					}
					if (iothreadTimer.dueTime <= iothreadTimer4.dueTime)
					{
						break;
					}
					array[num2] = iothreadTimer4;
					iothreadTimer4.index = num2;
					num2 = num5;
				}
				while (num3 < num);
				array[num2] = iothreadTimer;
				iothreadTimer.index = num2;
				array[num + 1] = null;
			}

			private int count;

			private IOThreadTimer[] timers;
		}

		private class WaitableTimer : WaitHandle
		{
			[SecuritySafeCritical]
			public WaitableTimer()
			{
				base.SafeWaitHandle = IOThreadTimer.WaitableTimer.TimerHelper.CreateWaitableTimer();
			}

			public long DueTime
			{
				get
				{
					return this.dueTime;
				}
			}

			[SecuritySafeCritical]
			public void Set(long dueTime)
			{
				this.dueTime = IOThreadTimer.WaitableTimer.TimerHelper.Set(base.SafeWaitHandle, dueTime);
			}

			private long dueTime;

			[SecurityCritical]
			private static class TimerHelper
			{
				public static SafeWaitHandle CreateWaitableTimer()
				{
					SafeWaitHandle safeWaitHandle = UnsafeNativeMethods.CreateWaitableTimer(IntPtr.Zero, false, null);
					if (safeWaitHandle.IsInvalid)
					{
						Exception ex = new Win32Exception();
						safeWaitHandle.SetHandleAsInvalid();
						throw Fx.Exception.AsError(ex);
					}
					return safeWaitHandle;
				}

				public static long Set(SafeWaitHandle timer, long dueTime)
				{
					if (!UnsafeNativeMethods.SetWaitableTimer(timer, ref dueTime, 0, IntPtr.Zero, IntPtr.Zero, false))
					{
						throw Fx.Exception.AsError(new Win32Exception());
					}
					return dueTime;
				}
			}
		}
	}
}
