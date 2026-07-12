using System;
using Database;
using TUNING;
using UnityEngine;

public class MonumentTopConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MonumentTop";
		int num = 5;
		int num2 = 5;
		string text2 = "monument_upper_a_kanim";
		int num3 = 1000;
		float num4 = 60f;
		float[] array = new float[] { 2500f, 2500f, 5000f };
		string[] array2 = new string[]
		{
			SimHashes.Glass.ToString(),
			SimHashes.Diamond.ToString(),
			SimHashes.Steel.ToString()
		};
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.BuildingAttachPoint;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE, tier, 0.2f);
		BuildingTemplates.CreateMonumentBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.AttachmentSlotTag = "MonumentTop";
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.RequiresPowerInput = false;
		buildingDef.CanMove = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<MonumentPart>().part = MonumentPartResource.Part.Top;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<KBatchedAnimController>().initialAnim = "option_a";
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			MonumentPart monumentPart = game_object.AddOrGet<MonumentPart>();
			monumentPart.part = MonumentPartResource.Part.Top;
			monumentPart.stateUISymbol = "upper";
		};
	}

	public const string ID = "MonumentTop";
}
