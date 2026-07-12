using System;

public interface IWorkItem<SharedDataType>
{
	void Run(SharedDataType shared_data, int threadIndex);
}
