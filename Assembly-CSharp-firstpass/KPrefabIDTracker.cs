using System;
using System.Collections.Generic;

public class KPrefabIDTracker
{
	public static void DestroyInstance()
	{
		KPrefabIDTracker.Instance = null;
	}

	public static KPrefabIDTracker Get()
	{
		if (KPrefabIDTracker.Instance == null)
		{
			KPrefabIDTracker.Instance = new KPrefabIDTracker();
		}
		return KPrefabIDTracker.Instance;
	}

	public void Register(KPrefabID instance)
	{
		if (instance.InstanceID != -1)
		{
			KPrefabIDTracker.Entry entry = new KPrefabIDTracker.Entry
			{
				id = instance.InstanceID,
				instance = instance
			};
			this.entryMap[instance] = entry;
			this.prefabIdMap[instance.InstanceID] = instance;
		}
	}

	public void Unregister(KPrefabID instance)
	{
		this.entryMap.Remove(instance);
		this.prefabIdMap.Remove(instance.InstanceID);
	}

	public void Update(KPrefabID instance)
	{
		KPrefabIDTracker.Entry entry = default(KPrefabIDTracker.Entry);
		if (this.entryMap.TryGetValue(instance, out entry))
		{
			entry.id = instance.InstanceID;
			entry.instance = instance;
			this.entryMap[instance] = entry;
			this.prefabIdMap[entry.id] = instance;
		}
	}

	public KPrefabID GetInstance(int instance_id)
	{
		KPrefabID kprefabID = null;
		this.prefabIdMap.TryGetValue(instance_id, out kprefabID);
		return kprefabID;
	}

	private static KPrefabIDTracker Instance;

	private Dictionary<KPrefabID, KPrefabIDTracker.Entry> entryMap = new Dictionary<KPrefabID, KPrefabIDTracker.Entry>();

	private Dictionary<int, KPrefabID> prefabIdMap = new Dictionary<int, KPrefabID>();

	public struct Entry
	{
		public int id;

		public KPrefabID instance;
	}
}
