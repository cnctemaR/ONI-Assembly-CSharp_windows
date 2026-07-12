using System;
using TUNING;
using UnityEngine;

public class LadderBedConfig : IBuildingConfig
{
	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string id = LadderBedConfig.ID;
		int num = 2;
		int num2 = 2;
		string text = "ladder_bed_kanim";
		int num3 = 10;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloorOrBuildingAttachPoint;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.AttachmentSlotTag = GameTags.LadderBed;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.BedType, false);
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.LadderBed, null)
		};
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		CellOffset[] array = new CellOffset[]
		{
			new CellOffset(0, 0),
			new CellOffset(0, 1)
		};
		Ladder ladder = go.AddOrGet<Ladder>();
		ladder.upwardsMovementSpeedMultiplier = 0.75f;
		ladder.downwardsMovementSpeedMultiplier = 0.75f;
		ladder.offsets = array;
		go.AddOrGetDef<LadderBed.Def>().offsets = array;
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		Bed bed = go.AddOrGet<Bed>();
		bed.effects = new string[] { "LadderBedStamina", "BedHealth" };
		bed.workLayer = Grid.SceneLayer.BuildingFront;
		Sleepable sleepable = go.AddOrGet<Sleepable>();
		sleepable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_ladder_bed_kanim") };
		sleepable.workLayer = Grid.SceneLayer.BuildingFront;
		go.AddOrGet<Ownable>().slotID = Db.Get().AssignableSlots.Bed.Id;
		go.AddOrGetDef<RocketUsageRestriction.Def>();
	}

	public static string ID = "LadderBed";
}
