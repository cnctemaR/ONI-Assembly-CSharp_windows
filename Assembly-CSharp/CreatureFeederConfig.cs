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
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 2000f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.allowItemRemoval = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		go.AddOrGet<StorageLocker>().choreTypeID = Db.Get().ChoreTypes.RanchingFetch.Id;
		go.AddOrGet<UserNameable>();
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<CreatureFeeder>();
		go.GetComponent<KPrefabID>().prefabInitFn += this.OnPrefabInit;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
	}

	public override void ConfigurePost(BuildingDef def)
	{
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, Diet> keyValuePair in DietManager.CollectDiets(new Tag[]
		{
			GameTags.Creatures.Species.LightBugSpecies,
			GameTags.Creatures.Species.HatchSpecies,
			GameTags.Creatures.Species.MoleSpecies,
			GameTags.Creatures.Species.CrabSpecies,
			GameTags.Creatures.Species.StaterpillarSpecies,
			GameTags.Creatures.Species.DivergentSpecies,
			GameTags.Creatures.Species.DeerSpecies,
			GameTags.Creatures.Species.BellySpecies,
			GameTags.Creatures.Species.SealSpecies,
			GameTags.Creatures.Species.StegoSpecies,
			GameTags.Creatures.Species.RaptorSpecies,
			GameTags.Creatures.Species.ChameleonSpecies,
			GameTags.Creatures.Species.MooSpecies
		}))
		{
			Diet value = keyValuePair.Value;
			if (value.CanEatPreyCritter)
			{
				Diet.Info[] preyInfos = value.preyInfos;
				for (int i = 0; i < preyInfos.Length; i++)
				{
					foreach (Tag tag in preyInfos[i].consumedTags)
					{
						CreatureFeederConfig.forbiddenTags.Add(tag);
					}
				}
			}
			list.Add(keyValuePair.Key);
		}
		def.BuildingComplete.GetComponent<Storage>().storageFilters = list;
	}

	private void OnPrefabInit(GameObject instance)
	{
		TreeFilterable component = instance.GetComponent<TreeFilterable>();
		foreach (Tag tag in CreatureFeederConfig.forbiddenTags)
		{
			component.ForbiddenTags.Add(tag);
		}
	}

	public const string ID = "CreatureFeeder";

	private static HashSet<Tag> forbiddenTags = new HashSet<Tag>();
}
