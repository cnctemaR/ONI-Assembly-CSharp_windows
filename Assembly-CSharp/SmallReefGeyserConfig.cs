using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SmallReefGeyserConfig : IEntityConfig, IHasDlcRestrictions
{
	public GameObject CreatePrefab()
	{
		string id = SmallReefGeyserConfig.ID;
		string text = global::STRINGS.CREATURES.SPECIES.GEYSER.SMALLREEFGEYSER.NAME;
		string text2 = global::STRINGS.CREATURES.SPECIES.GEYSER.SMALLREEFGEYSER.NAME;
		float num = 2000f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim("geyser_reef_kanim");
		string text3 = "inactive";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Building;
		int num2 = 3;
		int num3 = 2;
		EffectorValues effectorValues = tier;
		List<Tag> list = new List<Tag> { GameTags.GeyserFeature };
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, text, text2, num, anim, text3, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, list, 293f);
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Katairite, true);
		component.Temperature = 305.15f;
		gameObject.AddOrGet<LoopingSounds>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(gameObject, false);
		storage.capacityKg = 15000f;
		storage.showInUI = true;
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.storeOnConsume = true;
		elementConsumer.configuration = ElementConsumer.Configuration.AllLiquid;
		elementConsumer.capacityKG = 15000f;
		elementConsumer.consumptionRate = 500f;
		elementConsumer.consumptionRadius = 1;
		elementConsumer.sampleCellOffset = new Vector3(0f, 1f);
		elementConsumer.overrideStatusItemString = global::STRINGS.CREATURES.SPECIES.GEYSER.SMALLREEFGEYSER.LIQUID_CONSUMPTION;
		BreathingGeyser.Def def = gameObject.AddOrGetDef<BreathingGeyser.Def>();
		def.inhaleRate = 500f;
		def.exhaleRate = 166.66667f;
		gameObject.AddOrGet<Submergable>().GetStatusItem = new Func<StatusItem>(SmallReefGeyserConfig.GetSubmergableStatusItem);
		gameObject.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), GameTags.ReefGenerator, null)
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.AddOrGet<Submergable>().GetStatusItem = new Func<StatusItem>(SmallReefGeyserConfig.GetSubmergableStatusItem);
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("geotracker_target", false);
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	private static StatusItem GetSubmergableStatusItem()
	{
		return Db.Get().CreatureStatusItems.NotSubmerged;
	}

	public static string ID = "SmallReefGeyser";

	private const float INHALE_RATE = 500f;

	private const float INHALE_TIME = 30f;

	private const float LIQUID_CAPACITY = 15000f;

	private const float EXHALE_TIME = 90f;

	private const float EXHALE_RATE = 166.66667f;

	public const float APPROXIMATE_EXHALE_TIME_PER_CYCLE = 450f;
}
