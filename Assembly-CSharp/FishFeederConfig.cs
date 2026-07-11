using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class FishFeederConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "FishFeeder";
		int num = 1;
		int num2 = 3;
		string text2 = "fishfeeder_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, none, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.Entombable = false;
		buildingDef.Floodable = false;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileFront;
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.CreatureFeeder);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 200f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.allowItemRemoval = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 200f;
		storage2.showInUI = true;
		storage2.showDescriptor = true;
		storage2.allowItemRemoval = false;
		go.AddOrGet<StorageLocker>();
		Effect effect = new Effect("AteFromFeeder", global::STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME, global::STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.TOOLTIP, 600f, true, false, false, null, 0f, null);
		effect.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, -0.033333335f, global::STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME, false, false, true));
		effect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 2f, global::STRINGS.CREATURES.MODIFIERS.ATE_FROM_FEEDER.NAME, false, false, true));
		Db.Get().effects.Add(effect);
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<CreatureFeeder>().effectId = effect.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			StorageController.Instance instance = new StorageController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
		go.AddOrGetDef<FishFeeder.Def>();
		go.AddOrGetDef<MakeBaseSolid.Def>();
		SymbolOverrideControllerUtil.AddToPrefab(go);
	}

	public override void ConfigurePost(BuildingDef def)
	{
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, Diet> keyValuePair in DietManager.CollectDiets(new Tag[] { GameTags.Creatures.Species.PacuSpecies }))
		{
			list.Add(keyValuePair.Key);
		}
		def.BuildingComplete.GetComponent<Storage>().storageFilters = list;
	}

	public const string ID = "FishFeeder";
}
