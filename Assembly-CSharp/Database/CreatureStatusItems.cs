using System;
using STRINGS;
using UnityEngine;

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
			this.Idle = new StatusItem("Idle", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Burrowing = new StatusItem("Burrowing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Burrowed = new StatusItem("Burrowed", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Emerging = new StatusItem("Emerging", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.LookingForFood = new StatusItem("LookingForFood", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Eating = new StatusItem("Eating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Sleeping = new StatusItem("Sleeping", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Struggling = new StatusItem("Struggling", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Dead = new StatusItem("Dead", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Hot = new StatusItem("Hot", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			this.Hot.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureVulnerable.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			};
			this.Hot_Crop = new StatusItem("Hot_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			this.Hot_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable2 = (TemperatureVulnerable)data;
				str = str.Replace("{low_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable2.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{high_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable2.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Scalding = new StatusItem("Scalding", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, true, SimViewMode.None, true, 30718);
			this.Scalding.resolveTooltipCallback = delegate(string str, object data)
			{
				float averageExternalTemperature = ((ExternalTemperatureMonitor.Instance)data).AverageExternalTemperature;
				float scaldingThreshold = ((ExternalTemperatureMonitor.Instance)data).GetScaldingThreshold();
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(averageExternalTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(scaldingThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Scalding.AddNotification(null, null, null, 0f);
			this.Cold = new StatusItem("Cold", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			this.Cold.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable3 = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable3.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureVulnerable3.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			};
			this.Cold_Crop = new StatusItem("Cold_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			this.Cold_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable4 = (TemperatureVulnerable)data;
				str = str.Replace("low_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable4.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("high_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable4.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Crop_Too_Dark = new StatusItem("Crop_Too_Dark", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			this.Crop_Too_Dark.resolveStringCallback = (string str, object data) => str;
			this.Crop_Too_Bright = new StatusItem("Crop_Too_Bright", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			this.Crop_Too_Bright.resolveStringCallback = (string str, object data) => str;
			this.Hyperthermia = new StatusItem("Hyperthermia", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 30718);
			this.Hyperthermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value = ((TemperatureMonitor.Instance)data).temperature.value;
				float hyperthermiaThreshold = ((TemperatureMonitor.Instance)data).HyperthermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hyperthermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Hypothermia = new StatusItem("Hypothermia", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 30718);
			this.Hypothermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value2 = ((TemperatureMonitor.Instance)data).temperature.value;
				float hypothermiaThreshold = ((TemperatureMonitor.Instance)data).HypothermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hypothermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Hungry = new StatusItem("Hungry", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.LayingAnEgg = new StatusItem("LayingAnEgg", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Suffocating = new StatusItem("Suffocating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Hatching = new StatusItem("Hatching", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Incubating = new StatusItem("Incubating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.ConsideringLure = new StatusItem("ConsideringLure", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Falling = new StatusItem("Falling", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Drowning = new StatusItem("Drowning", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Drowning.resolveStringCallback = (string str, object data) => str;
			this.DryingOut = new StatusItem("DryingOut", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 1026);
			this.DryingOut.resolveStringCallback = (string str, object data) => str;
			this.ReadyForHarvest = new StatusItem("ReadyForHarvest", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 1026);
			this.Growing = new StatusItem("Growing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 1026);
			this.Growing.resolveStringCallback = delegate(string str, object data)
			{
				Crop component = ((Growing)data).GetComponent<Crop>();
				if (component != null)
				{
					float num = ((Growing)data).TimeUntilNextHarvest();
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(num, "F1"));
				}
				float num2 = 100f * ((Growing)data).PercentGrown();
				str = str.Replace("{PercentGrow}", Math.Floor((double)Math.Max(num2, 0f)).ToString("F0"));
				return str;
			};
			this.EnvironmentTooWarm = new StatusItem("EnvironmentTooWarm", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.EnvironmentTooWarm.resolveStringCallback = delegate(string str, object data)
			{
				float num3 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num4 = ((TemperatureVulnerable)data).internalTemperatureLethal_High - 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num3, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.EnvironmentTooCold = new StatusItem("EnvironmentTooCold", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.EnvironmentTooCold.resolveStringCallback = delegate(string str, object data)
			{
				float num5 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num6 = ((TemperatureVulnerable)data).internalTemperatureLethal_Low + 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num5, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num6, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Entombed = new StatusItem("Entombed", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Entombed.resolveStringCallback = (string str, object data) => str;
			this.Wilting = new StatusItem("Wilting", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 1026);
			this.Wilting.resolveStringCallback = delegate(string str, object data)
			{
				if (data is Growing && data != null)
				{
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(Mathf.Min(((Growing)data).growthTime, ((Growing)data).TimeUntilNextHarvest()), "F1"));
				}
				str = str.Replace("{Reasons}", (data as KMonoBehaviour).GetComponent<WiltCondition>().WiltCausesString());
				return str;
			};
			this.WiltingDomestic = new StatusItem("WiltingDomestic", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 1026);
			this.WiltingDomestic.resolveStringCallback = delegate(string str, object data)
			{
				if (data is Growing && data != null)
				{
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(Mathf.Min(((Growing)data).growthTime, ((Growing)data).TimeUntilNextHarvest()), "F1"));
				}
				str = str.Replace("{Reasons}", (data as KMonoBehaviour).GetComponent<WiltCondition>().WiltCausesString());
				return str;
			};
			this.WiltingNonGrowing = new StatusItem("WiltingNonGrowing", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 1026);
			this.WiltingNonGrowing.resolveStringCallback = delegate(string str, object data)
			{
				str = CREATURES.STATUSITEMS.WILTING_NON_GROWING_PLANT.NAME;
				str = str.Replace("{Reasons}", (data as WiltCondition).WiltCausesString());
				return str;
			};
			this.WiltingNonGrowingDomestic = new StatusItem("WiltingNonGrowing", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 1026);
			this.WiltingNonGrowingDomestic.resolveStringCallback = delegate(string str, object data)
			{
				str = CREATURES.STATUSITEMS.WILTING_NON_GROWING_PLANT.NAME;
				str = str.Replace("{Reasons}", (data as WiltCondition).WiltCausesString());
				return str;
			};
			this.WrongAtmosphere = new StatusItem("WrongAtmosphere", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			this.WrongAtmosphere.resolveStringCallback = delegate(string str, object data)
			{
				string text = string.Empty;
				foreach (Element element in (data as PressureVulnerable).safe_atmospheres)
				{
					text = text + "\n    •  " + element.name;
				}
				str = str.Replace("{elements}", text);
				return str;
			};
			this.AtmosphericPressureTooLow = new StatusItem("AtmosphericPressureTooLow", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			this.AtmosphericPressureTooLow.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.AtmosphericPressureTooHigh = new StatusItem("AtmosphericPressureTooHigh", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			this.AtmosphericPressureTooHigh.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable2 = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.HealthStatus = new StatusItem("HealthStatus", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.HealthStatus.resolveStringCallback = delegate(string str, object data)
			{
				string text2 = string.Empty;
				switch ((Health.HealthState)data)
				{
				case Health.HealthState.Perfect:
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.NAME;
					break;
				case Health.HealthState.Scuffed:
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.NAME;
					break;
				case Health.HealthState.Injured:
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.NAME;
					break;
				case Health.HealthState.Critical:
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.NAME;
					break;
				case Health.HealthState.Incapacitated:
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.NAME;
					break;
				case Health.HealthState.Dead:
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.NAME;
					break;
				}
				str = str.Replace("{healthState}", text2);
				return str;
			};
			this.HealthStatus.resolveTooltipCallback = delegate(string str, object data)
			{
				string text3 = string.Empty;
				switch ((Health.HealthState)data)
				{
				case Health.HealthState.Perfect:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.TOOLTIP;
					break;
				case Health.HealthState.Scuffed:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.TOOLTIP;
					break;
				case Health.HealthState.Injured:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.TOOLTIP;
					break;
				case Health.HealthState.Critical:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.TOOLTIP;
					break;
				case Health.HealthState.Incapacitated:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.TOOLTIP;
					break;
				case Health.HealthState.Dead:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.TOOLTIP;
					break;
				}
				str = str.Replace("{healthState}", text3);
				return str;
			};
			this.Barren = new StatusItem("Barren", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Fleeing = new StatusItem("Fleeing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.NeedsFertilizer = new StatusItem("NeedsFertilizer", "CREATURES", "status_item_plant_solid", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			Func<string, object, string> func = (string str, object data) => str;
			this.NeedsFertilizer.resolveStringCallback = func;
			this.NeedsIrrigation = new StatusItem("NeedsIrrigation", "CREATURES", "status_item_plant_liquid", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 30718);
			Func<string, object, string> func2 = (string str, object data) => str;
			this.NeedsIrrigation.resolveStringCallback = func2;
			this.WrongFertilizer = new StatusItem("WrongFertilizer", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			Func<string, object, string> func3 = (string str, object data) => str;
			this.WrongFertilizer.resolveStringCallback = func3;
			this.WrongFertilizerMajor = new StatusItem("WrongFertilizerMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.WrongFertilizerMajor.resolveStringCallback = func3;
			this.WrongIrrigation = new StatusItem("WrongIrrigation", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			Func<string, object, string> func4 = (string str, object data) => str;
			this.WrongIrrigation.resolveStringCallback = func4;
			this.WrongIrrigationMajor = new StatusItem("WrongIrrigationMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.WrongIrrigationMajor.resolveStringCallback = func4;
			this.CantAcceptFertilizer = new StatusItem("CantAcceptFertilizer", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Rotting = new StatusItem("Rotting", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Rotting.resolveStringCallback = (string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			this.Fresh = new StatusItem("Fresh", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Fresh.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance = (Rottable.Instance)data;
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance.RotConstitutionPercentage * 100f) + "%)");
			};
			this.Fresh.resolveTooltipCallback = delegate(string str, object data)
			{
				Rottable.Instance instance2 = (Rottable.Instance)data;
				return str.Replace("{RotTooltip}", instance2.GetToolTip());
			};
			this.Stale = new StatusItem("Stale", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Stale.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance3 = (Rottable.Instance)data;
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance3.RotConstitutionPercentage * 100f) + "%)");
			};
			this.Stale.resolveTooltipCallback = delegate(string str, object data)
			{
				Rottable.Instance instance4 = (Rottable.Instance)data;
				return str.Replace("{RotTooltip}", instance4.GetToolTip());
			};
			this.Spoiled = new StatusItem("Spoiled", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Refrigerated = new StatusItem("Refrigerated", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Unrefrigerated = new StatusItem("Unrefrigerated", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Unrefrigerated.resolveStringCallback = (string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			this.SterilizingAtmosphere = new StatusItem("SterilizingAtmosphere", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.ContaminatedAtmosphere = new StatusItem("ContaminatedAtmosphere", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 30718);
			this.Exhaling = new StatusItem("Exhaling", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
			this.Trapped = new StatusItem("Trapped", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 30718);
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

		public StatusItem Hot_Crop;

		public StatusItem Scalding;

		public StatusItem Cold;

		public StatusItem Cold_Crop;

		public StatusItem Crop_Too_Dark;

		public StatusItem Crop_Too_Bright;

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

		public StatusItem ReadyForHarvest;

		public StatusItem EnvironmentTooWarm;

		public StatusItem EnvironmentTooCold;

		public StatusItem Entombed;

		public StatusItem Wilting;

		public StatusItem WiltingDomestic;

		public StatusItem WiltingNonGrowing;

		public StatusItem WiltingNonGrowingDomestic;

		public StatusItem WrongAtmosphere;

		public StatusItem AtmosphericPressureTooLow;

		public StatusItem AtmosphericPressureTooHigh;

		public StatusItem Barren;

		public StatusItem Fleeing;

		public StatusItem NeedsFertilizer;

		public StatusItem NeedsIrrigation;

		public StatusItem WrongTemperature;

		public StatusItem WrongFertilizer;

		public StatusItem WrongIrrigation;

		public StatusItem WrongFertilizerMajor;

		public StatusItem WrongIrrigationMajor;

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

		public StatusItem Trapped;
	}
}
