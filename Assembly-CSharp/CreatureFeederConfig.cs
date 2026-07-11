using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class CreatureFeederConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "CreatureFeeder";
		int num = 1;
		int num2 = 2;
		string text2 = "feeder_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER2, none, 0.2f);
		buildingDef.AudioCategory = "Metal";
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
		storage.capacityKg = 2000f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.allowItemRemoval = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		go.AddOrGet<StorageLocker>();
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<CreatureFeeder>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			StorageController.Instance instance = new StorageController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public override void ConfigurePost(BuildingDef def)
	{
		List<Tag> list = new List<Tag>();
		Tag[] array = new Tag[]
		{
			GameTags.Creatures.Species.LightBugSpecies,
			GameTags.Creatures.Species.HatchSpecies
		};
		foreach (KeyValuePair<Tag, Diet> keyValuePair in DietManager.CollectDiets(array))
		{
			list.Add(keyValuePair.Key);
		}
		def.BuildingComplete.GetComponent<Storage>().storageFilters = list;
	}

	public const string ID = "CreatureFeeder";
}
