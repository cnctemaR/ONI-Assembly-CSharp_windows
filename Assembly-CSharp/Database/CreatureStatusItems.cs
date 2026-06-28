using System;
using STRINGS;

namespace Database
{
	public class CreatureStatusItems : StatusItems
	{
		public CreatureStatusItems(ResourceSet parent)
			: base("CreatureStatusItems", parent)
		{
			this.CreateStatusItems();
		}

		private void CreateStatusItems()
		{
			this.Idle = new StatusItem("Idle", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Burrowing = new StatusItem("Burrowing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Burrowed = new StatusItem("Burrowed", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Emerging = new StatusItem("Emerging", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.LookingForFood = new StatusItem("LookingForFood", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Eating = new StatusItem("Eating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Sleeping = new StatusItem("Sleeping", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Struggling = new StatusItem("Struggling", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Dead = new StatusItem("Dead", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Hot = new StatusItem("Hot", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Hot.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable.externalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureVulnerable.externalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			};
			this.Scalding = new StatusItem("Scalding", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, true, SimViewMode.None, SimViewMode.None, true);
			this.Scalding.resolveStringCallback = delegate(string str, object data)
			{
				string text = CREATURES.STATUSITEMS.SCALDING.TOOLTIP;
				float averageExternalTemperature = ((ExternalTemperatureMonitor.Instance)data).AverageExternalTemperature;
				float scaldingThreshold = ((ExternalTemperatureMonitor.Instance)data).ScaldingThreshold;
				text = text.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(averageExternalTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				text = text.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(scaldingThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				this.Scalding.tooltipText = text;
				return str;
			};
			this.Scalding.AddNotification(null, null, null, 0f);
			this.Cold = new StatusItem("Cold", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Cold.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable2 = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable2.externalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureVulnerable2.externalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			};
			this.PerfectTemperature = new StatusItem("PerfectTemperature", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Good, false, SimViewMode.None, SimViewMode.None, true);
			this.PerfectTemperature.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable3 = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable3.externalTemperaturePerfect_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureVulnerable3.externalTemperaturePerfect_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			};
			this.Hyperthermia = new StatusItem("Hyperthermia", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.Hyperthermia.resolveStringCallback = delegate(string str, object data)
			{
				string text2 = CREATURES.STATUSITEMS.HYPERTHERMIA.TOOLTIP;
				float value = ((TemperatureMonitor.Instance)data).temperature.value;
				float hyperthermiaThreshold = ((TemperatureMonitor.Instance)data).HyperthermiaThreshold;
				text2 = text2.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				text2 = text2.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hyperthermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				this.Hyperthermia.tooltipText = text2;
				return str;
			};
			this.Hypothermia = new StatusItem("Hypothermia", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, true);
			this.Hypothermia.resolveStringCallback = delegate(string str, object data)
			{
				string text3 = CREATURES.STATUSITEMS.HYPOTHERMIA.TOOLTIP;
				float value2 = ((TemperatureMonitor.Instance)data).temperature.value;
				float hypothermiaThreshold = ((TemperatureMonitor.Instance)data).HypothermiaThreshold;
				text3 = text3.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				text3 = text3.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hypothermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				this.Hypothermia.tooltipText = text3;
				return str;
			};
			this.Hungry = new StatusItem("Hungry", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.LayingAnEgg = new StatusItem("LayingAnEgg", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Suffocating = new StatusItem("Suffocating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Hatching = new StatusItem("Hatching", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Incubating = new StatusItem("Incubating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.ConsideringLure = new StatusItem("ConsideringLure", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Falling = new StatusItem("Falling", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Drowning = new StatusItem("Drowning", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.DryingOut = new StatusItem("DryingOut", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.ReadyForHarvest = new StatusItem("ReadyForHarvest", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Growing = new StatusItem("Growing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Growing.resolveStringCallback = delegate(string str, object data)
			{
				float num = 100f * ((Growing)data).PercentGrown();
				str = str.Replace("{PercentGrow}", Math.Floor((double)Math.Max(num, 0f)).ToString("F0"));
				return str;
			};
			this.HarvestsRemaining = new StatusItem("HarvestsRemaining", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.HarvestsRemaining.resolveStringCallback = delegate(string str, object data)
			{
				str = str.Replace("{HarvestsRemaining}", ((Crop)data).GetHarvestsRemaining().ToString());
				return str;
			};
			this.EnvironmentTooWarm = new StatusItem("EnvironmentTooWarm", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.EnvironmentTooWarm.resolveStringCallback = delegate(string str, object data)
			{
				float num2 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num3 = ((TemperatureVulnerable)data).externalTemperatureLethal_High - 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num3, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.EnvironmentTooCold = new StatusItem("EnvironmentTooCold", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.EnvironmentTooCold.resolveStringCallback = delegate(string str, object data)
			{
				float num4 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num5 = ((TemperatureVulnerable)data).externalTemperatureLethal_Low + 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num5, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Entombed = new StatusItem("Entombed", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Wilting = new StatusItem("Wilting", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.WiltingDomestic = new StatusItem("WiltingDomestic", CREATURES.STATUSITEMS.WILTING.NAME, CREATURES.STATUSITEMS.WILTING.TOOLTIP, "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.AtmosphericPressureTooLow = new StatusItem("AtmosphericPressureTooLow", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.AtmosphericPressureTooLow.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable = (PressureVulnerable)data;
				float pressureWarning_Low = pressureVulnerable.pressureWarning_Low;
				return str.Replace("{TargetPressure}", GameUtil.GetFormattedMass(pressureWarning_Low, GameUtil.TimeSlice.None, true, "{0:0.#}"));
			};
			this.AtmosphericPressureTooHigh = new StatusItem("AtmosphericPressureTooHigh", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.AtmosphericPressureTooHigh.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable2 = (PressureVulnerable)data;
				float pressureWarning_High = pressureVulnerable2.pressureWarning_High;
				return str.Replace("{TargetPressure}", GameUtil.GetFormattedMass(pressureWarning_High, GameUtil.TimeSlice.None, true, "{0:0.#}"));
			};
			this.PerfectAtmosphericPressure = new StatusItem("PerfectAtmosphericPressure", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Good, false, SimViewMode.None, SimViewMode.None, true);
			this.PerfectAtmosphericPressure.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable3 = (PressureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedMass(pressureVulnerable3.pressurePerfect_Low, GameUtil.TimeSlice.None, true, "{0:0.#}"), GameUtil.GetFormattedMass(pressureVulnerable3.pressurePerfect_High, GameUtil.TimeSlice.None, true, "{0:0.#}"));
			};
			this.HealthStatus = new StatusItem("HealthStatus", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.HealthStatus.resolveStringCallback = delegate(string str, object data)
			{
				string text4 = string.Empty;
				string text5 = string.Empty;
				switch ((int)data)
				{
				case 0:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.NAME;
					text5 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.TOOLTIP;
					break;
				case 1:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.NAME;
					text5 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.TOOLTIP;
					break;
				case 2:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.NAME;
					text5 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.TOOLTIP;
					break;
				case 3:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.NAME;
					text5 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.TOOLTIP;
					break;
				case 4:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.NAME;
					text5 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.TOOLTIP;
					break;
				case 5:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.NAME;
					text5 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.TOOLTIP;
					break;
				}
				str = str.Replace("{healthState}", text4);
				str = str.Replace("{healthStateTooltip}", text5);
				return str;
			};
			this.Barren = new StatusItem("Barren", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Fleeing = new StatusItem("Fleeing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			Func<string, object, string> func = delegate(string str, object data)
			{
				FertilizationMonitor.Instance instance = (FertilizationMonitor.Instance)data;
				if (str.Contains("{Required}"))
				{
					string text6 = string.Empty;
					foreach (FertilizationMonitor.FertilizerInfo fertilizerInfo in instance.def.consumedElements)
					{
						text6 += string.Format(CREATURES.STATUSITEMS.NEEDSFERTILIZER.LINE_ITEM, fertilizerInfo.tag.ProperName(), GameUtil.GetFormattedMass(fertilizerInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, true, "{0:0.#}"));
					}
					str = str.Replace("{Required}", text6);
				}
				return str;
			};
			this.NeedsFertilizer = new StatusItem("NeedsFertilizer", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.NeedsFertilizer.resolveStringCallback = func;
			this.NeedsIrrigation = new StatusItem("NeedsIrrigation", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.NeedsIrrigation.resolveStringCallback = func;
			this.CantAcceptFertilizer = new StatusItem("CantAcceptFertilizer", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.CantAcceptIrrigation = new StatusItem("CantAcceptIrrigation", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Rotting = new StatusItem("Rotting", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Rotting.resolveStringCallback = (string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			this.Fresh = new StatusItem("Fresh", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Fresh.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance2 = (Rottable.Instance)data;
				this.Fresh.tooltipText = instance2.GetToolTip();
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance2.RotConstitutionPercentage * 100f) + "%)");
			};
			this.Stale = new StatusItem("Stale", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Stale.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance3 = (Rottable.Instance)data;
				this.Stale.tooltipText = instance3.GetToolTip();
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance3.RotConstitutionPercentage * 100f) + "%)");
			};
			this.Spoiled = new StatusItem("Spoiled", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Refrigerated = new StatusItem("Refrigerated", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.Unrefrigerated = new StatusItem("Unrefrigerated", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Unrefrigerated.resolveStringCallback = (string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			this.SterilizingAtmosphere = new StatusItem("SterilizingAtmosphere", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
			this.ContaminatedAtmosphere = new StatusItem("ContaminatedAtmosphere", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None, true);
			this.Exhaling = new StatusItem("Exhaling", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None, true);
		}

		public StatusItem Idle;

		public StatusItem HealthStatus;

		public StatusItem Burrowing;

		public StatusItem Burrowed;

		public StatusItem Emerging;

		public StatusItem Eating;

		public StatusItem LookingForFood;

		public StatusItem Sleeping;

		public StatusItem Struggling;

		public StatusItem Dead;

		public StatusItem Hot;

		public StatusItem Scalding;

		public StatusItem Cold;

		public StatusItem PerfectTemperature;

		public StatusItem Hypothermia;

		public StatusItem Hyperthermia;

		public StatusItem Hungry;

		public StatusItem LayingAnEgg;

		public StatusItem Suffocating;

		public StatusItem Hatching;

		public StatusItem Incubating;

		public StatusItem ConsideringLure;

		public StatusItem Falling;

		public StatusItem Drowning;

		public StatusItem DryingOut;

		public StatusItem Growing;

		public StatusItem HarvestsRemaining;

		public StatusItem ReadyForHarvest;

		public StatusItem EnvironmentTooWarm;

		public StatusItem EnvironmentTooCold;

		public StatusItem Entombed;

		public StatusItem Wilting;

		public StatusItem WiltingDomestic;

		public StatusItem AtmosphericPressureTooLow;

		public StatusItem AtmosphericPressureTooHigh;

		public StatusItem PerfectAtmosphericPressure;

		public StatusItem Barren;

		public StatusItem Fleeing;

		public StatusItem NeedsFertilizer;

		public StatusItem NeedsIrrigation;

		public StatusItem CantAcceptFertilizer;

		public StatusItem CantAcceptIrrigation;

		public StatusItem Rotting;

		public StatusItem Fresh;

		public StatusItem Stale;

		public StatusItem Spoiled;

		public StatusItem Refrigerated;

		public StatusItem Unrefrigerated;

		public StatusItem SterilizingAtmosphere;

		public StatusItem ContaminatedAtmosphere;

		public StatusItem Exhaling;
	}
}
