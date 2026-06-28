using System;
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
			this.BedUnreachable = new StatusItem("BedUnreachable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.BedUnreachable.AddNotification(null, null, null, 0f);
			this.DailyRationLimitReached = new StatusItem("DailyRationLimitReached", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.DailyRationLimitReached.AddNotification(null, null, null, 0f);
			this.HoldingBreath = new StatusItem("HoldingBreath", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Hungry = new StatusItem("Hungry", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Unhappy = new StatusItem("Unhappy", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Unhappy.AddNotification(null, null, null, 0f);
			this.NervousBreakdown = new StatusItem("NervousBreakdown", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.NervousBreakdown.AddNotification(null, null, null, 0f);
			this.NoRationsAvailable = new StatusItem("NoRationsAvailable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.PendingPacification = new StatusItem("PendingPacification", "DUPLICANTS", "status_item_pending_pacification", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.QuarantineAreaUnassigned = new StatusItem("QuarantineAreaUnassigned", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.QuarantineAreaUnassigned.AddNotification(null, null, null, 0f);
			this.QuarantineAreaUnreachable = new StatusItem("QuarantineAreaUnreachable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.QuarantineAreaUnreachable.AddNotification(null, null, null, 0f);
			this.Quarantined = new StatusItem("Quarantined", "DUPLICANTS", "status_item_quarantined", StatusItem.IconType.Custom, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.RationsUnreachable = new StatusItem("RationsUnreachable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.RationsUnreachable.AddNotification(null, null, null, 0f);
			this.Rotten = new StatusItem("Rotten", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Starving = new StatusItem("Starving", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.Starving.AddNotification(null, null, null, 0f);
			this.Suffocating = new StatusItem("Suffocating", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.Suffocating.AddNotification(null, null, null, 0f);
			this.Tired = new StatusItem("Tired", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Idle = new StatusItem("Idle", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Idle.AddNotification(null, null, null, 0f);
			this.Pacified = new StatusItem("Pacified", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Dead = new StatusItem("Dead", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Dead.resolveStringCallback = delegate(string str, object data)
			{
				Death death = (Death)data;
				return str.Replace("{Death}", death.Name);
			};
			this.MoveToSuitNotRequired = new StatusItem("MoveToSuitNotRequired", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.DroppingUnusedInventory = new StatusItem("DroppingUnusedInventory", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.MovingToSafeArea = new StatusItem("MovingToSafeArea", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.ToiletUnreachable = new StatusItem("ToiletUnreachable", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.ToiletUnreachable.AddNotification(null, null, null, 0f);
			this.NoUsableToilets = new StatusItem("NoUsableToilets", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.NoUsableToilets.AddNotification(null, null, null, 0f);
			this.NoToilets = new StatusItem("NoToilets", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.NoToilets.AddNotification(null, null, null, 0f);
			this.FullBladder = new StatusItem("FullBladder", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.FullBladder.AddEffect("FullBladder");
			this.StressfullyEmptyingBladder = new StatusItem("StressfullyEmptyingBladder", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.StressfullyEmptyingBladder.AddEffect("StressfulyEmptyingBladder");
			this.BreathingO2 = new StatusItem("BreathingO2", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.BreathingO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather = (OxygenBreather)data;
				return str.Replace("{ConsumptionRate}", GameUtil.GetFormattedMass(-oxygenBreather.O2ConsumptionRate, GameUtil.TimeSlice.PerSecond, true, "{0:0.#}"));
			};
			this.EmittingCO2 = new StatusItem("EmittingCO2", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.EmittingCO2.resolveStringCallback = delegate(string str, object data)
			{
				OxygenBreather oxygenBreather2 = (OxygenBreather)data;
				return str.Replace("{EmittingRate}", GameUtil.GetFormattedMass(oxygenBreather2.CO2EmitRate, GameUtil.TimeSlice.PerSecond, true, "{0:0.#}"));
			};
			this.Vomiting = new StatusItem("Vomiting", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.LowOxygen = new StatusItem("LowOxygen", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.LowOxygen.AddNotification(null, null, null, 0f);
			this.RedAlert = new StatusItem("RedAlert", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Sleeping = new StatusItem("Sleeping", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Eating = new StatusItem("Eating", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Eating.resolveStringCallback = func;
			this.Digging = new StatusItem("Digging", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Cleaning = new StatusItem("Cleaning", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Cleaning.resolveStringCallback = func;
			this.PickingUp = new StatusItem("PickingUp", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.PickingUp.resolveStringCallback = func;
			this.Mopping = new StatusItem("Mopping", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Cooking = new StatusItem("Cooking", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Mushing = new StatusItem("Mushing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Researching = new StatusItem("Researching", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Researching.resolveStringCallback = delegate(string str, object data)
			{
				TechInstance activeResearch = Research.Instance.GetActiveResearch();
				if (activeResearch != null)
				{
					return str.Replace("{Tech}", activeResearch.tech.Name);
				}
				return str;
			};
			this.Storing = new StatusItem("Storing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Building = new StatusItem("Building", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Building.resolveStringCallback = func;
			this.Equipping = new StatusItem("Equipping", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Equipping.resolveStringCallback = func;
			this.WarmingUp = new StatusItem("WarmingUp", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.WarmingUp.resolveStringCallback = func;
			this.GeneratingPower = new StatusItem("GeneratingPower", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.GeneratingPower.resolveStringCallback = func;
			this.Harvesting = new StatusItem("Harvesting", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Harvesting.resolveStringCallback = func;
			this.Uprooting = new StatusItem("Uprooting", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Uprooting.resolveStringCallback = func;
			this.Emptying = new StatusItem("Emptying", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Emptying.resolveStringCallback = func;
			this.Toggling = new StatusItem("Toggling", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Toggling.resolveStringCallback = func;
			this.Deconstructing = new StatusItem("Deconstructing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Deconstructing.resolveStringCallback = func;
			this.Upgrading = new StatusItem("Upgrading", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Upgrading.resolveStringCallback = func;
			this.Fabricating = new StatusItem("Fabricating", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Fabricating.resolveStringCallback = func;
			this.Clearing = new StatusItem("Clearing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Clearing.resolveStringCallback = func;
			this.GeneratingPower = new StatusItem("GeneratingPower", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Emptying = new StatusItem("Emptying", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Toggling = new StatusItem("Toggling", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Deconstructing = new StatusItem("Deconstructing", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Upgrading = new StatusItem("Upgrading", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Fabricating = new StatusItem("Fabricating", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Cold = new StatusItem("Cold", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Cold.resolveStringCallback = delegate(string str, object data)
			{
				string text = DUPLICANTS.STATUSITEMS.COLD.TOOLTIP;
				text = text.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("ColdAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float num = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				text = text.Replace("{currentTransferWattage}", GameUtil.GetFormattedWattage(num, "F1"));
				AttributeInstance attributeInstance = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string text2 = attributeInstance.GetFormattedValue(true);
				text2 += "\n----------\n";
				foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in attributeInstance.Modifiers)
				{
					text2 = text2 + attributeModifierEntry.Name + " " + attributeModifierEntry.Modifier.GetFormattedString();
					text2 += "\n";
				}
				text = text.Replace("{conductivityBarrier}", text2);
				this.Cold.tooltipText = text;
				return str;
			};
			this.Hot = new StatusItem("Hot", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Hot.resolveStringCallback = delegate(string str, object data)
			{
				string text3 = DUPLICANTS.STATUSITEMS.HOT.TOOLTIP;
				text3 = text3.Replace("{StressModification}", GameUtil.GetFormattedPercent(Db.Get().effects.Get("WarmAir").SelfModifiers[0].Value, GameUtil.TimeSlice.PerCycle));
				float num2 = ((ExternalTemperatureMonitor.Instance)data).temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage * 1000f;
				text3 = text3.Replace("{currentTransferWattage}", GameUtil.GetFormattedWattage(num2, "F1"));
				AttributeInstance attributeInstance2 = ((ExternalTemperatureMonitor.Instance)data).attributes.Get("ThermalConductivityBarrier");
				string text4 = attributeInstance2.GetFormattedValue(true);
				text4 += "\n----------\n";
				foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry2 in attributeInstance2.Modifiers)
				{
					text4 = text4 + attributeModifierEntry2.Name + " " + attributeModifierEntry2.Modifier.GetFormattedString();
					text4 += "\n";
				}
				text3 = text3.Replace("{conductivityBarrier}", text4);
				this.Hot.tooltipText = text3;
				return str;
			};
			this.BodyRegulatingHeating = new StatusItem("BodyRegulatingHeating", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.BodyRegulatingHeating.resolveStringCallback = delegate(string str, object data)
			{
				WarmBlooded.StatesInstance statesInstance = (WarmBlooded.StatesInstance)data;
				return str.Replace("{TempDelta}", GameUtil.GetFormattedTemperature(statesInstance.TemperatureDelta, GameUtil.TimeSlice.PerSecond, GameUtil.TemperatureInterpretation.Relative, true));
			};
			this.BodyRegulatingCooling = new StatusItem("BodyRegulatingCooling", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.BodyRegulatingCooling.resolveStringCallback = this.BodyRegulatingHeating.resolveStringCallback;
			this.EntombedChore = new StatusItem("EntombedChore", "DUPLICANTS", "status_item_entombed", StatusItem.IconType.Custom, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.EntombedChore.AddNotification(null, null, null, 0f);
			this.EarlyMorning = new StatusItem("EarlyMorning", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.NightTime = new StatusItem("NightTime", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.PoorDecor = new StatusItem("PoorDecor", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.PoorFoodQuality = new StatusItem("PoorFoodQuality", DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.POOR_FOOD_QUALITY.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.GoodFoodQuality = new StatusItem("GoodFoodQuality", DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.NAME, DUPLICANTS.STATUSITEMS.GOOD_FOOD_QUALITY.TOOLTIP, string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Arting = new StatusItem("Arting", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Arting.resolveStringCallback = func;
			this.SevereWounds = new StatusItem("SevereWounds", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.SevereWounds.AddNotification(null, null, null, 0f);
			this.SevereWounds.resolveStringCallback = (string str, object data) => str;
			this.Incapacitated = new StatusItem("Incapacitated", "DUPLICANTS", "status_item_broken", StatusItem.IconType.Custom, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.Incapacitated.AddNotification(null, null, null, 0f);
			this.Incapacitated.resolveStringCallback = delegate(string str, object data)
			{
				Health health = (Health)data;
				float bleedLifeTime = health.GetBleedLifeTime();
				return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedTime(bleedLifeTime));
			};
			this.Relocating = new StatusItem("Relocating", "DUPLICANTS", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Relocating.resolveStringCallback = func;
			this.Fighting = new StatusItem("Fighting", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.Fighting.AddNotification(null, null, null, 0f);
			this.Fighting.resolveStringCallback = (string str, object data) => str;
			this.Fleeing = new StatusItem("Fleeing", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.Fleeing.AddNotification(null, null, null, 0f);
			this.Fleeing.resolveStringCallback = (string str, object data) => str;
			this.Stressed = new StatusItem("Stressed", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Stressed.AddNotification(null, null, null, 0f);
			this.LashingOut = new StatusItem("LashingOut", "DUPLICANTS", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.LashingOut.AddNotification(null, null, null, 0f);
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

		public StatusItem FullBladder;

		public StatusItem StressfullyEmptyingBladder;

		public StatusItem Vomiting;

		public StatusItem BreathingO2;

		public StatusItem EmittingCO2;

		public StatusItem LowOxygen;

		public StatusItem RedAlert;

		public StatusItem Digging;

		public StatusItem Eating;

		public StatusItem Sleeping;

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

		public StatusItem Relocating;

		public StatusItem Upgrading;

		public StatusItem Fabricating;

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
	}
}
