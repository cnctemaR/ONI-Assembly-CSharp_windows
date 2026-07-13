using System;
using System.Collections.Generic;

public abstract class WorkItemCollectionWithThreadContex<SharedDataType, ThreadDataType> : IWorkItemCollection where ThreadDataType : class
{
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	public abstract void RunItem(int item, ref SharedDataType shared_data, ThreadDataType thread_context, int threadIndex);

	void IWorkItemCollection.InternalDoWorkItem(int work_item_idx, int threadIndex)
	{
		this.RunItem(work_item_idx, ref this.sharedData, this.threadContexts[threadIndex], threadIndex);
	}

	protected int count;

	public SharedDataType sharedData;

	public List<ThreadDataType> threadContexts;
}
