using System;
using TUNING;
using UnityEngine;

public class MassageTableConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MassageTable", 2, 2, "masseur_kanim", 200f, 10f, BUILDINGS.CONSTRUCTION_MASS.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, null);
		buildingDef.AudioCategory = "Metal";
		buildingDef.MinionEffect = "Sleep";
		buildingDef.Slot = Db.Get().OwnableSlots.RelaxationPoint.Id;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Prioritizable>();
		go.AddOrGet<MassageTable>();
		MassageTable massageTable = go.AddOrGet<MassageTable>();
		massageTable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_masseur_kanim") };
		massageTable.stressModificationValue = -40f;
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
	}
}
