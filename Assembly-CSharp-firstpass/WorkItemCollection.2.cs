using System;

public abstract class WorkItemCollection<SharedDataType> : IWorkItemCollection
{
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	public abstract void RunItem(int item, ref SharedDataType shared_data, int threadIndex);

	void IWorkItemCollection.InternalDoWorkItem(int work_item_idx, int threadIndex)
	{
		this.RunItem(work_item_idx, ref this.sharedData, threadIndex);
	}

	protected int count;

	public SharedDataType sharedData;
}
