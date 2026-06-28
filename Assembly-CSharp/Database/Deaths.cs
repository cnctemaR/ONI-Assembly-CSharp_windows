using System;

namespace Database
{
	public class Deaths : ResourceSet<Death>
	{
		public Deaths(ResourceSet parent)
			: base("Deaths", parent)
		{
			this.Generic = new Death("Generic", this, "Generic", "{Target} has died.", "dead_on_back", "dead_on_back");
			this.Frozen = new Death("Frozen", this, "Frozen", "{Target} has frozen to death.", "death_freeze_trans", "death_freeze_solid");
			this.Suffocation = new Death("Suffocation", this, "Suffocation", "{Target} has suffocated to death.", "death_suffocation", "dead_on_back");
			this.Starvation = new Death("Starvation", this, "Starvation", "{Target} has starved to death.", "dead_on_back", "dead_on_back");
			this.Overheating = new Death("Overheating", this, "Overheating", "{Target} has overheated to death.", "dead_on_back", "dead_on_back");
			this.Drowned = new Death("Drowned", this, "Drowned", "{Target} has drowned.", "death_suffocation", "dead_on_back");
			this.Explosion = new Death("Explosion", this, "Explosion", "{Target} has died in an explosion.", "dead_on_back", "dead_on_back");
		}

		public Death Generic;

		public Death Frozen;

		public Death Suffocation;

		public Death Starvation;

		public Death Overheating;

		public Death Drowned;

		public Death Explosion;
	}
}
