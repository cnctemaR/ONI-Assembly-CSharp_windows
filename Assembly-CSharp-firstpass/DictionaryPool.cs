using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class DictionaryPool<KeyType, ObjectType, PoolIdentifier>
{
	public static DictionaryPool<KeyType, ObjectType, PoolIdentifier>.PooledDictionary Allocate()
	{
		return DictionaryPool<KeyType, ObjectType, PoolIdentifier>.pool.Allocate();
	}

	private static void Free(DictionaryPool<KeyType, ObjectType, PoolIdentifier>.PooledDictionary dictionary)
	{
		dictionary.Clear();
		DictionaryPool<KeyType, ObjectType, PoolIdentifier>.pool.Free(dictionary);
	}

	public static ContainerPool GetPool()
	{
		return DictionaryPool<KeyType, ObjectType, PoolIdentifier>.pool;
	}

	private static ContainerPool<DictionaryPool<KeyType, ObjectType, PoolIdentifier>.PooledDictionary, PoolIdentifier> pool = new ContainerPool<DictionaryPool<KeyType, ObjectType, PoolIdentifier>.PooledDictionary, PoolIdentifier>();

	[DebuggerDisplay("Count={Count}")]
	public class PooledDictionary : Dictionary<KeyType, ObjectType>, IDisposable
	{
		public void Recycle()
		{
			DictionaryPool<KeyType, ObjectType, PoolIdentifier>.Free(this);
		}

		public void Dispose()
		{
			this.Recycle();
		}
	}
}
