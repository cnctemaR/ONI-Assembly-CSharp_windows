using System;
using System.Collections.Generic;

public class WorkItemCollectionWithThreadContext<WorkItemType, SharedDataType, ThreadDataType> : IWorkItemCollection where WorkItemType : IWorkItemWithThreadContext<SharedDataType, ThreadDataType> where ThreadDataType : class
{
	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	void IWorkItemCollection.InternalDoWorkItem(int work_item_idx, int threadIndex)
	{
		WorkItemType workItemType = this.items[work_item_idx];
		workItemType.Run(ref this.sharedData, this.threadContexts[threadIndex], threadIndex);
		this.items[work_item_idx] = workItemType;
	}

	public List<WorkItemType> items = new List<WorkItemType>();

	public SharedDataType sharedData;

	public List<ThreadDataType> threadContexts = new List<ThreadDataType>();
}
