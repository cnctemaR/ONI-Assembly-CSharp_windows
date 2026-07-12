using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GraveConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Grave";
		int num = 1;
		int num2 = 2;
		string text2 = "gravestone_kanim";
		int num3 = 30;
		float num4 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER1, none, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GraveConfig.STORAGE_OVERRIDE_ANIM_FILES = new KAnimFile[] { Assets.GetAnim("anim_bury_dupe_kanim") };
		GraveStorage graveStorage = go.AddOrGet<GraveStorage>();
		graveStorage.showInUI = true;
		graveStorage.SetDefaultStoredItemModifiers(GraveConfig.StorageModifiers);
		graveStorage.overrideAnims = GraveConfig.STORAGE_OVERRIDE_ANIM_FILES;
		graveStorage.workAnims = GraveConfig.STORAGE_WORK_ANIMS;
		graveStorage.workingPstComplete = new HashedString[] { GraveConfig.STORAGE_PST_ANIM };
		graveStorage.synchronizeAnims = false;
		graveStorage.useGunForDelivery = false;
		graveStorage.workAnimPlayMode = KAnim.PlayMode.Once;
		go.AddOrGet<Grave>();
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().prefabInitFn += this.OnInit;
	}

	private void OnInit(GameObject go)
	{
		GraveStorage graveStorage = go.AddOrGet<GraveStorage>();
		KAnimFile[] array = new KAnimFile[] { Assets.GetAnim("anim_bury_dupe_kanim") };
		graveStorage.workerTypeOverrideAnims.Add(MinionConfig.ID, array);
		graveStorage.workerTypeOverrideAnims.Add(BionicMinionConfig.ID, new KAnimFile[] { Assets.GetAnim("anim_bionic_bury_dupe_kanim") });
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "Grave";

	public const string AnimFile = "gravestone_kanim";

	private static KAnimFile[] STORAGE_OVERRIDE_ANIM_FILES;

	private static readonly HashedString[] STORAGE_WORK_ANIMS = new HashedString[] { "working_pre" };

	private static readonly HashedString STORAGE_PST_ANIM = HashedString.Invalid;

	private static readonly List<Storage.StoredItemModifier> StorageModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve
	};
}
