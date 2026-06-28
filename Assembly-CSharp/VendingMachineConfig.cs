using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class VendingMachineConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("VendingMachine", SETITEMS.VENDINGMACHINE.NAME, SETITEMS.VENDINGMACHINE.DESC, 100f, Assets.GetAnim("vendingmachine_kanim"), "on", Grid.SceneLayer.Building, 2, 3, global::TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		gameObject.UpdateComponentRequirement<Workable>(true);
		gameObject.UpdateComponentRequirement<VendingMachine>(true);
		gameObject.UpdateComponentRequirement<LoreBearer>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
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
