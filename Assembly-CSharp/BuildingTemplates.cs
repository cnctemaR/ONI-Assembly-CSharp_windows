using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BuildingTemplates
{
	public static BuildingDef CreateBuildingDef(string id, int width, int height, string anim, float mass, int hitpoints, float construction_time, float[] construction_mass, string[] construction_materials, float melting_point, BuildLocationRule build_location_rule, EffectorValues decor, EffectorValues noise)
	{
		BuildingDef buildingDef = ScriptableObject.CreateInstance<BuildingDef>();
		buildingDef.PrefabID = id;
		buildingDef.InitDef();
		buildingDef.name = id;
		buildingDef.Mass = construction_mass;
		buildingDef.MassForTemperatureModification = construction_mass[0] * 0.2f;
		buildingDef.WidthInCells = width;
		buildingDef.HeightInCells = height;
		buildingDef.HitPoints = hitpoints;
		buildingDef.ConstructionTime = construction_time;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.MaterialCategory = construction_materials;
		buildingDef.BaseMeltingPoint = melting_point;
		switch (build_location_rule)
		{
		case BuildLocationRule.Anywhere:
		case BuildLocationRule.Tile:
			buildingDef.ContinuouslyCheckFoundation = false;
			goto IL_009B;
		}
		buildingDef.ContinuouslyCheckFoundation = true;
		IL_009B:
		buildingDef.BuildLocationRule = build_location_rule;
		BuildingTemplates.GetPlanCategory(id, out buildingDef.PlanCategory, out buildingDef.PlanOrder);
		BuildingTemplates.GetResearchRequirement(id, out buildingDef.RequiredTechName);
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.AnimFiles = new KAnimFile[] { Assets.GetAnim(anim) };
		buildingDef.GenerateOffsets();
		buildingDef.BaseDecor = (float)decor.amount;
		buildingDef.BaseDecorRadius = (float)decor.radius;
		buildingDef.BaseNoisePollution = noise.amount;
		buildingDef.BaseNoisePollutionRadius = noise.radius;
		return buildingDef;
	}

	public static void GetPlanCategory(string id, out string planCategory, out float planOrder)
	{
		foreach (KeyValuePair<PlanCategory, string[]> keyValuePair in BUILDINGS.PLANORDER)
		{
			string[] value = keyValuePair.Value;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == id)
				{
					planCategory = keyValuePair.Key.ToString();
					planOrder = (float)i;
					return;
				}
			}
		}
		planCategory = PlanCategory.Base.ToString();
		planOrder = 0f;
	}

	public static void GetResearchRequirement(string id, out string researchName)
	{
		foreach (KeyValuePair<string, string[]> keyValuePair in BUILDINGS.RESEARCH)
		{
			string[] value = keyValuePair.Value;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == id)
				{
					researchName = keyValuePair.Key;
					return;
				}
			}
		}
		researchName = string.Empty;
	}

	public static void CreateStandardBuildingDef(BuildingDef def)
	{
		def.Breakable = true;
	}

	public static void CreateElectricalBuildingDef(BuildingDef def)
	{
		BuildingTemplates.CreateStandardBuildingDef(def);
		def.RequiresPowerInput = true;
		def.ViewMode = SimViewMode.PowerMap;
		def.MaterialCategory = MATERIALS.ALL_METALS;
		def.AudioCategory = "HollowMetal";
	}

	public static Storage CreateDefaultStorage(GameObject go, bool forceCreate = false)
	{
		Storage storage = ((!forceCreate) ? go.AddOrGet<Storage>() : go.AddComponent<Storage>());
		storage.capacityKg = 2000f;
		storage.disableOnStore = true;
		return storage;
	}

	public static void CreateFabricatorStorage(GameObject go, Fabricator fabricator)
	{
		fabricator.inStorage = go.AddComponent<Storage>();
		fabricator.inStorage.capacityKg = 500f;
		fabricator.inStorage.disableOnStore = true;
		fabricator.inStorage.showInUI = true;
		fabricator.inStorage.defaultStoredItemModifers = BuildingTemplates.StoredItemModifiers;
		fabricator.buildStorage = go.AddComponent<Storage>();
		fabricator.buildStorage.capacityKg = 500f;
		fabricator.buildStorage.disableOnStore = true;
		fabricator.buildStorage.showInUI = true;
		fabricator.buildStorage.defaultStoredItemModifers = BuildingTemplates.StoredItemModifiers;
		fabricator.outStorage = go.AddComponent<Storage>();
		fabricator.outStorage.capacityKg = 500f;
		fabricator.outStorage.disableOnStore = true;
		fabricator.outStorage.showInUI = true;
		fabricator.outStorage.allowItemRemoval = true;
		fabricator.outStorage.defaultStoredItemModifers = BuildingTemplates.StoredItemModifiers;
	}

	public static void DoPostConfigure(GameObject go)
	{
	}

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve
	};
}
