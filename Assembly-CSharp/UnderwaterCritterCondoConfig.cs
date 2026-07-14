using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class UnderwaterCritterCondoConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "UnderwaterCritterCondo";
		int num = 3;
		int num2 = 3;
		string text2 = "underwater_critter_condo_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] array = new float[] { 200f };
		string[] plastics = MATERIALS.PLASTICS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, plastics, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.BONUS.TIER3, none, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.Floodable = false;
		buildingDef.AddSearchTerms(SEARCH_TERMS.CRITTER);
		buildingDef.AddSearchTerms(SEARCH_TERMS.RANCHING);
		buildingDef.AddSearchTerms(SEARCH_TERMS.WATER);
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
	}

	private static StatusItem GetSubmergableStatusItem()
	{
		return Db.Get().BuildingStatusItems.NotSubmerged;
	}

	private static void DisableAllFGSymbols(KBatchedAnimController animController)
	{
		if (animController == null)
		{
			return;
		}
		for (int i = 0; i < UnderwaterCritterCondoConfig.AllFGSymbols.Length; i++)
		{
			string text = UnderwaterCritterCondoConfig.AllFGSymbols[i];
			animController.SetSymbolVisiblity(text, false);
		}
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<BuildingSubmergable>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStationType, false);
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		Effect effect = new Effect("InteractedWithUnderwaterCondo", global::STRINGS.CREATURES.MODIFIERS.CRITTERCONDOINTERACTEFFECT.NAME, global::STRINGS.CREATURES.MODIFIERS.UNDERWATERCRITTERCONDOINTERACTEFFECT.TOOLTIP, 600f, true, true, false, null, -1f, 0f, null, "");
		effect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 1f, global::STRINGS.CREATURES.MODIFIERS.CRITTERCONDOINTERACTEFFECT.NAME, false, false, true));
		Db.Get().effects.Add(effect);
		CritterCondo.Def def = go.AddOrGetDef<CritterCondo.Def>();
		def.IsCritterCondoOperationalCb = delegate(CritterCondo.Instance condo_smi)
		{
			if (!condo_smi.GetComponent<RoomTracker>().IsInCorrectRoom())
			{
				return false;
			}
			Building component = condo_smi.GetComponent<Building>();
			for (int i = 0; i < component.PlacementCells.Length; i++)
			{
				if (!Grid.IsLiquid(component.PlacementCells[i]))
				{
					return false;
				}
			}
			Operational component2 = condo_smi.GetComponent<Operational>();
			return !(component2 != null) || component2.IsOperational;
		};
		def.UpdateForegroundVisibilitySymbols = delegate(KBatchedAnimController foreground_controller, CritterCondo.CreatureFGLayerType layer)
		{
			if (foreground_controller != null)
			{
				UnderwaterCritterCondoConfig.DisableAllFGSymbols(foreground_controller);
				foreground_controller.SetSymbolVisiblity(UnderwaterCritterCondoConfig.AnimFGLayersToSymbolName[layer], true);
			}
		};
		def.moveToStatusItem = new StatusItem("UNDERWATERCRITTERCONDO.MOVINGTO", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
		def.interactStatusItem = new StatusItem("UNDERWATERCRITTERCONDO.INTERACTING", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
		def.condoTag = "UnderwaterCritterCondo";
		def.effectId = effect.Id;
	}

	public override void ConfigurePost(BuildingDef def)
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static UnderwaterCritterCondoConfig()
	{
		Dictionary<CritterCondo.CreatureFGLayerType, string> dictionary = new Dictionary<CritterCondo.CreatureFGLayerType, string>();
		dictionary[CritterCondo.CreatureFGLayerType.SmallCreatureLayer] = UnderwaterCritterCondoConfig.AllFGSymbols[0];
		dictionary[CritterCondo.CreatureFGLayerType.LargeCreatureLayer] = UnderwaterCritterCondoConfig.AllFGSymbols[1];
		dictionary[CritterCondo.CreatureFGLayerType.SquidLayer] = UnderwaterCritterCondoConfig.AllFGSymbols[2];
		UnderwaterCritterCondoConfig.AnimFGLayersToSymbolName = dictionary;
	}

	public const string ID = "UnderwaterCritterCondo";

	public static readonly Operational.Flag Submerged = new Operational.Flag("Submerged", Operational.Flag.Type.Requirement);

	private static string[] AllFGSymbols = new string[] { "doorway_fg", "condo_fg", "doorway_squid_fg" };

	private static Dictionary<CritterCondo.CreatureFGLayerType, string> AnimFGLayersToSymbolName;
}
