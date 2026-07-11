using System;
using System.Collections.Generic;

public static class ListPool<ObjectType, PoolIdentifier>
{
	public static ListPool<ObjectType, PoolIdentifier>.PooledList Allocate(List<ObjectType> objects)
	{
		ListPool<ObjectType, PoolIdentifier>.PooledList pooledList = ListPool<ObjectType, PoolIdentifier>.pool.Allocate();
		pooledList.AddRange(objects);
		return pooledList;
	}

	public static ListPool<ObjectType, PoolIdentifier>.PooledList Allocate()
	{
		return ListPool<ObjectType, PoolIdentifier>.pool.Allocate();
	}

	private static void Free(ListPool<ObjectType, PoolIdentifier>.PooledList list)
	{
		list.Clear();
		ListPool<ObjectType, PoolIdentifier>.pool.Free(list);
	}

	private static ContainerPool<ListPool<ObjectType, PoolIdentifier>.PooledList, PoolIdentifier> pool = new ContainerPool<ListPool<ObjectType, PoolIdentifier>.PooledList, PoolIdentifier>();

	public class PooledList : List<ObjectType>
	{
		public void Recycle()
		{
			ListPool<ObjectType, PoolIdentifier>.Free(this);
		}
	}
}
