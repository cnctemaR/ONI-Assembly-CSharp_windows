using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Threading
{
	[ComVisible(false)]
	[DebuggerDisplay("Set = {IsSet}")]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public class ManualResetEventSlim : IDisposable
	{
		public WaitHandle WaitHandle
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.m_eventObj == null)
				{
					this.LazyInitializeEvent();
				}
				return this.m_eventObj;
			}
		}

		public bool IsSet
		{
			get
			{
				return ManualResetEventSlim.ExtractStatePortion(this.m_combinedState, int.MinValue) != 0;
			}
			private set
			{
				this.UpdateStateAtomically((value ? 1 : 0) << 31, int.MinValue);
			}
		}

		public int SpinCount
		{
			get
			{
				return ManualResetEventSlim.ExtractStatePortionAndShiftRight(this.m_combinedState, 1073217536, 19);
			}
			private set
			{
				this.m_combinedState = (this.m_combinedState & -1073217537) | (value << 19);
			}
		}

		private int Waiters
		{
			get
			{
				return ManualResetEventSlim.ExtractStatePortionAndShiftRight(this.m_combinedState, 524287, 0);
			}
			set
			{
				if (value >= 524287)
				{
					throw new InvalidOperationException(string.Format(Environment.GetResourceString("There are too many threads currently waiting on the event. A maximum of {0} waiting threads are supported."), 524287));
				}
				this.UpdateStateAtomically(value, 524287);
			}
		}

		public ManualResetEventSlim()
			: this(false)
		{
		}

		public ManualResetEventSlim(bool initialState)
		{
			this.Initialize(initialState, 10);
		}

		public ManualResetEventSlim(bool initialState, int spinCount)
		{
			if (spinCount < 0)
			{
				throw new ArgumentOutOfRangeException("spinCount");
			}
			if (spinCount > 2047)
			{
				throw new ArgumentOutOfRangeException("spinCount", string.Format(Environment.GetResourceString("The spinCount argument must be in the range 0 to {0}, inclusive."), 2047));
			}
			this.Initialize(initialState, spinCount);
		}

		private void Initialize(bool initialState, int spinCount)
		{
			this.m_combinedState = (initialState ? int.MinValue : 0);
			this.SpinCount = (PlatformHelper.IsSingleProcessor ? 1 : spinCount);
		}

		private void EnsureLockObjectCreated()
		{
			if (this.m_lock != null)
			{
				return;
			}
			object obj = new object();
			Interlocked.CompareExchange(ref this.m_lock, obj, null);
		}

		private bool LazyInitializeEvent()
		{
			bool isSet = this.IsSet;
			ManualResetEvent manualResetEvent = new ManualResetEvent(isSet);
			if (Interlocked.CompareExchange<ManualResetEvent>(ref this.m_eventObj, manualResetEvent, null) != null)
			{
				manualResetEvent.Close();
				return false;
			}
			if (this.IsSet != isSet)
			{
				ManualResetEvent manualResetEvent2 = manualResetEvent;
				lock (manualResetEvent2)
				{
					if (this.m_eventObj == manualResetEvent)
					{
						manualResetEvent.Set();
					}
				}
			}
			return true;
		}

		public void Set()
		{
			this.Set(false);
		}

		private void Set(bool duringCancellation)
		{
			this.IsSet = true;
			if (this.Waiters > 0)
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					Monitor.PulseAll(this.m_lock);
				}
			}
			ManualResetEvent eventObj = this.m_eventObj;
			if (eventObj != null && !duringCancellation)
			{
				ManualResetEvent manualResetEvent = eventObj;
				lock (manualResetEvent)
				{
					if (this.m_eventObj != null)
					{
						this.m_eventObj.Set();
					}
				}
			}
		}

		public void Reset()
		{
			this.ThrowIfDisposed();
			if (this.m_eventObj != null)
			{
				this.m_eventObj.Reset();
			}
			this.IsSet = false;
		}

		public void Wait()
		{
			this.Wait(-1, default(CancellationToken));
		}

		public void Wait(CancellationToken cancellationToken)
		{
			this.Wait(-1, cancellationToken);
		}

		public bool Wait(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return this.Wait((int)num, default(CancellationToken));
		}

		public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return this.Wait((int)num, cancellationToken);
		}

		public bool Wait(int millisecondsTimeout)
		{
			return this.Wait(millisecondsTimeout, default(CancellationToken));
		}

		public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
		{
			this.ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			if (!this.IsSet)
			{
				if (millisecondsTimeout == 0)
				{
					return false;
				}
				uint num = 0U;
				bool flag = false;
				int num2 = millisecondsTimeout;
				if (millisecondsTimeout != -1)
				{
					num = TimeoutHelper.GetTime();
					flag = true;
				}
				int num3 = 10;
				int num4 = 5;
				int num5 = 20;
				int spinCount = this.SpinCount;
				for (int i = 0; i < spinCount; i++)
				{
					if (this.IsSet)
					{
						return true;
					}
					if (i < num3)
					{
						if (i == num3 / 2)
						{
							Thread.Yield();
						}
						else
						{
							Thread.SpinWait(PlatformHelper.ProcessorCount * (4 << i));
						}
					}
					else if (i % num5 == 0)
					{
						Thread.Sleep(1);
					}
					else if (i % num4 == 0)
					{
						Thread.Sleep(0);
					}
					else
					{
						Thread.Yield();
					}
					if (i >= 100 && i % 10 == 0)
					{
						cancellationToken.ThrowIfCancellationRequested();
					}
				}
				this.EnsureLockObjectCreated();
				using (cancellationToken.InternalRegisterWithoutEC(ManualResetEventSlim.s_cancellationTokenCallback, this))
				{
					object @lock = this.m_lock;
					lock (@lock)
					{
						while (!this.IsSet)
						{
							cancellationToken.ThrowIfCancellationRequested();
							if (flag)
							{
								num2 = TimeoutHelper.UpdateTimeOut(num, millisecondsTimeout);
								if (num2 <= 0)
								{
									return false;
								}
							}
							this.Waiters++;
							if (this.IsSet)
							{
								int waiters = this.Waiters;
								this.Waiters = waiters - 1;
								return true;
							}
							try
							{
								if (!Monitor.Wait(this.m_lock, num2))
								{
									return false;
								}
							}
							finally
							{
								this.Waiters--;
							}
						}
					}
				}
				return true;
			}
			return true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if ((this.m_combinedState & 1073741824) != 0)
			{
				return;
			}
			this.m_combinedState |= 1073741824;
			if (disposing)
			{
				ManualResetEvent eventObj = this.m_eventObj;
				if (eventObj != null)
				{
					ManualResetEvent manualResetEvent = eventObj;
					lock (manualResetEvent)
					{
						eventObj.Close();
						this.m_eventObj = null;
					}
				}
			}
		}

		private void ThrowIfDisposed()
		{
			if ((this.m_combinedState & 1073741824) != 0)
			{
				throw new ObjectDisposedException(Environment.GetResourceString("The event has been disposed."));
			}
		}

		private static void CancellationTokenCallback(object obj)
		{
			ManualResetEventSlim manualResetEventSlim = obj as ManualResetEventSlim;
			object @lock = manualResetEventSlim.m_lock;
			lock (@lock)
			{
				Monitor.PulseAll(manualResetEventSlim.m_lock);
			}
		}

		private void UpdateStateAtomically(int newBits, int updateBitsMask)
		{
			SpinWait spinWait = default(SpinWait);
			for (;;)
			{
				int combinedState = this.m_combinedState;
				int num = (combinedState & ~updateBitsMask) | newBits;
				if (Interlocked.CompareExchange(ref this.m_combinedState, num, combinedState) == combinedState)
				{
					break;
				}
				spinWait.SpinOnce();
			}
		}

		private static int ExtractStatePortionAndShiftRight(int state, int mask, int rightBitShiftCount)
		{
			return (int)((uint)(state & mask) >> rightBitShiftCount);
		}

		private static int ExtractStatePortion(int state, int mask)
		{
			return state & mask;
		}

		private const int DEFAULT_SPIN_SP = 1;

		private const int DEFAULT_SPIN_MP = 10;

		private volatile object m_lock;

		private volatile ManualResetEvent m_eventObj;

		private volatile int m_combinedState;

		private const int SignalledState_BitMask = -2147483648;

		private const int SignalledState_ShiftCount = 31;

		private const int Dispose_BitMask = 1073741824;

		private const int SpinCountState_BitMask = 1073217536;

		private const int SpinCountState_ShiftCount = 19;

		private const int SpinCountState_MaxValue = 2047;

		private const int NumWaitersState_BitMask = 524287;

		private const int NumWaitersState_ShiftCount = 0;

		private const int NumWaitersState_MaxValue = 524287;

		private static Action<object> s_cancellationTokenCallback = new Action<object>(ManualResetEventSlim.CancellationTokenCallback);
	}
}
