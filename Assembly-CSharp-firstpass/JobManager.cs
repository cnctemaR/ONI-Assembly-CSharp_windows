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
		return this.currentJob.DoNextWorkItem();
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

	public void Run<WorkItemType, SharedDataType>(List<WorkItemType> work_items, SharedDataType shared_data) where WorkItemType : IWorkItem<SharedDataType>
	{
		if (JobManager.runSingleThreaded)
		{
			foreach (WorkItemType workItemType in work_items)
			{
				workItemType.Run(shared_data);
			}
		}
		else
		{
			this.currentJob = new Job<WorkItemType, SharedDataType>(work_items, shared_data, this.threads.Count);
			this.semaphore.Release(this.threads.Count);
			this.currentJob.Wait();
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
		this.currentJob.DecrementWorkerThreadCount();
	}

	public static bool errorOccured;

	private List<JobManager.WorkerThread> threads = new List<JobManager.WorkerThread>();

	private Semaphore semaphore;

	private Job currentJob;

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
