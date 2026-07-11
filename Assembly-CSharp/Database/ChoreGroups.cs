using System;
using STRINGS;

namespace Database
{
	public class ChoreGroups : ResourceSet<ChoreGroup>
	{
		public ChoreGroups(ResourceSet parent)
			: base("ChoreGroups", parent)
		{
			this.Combat = this.Add("Combat", DUPLICANTS.CHOREGROUPS.COMBAT.NAME, "Digging", 5);
			this.LifeSupport = this.Add("LifeSupport", DUPLICANTS.CHOREGROUPS.LIFESUPPORT.NAME, "LifeSupport", 5);
			this.Toggle = this.Add("Toggle", DUPLICANTS.CHOREGROUPS.TOGGLE.NAME, "Toggle", 5);
			this.MedicalAid = this.Add("MedicalAid", DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME, "Caring", 4);
			this.Basekeeping = this.Add("Basekeeping", DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME, "Athletics", 4);
			this.Cook = this.Add("Cook", DUPLICANTS.CHOREGROUPS.COOK.NAME, "Cooking", 3);
			this.Art = this.Add("Art", DUPLICANTS.CHOREGROUPS.ART.NAME, "Art", 3);
			this.Research = this.Add("Research", DUPLICANTS.CHOREGROUPS.RESEARCH.NAME, "Learning", 3);
			this.MachineOperating = this.Add("MachineOperating", DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME, "Machinery", 3);
			this.Farming = this.Add("Farming", DUPLICANTS.CHOREGROUPS.FARMING.NAME, "Botanist", 3);
			this.Ranching = this.Add("Ranching", DUPLICANTS.CHOREGROUPS.RANCHING.NAME, "Ranching", 3);
			this.Build = this.Add("Build", DUPLICANTS.CHOREGROUPS.BUILD.NAME, "Construction", 2);
			this.Dig = this.Add("Dig", DUPLICANTS.CHOREGROUPS.DIG.NAME, "Digging", 2);
			this.Hauling = this.Add("Hauling", DUPLICANTS.CHOREGROUPS.HAULING.NAME, "Athletics", 1);
			this.Storage = this.Add("Storage", DUPLICANTS.CHOREGROUPS.STORAGE.NAME, "Athletics", 1);
			Debug.Assert(true);
		}

		private ChoreGroup Add(string id, string name, string attribute, int default_personal_priority)
		{
			ChoreGroup choreGroup = new ChoreGroup(id, name, attribute, default_personal_priority);
			base.Add(choreGroup);
			return choreGroup;
		}

		public ChoreGroup FindByHash(HashedString id)
		{
			ChoreGroup choreGroup = null;
			foreach (ChoreGroup choreGroup2 in Db.Get().ChoreGroups.resources)
			{
				if (choreGroup2.IdHash == id)
				{
					choreGroup = choreGroup2;
					break;
				}
			}
			return choreGroup;
		}

		public ChoreGroup Build;

		public ChoreGroup Basekeeping;

		public ChoreGroup Cook;

		public ChoreGroup Art;

		public ChoreGroup Dig;

		public ChoreGroup Research;

		public ChoreGroup Farming;

		public ChoreGroup Ranching;

		public ChoreGroup Hauling;

		public ChoreGroup Storage;

		public ChoreGroup MachineOperating;

		public ChoreGroup MedicalAid;

		public ChoreGroup Combat;

		public ChoreGroup LifeSupport;

		public ChoreGroup Toggle;
	}
}
