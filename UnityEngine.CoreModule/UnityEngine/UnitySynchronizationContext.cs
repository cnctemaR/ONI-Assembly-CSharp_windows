using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class UnitySynchronizationContext : SynchronizationContext
	{
		private UnitySynchronizationContext(int mainThreadID)
		{
			this.m_AsyncWorkQueue = new List<UnitySynchronizationContext.WorkRequest>(20);
			this.m_MainThreadID = mainThreadID;
		}

		private UnitySynchronizationContext(List<UnitySynchronizationContext.WorkRequest> queue, int mainThreadID)
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
						this.m_AsyncWorkQueue.Add(new UnitySynchronizationContext.WorkRequest(callback, state, manualResetEvent));
					}
					manualResetEvent.WaitOne();
				}
			}
		}

		public override void OperationStarted()
		{
			Interlocked.Increment(ref this.m_TrackedCount);
		}

		public override void OperationCompleted()
		{
			Interlocked.Decrement(ref this.m_TrackedCount);
		}

		public override void Post(SendOrPostCallback callback, object state)
		{
			object asyncWorkQueue = this.m_AsyncWorkQueue;
			lock (asyncWorkQueue)
			{
				this.m_AsyncWorkQueue.Add(new UnitySynchronizationContext.WorkRequest(callback, state, null));
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
				this.m_CurrentFrameWork.AddRange(this.m_AsyncWorkQueue);
				this.m_AsyncWorkQueue.Clear();
			}
			foreach (UnitySynchronizationContext.WorkRequest workRequest in this.m_CurrentFrameWork)
			{
				workRequest.Invoke();
			}
			this.m_CurrentFrameWork.Clear();
		}

		private bool HasPendingTasks()
		{
			return this.m_AsyncWorkQueue.Count != 0 || this.m_TrackedCount != 0;
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

		[RequiredByNativeCode]
		private static bool ExecutePendingTasks(long millisecondsTimeout)
		{
			UnitySynchronizationContext unitySynchronizationContext = SynchronizationContext.Current as UnitySynchronizationContext;
			bool flag;
			if (unitySynchronizationContext == null)
			{
				flag = true;
			}
			else
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				while (unitySynchronizationContext.HasPendingTasks())
				{
					if (stopwatch.ElapsedMilliseconds > millisecondsTimeout)
					{
						break;
					}
					unitySynchronizationContext.Exec();
					Thread.Sleep(1);
				}
				flag = !unitySynchronizationContext.HasPendingTasks();
			}
			return flag;
		}

		private const int kAwqInitialCapacity = 20;

		private readonly List<UnitySynchronizationContext.WorkRequest> m_AsyncWorkQueue;

		private readonly List<UnitySynchronizationContext.WorkRequest> m_CurrentFrameWork = new List<UnitySynchronizationContext.WorkRequest>(20);

		private readonly int m_MainThreadID;

		private int m_TrackedCount = 0;

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
				try
				{
					this.m_DelagateCallback(this.m_DelagateState);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
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
