using System;
using System.Collections.Generic;
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
		buildingDef.Entombable = true;
		buildingDef.Floodable = false;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
		buildingDef.AddSearchTerms(SEARCH_TERMS.RANCHING);
		buildingDef.AddSearchTerms(SEARCH_TERMS.CRITTER);
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 200f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.allowItemRemoval = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		storage.dropOffset = Vector2.up * 1f;
		storage.storageID = new Tag("FishFeederTop");
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 200f;
		storage2.showInUI = true;
		storage2.showDescriptor = true;
		storage2.allowItemRemoval = false;
		storage2.dropOffset = Vector2.up * 3.5f;
		storage2.storageID = new Tag("FishFeederBot");
		go.AddOrGet<StorageLocker>().choreTypeID = Db.Get().ChoreTypes.RanchingFetch.Id;
		go.AddOrGet<UserNameable>();
		go.AddOrGet<TreeFilterable>().filterAllStoragesOnBuilding = true;
		CreatureFeeder creatureFeeder = go.AddOrGet<CreatureFeeder>();
		creatureFeeder.effectId = "AteFromFeeder";
		creatureFeeder.feederOffset = new CellOffset(0, -2);
		go.GetComponent<KPrefabID>().prefabInitFn += this.OnPrefabInit;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
		go.AddOrGetDef<FishFeeder.Def>();
		go.AddOrGetDef<MakeBaseSolid.Def>().solidOffsets = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
		SymbolOverrideControllerUtil.AddToPrefab(go);
	}

	public override void ConfigurePost(BuildingDef def)
	{
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, Diet> keyValuePair in DietManager.CollectDiets(new Tag[]
		{
			GameTags.Creatures.Species.PacuSpecies,
			GameTags.Creatures.Species.PrehistoricPacuSpecies,
			GameTags.Creatures.Species.ParrotFishSpecies,
			GameTags.Creatures.Species.PufferFishSpecies,
			GameTags.Creatures.Species.SeaHorseSpecies,
			GameTags.Creatures.Species.SeaTurtleSpecies
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
						FishFeederConfig.forbiddenTags.Add(tag);
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
		foreach (Tag tag in FishFeederConfig.forbiddenTags)
		{
			component.ForbiddenTags.Add(tag);
		}
		foreach (KBatchedAnimController kbatchedAnimController in instance.GetComponentsInChildrenOnly<KBatchedAnimController>())
		{
			if (kbatchedAnimController.name.Contains("_fg"))
			{
				kbatchedAnimController.SetBlendValue(KBatchedAnimInstanceData.BlendActiveOptions.LiquidVisibilityLayer, false);
				kbatchedAnimController.SetBlendValue(KBatchedAnimInstanceData.BlendActiveOptions.WaterProof, true);
			}
		}
	}

	public const string ID = "FishFeeder";

	private static HashSet<Tag> forbiddenTags = new HashSet<Tag>();
}
