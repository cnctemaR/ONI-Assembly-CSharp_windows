using System;
using System.Collections.Generic;

public abstract class KComponentManager<T> : KCompactedVector<T>, IComponentManager where T : new()
{
	public KComponentManager()
		: base(0)
	{
		this.Name = base.GetType().Name;
	}

	public string Name { get; set; }

	public bool Has(object go)
	{
		if (this.cleanupList.Exists((KComponentManager<T>.CleanupInfo x) => x.instance == go))
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
		return handle;
	}

	protected void InternalRemoveComponent(KComponentManager<T>.CleanupInfo info)
	{
		if (info.instance != null)
		{
			if (!this.instanceHandleMap.ContainsKey(info.instance))
			{
				Output.LogError(new object[]
				{
					"Tried to remove component of type",
					typeof(T).ToString(),
					"on instance",
					info.instance.ToString(),
					"but instance has not been registered yet. Handle:",
					info.handle
				});
				return;
			}
			this.instanceHandleMap.Remove(info.instance);
		}
		else
		{
			foreach (KeyValuePair<object, HandleVector<int>.Handle> keyValuePair in this.instanceHandleMap)
			{
				if (keyValuePair.Value == info.handle)
				{
					this.instanceHandleMap.Remove(keyValuePair.Key);
				}
			}
		}
		base.Free(info.handle);
		this.spawnList.Remove(info.handle);
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
		foreach (KComponentManager<T>.CleanupInfo cleanupInfo in this.cleanupList)
		{
			HandleVector<int>.Handle handle = this.GetHandle(cleanupInfo);
			this.shadowSpawnList.Remove(handle);
		}
		foreach (HandleVector<int>.Handle handle2 in this.shadowSpawnList)
		{
			this.OnSpawn(handle2);
		}
		this.shadowSpawnList.Clear();
	}

	public virtual void RenderEveryTick(float dt)
	{
	}

	public virtual void FixedUpdate(float dt)
	{
	}

	public virtual void Sim200ms(float dt)
	{
	}

	public void CleanUp()
	{
		this.shadowCleanupList.AddRange(this.cleanupList);
		this.cleanupList.Clear();
		foreach (KComponentManager<T>.CleanupInfo cleanupInfo in this.shadowCleanupList)
		{
			this.OnCleanUp(cleanupInfo.handle);
			this.InternalRemoveComponent(cleanupInfo);
		}
		this.shadowCleanupList.Clear();
	}

	protected void RemoveFromCleanupList(object instance)
	{
		for (int i = 0; i < this.cleanupList.Count; i++)
		{
			if (this.cleanupList[i].instance == instance)
			{
				this.cleanupList[i] = this.cleanupList[this.cleanupList.Count - 1];
				this.cleanupList.RemoveAt(this.cleanupList.Count - 1);
				break;
			}
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

	int IComponentManager.get_Count()
	{
		return base.Count;
	}

	protected Dictionary<object, HandleVector<int>.Handle> instanceHandleMap = new Dictionary<object, HandleVector<int>.Handle>();

	private List<HandleVector<int>.Handle> spawnList = new List<HandleVector<int>.Handle>();

	private List<HandleVector<int>.Handle> shadowSpawnList = new List<HandleVector<int>.Handle>();

	protected List<KComponentManager<T>.CleanupInfo> cleanupList = new List<KComponentManager<T>.CleanupInfo>();

	private List<KComponentManager<T>.CleanupInfo> shadowCleanupList = new List<KComponentManager<T>.CleanupInfo>();

	protected struct CleanupInfo
	{
		public CleanupInfo(object instance, HandleVector<int>.Handle handle)
		{
			this.instance = instance;
			this.handle = handle;
		}

		public object instance;

		public HandleVector<int>.Handle handle;
	}
}
