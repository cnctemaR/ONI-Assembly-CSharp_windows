using System;
using TUNING;
using UnityEngine;

public class WallToiletConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "WallToilet";
		int num = 1;
		int num2 = 3;
		string text2 = "toilet_wall_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] plastics = MATERIALS.PLASTICS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.WallFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, plastics, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.DiseaseCellVisName = DUPLICANTSTATS.STANDARD.Secretions.PEE_DISEASE;
		buildingDef.UtilityOutputOffset = new CellOffset(-2, 0);
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		go.AddOrGet<LoopingSounds>();
		component.AddTag(RoomConstraints.ConstraintTags.ToiletType, false);
		component.AddTag(RoomConstraints.ConstraintTags.FlushToiletType, false);
		FlushToilet flushToilet = go.AddOrGet<FlushToilet>();
		flushToilet.massConsumedPerUse = 2.5f;
		flushToilet.massEmittedPerUse = 2.5f + DUPLICANTSTATS.STANDARD.Secretions.PEE_PER_TOILET_PEE;
		flushToilet.newPeeTemperature = DUPLICANTSTATS.STANDARD.Temperature.Internal.IDEAL;
		flushToilet.diseaseId = DUPLICANTSTATS.STANDARD.Secretions.PEE_DISEASE;
		flushToilet.diseasePerFlush = DUPLICANTSTATS.STANDARD.Secretions.DISEASE_PER_PEE;
		flushToilet.diseaseOnDupePerFlush = DUPLICANTSTATS.STANDARD.Secretions.DISEASE_PER_PEE / 5;
		flushToilet.requireOutput = false;
		flushToilet.meterOffset = Meter.Offset.Infront;
		ToiletWorkableUse toiletWorkableUse = go.AddOrGet<ToiletWorkableUse>();
		toiletWorkableUse.workLayer = Grid.SceneLayer.BuildingUse;
		toiletWorkableUse.resetProgressOnStop = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.capacityKG = 2.5f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		AutoStorageDropper.Def def = go.AddOrGetDef<AutoStorageDropper.Def>();
		def.dropOffset = new CellOffset(-2, 0);
		def.elementFilter = new SimHashes[] { SimHashes.Water };
		def.invertElementFilterInitialValue = true;
		def.blockedBySubstantialLiquid = true;
		def.fxOffset = new Vector3(0.5f, 0f, 0f);
		def.leftFx = new AutoStorageDropper.DropperFxConfig
		{
			animFile = "liquidleak_kanim",
			animName = "side",
			flipX = true,
			layer = Grid.SceneLayer.BuildingBack
		};
		def.rightFx = new AutoStorageDropper.DropperFxConfig
		{
			animFile = "liquidleak_kanim",
			animName = "side",
			flipX = false,
			layer = Grid.SceneLayer.BuildingBack
		};
		def.delay = 0f;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 12.5f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.Toilet.Id;
		ownable.canBePublic = true;
		ToiletWorkableClean toiletWorkableClean = go.AddOrGet<ToiletWorkableClean>();
		toiletWorkableClean.workTime = 90f;
		toiletWorkableClean.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_toilet_wall_kanim") };
		toiletWorkableClean.workLayer = Grid.SceneLayer.BuildingFront;
		toiletWorkableClean.SetIsCloggedByGunk(true);
		go.AddOrGetDef<RocketUsageRestriction.Def>();
		component.prefabInitFn += this.OnInit;
	}

	private void OnInit(GameObject go)
	{
		ToiletWorkableUse component = go.GetComponent<ToiletWorkableUse>();
		HashedString[] array = new HashedString[] { "working_pst" };
		KAnimFile[] array2 = new KAnimFile[] { Assets.GetAnim("anim_interacts_toilet_wall_kanim") };
		component.workerTypeOverrideAnims.Add(MinionConfig.ID, array2);
		component.workerTypeOverrideAnims.Add(BionicMinionConfig.ID, new KAnimFile[] { Assets.GetAnim("anim_bionic_interacts_toilet_wall_kanim") });
		component.workerTypePstAnims.Add(MinionConfig.ID, array);
		component.workerTypePstAnims.Add(BionicMinionConfig.ID, new HashedString[] { "working_gunky_pst" });
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	private const float WATER_USAGE = 2.5f;

	public const string ID = "WallToilet";
}
