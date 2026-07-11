using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class Skills : ResourceSet<Skill>
	{
		public Skills(ResourceSet parent)
			: base("Skills", parent)
		{
			this.Mining1 = base.Add(new Skill("Mining1", DUPLICANTS.ROLES.JUNIOR_MINER.NAME, DUPLICANTS.ROLES.JUNIOR_MINER.DESCRIPTION, 0, "hat_role_mining1", Db.Get().SkillGroups.Mining.Id));
			this.Mining1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseDigSpeedSmall,
				Db.Get().SkillPerks.CanDigVeryFirm
			};
			this.Mining2 = base.Add(new Skill("Mining2", DUPLICANTS.ROLES.MINER.NAME, DUPLICANTS.ROLES.MINER.DESCRIPTION, 1, "hat_role_mining2", Db.Get().SkillGroups.Mining.Id));
			this.Mining2.priorSkills = new List<string> { this.Mining1.Id };
			this.Mining2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseDigSpeedMedium,
				Db.Get().SkillPerks.CanDigNearlyImpenetrable
			};
			this.Mining3 = base.Add(new Skill("Mining3", DUPLICANTS.ROLES.SENIOR_MINER.NAME, DUPLICANTS.ROLES.SENIOR_MINER.DESCRIPTION, 2, "hat_role_mining3", Db.Get().SkillGroups.Mining.Id));
			this.Mining3.priorSkills = new List<string> { this.Mining2.Id };
			this.Mining3.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseDigSpeedLarge };
			this.Building1 = base.Add(new Skill("Building1", DUPLICANTS.ROLES.JUNIOR_BUILDER.NAME, DUPLICANTS.ROLES.JUNIOR_BUILDER.DESCRIPTION, 0, "hat_role_building1", Db.Get().SkillGroups.Building.Id));
			this.Building1.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionSmall };
			this.Building2 = base.Add(new Skill("Building2", DUPLICANTS.ROLES.BUILDER.NAME, DUPLICANTS.ROLES.BUILDER.DESCRIPTION, 1, "hat_role_building2", Db.Get().SkillGroups.Building.Id));
			this.Building2.priorSkills = new List<string> { this.Building1.Id };
			this.Building2.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionMedium };
			this.Building3 = base.Add(new Skill("Building3", DUPLICANTS.ROLES.SENIOR_BUILDER.NAME, DUPLICANTS.ROLES.SENIOR_BUILDER.DESCRIPTION, 2, "hat_role_building3", Db.Get().SkillGroups.Building.Id));
			this.Building3.priorSkills = new List<string> { this.Building2.Id };
			this.Building3.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseConstructionLarge };
			this.Farming1 = base.Add(new Skill("Farming1", DUPLICANTS.ROLES.JUNIOR_FARMER.NAME, DUPLICANTS.ROLES.JUNIOR_FARMER.DESCRIPTION, 0, "hat_role_farming1", Db.Get().SkillGroups.Farming.Id));
			this.Farming1.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseBotanySmall };
			this.Farming2 = base.Add(new Skill("Farming2", DUPLICANTS.ROLES.FARMER.NAME, DUPLICANTS.ROLES.FARMER.DESCRIPTION, 1, "hat_role_farming2", Db.Get().SkillGroups.Farming.Id));
			this.Farming2.priorSkills = new List<string> { this.Farming1.Id };
			this.Farming2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseBotanyMedium,
				Db.Get().SkillPerks.CanFarmTinker
			};
			this.Farming3 = base.Add(new Skill("Farming3", DUPLICANTS.ROLES.SENIOR_FARMER.NAME, DUPLICANTS.ROLES.SENIOR_FARMER.DESCRIPTION, 2, "hat_role_farming3", Db.Get().SkillGroups.Farming.Id));
			this.Farming3.priorSkills = new List<string> { this.Farming2.Id };
			this.Farming3.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseBotanyLarge };
			this.Ranching1 = base.Add(new Skill("Ranching1", DUPLICANTS.ROLES.RANCHER.NAME, DUPLICANTS.ROLES.RANCHER.DESCRIPTION, 1, "hat_role_rancher1", Db.Get().SkillGroups.Ranching.Id));
			this.Ranching1.priorSkills = new List<string> { this.Farming1.Id };
			this.Ranching1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanWrangleCreatures,
				Db.Get().SkillPerks.CanUseRanchStation,
				Db.Get().SkillPerks.IncreaseRanchingSmall
			};
			this.Ranching2 = base.Add(new Skill("Ranching2", DUPLICANTS.ROLES.SENIOR_RANCHER.NAME, DUPLICANTS.ROLES.SENIOR_RANCHER.DESCRIPTION, 2, "hat_role_rancher2", Db.Get().SkillGroups.Ranching.Id));
			this.Ranching2.priorSkills = new List<string> { this.Ranching1.Id };
			this.Ranching2.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseRanchingMedium };
			this.Researching1 = base.Add(new Skill("Researching1", DUPLICANTS.ROLES.JUNIOR_RESEARCHER.NAME, DUPLICANTS.ROLES.JUNIOR_RESEARCHER.DESCRIPTION, 0, "hat_role_research1", Db.Get().SkillGroups.Research.Id));
			this.Researching1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningSmall,
				Db.Get().SkillPerks.AllowAdvancedResearch
			};
			this.Researching2 = base.Add(new Skill("Researching2", DUPLICANTS.ROLES.RESEARCHER.NAME, DUPLICANTS.ROLES.RESEARCHER.DESCRIPTION, 1, "hat_role_research2", Db.Get().SkillGroups.Research.Id));
			this.Researching2.priorSkills = new List<string> { this.Researching1.Id };
			this.Researching2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningMedium,
				Db.Get().SkillPerks.CanStudyWorldObjects
			};
			this.Researching3 = base.Add(new Skill("Researching3", DUPLICANTS.ROLES.SENIOR_RESEARCHER.NAME, DUPLICANTS.ROLES.SENIOR_RESEARCHER.DESCRIPTION, 2, "hat_role_research3", Db.Get().SkillGroups.Research.Id));
			this.Researching3.priorSkills = new List<string> { this.Researching2.Id };
			this.Researching3.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseLearningLarge,
				Db.Get().SkillPerks.AllowInterstellarResearch
			};
			this.Cooking1 = base.Add(new Skill("Cooking1", DUPLICANTS.ROLES.JUNIOR_COOK.NAME, DUPLICANTS.ROLES.JUNIOR_COOK.DESCRIPTION, 0, "hat_role_cooking1", Db.Get().SkillGroups.Cooking.Id));
			this.Cooking1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseCookingSmall,
				Db.Get().SkillPerks.CanElectricGrill
			};
			this.Cooking2 = base.Add(new Skill("Cooking2", DUPLICANTS.ROLES.COOK.NAME, DUPLICANTS.ROLES.COOK.DESCRIPTION, 1, "hat_role_cooking2", Db.Get().SkillGroups.Cooking.Id));
			this.Cooking2.priorSkills = new List<string> { this.Cooking1.Id };
			this.Cooking2.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseCookingMedium };
			this.Arting1 = base.Add(new Skill("Arting1", DUPLICANTS.ROLES.JUNIOR_ARTIST.NAME, DUPLICANTS.ROLES.JUNIOR_ARTIST.DESCRIPTION, 0, "hat_role_art1", Db.Get().SkillGroups.Art.Id));
			this.Arting1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanArt,
				Db.Get().SkillPerks.CanArtUgly,
				Db.Get().SkillPerks.IncreaseArtSmall
			};
			this.Arting2 = base.Add(new Skill("Arting2", DUPLICANTS.ROLES.ARTIST.NAME, DUPLICANTS.ROLES.ARTIST.DESCRIPTION, 1, "hat_role_art2", Db.Get().SkillGroups.Art.Id));
			this.Arting2.priorSkills = new List<string> { this.Arting1.Id };
			this.Arting2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanArtOkay,
				Db.Get().SkillPerks.IncreaseArtMedium
			};
			this.Arting3 = base.Add(new Skill("Arting3", DUPLICANTS.ROLES.MASTER_ARTIST.NAME, DUPLICANTS.ROLES.MASTER_ARTIST.DESCRIPTION, 2, "hat_role_art3", Db.Get().SkillGroups.Art.Id));
			this.Arting3.priorSkills = new List<string> { this.Arting2.Id };
			this.Arting3.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanArtGreat,
				Db.Get().SkillPerks.IncreaseArtLarge
			};
			this.Hauling1 = base.Add(new Skill("Hauling1", DUPLICANTS.ROLES.HAULER.NAME, DUPLICANTS.ROLES.HAULER.DESCRIPTION, 0, "hat_role_hauling1", Db.Get().SkillGroups.Hauling.Id));
			this.Hauling1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthGofer,
				Db.Get().SkillPerks.IncreaseCarryAmountSmall
			};
			this.Hauling2 = base.Add(new Skill("Hauling2", DUPLICANTS.ROLES.MATERIALS_MANAGER.NAME, DUPLICANTS.ROLES.MATERIALS_MANAGER.DESCRIPTION, 1, "hat_role_hauling2", Db.Get().SkillGroups.Hauling.Id));
			this.Hauling2.priorSkills = new List<string> { this.Hauling1.Id };
			this.Hauling2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthCourier,
				Db.Get().SkillPerks.IncreaseCarryAmountMedium
			};
			this.Suits1 = base.Add(new Skill("Suits1", DUPLICANTS.ROLES.SUIT_EXPERT.NAME, DUPLICANTS.ROLES.SUIT_EXPERT.DESCRIPTION, 2, "hat_role_suits1", Db.Get().SkillGroups.Suits.Id));
			this.Suits1.priorSkills = new List<string> { this.Hauling2.Id };
			this.Suits1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.ExosuitExpertise,
				Db.Get().SkillPerks.IncreaseAthleticsMedium
			};
			this.Technicals1 = base.Add(new Skill("Technicals1", DUPLICANTS.ROLES.MACHINE_TECHNICIAN.NAME, DUPLICANTS.ROLES.MACHINE_TECHNICIAN.DESCRIPTION, 0, "hat_role_technicals1", Db.Get().SkillGroups.Technicals.Id));
			this.Technicals1.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseMachinerySmall };
			this.Technicals2 = base.Add(new Skill("Technicals2", DUPLICANTS.ROLES.POWER_TECHNICIAN.NAME, DUPLICANTS.ROLES.POWER_TECHNICIAN.DESCRIPTION, 1, "hat_role_technicals2", Db.Get().SkillGroups.Technicals.Id));
			this.Technicals2.priorSkills = new List<string> { this.Technicals1.Id };
			this.Technicals2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseMachineryMedium,
				Db.Get().SkillPerks.CanPowerTinker
			};
			this.Engineering1 = base.Add(new Skill("Engineering1", DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.NAME, DUPLICANTS.ROLES.MECHATRONIC_ENGINEER.DESCRIPTION, 2, "hat_role_engineering1", Db.Get().SkillGroups.Technicals.Id));
			this.Engineering1.priorSkills = new List<string>
			{
				this.Technicals2.Id,
				this.Hauling2.Id
			};
			this.Engineering1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseMachineryLarge,
				Db.Get().SkillPerks.IncreaseConstructionMechatronics,
				Db.Get().SkillPerks.ConveyorBuild
			};
			this.Basekeeping1 = base.Add(new Skill("Basekeeping1", DUPLICANTS.ROLES.HANDYMAN.NAME, DUPLICANTS.ROLES.HANDYMAN.DESCRIPTION, 0, "hat_role_basekeeping1", Db.Get().SkillGroups.Basekeeping.Id));
			this.Basekeeping1.perks = new List<SkillPerk> { Db.Get().SkillPerks.IncreaseStrengthGroundskeeper };
			this.Basekeeping2 = base.Add(new Skill("Basekeeping2", DUPLICANTS.ROLES.PLUMBER.NAME, DUPLICANTS.ROLES.PLUMBER.DESCRIPTION, 1, "hat_role_basekeeping2", Db.Get().SkillGroups.Basekeeping.Id));
			this.Basekeeping2.priorSkills = new List<string> { this.Basekeeping1.Id };
			this.Basekeeping2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.IncreaseStrengthPlumber,
				Db.Get().SkillPerks.CanDoPlumbing
			};
			this.Astronauting1 = base.Add(new Skill("Astronauting1", DUPLICANTS.ROLES.ASTRONAUTTRAINEE.NAME, DUPLICANTS.ROLES.ASTRONAUTTRAINEE.DESCRIPTION, 3, "hat_role_astronaut1", Db.Get().SkillGroups.Suits.Id));
			this.Astronauting1.priorSkills = new List<string>
			{
				this.Researching3.Id,
				this.Suits1.Id
			};
			this.Astronauting1.perks = new List<SkillPerk> { Db.Get().SkillPerks.CanUseRockets };
			this.Astronauting2 = base.Add(new Skill("Astronauting2", DUPLICANTS.ROLES.ASTRONAUT.NAME, DUPLICANTS.ROLES.ASTRONAUT.DESCRIPTION, 4, "hat_role_astronaut2", Db.Get().SkillGroups.Suits.Id));
			this.Astronauting2.priorSkills = new List<string> { this.Astronauting1.Id };
			this.Astronauting2.perks = new List<SkillPerk> { Db.Get().SkillPerks.FasterSpaceFlight };
			this.Medicine1 = base.Add(new Skill("Medicine1", DUPLICANTS.ROLES.JUNIOR_MEDIC.NAME, DUPLICANTS.ROLES.JUNIOR_MEDIC.DESCRIPTION, 0, "hat_role_medicalaid1", Db.Get().SkillGroups.MedicalAid.Id));
			this.Medicine1.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanCompound,
				Db.Get().SkillPerks.IncreaseCaringSmall
			};
			this.Medicine2 = base.Add(new Skill("Medicine2", DUPLICANTS.ROLES.MEDIC.NAME, DUPLICANTS.ROLES.MEDIC.DESCRIPTION, 1, "hat_role_medicalaid1", Db.Get().SkillGroups.MedicalAid.Id));
			this.Medicine2.priorSkills = new List<string> { this.Medicine1.Id };
			this.Medicine2.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanDoctor,
				Db.Get().SkillPerks.IncreaseCaringMedium
			};
			this.Medicine3 = base.Add(new Skill("Medicine3", DUPLICANTS.ROLES.SENIOR_MEDIC.NAME, DUPLICANTS.ROLES.SENIOR_MEDIC.DESCRIPTION, 2, "hat_role_medicalaid1", Db.Get().SkillGroups.MedicalAid.Id));
			this.Medicine3.priorSkills = new List<string> { this.Medicine2.Id };
			this.Medicine3.perks = new List<SkillPerk>
			{
				Db.Get().SkillPerks.CanAdvancedMedicine,
				Db.Get().SkillPerks.IncreaseCaringLarge
			};
		}

		public List<Skill> GetSkillsWithPerk(string perk)
		{
			List<Skill> list = new List<Skill>();
			foreach (Skill skill in this.resources)
			{
				if (skill.GivesPerk(perk))
				{
					list.Add(skill);
				}
			}
			return list;
		}

		public List<Skill> GetSkillsWithPerk(SkillPerk perk)
		{
			List<Skill> list = new List<Skill>();
			foreach (Skill skill in this.resources)
			{
				if (skill.GivesPerk(perk))
				{
					list.Add(skill);
				}
			}
			return list;
		}

		public Skill Mining1;

		public Skill Mining2;

		public Skill Mining3;

		public Skill Building1;

		public Skill Building2;

		public Skill Building3;

		public Skill Farming1;

		public Skill Farming2;

		public Skill Farming3;

		public Skill Ranching1;

		public Skill Ranching2;

		public Skill Researching1;

		public Skill Researching2;

		public Skill Researching3;

		public Skill Cooking1;

		public Skill Cooking2;

		public Skill Arting1;

		public Skill Arting2;

		public Skill Arting3;

		public Skill Hauling1;

		public Skill Hauling2;

		public Skill Suits1;

		public Skill Technicals1;

		public Skill Technicals2;

		public Skill Engineering1;

		public Skill Basekeeping1;

		public Skill Basekeeping2;

		public Skill Astronauting1;

		public Skill Astronauting2;

		public Skill Medicine1;

		public Skill Medicine2;

		public Skill Medicine3;
	}
}
