using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class SkillGroups : ResourceSet<SkillGroup>
	{
		public SkillGroups(ResourceSet parent)
			: base("SkillGroups", parent)
		{
			this.Mining = base.Add(new SkillGroup("Mining", Db.Get().ChoreGroups.Dig.Id, DUPLICANTS.CHOREGROUPS.DIG.NAME));
			this.Mining.relevantAttributes = new List<string> { Db.Get().Attributes.Digging.Id };
			this.Mining.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Dig.Id };
			this.Building = base.Add(new SkillGroup("Building", Db.Get().ChoreGroups.Build.Id, DUPLICANTS.CHOREGROUPS.BUILD.NAME));
			this.Building.relevantAttributes = new List<string> { Db.Get().Attributes.Construction.Id };
			this.Building.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Build.Id };
			this.Farming = base.Add(new SkillGroup("Farming", Db.Get().ChoreGroups.Farming.Id, DUPLICANTS.CHOREGROUPS.FARMING.NAME));
			this.Farming.relevantAttributes = new List<string> { Db.Get().Attributes.Botanist.Id };
			this.Farming.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Farming.Id };
			this.Ranching = base.Add(new SkillGroup("Ranching", Db.Get().ChoreGroups.Ranching.Id, DUPLICANTS.CHOREGROUPS.RANCHING.NAME));
			this.Ranching.relevantAttributes = new List<string> { Db.Get().Attributes.Ranching.Id };
			this.Ranching.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Ranching.Id };
			this.Cooking = base.Add(new SkillGroup("Cooking", Db.Get().ChoreGroups.Cook.Id, DUPLICANTS.CHOREGROUPS.COOK.NAME));
			this.Cooking.relevantAttributes = new List<string> { Db.Get().Attributes.Cooking.Id };
			this.Cooking.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Cook.Id };
			this.Art = base.Add(new SkillGroup("Art", Db.Get().ChoreGroups.Art.Id, DUPLICANTS.CHOREGROUPS.ART.NAME));
			this.Art.relevantAttributes = new List<string> { Db.Get().Attributes.Art.Id };
			this.Art.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Art.Id };
			this.Research = base.Add(new SkillGroup("Research", Db.Get().ChoreGroups.Research.Id, DUPLICANTS.CHOREGROUPS.RESEARCH.NAME));
			this.Research.relevantAttributes = new List<string> { Db.Get().Attributes.Learning.Id };
			this.Research.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Research.Id };
			this.Suits = base.Add(new SkillGroup("Suits", string.Empty, DUPLICANTS.ROLES.GROUPS.SUITS));
			this.Suits.relevantAttributes = new List<string> { Db.Get().Attributes.Athletics.Id };
			this.Suits.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Hauling.Id };
			this.Hauling = base.Add(new SkillGroup("Hauling", Db.Get().ChoreGroups.Hauling.Id, DUPLICANTS.CHOREGROUPS.HAULING.NAME));
			this.Hauling.relevantAttributes = new List<string> { Db.Get().Attributes.Athletics.Id };
			this.Hauling.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Hauling.Id };
			this.Technicals = base.Add(new SkillGroup("Technicals", Db.Get().ChoreGroups.MachineOperating.Id, DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME));
			this.Technicals.relevantAttributes = new List<string> { Db.Get().Attributes.Machinery.Id };
			this.Technicals.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.MachineOperating.Id };
			this.MedicalAid = base.Add(new SkillGroup("MedicalAid", Db.Get().ChoreGroups.MedicalAid.Id, DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME));
			this.Basekeeping = base.Add(new SkillGroup("Basekeeping", Db.Get().ChoreGroups.Basekeeping.Id, DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME));
			this.Basekeeping.relevantAttributes = new List<string> { Db.Get().Attributes.Athletics.Id };
			this.Basekeeping.requiredChoreGroups = new List<string> { Db.Get().ChoreGroups.Basekeeping.Id };
		}

		public SkillGroup Mining;

		public SkillGroup Building;

		public SkillGroup Farming;

		public SkillGroup Ranching;

		public SkillGroup Cooking;

		public SkillGroup Art;

		public SkillGroup Research;

		public SkillGroup Suits;

		public SkillGroup Hauling;

		public SkillGroup Technicals;

		public SkillGroup MedicalAid;

		public SkillGroup Basekeeping;
	}
}
