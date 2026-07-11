using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GeneShufflerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GeneShuffler";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.GENESHUFFLER.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.GENESHUFFLER.DESC;
		float num = 2000f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("geneshuffler_kanim"), "on", Grid.SceneLayer.Building, 4, 3, tier, tier2, SimHashes.Creature, null, 293f);
		gameObject.AddTag(GameTags.NotRoomAssignable);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<Operational>();
		gameObject.AddOrGet<Notifier>();
		gameObject.AddOrGet<GeneShuffler>();
		gameObject.AddOrGet<LoreBearer>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<Ownable>();
		gameObject.AddOrGet<Prioritizable>();
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.dropOnLoad = true;
		ManualDeliveryKG manualDeliveryKG = gameObject.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG.requestedItemTag = new Tag("GeneShufflerRecharge");
		manualDeliveryKG.refillMass = 1f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.capacity = 1f;
		KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
		kbatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<GeneShuffler>().workLayer = Grid.SceneLayer.Building;
		inst.GetComponent<Ownable>().slotID = Db.Get().AssignableSlots.GeneShuffler.Id;
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		inst.GetComponent<Deconstructable>();
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
