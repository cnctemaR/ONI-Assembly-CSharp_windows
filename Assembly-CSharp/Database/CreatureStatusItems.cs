using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Klei.AI;
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
			this.Dead = new StatusItem("Dead", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.Hot = new StatusItem("Hot", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.Hot.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable.TemperatureWarningLow, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(temperatureVulnerable.TemperatureWarningHigh, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			};
			this.Hot_Crop = new StatusItem("Hot_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.Hot_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable2 = (TemperatureVulnerable)data;
				str = str.Replace("{low_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable2.TemperatureWarningLow, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{high_temperature}", GameUtil.GetFormattedTemperature(temperatureVulnerable2.TemperatureWarningHigh, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			this.Scalding = new StatusItem("Scalding", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.DuplicantThreatening, true, OverlayModes.None.ID, true, 129022, null);
			this.Scalding.resolveTooltipCallback = delegate(string str, object data)
			{
				float averageExternalTemperature = ((ExternalTemperatureMonitor.Instance)data).AverageExternalTemperature;
				float scaldingThreshold = ((ExternalTemperatureMonitor.Instance)data).GetScaldingThreshold();
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(averageExternalTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(scaldingThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			this.Scalding.AddNotification(null, null, null);
			this.Cold = new StatusItem("Cold", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.Cold.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable3 = (TemperatureVulnerable)data;
				return string.Format(str, GameUtil.GetFormattedTemperature(temperatureVulnerable3.TemperatureWarningLow, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), GameUtil.GetFormattedTemperature(temperatureVulnerable3.TemperatureWarningHigh, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			};
			this.Cold_Crop = new StatusItem("Cold_Crop", "CREATURES", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.Cold_Crop.resolveStringCallback = delegate(string str, object data)
			{
				TemperatureVulnerable temperatureVulnerable4 = (TemperatureVulnerable)data;
				str = str.Replace("low_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable4.TemperatureWarningLow, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("high_temperature", GameUtil.GetFormattedTemperature(temperatureVulnerable4.TemperatureWarningHigh, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			this.Crop_Too_Dark = new StatusItem("Crop_Too_Dark", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.Crop_Too_Bright = new StatusItem("Crop_Too_Bright", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.Crop_Blighted = new StatusItem("Crop_Blighted", "CREATURES", "status_item_plant_blighted", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.Hyperthermia = new StatusItem("Hyperthermia", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022, null);
			this.Hyperthermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value = ((TemperatureMonitor.Instance)data).temperature.value;
				float hyperthermiaThreshold = ((TemperatureMonitor.Instance)data).HyperthermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hyperthermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			this.Hypothermia = new StatusItem("Hypothermia", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022, null);
			this.Hypothermia.resolveTooltipCallback = delegate(string str, object data)
			{
				float value2 = ((TemperatureMonitor.Instance)data).temperature.value;
				float hypothermiaThreshold = ((TemperatureMonitor.Instance)data).HypothermiaThreshold;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(value2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(hypothermiaThreshold, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			this.Suffocating = new StatusItem("Suffocating", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.Hatching = new StatusItem("Hatching", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.Incubating = new StatusItem("Incubating", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.Drowning = new StatusItem("Drowning", "CREATURES", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.Drowning.resolveStringCallback = (string str, object data) => str;
			this.Saturated = new StatusItem("Saturated", "CREATURES", "status_item_flooded", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.Saturated.resolveStringCallback = (string str, object data) => str;
			this.DryingOut = new StatusItem("DryingOut", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 1026, null);
			this.DryingOut.resolveStringCallback = (string str, object data) => str;
			this.ReadyForHarvest = new StatusItem("ReadyForHarvest", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 1026, null);
			this.Growing = new StatusItem("Growing", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 1026, null);
			this.Growing.resolveStringCallback = delegate(string str, object data)
			{
				if (((Growing)data).GetComponent<Crop>() != null)
				{
					float num = ((Growing)data).TimeUntilNextHarvest();
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(num, "F1", false));
				}
				float num2 = 100f * ((Growing)data).PercentGrown();
				str = str.Replace("{PercentGrow}", Math.Floor((double)Math.Max(num2, 0f)).ToString("F0"));
				return str;
			};
			this.CropSleeping = new StatusItem("Crop_Sleeping", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 1026, null);
			this.CropSleeping.resolveStringCallback = delegate(string str, object data)
			{
				CropSleepingMonitor.Instance instance = (CropSleepingMonitor.Instance)data;
				return str.Replace("{REASON}", instance.def.prefersDarkness ? CREATURES.STATUSITEMS.CROP_SLEEPING.REASON_TOO_BRIGHT : CREATURES.STATUSITEMS.CROP_SLEEPING.REASON_TOO_DARK);
			};
			this.CropSleeping.resolveTooltipCallback = delegate(string str, object data)
			{
				CropSleepingMonitor.Instance instance2 = (CropSleepingMonitor.Instance)data;
				AttributeInstance attributeInstance = Db.Get().PlantAttributes.MinLightLux.Lookup(instance2.gameObject);
				string text = string.Format(CREATURES.STATUSITEMS.CROP_SLEEPING.REQUIREMENT_LUMINANCE, attributeInstance.GetTotalValue());
				return str.Replace("{REQUIREMENTS}", text);
			};
			this.EnvironmentTooWarm = new StatusItem("EnvironmentTooWarm", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.EnvironmentTooWarm.resolveStringCallback = delegate(string str, object data)
			{
				float num3 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num4 = ((TemperatureVulnerable)data).TemperatureLethalHigh - 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num3, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			this.EnvironmentTooCold = new StatusItem("EnvironmentTooCold", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.EnvironmentTooCold.resolveStringCallback = delegate(string str, object data)
			{
				float num5 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num6 = ((TemperatureVulnerable)data).TemperatureLethalLow + 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num5, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num6, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				return str;
			};
			this.Entombed = new StatusItem("Entombed", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.Entombed.resolveStringCallback = (string str, object go) => str;
			this.Entombed.resolveTooltipCallback = delegate(string str, object go)
			{
				GameObject gameObject = go as GameObject;
				return string.Format(str, GameUtil.GetIdentityDescriptor(gameObject, GameUtil.IdentityDescriptorTense.Normal));
			};
			this.Wilting = new StatusItem("Wilting", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 1026, null);
			this.Wilting.resolveStringCallback = delegate(string str, object data)
			{
				Growing growing = data as Growing;
				if (growing != null && data != null)
				{
					AmountInstance amountInstance = growing.gameObject.GetAmounts().Get(Db.Get().Amounts.Maturity);
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(Mathf.Min(amountInstance.GetMax(), growing.TimeUntilNextHarvest()), "F1", false));
				}
				str = str.Replace("{Reasons}", (data as KMonoBehaviour).GetComponent<WiltCondition>().WiltCausesString());
				return str;
			};
			this.WiltingDomestic = new StatusItem("WiltingDomestic", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 1026, null);
			this.WiltingDomestic.resolveStringCallback = delegate(string str, object data)
			{
				Growing growing2 = data as Growing;
				if (growing2 != null && data != null)
				{
					AmountInstance amountInstance2 = growing2.gameObject.GetAmounts().Get(Db.Get().Amounts.Maturity);
					str = str.Replace("{TimeUntilNextHarvest}", GameUtil.GetFormattedCycles(Mathf.Min(amountInstance2.GetMax(), growing2.TimeUntilNextHarvest()), "F1", false));
				}
				str = str.Replace("{Reasons}", (data as KMonoBehaviour).GetComponent<WiltCondition>().WiltCausesString());
				return str;
			};
			this.WiltingNonGrowing = new StatusItem("WiltingNonGrowing", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 1026, null);
			this.WiltingNonGrowing.resolveStringCallback = delegate(string str, object data)
			{
				str = CREATURES.STATUSITEMS.WILTING_NON_GROWING_PLANT.NAME;
				str = str.Replace("{Reasons}", (data as WiltCondition).WiltCausesString());
				return str;
			};
			this.WiltingNonGrowingDomestic = new StatusItem("WiltingNonGrowing", "CREATURES", "status_item_need_plant", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 1026, null);
			this.WiltingNonGrowingDomestic.resolveStringCallback = delegate(string str, object data)
			{
				str = CREATURES.STATUSITEMS.WILTING_NON_GROWING_PLANT.NAME;
				str = str.Replace("{Reasons}", (data as WiltCondition).WiltCausesString());
				return str;
			};
			this.WrongAtmosphere = new StatusItem("WrongAtmosphere", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.WrongAtmosphere.resolveStringCallback = delegate(string str, object data)
			{
				string text2 = "";
				foreach (Element element in (data as PressureVulnerable).safe_atmospheres)
				{
					text2 = text2 + "\n    •  " + element.name;
				}
				str = str.Replace("{elements}", text2);
				return str;
			};
			this.AtmosphericPressureTooLow = new StatusItem("AtmosphericPressureTooLow", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.AtmosphericPressureTooLow.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.AtmosphericPressureTooHigh = new StatusItem("AtmosphericPressureTooHigh", "CREATURES", "status_item_plant_atmosphere", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.AtmosphericPressureTooHigh.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable2 = (PressureVulnerable)data;
				str = str.Replace("{low_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				str = str.Replace("{high_mass}", GameUtil.GetFormattedMass(pressureVulnerable2.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.HealthStatus = new StatusItem("HealthStatus", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.HealthStatus.resolveStringCallback = delegate(string str, object data)
			{
				string text3 = "";
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
				string text4 = "";
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
			this.Barren = new StatusItem("Barren", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.NeedsFertilizer = new StatusItem("NeedsFertilizer", "CREATURES", "status_item_plant_solid", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			Func<string, object, string> func = (string str, object data) => str;
			this.NeedsFertilizer.resolveStringCallback = func;
			this.NeedsIrrigation = new StatusItem("NeedsIrrigation", "CREATURES", "status_item_plant_liquid", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			Func<string, object, string> func2 = (string str, object data) => str;
			this.NeedsIrrigation.resolveStringCallback = func2;
			this.WrongFertilizer = new StatusItem("WrongFertilizer", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			Func<string, object, string> func3 = (string str, object data) => str;
			this.WrongFertilizer.resolveStringCallback = func3;
			this.WrongFertilizerMajor = new StatusItem("WrongFertilizerMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.WrongFertilizerMajor.resolveStringCallback = func3;
			this.WrongIrrigation = new StatusItem("WrongIrrigation", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			Func<string, object, string> func4 = (string str, object data) => str;
			this.WrongIrrigation.resolveStringCallback = func4;
			this.WrongIrrigationMajor = new StatusItem("WrongIrrigationMajor", "CREATURES", "status_item_fabricator_empty", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.WrongIrrigationMajor.resolveStringCallback = func4;
			this.CantAcceptFertilizer = new StatusItem("CantAcceptFertilizer", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.Rotting = new StatusItem("Rotting", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.Rotting.resolveStringCallback = (string str, object data) => str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(277.15f, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			this.Fresh = new StatusItem("Fresh", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
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
			this.Stale = new StatusItem("Stale", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
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
			this.Spoiled = new StatusItem("Spoiled", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			Func<string, object, string> func5 = delegate(string str, object data)
			{
				IRottable rottable = (IRottable)data;
				return str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(rottable.RotTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)).Replace("{PreserveTemperature}", GameUtil.GetFormattedTemperature(rottable.PreserveTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			};
			this.Refrigerated = new StatusItem("Refrigerated", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.Refrigerated.resolveStringCallback = func5;
			this.RefrigeratedFrozen = new StatusItem("RefrigeratedFrozen", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.RefrigeratedFrozen.resolveStringCallback = func5;
			this.Unrefrigerated = new StatusItem("Unrefrigerated", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.Unrefrigerated.resolveStringCallback = func5;
			this.SterilizingAtmosphere = new StatusItem("SterilizingAtmosphere", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.ContaminatedAtmosphere = new StatusItem("ContaminatedAtmosphere", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.Old = new StatusItem("Old", "CREATURES", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.Old.resolveTooltipCallback = delegate(string str, object data)
			{
				AgeMonitor.Instance instance7 = (AgeMonitor.Instance)data;
				return str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedCycles(instance7.CyclesUntilDeath * 600f, "F1", false));
			};
			this.ExchangingElementConsume = new StatusItem("ExchangingElementConsume", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.ExchangingElementConsume.resolveStringCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{ConsumeElement}", ElementLoader.FindElementByHash(statesInstance.master.consumedElement).tag.ProperName());
				str = str.Replace("{ConsumeRate}", GameUtil.GetFormattedMass(statesInstance.master.consumeRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.ExchangingElementConsume.resolveTooltipCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance2 = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{ConsumeElement}", ElementLoader.FindElementByHash(statesInstance2.master.consumedElement).tag.ProperName());
				str = str.Replace("{ConsumeRate}", GameUtil.GetFormattedMass(statesInstance2.master.consumeRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.ExchangingElementOutput = new StatusItem("ExchangingElementOutput", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.ExchangingElementOutput.resolveStringCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance3 = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{OutputElement}", ElementLoader.FindElementByHash(statesInstance3.master.emittedElement).tag.ProperName());
				str = str.Replace("{OutputRate}", GameUtil.GetFormattedMass(statesInstance3.master.consumeRate * statesInstance3.master.exchangeRatio, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.ExchangingElementOutput.resolveTooltipCallback = delegate(string str, object data)
			{
				EntityElementExchanger.StatesInstance statesInstance4 = (EntityElementExchanger.StatesInstance)data;
				str = str.Replace("{OutputElement}", ElementLoader.FindElementByHash(statesInstance4.master.emittedElement).tag.ProperName());
				str = str.Replace("{OutputRate}", GameUtil.GetFormattedMass(statesInstance4.master.consumeRate * statesInstance4.master.exchangeRatio, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				return str;
			};
			this.Hungry = new StatusItem("Hungry", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.Hungry.resolveTooltipCallback = delegate(string str, object data)
			{
				Diet diet = ((CreatureCalorieMonitor.Instance)data).master.gameObject.GetDef<CreatureCalorieMonitor.Def>().diet;
				if (diet.consumedTags.Count > 0)
				{
					string[] array = diet.consumedTags.Select<KeyValuePair<Tag, float>, string>((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray<string>();
					if (array.Length > 3)
					{
						array = new string[]
						{
							array[0],
							array[1],
							array[2],
							"..."
						};
					}
					string text5 = string.Join(", ", array);
					return str + "\n" + UI.BUILDINGEFFECTS.DIET_CONSUMED.text.Replace("{Foodlist}", text5);
				}
				return str;
			};
			this.HiveHungry = new StatusItem("HiveHungry", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.HiveHungry.resolveTooltipCallback = delegate(string str, object data)
			{
				Diet diet2 = ((BeehiveCalorieMonitor.Instance)data).master.gameObject.GetDef<BeehiveCalorieMonitor.Def>().diet;
				if (diet2.consumedTags.Count > 0)
				{
					string[] array2 = diet2.consumedTags.Select<KeyValuePair<Tag, float>, string>((KeyValuePair<Tag, float> t) => t.Key.ProperName()).ToArray<string>();
					if (array2.Length > 3)
					{
						array2 = new string[]
						{
							array2[0],
							array2[1],
							array2[2],
							"..."
						};
					}
					string text6 = string.Join(", ", array2);
					return str + "\n" + UI.BUILDINGEFFECTS.DIET_STORED.text.Replace("{Foodlist}", text6);
				}
				return str;
			};
			this.NoSleepSpot = new StatusItem("NoSleepSpot", "CREATURES", "", StatusItem.IconType.Exclamation, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.OriginalPlantMutation = new StatusItem("OriginalPlantMutation", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.UnknownMutation = new StatusItem("UnknownMutation", "CREATURES", "status_item_unknown_mutation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, null);
			this.SpecificPlantMutation = new StatusItem("SpecificPlantMutation", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.SpecificPlantMutation.resolveStringCallback = delegate(string str, object data)
			{
				PlantMutation plantMutation = (PlantMutation)data;
				return str.Replace("{MutationName}", plantMutation.Name);
			};
			this.SpecificPlantMutation.resolveTooltipCallback = delegate(string str, object data)
			{
				PlantMutation plantMutation2 = (PlantMutation)data;
				str = str.Replace("{MutationName}", plantMutation2.Name);
				return str + "\n" + plantMutation2.GetTooltip();
			};
			this.Crop_Too_NonRadiated = new StatusItem("Crop_Too_NonRadiated", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.Crop_Too_Radiated = new StatusItem("Crop_Too_Radiated", "CREATURES", "status_item_plant_light", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false, 129022, null);
			this.ElementGrowthGrowing = new StatusItem("Element_Growth_Growing", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.ElementGrowthGrowing.resolveTooltipCallback = delegate(string str, object data)
			{
				ElementGrowthMonitor.Instance instance8 = (ElementGrowthMonitor.Instance)data;
				StringBuilder stringBuilder = new StringBuilder(str, str.Length * 2);
				stringBuilder.Replace("{templo}", GameUtil.GetFormattedTemperature(instance8.def.minTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				stringBuilder.Replace("{temphi}", GameUtil.GetFormattedTemperature(instance8.def.maxTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				if (instance8.lastConsumedTemperature > 0f)
				{
					stringBuilder.Append("\n\n");
					stringBuilder.Append(CREATURES.STATUSITEMS.ELEMENT_GROWTH_GROWING.PREFERRED_TEMP);
					stringBuilder.Replace("{element}", ElementLoader.FindElementByHash(instance8.lastConsumedElement).name);
					stringBuilder.Replace("{temperature}", GameUtil.GetFormattedTemperature(instance8.lastConsumedTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
				}
				return stringBuilder.ToString();
			};
			this.ElementGrowthStunted = new StatusItem("Element_Growth_Stunted", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022, null);
			this.ElementGrowthStunted.resolveTooltipCallback = this.ElementGrowthGrowing.resolveTooltipCallback;
			this.ElementGrowthStunted.resolveStringCallback = delegate(string str, object data)
			{
				ElementGrowthMonitor.Instance instance9 = (ElementGrowthMonitor.Instance)data;
				string text7 = ((instance9.lastConsumedTemperature < instance9.def.minTemperature) ? CREATURES.STATUSITEMS.ELEMENT_GROWTH_STUNTED.TOO_COLD : CREATURES.STATUSITEMS.ELEMENT_GROWTH_STUNTED.TOO_HOT);
				str = str.Replace("{reason}", text7);
				return str;
			};
			this.ElementGrowthHalted = new StatusItem("Element_Growth_Halted", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022, null);
			this.ElementGrowthHalted.resolveTooltipCallback = this.ElementGrowthGrowing.resolveTooltipCallback;
			this.ElementGrowthComplete = new StatusItem("Element_Growth_Complete", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Bad, false, OverlayModes.None.ID, true, 129022, null);
			this.ElementGrowthComplete.resolveTooltipCallback = this.ElementGrowthGrowing.resolveTooltipCallback;
			this.LookingForFood = new StatusItem("LookingForFood", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.LookingForGas = new StatusItem("LookingForGas", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
			this.LookingForLiquid = new StatusItem("LookingForLiquid", "CREATURES", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022, null);
		}

		public StatusItem Dead;

		public StatusItem HealthStatus;

		public StatusItem Hot;

		public StatusItem Hot_Crop;

		public StatusItem Scalding;

		public StatusItem Cold;

		public StatusItem Cold_Crop;

		public StatusItem Crop_Too_Dark;

		public StatusItem Crop_Too_Bright;

		public StatusItem Crop_Blighted;

		public StatusItem Hypothermia;

		public StatusItem Hyperthermia;

		public StatusItem Suffocating;

		public StatusItem Hatching;

		public StatusItem Incubating;

		public StatusItem Drowning;

		public StatusItem Saturated;

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

		public StatusItem RefrigeratedFrozen;

		public StatusItem Unrefrigerated;

		public StatusItem SterilizingAtmosphere;

		public StatusItem ContaminatedAtmosphere;

		public StatusItem Old;

		public StatusItem ExchangingElementOutput;

		public StatusItem ExchangingElementConsume;

		public StatusItem Hungry;

		public StatusItem HiveHungry;

		public StatusItem NoSleepSpot;

		public StatusItem OriginalPlantMutation;

		public StatusItem UnknownMutation;

		public StatusItem SpecificPlantMutation;

		public StatusItem Crop_Too_NonRadiated;

		public StatusItem Crop_Too_Radiated;

		public StatusItem ElementGrowthGrowing;

		public StatusItem ElementGrowthStunted;

		public StatusItem ElementGrowthHalted;

		public StatusItem ElementGrowthComplete;

		public StatusItem LookingForFood;

		public StatusItem LookingForGas;

		public StatusItem LookingForLiquid;
	}
}
