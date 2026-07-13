using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GravitasBathroomStallConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "GravitasBathroomStall";
		int num = 2;
		int num2 = 2;
		string text2 = "gravitas_toilet_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.BONUS.TIER0, tier2, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.ShowInBuildMenu = false;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Granite, true);
		component.Temperature = 294.15f;
		BuildingTemplates.ExtendBuildingToGravitas(go);
		go.AddOrGet<Demolishable>();
		go.AddOrGetDef<GravitasBathroomStall.Def>();
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		base.ConfigureBuildingTemplate(go, prefab_tag);
		Activatable activatable = go.AddOrGet<Activatable>();
		activatable.SetWorkTime(5f);
		activatable.SetButtonTextOverride(new ButtonMenuTextOverride
		{
			Text = UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.ACTIVATE_TOILET_BUTTON,
			ToolTip = UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.ACTIVATE_TOILET_BUTTON_TOOLTIP,
			CancelText = UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.ACTIVATE_TOILET_BUTTON_CANCEL,
			CancelToolTip = UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.ACTIVATE_TOILET_BUTTON_CANCEL_TOOLTIP
		});
		activatable.Required = true;
		activatable.synchronizeAnims = true;
		activatable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_gravitas_toilet_kanim") };
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GravitasBathroomStall";
}
