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
			if (this.prefabIdMap.ContainsKey(instance.InstanceID))
			{
				Debug.LogWarningFormat(instance.gameObject, "KPID instance id {0} was previously used by {1} but we're trying to add it from {2}. Conflict!", new object[]
				{
					instance.InstanceID,
					this.prefabIdMap[instance.InstanceID].gameObject,
					instance.name
				});
			}
			this.prefabIdMap[instance.InstanceID] = instance;
		}
	}

	public void Unregister(KPrefabID instance)
	{
		this.prefabIdMap.Remove(instance.InstanceID);
	}

	public KPrefabID GetInstance(int instance_id)
	{
		KPrefabID kprefabID = null;
		this.prefabIdMap.TryGetValue(instance_id, out kprefabID);
		return kprefabID;
	}

	private static KPrefabIDTracker Instance;

	private Dictionary<int, KPrefabID> prefabIdMap = new Dictionary<int, KPrefabID>();
}
