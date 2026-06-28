using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SculptureConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Sculpture", 1, 3, "sculpture_kanim", 100f, 30, 120f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, new DecorValues
		{
			decor = 5,
			radius = 8
		}, null);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = SimViewMode.Decor;
		buildingDef.DefaultAnimState = "slab";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		Artable artable = go.AddComponent<Sculpture>();
		artable.stages.Add(new Artable.Stage("Default", global::STRINGS.BUILDINGS.PREFABS.SCULPTURE.NAME, "slab", 0, 0, false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", global::STRINGS.BUILDINGS.PREFABS.SCULPTURE.POORQUALITYNAME, "crap", 0, 5, false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", global::STRINGS.BUILDINGS.PREFABS.SCULPTURE.AVERAGEQUALITYNAME, "idle", 2, 15, true, Artable.Status.Okay));
	}
}
