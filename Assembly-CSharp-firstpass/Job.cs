using System;
using System.Threading;

public abstract class Job
{
	public Job(int worker_thread_count)
	{
		this.workerThreadCount = worker_thread_count;
	}

	public abstract bool DoNextWorkItem();

	public void Wait()
	{
		this.manualResetEvent.WaitOne();
	}

	public void DecrementWorkerThreadCount()
	{
		if (Interlocked.Decrement(ref this.workerThreadCount) == 0)
		{
			this.manualResetEvent.Set();
		}
	}

	protected ManualResetEvent manualResetEvent = new ManualResetEvent(false);

	private int workerThreadCount;
}
