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
			this.AssignedTo = new StatusItem("AssignedTo", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
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
			this.Broken = new StatusItem("Broken", "BUILDING", "status_item_broken", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.ChangeDoorControlState = new StatusItem("ChangeDoorControlState", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.ChangeDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door = (Door)data;
				return str.Replace("{ControlState}", door.RequestedState.ToString());
			};
			this.CurrentDoorControlState = new StatusItem("CurrentDoorControlState", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.CurrentDoorControlState.resolveStringCallback = delegate(string str, object data)
			{
				Door door2 = (Door)data;
				string text = Strings.Get("STRINGS.BUILDING.STATUSITEMS.CURRENTDOORCONTROLSTATE." + door2.CurrentState.ToString().ToUpper());
				return str.Replace("{ControlState}", text);
			};
			this.ConduitBlocked = new StatusItem("ConduitBlocked", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.ConstructionUnreachable = new StatusItem("ConstructionUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.DigUnreachable = new StatusItem("DigUnreachable", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Entombed = new StatusItem("Entombed", "BUILDING", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Entombed.AddNotification(null, null, null, 0f);
			this.Flooded = new StatusItem("Flooded", "BUILDING", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Flooded.AddNotification(null, null, null, 0f);
			this.GasVentObstructed = new StatusItem("GasVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None);
			this.GasVentOverPressure = new StatusItem("GasVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None);
			this.InvalidBuildingLocation = new StatusItem("InvalidBuildingLocation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.LiquidVentObstructed = new StatusItem("LiquidVentObstructed", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None);
			this.LiquidVentOverPressure = new StatusItem("LiquidVentOverPressure", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None);
			this.MaterialsUnavailable = new MaterialsUnavailableStatusItem("MaterialsUnavailable", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, true, SimViewMode.None, SimViewMode.None);
			this.MaterialsUnavailable.AddNotification(null, null, null, 0f);
			this.MaterialsUnavailable.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList = (IFetchList)data;
				string text2 = string.Empty;
				Dictionary<Tag, float> remainingMinimum = fetchList.GetRemainingMinimum();
				if (remainingMinimum.Count > 0)
				{
					bool flag = true;
					foreach (KeyValuePair<Tag, float> keyValuePair in remainingMinimum)
					{
						if (keyValuePair.Value != 0f)
						{
							if (!flag)
							{
								text2 += "\n";
							}
							text2 += string.Format("{0} {1} {2}", "• ", keyValuePair.Key.ProperName(), GameUtil.GetFormattedMass(keyValuePair.Value, GameUtil.TimeSlice.None, true, "F1"));
							flag = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text2);
				return str;
			};
			this.MaterialsUnavailableForRefill = new MaterialsUnavailableForRefillStatusItem("MaterialsUnavailableForRefill", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, true, SimViewMode.None, SimViewMode.None);
			this.MaterialsUnavailableForRefill.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList2 = (IFetchList)data;
				string text3 = string.Empty;
				Dictionary<Tag, float> remaining = fetchList2.GetRemaining();
				if (remaining.Count > 0)
				{
					bool flag2 = true;
					foreach (KeyValuePair<Tag, float> keyValuePair2 in remaining)
					{
						if (keyValuePair2.Value != 0f)
						{
							if (!flag2)
							{
								text3 += "\n";
							}
							text3 += string.Format("{0} {1}", "• ", keyValuePair2.Key.ProperName());
							flag2 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text3);
				return str;
			};
			this.WaitingForMaterials = new WaitingForMaterialsStatusItem("WaitingForMaterials", "BUILDING", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None);
			this.WaitingForMaterials.resolveStringCallback = delegate(string str, object data)
			{
				IFetchList fetchList3 = (IFetchList)data;
				string text4 = string.Empty;
				Dictionary<Tag, float> remaining2 = fetchList3.GetRemaining();
				if (remaining2.Count > 0)
				{
					bool flag3 = true;
					foreach (KeyValuePair<Tag, float> keyValuePair3 in remaining2)
					{
						if (keyValuePair3.Value != 0f)
						{
							if (!flag3)
							{
								text4 += "\n";
							}
							text4 += string.Format("{0} {1} {2}", "• ", keyValuePair3.Key.ProperName(), GameUtil.GetFormattedMass(keyValuePair3.Value, GameUtil.TimeSlice.None, true, "F1"));
							flag3 = false;
						}
					}
				}
				str = str.Replace("{ItemsRemaining}", text4);
				return str;
			};
			this.MeltingDown = new StatusItem("MeltingDown", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.MissingFoundation = new StatusItem("MissingFoundation", "BUILDING", "status_item_missing_foundation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.NeedBoringMachine = new StatusItem("NeedBoringMachine", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.NeutroniumUnminable = new StatusItem("NeutroniumUnminable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.NeedGasIn = new StatusItem("NeedGasIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None);
			this.NeedGasOut = new StatusItem("NeedGasOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None);
			this.NeedLiquidIn = new StatusItem("NeedLiquidIn", "BUILDING", "status_item_need_supply_in", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None);
			this.NeedLiquidOut = new StatusItem("NeedLiquidOut", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None);
			this.LiquidPipeEmpty = new StatusItem("LiquidPipeEmpty", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None);
			this.LiquidPipeObstructed = new StatusItem("LiquidPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.LiquidVentMap, SimViewMode.None);
			this.GasPipeEmpty = new StatusItem("GasPipeEmpty", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None);
			this.GasPipeObstructed = new StatusItem("GasPipeObstructed", "BUILDING", "status_item_wrong_resource_in_pipe", StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.GasVentMap, SimViewMode.None);
			this.NeedPlant = new StatusItem("NeedPlant", "BUILDING", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.NeedPower = new StatusItem("NeedPower", "BUILDING", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None);
			this.NewDuplicantsAvailable = new StatusItem("NewDuplicantsAvailable", "BUILDING", "status_item_new_duplicants_available", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.NewDuplicantsAvailable.AddNotification(null, null, null, 0f);
			this.NewDuplicantsAvailable.notificationClickCallback = delegate(object data)
			{
				Telepad telepad = (Telepad)data;
				ImmigrantScreen.InitializeImmigrantScreen(telepad);
			};
			this.NoStorageFilterSet = new StatusItem("NoStorageFilterSet", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.Regions, SimViewMode.None);
			this.NoFishableWaterBelow = new StatusItem("NoFishableWaterBelow", "BUILDING", "status_item_no_fishable_water_below", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.NoPowerConsumers = new StatusItem("NoPowerConsumers", "BUILDING", "status_item_no_power_consumers", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None);
			this.NoPowerSource = new StatusItem("NoPowerSource", "BUILDING", "status_item_no_power_source", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None);
			this.NoWireConnected = new StatusItem("NoWireConnected", "BUILDING", "status_item_no_wire_connected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None);
			this.PendingDeconstruction = new StatusItem("PendingDeconstruction", "BUILDING", "status_item_pending_deconstruction", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.PendingDeconstruction.conditionalOverlayCallback = delegate(SimViewMode mode, object data)
			{
				BuildingComplete component = ((Transform)data).GetComponent<BuildingComplete>();
				if (component.Def.isGraphTile)
				{
					Vent component2 = component.GetComponent<Vent>();
					if (!(component2 != null))
					{
						return mode == SimViewMode.PowerMap;
					}
					if ((mode == SimViewMode.GasVentMap || mode == SimViewMode.OxygenMap) && component2.transferType == Vent.Transfer.Gas)
					{
						return true;
					}
					if (mode == SimViewMode.LiquidVentMap && component2.transferType == Vent.Transfer.Liquid)
					{
						return true;
					}
				}
				return false;
			};
			this.PendingFish = new StatusItem("PendingFish", "BUILDING", "status_item_pending_fish", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.PendingRepair = new StatusItem("PendingRepair", "BUILDING", "status_item_pending_repair", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.PendingSwitchToggle = new StatusItem("PendingSwitchToggle", "BUILDING", "status_item_pending_switch_toggle", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.PendingUpgrade = new StatusItem("PendingUpgrade", "BUILDING", "status_item_pending_upgrade", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.PendingWork = new StatusItem("PendingWork", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.PowerButtonOff = new StatusItem("PowerButtonOff", "BUILDING", "status_item_power_button_off", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.PressureOk = new StatusItem("PressureOk", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.OxygenMap, SimViewMode.None);
			this.StorageLocker = new StatusItem("StorageLocker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None);
			this.StorageLocker.resolveStringCallback = delegate(string str, object data)
			{
				KMonoBehaviour kmonoBehaviour = (KMonoBehaviour)data;
				Storage component3 = kmonoBehaviour.GetComponent<Storage>();
				string text5 = Util.FormatWholeNumber(component3.MassStored());
				string text6 = Util.FormatWholeNumber(component3.capacityKg);
				str = str.Replace("{Stored}", text5);
				str = str.Replace("{Capacity}", text6);
				return str;
			};
			this.Unassigned = new StatusItem("Unassigned", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.Rooms, SimViewMode.None);
			this.UnderConstruction = new StatusItem("UnderConstruction", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.UnderConstructionNoWorker = new StatusItem("UnderConstructionNoWorker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Normal = new StatusItem("Normal", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.ManualGeneratorChargingUp = new StatusItem("ManualGeneratorChargingUp", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None);
			this.ManualGeneratorReleasingEnergy = new StatusItem("ManualGeneratorReleasingEnergy", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None);
			this.GeneratorOffline = new StatusItem("GeneratorOffline", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None);
			this.Pipe = new StatusItem("Pipe", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.LiquidVentMap, SimViewMode.GasVentMap);
			this.Pipe.resolveStringCallback = delegate(string str, object data)
			{
				Conduit conduit = (Conduit)data;
				Vent component4 = conduit.GetComponent<Vent>();
				int num = Grid.PosToCell(conduit);
				ConduitFlow conduitFlowManager = component4.GetConduitFlowManager();
				ConduitFlow.ConduitContents contents = conduitFlowManager.GetContents(num);
				string text7 = "Empty";
				Element element = ElementLoader.FindElementByHash(contents.element);
				if (element != null && !element.IsVacuum)
				{
					text7 = element.name;
				}
				if (contents.mass != 0f)
				{
					text7 = string.Format(Strings.Get("STRINGS.BUILDING.STATUSITEMS.PIPECONTENTS.NAME"), GameUtil.GetFormattedMass(contents.mass, GameUtil.TimeSlice.None, true, "F1"), text7, GameUtil.GetFormattedTemperature(contents.temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
				}
				str = str.Replace("{Contents}", text7);
				return str;
			};
			this.FabricatorEmpty = new StatusItem("FabricatorEmpty", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.Regions, SimViewMode.None);
			this.Toilet = new StatusItem("Toilet", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Toilet.resolveStringCallback = delegate(string str, object data)
			{
				Toilet.StatesInstance statesInstance = (Toilet.StatesInstance)data;
				if (statesInstance != null)
				{
					str = str.Replace("{FlushesRemaining}", statesInstance.GetFlushesRemaining().ToString());
				}
				return str;
			};
			this.ToiletNeedsEmptying = new StatusItem("ToiletNeedsEmptying", "BUILDING", "status_item_toilet_needs_emptying", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Unusable = new StatusItem("Unusable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.NoResearchSelected = new StatusItem("NoResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.NoResearchSelected.AddNotification(null, null, null, 0f);
			this.NoApplicableResearchSelected = new StatusItem("NoApplicableResearchSelected", "BUILDING", "status_item_no_research_selected", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.NoApplicableResearchSelected.AddNotification(null, null, null, 0f);
			this.Valve = new StatusItem("Valve", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Valve.resolveStringCallback = delegate(string str, object data)
			{
				Valve valve = (Valve)data;
				str = str.Replace("{MaxFlow}", GameUtil.GetFormattedMass(valve.MaxFlow, GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.ValveRequest = new StatusItem("ValveRequest", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.ValveRequest.resolveStringCallback = delegate(string str, object data)
			{
				Valve valve2 = (Valve)data;
				str = str.Replace("{QueuedMaxFlow}", GameUtil.GetFormattedMass(valve2.QueuedMaxFlow, GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.EmittingLight = new StatusItem("EmittingLight", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.EmittingLight.resolveStringCallback = delegate(string str, object data)
			{
				string text8 = GameInputMapping.FindEntry(global::Action.Overlay5).mKeyCode.ToString();
				str = str.Replace("{LightGridOverlay}", text8);
				return str;
			};
			this.RationBoxContents = new StatusItem("RationBoxContents", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.RationBoxContents.resolveStringCallback = delegate(string str, object data)
			{
				RationBox rationBox = (RationBox)data;
				if (rationBox == null)
				{
					return str;
				}
				Storage component5 = rationBox.GetComponent<Storage>();
				if (component5 == null)
				{
					return str;
				}
				float num2 = 0f;
				foreach (GameObject gameObject in component5)
				{
					Edible component6 = gameObject.GetComponent<Edible>();
					if (component6)
					{
						num2 += component6.rations * 100000f;
					}
				}
				str = str.Replace("{Stored}", GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true));
				return str;
			};
			this.EmittingElement = new StatusItem("EmittingElement", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.EmittingElement.resolveStringCallback = delegate(string str, object data)
			{
				BuildingElementEmitter buildingElementEmitter = (BuildingElementEmitter)data;
				string text9 = ElementLoader.FindElementByHash(buildingElementEmitter.element).tag.ProperName();
				str = str.Replace("{ElementType}", text9);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(buildingElementEmitter.AverageEmitRate, GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.EmittingOxygenAvg = new StatusItem("EmittingOxygenAvg", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.EmittingOxygenAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates = (Sublimates)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates.AvgFlowRate(), GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.EmittingGasAvg = new StatusItem("EmittingGasAvg", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.EmittingGasAvg.resolveStringCallback = delegate(string str, object data)
			{
				Sublimates sublimates2 = (Sublimates)data;
				str = str.Replace("{Element}", ElementLoader.FindElementByHash(sublimates2.info.sublimatedElement).name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(sublimates2.AvgFlowRate(), GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.PumpingLiquidOrGas = new StatusItem("PumpingLiquidOrGas", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.LiquidVentMap, SimViewMode.GasVentMap);
			this.PumpingLiquidOrGas.resolveStringCallback = delegate(string str, object data)
			{
				Accumulator accumulator = (Accumulator)data;
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(accumulator.AvgFlowRate, GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.NoLiquidElementToPump = new StatusItem("NoLiquidElementToPump", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.LiquidVentMap, SimViewMode.None);
			this.NoGasElementToPump = new StatusItem("NoGasElementToPump", "BUILDING", "status_item_no_gas_to_pump", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.GasVentMap, SimViewMode.None);
			this.NoFilterElementSelected = new StatusItem("NoFilterElementSelected", "BUILDING", "status_item_need_supply_out", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.ElementConsumer = new StatusItem("ElementConsumer", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None);
			this.ElementConsumer.resolveStringCallback = delegate(string str, object data)
			{
				ElementConsumer elementConsumer = (ElementConsumer)data;
				string text10 = ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag.ProperName();
				str = str.Replace("{ElementTypes}", text10);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementConsumer.AverageConsumeRate, GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.ElementEmitterOutput = new StatusItem("ElementEmitterOutput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None);
			this.ElementEmitterOutput.resolveStringCallback = delegate(string str, object data)
			{
				ElementEmitter elementEmitter = (ElementEmitter)data;
				str = str.Replace("{ElementTypes}", elementEmitter.outputElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(elementEmitter.outputElement.outputMass / elementEmitter.emissionFrequency, GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.ElementConverterOutput = new StatusItem("ElementConverterOutput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None);
			this.ElementConverterOutput.resolveStringCallback = delegate(string str, object data)
			{
				ElementConverter.OutputElement outputElement = (ElementConverter.OutputElement)data;
				str = str.Replace("{ElementTypes}", outputElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(outputElement.Rate, GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.ElementConverterInput = new StatusItem("ElementConverterInput", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None);
			this.ElementConverterInput.resolveStringCallback = delegate(string str, object data)
			{
				ElementConverter.ConsumedElement consumedElement = (ElementConverter.ConsumedElement)data;
				str = str.Replace("{ElementTypes}", consumedElement.Name);
				str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(consumedElement.Rate, GameUtil.TimeSlice.PerSecond, true, "F1"));
				return str;
			};
			this.AwaitingWaste = new StatusItem("AwaitingWaste", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None);
			this.AwaitingCompostFlip = new StatusItem("AwaitingCompostFlip", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, true, SimViewMode.None, SimViewMode.None);
			this.JoulesAvailable = new StatusItem("JoulesAvailable", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None);
			this.JoulesAvailable.resolveStringCallback = delegate(string str, object data)
			{
				Generator generator = (Generator)data;
				str = str.Replace("{JoulesAvailable}", GameUtil.GetFormattedJoules(generator.JoulesAvailable));
				return str;
			};
			this.Wattage = new StatusItem("Wattage", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None);
			this.Wattage.resolveStringCallback = delegate(string str, object data)
			{
				Generator generator2 = (Generator)data;
				str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(generator2.WattageRating));
				return str;
			};
			this.Wattson = new StatusItem("Wattson", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Wattson.resolveStringCallback = delegate(string str, object data)
			{
				Telepad telepad2 = (Telepad)data;
				if (GameFlowManager.Instance != null && GameFlowManager.Instance.IsGameOver())
				{
					str = Strings.Get("STRINGS.BUILDING.STATUSITEMS.WATTSONGAMEOVER.NAME");
				}
				else
				{
					str = str.Replace("{TimeRemaining}", Math.Max(telepad2.GetTimeRemaining(), 0f).ToString("F2"));
				}
				return str;
			};
			this.FlushToilet = new StatusItem("FlushToilet", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.FlushToiletInUse = new StatusItem("FlushToiletInUse", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.WireNominal = new StatusItem("WireNominal", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None);
			this.WireConnected = new StatusItem("WireConnected", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.PowerMap, SimViewMode.None);
			this.WireDisconnected = new StatusItem("WireDisconnected", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.PowerMap, SimViewMode.None);
			this.Cooling = new StatusItem("Cooling", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Working = new StatusItem("Working", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.NeedsRegion = new StatusItem("NeedsRegion", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None);
			this.NeedsRegion.resolveStringCallback = delegate(string str, object data)
			{
				RequiresRegion requiresRegion = (RequiresRegion)data;
				KPrefabID component7 = requiresRegion.GetComponent<KPrefabID>();
				if (component7 == null)
				{
					Debug.LogError("The object provided does not have a prefabID.");
					return string.Empty;
				}
				string text11 = requiresRegion.RequiredRegions.ToString();
				str = string.Format(str, text11);
				return str;
			};
			this.NeedsValidRegion = new StatusItem("NeedsValidRegion", "BUILDING", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None);
			this.NeedSeed = new StatusItem("NeedSeed", "BUILDING", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.AwaitingSeedDelivery = new StatusItem("AwaitingSeedDelivery", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.NoAvailableSeed = new StatusItem("NoAvailableSeed", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Grave = new StatusItem("Grave", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Grave.resolveStringCallback = delegate(string str, object data)
			{
				Grave.StatesInstance statesInstance2 = (Grave.StatesInstance)data;
				string text12 = str.Replace("{DeadDupe}", statesInstance2.master.graveName);
				return text12.Replace("{Epitaph}", NAMEGEN.GRAVE.EPITAPHS.GetRandom<string>());
			};
			this.GraveEmpty = new StatusItem("GraveEmpty", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.CannotCoolFurther = new StatusItem("CannotCoolFurther", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.BuildingDisabled = new StatusItem("BuildingDisabled", "BUILDING", "status_item_building_disabled", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
		}

		public MaterialsStatusItem MaterialsUnavailable;

		public MaterialsStatusItem MaterialsUnavailableForRefill;

		public StatusItem DigUnreachable;

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

		public StatusItem ChangeDoorControlState;

		public StatusItem CurrentDoorControlState;

		public StatusItem Entombed;

		public WaitingForMaterialsStatusItem WaitingForMaterials;

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

		public StatusItem AssignedTo;

		public StatusItem Unassigned;

		public StatusItem StorageLocker;

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

		public StatusItem Valve;

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

		public StatusItem ElementConverterInput;

		public StatusItem ElementConverterOutput;

		public StatusItem AwaitingWaste;

		public StatusItem AwaitingCompostFlip;

		public StatusItem JoulesAvailable;

		public StatusItem Wattage;

		public StatusItem Wattson;

		public StatusItem WireConnected;

		public StatusItem WireNominal;

		public StatusItem WireDisconnected;

		public StatusItem Cooling;

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
	}
}
