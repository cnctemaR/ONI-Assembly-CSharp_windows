using System;

namespace Database
{
	public class StatusItemCategories : ResourceSet<StatusItemCategory>
	{
		public StatusItemCategories(ResourceSet parent)
			: base("StatusItemCategories", parent)
		{
			this.Main = new StatusItemCategory("Main", this, "Main");
			this.Role = new StatusItemCategory("Role", this, "Role");
			this.Power = new StatusItemCategory("Power", this, "Power");
			this.Toilet = new StatusItemCategory("Toilet", this, "Toilet");
			this.Research = new StatusItemCategory("Research", this, "Research");
			this.Hitpoints = new StatusItemCategory("Hitpoints", this, "Hitpoints");
			this.Suffocation = new StatusItemCategory("Suffocation", this, "Suffocation");
			this.WoundEffects = new StatusItemCategory("WoundEffects", this, "WoundEffects");
			this.EntityReceptacle = new StatusItemCategory("EntityReceptacle", this, "EntityReceptacle");
			this.PreservationState = new StatusItemCategory("PreservationState", this, "PreservationState");
			this.PreservationTemperature = new StatusItemCategory("PreservationTemperature", this, "PreservationTemperature");
			this.PreservationAtmosphere = new StatusItemCategory("PreservationAtmosphere", this, "PreservationAtmosphere");
			this.ExhaustTemperature = new StatusItemCategory("ExhaustTemperature", this, "ExhaustTemperature");
			this.OperatingEnergy = new StatusItemCategory("OperatingEnergy", this, "OperatingEnergy");
			this.AccessControl = new StatusItemCategory("AccessControl", this, "AccessControl");
			this.RequiredRoom = new StatusItemCategory("RequiredRoom", this, "RequiredRoom");
			this.Yield = new StatusItemCategory("Yield", this, "Yield");
			this.Heat = new StatusItemCategory("Heat", this, "Heat");
		}

		public StatusItemCategory Main;

		public StatusItemCategory Role;

		public StatusItemCategory Power;

		public StatusItemCategory Toilet;

		public StatusItemCategory Research;

		public StatusItemCategory Hitpoints;

		public StatusItemCategory Suffocation;

		public StatusItemCategory WoundEffects;

		public StatusItemCategory EntityReceptacle;

		public StatusItemCategory PreservationState;

		public StatusItemCategory PreservationAtmosphere;

		public StatusItemCategory PreservationTemperature;

		public StatusItemCategory ExhaustTemperature;

		public StatusItemCategory OperatingEnergy;

		public StatusItemCategory AccessControl;

		public StatusItemCategory RequiredRoom;

		public StatusItemCategory Yield;

		public StatusItemCategory Heat;
	}
}
