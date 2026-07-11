using System;

public interface ISharedWorkable
{
	void AddWorker(Worker worker);

	void RemoveWorker(Worker worker);
}
