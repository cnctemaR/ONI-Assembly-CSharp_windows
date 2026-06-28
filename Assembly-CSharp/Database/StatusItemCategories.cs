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
		}

		public StatusItemCategory Main;

		public StatusItemCategory Power;

		public StatusItemCategory Toilet;

		public StatusItemCategory Research;

		public StatusItemCategory Suffocation;

		public StatusItemCategory EntityReceptacle;

		public StatusItemCategory Hitpoints;

		public StatusItemCategory WoundEffects;
	}
}
