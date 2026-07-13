using System;

public interface IWorkItemWithThreadContext<SharedDataType, ThreadDataType>
{
	void Run(ref SharedDataType shared_data, ThreadDataType thread_data, int threadIndex);
}
