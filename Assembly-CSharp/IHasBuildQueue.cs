using System;
using System.Collections.Generic;

public interface IHasBuildQueue
{
	int NumOrders { get; }

	void CancelOrder(int idx);

	List<IBuildQueueOrder> Orders { get; }

	bool HasWorker { get; }

	bool NeedsWorker { get; }
}
