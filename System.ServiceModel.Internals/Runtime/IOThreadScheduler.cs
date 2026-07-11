using System;
using System.Security;
using System.Threading;

namespace System.Runtime
{
	internal class IOThreadScheduler
	{
		[SecuritySafeCritical]
		private IOThreadScheduler(int capacity, int capacityLowPri)
		{
			this.slots = new IOThreadScheduler.Slot[capacity];
			this.slotsLowPri = new IOThreadScheduler.Slot[capacityLowPri];
			this.overlapped = new IOThreadScheduler.ScheduledOverlapped();
		}

		[SecurityCritical]
		public static void ScheduleCallbackNoFlow(Action<object> callback, object state)
		{
			if (callback == null)
			{
				throw Fx.Exception.ArgumentNull("callback");
			}
			bool flag = false;
			while (!flag)
			{
				try
				{
				}
				finally
				{
					flag = IOThreadScheduler.current.ScheduleCallbackHelper(callback, state);
				}
			}
		}

		[SecurityCritical]
		public static void ScheduleCallbackLowPriNoFlow(Action<object> callback, object state)
		{
			if (callback == null)
			{
				throw Fx.Exception.ArgumentNull("callback");
			}
			bool flag = false;
			while (!flag)
			{
				try
				{
				}
				finally
				{
					flag = IOThreadScheduler.current.ScheduleCallbackLowPriHelper(callback, state);
				}
			}
		}

		[SecurityCritical]
		private bool ScheduleCallbackHelper(Action<object> callback, object state)
		{
			int num = Interlocked.Add(ref this.headTail, 65536);
			bool flag = IOThreadScheduler.Bits.Count(num) == 0;
			if (flag)
			{
				num = Interlocked.Add(ref this.headTail, 65536);
			}
			if (IOThreadScheduler.Bits.Count(num) == -1)
			{
				throw Fx.AssertAndThrowFatal("Head/Tail overflow!");
			}
			bool flag3;
			bool flag2 = this.slots[(num >> 16) & this.SlotMask].TryEnqueueWorkItem(callback, state, out flag3);
			if (flag3)
			{
				IOThreadScheduler iothreadScheduler = new IOThreadScheduler(Math.Min(this.slots.Length * 2, 32768), this.slotsLowPri.Length);
				Interlocked.CompareExchange<IOThreadScheduler>(ref IOThreadScheduler.current, iothreadScheduler, this);
			}
			if (flag)
			{
				this.overlapped.Post(this);
			}
			return flag2;
		}

		[SecurityCritical]
		private bool ScheduleCallbackLowPriHelper(Action<object> callback, object state)
		{
			int num = Interlocked.Add(ref this.headTailLowPri, 65536);
			bool flag = false;
			if (IOThreadScheduler.Bits.CountNoIdle(num) == 1)
			{
				int num2 = this.headTail;
				if (IOThreadScheduler.Bits.Count(num2) == -1)
				{
					int num3 = Interlocked.CompareExchange(ref this.headTail, num2 + 65536, num2);
					if (num2 == num3)
					{
						flag = true;
					}
				}
			}
			if (IOThreadScheduler.Bits.CountNoIdle(num) == 0)
			{
				throw Fx.AssertAndThrowFatal("Low-priority Head/Tail overflow!");
			}
			bool flag3;
			bool flag2 = this.slotsLowPri[(num >> 16) & this.SlotMaskLowPri].TryEnqueueWorkItem(callback, state, out flag3);
			if (flag3)
			{
				IOThreadScheduler iothreadScheduler = new IOThreadScheduler(this.slots.Length, Math.Min(this.slotsLowPri.Length * 2, 32768));
				Interlocked.CompareExchange<IOThreadScheduler>(ref IOThreadScheduler.current, iothreadScheduler, this);
			}
			if (flag)
			{
				this.overlapped.Post(this);
			}
			return flag2;
		}

