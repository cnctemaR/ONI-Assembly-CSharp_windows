using System;
using System.Collections.Generic;
using System.Threading;

public class JobBatch
{
	private static void JobCallback(object data)
	{
		JobBatch.JobData jobData = (JobBatch.JobData)data;
		try
		{
			jobData.callback();
		}
		catch (Exception ex)
		{
			Output.LogError(new object[] { ex });
		}
		jobData.evt.Set();
	}

	public void Add(global::System.Action job)
	{
		ManualResetEvent manualResetEvent = new ManualResetEvent(false);
		this.doneEvents.Add(manualResetEvent);
		ThreadPool.QueueUserWorkItem(new WaitCallback(JobBatch.JobCallback), new JobBatch.JobData
		{
			callback = job,
			evt = manualResetEvent
		});
	}

	public void Run()
	{
		WaitHandle.WaitAll(this.doneEvents.ToArray());
	}

	private List<ManualResetEvent> doneEvents = new List<ManualResetEvent>();

	private struct JobData
	{
		public global::System.Action callback;

		public ManualResetEvent evt;
	}
}
