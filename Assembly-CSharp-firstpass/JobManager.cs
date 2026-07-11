using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class JobManager
{
	public bool isShuttingDown { get; private set; }

	private void Initialize()
	{
		this.semaphore = new Semaphore(0, CPUBudget.coreCount);
		for (int i = 0; i < CPUBudget.coreCount; i++)
		{
			this.threads.Add(new JobManager.WorkerThread(this.semaphore, this, string.Format("KWorker{0}", i)));
		}
	}

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
		if (this.semaphore == null)
		{
			this.Initialize();
		}
		if (JobManager.runSingleThreaded || this.threads.Count == 0)
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
		public WorkerThread(Semaphore semaphore, JobManager job_manager, string name)
		{
			this.semaphore = semaphore;
			this.thread = new Thread(new ParameterizedThreadStart(JobManager.WorkerThread.ThreadMain), 131072);
			Util.ApplyInvariantCultureToThread(this.thread);
			this.thread.Priority = ThreadPriority.AboveNormal;
			this.thread.Name = name;
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
					bool flag = true;
					while (flag)
					{
						flag = this.jobManager.DoNextWorkItem();
					}
				}
				catch (Exception ex)
				{
					this.exceptions.Add(ex);
					JobManager.errorOccured = true;
					Debugger.Break();
				}
				this.jobManager.DecrementActiveWorkerThreadCount();
			}
		}

		public void PrintExceptions()
		{
			foreach (Exception ex in this.exceptions)
			{
				global::Debug.LogError(ex);
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
