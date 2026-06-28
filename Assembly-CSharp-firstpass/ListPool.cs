using System;
using System.Collections.Generic;

public static class ListPool<ObjectType, PoolIdentifier>
{
	public static List<ObjectType> Allocate()
	{
		if (ListPool<ObjectType, PoolIdentifier>.freeLists.Count == 0)
		{
			return new List<ObjectType>();
		}
		return ListPool<ObjectType, PoolIdentifier>.freeLists.Pop();
	}

	public static void Free(List<ObjectType> list)
	{
		list.Clear();
		ListPool<ObjectType, PoolIdentifier>.freeLists.Push(list);
	}

	private static Stack<List<ObjectType>> freeLists = new Stack<List<ObjectType>>();
}
