using System;
using System.Runtime.ConstrainedExecution;

namespace System.Threading
{
	public class SynchronizationContext
	{
		public SynchronizationContext()
		{
		}

		internal SynchronizationContext(SynchronizationContext context)
		{
			SynchronizationContext.currentContext = context;
		}

		public static SynchronizationContext Current
		{
			get
			{
				return SynchronizationContext.currentContext;
			}
		}

		public virtual SynchronizationContext CreateCopy()
		{
			return new SynchronizationContext(this);
		}

		public bool IsWaitNotificationRequired()
		{
			return this.notification_required;
		}

		public virtual void OperationCompleted()
		{
		}

		public virtual void OperationStarted()
		{
		}

		public virtual void Post(SendOrPostCallback d, object state)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(d.Invoke), state);
		}

		public virtual void Send(SendOrPostCallback d, object state)
		{
			d(state);
		}

		public static void SetSynchronizationContext(SynchronizationContext syncContext)
		{
			SynchronizationContext.currentContext = syncContext;
		}

		[MonoTODO]
		protected void SetWaitNotificationRequired()
		{
			this.notification_required = true;
			throw new NotImplementedException();
		}

		[CLSCompliant(false)]
		[PrePrepareMethod]
		public virtual int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
		{
			return SynchronizationContext.WaitHelper(waitHandles, waitAll, millisecondsTimeout);
		}

		[CLSCompliant(false)]
		[PrePrepareMethod]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MonoTODO]
		protected static int WaitHelper(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
		{
			throw new NotImplementedException();
		}

		private bool notification_required;

		[ThreadStatic]
		private static SynchronizationContext currentContext;
	}
}
