using System;
using STRINGS;

namespace Database
{
	public class ChoreGroups : ResourceSet<ChoreGroup>
	{
		public ChoreGroups(ResourceSet parent)
			: base("ChoreGroups", parent)
		{
			this.Combat = this.Add("Combat", DUPLICANTS.CHOREGROUPS.COMBAT.NAME, "Digging");
			this.Massage = this.Add("Massage", DUPLICANTS.CHOREGROUPS.MASSAGE.NAME, "Machinery");
			this.Cook = this.Add("Cook", DUPLICANTS.CHOREGROUPS.COOK.NAME, "Cooking");
			this.Art = this.Add("Art", DUPLICANTS.CHOREGROUPS.ART.NAME, "Art");
			this.Compost = this.Add("Compost", DUPLICANTS.CHOREGROUPS.COMPOST.NAME, "Machinery");
			this.Research = this.Add("Research", DUPLICANTS.CHOREGROUPS.RESEARCH.NAME, "Learning");
			this.LiquidCooledFan = this.Add("LiquidCooledFan", DUPLICANTS.CHOREGROUPS.LIQUIDCOOLEDFAN.NAME, "Machinery");
			this.GeneratePower = this.Add("GeneratePower", DUPLICANTS.CHOREGROUPS.GENERATEPOWER.NAME, "Athletics");
			this.Repair = this.Add("Repair", DUPLICANTS.CHOREGROUPS.REPAIR.NAME, "Machinery");
			this.Mop = this.Add("Mop", DUPLICANTS.CHOREGROUPS.MOP.NAME, "Digging");
			this.Harvest = this.Add("Harvest", DUPLICANTS.CHOREGROUPS.HARVEST.NAME, "Digging");
			this.Transport = this.Add("Sweep", DUPLICANTS.CHOREGROUPS.SWEEP.NAME, "Athletics");
			this.Build = this.Add("Build", DUPLICANTS.CHOREGROUPS.BUILD.NAME, "Construction");
			this.Deliver = this.Add("Deliver", DUPLICANTS.CHOREGROUPS.DELIVER.NAME, "Athletics");
			this.Dig = this.Add("Dig", DUPLICANTS.CHOREGROUPS.DIG.NAME, "Digging");
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

		public ChoreGroup LiquidCooledFan;

		public ChoreGroup GeneratePower;

		public ChoreGroup Harvest;

		public ChoreGroup Compost;

		public ChoreGroup Transport;

		public ChoreGroup Deliver;

		public ChoreGroup Repair;

		public ChoreGroup Mop;

		public ChoreGroup Massage;
	}
}
