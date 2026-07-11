using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class UnitySynchronizationContext : SynchronizationContext
	{
		private UnitySynchronizationContext(int mainThreadID)
		{
			this.m_AsyncWorkQueue = new Queue<UnitySynchronizationContext.WorkRequest>(20);
			this.m_MainThreadID = mainThreadID;
		}

		private UnitySynchronizationContext(Queue<UnitySynchronizationContext.WorkRequest> queue, int mainThreadID)
		{
			this.m_AsyncWorkQueue = queue;
			this.m_MainThreadID = mainThreadID;
		}

		public override void Send(SendOrPostCallback callback, object state)
		{
			if (this.m_MainThreadID == Thread.CurrentThread.ManagedThreadId)
			{
				callback(state);
			}
			else
			{
				using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
				{
					object asyncWorkQueue = this.m_AsyncWorkQueue;
					lock (asyncWorkQueue)
					{
						this.m_AsyncWorkQueue.Enqueue(new UnitySynchronizationContext.WorkRequest(callback, state, manualResetEvent));
					}
					manualResetEvent.WaitOne();
				}
			}
		}

		public override void Post(SendOrPostCallback callback, object state)
		{
			object asyncWorkQueue = this.m_AsyncWorkQueue;
			lock (asyncWorkQueue)
			{
				this.m_AsyncWorkQueue.Enqueue(new UnitySynchronizationContext.WorkRequest(callback, state, null));
			}
		}

		public override SynchronizationContext CreateCopy()
		{
			return new UnitySynchronizationContext(this.m_AsyncWorkQueue, this.m_MainThreadID);
		}

		private void Exec()
		{
			object asyncWorkQueue = this.m_AsyncWorkQueue;
			lock (asyncWorkQueue)
			{
				int count = this.m_AsyncWorkQueue.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_AsyncWorkQueue.Dequeue().Invoke();
				}
			}
		}

		[RequiredByNativeCode]
		private static void InitializeSynchronizationContext()
		{
			if (SynchronizationContext.Current == null)
			{
				SynchronizationContext.SetSynchronizationContext(new UnitySynchronizationContext(Thread.CurrentThread.ManagedThreadId));
			}
		}

		[RequiredByNativeCode]
		private static void ExecuteTasks()
		{
			UnitySynchronizationContext unitySynchronizationContext = SynchronizationContext.Current as UnitySynchronizationContext;
			if (unitySynchronizationContext != null)
			{
				unitySynchronizationContext.Exec();
			}
		}

		private const int kAwqInitialCapacity = 20;

		private readonly Queue<UnitySynchronizationContext.WorkRequest> m_AsyncWorkQueue;

		private readonly int m_MainThreadID;

		private struct WorkRequest
		{
			public WorkRequest(SendOrPostCallback callback, object state, ManualResetEvent waitHandle = null)
			{
				this.m_DelagateCallback = callback;
				this.m_DelagateState = state;
				this.m_WaitHandle = waitHandle;
			}

			public void Invoke()
			{
				this.m_DelagateCallback(this.m_DelagateState);
				if (this.m_WaitHandle != null)
				{
					this.m_WaitHandle.Set();
				}
			}

			private readonly SendOrPostCallback m_DelagateCallback;

			private readonly object m_DelagateState;

			private readonly ManualResetEvent m_WaitHandle;
		}
	}
}
