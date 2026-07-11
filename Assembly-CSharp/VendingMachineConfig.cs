using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class VendingMachineConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "VendingMachine";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.VENDINGMACHINE.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.VENDINGMACHINE.DESC;
		float num = 100f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("vendingmachine_kanim"), "on", Grid.SceneLayer.Building, 2, 3, tier, tier2, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		SetLocker setLocker = gameObject.AddOrGet<SetLocker>();
		setLocker.machineSound = "VendingMachine_LP";
		setLocker.overrideAnim = "anim_break_kanim";
		setLocker.dropOffset = new Vector2I(1, 1);
		setLocker.possible_contents_ids = new string[] { "FieldRation" };
		gameObject.AddOrGet<LoreBearer>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
