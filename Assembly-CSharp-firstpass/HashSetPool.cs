using System;
using System.Collections.Generic;

public static class HashSetPool<ObjectType, PoolIdentifier>
{
	public static HashSetPool<ObjectType, PoolIdentifier>.PooledHashSet Allocate()
	{
		return HashSetPool<ObjectType, PoolIdentifier>.pool.Allocate();
	}

	private static void Free(HashSetPool<ObjectType, PoolIdentifier>.PooledHashSet hash_set)
	{
		hash_set.Clear();
		HashSetPool<ObjectType, PoolIdentifier>.pool.Free(hash_set);
	}

	public static ContainerPool GetPool()
	{
		return HashSetPool<ObjectType, PoolIdentifier>.pool;
	}

	private static ContainerPool<HashSetPool<ObjectType, PoolIdentifier>.PooledHashSet, PoolIdentifier> pool = new ContainerPool<HashSetPool<ObjectType, PoolIdentifier>.PooledHashSet, PoolIdentifier>();

	public class PooledHashSet : HashSet<ObjectType>
	{
		public void Recycle()
		{
			HashSetPool<ObjectType, PoolIdentifier>.Free(this);
		}
	}
}
