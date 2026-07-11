using System;
using System.Collections.Generic;

public class ContainerPool<ContainerType, PoolIdentifier> : ContainerPool where ContainerType : new()
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

	public override string GetName()
	{
		return typeof(PoolIdentifier).Name + "." + typeof(ContainerType).Name;
	}

	private Stack<ContainerType> freeContainers = new Stack<ContainerType>();
}
