using System;
using System.Collections.Generic;
using Klei.AI;
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

		private StatusItem CreateStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, bool showWorldIcon = true, int status_overlays = 129022)
		{
			return base.Add(new StatusItem(id, prefix, icon, icon_type, notification_type, allow_multiples, render_overlay, showWorldIcon, status_overlays, null));
		}

		private StatusItem CreateStatusItem(string id, string name, string tooltip, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, int status_overlays = 129022)
		{
			return base.Add(new StatusItem(id, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays, true, null));
		}

		private void CreateStatusItems()
		{
			this.AttentionRequired = this.CreateStatusItem("AttentionRequired", "MISC", "status_item_doubleexclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.Edible = this.CreateStatusItem("Edible", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.Edible.resolveStringCallback = delegate(string str, object data)
			{
				Edible edible = (Edible)data;
				str = string.Format(str, GameUtil.GetFormattedCalories(edible.Calories, GameUtil.TimeSlice.None, true));
				return str;
			};
			this.PendingClear = this.CreateStatusItem("PendingClear", "MISC", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.PendingClearNoStorage = this.CreateStatusItem("PendingClearNoStorage", "MISC", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			this.MarkedForCompost = this.CreateStatusItem("MarkedForCompost", "MISC", "status_item_pending_compost", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.MarkedForCompostInStorage = this.CreateStatusItem("MarkedForCompostInStorage", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.MarkedForDisinfection = this.CreateStatusItem("MarkedForDisinfection", "MISC", "status_item_disinfect", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.Disease.ID, true, 129022);
			this.NoClearLocationsAvailable = this.CreateStatusItem("NoClearLocationsAvailable", "MISC", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			this.WaitingForDig = this.CreateStatusItem("WaitingForDig", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.WaitingForMop = this.CreateStatusItem("WaitingForMop", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.OreMass = this.CreateStatusItem("OreMass", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.OreMass.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject = (GameObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(gameObject.GetComponent<PrimaryElement>().Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.OreTemp = this.CreateStatusItem("OreTemp", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.OreTemp.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject2 = (GameObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(gameObject2.GetComponent<PrimaryElement>().Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			this.ElementalState = this.CreateStatusItem("ElementalState", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.ElementalState.resolveStringCallback = delegate(string str, object data)
			{
				Element element = ((Func<Element>)data)();
				str = str.Replace("{State}", element.GetStateString());
				return str;
			};
			this.ElementalCategory = this.CreateStatusItem("ElementalCategory", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.ElementalCategory.resolveStringCallback = delegate(string str, object data)
			{
				Element element2 = ((Func<Element>)data)();
				str = str.Replace("{Category}", element2.GetMaterialCategoryTag().ProperName());
				return str;
			};
			this.ElementalTemperature = this.CreateStatusItem("ElementalTemperature", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.ElementalTemperature.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject = (CellSelectionObject)data;
				str = str.Replace("{Temp}", GameUtil.GetFormattedTemperature(cellSelectionObject.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			this.ElementalMass = this.CreateStatusItem("ElementalMass", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.ElementalMass.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject2 = (CellSelectionObject)data;
				str = str.Replace("{Mass}", GameUtil.GetFormattedMass(cellSelectionObject2.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.ElementalDisease = this.CreateStatusItem("ElementalDisease", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
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
			this.TreeFilterableTags = this.CreateStatusItem("TreeFilterableTags", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.TreeFilterableTags.resolveStringCallback = delegate(string str, object data)
			{
				TreeFilterable treeFilterable = (TreeFilterable)data;
				str = str.Replace("{Tags}", treeFilterable.GetTagsAsStatus(6));
				return str;
			};
			this.SublimationEmitting = this.CreateStatusItem("SublimationEmitting", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SublimationEmitting.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject5 = (CellSelectionObject)data;
				if (cellSelectionObject5.element.sublimateId == (SimHashes)0)
				{
					return str;
				}
				str = str.Replace("{Element}", GameUtil.GetElementNameByElementHash(cellSelectionObject5.element.sublimateId));
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(cellSelectionObject5.FlowRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.SublimationEmitting.resolveTooltipCallback = this.SublimationEmitting.resolveStringCallback;
			this.SublimationBlocked = this.CreateStatusItem("SublimationBlocked", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SublimationBlocked.resolveStringCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject6 = (CellSelectionObject)data;
				if (cellSelectionObject6.element.sublimateId == (SimHashes)0)
				{
					return str;
				}
				str = str.Replace("{Element}", cellSelectionObject6.element.name);
				str = str.Replace("{SubElement}", GameUtil.GetElementNameByElementHash(cellSelectionObject6.element.sublimateId));
				return str;
			};
			this.SublimationBlocked.resolveTooltipCallback = this.SublimationBlocked.resolveStringCallback;
			this.SublimationOverpressure = this.CreateStatusItem("SublimationOverpressure", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SublimationOverpressure.resolveTooltipCallback = delegate(string str, object data)
			{
				CellSelectionObject cellSelectionObject7 = (CellSelectionObject)data;
				if (cellSelectionObject7.element.sublimateId == (SimHashes)0)
				{
					return str;
				}
				str = str.Replace("{Element}", cellSelectionObject7.element.name);
				str = str.Replace("{SubElement}", GameUtil.GetElementNameByElementHash(cellSelectionObject7.element.sublimateId));
				return str;
			};
			this.Space = this.CreateStatusItem("Space", "MISC", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022);
			this.BuriedItem = this.CreateStatusItem("BuriedItem", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SpoutOverPressure = this.CreateStatusItem("SpoutOverPressure", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SpoutOverPressure.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance = (Geyser.StatesInstance)data;
				Studyable component = statesInstance.GetComponent<Studyable>();
				if (statesInstance != null && component != null && component.Studied)
				{
					str = str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTOVERPRESSURE.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance.master.RemainingEruptTime(), "F1", false)));
				}
				else
				{
					str = str.Replace("{StudiedDetails}", "");
				}
				return str;
			};
			this.SpoutEmitting = this.CreateStatusItem("SpoutEmitting", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SpoutEmitting.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance2 = (Geyser.StatesInstance)data;
				Studyable component2 = statesInstance2.GetComponent<Studyable>();
				if (statesInstance2 != null && component2 != null && component2.Studied)
				{
					str = str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTEMITTING.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance2.master.RemainingEruptTime(), "F1", false)));
				}
				else
				{
					str = str.Replace("{StudiedDetails}", "");
				}
				return str;
			};
			this.SpoutPressureBuilding = this.CreateStatusItem("SpoutPressureBuilding", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SpoutPressureBuilding.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance3 = (Geyser.StatesInstance)data;
				Studyable component3 = statesInstance3.GetComponent<Studyable>();
				if (statesInstance3 != null && component3 != null && component3.Studied)
				{
					str = str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTPRESSUREBUILDING.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance3.master.RemainingNonEruptTime(), "F1", false)));
				}
				else
				{
					str = str.Replace("{StudiedDetails}", "");
				}
				return str;
			};
			this.SpoutIdle = this.CreateStatusItem("SpoutIdle", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SpoutIdle.resolveStringCallback = delegate(string str, object data)
			{
				Geyser.StatesInstance statesInstance4 = (Geyser.StatesInstance)data;
				Studyable component4 = statesInstance4.GetComponent<Studyable>();
				if (statesInstance4 != null && component4 != null && component4.Studied)
				{
					str = str.Replace("{StudiedDetails}", MISC.STATUSITEMS.SPOUTIDLE.STUDIED.text.Replace("{Time}", GameUtil.GetFormattedCycles(statesInstance4.master.RemainingNonEruptTime(), "F1", false)));
				}
				else
				{
					str = str.Replace("{StudiedDetails}", "");
				}
				return str;
			};
			this.SpoutDormant = this.CreateStatusItem("SpoutDormant", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SpicedFood = this.CreateStatusItem("SpicedFood", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.SpicedFood.resolveTooltipCallback = delegate(string baseString, object data)
			{
				string text = baseString;
				string text2 = "\n    • ";
				foreach (SpiceInstance spiceInstance in ((List<SpiceInstance>)data))
				{
					string text3 = "STRINGS.ITEMS.SPICES.";
					Tag id = spiceInstance.Id;
					string text4 = text3 + id.Name.ToUpper() + ".NAME";
					StringEntry stringEntry;
					Strings.TryGet(text4, out stringEntry);
					string text5 = ((stringEntry == null) ? ("MISSING " + text4) : stringEntry.String);
					text = text + text2 + text5;
					string text6 = "\n        • ";
					if (spiceInstance.StatBonus != null)
					{
						text += Effect.CreateTooltip(spiceInstance.StatBonus, false, text6, false);
					}
				}
				return text;
			};
			this.RehydratedFood = this.CreateStatusItem("RehydratedFood", "MISC", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			this.OrderAttack = this.CreateStatusItem("OrderAttack", "MISC", "status_item_attack", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.OrderCapture = this.CreateStatusItem("OrderCapture", "MISC", "status_item_capture", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.PendingHarvest = this.CreateStatusItem("PendingHarvest", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.NotMarkedForHarvest = this.CreateStatusItem("NotMarkedForHarvest", "MISC", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
			this.NotMarkedForHarvest.conditionalOverlayCallback = (HashedString viewMode, object o) => !(viewMode != OverlayModes.None.ID);
			this.PendingUproot = this.CreateStatusItem("PendingUproot", "MISC", "status_item_pending_uproot", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.PickupableUnreachable = this.CreateStatusItem("PickupableUnreachable", "MISC", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.Prioritized = this.CreateStatusItem("Prioritized", "MISC", "status_item_prioritized", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.Using = this.CreateStatusItem("Using", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.Using.resolveStringCallback = delegate(string str, object data)
			{
				Workable workable = (Workable)data;
				if (workable != null)
				{
					KSelectable component5 = workable.GetComponent<KSelectable>();
					if (component5 != null)
					{
						str = str.Replace("{Target}", component5.GetName());
					}
				}
				return str;
			};
			this.Operating = this.CreateStatusItem("Operating", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.Cleaning = this.CreateStatusItem("Cleaning", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.RegionIsBlocked = this.CreateStatusItem("RegionIsBlocked", "MISC", "status_item_solids_blocking", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.AwaitingStudy = this.CreateStatusItem("AwaitingStudy", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.Studied = this.CreateStatusItem("Studied", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.HighEnergyParticleCount = this.CreateStatusItem("HighEnergyParticleCount", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.HighEnergyParticleCount.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject3 = (GameObject)data;
				return GameUtil.GetFormattedHighEnergyParticles(gameObject3.IsNullOrDestroyed() ? 0f : gameObject3.GetComponent<HighEnergyParticle>().payload, GameUtil.TimeSlice.None, true);
			};
			this.Durability = this.CreateStatusItem("Durability", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.Durability.resolveStringCallback = delegate(string str, object data)
			{
				Durability component6 = ((GameObject)data).GetComponent<Durability>();
				str = str.Replace("{durability}", GameUtil.GetFormattedPercent(component6.GetDurability() * 100f, GameUtil.TimeSlice.None));
				return str;
			};
			this.StoredItemDurability = this.CreateStatusItem("StoredItemDurability", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.StoredItemDurability.resolveStringCallback = delegate(string str, object data)
			{
				Durability component7 = ((GameObject)data).GetComponent<Durability>();
				float num = ((component7 != null) ? (component7.GetDurability() * 100f) : 100f);
				str = str.Replace("{durability}", GameUtil.GetFormattedPercent(num, GameUtil.TimeSlice.None));
				return str;
			};
			this.ClusterMeteorRemainingTravelTime = this.CreateStatusItem("ClusterMeteorRemainingTravelTime", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.ClusterMeteorRemainingTravelTime.resolveStringCallback = delegate(string str, object data)
			{
				float num2 = ((ClusterMapMeteorShower.Instance)data).ArrivalTime - GameUtil.GetCurrentTimeInCycles() * 600f;
				str = str.Replace("{time}", GameUtil.GetFormattedCycles(num2, "F1", false));
				return str;
			};
			this.ArtifactEntombed = this.CreateStatusItem("ArtifactEntombed", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.TearOpen = this.CreateStatusItem("TearOpen", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.TearClosed = this.CreateStatusItem("TearClosed", "MISC", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.MarkedForMove = this.CreateStatusItem("MarkedForMove", "MISC", "status_item_manually_controlled", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			this.MoveStorageUnreachable = this.CreateStatusItem("MoveStorageUnreachable", "MISC", "status_item_manually_controlled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
		}

		public StatusItem AttentionRequired;

		public StatusItem MarkedForDisinfection;

		public StatusItem MarkedForCompost;

		public StatusItem MarkedForCompostInStorage;

		public StatusItem PendingClear;

		public StatusItem PendingClearNoStorage;

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

		public StatusItem SublimationOverpressure;

		public StatusItem SublimationEmitting;

		public StatusItem SublimationBlocked;

		public StatusItem BuriedItem;

		public StatusItem SpoutOverPressure;

		public StatusItem SpoutEmitting;

		public StatusItem SpoutPressureBuilding;

		public StatusItem SpoutIdle;

		public StatusItem SpoutDormant;

		public StatusItem SpicedFood;

		public StatusItem RehydratedFood;

		public StatusItem OrderAttack;

		public StatusItem OrderCapture;

		public StatusItem PendingHarvest;

		public StatusItem NotMarkedForHarvest;

		public StatusItem PendingUproot;

		public StatusItem PickupableUnreachable;

		public StatusItem Prioritized;

		public StatusItem Using;

		public StatusItem Operating;

		public StatusItem Cleaning;

		public StatusItem RegionIsBlocked;

		public StatusItem NoClearLocationsAvailable;

		public StatusItem AwaitingStudy;

		public StatusItem Studied;

		public StatusItem StudiedGeyserTimeRemaining;

		public StatusItem Space;

		public StatusItem HighEnergyParticleCount;

		public StatusItem Durability;

		public StatusItem StoredItemDurability;

		public StatusItem ArtifactEntombed;

		public StatusItem TearOpen;

		public StatusItem TearClosed;

		public StatusItem ClusterMeteorRemainingTravelTime;

		public StatusItem MarkedForMove;

		public StatusItem MoveStorageUnreachable;
	}
}
