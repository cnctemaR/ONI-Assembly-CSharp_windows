using System;
using System.Collections.Generic;

public class HashMapObjectPool<PoolKey, PoolValue>
{
	public HashMapObjectPool(Func<PoolKey, PoolValue> instantiator, int initialCount = 0)
	{
		this.initialCount = initialCount;
		this.instantiator = instantiator;
	}

	public HashMapObjectPool(HashMapObjectPool<PoolKey, PoolValue>.IPoolDescriptor[] descriptors, int initialCount = 0)
	{
		this.initialCount = initialCount;
		for (int i = 0; i < descriptors.Length; i++)
		{
			if (this.objectPoolMap.ContainsKey(descriptors[i].PoolId))
			{
				Debug.LogWarning(string.Format("HshMapObjectPool alaready contains key of {0}! Skipping!", descriptors[i].PoolId));
			}
			else
			{
				this.objectPoolMap[descriptors[i].PoolId] = new ObjectPool<PoolValue>(new Func<PoolValue>(descriptors[i].GetInstance), initialCount);
			}
		}
	}

	public PoolValue GetInstance(PoolKey poolId)
	{
		ObjectPool<PoolValue> objectPool;
		if (!this.objectPoolMap.TryGetValue(poolId, out objectPool))
		{
			objectPool = (this.objectPoolMap[poolId] = new ObjectPool<PoolValue>(new Func<PoolValue>(this.PoolInstantiator), this.initialCount));
		}
		this.currentPoolId = poolId;
		return objectPool.GetInstance();
	}

	public void ReleaseInstance(PoolKey poolId, PoolValue inst)
	{
		ObjectPool<PoolValue> objectPool;
		if (inst == null || !this.objectPoolMap.TryGetValue(poolId, out objectPool))
		{
			return;
		}
		objectPool.ReleaseInstance(inst);
	}

	private PoolValue PoolInstantiator()
	{
		if (this.instantiator == null)
		{
			return default(PoolValue);
		}
		return this.instantiator(this.currentPoolId);
	}

	private Dictionary<PoolKey, ObjectPool<PoolValue>> objectPoolMap = new Dictionary<PoolKey, ObjectPool<PoolValue>>();

	private int initialCount;

	private PoolKey currentPoolId;

	private Func<PoolKey, PoolValue> instantiator;

	public interface IPoolDescriptor
	{
		PoolKey PoolId { get; }

		PoolValue GetInstance();
	}
}
