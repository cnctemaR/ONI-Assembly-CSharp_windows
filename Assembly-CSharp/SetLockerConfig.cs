using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SetLockerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SetLocker", SETITEMS.LOCKER.NAME, SETITEMS.LOCKER.DESC, 100f, Assets.GetAnim("setpiece_locker_kanim"), "on", Grid.SceneLayer.Building, 1, 2, global::TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<Workable>();
		gameObject.AddOrGet<SetLocker>();
		gameObject.AddOrGet<LoreBearer>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayer = ObjectLayer.Building;
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
