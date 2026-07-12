using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class PowerControlStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "PowerControlStation";
		int num = 2;
		int num2 = 4;
		string text2 = "electricianworkdesk_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.PowerBuilding, false);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 50f;
		storage.showInUI = true;
		storage.storageFilters = new List<Tag> { PowerControlStationConfig.MATERIAL_FOR_TINKER };
		TinkerStation tinkerstation = go.AddOrGet<TinkerStation>();
		tinkerstation.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_electricianworkdesk_kanim") };
		tinkerstation.inputMaterial = PowerControlStationConfig.MATERIAL_FOR_TINKER;
		tinkerstation.massPerTinker = 5f;
		tinkerstation.outputPrefab = PowerControlStationConfig.TINKER_TOOLS;
		tinkerstation.outputTemperature = 308.15f;
		tinkerstation.requiredSkillPerk = PowerControlStationConfig.ROLE_PERK;
		tinkerstation.choreType = Db.Get().ChoreTypes.PowerFabricate.IdHash;
		tinkerstation.useFilteredStorage = true;
		tinkerstation.fetchChoreType = Db.Get().ChoreTypes.PowerFetch.IdHash;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.PowerPlant.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			TinkerStation component = game_object.GetComponent<TinkerStation>();
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
			tinkerstation.toolProductionTime = 160f;
		};
	}

	public const string ID = "PowerControlStation";

	public static Tag MATERIAL_FOR_TINKER = GameTags.RefinedMetal;

	public static Tag TINKER_TOOLS = PowerStationToolsConfig.tag;

	public const float MASS_PER_TINKER = 5f;

	public static string ROLE_PERK = "CanPowerTinker";

	public const float OUTPUT_TEMPERATURE = 308.15f;
}
