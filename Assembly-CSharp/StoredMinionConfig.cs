using System;
using Klei.AI;
using UnityEngine;

public class StoredMinionConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(StoredMinionConfig.ID, StoredMinionConfig.ID, true);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<KPrefabID>();
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<Schedulable>();
		gameObject.AddOrGet<StoredMinionIdentity>();
		gameObject.AddOrGet<KSelectable>().IsSelectable = false;
		gameObject.AddOrGet<MinionModifiers>().addBaseTraits = false;
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
		StoredMinionIdentity.IStoredMinionExtension[] components = Assets.GetPrefab(BionicMinionConfig.ID).GetComponents<StoredMinionIdentity.IStoredMinionExtension>();
		if (components != null)
		{
			for (int i = 0; i < components.Length; i++)
			{
				components[i].AddStoredMinionGameObjectRequirements(go);
			}
		}
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "StoredMinion";
}
