using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

namespace Database
{
	public class DuplicantStatusItems : StatusItems
	{
		public DuplicantStatusItems(ResourceSet parent)
			: base("DuplicantStatusItems", parent)
		{
			this.CreateStatusItems();
		}

		private StatusItem CreateStatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, bool showWorldIcon = true, int status_overlays = 2)
		{
			return base.Add(new StatusItem(id, prefix, icon, icon_type, notification_type, allow_multiples, render_overlay, showWorldIcon, status_overlays, null));
		}

		private StatusItem CreateStatusItem(string id, string name, string tooltip, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, HashedString render_overlay, int status_overlays = 2)
		{
			return base.Add(new StatusItem(id, name, tooltip, icon, icon_type, notification_type, allow_multiples, render_overlay, status_overlays, true, null));
		}

		private void CreateStatusItems()
		{
			Func<string, object, string> func = delegate(string str, object data)
			{
				Workable workable = (Workable)data;
				if (workable != null && workable.GetComponent<KSelectable>() != null)
				{
					str = str.Replace("{Target}", workable.GetComponent<KSelectable>().GetName());
				}
				return str;
			};
			Func<string, object, string> func2 = delegate(string str, object data)
			{
				Workable workable2 = (Workable)data;
				if (workable2 != null)
				{
					str = str.Replace("{Target}", workable2.GetComponent<KSelectable>().GetName());
					ComplexFabricatorWorkable complexFabricatorWorkable = workable2 as ComplexFabricatorWorkable;
					if (complexFabricatorWorkable != null)
					{
						ComplexRecipe currentWorkingOrder = complexFabricatorWorkable.CurrentWorkingOrder;
						if (currentWorkingOrder != null)
						{
							str = str.Replace("{Item}", currentWorkingOrder.FirstResult.ProperName());
						}
					}
				}
				return str;
			};
			this.BedUnreachable = this.CreateStatusItem("BedUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.BedUnreachable.AddNotification(null, null, null);
			this.DailyRationLimitReached = this.CreateStatusItem("DailyRationLimitReached", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.DailyRationLimitReached.AddNotification(null, null, null);
			this.HoldingBreath = this.CreateStatusItem("HoldingBreath", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Hungry = this.CreateStatusItem("Hungry", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Unhappy = this.CreateStatusItem("Unhappy", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Unhappy.AddNotification(null, null, null);
			this.NervousBreakdown = this.CreateStatusItem("NervousBreakdown", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 2);
			this.NervousBreakdown.AddNotification(null, null, null);
			this.NoRationsAvailable = this.CreateStatusItem("NoRationsAvailable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 2);
			this.PendingPacification = this.CreateStatusItem("PendingPacification", "DUPLICANTS", "status_item_pending_pacification", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.QuarantineAreaUnassigned = this.CreateStatusItem("QuarantineAreaUnassigned", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.QuarantineAreaUnassigned.AddNotification(null, null, null);
			this.QuarantineAreaUnreachable = this.CreateStatusItem("QuarantineAreaUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.QuarantineAreaUnreachable.AddNotification(null, null, null);
			this.Quarantined = this.CreateStatusItem("Quarantined", "DUPLICANTS", "status_item_quarantined", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.RationsUnreachable = this.CreateStatusItem("RationsUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.RationsUnreachable.AddNotification(null, null, null);
			this.RationsNotPermitted = this.CreateStatusItem("RationsNotPermitted", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.RationsNotPermitted.AddNotification(null, null, null);
			this.Rotten = this.CreateStatusItem("Rotten", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Starving = this.CreateStatusItem("Starving", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 2);
			this.Starving.AddNotification(null, null, null);
			this.Suffocating = this.CreateStatusItem("Suffocating", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 2);
			this.Suffocating.AddNotification(null, null, null);
			this.Tired = this.CreateStatusItem("Tired", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Idle = this.CreateStatusItem("Idle", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Idle.AddNotification(null, null, null);
			this.Pacified = this.CreateStatusItem("Pacified", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Dead = this.CreateStatusItem("Dead", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Dead.resolveStringCallback = delegate(string str, object data)
			{
				Death death = (Death)data;
				return str.Replace("{Death}", death.Name);
			};
			this.MoveToSuitNotRequired = this.CreateStatusItem("MoveToSuitNotRequired", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.DroppingUnusedInventory = this.CreateStatusItem("DroppingUnusedInventory", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.MovingToSafeArea = this.CreateStatusItem("MovingToSafeArea", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.ToiletUnreachable = this.CreateStatusItem("ToiletUnreachable", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.ToiletUnreachable.AddNotification(null, null, null);
			this.NoUsableToilets = this.CreateStatusItem("NoUsableToilets", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.NoUsableToilets.AddNotification(null, null, null);
			this.NoToilets = this.CreateStatusItem("NoToilets", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.NoToilets.AddNotification(null, null, null);
			this.BreathingO2 = this.CreateStatusItem("BreathingO2", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 130);
			this.BreathingO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather = (OxygenBreather)data;
				float averageRate = Game.Instance.accumulators.GetAverageRate(oxygenBreather.O2Accumulator);
				return str.Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(-averageRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			};
			this.EmittingCO2 = this.CreateStatusItem("EmittingCO2", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 130);
			this.EmittingCO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather2 = (OxygenBreather)data;
				return str.Replace("{EmittingRate}", GameUtil.GetFormattedMass(oxygenBreather2.CO2EmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			};
			this.Vomiting = this.CreateStatusItem("Vomiting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Coughing = this.CreateStatusItem("Coughing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.LowOxygen = this.CreateStatusItem("LowOxygen", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.LowOxygen.AddNotification(null, null, null);
			this.RedAlert = this.CreateStatusItem("RedAlert", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Dreaming = this.CreateStatusItem("Dreaming", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Sleeping = this.CreateStatusItem("Sleeping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Sleeping.resolveTooltipCallback = delegate(string str, object data)
			{
				if (data is SleepChore.StatesInstance)
				{
					string stateChangeNoiseSource = ((SleepChore.StatesInstance)data).stateChangeNoiseSource;
					if (!string.IsNullOrEmpty(stateChangeNoiseSource))
					{
						string text = DUPLICANTS.STATUSITEMS.SLEEPING.TOOLTIP;
						text = text.Replace("{Disturber}", stateChangeNoiseSource);
						str += text;
					}
				}
				return str;
			};
			this.SleepingInterruptedByNoise = this.CreateStatusItem("SleepingInterruptedByNoise", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.SleepingInterruptedByLight = this.CreateStatusItem("SleepingInterruptedByLight", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.SleepingInterruptedByFearOfDark = this.CreateStatusItem("SleepingInterruptedByFearOfDark", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.SleepingInterruptedByMovement = this.CreateStatusItem("SleepingInterruptedByMovement", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Eating = this.CreateStatusItem("Eating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Eating.resolveStringCallback = func;
			this.Digging = this.CreateStatusItem("Digging", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Cleaning = this.CreateStatusItem("Cleaning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Cleaning.resolveStringCallback = func;
			this.PickingUp = this.CreateStatusItem("PickingUp", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.PickingUp.resolveStringCallback = func;
			this.Mopping = this.CreateStatusItem("Mopping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Cooking = this.CreateStatusItem("Cooking", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Cooking.resolveStringCallback = func2;
			this.Mushing = this.CreateStatusItem("Mushing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Mushing.resolveStringCallback = func2;
			this.Researching = this.CreateStatusItem("Researching", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Researching.resolveStringCallback = delegate(string str, object data)
			{
				TechInstance activeResearch = Research.Instance.GetActiveResearch();
				if (activeResearch != null)
				{
					return str.Replace("{Tech}", activeResearch.tech.Name);
				}
				return str;
			};
			this.MissionControlling = this.CreateStatusItem("MissionControlling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Tinkering = this.CreateStatusItem("Tinkering", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Tinkering.resolveStringCallback = delegate(string str, object data)
			{
				Tinkerable tinkerable = (Tinkerable)data;
				if (tinkerable != null)
				{
					return string.Format(str, tinkerable.tinkerMaterialTag.ProperName());
				}
				return str;
			};
			this.Storing = this.CreateStatusItem("Storing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Storing.resolveStringCallback = delegate(string str, object data)
			{
				Workable workable3 = (Workable)data;
				if (workable3 != null && workable3.worker != null)
				{
					KSelectable component = workable3.GetComponent<KSelectable>();
					if (component)
					{
						str = str.Replace("{Target}", component.GetName());
					}
					Pickupable pickupable = workable3.worker.workCompleteData as Pickupable;
					if (workable3.worker != null && pickupable)
					{
						KSelectable component2 = pickupable.GetComponent<KSelectable>();
						if (component2)
						{
							str = str.Replace("{Item}", component2.GetName());
						}
					}
				}
				return str;
			};
			this.Building = this.CreateStatusItem("Building", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Building.resolveStringCallback = func;
			this.Equipping = this.CreateStatusItem("Equipping", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Equipping.resolveStringCallback = func;
			this.WarmingUp = this.CreateStatusItem("WarmingUp", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.WarmingUp.resolveStringCallback = func;
			this.GeneratingPower = this.CreateStatusItem("GeneratingPower", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.GeneratingPower.resolveStringCallback = func;
			this.Harvesting = this.CreateStatusItem("Harvesting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Ranching = this.CreateStatusItem("Ranching", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Harvesting.resolveStringCallback = func;
			this.Uprooting = this.CreateStatusItem("Uprooting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Uprooting.resolveStringCallback = func;
			this.Emptying = this.CreateStatusItem("Emptying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Emptying.resolveStringCallback = func;
			this.Toggling = this.CreateStatusItem("Toggling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Toggling.resolveStringCallback = func;
			this.Deconstructing = this.CreateStatusItem("Deconstructing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Deconstructing.resolveStringCallback = func;
			this.Disinfecting = this.CreateStatusItem("Disinfecting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Disinfecting.resolveStringCallback = func;
			this.Upgrading = this.CreateStatusItem("Upgrading", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Upgrading.resolveStringCallback = func;
			this.Fabricating = this.CreateStatusItem("Fabricating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Fabricating.resolveStringCallback = func2;
			this.Processing = this.CreateStatusItem("Processing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Processing.resolveStringCallback = func2;
			this.Spicing = this.CreateStatusItem("Spicing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Clearing = this.CreateStatusItem("Clearing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Clearing.resolveStringCallback = func;
			this.GeneratingPower = this.CreateStatusItem("GeneratingPower", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.GeneratingPower.resolveStringCallback = func;
			this.Cold = this.CreateStatusItem("Cold", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Cold.resolveTooltipCallback = delegate(string str, object data)
			{
				str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("ColdAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float num = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedHeatEnergyRate(num, GameUtil.HeatEnergyFormatterUnit.Automatic));
				AttributeInstance attributeInstance = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string text2 = "<b>" + attributeInstance.GetFormattedValue() + "</b>";
				for (int num2 = 0; num2 != attributeInstance.Modifiers.Count; num2++)
				{
					AttributeModifier attributeModifier = attributeInstance.Modifiers[num2];
					text2 += "\n";
					text2 = string.Concat(new string[]
					{
						text2,
						"    • ",
						attributeModifier.GetDescription(),
						" <b>",
						attributeModifier.GetFormattedString(),
						"</b>"
					});
				}
				str = str.Replace("{conductivityBarrier}", text2);
				return str;
			};
			this.Hot = this.CreateStatusItem("Hot", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.Hot.resolveTooltipCallback = delegate(string str, object data)
			{
				str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("WarmAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float num3 = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedHeatEnergyRate(num3, GameUtil.HeatEnergyFormatterUnit.Automatic));
				AttributeInstance attributeInstance2 = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string text3 = "<b>" + attributeInstance2.GetFormattedValue() + "</b>";
				for (int num4 = 0; num4 != attributeInstance2.Modifiers.Count; num4++)
				{
					AttributeModifier attributeModifier2 = attributeInstance2.Modifiers[num4];
					text3 += "\n";
					text3 = string.Concat(new string[]
					{
						text3,
						"    • ",
						attributeModifier2.GetDescription(),
						" <b>",
						attributeModifier2.GetFormattedString(),
						"</b>"
					});
				}
				str = str.Replace("{conductivityBarrier}", text3);
				return str;
			};
			this.BodyRegulatingHeating = this.CreateStatusItem("BodyRegulatingHeating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.BodyRegulatingHeating.resolveStringCallback = delegate(string str, object data)
			{
				WarmBlooded.StatesInstance statesInstance = (WarmBlooded.StatesInstance)data;
				return str.Replace("{TempDelta}", GameUtil.GetFormattedTemperature(statesInstance.TemperatureDelta, GameUtil.TimeSlice.PerSecond, GameUtil.TemperatureInterpretation.Relative, true, false));
			};
			this.BodyRegulatingCooling = this.CreateStatusItem("BodyRegulatingCooling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.BodyRegulatingCooling.resolveStringCallback = this.BodyRegulatingHeating.resolveStringCallback;
			this.EntombedChore = this.CreateStatusItem("EntombedChore", "DUPLICANTS", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 2);
			this.EntombedChore.AddNotification(null, null, null);
			this.EarlyMorning = this.CreateStatusItem("EarlyMorning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.NightTime = this.CreateStatusItem("NightTime", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.PoorDecor = this.CreateStatusItem("PoorDecor", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.PoorQualityOfLife = this.CreateStatusItem("PoorQualityOfLife", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.PoorFoodQuality = this.CreateStatusItem("PoorFoodQuality", DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, 2);
			this.GoodFoodQuality = this.CreateStatusItem("GoodFoodQuality", DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.TOOLTIP, "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, 2);
			this.Arting = this.CreateStatusItem("Arting", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Arting.resolveStringCallback = func;
			this.SevereWounds = this.CreateStatusItem("SevereWounds", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.Bad, false, OverlayModes.None.ID, true, 2);
			this.SevereWounds.AddNotification(null, null, null);
			this.Incapacitated = this.CreateStatusItem("Incapacitated", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, false, OverlayModes.None.ID, true, 2);
			this.Incapacitated.AddNotification(null, null, null);
			this.Incapacitated.resolveStringCallback = delegate(string str, object data)
			{
				IncapacitationMonitor.Instance instance = (IncapacitationMonitor.Instance)data;
				float bleedLifeTime = instance.GetBleedLifeTime(instance);
				str = str.Replace("{CauseOfIncapacitation}", instance.GetCauseOfIncapacitation().Name);
				return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedTime(bleedLifeTime, "F0"));
			};
			this.Relocating = this.CreateStatusItem("Relocating", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Relocating.resolveStringCallback = func;
			this.Fighting = this.CreateStatusItem("Fighting", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 2);
			this.Fighting.AddNotification(null, null, null);
			this.Fleeing = this.CreateStatusItem("Fleeing", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 2);
			this.Fleeing.AddNotification(null, null, null);
			this.Stressed = this.CreateStatusItem("Stressed", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Stressed.AddNotification(null, null, null);
			this.LashingOut = this.CreateStatusItem("LashingOut", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 2);
			this.LashingOut.AddNotification(null, null, null);
			this.LowImmunity = this.CreateStatusItem("LowImmunity", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.LowImmunity.AddNotification(null, null, null);
			this.Studying = this.CreateStatusItem("Studying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Socializing = this.CreateStatusItem("Socializing", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 2);
			this.Mingling = this.CreateStatusItem("Mingling", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 2);
			this.ContactWithGerms = this.CreateStatusItem("ContactWithGerms", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.Disease.ID, true, 2);
			this.ContactWithGerms.resolveStringCallback = delegate(string str, object data)
			{
				GermExposureMonitor.ExposureStatusData exposureStatusData = (GermExposureMonitor.ExposureStatusData)data;
				string name = Db.Get().Sicknesses.Get(exposureStatusData.exposure_type.sickness_id).Name;
				str = str.Replace("{Sickness}", name);
				return str;
			};
			this.ContactWithGerms.statusItemClickCallback = delegate(object data)
			{
				GermExposureMonitor.ExposureStatusData exposureStatusData2 = (GermExposureMonitor.ExposureStatusData)data;
				Vector3 lastExposurePosition = exposureStatusData2.owner.GetLastExposurePosition(exposureStatusData2.exposure_type.germ_id);
				CameraController.Instance.CameraGoTo(lastExposurePosition, 2f, true);
				if (OverlayScreen.Instance.mode == OverlayModes.None.ID)
				{
					OverlayScreen.Instance.ToggleOverlay(OverlayModes.Disease.ID, true);
				}
			};
			this.ExposedToGerms = this.CreateStatusItem("ExposedToGerms", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.Disease.ID, true, 2);
			this.ExposedToGerms.resolveStringCallback = delegate(string str, object data)
			{
				GermExposureMonitor.ExposureStatusData exposureStatusData3 = (GermExposureMonitor.ExposureStatusData)data;
				string name2 = Db.Get().Sicknesses.Get(exposureStatusData3.exposure_type.sickness_id).Name;
				AttributeInstance attributeInstance3 = Db.Get().Attributes.GermResistance.Lookup(exposureStatusData3.owner.gameObject);
				string lastDiseaseSource = exposureStatusData3.owner.GetLastDiseaseSource(exposureStatusData3.exposure_type.germ_id);
				GermExposureMonitor.Instance smi = exposureStatusData3.owner.GetSMI<GermExposureMonitor.Instance>();
				float num5 = (float)exposureStatusData3.exposure_type.base_resistance + GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[0];
				float totalValue = attributeInstance3.GetTotalValue();
				float resistanceToExposureType = smi.GetResistanceToExposureType(exposureStatusData3.exposure_type, -1f);
				float contractionChance = GermExposureMonitor.GetContractionChance(resistanceToExposureType);
				float exposureTier = smi.GetExposureTier(exposureStatusData3.exposure_type.germ_id);
				float num6 = GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[(int)exposureTier - 1] - GERM_EXPOSURE.EXPOSURE_TIER_RESISTANCE_BONUSES[0];
				str = str.Replace("{Severity}", DUPLICANTS.STATUSITEMS.EXPOSEDTOGERMS.EXPOSURE_TIERS[(int)exposureTier - 1]);
				str = str.Replace("{Sickness}", name2);
				str = str.Replace("{Source}", lastDiseaseSource);
				str = str.Replace("{Base}", GameUtil.GetFormattedSimple(num5, GameUtil.TimeSlice.None, null));
				str = str.Replace("{Dupe}", GameUtil.GetFormattedSimple(totalValue, GameUtil.TimeSlice.None, null));
				str = str.Replace("{Total}", GameUtil.GetFormattedSimple(resistanceToExposureType, GameUtil.TimeSlice.None, null));
				str = str.Replace("{ExposureLevelBonus}", GameUtil.GetFormattedSimple(num6, GameUtil.TimeSlice.None, null));
				str = str.Replace("{Chance}", GameUtil.GetFormattedPercent(contractionChance * 100f, GameUtil.TimeSlice.None));
				return str;
			};
			this.ExposedToGerms.statusItemClickCallback = delegate(object data)
			{
				GermExposureMonitor.ExposureStatusData exposureStatusData4 = (GermExposureMonitor.ExposureStatusData)data;
				Vector3 lastExposurePosition2 = exposureStatusData4.owner.GetLastExposurePosition(exposureStatusData4.exposure_type.germ_id);
				CameraController.Instance.CameraGoTo(lastExposurePosition2, 2f, true);
				if (OverlayScreen.Instance.mode == OverlayModes.None.ID)
				{
					OverlayScreen.Instance.ToggleOverlay(OverlayModes.Disease.ID, true);
				}
			};
			this.LightWorkEfficiencyBonus = this.CreateStatusItem("LightWorkEfficiencyBonus", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 2);
			this.LightWorkEfficiencyBonus.resolveStringCallback = delegate(string str, object data)
			{
				string text4 = string.Format(DUPLICANTS.STATUSITEMS.LIGHTWORKEFFICIENCYBONUS.NO_BUILDING_WORK_ATTRIBUTE, GameUtil.AddPositiveSign(GameUtil.GetFormattedPercent(15.000001f, GameUtil.TimeSlice.None), true));
				return string.Format(str, text4);
			};
			this.LaboratoryWorkEfficiencyBonus = this.CreateStatusItem("LaboratoryWorkEfficiencyBonus", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 2);
			this.LaboratoryWorkEfficiencyBonus.resolveStringCallback = delegate(string str, object data)
			{
				string text5 = string.Format(DUPLICANTS.STATUSITEMS.LABORATORYWORKEFFICIENCYBONUS.NO_BUILDING_WORK_ATTRIBUTE, GameUtil.AddPositiveSign(GameUtil.GetFormattedPercent(10f, GameUtil.TimeSlice.None), true));
				return string.Format(str, text5);
			};
			this.BeingProductive = this.CreateStatusItem("BeingProductive", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.BalloonArtistPlanning = this.CreateStatusItem("BalloonArtistPlanning", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.BalloonArtistHandingOut = this.CreateStatusItem("BalloonArtistHandingOut", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.Partying = this.CreateStatusItem("Partying", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 2);
			this.GasLiquidIrritation = this.CreateStatusItem("GasLiquidIrritated", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 2);
			this.GasLiquidIrritation.resolveStringCallback = (string str, object data) => ((GasLiquidExposureMonitor.Instance)data).IsMajorIrritation() ? DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.NAME_MAJOR : DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.NAME_MINOR;
			this.GasLiquidIrritation.resolveTooltipCallback = delegate(string str, object data)
			{
				GasLiquidExposureMonitor.Instance instance2 = (GasLiquidExposureMonitor.Instance)data;
				string text6 = DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP;
				string text7 = "";
				Effect appliedEffect = instance2.sm.GetAppliedEffect(instance2);
				if (appliedEffect != null)
				{
					text7 = Effect.CreateTooltip(appliedEffect, false, "\n    • ", true);
				}
				string text8 = DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_EXPOSED.Replace("{element}", instance2.CurrentlyExposedToElement().name);
				float currentExposure = instance2.sm.GetCurrentExposure(instance2);
				if (currentExposure < 0f)
				{
					text8 = text8.Replace("{rate}", DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_RATE_DECREASE);
				}
				else if (currentExposure > 0f)
				{
					text8 = text8.Replace("{rate}", DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_RATE_INCREASE);
				}
				else
				{
					text8 = text8.Replace("{rate}", DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_RATE_STAYS);
				}
				float num7 = (instance2.exposure - instance2.minorIrritationThreshold) / Math.Abs(instance2.exposureRate);
				string text9 = DUPLICANTS.STATUSITEMS.GASLIQUIDEXPOSURE.TOOLTIP_EXPOSURE_LEVEL.Replace("{time}", GameUtil.GetFormattedTime(num7, "F0"));
				return string.Concat(new string[] { text6, "\n\n", text7, "\n\n", text8, "\n\n", text9 });
			};
			this.ExpellingRads = this.CreateStatusItem("ExpellingRads", "DUPLICANTS", "", StatusItem.IconType.Exclamation, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
			this.AnalyzingGenes = this.CreateStatusItem("AnalyzingGenes", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 2);
			this.AnalyzingArtifact = this.CreateStatusItem("AnalyzingArtifact", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 2);
			this.MegaBrainTank_Pajamas_Wearing = this.CreateStatusItem("MegaBrainTank_Pajamas_Wearing", DUPLICANTS.STATUSITEMS.WEARING_PAJAMAS.NAME, DUPLICANTS.STATUSITEMS.WEARING_PAJAMAS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, 2);
			this.MegaBrainTank_Pajamas_Wearing.resolveTooltipCallback_shouldStillCallIfDataIsNull = true;
			this.MegaBrainTank_Pajamas_Wearing.resolveTooltipCallback = delegate(string str, object data)
			{
				string text10 = DUPLICANTS.STATUSITEMS.WEARING_PAJAMAS.TOOLTIP;
				Effect effect = Db.Get().effects.Get("SleepClinic");
				string text11;
				if (effect != null)
				{
					text11 = Effect.CreateTooltip(effect, false, "\n    • ", true);
				}
				else
				{
					text11 = "";
				}
				return text10 + "\n\n" + text11;
			};
			this.MegaBrainTank_Pajamas_Sleeping = this.CreateStatusItem("MegaBrainTank_Pajamas_Sleeping", DUPLICANTS.STATUSITEMS.DREAMING.NAME, DUPLICANTS.STATUSITEMS.DREAMING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, 2);
			this.MegaBrainTank_Pajamas_Sleeping.resolveTooltipCallback = delegate(string str, object data)
			{
				ClinicDreamable clinicDreamable = (ClinicDreamable)data;
				return str.Replace("{time}", GameUtil.GetFormattedTime(clinicDreamable.WorkTimeRemaining, "F0"));
			};
			this.FossilHunt_WorkerExcavating = this.CreateStatusItem("FossilHunt_WorkerExcavating", DUPLICANTS.STATUSITEMS.FOSSILHUNT.WORKEREXCAVATING.NAME, DUPLICANTS.STATUSITEMS.FOSSILHUNT.WORKEREXCAVATING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, 2);
			this.JoyResponse_HasBalloon = this.CreateStatusItem("JoyResponse_HasBalloon", DUPLICANTS.MODIFIERS.HASBALLOON.NAME, DUPLICANTS.MODIFIERS.HASBALLOON.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, 2);
			this.JoyResponse_HasBalloon.resolveTooltipCallback = delegate(string str, object data)
			{
				EquippableBalloon.StatesInstance statesInstance2 = (EquippableBalloon.StatesInstance)data;
				return str + "\n\n" + DUPLICANTS.MODIFIERS.TIME_REMAINING.Replace("{0}", GameUtil.GetFormattedCycles(statesInstance2.transitionTime - GameClock.Instance.GetTime(), "F1", false));
			};
			this.JoyResponse_HeardJoySinger = this.CreateStatusItem("JoyResponse_HeardJoySinger", DUPLICANTS.MODIFIERS.HEARDJOYSINGER.NAME, DUPLICANTS.MODIFIERS.HEARDJOYSINGER.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, 2);
			this.JoyResponse_HeardJoySinger.resolveTooltipCallback = delegate(string str, object data)
			{
				InspirationEffectMonitor.Instance instance3 = (InspirationEffectMonitor.Instance)data;
				return str + "\n\n" + DUPLICANTS.MODIFIERS.TIME_REMAINING.Replace("{0}", GameUtil.GetFormattedCycles(instance3.sm.inspirationTimeRemaining.Get(instance3), "F1", false));
			};
			this.Meteorphile = this.CreateStatusItem("Meteorphile", "DUPLICANTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 2);
		}

		public StatusItem Idle;

		public StatusItem Pacified;

		public StatusItem PendingPacification;

		public StatusItem Dead;

		public StatusItem MoveToSuitNotRequired;

		public StatusItem DroppingUnusedInventory;

		public StatusItem MovingToSafeArea;

		public StatusItem BedUnreachable;

		public StatusItem Hungry;

		public StatusItem Starving;

		public StatusItem Rotten;

		public StatusItem Quarantined;

		public StatusItem NoRationsAvailable;

		public StatusItem RationsUnreachable;

		public StatusItem RationsNotPermitted;

		public StatusItem DailyRationLimitReached;

		public StatusItem Scalding;

		public StatusItem Hot;

		public StatusItem Cold;

		public StatusItem QuarantineAreaUnassigned;

		public StatusItem QuarantineAreaUnreachable;

		public StatusItem Tired;

		public StatusItem NervousBreakdown;

		public StatusItem Unhappy;

		public StatusItem Suffocating;

		public StatusItem HoldingBreath;

		public StatusItem ToiletUnreachable;

		public StatusItem NoUsableToilets;

		public StatusItem NoToilets;

		public StatusItem Vomiting;

		public StatusItem Coughing;

		public StatusItem BreathingO2;

		public StatusItem EmittingCO2;

		public StatusItem LowOxygen;

		public StatusItem RedAlert;

		public StatusItem Digging;

		public StatusItem Eating;

		public StatusItem Dreaming;

		public StatusItem Sleeping;

		public StatusItem SleepingInterruptedByLight;

		public StatusItem SleepingInterruptedByNoise;

		public StatusItem SleepingInterruptedByFearOfDark;

		public StatusItem SleepingInterruptedByMovement;

		public StatusItem SleepingPeacefully;

		public StatusItem SleepingBadly;

		public StatusItem SleepingTerribly;

		public StatusItem Cleaning;

		public StatusItem PickingUp;

		public StatusItem Mopping;

		public StatusItem Cooking;

		public StatusItem Arting;

		public StatusItem Mushing;

		public StatusItem Researching;

		public StatusItem MissionControlling;

		public StatusItem Tinkering;

		public StatusItem Storing;

		public StatusItem Building;

		public StatusItem Equipping;

		public StatusItem WarmingUp;

		public StatusItem GeneratingPower;

		public StatusItem Ranching;

		public StatusItem Harvesting;

		public StatusItem Uprooting;

		public StatusItem Emptying;

		public StatusItem Toggling;

		public StatusItem Deconstructing;

		public StatusItem Disinfecting;

		public StatusItem Relocating;

		public StatusItem Upgrading;

		public StatusItem Fabricating;

		public StatusItem Processing;

		public StatusItem Spicing;

		public StatusItem Clearing;

		public StatusItem BodyRegulatingHeating;

		public StatusItem BodyRegulatingCooling;

		public StatusItem EntombedChore;

		public StatusItem EarlyMorning;

		public StatusItem NightTime;

		public StatusItem PoorDecor;

		public StatusItem PoorQualityOfLife;

		public StatusItem PoorFoodQuality;

		public StatusItem GoodFoodQuality;

		public StatusItem SevereWounds;

		public StatusItem Incapacitated;

		public StatusItem Fighting;

		public StatusItem Fleeing;

		public StatusItem Stressed;

		public StatusItem LashingOut;

		public StatusItem LowImmunity;

		public StatusItem Studying;

		public StatusItem Socializing;

		public StatusItem Mingling;

		public StatusItem ContactWithGerms;

		public StatusItem ExposedToGerms;

		public StatusItem LightWorkEfficiencyBonus;

		public StatusItem LaboratoryWorkEfficiencyBonus;

		public StatusItem BeingProductive;

		public StatusItem BalloonArtistPlanning;

		public StatusItem BalloonArtistHandingOut;

		public StatusItem Partying;

		public StatusItem GasLiquidIrritation;

		public StatusItem ExpellingRads;

		public StatusItem AnalyzingGenes;

		public StatusItem AnalyzingArtifact;

		public StatusItem MegaBrainTank_Pajamas_Wearing;

		public StatusItem MegaBrainTank_Pajamas_Sleeping;

		public StatusItem JoyResponse_HasBalloon;

		public StatusItem JoyResponse_HeardJoySinger;

		public StatusItem Meteorphile;

		public StatusItem FossilHunt_WorkerExcavating;

		private const int NONE_OVERLAY = 0;
	}
}
