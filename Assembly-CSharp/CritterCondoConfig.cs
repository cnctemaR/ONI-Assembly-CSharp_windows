using System;
using TUNING;
using UnityEngine;

public class CritterCondoConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "CritterCondo";
		int num = 3;
		int num2 = 3;
		string text2 = "critter_condo_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] array = new float[] { 200f, 10f };
		string[] array2 = new string[] { "BuildableRaw", "BuildingFiber" };
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER3, none, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		CritterCondo.Def def = go.AddOrGetDef<CritterCondo.Def>();
		def.IsCritterCondoOperationalCb = (CritterCondo.Instance condo_smi) => condo_smi.GetComponent<RoomTracker>().IsInCorrectRoom() && !condo_smi.GetComponent<Floodable>().IsFlooded;
		def.moveToStatusItem = new StatusItem("CRITTERCONDO.MOVINGTO", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
		def.interactStatusItem = new StatusItem("CRITTERCONDO.INTERACTING", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStationType, false);
	}

	public override void ConfigurePost(BuildingDef def)
	{
	}

	public const string ID = "CritterCondo";

	public const string INTERACTED_WITH_CONDO_EFFECT_ID = "InteractedWithCondo";

	public const float EFFECT_DURATION_IN_SECONDS = 600f;
}
