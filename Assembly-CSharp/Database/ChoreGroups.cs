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
			this.MedicalAid = this.Add("MedicalAid", DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME, "Caring");
			this.Cook = this.Add("Cook", DUPLICANTS.CHOREGROUPS.COOK.NAME, "Cooking");
			this.Art = this.Add("Art", DUPLICANTS.CHOREGROUPS.ART.NAME, "Art");
			this.Research = this.Add("Research", DUPLICANTS.CHOREGROUPS.RESEARCH.NAME, "Learning");
			this.Operating = this.Add("MachineOperating", DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME, "Machinery");
			this.Farming = this.Add("Farming", DUPLICANTS.CHOREGROUPS.FARMING.NAME, "Botanist");
			this.Basekeeping = this.Add("Basekeeping", DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME, "Athletics");
			this.Build = this.Add("Build", DUPLICANTS.CHOREGROUPS.BUILD.NAME, "Construction");
			this.Hauling = this.Add("Hauling", DUPLICANTS.CHOREGROUPS.HAULING.NAME, "Athletics");
			this.Dig = this.Add("Dig", DUPLICANTS.CHOREGROUPS.DIG.NAME, "Digging");
		}

		private ChoreGroup Add(string id, string name, string attribute)
		{
			ChoreGroup choreGroup = new ChoreGroup(id, name, attribute);
			base.Add(choreGroup);
			return choreGroup;
		}

		public ChoreGroup Build;

		public ChoreGroup Basekeeping;

		public ChoreGroup Cook;

		public ChoreGroup Art;

		public ChoreGroup Dig;

		public ChoreGroup Research;

		public ChoreGroup Farming;

		public ChoreGroup Hauling;

		public ChoreGroup Operating;

		public ChoreGroup MedicalAid;

		public ChoreGroup Combat;
	}
}
