using System;

public static class GlobalJobManager
{
	public static void Run(IWorkItemCollection work_items)
	{
		GlobalJobManager.jobManager.Run(work_items);
	}

	public static void Cleanup()
	{
		if (GlobalJobManager.jobManager != null)
		{
			GlobalJobManager.jobManager.Cleanup();
		}
		GlobalJobManager.jobManager = null;
	}

	private static JobManager jobManager = new JobManager();
}
