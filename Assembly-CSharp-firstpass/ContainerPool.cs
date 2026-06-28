using System;
using System.Collections.Generic;

internal class ContainerPool<ContainerType, PoolIdentifier> where ContainerType : new()
{
	public ContainerType Allocate()
	{
		if (this.freeContainers.Count == 0)
		{
			return new ContainerType();
		}
		return this.freeContainers.Pop();
	}

	public void Free(ContainerType container)
	{
		this.freeContainers.Push(container);
	}

	private Stack<ContainerType> freeContainers = new Stack<ContainerType>();
}
