using System;

public interface IWorkerPrioritizable
{
	bool GetWorkerPriority(WorkerBase worker, out int priority);
}
