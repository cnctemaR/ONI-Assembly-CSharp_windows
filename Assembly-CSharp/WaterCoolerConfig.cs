using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class WaterCoolerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "WaterCooler";
		int num = 2;
		int num2 = 2;
		string text2 = "watercooler_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER1, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding, false);
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 10f;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Insulate
		});
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.capacity = 10f;
		manualDeliveryKG.refillMass = 9f;
		manualDeliveryKG.MinimumMass = 1f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		HeatImmunityProvider.Def def = go.AddOrGetDef<HeatImmunityProvider.Def>();
		def.range = new CellOffset[][]
		{
			new CellOffset[]
			{
				new CellOffset(1, 0)
			},
			new CellOffset[]
			{
				new CellOffset(0, 0)
			}
		};
		def.overrideFileName = "anim_interacts_watercooler_kanim";
		def.overrideAnims = new string[] { "working_pre", "working_loop", "working_pst" };
		def.specialRequirements = new Func<GameObject, bool>(this.RefreshFromHeatCondition);
		HeatImmunityProvider.Def def2 = def;
		def2.onEffectApplied = (Action<GameObject, HeatImmunityProvider.Instance>)Delegate.Combine(def2.onEffectApplied, new Action<GameObject, HeatImmunityProvider.Instance>(this.OnHeatImmunityEffectApplied));
		go.AddOrGet<WaterCooler>();
		WaterCooler.OnDuplicantDrank = (Action<GameObject, GameObject>)Delegate.Combine(WaterCooler.OnDuplicantDrank, new Action<GameObject, GameObject>(this.ApplyImmunityEffectWhenDrankRecreationally));
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
		go.AddOrGetDef<RocketUsageRestriction.Def>();
	}

	private void ApplyImmunityEffectWhenDrankRecreationally(GameObject duplicant, GameObject waterCoolerInstance)
	{
		HeatImmunityProvider.Instance smi = waterCoolerInstance.GetSMI<HeatImmunityProvider.Instance>();
		if (smi != null)
		{
			smi.ApplyImmunityEffect(duplicant, false);
		}
	}

	private void OnHeatImmunityEffectApplied(GameObject duplicant, HeatImmunityProvider.Instance smi)
	{
		smi.GetSMI<WaterCooler.StatesInstance>().Drink(duplicant, false);
	}

	private bool RefreshFromHeatCondition(GameObject go_instance)
	{
		WaterCooler.StatesInstance smi = go_instance.GetSMI<WaterCooler.StatesInstance>();
		return smi != null && smi.IsInsideState(smi.sm.dispensing);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "WaterCooler";

	public static global::Tuple<Tag, string>[] BEVERAGE_CHOICE_OPTIONS = new global::Tuple<Tag, string>[]
	{
		new global::Tuple<Tag, string>(SimHashes.Water.CreateTag(), ""),
		new global::Tuple<Tag, string>(SimHashes.Milk.CreateTag(), "DuplicantGotMilk")
	};

	public const string MilkEffectID = "DuplicantGotMilk";
}
