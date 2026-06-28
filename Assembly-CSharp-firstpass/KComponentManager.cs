using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class KComponentManager<T> : KCompactedVector<T>, IComponentManager where T : new()
{
	protected KComponentManager()
		: base(0)
	{
	}

	public int Count
	{
		get
		{
			return this.data.Count;
		}
	}

	public bool Has(object go)
	{
		if (this.cleanupList.Contains(go))
		{
			return false;
		}
		HandleVector<int>.Handle handle = this.GetHandle(go);
		return !(handle == HandleVector<int>.InvalidHandle);
	}

	protected HandleVector<int>.Handle InternalAddComponent(object instance, T cmp_values)
	{
		HandleVector<int>.Handle handle = HandleVector<int>.InvalidHandle;
		this.RemoveFromCleanupList(instance);
		if (!this.instanceHandleMap.TryGetValue(instance, out handle))
		{
			handle = base.Allocate(cmp_values);
			this.instanceHandleMap[instance] = handle;
		}
		else
		{
			base.SetData(handle, cmp_values);
		}
		this.spawnList.Remove(handle);
		this.OnPrefabInit(handle);
		this.spawnList.Add(handle);
		Assert.IsTrue(handle.IsValid());
		return handle;
	}

	protected void InternalRemoveComponent(object instance)
	{
		if (!this.instanceHandleMap.ContainsKey(instance))
		{
			Output.LogError(new object[]
			{
				"Tried to remove component of type",
				typeof(T).ToString(),
				"on instance",
				instance.ToString(),
				"but instance has not been registered yet."
			});
			return;
		}
		HandleVector<int>.Handle handle = this.instanceHandleMap[instance];
		base.Free(handle);
		this.instanceHandleMap.Remove(instance);
		this.spawnList.Remove(handle);
	}

	public HandleVector<int>.Handle GetHandle(object instance)
	{
		HandleVector<int>.Handle invalidHandle;
		if (!this.instanceHandleMap.TryGetValue(instance, out invalidHandle))
		{
			invalidHandle = HandleVector<int>.InvalidHandle;
		}
		return invalidHandle;
	}

	public void Spawn()
	{
		this.shadowSpawnList.AddRange(this.spawnList);
		this.spawnList.Clear();
		foreach (object obj in this.cleanupList)
		{
			HandleVector<int>.Handle handle = this.GetHandle(obj);
			this.shadowSpawnList.Remove(handle);
		}
		foreach (HandleVector<int>.Handle handle2 in this.shadowSpawnList)
		{
			this.OnSpawn(handle2);
		}
		this.shadowSpawnList.Clear();
	}

	public virtual void Update(float dt)
	{
	}

	public virtual void FixedUpdate(float dt)
	{
	}

	public virtual void SimUpdate(float dt)
	{
	}

	public void CleanUp()
	{
		this.shadowCleanupList.AddRange(this.cleanupList);
		this.cleanupList.Clear();
		foreach (object obj in this.shadowCleanupList)
		{
			HandleVector<int>.Handle handle = this.GetHandle(obj);
			this.OnCleanUp(handle);
		}
		foreach (object obj2 in this.shadowCleanupList)
		{
			this.InternalRemoveComponent(obj2);
		}
		this.shadowCleanupList.Clear();
	}

	protected void RemoveFromCleanupList(object instance)
	{
		int num = this.cleanupList.IndexOf(instance);
		if (num != -1)
		{
			this.cleanupList[num] = this.cleanupList[this.cleanupList.Count - 1];
			this.cleanupList.RemoveAt(this.cleanupList.Count - 1);
		}
	}

	public override void Clear()
	{
		base.Clear();
		this.spawnList.Clear();
		this.shadowSpawnList.Clear();
		this.cleanupList.Clear();
		this.shadowCleanupList.Clear();
		this.instanceHandleMap.Clear();
	}

	protected virtual void OnPrefabInit(HandleVector<int>.Handle h)
	{
	}

	protected virtual void OnSpawn(HandleVector<int>.Handle h)
	{
	}

	protected virtual void OnCleanUp(HandleVector<int>.Handle h)
	{
	}

	protected Dictionary<object, HandleVector<int>.Handle> instanceHandleMap = new Dictionary<object, HandleVector<int>.Handle>();

	private List<HandleVector<int>.Handle> spawnList = new List<HandleVector<int>.Handle>();

	private List<HandleVector<int>.Handle> shadowSpawnList = new List<HandleVector<int>.Handle>();

	protected List<object> cleanupList = new List<object>();

	private List<object> shadowCleanupList = new List<object>();
}
