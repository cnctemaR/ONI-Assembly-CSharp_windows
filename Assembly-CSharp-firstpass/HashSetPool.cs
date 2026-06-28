using System;
using System.Collections.Generic;

public static class HashSetPool<ObjectType, PoolIdentifier>
{
	public static HashSet<ObjectType> Allocate()
	{
		return HashSetPool<ObjectType, PoolIdentifier>.pool.Allocate();
	}

	public static void Free(HashSet<ObjectType> hash_set)
	{
		hash_set.Clear();
		HashSetPool<ObjectType, PoolIdentifier>.pool.Free(hash_set);
	}

	private static ContainerPool<HashSet<ObjectType>, PoolIdentifier> pool = new ContainerPool<HashSet<ObjectType>, PoolIdentifier>();
}
