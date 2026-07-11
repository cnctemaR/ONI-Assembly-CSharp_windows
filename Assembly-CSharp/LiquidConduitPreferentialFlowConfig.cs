using System;
using TUNING;
using UnityEngine;

public class LiquidConduitPreferentialFlowConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LiquidConduitPreferentialFlow";
		int num = 2;
		int num2 = 2;
		string text2 = "valveliquid_kanim";
		int num3 = 10;
		float num4 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Conduit;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.Deprecated = true;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, buildingDef.PrefabID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		this.AttachPort(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		this.AttachPort(go);
	}

	private void AttachPort(GameObject go)
	{
		go.AddComponent<ConduitSecondaryInput>().portInfo = this.secondaryPort;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<ConduitPreferentialFlow>().portInfo = this.secondaryPort;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);
	}

	public const string ID = "LiquidConduitPreferentialFlow";

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

	private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 1));
}
