using System;
using TUNING;
using UnityEngine;

public class SteamEngineConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SteamEngine";
		int num = 7;
		int num2 = 5;
		string text2 = "rocket_steam_engine_kanim";
		int num3 = 1000;
		float num4 = 480f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER7;
		string[] array = new string[] { SimHashes.Steel.ToString() };
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloorOrBuildingAttachPoint;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, array, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.ViewMode = SimViewMode.None;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.RequiresPowerInput = false;
		buildingDef.CanMove = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		RocketEngine rocketEngine = go.AddOrGet<RocketEngine>();
		rocketEngine.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
		go.AddOrGet<RocketModule>();
		EntityTemplates.ExtendEntityToRocketModule(go);
	}

	public const string ID = "SteamEngine";
}
