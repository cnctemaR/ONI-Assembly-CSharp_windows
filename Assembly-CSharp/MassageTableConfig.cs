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
		int num3 = 10;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.Overheatable = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.RecBuilding);
		MassageTable massageTable = go.AddOrGet<MassageTable>();
		massageTable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_masseur_kanim") };
		massageTable.stressModificationValue = -30f;
		massageTable.roomBonusValue = -30f;
		massageTable.workLayer = Grid.SceneLayer.BuildingFront;
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.MassageTable.Id;
		ownable.canBePublic = true;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.AddOrGet<CopyBuildingSettings>();
	}

	public const string ID = "MassageTable";
}
