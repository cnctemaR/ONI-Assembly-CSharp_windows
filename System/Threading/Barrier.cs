using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.Threading
{
	[DebuggerDisplay("Participant Count={ParticipantCount},Participants Remaining={ParticipantsRemaining}")]
	[ComVisible(false)]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public class Barrier : IDisposable
	{
		public int ParticipantsRemaining
		{
			get
			{
				int currentTotalCount = this.m_currentTotalCount;
				int num = currentTotalCount & 32767;
				int num2 = (currentTotalCount & 2147418112) >> 16;
				return num - num2;
			}
		}

		public int ParticipantCount
		{
			get
			{
				return this.m_currentTotalCount & 32767;
			}
		}

		public long CurrentPhaseNumber
		{
			get
			{
				return Volatile.Read(ref this.m_currentPhase);
			}
			internal set
			{
				Volatile.Write(ref this.m_currentPhase, value);
			}
		}

		public Barrier(int participantCount)
			: this(participantCount, null)
		{
		}

		public Barrier(int participantCount, Action<Barrier> postPhaseAction)
		{
			if (participantCount < 0 || participantCount > 32767)
			{
				throw new ArgumentOutOfRangeException("participantCount", participantCount, SR.GetString("The participantCount argument must be non-negative and less than or equal to 32767."));
			}
			this.m_currentTotalCount = participantCount;
			this.m_postPhaseAction = postPhaseAction;
			this.m_oddEvent = new ManualResetEventSlim(true);
			this.m_evenEvent = new ManualResetEventSlim(false);
			if (postPhaseAction != null && !ExecutionContext.IsFlowSuppressed())
			{
				this.m_ownerThreadContext = ExecutionContext.Capture();
			}
			this.m_actionCallerID = 0;
		}

		private void GetCurrentTotal(int currentTotal, out int current, out int total, out bool sense)
		{
			total = currentTotal & 32767;
			current = (currentTotal & 2147418112) >> 16;
			sense = (currentTotal & int.MinValue) == 0;
		}

		private bool SetCurrentTotal(int currentTotal, int current, int total, bool sense)
		{
			int num = (current << 16) | total;
			if (!sense)
			{
				num |= int.MinValue;
			}
			return Interlocked.CompareExchange(ref this.m_currentTotalCount, num, currentTotal) == currentTotal;
		}

		public long AddParticipant()
		{
			long num;
			try
			{
				num = this.AddParticipants(1);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new InvalidOperationException(SR.GetString("Adding participantCount participants would result in the number of participants exceeding the maximum number allowed."));
			}
			return num;
		}

		public long AddParticipants(int participantCount)
		{
			this.ThrowIfDisposed();
			if (participantCount < 1)
			{
				throw new ArgumentOutOfRangeException("participantCount", participantCount, SR.GetString("The participantCount argument must be a positive value."));
			}
			if (participantCount > 32767)
			{
				throw new ArgumentOutOfRangeException("participantCount", SR.GetString("Adding participantCount participants would result in the number of participants exceeding the maximum number allowed."));
			}
			if (this.m_actionCallerID != 0 && Thread.CurrentThread.ManagedThreadId == this.m_actionCallerID)
			{
				throw new InvalidOperationException(SR.GetString("This method may not be called from within the postPhaseAction."));
			}
			SpinWait spinWait = default(SpinWait);
			bool flag;
			for (;;)
			{
				int currentTotalCount = this.m_currentTotalCount;
				int num;
				int num2;
				this.GetCurrentTotal(currentTotalCount, out num, out num2, out flag);
				if (participantCount + num2 > 32767)
				{
					break;
				}
				if (this.SetCurrentTotal(currentTotalCount, num, num2 + participantCount, flag))
				{
					goto Block_6;
				}
				spinWait.SpinOnce();
			}
			throw new ArgumentOutOfRangeException("participantCount", SR.GetString("Adding participantCount participants would result in the number of participants exceeding the maximum number allowed."));
			Block_6:
			long currentPhaseNumber = this.CurrentPhaseNumber;
			long num3 = ((flag != (currentPhaseNumber % 2L == 0L)) ? (currentPhaseNumber + 1L) : currentPhaseNumber);
			if (num3 != currentPhaseNumber)
			{
				if (flag)
				{
					this.m_oddEvent.Wait();
				}
				else
				{
					this.m_evenEvent.Wait();
				}
			}
			else if (flag && this.m_evenEvent.IsSet)
			{
				this.m_evenEvent.Reset();
			}
			else if (!flag && this.m_oddEvent.IsSet)
			{
				this.m_oddEvent.Reset();
			}
			return num3;
		}

		public void RemoveParticipant()
		{
			this.RemoveParticipants(1);
		}

		public void RemoveParticipants(int participantCount)
		{
			this.ThrowIfDisposed();
			if (participantCount < 1)
			{
				throw new ArgumentOutOfRangeException("participantCount", participantCount, SR.GetString("The participantCount argument must be a positive value."));
			}
			if (this.m_actionCallerID != 0 && Thread.CurrentThread.ManagedThreadId == this.m_actionCallerID)
			{
				throw new InvalidOperationException(SR.GetString("This method may not be called from within the postPhaseAction."));
			}
			SpinWait spinWait = default(SpinWait);
			bool flag;
			for (;;)
			{
				int currentTotalCount = this.m_currentTotalCount;
				int num;
				int num2;
				this.GetCurrentTotal(currentTotalCount, out num, out num2, out flag);
				if (num2 < participantCount)
				{
					break;
				}
				if (num2 - participantCount < num)
				{
					goto Block_5;
				}
				int num3 = num2 - participantCount;
				if (num3 > 0 && num == num3)
				{
					if (this.SetCurrentTotal(currentTotalCount, 0, num2 - participantCount, !flag))
					{
						goto Block_8;
					}
				}
				else if (this.SetCurrentTotal(currentTotalCount, num, num2 - participantCount, flag))
				{
					return;
				}
				spinWait.SpinOnce();
			}
			throw new ArgumentOutOfRangeException("participantCount", SR.GetString("The participantCount argument must be less than or equal the number of participants."));
			Block_5:
			throw new InvalidOperationException(SR.GetString("The participantCount argument is greater than the number of participants that haven't yet arrived at the barrier in this phase."));
			Block_8:
			this.FinishPhase(flag);
		}

		public void SignalAndWait()
		{
			this.SignalAndWait(default(CancellationToken));
		}

		public void SignalAndWait(CancellationToken cancellationToken)
		{
			this.SignalAndWait(-1, cancellationToken);
		}

		public bool SignalAndWait(TimeSpan timeout)
		{
			return this.SignalAndWait(timeout, default(CancellationToken));
		}

		public bool SignalAndWait(TimeSpan timeout, CancellationToken cancellationToken)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout", timeout, SR.GetString("The specified timeout must represent a value between -1 and Int32.MaxValue, inclusive."));
			}
			return this.SignalAndWait((int)timeout.TotalMilliseconds, cancellationToken);
		}

		public bool SignalAndWait(int millisecondsTimeout)
		{
			return this.SignalAndWait(millisecondsTimeout, default(CancellationToken));
		}

		public bool SignalAndWait(int millisecondsTimeout, CancellationToken cancellationToken)
		{
			this.ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", millisecondsTimeout, SR.GetString("The specified timeout must represent a value between -1 and Int32.MaxValue, inclusive."));
			}
			if (this.m_actionCallerID != 0 && Thread.CurrentThread.ManagedThreadId == this.m_actionCallerID)
			{
				throw new InvalidOperationException(SR.GetString("This method may not be called from within the postPhaseAction."));
			}
			SpinWait spinWait = default(SpinWait);
			bool flag;
			long currentPhaseNumber;
			for (;;)
			{
				int num = this.m_currentTotalCount;
				int num2;
				int num3;
				this.GetCurrentTotal(num, out num2, out num3, out flag);
				currentPhaseNumber = this.CurrentPhaseNumber;
				if (num3 == 0)
				{
					break;
				}
				if (num2 == 0 && flag != (this.CurrentPhaseNumber % 2L == 0L))
				{
					goto Block_6;
				}
				if (num2 + 1 == num3)
				{
					if (this.SetCurrentTotal(num, 0, num3, !flag))
					{
						goto Block_8;
					}
				}
				else if (this.SetCurrentTotal(num, num2 + 1, num3, flag))
				{
					goto IL_00EA;
				}
				spinWait.SpinOnce();
			}
			throw new InvalidOperationException(SR.GetString("The barrier has no registered participants."));
			Block_6:
			throw new InvalidOperationException(SR.GetString("The number of threads using the barrier exceeded the total number of registered participants."));
			Block_8:
			this.FinishPhase(flag);
			return true;
			IL_00EA:
			ManualResetEventSlim manualResetEventSlim = (flag ? this.m_evenEvent : this.m_oddEvent);
			bool flag2 = false;
			bool flag3 = false;
			try
			{
				flag3 = this.DiscontinuousWait(manualResetEventSlim, millisecondsTimeout, cancellationToken, currentPhaseNumber);
			}
			catch (OperationCanceledException)
			{
				flag2 = true;
			}
			catch (ObjectDisposedException)
			{
				if (currentPhaseNumber >= this.CurrentPhaseNumber)
				{
					throw;
				}
				flag3 = true;
			}
			if (!flag3)
			{
				spinWait.Reset();
				for (;;)
				{
					int num = this.m_currentTotalCount;
					int num2;
					int num3;
					bool flag4;
					this.GetCurrentTotal(num, out num2, out num3, out flag4);
					if (currentPhaseNumber < this.CurrentPhaseNumber || flag != flag4)
					{
						break;
					}
					if (this.SetCurrentTotal(num, num2 - 1, num3, flag))
					{
						goto Block_13;
					}
					spinWait.SpinOnce();
				}
				this.WaitCurrentPhase(manualResetEventSlim, currentPhaseNumber);
				goto IL_0197;
				Block_13:
				if (flag2)
				{
					throw new OperationCanceledException(SR.GetString("The operation was canceled."), cancellationToken);
				}
				return false;
			}
			IL_0197:
			if (this.m_exception != null)
			{
				throw new BarrierPostPhaseException(this.m_exception);
			}
			return true;
		}

		[SecuritySafeCritical]
		private void FinishPhase(bool observedSense)
		{
			if (this.m_postPhaseAction != null)
			{
				try
				{
					this.m_actionCallerID = Thread.CurrentThread.ManagedThreadId;
					if (this.m_ownerThreadContext != null)
					{
						ExecutionContext ownerThreadContext = this.m_ownerThreadContext;
						this.m_ownerThreadContext = this.m_ownerThreadContext.CreateCopy();
						ContextCallback contextCallback = Barrier.s_invokePostPhaseAction;
						if (contextCallback == null)
						{
							contextCallback = (Barrier.s_invokePostPhaseAction = new ContextCallback(Barrier.InvokePostPhaseAction));
						}
						ExecutionContext.Run(ownerThreadContext, contextCallback, this);
						ownerThreadContext.Dispose();
					}
					else
					{
						this.m_postPhaseAction(this);
					}
					this.m_exception = null;
					return;
				}
				catch (Exception ex)
				{
					this.m_exception = ex;
					return;
				}
				finally
				{
					this.m_actionCallerID = 0;
					this.SetResetEvents(observedSense);
					if (this.m_exception != null)
					{
						throw new BarrierPostPhaseException(this.m_exception);
					}
				}
			}
			this.SetResetEvents(observedSense);
		}

		[SecurityCritical]
		private static void InvokePostPhaseAction(object obj)
		{
			Barrier barrier = (Barrier)obj;
			barrier.m_postPhaseAction(barrier);
		}

		private void SetResetEvents(bool observedSense)
		{
			this.CurrentPhaseNumber += 1L;
			if (observedSense)
			{
				this.m_oddEvent.Reset();
				this.m_evenEvent.Set();
				return;
			}
			this.m_evenEvent.Reset();
			this.m_oddEvent.Set();
		}

		private void WaitCurrentPhase(ManualResetEventSlim currentPhaseEvent, long observedPhase)
		{
			SpinWait spinWait = default(SpinWait);
			while (!currentPhaseEvent.IsSet && this.CurrentPhaseNumber - observedPhase <= 1L)
			{
				spinWait.SpinOnce();
			}
		}

		private bool DiscontinuousWait(ManualResetEventSlim currentPhaseEvent, int totalTimeout, CancellationToken token, long observedPhase)
		{
			int num = 100;
			int num2 = 10000;
			while (observedPhase == this.CurrentPhaseNumber)
			{
				int num3 = ((totalTimeout == -1) ? num : Math.Min(num, totalTimeout));
				if (currentPhaseEvent.Wait(num3, token))
				{
					return true;
				}
				if (totalTimeout != -1)
				{
					totalTimeout -= num3;
					if (totalTimeout <= 0)
					{
						return false;
					}
				}
				num = ((num >= num2) ? num2 : Math.Min(num << 1, num2));
			}
			this.WaitCurrentPhase(currentPhaseEvent, observedPhase);
			return true;
		}

		public void Dispose()
		{
			if (this.m_actionCallerID != 0 && Thread.CurrentThread.ManagedThreadId == this.m_actionCallerID)
			{
				throw new InvalidOperationException(SR.GetString("This method may not be called from within the postPhaseAction."));
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				if (disposing)
				{
					this.m_oddEvent.Dispose();
					this.m_evenEvent.Dispose();
					if (this.m_ownerThreadContext != null)
					{
						this.m_ownerThreadContext.Dispose();
						this.m_ownerThreadContext = null;
					}
				}
				this.m_disposed = true;
			}
		}

		private void ThrowIfDisposed()
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException("Barrier", SR.GetString("The barrier has been disposed."));
			}
		}

		private volatile int m_currentTotalCount;

		private const int CURRENT_MASK = 2147418112;

		private const int TOTAL_MASK = 32767;

		private const int SENSE_MASK = -2147483648;

		private const int MAX_PARTICIPANTS = 32767;

		private long m_currentPhase;

		private bool m_disposed;

		private ManualResetEventSlim m_oddEvent;

		private ManualResetEventSlim m_evenEvent;

		private ExecutionContext m_ownerThreadContext;

		[SecurityCritical]
		private static ContextCallback s_invokePostPhaseAction;

		private Action<Barrier> m_postPhaseAction;

		private Exception m_exception;

		private int m_actionCallerID;
	}
}
