using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class JobManager
{
	public JobManager()
	{
		int num = Math.Max(SystemInfo.processorCount, 1);
		this.semaphore = new Semaphore(0, num);
		for (int i = 0; i < num; i++)
		{
			this.threads.Add(new JobManager.WorkerThread(this.semaphore, this));
		}
	}

	public bool isShuttingDown { get; private set; }

	public bool DoNextWorkItem()
	{
		int num = Interlocked.Increment(ref this.nextWorkIndex);
		if (num < this.workItems.Count)
		{
			this.workItems.InternalDoWorkItem(num);
			return true;
		}
		return false;
	}

	public void Cleanup()
	{
		this.isShuttingDown = true;
		this.semaphore.Release(this.threads.Count);
		foreach (JobManager.WorkerThread workerThread in this.threads)
		{
			workerThread.Cleanup();
		}
		this.threads.Clear();
	}

	public void Run(IWorkItemCollection work_items)
	{
		if (JobManager.runSingleThreaded)
		{
			for (int i = 0; i < work_items.Count; i++)
			{
				work_items.InternalDoWorkItem(i);
			}
		}
		else
		{
			this.workerThreadCount = this.threads.Count;
			this.nextWorkIndex = -1;
			this.workItems = work_items;
			Thread.MemoryBarrier();
			this.semaphore.Release(this.threads.Count);
			this.manualResetEvent.WaitOne();
			this.manualResetEvent.Reset();
			if (JobManager.errorOccured)
			{
				foreach (JobManager.WorkerThread workerThread in this.threads)
				{
					workerThread.PrintExceptions();
				}
			}
		}
	}

	public void DecrementActiveWorkerThreadCount()
	{
		if (Interlocked.Decrement(ref this.workerThreadCount) == 0)
		{
			this.manualResetEvent.Set();
		}
	}

	public static bool errorOccured;

	private List<JobManager.WorkerThread> threads = new List<JobManager.WorkerThread>();

	private Semaphore semaphore;

	private IWorkItemCollection workItems;

	private int nextWorkIndex = -1;

	private int workerThreadCount;

	private ManualResetEvent manualResetEvent = new ManualResetEvent(false);

	private static bool runSingleThreaded;

	private class WorkerThread
	{
		public WorkerThread(Semaphore semaphore, JobManager job_manager)
		{
			this.semaphore = semaphore;
			this.thread = new Thread(new ParameterizedThreadStart(JobManager.WorkerThread.ThreadMain), 131072);
			this.thread.Priority = global::System.Threading.ThreadPriority.AboveNormal;
			this.thread.Name = "JobManagerWorkerThread";
			this.jobManager = job_manager;
			this.exceptions = new List<Exception>();
			this.thread.Start(this);
		}

		public void Run()
		{
			for (;;)
			{
				this.semaphore.WaitOne();
				if (this.jobManager.isShuttingDown)
				{
					break;
				}
				try
				{
					while (this.jobManager.DoNextWorkItem())
					{
					}
				}
				catch (Exception ex)
				{
					this.exceptions.Add(ex);
					JobManager.errorOccured = true;
				}
				this.jobManager.DecrementActiveWorkerThreadCount();
			}
		}

		public void PrintExceptions()
		{
			foreach (Exception ex in this.exceptions)
			{
				global::Debug.LogError(ex, null);
			}
		}

		public void Cleanup()
		{
		}

		public static void ThreadMain(object data)
		{
			JobManager.WorkerThread workerThread = (JobManager.WorkerThread)data;
			workerThread.Run();
		}

		private Thread thread;

		private Semaphore semaphore;

		private JobManager jobManager;

		private List<Exception> exceptions;
	}
}
