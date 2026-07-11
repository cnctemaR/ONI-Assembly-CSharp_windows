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
			this.Hot = new StatusItem("Hot", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			this.Hot.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureVulnerable.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			};
			this.Hot_Crop = new StatusItem("Hot_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			this.Hot_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable2 = (TemperatureVulnerable)data;
				str = str.Replace("{low_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable2.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{high_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable2.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Scalding = new StatusItem("Scalding", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, true, SimViewMode.None, true, 63486);
			this.Scalding.resolveTooltipCallback = delegate(string str, object data)
			{
				float averageExternalTemperature = ((ExternalTemperatureMonitor.Instance)data).AverageExternalTemperature;
				float scaldingThreshold = ((ExternalTemperatureMonitor.Instance)data).GetScaldingThreshold();
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(averageExternalTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(scaldingThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Scalding.AddNotification(null, null, null, 0f);
			this.Cold = new StatusItem("Cold", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			this.Cold.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable3 = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable3.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), GameUtil.GetFormattedTemperature(temperatureVulnerable3.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			};
			this.Cold_Crop = new StatusItem("Cold_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			this.Cold_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable4 = (TemperatureVulnerable)data;
				str = str.Replace("low_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable4.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("high_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable4.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Crop_Too_Dark = new StatusItem("Crop_Too_Dark", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			this.Crop_Too_Dark.resolveStringCallback = (string str, object data) => str;
			this.Crop_Too_Bright = new StatusItem("Crop_Too_Bright", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			this.Crop_Too_Bright.resolveStringCallback = (string str, object data) => str;
			this.Hyperthermia = new StatusItem("Hyperthermia", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 63486);
			this.Hyperthermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value = ((TemperatureMonitor.Instance)data).temperature.value;
				float hyperthermiaThreshold = ((TemperatureMonitor.Instance)data).HyperthermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hyperthermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Hypothermia = new StatusItem("Hypothermia", "CREATURES", string.Empty, StatusItem.IconType.Exclamation, NotificationType.Bad, false, SimViewMode.None, true, 63486);
			this.Hypothermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value2 = ((TemperatureMonitor.Instance)data).temperature.value;
				float hypothermiaThreshold = ((TemperatureMonitor.Instance)data).HypothermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hypothermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Suffocating = new StatusItem("Suffocating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			this.Hatching = new StatusItem("Hatching", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			this.Incubating = new StatusItem("Incubating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			this.Drowning = new StatusItem("Drowning", "CREATURES", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, true, SimViewMode.None, true, 63486);
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
			this.CropSleeping = new StatusItem("Crop_Sleeping", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 1026);
			this.CropSleeping.resolveStringCallback = delegate(string str, object data)
			{
				CropSleepingMonitor.Instance instance = (CropSleepingMonitor.Instance)data;
				return str.Replace("{REASON}", (!instance.def.prefersDarkness) ? CREATURES.STATUSITEMS.CROP_SLEEPING.REASON_TOO_DARK : CREATURES.STATUSITEMS.CROP_SLEEPING.REASON_TOO_BRIGHT);
			};
			this.CropSleeping.resolveTooltipCallback = delegate(string str, object data)
			{
				CropSleepingMonitor.Instance instance2 = (CropSleepingMonitor.Instance)data;
				string text = string.Format(CREATURES.STATUSITEMS.CROP_SLEEPING.REQUIREMENT_LUMINANCE, instance2.def.lightIntensityThreshold);
				return str.Replace("{REQUIREMENTS}", text);
			};
			this.EnvironmentTooWarm = new StatusItem("EnvironmentTooWarm", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.EnvironmentTooWarm.resolveStringCallback = delegate(string str, object data)
			{
				float num3 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num4 = ((TemperatureVulnerable)data).internalTemperatureLethal_High - 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num3, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.EnvironmentTooCold = new StatusItem("EnvironmentTooCold", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.EnvironmentTooCold.resolveStringCallback = delegate(string str, object data)
			{
				float num5 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num6 = ((TemperatureVulnerable)data).internalTemperatureLethal_Low + 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num5, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num6, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				return str;
			};
			this.Entombed = new StatusItem("Entombed", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
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
			this.WrongAtmosphere = new StatusItem("WrongAtmosphere", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			this.WrongAtmosphere.resolveStringCallback = delegate(string str, object data)
			{
				string text2 = string.Empty;
				foreach (Element element in (data as PressureVulnerable).safe_atmospheres)
				{
					text2 = text2 + "\n    •  " + element.name;
				}
				str = str.Replace("{elements}", text2);
				return str;
			};
			this.AtmosphericPressureTooLow = new StatusItem("AtmosphericPressureTooLow", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			this.AtmosphericPressureTooLow.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.AtmosphericPressureTooHigh = new StatusItem("AtmosphericPressureTooHigh", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			this.AtmosphericPressureTooHigh.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable2 = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.HealthStatus = new StatusItem("HealthStatus", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			this.HealthStatus.resolveStringCallback = delegate(string str, object data)
			{
				string text3 = string.Empty;
				switch ((Health.HealthState)data)
				{
				case Health.HealthState.Perfect:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.NAME;
					break;
				case Health.HealthState.Scuffed:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.NAME;
					break;
				case Health.HealthState.Injured:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.NAME;
					break;
				case Health.HealthState.Critical:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.NAME;
					break;
				case Health.HealthState.Incapacitated:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.NAME;
					break;
				case Health.HealthState.Dead:
					text3 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.NAME;
					break;
				}
				str = str.Replace("{healthState}", text3);
				return str;
			};
			this.HealthStatus.resolveTooltipCallback = delegate(string str, object data)
			{
				string text4 = string.Empty;
				switch ((Health.HealthState)data)
				{
				case Health.HealthState.Perfect:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.TOOLTIP;
					break;
				case Health.HealthState.Scuffed:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.TOOLTIP;
					break;
				case Health.HealthState.Injured:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.TOOLTIP;
					break;
				case Health.HealthState.Critical:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.TOOLTIP;
					break;
				case Health.HealthState.Incapacitated:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.TOOLTIP;
					break;
				case Health.HealthState.Dead:
					text4 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.TOOLTIP;
					break;
				}
				str = str.Replace("{healthState}", text4);
				return str;
			};
			this.Barren = new StatusItem("Barren", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.NeedsFertilizer = new StatusItem("NeedsFertilizer", "CREATURES", "status_item_plant_solid", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			Func<string, object, string> func = (string str, object data) => str;
			this.NeedsFertilizer.resolveStringCallback = func;
			this.NeedsIrrigation = new StatusItem("NeedsIrrigation", "CREATURES", "status_item_plant_liquid", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, false, 63486);
			Func<string, object, string> func2 = (string str, object data) => str;
			this.NeedsIrrigation.resolveStringCallback = func2;
			this.WrongFertilizer = new StatusItem("WrongFertilizer", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			Func<string, object, string> func3 = (string str, object data) => str;
			this.WrongFertilizer.resolveStringCallback = func3;
			this.WrongFertilizerMajor = new StatusItem("WrongFertilizerMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.WrongFertilizerMajor.resolveStringCallback = func3;
			this.WrongIrrigation = new StatusItem("WrongIrrigation", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			Func<string, object, string> func4 = (string str, object data) => str;
			this.WrongIrrigation.resolveStringCallback = func4;
			this.WrongIrrigationMajor = new StatusItem("WrongIrrigationMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.WrongIrrigationMajor.resolveStringCallback = func4;
			this.CantAcceptFertilizer = new StatusItem("CantAcceptFertilizer", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.Rotting = new StatusItem("Rotting", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			this.Rotting.resolveStringCallback = (string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			this.Fresh = new StatusItem("Fresh", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			this.Fresh.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance3 = (Rottable.Instance)data;
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance3.RotConstitutionPercentage * 100f) + "%)");
			};
			this.Fresh.resolveTooltipCallback = delegate(string str, object data)
			{
				Rottable.Instance instance4 = (Rottable.Instance)data;
				return str.Replace("{RotTooltip}", instance4.GetToolTip());
			};
			this.Stale = new StatusItem("Stale", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.Stale.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance5 = (Rottable.Instance)data;
				return str.Replace("{RotPercentage}", "(" + Util.FormatWholeNumber(instance5.RotConstitutionPercentage * 100f) + "%)");
			};
			this.Stale.resolveTooltipCallback = delegate(string str, object data)
			{
				Rottable.Instance instance6 = (Rottable.Instance)data;
				return str.Replace("{RotTooltip}", instance6.GetToolTip());
			};
			this.Spoiled = new StatusItem("Spoiled", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.Refrigerated = new StatusItem("Refrigerated", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			this.Unrefrigerated = new StatusItem("Unrefrigerated", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.Unrefrigerated.resolveStringCallback = (string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			this.SterilizingAtmosphere = new StatusItem("SterilizingAtmosphere", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			this.ContaminatedAtmosphere = new StatusItem("ContaminatedAtmosphere", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.Old = new StatusItem("Old", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
			this.Old.resolveTooltipCallback = delegate(string str, object data)
			{
				AgeMonitor.Instance instance7 = (AgeMonitor.Instance)data;
				return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedCycles(instance7.CyclesUntilDeath * 600f, "F1"));
			};
		}

		public StatusItem HealthStatus;

		public StatusItem Hot;

		public StatusItem Hot_Crop;

		public StatusItem Scalding;

		public StatusItem Cold;

		public StatusItem Cold_Crop;

		public StatusItem Crop_Too_Dark;

		public StatusItem Crop_Too_Bright;

		public StatusItem Hypothermia;

		public StatusItem Hyperthermia;

		public StatusItem Suffocating;

		public StatusItem Hatching;

		public StatusItem Incubating;

		public StatusItem Drowning;

		public StatusItem DryingOut;

		public StatusItem Growing;

		public StatusItem CropSleeping;

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

		public StatusItem Old;
	}
}
