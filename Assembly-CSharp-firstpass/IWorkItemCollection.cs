using System;

public interface IWorkItemCollection
{
	int Count { get; }

	void InternalDoWorkItem(int work_item_idx, int threadIndex);
}
