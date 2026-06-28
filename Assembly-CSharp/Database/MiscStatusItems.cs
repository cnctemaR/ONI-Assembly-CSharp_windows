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
			this.Edible = new StatusItem("Edible", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Edible.resolveStringCallback = delegate(string str, object data)
			{
				Edible edible = (Edible)data;
				str = string.Format(str, GameUtil.GetFormattedCalories(edible.Calories, GameUtil.TimeSlice.None, true));
				return str;
			};
			this.PendingClear = new StatusItem("PendingClear", "MISC", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.MarkedForCompost = new StatusItem("MarkedForCompost", "MISC", "status_item_pending_compost", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.MarkedForCompostInStorage = new StatusItem("MarkedForCompostInStorage", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.MarkedForDisinfection = new StatusItem("MarkedForDisinfection", "MISC", "status_item_disinfect", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Disease, true, 30718);
			this.NoClearLocationsAvailable = new StatusItem("NoClearLocationsAvailable", "MISC", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.WaitingForDig = new StatusItem("WaitingForDig", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.WaitingForMop = new StatusItem("WaitingForMop", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.OreMass = new StatusItem("OreMass", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.OreMass.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject = (GameObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(gameObject.GetComponent<PrimaryElement>().Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.OreTemp = new StatusItem("OreTemp", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.OreTemp.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject2 = (GameObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(gameObject2.GetComponent<PrimaryElement>().Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.ElementalState = new StatusItem("ElementalState", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.ElementalState.resolveStringCallback = delegate(string str, object data)
			{
				Element element = ((Func<Element>)data)();
				str = str.Replace("{State}", element.GetStateString());
				return str;
			};
			this.ElementalCategory = new StatusItem("ElementalCategory", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.ElementalCategory.resolveStringCallback = delegate(string str, object data)
			{
				Element element2 = ((Func<Element>)data)();
				str = str.Replace("{Category}", element2.GetMaterialCategoryTag().ProperName());
				return str;
			};
			this.ElementalTemperature = new StatusItem("ElementalTemperature", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.ElementalTemperature.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject = (CellSelectionObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(cellSelectionObject.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.ElementalMass = new StatusItem("ElementalMass", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.ElementalMass.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject2 = (CellSelectionObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(cellSelectionObject2.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.ElementalDisease = new StatusItem("ElementalDisease", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.ElementalDisease.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject3 = (CellSelectionObject)data;
				str = str.Replace("{Disease}", GameUtil.GetFormattedDisease(cellSelectionObject3.diseaseIdx, cellSelectionObject3.diseaseCount, false));
				return str;
			};
			this.ElementalDisease.resolveTooltipCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject4 = (CellSelectionObject)data;
				str = str.Replace("{Disease}", GameUtil.GetFormattedDisease(cellSelectionObject4.diseaseIdx, cellSelectionObject4.diseaseCount, true));
				return str;
			};
			this.TreeFilterableTags = new StatusItem("TreeFilterableTags", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.TreeFilterableTags.resolveStringCallback = delegate(string str, object data)
			{
				TreeFilterable treeFilterable = (TreeFilterable)data;
				str = str.Replace("{Tags}", treeFilterable.GetTagsAsStatus(6));
				return str;
			};
			this.OxyRockEmitting = new StatusItem("OxyRockEmitting", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.OxyRockEmitting.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject5 = (CellSelectionObject)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(cellSelectionObject5.FlowRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.OxyRockBlocked = new StatusItem("OxyRockBlocked", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.OxyRockBlocked.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject6 = (CellSelectionObject)data;
				bool flag;
				bool flag2;
				GameUtil.IsEmissionBlocked(cellSelectionObject6.SelectedCell, out flag, out flag2);
				string text = null;
				if (flag)
				{
					text = MISC.STATUSITEMS.OXYROCK.NEIGHBORSBLOCKED.NAME;
				}
				else if (flag2)
				{
					text = MISC.STATUSITEMS.OXYROCK.OVERPRESSURE.NAME;
				}
				str = str.Replace("{BlockedString}", text);
				return str;
			};
			this.OxyRockInactive = new StatusItem("OxyRockInactive", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.BuriedItem = new StatusItem("BuriedItem", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.SpoutOverPressure = new StatusItem("SpoutOverPressure", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.SpoutEmitting = new StatusItem("SpoutEmitting", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.SpoutPressureBuilding = new StatusItem("SpoutPressureBuilding", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.OrderAttack = new StatusItem("OrderAttack", "MISC", "status_item_attack", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.PendingHarvest = new StatusItem("PendingHarvest", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.NotMarkedForHarvest = new StatusItem("NotMarkedForHarvest", "MISC", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.NotMarkedForHarvest.conditionalOverlayCallback = (SimViewMode viewMode, object o) => viewMode == SimViewMode.None;
			this.PendingUproot = new StatusItem("PendingUproot", "MISC", "status_item_pending_uproot", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.PickupableUnreachable = new StatusItem("PickupableUnreachable", "MISC", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Prioritized = new StatusItem("Prioritized", "MISC", "status_item_prioritized", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Using = new StatusItem("Using", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Using.resolveStringCallback = delegate(string str, object data)
			{
				Workable workable = (Workable)data;
				if (workable != null)
				{
					KSelectable component = workable.GetComponent<KSelectable>();
					if (component != null)
					{
						str = str.Replace("{Target}", component.GetName());
					}
				}
				return str;
			};
			this.Operating = new StatusItem("Operating", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Cleaning = new StatusItem("Cleaning", "MISC", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.RegionInvalid = new StatusItem("RegionInvalid", "MISC", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, true, 30718);
			this.RegionInvalid.resolveStringCallback = delegate(string str, object data)
			{
				Region region = (Region)data;
				if (region == null)
				{
					global::Debug.LogError("The region provided was null.", null);
					return string.Empty;
				}
				str = string.Format(str, region.GetMissingRequirementsString());
				return str;
			};
			this.RegionNeedsFurniture = new StatusItem("RegionNeedsFurniture", "MISC", "status_item_needs_furniture", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, true, 30718);
			this.RegionNeedsFurniture.resolveStringCallback = delegate(string str, object data)
			{
				Region region2 = (Region)data;
				str = string.Format(str, region2.GetMissingBuildingRequirementsString());
				return str;
			};
			this.RegionNeedsSize = new StatusItem("RegionNeedsSize", "MISC", "status_item_size_requirement", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, true, 30718);
			this.RegionNeedsSize.resolveStringCallback = delegate(string str, object data)
			{
				Region region3 = (Region)data;
				str = string.Format(str, region3.MinHeight, region3.MinWidth);
				return str;
			};
			this.RegionNeedsClosure = new StatusItem("RegionNeedsClosure", "MISC", "status_item_not_enclosed", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, true, 30718);
			this.RegionIsBlocked = new StatusItem("RegionIsBlocked", "MISC", "status_item_solids_blocking", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, true, 30718);
			this.RegionNeedsDoor = new StatusItem("RegionNeedsDoor", "MISC", "status_item_change_door_control_state", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, true, 30718);
		}

		public StatusItem MarkedForDisinfection;

		public StatusItem MarkedForCompost;

		public StatusItem MarkedForCompostInStorage;

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

		public StatusItem ElementalDisease;

		public StatusItem TreeFilterableTags;

		public StatusItem OxyRockInactive;

		public StatusItem OxyRockEmitting;

		public StatusItem OxyRockBlocked;

		public StatusItem BuriedItem;

		public StatusItem SpoutOverPressure;

		public StatusItem SpoutEmitting;

		public StatusItem SpoutPressureBuilding;

		public StatusItem OrderAttack;

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
