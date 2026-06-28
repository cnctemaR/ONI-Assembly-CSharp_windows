using System;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class MiscStatusItems : StatusItems
	{
		public MiscStatusItems(ResourceSet parent)
			: base("MiscStatusItems", parent)
		{
			this.CreateStatusItems();
		}

		private void CreateStatusItems()
		{
			this.Edible = new StatusItem("Edible", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Edible.resolveStringCallback = delegate(string str, object data)
			{
				Edible edible = (Edible)data;
				str = string.Format(str, GameUtil.GetFormattedCalories(edible.Calories, GameUtil.TimeSlice.None, true));
				return str;
			};
			this.PendingClear = new StatusItem("PendingClear", "MISC", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.NoClearLocationsAvailable = new StatusItem("NoClearLocationsAvailable", "MISC", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.WaitingForDig = new StatusItem("WaitingForDig", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.WaitingForMop = new StatusItem("WaitingForMop", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.OreMass = new StatusItem("OreMass", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.OreMass.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject = (GameObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(gameObject.GetComponent<PrimaryElement>().Mass, GameUtil.TimeSlice.None, true, "{0:0.#}"));
				return str;
			};
			this.OreTemp = new StatusItem("OreTemp", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.OreTemp.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject2 = (GameObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(gameObject2.GetComponent<PrimaryElement>().Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.ElementalState = new StatusItem("ElementalState", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.ElementalState.resolveStringCallback = delegate(string str, object data)
			{
				Element element = ((Func<Element>)data)();
				string text;
				if (element.IsSolid)
				{
					text = ELEMENTS.STATESOLID;
				}
				else if (element.IsLiquid)
				{
					text = ELEMENTS.STATELIQUID;
				}
				else if (element.IsGas)
				{
					text = ELEMENTS.STATEGAS;
				}
				else
				{
					text = ELEMENTS.STATEVACUUM;
				}
				str = str.Replace("{State}", text);
				return str;
			};
			this.ElementalCategory = new StatusItem("ElementalCategory", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.ElementalCategory.resolveStringCallback = delegate(string str, object data)
			{
				Element element2 = ((Func<Element>)data)();
				str = str.Replace("{Category}", element2.GetMaterialCategoryTag().ProperName());
				return str;
			};
			this.ElementalTemperature = new StatusItem("ElementalTemperature", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.ElementalTemperature.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject = (CellSelectionObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(cellSelectionObject.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.ElementalMass = new StatusItem("ElementalMass", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.ElementalMass.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject2 = (CellSelectionObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(cellSelectionObject2.Mass, GameUtil.TimeSlice.None, true, "{0:0.#}"));
				return str;
			};
			this.TreeFilterableTags = new StatusItem("TreeFilterableTags", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.TreeFilterableTags.resolveStringCallback = delegate(string str, object data)
			{
				TreeFilterable treeFilterable = (TreeFilterable)data;
				str = str.Replace("{Tags}", treeFilterable.GetTagsAsStatus(6));
				return str;
			};
			this.OxyRockEmitting = new StatusItem("OxyRockEmitting", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.OxyRockEmitting.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject3 = (CellSelectionObject)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(cellSelectionObject3.FlowRate, GameUtil.TimeSlice.PerSecond, true, "{0:0.#}"));
				return str;
			};
			this.OxyRockBlocked = new StatusItem("OxyRockBlocked", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.OxyRockBlocked.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject4 = (CellSelectionObject)data;
				bool flag;
				bool flag2;
				GameUtil.IsEmissionBlocked(cellSelectionObject4.SelectedCell, out flag, out flag2);
				string text2 = null;
				if (flag)
				{
					text2 = MISC.STATUSITEMS.OXYROCK.NEIGHBORSBLOCKED.NAME;
				}
				else if (flag2)
				{
					text2 = MISC.STATUSITEMS.OXYROCK.OVERPRESSURE.NAME;
				}
				str = str.Replace("{BlockedString}", text2);
				return str;
			};
			this.OxyRockInactive = new StatusItem("OxyRockInactive", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.BuriedItem = new StatusItem("BuriedItem", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.SpoutOverPressure = new StatusItem("SpoutOverPressure", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.SpoutEmitting = new StatusItem("SpoutEmitting", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.SpoutPressureBuilding = new StatusItem("SpoutPressureBuilding", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.PendingHarvest = new StatusItem("PendingHarvest", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.NotMarkedForHarvest = new StatusItem("NotMarkedForHarvest", "MISC", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.NotMarkedForHarvest.conditionalOverlayCallback = (SimViewMode viewMode, object o) => viewMode == SimViewMode.None;
			this.PendingUproot = new StatusItem("PendingUproot", "MISC", "status_item_pending_uproot", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.PickupableUnreachable = new StatusItem("PickupableUnreachable", "MISC", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Prioritized = new StatusItem("Prioritized", "MISC", "status_item_prioritized", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Using = new StatusItem("Using", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Using.resolveStringCallback = delegate(string str, object data)
			{
				Workable workable = (Workable)data;
				str = str.Replace("{Target}", workable.GetComponent<KSelectable>().GetName());
				return str;
			};
			this.Operating = new StatusItem("Operating", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Cleaning = new StatusItem("Cleaning", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.RegionInvalid = new StatusItem("RegionInvalid", "MISC", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None, true);
			this.RegionInvalid.resolveStringCallback = delegate(string str, object data)
			{
				Region region = (Region)data;
				if (region == null)
				{
					Debug.LogError("The region provided was null.");
					return string.Empty;
				}
				str = string.Format(str, region.GetMissingRequirementsString());
				return str;
			};
			this.RegionNeedsFurniture = new StatusItem("RegionNeedsFurniture", "MISC", "status_item_needs_furniture", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None, true);
			this.RegionNeedsFurniture.resolveStringCallback = delegate(string str, object data)
			{
				Region region2 = (Region)data;
				str = string.Format(str, region2.GetMissingBuildingRequirementsString());
				return str;
			};
			this.RegionNeedsSize = new StatusItem("RegionNeedsSize", "MISC", "status_item_size_requirement", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None, true);
			this.RegionNeedsSize.resolveStringCallback = delegate(string str, object data)
			{
				Region region3 = (Region)data;
				str = string.Format(str, region3.MinHeight, region3.MinWidth);
				return str;
			};
			this.RegionNeedsClosure = new StatusItem("RegionNeedsClosure", "MISC", "status_item_not_enclosed", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None, true);
			this.RegionIsBlocked = new StatusItem("RegionIsBlocked", "MISC", "status_item_solids_blocking", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None, true);
			this.RegionNeedsDoor = new StatusItem("RegionNeedsDoor", "MISC", "status_item_change_door_control_state", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None, true);
		}

		public StatusItem PendingClear;

		public StatusItem Edible;

		public StatusItem WaitingForDig;

		public StatusItem WaitingForMop;

		public StatusItem OreMass;

		public StatusItem OreTemp;

		public StatusItem ElementalCategory;

		public StatusItem ElementalState;

		public StatusItem ElementalTemperature;

		public StatusItem ElementalMass;

		public StatusItem TreeFilterableTags;

		public StatusItem OxyRockInactive;

		public StatusItem OxyRockEmitting;

		public StatusItem OxyRockBlocked;

		public StatusItem BuriedItem;

		public StatusItem SpoutOverPressure;

		public StatusItem SpoutEmitting;

		public StatusItem SpoutPressureBuilding;

		public StatusItem PendingHarvest;

		public StatusItem NotMarkedForHarvest;

		public StatusItem PendingUproot;

		public StatusItem PickupableUnreachable;

		public StatusItem Prioritized;

		public StatusItem Using;

		public StatusItem Operating;

		public StatusItem Cleaning;

		public StatusItem RegionInvalid;

		public StatusItem RegionNeedsFurniture;

		public StatusItem RegionNeedsSize;

		public StatusItem RegionNeedsClosure;

		public StatusItem RegionIsBlocked;

		public StatusItem RegionNeedsDoor;

		public StatusItem NoClearLocationsAvailable;
	}
}
