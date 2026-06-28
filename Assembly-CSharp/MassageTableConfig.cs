using System;
using TUNING;
using UnityEngine;

public class MassageTableConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MassageTable";
		int num = 2;
		int num2 = 2;
		string text2 = "masseur_kanim";
		float num3 = 200f;
		int num4 = 10;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, tier2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.Overheatable = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.OperatingKilowatts = 0.5f;
		buildingDef.AudioCategory = "Metal";
		buildingDef.HotKey = global::Action.BuildMenuKeyT;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		MassageTable massageTable = go.AddOrGet<MassageTable>();
		massageTable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_masseur_kanim") };
		massageTable.stressModificationValue = -60f;
		massageTable.workLayer = Grid.SceneLayer.BuildingFront;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.AddOrGet<CopyBuildingSettings>();
	}

	public const string ID = "MassageTable";
}