		[SecurityCritical]
		private void CompletionCallback(out Action<object> callback, out object state)
		{
			int num = this.headTail;
			int num2;
			for (;;)
			{
				bool flag = IOThreadScheduler.Bits.Count(num) == 0;
				if (flag)
				{
					num2 = this.headTailLowPri;
					while (IOThreadScheduler.Bits.CountNoIdle(num2) != 0)
					{
						if (num2 == (num2 = Interlocked.CompareExchange(ref this.headTailLowPri, IOThreadScheduler.Bits.IncrementLo(num2), num2)))
						{
							goto Block_2;
						}
					}
				}
				if (num == (num = Interlocked.CompareExchange(ref this.headTail, IOThreadScheduler.Bits.IncrementLo(num), num)))
				{
					if (!flag)
					{
						goto Block_4;
					}
					num2 = this.headTailLowPri;
					if (IOThreadScheduler.Bits.CountNoIdle(num2) == 0)
					{
						goto IL_00DD;
					}
					num = IOThreadScheduler.Bits.IncrementLo(num);
					if (num != Interlocked.CompareExchange(ref this.headTail, num + 65536, num))
					{
						goto IL_00DD;
					}
					num += 65536;
				}
			}
			Block_2:
			this.overlapped.Post(this);
			this.slotsLowPri[num2 & this.SlotMaskLowPri].DequeueWorkItem(out callback, out state);
			return;
			Block_4:
			this.overlapped.Post(this);
			this.slots[num & this.SlotMask].DequeueWorkItem(out callback, out state);
			return;
			IL_00DD:
			callback = null;
			state = null;
		}

		[SecurityCritical]
		private bool TryCoalesce(out Action<object> callback, out object state)
		{
			int num = this.headTail;
			int num2;
			for (;;)
			{
				if (IOThreadScheduler.Bits.Count(num) > 0)
				{
					if (num == (num = Interlocked.CompareExchange(ref this.headTail, IOThreadScheduler.Bits.IncrementLo(num), num)))
					{
						break;
					}
				}
				else
				{
					num2 = this.headTailLowPri;
					if (IOThreadScheduler.Bits.CountNoIdle(num2) <= 0)
					{
						goto IL_0092;
					}
					if (num2 == (num2 = Interlocked.CompareExchange(ref this.headTailLowPri, IOThreadScheduler.Bits.IncrementLo(num2), num2)))
					{
						goto Block_4;
					}
					num = this.headTail;
				}
			}
			this.slots[num & this.SlotMask].DequeueWorkItem(out callback, out state);
			return true;
			Block_4:
			this.slotsLowPri[num2 & this.SlotMaskLowPri].DequeueWorkItem(out callback, out state);
			return true;
			IL_0092:
			callback = null;
			state = null;
			return false;
		}

		private int SlotMask
		{
			[SecurityCritical]
			get
			{
				return this.slots.Length - 1;
			}
		}

		private int SlotMaskLowPri
		{
			[SecurityCritical]
			get
			{
				return this.slotsLowPri.Length - 1;
			}
		}

		~IOThreadScheduler()
		{
			if (!Environment.HasShutdownStarted && !AppDomain.CurrentDomain.IsFinalizingForUnload())
			{
				this.Cleanup();
			}
		}

		[SecuritySafeCritical]
		private void Cleanup()
		{
			if (this.overlapped != null)
			{
				this.overlapped.Cleanup();
			}
		}

		private const int MaximumCapacity = 32768;

		private static IOThreadScheduler current = new IOThreadScheduler(32, 32);

		private readonly IOThreadScheduler.ScheduledOverlapped overlapped;

		[SecurityCritical]
		private readonly IOThreadScheduler.Slot[] slots;

		[SecurityCritical]
		private readonly IOThreadScheduler.Slot[] slotsLowPri;

		private int headTail = -131072;

		private int headTailLowPri = -65536;

		private static class Bits
		{
			public static int Count(int slot)
			{
				return (((slot >> 16) - slot + 2) & 65535) - 1;
			}

			public static int CountNoIdle(int slot)
			{
				return ((slot >> 16) - slot + 1) & 65535;
			}

