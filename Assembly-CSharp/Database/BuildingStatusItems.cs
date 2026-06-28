using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class BuildingStatusItems : StatusItems
	{
		public BuildingStatusItems(ResourceSet parent)
			: base("BuildingStatusItems", parent)
		{
			this.CreateStatusItems();
		}

		private void CreateStatusItems()
		{
			this.AngerDamage = new StatusItem("AngerDamage", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.AssignedTo = new StatusItem("AssignedTo", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.AssignedTo.resolveStringCallback = delegate(string str, object data)
			{
				Assignable assignable = (Assignable)data;
				Assignables assignee = assignable.assignee;
				if (assignee != null)
				{
					string name = assignee.GetComponent<KSelectable>().GetName();
					str = str.Replace("{Assignee}", name);
				}
				return str;
			};
			this.Broken = new StatusItem("Broken", "BUILDING", "status_item_broken", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Broken.resolveStringCallback = delegate(string str, object data)
			{
				BuildingHP.SMInstance sminstance = (BuildingHP.SMInstance)data;
				return str.Replace("{DamageInfo}", sminstance.master.GetDamageSourceInfo().ToString());
			};
			this.Broken.conditionalOverlayCallback = delegate(SimViewMode mode, object data)
			{
				Transform transform = (Transform)data;
				bool flag = false;
				if (mode != SimViewMode.LiquidVentMap)
				{
					if (mode != SimViewMode.PowerMap)
					{
						if (mode == SimViewMode.GasVentMap)
						{
							Conduit component = transform.GetComponent<Conduit>();
							flag = component != null && component.type == ConduitType.Gas;
						}
					}
					else
					{
						flag = transform.GetComponent<Wire>() != null;
					}
				}
				else
				{
					Conduit component2 = transform.GetComponent<Conduit>();
					flag = component2 != null && component2.type == ConduitType.Liquid;
				}
				return flag;
			};
			this.ChangeDoorControlState = new StatusItem("ChangeDoorControlState", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.ChangeDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door = (Door)data;
				return str.Replace("{ControlState}", door.RequestedState.ToString());
			};
			this.CurrentDoorControlState = new StatusItem("CurrentDoorControlState", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.CurrentDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door2 = (Door)data;
				string text = Strings.Get("STRINGS.BUILDING.STATUSITEMS.CURRENTDOORCONTROLSTATE." + door2.CurrentState.ToString().ToUpper());
				return str.Replace("{ControlState}", text);
			};
			this.ConduitBlocked = new StatusItem("ConduitBlocked", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.ConstructionUnreachable = new StatusItem("ConstructionUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.DigUnreachable = new StatusItem("DigUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.DirectionControl = new StatusItem("DirectionControl", BUILDING.STATUSITEMS.DIRECTION_CONTROL.NAME, BUILDING.STATUSITEMS.DIRECTION_CONTROL.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, 2046);
			this.DirectionControl.resolveStringCallback = delegate(string str, object data)
			{
				DirectionControl directionControl = (DirectionControl)data;
				string text2 = BUILDING.STATUSITEMS.DIRECTION_CONTROL.DIRECTIONS.BOTH;
				WorkableReactable.AllowedDirection allowedDirection = directionControl.allowedDirection;
				if (allowedDirection != WorkableReactable.AllowedDirection.Left)
				{
					if (allowedDirection == WorkableReactable.AllowedDirection.Right)
					{
						text2 = BUILDING.STATUSITEMS.DIRECTION_CONTROL.DIRECTIONS.RIGHT;
					}
				}
				else
				{
					text2 = BUILDING.STATUSITEMS.DIRECTION_CONTROL.DIRECTIONS.LEFT;
				}
				str = str.Replace("{Direction}", text2);
				return str;
			};
			this.ConstructableDigUnreachable = new StatusItem("ConstructableDigUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Entombed = new StatusItem("Entombed", "BUILDING", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Entombed.AddNotification(null, null, null, 0f);
			this.Flooded = new StatusItem("Flooded", "BUILDING", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Flooded.AddNotification(null, null, null, 0f);
			this.GasVentObstructed = new StatusItem("GasVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None, true, 2046);
			this.GasVentOverPressure = new StatusItem("GasVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None, true, 2046);
			this.GeneShuffleCompleted = new StatusItem("GeneShuffleCompleted", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.InvalidBuildingLocation = new StatusItem("InvalidBuildingLocation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.LiquidVentObstructed = new StatusItem("LiquidVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None, true, 2046);
			this.LiquidVentOverPressure = new StatusItem("LiquidVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None, true, 2046);
			this.MaterialsUnavailable = new MaterialsUnavailableStatusItem("MaterialsUnavailable", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, true, SimViewMode.None, SimViewMode.None);
			this.MaterialsUnavailable.AddNotification(null, null, null, 0f);
			this.MaterialsUnavailable.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList = (IFetchList)data;
				string text3 = string.Empty;
				Dictionary<Tag, float> remainingMinimum = fetchList.GetRemainingMinimum();
				if (remainingMinimum.Count > 0)
				{
					bool flag2 = true;
					foreach (KeyValuePair<Tag, float> keyValuePair in remainingMinimum)
					{
						if (keyValuePair.Value != 0f)
						{
							if (!flag2)
							{
								text3 += "\n";
							}
							if (Assets.IsTagCountable(keyValuePair.Key))
							{
								text3 += string.Format(BUILDING.STATUSITEMS.MATERIALSUNAVAILABLE.LINE_ITEM_UNITS, GameUtil.GetUnitFormattedName(keyValuePair.Key.ProperName(), keyValuePair.Value, false));
							}
							else
							{
								text3 += string.Format(BUILDING.STATUSITEMS.MATERIALSUNAVAILABLE.LINE_ITEM_MASS, keyValuePair.Key.ProperName(), GameUtil.GetFormattedMass(keyValuePair.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							}
							flag2 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text3);
				return str;
			};
			this.MaterialsUnavailableForRefill = new MaterialsUnavailableForRefillStatusItem("MaterialsUnavailableForRefill", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, true, SimViewMode.None, SimViewMode.None);
			this.MaterialsUnavailableForRefill.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList2 = (IFetchList)data;
				string text4 = string.Empty;
				Dictionary<Tag, float> remaining = fetchList2.GetRemaining();
				if (remaining.Count > 0)
				{
					bool flag3 = true;
					foreach (KeyValuePair<Tag, float> keyValuePair2 in remaining)
					{
						if (keyValuePair2.Value != 0f)
						{
							if (!flag3)
							{
								text4 += "\n";
							}
							text4 += string.Format(BUILDING.STATUSITEMS.MATERIALSUNAVAILABLEFORREFILL.LINE_ITEM, keyValuePair2.Key.ProperName());
							flag3 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text4);
				return str;
			};
			this.WaitingForRepairMaterials = new StatusItem("WaitingForRepairMaterials", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Exclamation, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None, false, 2046);
			this.WaitingForRepairMaterials.resolveStringCallback = delegate(string str, object data)
			{
				KeyValuePair<Tag, float> keyValuePair3 = (KeyValuePair<Tag, float>)data;
				if (keyValuePair3.Value != 0f)
				{
					string text5 = string.Format(BUILDING.STATUSITEMS.WAITINGFORMATERIALS.LINE_ITEM_MASS, keyValuePair3.Key.ProperName(), GameUtil.GetFormattedMass(keyValuePair3.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
					str = str.Replace("{ItemsRemaining}", text5);
				}
				return str;
			};
			this.WaitingForMaterials = new WaitingForMaterialsStatusItem("WaitingForMaterials", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None);
			this.WaitingForMaterials.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList3 = (IFetchList)data;
				string text6 = string.Empty;
				Dictionary<Tag, float> remaining2 = fetchList3.GetRemaining();
				if (remaining2.Count > 0)
				{
					bool flag4 = true;
					foreach (KeyValuePair<Tag, float> keyValuePair4 in remaining2)
					{
						if (keyValuePair4.Value != 0f)
						{
							if (!flag4)
							{
								text6 += "\n";
							}
							if (Assets.IsTagCountable(keyValuePair4.Key))
							{
								text6 += string.Format(BUILDING.STATUSITEMS.WAITINGFORMATERIALS.LINE_ITEM_UNITS, GameUtil.GetUnitFormattedName(keyValuePair4.Key.ProperName(), keyValuePair4.Value, false));
							}
							else
							{
								text6 += string.Format(BUILDING.STATUSITEMS.WAITINGFORMATERIALS.LINE_ITEM_MASS, keyValuePair4.Key.ProperName(), GameUtil.GetFormattedMass(keyValuePair4.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							}
							flag4 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text6);
				return str;
			};
			this.MeltingDown = new StatusItem("MeltingDown", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.MissingFoundation = new StatusItem("MissingFoundation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NeedBoringMachine = new StatusItem("NeedBoringMachine", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NeutroniumUnminable = new StatusItem("NeutroniumUnminable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NeedGasIn = new StatusItem("NeedGasIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None, true, 2046);
			this.NeedGasOut = new StatusItem("NeedGasOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None, true, 2046);
			this.NeedLiquidIn = new StatusItem("NeedLiquidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None, true, 2046);
			this.NeedLiquidOut = new StatusItem("NeedLiquidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None, true, 2046);
			this.LiquidPipeEmpty = new StatusItem("LiquidPipeEmpty", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None, true, 2046);
			this.LiquidPipeObstructed = new StatusItem("LiquidPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.LiquidVentMap, SimViewMode.None, true, 2046);
			this.GasPipeEmpty = new StatusItem("GasPipeEmpty", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None, true, 2046);
			this.GasPipeObstructed = new StatusItem("GasPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.GasVentMap, SimViewMode.None, true, 2046);
			this.NeedPlant = new StatusItem("NeedPlant", "BUILDING", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NeedPower = new StatusItem("NeedPower", "BUILDING", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.NewDuplicantsAvailable = new StatusItem("NewDuplicantsAvailable", "BUILDING", "status_item_new_duplicants_available", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NewDuplicantsAvailable.AddNotification(null, null, null, 0f);
			this.NewDuplicantsAvailable.notificationClickCallback = delegate(object data)
			{
				Telepad telepad = (Telepad)data;
				ImmigrantScreen.InitializeImmigrantScreen(telepad);
			};
			this.NoStorageFilterSet = new StatusItem("NoStorageFilterSet", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.Regions, SimViewMode.None, true, 2046);
			this.NoFishableWaterBelow = new StatusItem("NoFishableWaterBelow", "BUILDING", "status_item_no_fishable_water_below", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NoPowerConsumers = new StatusItem("NoPowerConsumers", "BUILDING", "status_item_no_power_consumers", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.NoPowerSource = new StatusItem("NoPowerSource", "BUILDING", "status_item_no_power_source", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.NoWireConnected = new StatusItem("NoWireConnected", "BUILDING", "status_item_no_wire_connected", StatusItem.IconType.Custom, NotificationType.BadMinor, true, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.PendingDeconstruction = new StatusItem("PendingDeconstruction", "BUILDING", "status_item_pending_deconstruction", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PendingDeconstruction.conditionalOverlayCallback = delegate(SimViewMode mode, object data)
			{
				Transform transform2 = (Transform)data;
				bool flag5 = false;
				if (mode != SimViewMode.LiquidVentMap)
				{
					if (mode != SimViewMode.PowerMap)
					{
						if (mode == SimViewMode.GasVentMap)
						{
							Conduit component3 = transform2.GetComponent<Conduit>();
							flag5 = component3 != null && component3.type == ConduitType.Gas;
						}
					}
					else
					{
						flag5 = transform2.GetComponent<Wire>() != null;
					}
				}
				else
				{
					Conduit component4 = transform2.GetComponent<Conduit>();
					flag5 = component4 != null && component4.type == ConduitType.Liquid;
				}
				return flag5;
			};
			this.PendingRepair = new StatusItem("PendingRepair", "BUILDING", "status_item_pending_repair", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PendingRepair.resolveStringCallback = delegate(string str, object data)
			{
				Repairable.SMInstance sminstance2 = (Repairable.SMInstance)data;
				BuildingHP component5 = sminstance2.master.GetComponent<BuildingHP>();
				return str.Replace("{DamageInfo}", component5.GetDamageSourceInfo().ToString());
			};
			this.PendingRepair.conditionalOverlayCallback = (SimViewMode mode, object data) => true;
			this.SwitchStatusActive = new StatusItem("SwitchStatusActive", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.SwitchStatusInactive = new StatusItem("SwitchStatusInactive", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PendingFish = new StatusItem("PendingFish", "BUILDING", "status_item_pending_fish", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PendingSwitchToggle = new StatusItem("PendingSwitchToggle", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PendingUpgrade = new StatusItem("PendingUpgrade", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PendingWork = new StatusItem("PendingWork", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PowerButtonOff = new StatusItem("PowerButtonOff", "BUILDING", "status_item_power_button_off", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PressureOk = new StatusItem("PressureOk", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.OxygenMap, SimViewMode.None, true, 2046);
			this.UnderPressure = new StatusItem("UnderPressure", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.OxygenMap, SimViewMode.None, true, 2046);
			this.Unassigned = new StatusItem("Unassigned", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.Rooms, SimViewMode.None, true, 2046);
			this.UnderConstruction = new StatusItem("UnderConstruction", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.UnderConstructionNoWorker = new StatusItem("UnderConstructionNoWorker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Normal = new StatusItem("Normal", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.ManualGeneratorChargingUp = new StatusItem("ManualGeneratorChargingUp", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.ManualGeneratorReleasingEnergy = new StatusItem("ManualGeneratorReleasingEnergy", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.GeneratorOffline = new StatusItem("GeneratorOffline", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.Pipe = new StatusItem("Pipe", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.LiquidVentMap, SimViewMode.GasVentMap, true, 2046);
			this.Pipe.resolveStringCallback = delegate(string str, object data)
			{
				Conduit conduit = (Conduit)data;
				int num = Grid.PosToCell(conduit);
				ConduitFlow flowManager = conduit.GetFlowManager();
				ConduitFlow.ConduitContents contents = flowManager.GetContents(num);
				string text7 = BUILDING.STATUSITEMS.PIPECONTENTS.EMPTY;
				if (contents.mass > 0f)
				{
					Element element = ElementLoader.FindElementByHash(contents.element);
					text7 = string.Format(BUILDING.STATUSITEMS.PIPECONTENTS.CONTENTS, GameUtil.GetFormattedMass(contents.mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), element.name, GameUtil.GetFormattedTemperature(contents.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
					if (OverlayScreen.Instance != null && OverlayScreen.Instance.mode == SimViewMode.Disease && contents.diseaseIdx != 255)
					{
						text7 += string.Format(BUILDING.STATUSITEMS.PIPECONTENTS.CONTENTS_WITH_DISEASE, GameUtil.GetFormattedDisease(contents.diseaseIdx, contents.diseaseCount, true));
					}
				}
				str = str.Replace("{Contents}", text7);
				return str;
			};
			this.FabricatorEmpty = new StatusItem("FabricatorEmpty", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.Regions, SimViewMode.None, true, 2046);
			this.Toilet = new StatusItem("Toilet", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Toilet.resolveStringCallback = delegate(string str, object data)
			{
				Toilet.StatesInstance statesInstance = (Toilet.StatesInstance)data;
				if (statesInstance != null)
				{
					str = str.Replace("{FlushesRemaining}", statesInstance.GetFlushesRemaining().ToString());
				}
				return str;
			};
			this.ToiletNeedsEmptying = new StatusItem("ToiletNeedsEmptying", "BUILDING", "status_item_toilet_needs_emptying", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Unusable = new StatusItem("Unusable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NoResearchSelected = new StatusItem("NoResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NoResearchSelected.AddNotification(null, null, null, 0f);
			this.NoApplicableResearchSelected = new StatusItem("NoApplicableResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NoApplicableResearchSelected.AddNotification(null, null, null, 0f);
			this.ValveRequest = new StatusItem("ValveRequest", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.ValveRequest.resolveStringCallback = delegate(string str, object data)
			{
				Valve valve = (Valve)data;
				str = str.Replace("{QueuedMaxFlow}", GameUtil.GetFormattedMass(valve.QueuedMaxFlow, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.EmittingLight = new StatusItem("EmittingLight", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.EmittingLight.resolveStringCallback = delegate(string str, object data)
			{
				string text8 = GameInputMapping.FindEntry(global::Action.Overlay5).mKeyCode.ToString();
				str = str.Replace("{LightGridOverlay}", text8);
				return str;
			};
			this.RationBoxContents = new StatusItem("RationBoxContents", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.RationBoxContents.resolveStringCallback = delegate(string str, object data)
			{
				RationBox rationBox = (RationBox)data;
				if (rationBox == null)
				{
					return str;
				}
				Storage component6 = rationBox.GetComponent<Storage>();
				if (component6 == null)
				{
					return str;
				}
				float num2 = 0f;
				foreach (GameObject gameObject in component6)
				{
					Edible component7 = gameObject.GetComponent<Edible>();
					if (component7)
					{
						num2 += component7.Calories;
					}
				}
				str = str.Replace("{Stored}", GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true));
				return str;
			};
			this.EmittingElement = new StatusItem("EmittingElement", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.EmittingElement.resolveStringCallback = delegate(string str, object data)
			{
				BuildingElementEmitter buildingElementEmitter = (BuildingElementEmitter)data;
				string text9 = ElementLoader.FindElementByHash(buildingElementEmitter.element).tag.ProperName();
				str = str.Replace("{ElementType}", text9);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(buildingElementEmitter.AverageEmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.EmittingOxygenAvg = new StatusItem("EmittingOxygenAvg", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.EmittingOxygenAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates = (Sublimates)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates.AvgFlowRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.EmittingGasAvg = new StatusItem("EmittingGasAvg", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.EmittingGasAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates2 = (Sublimates)data;
				str = str.Replace("{Element}", ElementLoader.FindElementByHash(sublimates2.info.sublimatedElement).name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates2.AvgFlowRate(), GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.PumpingLiquidOrGas = new StatusItem("PumpingLiquidOrGas", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.LiquidVentMap, SimViewMode.GasVentMap, true, 2046);
			this.PumpingLiquidOrGas.resolveStringCallback = delegate(string str, object data)
			{
				Accumulator accumulator = (Accumulator)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(accumulator.AvgRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.NoLiquidElementToPump = new StatusItem("NoLiquidElementToPump", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None, true, 2046);
			this.NoGasElementToPump = new StatusItem("NoGasElementToPump", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None, true, 2046);
			this.NoFilterElementSelected = new StatusItem("NoFilterElementSelected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.ElementConsumer = new StatusItem("ElementConsumer", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None, true, 2046);
			this.ElementConsumer.resolveStringCallback = delegate(string str, object data)
			{
				ElementConsumer elementConsumer = (ElementConsumer)data;
				string text10 = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag.ProperName();
				str = str.Replace("{ElementTypes}", text10);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementConsumer.AverageConsumeRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.ElementEmitterOutput = new StatusItem("ElementEmitterOutput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None, true, 2046);
			this.ElementEmitterOutput.resolveStringCallback = delegate(string str, object data)
			{
				ElementEmitter elementEmitter = (ElementEmitter)data;
				if (elementEmitter != null)
				{
					str = str.Replace("{ElementTypes}", elementEmitter.outputElement.Name);
					str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementEmitter.outputElement.massGenerationRate / elementEmitter.emissionFrequency, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				}
				return str;
			};
			this.AwaitingWaste = new StatusItem("AwaitingWaste", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None, true, 2046);
			this.AwaitingCompostFlip = new StatusItem("AwaitingCompostFlip", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None, true, 2046);
			this.JoulesAvailable = new StatusItem("JoulesAvailable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.JoulesAvailable.resolveStringCallback = delegate(string str, object data)
			{
				IEnergyProducer energyProducer = (IEnergyProducer)data;
				str = str.Replace("{JoulesAvailable}", GameUtil.GetFormattedJoules(energyProducer.JoulesAvailable, "F1"));
				return str;
			};
			this.Wattage = new StatusItem("Wattage", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.Wattage.resolveStringCallback = delegate(string str, object data)
			{
				Generator generator = (Generator)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(generator.WattageRating, "F1"));
				return str;
			};
			this.Wattson = new StatusItem("Wattson", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Wattson.resolveStringCallback = delegate(string str, object data)
			{
				Telepad telepad2 = (Telepad)data;
				if (GameFlowManager.Instance != null && GameFlowManager.Instance.IsGameOver())
				{
					str = BUILDING.STATUSITEMS.WATTSONGAMEOVER.NAME;
				}
				else if (telepad2.GetComponent<Operational>().IsOperational)
				{
					str = str.Replace("{TimeRemaining}", GameUtil.GetFormattedCycles(telepad2.GetTimeRemaining(), "F1"));
				}
				else
				{
					str = str.Replace("{TimeRemaining}", BUILDING.STATUSITEMS.WATTSON.UNAVAILABLE);
				}
				return str;
			};
			this.FlushToilet = new StatusItem("FlushToilet", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.FlushToiletInUse = new StatusItem("FlushToiletInUse", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.WireNominal = new StatusItem("WireNominal", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.WireConnected = new StatusItem("WireConnected", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.WireDisconnected = new StatusItem("WireDisconnected", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None, true, 2046);
			this.Overheated = new StatusItem("Overheated", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Cooling = new StatusItem("Cooling", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.CoolingStalledColdGas = new StatusItem("CoolingStalledColdGas", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.CoolingStalledColdGas.resolveStringCallback = delegate(string str, object data)
			{
				AirConditioner airConditioner = (AirConditioner)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(airConditioner.lastGasTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			};
			this.CoolingStalledHotEnv = new StatusItem("CoolingStalledHotEnv", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.CoolingStalledHotEnv.resolveStringCallback = delegate(string str, object data)
			{
				AirConditioner airConditioner2 = (AirConditioner)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(airConditioner2.lastEnvTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(airConditioner2.lastGasTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(airConditioner2.maxEnvironmentDelta, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative, true));
			};
			this.Working = new StatusItem("Working", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NeedsRegion = new StatusItem("NeedsRegion", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None, true, 2046);
			this.NeedsRegion.resolveStringCallback = delegate(string str, object data)
			{
				RequiresRegion requiresRegion = (RequiresRegion)data;
				KPrefabID component8 = requiresRegion.GetComponent<KPrefabID>();
				if (component8 == null)
				{
					global::Debug.LogError("The object provided does not have a prefabID.", null);
					return string.Empty;
				}
				string text11 = requiresRegion.RequiredRegions.ToString();
				str = string.Format(str, text11);
				return str;
			};
			this.NeedsValidRegion = new StatusItem("NeedsValidRegion", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None, true, 2046);
			this.NeedSeed = new StatusItem("NeedSeed", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.AwaitingSeedDelivery = new StatusItem("AwaitingSeedDelivery", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.NoAvailableSeed = new StatusItem("NoAvailableSeed", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Grave = new StatusItem("Grave", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Grave.resolveStringCallback = delegate(string str, object data)
			{
				Grave.StatesInstance statesInstance2 = (Grave.StatesInstance)data;
				string text12 = str.Replace("{DeadDupe}", statesInstance2.master.graveName);
				return text12.Replace("{Epitaph}", LocString.GetStrings(typeof(NAMEGEN.GRAVE.EPITAPHS)).GetRandom<string>());
			};
			this.GraveEmpty = new StatusItem("GraveEmpty", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.CannotCoolFurther = new StatusItem("CannotCoolFurther", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.BuildingDisabled = new StatusItem("BuildingDisabled", "BUILDING", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.Expired = new StatusItem("Expired", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PumpingStation = new StatusItem("PumpingStation", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
			this.PumpingStation.resolveStringCallback = delegate(string str, object data)
			{
				LiquidPumpingStation liquidPumpingStation = (LiquidPumpingStation)data;
				if (liquidPumpingStation != null)
				{
					return liquidPumpingStation.ResolveString(str);
				}
				return str;
			};
			this.EmptyPumpingStation = new StatusItem("EmptyPumpingStation", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true, 2046);
		}

		public MaterialsStatusItem MaterialsUnavailable;

		public MaterialsStatusItem MaterialsUnavailableForRefill;

		public StatusItem AngerDamage;

		public StatusItem DigUnreachable;

		public StatusItem ConstructableDigUnreachable;

		public StatusItem ConstructionUnreachable;

		public StatusItem NewDuplicantsAvailable;

		public StatusItem NeedPlant;

		public StatusItem NeedPower;

		public StatusItem NeedLiquidIn;

		public StatusItem NeedGasIn;

		public StatusItem NeedLiquidOut;

		public StatusItem NeedGasOut;

		public StatusItem InvalidBuildingLocation;

		public StatusItem PendingDeconstruction;

		public StatusItem PendingSwitchToggle;

		public StatusItem GasVentObstructed;

		public StatusItem LiquidVentObstructed;

		public StatusItem LiquidPipeEmpty;

		public StatusItem LiquidPipeObstructed;

		public StatusItem GasPipeEmpty;

		public StatusItem GasPipeObstructed;

		public StatusItem Broken;

		public StatusItem PendingRepair;

		public StatusItem PendingUpgrade;

		public StatusItem PendingWork;

		public StatusItem Flooded;

		public StatusItem PowerButtonOff;

		public StatusItem SwitchStatusActive;

		public StatusItem SwitchStatusInactive;

		public StatusItem ChangeDoorControlState;

		public StatusItem CurrentDoorControlState;

		public StatusItem Entombed;

		public WaitingForMaterialsStatusItem WaitingForMaterials;

		public StatusItem WaitingForRepairMaterials;

		public StatusItem MissingFoundation;

		public StatusItem NeedBoringMachine;

		public StatusItem NeutroniumUnminable;

		public StatusItem NoStorageFilterSet;

		public StatusItem PendingFish;

		public StatusItem NoFishableWaterBelow;

		public StatusItem GasVentOverPressure;

		public StatusItem LiquidVentOverPressure;

		public StatusItem NoWireConnected;

		public StatusItem NoPowerConsumers;

		public StatusItem NoPowerSource;

		public StatusItem PressureOk;

		public StatusItem UnderPressure;

		public StatusItem AssignedTo;

		public StatusItem Unassigned;

		public StatusItem RationBoxContents;

		public StatusItem ConduitBlocked;

		public StatusItem MeltingDown;

		public StatusItem UnderConstruction;

		public StatusItem UnderConstructionNoWorker;

		public StatusItem Normal;

		public StatusItem ManualGeneratorChargingUp;

		public StatusItem ManualGeneratorReleasingEnergy;

		public StatusItem GeneratorOffline;

		public StatusItem Pipe;

		public StatusItem FabricatorEmpty;

		public StatusItem FlushToilet;

		public StatusItem FlushToiletInUse;

		public StatusItem Toilet;

		public StatusItem ToiletNeedsEmptying;

		public StatusItem Unusable;

		public StatusItem NoResearchSelected;

		public StatusItem NoApplicableResearchSelected;

		public StatusItem Researching;

		public StatusItem ValveRequest;

		public StatusItem EmittingLight;

		public StatusItem EmittingElement;

		public StatusItem EmittingOxygenAvg;

		public StatusItem EmittingGasAvg;

		public StatusItem PumpingLiquidOrGas;

		public StatusItem NoLiquidElementToPump;

		public StatusItem NoGasElementToPump;

		public StatusItem PipeFull;

		public StatusItem ElementConsumer;

		public StatusItem ElementEmitterOutput;

		public StatusItem AwaitingWaste;

		public StatusItem AwaitingCompostFlip;

		public StatusItem JoulesAvailable;

		public StatusItem Wattage;

		public StatusItem Wattson;

		public StatusItem WireConnected;

		public StatusItem WireNominal;

		public StatusItem WireDisconnected;

		public StatusItem Cooling;

		public StatusItem CoolingStalledHotEnv;

		public StatusItem CoolingStalledColdGas;

		public StatusItem Working;

		public StatusItem CannotCoolFurther;

		public StatusItem NeedsRegion;

		public StatusItem NeedsValidRegion;

		public StatusItem NeedSeed;

		public StatusItem AwaitingSeedDelivery;

		public StatusItem NoAvailableSeed;

		public StatusItem Grave;

		public StatusItem GraveEmpty;

		public StatusItem NoFilterElementSelected;

		public StatusItem BuildingDisabled;

		public StatusItem Overheated;

		public StatusItem Expired;

		public StatusItem PumpingStation;

		public StatusItem EmptyPumpingStation;

		public StatusItem GeneShuffleCompleted;

		public StatusItem DirectionControl;
	}
}
