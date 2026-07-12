using System;
using System.Collections.Generic;

public class ContainerPool<ContainerType, PoolIdentifier> : ContainerPool where ContainerType : new()
{
	public ContainerType Allocate()
	{
		Stack<ContainerType> stack = this.freeContainers;
		ContainerType containerType;
		lock (stack)
		{
			if (this.freeContainers.Count == 0)
			{
				containerType = new ContainerType();
			}
			else
			{
				containerType = this.freeContainers.Pop();
			}
		}
		return containerType;
	}

	public void Free(ContainerType container)
	{
		Stack<ContainerType> stack = this.freeContainers;
		lock (stack)
		{
			this.freeContainers.Push(container);
		}
	}

	public override string GetName()
	{
		return typeof(PoolIdentifier).Name + "." + typeof(ContainerType).Name;
	}

	private Stack<ContainerType> freeContainers = new Stack<ContainerType>();
}
