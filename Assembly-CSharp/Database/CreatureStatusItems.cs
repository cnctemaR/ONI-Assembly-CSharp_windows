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
			this.Idle = new StatusItem("Idle", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Burrowing = new StatusItem("Burrowing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Burrowed = new StatusItem("Burrowed", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Emerging = new StatusItem("Emerging", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.LookingForFood = new StatusItem("LookingForFood", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Eating = new StatusItem("Eating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Sleeping = new StatusItem("Sleeping", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Struggling = new StatusItem("Struggling", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Dead = new StatusItem("Dead", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Hot = new StatusItem("Hot", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Hot.resolveStringCallback = delegate(string str, object data)
			{
				float internalTemperature = ((TemperatureVulnerable)data).InternalTemperature;
				float num = ((TemperatureVulnerable)data).internalTemperatureWarning_High - 1f;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(internalTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
				return str;
			};
			this.Cold = new StatusItem("Cold", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Cold.resolveStringCallback = delegate(string str, object data)
			{
				float internalTemperature2 = ((TemperatureVulnerable)data).InternalTemperature;
				float num2 = ((TemperatureVulnerable)data).internalTemperatureWarning_Low + 1f;
				str = str.Replace("{InternalTemperature}", GameUtil.GetFormattedTemperature(internalTemperature2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
				return str;
			};
			this.Hungry = new StatusItem("Hungry", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.LayingAnEgg = new StatusItem("LayingAnEgg", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Suffocating = new StatusItem("Suffocating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Hatching = new StatusItem("Hatching", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Incubating = new StatusItem("Incubating", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.ConsideringLure = new StatusItem("ConsideringLure", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Falling = new StatusItem("Falling", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Drowning = new StatusItem("Drowning", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.ReadyForHarvest = new StatusItem("ReadyForHarvest", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Growing = new StatusItem("Growing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Growing.resolveStringCallback = delegate(string str, object data)
			{
				float num3 = 100f * ((Growing)data).PercentGrown();
				str = str.Replace("{PercentGrow}", Math.Floor((double)Math.Max(num3, 0f)).ToString("F0"));
				return str;
			};
			this.HarvestsRemaining = new StatusItem("HarvestsRemaining", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.HarvestsRemaining.resolveStringCallback = delegate(string str, object data)
			{
				str = str.Replace("{HarvestsRemaining}", ((Crop)data).GetHarvestsRemaining().ToString());
				return str;
			};
			this.EnvironmentTooWarm = new StatusItem("EnvironmentTooWarm", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.EnvironmentTooWarm.resolveStringCallback = delegate(string str, object data)
			{
				float num4 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num5 = ((TemperatureVulnerable)data).internalTemperatureLethal_High - 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num5, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
				return str;
			};
			this.EnvironmentTooCold = new StatusItem("EnvironmentTooCold", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.EnvironmentTooCold.resolveStringCallback = delegate(string str, object data)
			{
				float num6 = Grid.Temperature[Grid.PosToCell(((TemperatureVulnerable)data).gameObject)];
				float num7 = ((TemperatureVulnerable)data).internalTemperatureLethal_Low + 1f;
				str = str.Replace("{ExternalTemperature}", GameUtil.GetFormattedTemperature(num6, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
				str = str.Replace("{TargetTemperature}", GameUtil.GetFormattedTemperature(num7, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
				return str;
			};
			this.Entombed = new StatusItem("Entombed", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Wilting = new StatusItem("Wilting", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.WiltingDomestic = new StatusItem("WiltingDomestic", CREATURES.STATUSITEMS.WILTING.NAME, "status_item_need_plant", CREATURES.STATUSITEMS.WILTING.TOOLTIP, false, StatusItem.IconType.Custom, NotificationType.BadMinor, SimViewMode.None, SimViewMode.None);
			this.AtmosphericPressureTooLow = new StatusItem("AtmosphericPressureTooLow", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.AtmosphericPressureTooLow.resolveStringCallback = delegate(string str, object data)
			{
				PressureVulnerable pressureVulnerable = (PressureVulnerable)data;
				float massLowWarning = pressureVulnerable.MassLowWarning;
				return str.Replace("{TargetPressure}", GameUtil.GetFormattedMass(massLowWarning, GameUtil.TimeSlice.None, true, "F1"));
			};
			this.HealthStatus = new StatusItem("HealthStatus", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.HealthStatus.resolveStringCallback = delegate(string str, object data)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				switch ((int)data)
				{
				case 0:
					text = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.NAME;
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.PERFECT.TOOLTIP;
					break;
				case 1:
					text = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.NAME;
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.SCUFFED.TOOLTIP;
					break;
				case 2:
					text = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.NAME;
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.INJURED.TOOLTIP;
					break;
				case 3:
					text = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.NAME;
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.CRITICAL.TOOLTIP;
					break;
				case 4:
					text = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.NAME;
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.INCAPACITATED.TOOLTIP;
					break;
				case 5:
					text = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.NAME;
					text2 = MISC.STATUSITEMS.HEALTHSTATUS.DEAD.TOOLTIP;
					break;
				}
				str = str.Replace("{healthState}", text);
				str = str.Replace("{healthStateTooltip}", text2);
				return str;
			};
			this.Barren = new StatusItem("Barren", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Fleeing = new StatusItem("Fleeing", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.NeedsFertilizer = new StatusItem("NeedsFertilizer", "CREATURES", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, SimViewMode.None);
			this.Rotting = new StatusItem("Rotting", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Rotting.resolveStringCallback = delegate(string str, object data)
			{
				Rottable.Instance instance = (Rottable.Instance)data;
				return str.Replace("{RotTemperature}", GameUtil.GetFormattedTemperature(instance.foodInfo.RotTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute));
			};
			this.Preserved = new StatusItem("Preserved", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Fresh = new StatusItem("Fresh", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Stale = new StatusItem("Stale", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Spoiled = new StatusItem("Spoiled", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			this.Refrigerated = new StatusItem("Refrigerated", "CREATURES", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
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

		public StatusItem Cold;

		public StatusItem Hungry;

		public StatusItem LayingAnEgg;

		public StatusItem Suffocating;

		public StatusItem Hatching;

		public StatusItem Incubating;

		public StatusItem ConsideringLure;

		public StatusItem Falling;

		public StatusItem Drowning;

		public StatusItem Growing;

		public StatusItem HarvestsRemaining;

		public StatusItem ReadyForHarvest;

		public StatusItem EnvironmentTooWarm;

		public StatusItem EnvironmentTooCold;

		public StatusItem Entombed;

		public StatusItem Wilting;

		public StatusItem WiltingDomestic;

		public StatusItem AtmosphericPressureTooLow;

		public StatusItem Barren;

		public StatusItem Fleeing;

		public StatusItem NeedsFertilizer;

		public StatusItem Rotting;

		public StatusItem Preserved;

		public StatusItem Fresh;

		public StatusItem Stale;

		public StatusItem Spoiled;

		public StatusItem Refrigerated;
	}
}
