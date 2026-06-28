using System;

namespace Database
{
	public class StatusItemCategories : ResourceSet<StatusItemCategory>
	{
		public StatusItemCategories(ResourceSet parent)
			: base("StatusItemCategories", parent)
		{
			this.Main = new StatusItemCategory("Main", this, "Main");
			this.Power = new StatusItemCategory("Power", this, "Power");
			this.Toilet = new StatusItemCategory("Toilet", this, "Toilet");
			this.Research = new StatusItemCategory("Research", this, "Research");
			this.Suffocation = new StatusItemCategory("Suffocation", this, "Suffocation");
			this.EntityReceptacle = new StatusItemCategory("EntityReceptacle", this, "EntityReceptacle");
			this.Hitpoints = new StatusItemCategory("Hitpoints", this, "Hitpoints");
			this.WoundEffects = new StatusItemCategory("WoundEffects", this, "WoundEffects");
			this.PreservationState = new StatusItemCategory("PreservationState", this, "PreservationState");
			this.PreservationTemperature = new StatusItemCategory("PreservationTemperature", this, "PreservationTemperature");
			this.PreservationAtmosphere = new StatusItemCategory("PreservationAtmosphere", this, "PreservationAtmosphere");
			this.ExhaustTemperature = new StatusItemCategory("ExhaustTemperature", this, "ExhaustTemperature");
			this.OperatingEnergy = new StatusItemCategory("OperatingEnergy", this, "OperatingEnergy");
			this.AccessControl = new StatusItemCategory("AccessControl", this, "AccessControl");
			this.Heat = new StatusItemCategory("Heat", this, "Heat");
			this.Yield = new StatusItemCategory("Yield", this, "Yield");
			this.Sleep = new StatusItemCategory("Sleep", this, "Sleep");
		}

		public StatusItemCategory Main;

		public StatusItemCategory Power;

		public StatusItemCategory Toilet;

		public StatusItemCategory Research;

		public StatusItemCategory Suffocation;

		public StatusItemCategory EntityReceptacle;

		public StatusItemCategory Hitpoints;

		public StatusItemCategory WoundEffects;

		public StatusItemCategory PreservationState;

		public StatusItemCategory PreservationTemperature;

		public StatusItemCategory PreservationAtmosphere;

		public StatusItemCategory OperatingEnergy;

		public StatusItemCategory ExhaustTemperature;

		public StatusItemCategory AccessControl;

		public StatusItemCategory Heat;

		public StatusItemCategory Yield;

		public StatusItemCategory Sleep;
	}
}
