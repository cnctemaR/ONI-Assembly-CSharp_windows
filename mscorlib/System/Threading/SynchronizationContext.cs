using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;

namespace System.Threading
{
	[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy)]
	public class SynchronizationContext
	{
		[SecuritySafeCritical]
		protected void SetWaitNotificationRequired()
		{
			Type type = base.GetType();
			if (SynchronizationContext.s_cachedPreparedType1 != type && SynchronizationContext.s_cachedPreparedType2 != type && SynchronizationContext.s_cachedPreparedType3 != type && SynchronizationContext.s_cachedPreparedType4 != type && SynchronizationContext.s_cachedPreparedType5 != type)
			{
				RuntimeHelpers.PrepareDelegate(new SynchronizationContext.WaitDelegate(this.Wait));
				if (SynchronizationContext.s_cachedPreparedType1 == null)
				{
					SynchronizationContext.s_cachedPreparedType1 = type;
				}
				else if (SynchronizationContext.s_cachedPreparedType2 == null)
				{
					SynchronizationContext.s_cachedPreparedType2 = type;
				}
				else if (SynchronizationContext.s_cachedPreparedType3 == null)
				{
					SynchronizationContext.s_cachedPreparedType3 = type;
				}
				else if (SynchronizationContext.s_cachedPreparedType4 == null)
				{
					SynchronizationContext.s_cachedPreparedType4 = type;
				}
				else if (SynchronizationContext.s_cachedPreparedType5 == null)
				{
					SynchronizationContext.s_cachedPreparedType5 = type;
				}
			}
			this._props |= SynchronizationContextProperties.RequireWaitNotification;
		}

		public bool IsWaitNotificationRequired()
		{
			return (this._props & SynchronizationContextProperties.RequireWaitNotification) > SynchronizationContextProperties.None;
		}

		public virtual void Send(SendOrPostCallback d, object state)
		{
			d(state);
		}

		public virtual void Post(SendOrPostCallback d, object state)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(d.Invoke), state);
		}

		public virtual void OperationStarted()
		{
		}

		public virtual void OperationCompleted()
		{
		}

		[PrePrepareMethod]
		[SecurityCritical]
		[CLSCompliant(false)]
		public virtual int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
		{
			if (waitHandles == null)
			{
				throw new ArgumentNullException("waitHandles");
			}
			return SynchronizationContext.WaitHelper(waitHandles, waitAll, millisecondsTimeout);
		}

		[CLSCompliant(false)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[PrePrepareMethod]
		[SecurityCritical]
		protected static int WaitHelper(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
		{
			throw new NotImplementedException();
		}

		[SecurityCritical]
		public static void SetSynchronizationContext(SynchronizationContext syncContext)
		{
			ExecutionContext mutableExecutionContext = Thread.CurrentThread.GetMutableExecutionContext();
			mutableExecutionContext.SynchronizationContext = syncContext;
			mutableExecutionContext.SynchronizationContextNoFlow = syncContext;
		}

		public static SynchronizationContext Current
		{
			get
			{
				return Thread.CurrentThread.GetExecutionContextReader().SynchronizationContext ?? SynchronizationContext.GetThreadLocalContext();
			}
		}

		internal static SynchronizationContext CurrentNoFlow
		{
			[FriendAccessAllowed]
			get
			{
				return Thread.CurrentThread.GetExecutionContextReader().SynchronizationContextNoFlow ?? SynchronizationContext.GetThreadLocalContext();
			}
		}

		private static SynchronizationContext GetThreadLocalContext()
		{
			return null;
		}

		public virtual SynchronizationContext CreateCopy()
		{
			return new SynchronizationContext();
		}

		[SecurityCritical]
		private static int InvokeWaitMethodHelper(SynchronizationContext syncContext, IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
		{
			return syncContext.Wait(waitHandles, waitAll, millisecondsTimeout);
		}

		private SynchronizationContextProperties _props;

		private static Type s_cachedPreparedType1;

		private static Type s_cachedPreparedType2;

		private static Type s_cachedPreparedType3;

		private static Type s_cachedPreparedType4;

		private static Type s_cachedPreparedType5;

		private delegate int WaitDelegate(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout);
	}
}
