using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MissileLauncherConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MissileLauncher";
		int num = 3;
		int num2 = 5;
		string text2 = "missile_launcher_kanim";
		int num3 = 250;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = 400f;
		buildingDef.DefaultAnimState = "off";
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(-1, 0);
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.InputConduitType = ConduitType.Solid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.AddSearchTerms(SEARCH_TERMS.MISSILE);
		buildingDef.POIUnlockable = true;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		this.AddVisualizer(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		this.AddVisualizer(go);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGetDef<MissileLauncher.Def>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.storageID = "MissileBasic";
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.storageFilters = new List<Tag> { "MissileBasic" };
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.capacityKg = 300f;
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.RequestedItemTag = "MissileBasic";
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG.operationalRequirement = Operational.State.None;
		manualDeliveryKG.refillMass = 50f;
		manualDeliveryKG.MinimumMass = 10f;
		manualDeliveryKG.capacity = storage.Capacity();
		Storage storage2 = go.AddComponent<Storage>();
		storage2.storageID = "MissileLongRange";
		storage2.showInUI = true;
		storage2.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage2.storageFilters = MissileLauncherConfig.CosmicBlastShotTypes;
		storage2.allowSettingOnlyFetchMarkedItems = false;
		storage2.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage2.capacityKg = 1000f;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage2);
		manualDeliveryKG2.RequestedItemTag = GameTags.LongRangeMissile;
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG2.operationalRequirement = Operational.State.None;
		manualDeliveryKG2.refillMass = 1000f;
		manualDeliveryKG2.MinimumMass = 200f;
		manualDeliveryKG2.capacity = storage2.Capacity();
		manualDeliveryKG2.FillToMinimumMass = true;
		Storage storage3 = go.AddComponent<Storage>();
		storage3.storageID = "CondiutStorage";
		storage3.capacityKg = 200f;
		storage3.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		storage3.showInUI = false;
		SolidConduitConsumer solidConduitConsumer = go.AddOrGet<SolidConduitConsumer>();
		solidConduitConsumer.alwaysConsume = true;
		solidConduitConsumer.capacityKG = storage3.capacityKg;
		solidConduitConsumer.storage = storage3;
		if (DlcManager.IsContentSubscribed("EXPANSION1_ID"))
		{
			EntityClusterDestinationSelector entityClusterDestinationSelector = go.AddOrGet<EntityClusterDestinationSelector>();
			entityClusterDestinationSelector.assignable = true;
			entityClusterDestinationSelector.sidescreenTitleString = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.TITLE_MISSILE_TARGET;
			entityClusterDestinationSelector.changeTargetButtonTooltipString = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.CHANGE_DESTINATION_BUTTON_TOOLTIP_MISSILE;
			entityClusterDestinationSelector.clearTargetButtonTooltipString = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.CLEAR_DESTINATION_BUTTON_TOOLTIP_MISSILE;
			entityClusterDestinationSelector.requiredEntityLayer = EntityLayer.Meteor;
		}
		this.AddVisualizer(go);
	}

	private void AddVisualizer(GameObject go)
	{
		RangeVisualizer rangeVisualizer = go.AddOrGet<RangeVisualizer>();
		rangeVisualizer.OriginOffset = MissileLauncher.Def.LaunchOffset.ToVector2I();
		rangeVisualizer.RangeMin.x = -MissileLauncher.Def.launchRange.x;
		rangeVisualizer.RangeMax.x = MissileLauncher.Def.launchRange.x;
		rangeVisualizer.RangeMin.y = 0;
		rangeVisualizer.RangeMax.y = MissileLauncher.Def.launchRange.y;
		rangeVisualizer.AllowLineOfSightInvalidCells = true;
		go.GetComponent<KPrefabID>().instantiateFn += delegate(GameObject go)
		{
			go.GetComponent<RangeVisualizer>().BlockingCb = new Func<int, bool>(MissileLauncherConfig.IsCellSkyBlocked);
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
		go.AddOrGet<TreeFilterable>().dropIncorrectOnFilterChange = false;
		FlatTagFilterable flatTagFilterable = go.AddComponent<FlatTagFilterable>();
		flatTagFilterable.displayOnlyDiscoveredTags = false;
		flatTagFilterable.headerText = global::STRINGS.BUILDINGS.PREFABS.MISSILELAUNCHER.TARGET_SELECTION_HEADER;
	}

	public static bool IsCellSkyBlocked(int cell)
	{
		if (PlayerController.Instance != null)
		{
			int num = Grid.InvalidCell;
			BuildTool buildTool = PlayerController.Instance.ActiveTool as BuildTool;
			SelectTool selectTool = PlayerController.Instance.ActiveTool as SelectTool;
			if (buildTool != null)
			{
				num = buildTool.GetLastCell;
			}
			else if (selectTool != null)
			{
				num = Grid.PosToCell(selectTool.selected);
			}
			if (Grid.IsValidCell(cell) && Grid.IsValidCell(num) && Grid.WorldIdx[cell] == Grid.WorldIdx[num])
			{
				return Grid.Solid[cell];
			}
		}
		return false;
	}

	public const string ID = "MissileLauncher";

	public const string CONDUIT_STORAGE = "CondiutStorage";

	public static List<Tag> CosmicBlastShotTypes = new List<Tag> { "MissileLongRange" };
}
