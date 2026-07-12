using System;

public static class PoolsFor<PoolIdentifier>
{
	public static ListPool<ObjectType, PoolIdentifier>.PooledList AllocateList<ObjectType>()
	{
		return ListPool<ObjectType, PoolIdentifier>.Allocate();
	}

	public static HashSetPool<ObjectType, PoolIdentifier>.PooledHashSet AllocateHashSet<ObjectType>()
	{
		return HashSetPool<ObjectType, PoolIdentifier>.Allocate();
	}

	public static DictionaryPool<KeyType, ObjectType, PoolIdentifier>.PooledDictionary AllocateDict<KeyType, ObjectType>()
	{
		return DictionaryPool<KeyType, ObjectType, PoolIdentifier>.Allocate();
	}

	public static QueuePool<ObjectType, PoolIdentifier>.PooledQueue AllocateQueue<ObjectType>()
	{
		return QueuePool<ObjectType, PoolIdentifier>.Allocate();
	}
}
