using System;

namespace Database
{
	public class ChoreGroups : ResourceSet<ChoreGroup>
	{
		public ChoreGroups(ResourceSet parent)
			: base("ChoreGroups", parent)
		{
			this.Combat = this.Add("Combat", "Combat", "Digging");
			this.Cook = this.Add("Cook", "Cook", "Cooking");
			this.Art = this.Add("Art", "Art", "Art");
			this.Compost = this.Add("Compost", "Compost", "Machinery");
			this.Research = this.Add("Research", "Research", "Learning");
			this.GeneratePower = this.Add("GeneratePower", "Generate Power", "Athletics");
			this.Harvest = this.Add("Harvest", "Harvest", "Digging");
			this.Transport = this.Add("Sweep", "Sweep", "Athletics");
			this.Build = this.Add("Build", "Build", "Construction");
			this.Deliver = this.Add("Deliver", "Deliver", "Athletics");
			this.Dig = this.Add("Dig", "Dig", "Digging");
		}

		private ChoreGroup Add(string id, string name, string attribute)
		{
			ChoreGroup choreGroup = new ChoreGroup(id, name, attribute);
			base.Add(choreGroup);
			return choreGroup;
		}

		public ChoreGroup Build;

		public ChoreGroup Cleaning;

		public ChoreGroup Cook;

		public ChoreGroup Art;

		public ChoreGroup Dig;

		public ChoreGroup Research;

		public ChoreGroup Combat;

		public ChoreGroup GeneratePower;

		public ChoreGroup Harvest;

		public ChoreGroup Compost;

		public ChoreGroup Transport;

		public ChoreGroup Deliver;
	}
}