			public static int IncrementLo(int slot)
			{
				return ((slot + 1) & 65535) | (slot & -65536);
			}

			public static bool IsComplete(int gate)
			{
				return (gate & -65536) == gate << 16;
			}

			public const int HiShift = 16;

			public const int HiOne = 65536;

			public const int LoHiBit = 32768;

			public const int HiHiBit = -2147483648;

			public const int LoCountMask = 32767;

			public const int HiCountMask = 2147418112;

			public const int LoMask = 65535;

			public const int HiMask = -65536;

			public const int HiBits = -2147450880;
		}

		private struct Slot
		{
			public bool TryEnqueueWorkItem(Action<object> callback, object state, out bool wrapped)
			{
				int num = Interlocked.Increment(ref this.gate);
				wrapped = (num & 32767) != 1;
				if (wrapped)
				{
					if ((num & 32768) != 0 && IOThreadScheduler.Bits.IsComplete(num))
					{
						Interlocked.CompareExchange(ref this.gate, 0, num);
					}
					return false;
				}
				this.state = state;
				this.callback = callback;
				num = Interlocked.Add(ref this.gate, 32768);
				if ((num & 2147418112) == 0)
				{
					return true;
				}
				this.state = null;
				this.callback = null;
				if (num >> 16 != (num & 32767) || Interlocked.CompareExchange(ref this.gate, 0, num) != num)
				{
					num = Interlocked.Add(ref this.gate, int.MinValue);
					if (IOThreadScheduler.Bits.IsComplete(num))
					{
						Interlocked.CompareExchange(ref this.gate, 0, num);
					}
				}
				return false;
			}

			public void DequeueWorkItem(out Action<object> callback, out object state)
			{
				int num = Interlocked.Add(ref this.gate, 65536);
				if ((num & 32768) == 0)
				{
					callback = null;
					state = null;
					return;
				}
				if ((num & 2147418112) == 65536)
				{
					callback = this.callback;
					state = this.state;
					this.state = null;
					this.callback = null;
					if ((num & 32767) != 1 || Interlocked.CompareExchange(ref this.gate, 0, num) != num)
					{
						num = Interlocked.Add(ref this.gate, int.MinValue);
						if (IOThreadScheduler.Bits.IsComplete(num))
						{
							Interlocked.CompareExchange(ref this.gate, 0, num);
							return;
						}
					}
				}
				else
				{
					callback = null;
					state = null;
					if (IOThreadScheduler.Bits.IsComplete(num))
					{
						Interlocked.CompareExchange(ref this.gate, 0, num);
					}
				}
			}

			private int gate;

			private Action<object> callback;

			private object state;
		}

		[SecurityCritical]
		private class ScheduledOverlapped
		{
			public ScheduledOverlapped()
			{
				this.nativeOverlapped = new Overlapped().UnsafePack(Fx.ThunkCallback(new IOCompletionCallback(this.IOCallback)), null);
			}

			private unsafe void IOCallback(uint errorCode, uint numBytes, NativeOverlapped* nativeOverlapped)
			{
				IOThreadScheduler iothreadScheduler = this.scheduler;
				this.scheduler = null;
				Action<object> action;
				object obj;
				try
				{
				}
				finally
				{
					iothreadScheduler.CompletionCallback(out action, out obj);
				}
				bool flag = true;
				while (flag)
				{
					if (action != null)
					{
						action(obj);
					}
					try
					{
					}
					finally
					{
						flag = iothreadScheduler.TryCoalesce(out action, out obj);
					}
				}
			}

			public void Post(IOThreadScheduler iots)
			{
				this.scheduler = iots;
				ThreadPool.UnsafeQueueNativeOverlapped(this.nativeOverlapped);
			}

			public void Cleanup()
			{
				if (this.scheduler != null)
				{
					throw Fx.AssertAndThrowFatal("Cleanup called on an overlapped that is in-flight.");
				}
				Overlapped.Free(this.nativeOverlapped);
			}

			private unsafe readonly NativeOverlapped* nativeOverlapped;

			private IOThreadScheduler scheduler;
		}
	}
}
