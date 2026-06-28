using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class CanvasConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Canvas", 2, 2, "painting_kanim", 100f, 30, 120f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.Anywhere, new DecorValues
		{
			decor = 5,
			radius = 6
		}, null);
		buildingDef.Floodable = false;
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
		buildingDef.Overheatable = false;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = SimViewMode.Decor;
		buildingDef.DefaultAnimState = "off";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		Artable artable = go.AddComponent<Painting>();
		artable.stages.Add(new Artable.Stage("Default", global::STRINGS.BUILDINGS.PREFABS.CANVAS.NAME, "off", 0, 0, false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", global::STRINGS.BUILDINGS.PREFABS.CANVAS.POORQUALITYNAME, "art_a", 0, 5, false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", global::STRINGS.BUILDINGS.PREFABS.CANVAS.AVERAGEQUALITYNAME, "art_b", 2, 10, false, Artable.Status.Okay));
		artable.stages.Add(new Artable.Stage("Good", global::STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_c", 4, 15, true, Artable.Status.Great));
	}
}
