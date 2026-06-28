using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;

namespace Database
{
	public class DuplicantStatusItems : StatusItems
	{
		public DuplicantStatusItems(ResourceSet parent)
			: base("DuplicantStatusItems", parent)
		{
			this.CreateStatusItems();
		}

		private void CreateStatusItems()
		{
			Func<string, object, string> func = delegate(string str, object data)
			{
				Workable workable = (Workable)data;
				if (workable != null)
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
					IHasBuildQueue hasBuildQueue = workable2 as IHasBuildQueue;
					if (hasBuildQueue != null)
					{
						using (List<IBuildQueueOrder>.Enumerator enumerator = hasBuildQueue.Orders.GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								IBuildQueueOrder buildQueueOrder = enumerator.Current;
								str = str.Replace("{Item}", buildQueueOrder.Result.ProperName());
							}
						}
					}
				}
				return str;
			};
			this.BedUnreachable = new StatusItem("BedUnreachable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.BedUnreachable.AddNotification(null, null, null, 0f);
			this.DailyRationLimitReached = new StatusItem("DailyRationLimitReached", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.DailyRationLimitReached.AddNotification(null, null, null, 0f);
			this.HoldingBreath = new StatusItem("HoldingBreath", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Hungry = new StatusItem("Hungry", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Unhappy = new StatusItem("Unhappy", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Unhappy.AddNotification(null, null, null, 0f);
			this.NervousBreakdown = new StatusItem("NervousBreakdown", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 30718);
			this.NervousBreakdown.AddNotification(null, null, null, 0f);
			this.NoRationsAvailable = new StatusItem("NoRationsAvailable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 30718);
			this.PendingPacification = new StatusItem("PendingPacification", "DUPLICANTS", "status_item_pending_pacification", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.QuarantineAreaUnassigned = new StatusItem("QuarantineAreaUnassigned", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.QuarantineAreaUnassigned.AddNotification(null, null, null, 0f);
			this.QuarantineAreaUnreachable = new StatusItem("QuarantineAreaUnreachable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.QuarantineAreaUnreachable.AddNotification(null, null, null, 0f);
			this.Quarantined = new StatusItem("Quarantined", "DUPLICANTS", "status_item_quarantined", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.RationsUnreachable = new StatusItem("RationsUnreachable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.RationsUnreachable.AddNotification(null, null, null, 0f);
			this.Rotten = new StatusItem("Rotten", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Starving = new StatusItem("Starving", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 30718);
			this.Starving.AddNotification(null, null, null, 0f);
			this.Suffocating = new StatusItem("Suffocating", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, false, SimViewMode.None, true, 30718);
			this.Suffocating.AddNotification(null, null, null, 0f);
			this.Tired = new StatusItem("Tired", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Idle = new StatusItem("Idle", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Idle.AddNotification(null, null, null, 0f);
			this.Pacified = new StatusItem("Pacified", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Dead = new StatusItem("Dead", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Dead.resolveStringCallback = delegate(string str, object data)
			{
				Death death = (Death)data;
				return str.Replace("{Death}", death.Name);
			};
			this.MoveToSuitNotRequired = new StatusItem("MoveToSuitNotRequired", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.DroppingUnusedInventory = new StatusItem("DroppingUnusedInventory", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.MovingToSafeArea = new StatusItem("MovingToSafeArea", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.ToiletUnreachable = new StatusItem("ToiletUnreachable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.ToiletUnreachable.AddNotification(null, null, null, 0f);
			this.NoUsableToilets = new StatusItem("NoUsableToilets", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.NoUsableToilets.AddNotification(null, null, null, 0f);
			this.NoToilets = new StatusItem("NoToilets", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.NoToilets.AddNotification(null, null, null, 0f);
			this.BreathingO2 = new StatusItem("BreathingO2", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 130);
			this.BreathingO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather = (OxygenBreather)data;
				return str.Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(-oxygenBreather.O2ConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			};
			this.EmittingCO2 = new StatusItem("EmittingCO2", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 130);
			this.EmittingCO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather2 = (OxygenBreather)data;
				return str.Replace("{EmittingRate}", GameUtil.GetFormattedMass(oxygenBreather2.CO2EmitRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			};
			this.Vomiting = new StatusItem("Vomiting", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Coughing = new StatusItem("Coughing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.LowOxygen = new StatusItem("LowOxygen", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.LowOxygen.AddNotification(null, null, null, 0f);
			this.RedAlert = new StatusItem("RedAlert", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Sleeping = new StatusItem("Sleeping", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Sleeping.resolveTooltipCallback = delegate(string str, object data)
			{
				if (data is SleepChore.StatesInstance)
				{
					SleepChore.StatesInstance statesInstance = (SleepChore.StatesInstance)data;
					string stateChangeNoiseSource = statesInstance.stateChangeNoiseSource;
					if (!string.IsNullOrEmpty(stateChangeNoiseSource))
					{
						string text = DUPLICANTS.STATUSITEMS.SLEEPING.TOOLTIP;
						text = text.Replace("{Disturber}", stateChangeNoiseSource);
						str += text;
					}
				}
				return str;
			};
			this.SleepingInterrupted = new StatusItem("SleepingInterrupted", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Eating = new StatusItem("Eating", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Eating.resolveStringCallback = func;
			this.Digging = new StatusItem("Digging", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Cleaning = new StatusItem("Cleaning", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Cleaning.resolveStringCallback = func;
			this.PickingUp = new StatusItem("PickingUp", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.PickingUp.resolveStringCallback = func;
			this.Mopping = new StatusItem("Mopping", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Cooking = new StatusItem("Cooking", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Cooking.resolveStringCallback = func2;
			this.Mushing = new StatusItem("Mushing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Mushing.resolveStringCallback = func2;
			this.Researching = new StatusItem("Researching", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Researching.resolveStringCallback = delegate(string str, object data)
			{
				TechInstance activeResearch = Research.Instance.GetActiveResearch();
				if (activeResearch != null)
				{
					return str.Replace("{Tech}", activeResearch.tech.Name);
				}
				return str;
			};
			this.Storing = new StatusItem("Storing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Storing.resolveStringCallback = delegate(string str, object data)
			{
				Workable workable3 = (Workable)data;
				if (workable3 != null)
				{
					KSelectable component = workable3.GetComponent<KSelectable>();
					if (component)
					{
						str = str.Replace("{Target}", component.GetName());
					}
					if (workable3.worker != null)
					{
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
				}
				return str;
			};
			this.Building = new StatusItem("Building", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Building.resolveStringCallback = func;
			this.Equipping = new StatusItem("Equipping", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Equipping.resolveStringCallback = func;
			this.WarmingUp = new StatusItem("WarmingUp", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.WarmingUp.resolveStringCallback = func;
			this.GeneratingPower = new StatusItem("GeneratingPower", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.GeneratingPower.resolveStringCallback = func;
			this.Harvesting = new StatusItem("Harvesting", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Harvesting.resolveStringCallback = func;
			this.Uprooting = new StatusItem("Uprooting", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Uprooting.resolveStringCallback = func;
			this.Emptying = new StatusItem("Emptying", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Emptying.resolveStringCallback = func;
			this.Toggling = new StatusItem("Toggling", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Toggling.resolveStringCallback = func;
			this.Deconstructing = new StatusItem("Deconstructing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Deconstructing.resolveStringCallback = func;
			this.Disinfecting = new StatusItem("Disinfecting", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Disinfecting.resolveStringCallback = func;
			this.Upgrading = new StatusItem("Upgrading", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Upgrading.resolveStringCallback = func;
			this.Fabricating = new StatusItem("Fabricating", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Fabricating.resolveStringCallback = func2;
			this.Processing = new StatusItem("Processing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Processing.resolveStringCallback = func2;
			this.Clearing = new StatusItem("Clearing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Clearing.resolveStringCallback = func;
			this.GeneratingPower = new StatusItem("GeneratingPower", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Cold = new StatusItem("Cold", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Cold.resolveTooltipCallback = delegate(string str, object data)
			{
				str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("ColdAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float num = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedWattage(num, GameUtil.WattageFormatterUnit.Automatic));
				AttributeInstance attributeInstance = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string text2 = attributeInstance.GetFormattedValue();
				text2 += "\n----------\n";
				foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in attributeInstance.Modifiers)
				{
					text2 = text2 + attributeModifierEntry.Name + " " + attributeModifierEntry.Modifier.GetFormattedString(attributeInstance.gameObject);
					text2 += "\n";
				}
				str = str.Replace("{conductivityBarrier}", text2);
				return str;
			};
			this.Hot = new StatusItem("Hot", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Hot.resolveTooltipCallback = delegate(string str, object data)
			{
				str = str.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("WarmAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float num2 = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				str = str.Replace("{currentTransferWattage}", GameUtil.GetFormattedWattage(num2, GameUtil.WattageFormatterUnit.Automatic));
				AttributeInstance attributeInstance2 = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string text3 = attributeInstance2.GetFormattedValue();
				text3 += "\n----------\n";
				foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry2 in attributeInstance2.Modifiers)
				{
					text3 = text3 + attributeModifierEntry2.Name + " " + attributeModifierEntry2.Modifier.GetFormattedString(attributeInstance2.gameObject);
					text3 += "\n";
				}
				str = str.Replace("{conductivityBarrier}", text3);
				return str;
			};
			this.BodyRegulatingHeating = new StatusItem("BodyRegulatingHeating", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.BodyRegulatingHeating.resolveStringCallback = delegate(string str, object data)
			{
				WarmBlooded.StatesInstance statesInstance2 = (WarmBlooded.StatesInstance)data;
				return str.Replace("{TempDelta}", GameUtil.GetFormattedTemperature(statesInstance2.TemperatureDelta, GameUtil.TimeSlice.PerSecond, GameUtil.TemperatureInterpretation.Relative, true));
			};
			this.BodyRegulatingCooling = new StatusItem("BodyRegulatingCooling", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.BodyRegulatingCooling.resolveStringCallback = this.BodyRegulatingHeating.resolveStringCallback;
			this.EntombedChore = new StatusItem("EntombedChore", "DUPLICANTS", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, false, SimViewMode.None, true, 30718);
			this.EntombedChore.AddNotification(null, null, null, 0f);
			this.EarlyMorning = new StatusItem("EarlyMorning", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.NightTime = new StatusItem("NightTime", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.PoorDecor = new StatusItem("PoorDecor", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.PoorFoodQuality = new StatusItem("PoorFoodQuality", DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, 30718);
			this.GoodFoodQuality = new StatusItem("GoodFoodQuality", DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, 30718);
			this.Arting = new StatusItem("Arting", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Arting.resolveStringCallback = func;
			this.SevereWounds = new StatusItem("SevereWounds", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.Bad, false, SimViewMode.None, true, 30718);
			this.SevereWounds.AddNotification(null, null, null, 0f);
			this.Incapacitated = new StatusItem("Incapacitated", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.DuplicantThreatening, false, SimViewMode.None, true, 30718);
			this.Incapacitated.AddNotification(null, null, null, 0f);
			this.Incapacitated.resolveStringCallback = delegate(string str, object data)
			{
				IncapacitationMonitor.Instance instance = (IncapacitationMonitor.Instance)data;
				float bleedLifeTime = instance.GetBleedLifeTime(instance);
				str = str.Replace("{CauseOfIncapacitation}", instance.GetCauseOfIncapacitation().Name);
				return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedTime(bleedLifeTime));
			};
			this.Relocating = new StatusItem("Relocating", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Relocating.resolveStringCallback = func;
			this.Fighting = new StatusItem("Fighting", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 30718);
			this.Fighting.AddNotification(null, null, null, 0f);
			this.Fleeing = new StatusItem("Fleeing", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 30718);
			this.Fleeing.AddNotification(null, null, null, 0f);
			this.Stressed = new StatusItem("Stressed", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Stressed.AddNotification(null, null, null, 0f);
			this.LashingOut = new StatusItem("LashingOut", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 30718);
			this.LashingOut.AddNotification(null, null, null, 0f);
			this.LowImmunity = new StatusItem("LowImmunity", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.LowImmunity.AddNotification(null, null, null, 0f);
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

		public StatusItem Sleeping;

		public StatusItem SleepingInterruptedLight;

		public StatusItem SleepingInterrupted;

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

		public StatusItem Storing;

		public StatusItem Building;

		public StatusItem Equipping;

		public StatusItem WarmingUp;

		public StatusItem GeneratingPower;

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

		public StatusItem Clearing;

		public StatusItem BodyRegulatingHeating;

		public StatusItem BodyRegulatingCooling;

		public StatusItem EntombedChore;

		public StatusItem EarlyMorning;

		public StatusItem NightTime;

		public StatusItem PoorDecor;

		public StatusItem PoorFoodQuality;

		public StatusItem GoodFoodQuality;

		public StatusItem SevereWounds;

		public StatusItem Incapacitated;

		public StatusItem Fighting;

		public StatusItem Fleeing;

		public StatusItem Stressed;

		public StatusItem LashingOut;

		public StatusItem LowImmunity;

		public StatusItem Role;
	}
}
