using System;
using UnityEngine;

public class EntityConfigManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		EntityConfigManager.Instance = this;
	}

	public void RegisterEntity(IEntityConfig config)
	{
		GameObject gameObject = config.CreatePrefab();
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += config.OnPrefabInit;
		component.prefabSpawnFn += config.OnSpawn;
		Assets.AddPrefab(component);
	}

	public static EntityConfigManager Instance;
}
