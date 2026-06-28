using System;
using System.Collections.Generic;

public static class ListPool<ObjectType, PoolIdentifier>
{
	public static List<ObjectType> Allocate()
	{
		return ListPool<ObjectType, PoolIdentifier>.pool.Allocate();
	}

	public static void Free(List<ObjectType> list)
	{
		list.Clear();
		ListPool<ObjectType, PoolIdentifier>.pool.Free(list);
	}

	private static ContainerPool<List<ObjectType>, PoolIdentifier> pool = new ContainerPool<List<ObjectType>, PoolIdentifier>();
}
