using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GeneShufflerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("GeneShuffler", SETITEMS.GENESHUFFLER.NAME, SETITEMS.GENESHUFFLER.DESC, 2000f, Assets.GetAnim("geneshuffler_kanim"), "on", Grid.SceneLayer.Building, 4, 3, global::TUNING.BUILDINGS.DECOR.BONUS.TIER0, NOISE_POLLUTION.NOISY.TIER0, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<Operational>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGet<GeneShuffler>();
		gameObject.AddOrGet<LoreBearer>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<Ownable>();
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kbatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		GeneShuffler component = inst.GetComponent<GeneShuffler>();
		component.workLayer = Grid.SceneLayer.Building;
		Ownable component2 = inst.GetComponent<Ownable>();
		component2.slotID = Db.Get().AssignableSlots.GeneShuffler.Id;
		OccupyArea component3 = inst.GetComponent<OccupyArea>();
		component3.objectLayer = ObjectLayer.Building;
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
