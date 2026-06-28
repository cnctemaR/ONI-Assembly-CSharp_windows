using System;
using System.Collections.Generic;

public class KPrefabIDTracker
{
	public static KPrefabIDTracker Get()
	{
		if (KPrefabIDTracker.Instance == null)
		{
			KPrefabIDTracker.Instance = new KPrefabIDTracker();
		}
		return KPrefabIDTracker.Instance;
	}

	public static void CleanUp()
	{
		KPrefabIDTracker.Instance = null;
	}

	public void Register(KPrefabID instance, KPrefabID prefab)
	{
		if (instance.InstanceID != -1)
		{
			KPrefabIDTracker.Entry entry = new KPrefabIDTracker.Entry
			{
				id = instance.InstanceID,
				instance = instance,
				prefab = prefab
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
			this.prefabIdMap.Remove(entry.id);
			entry.id = instance.InstanceID;
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

	public KPrefabID GetOriginalPrefab(KPrefabID instance)
	{
		KPrefabIDTracker.Entry entry = default(KPrefabIDTracker.Entry);
		this.entryMap.TryGetValue(instance, out entry);
		return entry.prefab;
	}

	private static KPrefabIDTracker Instance;

	private Dictionary<KPrefabID, KPrefabIDTracker.Entry> entryMap = new Dictionary<KPrefabID, KPrefabIDTracker.Entry>();

	private Dictionary<int, KPrefabID> prefabIdMap = new Dictionary<int, KPrefabID>();

	public struct Entry
	{
		public int id;

		public KPrefabID instance;

		public KPrefabID prefab;
	}
}
