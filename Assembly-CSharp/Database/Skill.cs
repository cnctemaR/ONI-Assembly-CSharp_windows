using System;
using System.Collections.Generic;
using TUNING;

namespace Database
{
	public class Skill : Resource, IHasDlcRestrictions
	{
		public Skill(string id, string name, string description, int tier, string hat, string badge, string skillGroup, List<SkillPerk> perks = null, List<string> priorSkills = null, string requiredDuplicantModel = "Minion", string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
			: base(id, name)
		{
			this.description = description;
			this.requiredDlcIds = requiredDlcIds;
			this.forbiddenDlcIds = forbiddenDlcIds;
			this.tier = tier;
			this.hat = hat;
			this.badge = badge;
			this.skillGroup = skillGroup;
			this.perks = perks;
			if (this.perks == null)
			{
				this.perks = new List<SkillPerk>();
			}
			this.priorSkills = priorSkills;
			if (this.priorSkills == null)
			{
				this.priorSkills = new List<string>();
			}
			this.requiredDuplicantModel = requiredDuplicantModel;
		}

		[Obsolete]
		public Skill(string id, string name, string description, string dlcId, int tier, string hat, string badge, string skillGroup, List<SkillPerk> perks = null, List<string> priorSkills = null, string requiredDuplicantModel = "Minion")
			: this(id, name, description, tier, hat, badge, skillGroup, perks, priorSkills, requiredDuplicantModel, null, null)
		{
		}

		public int GetMoraleExpectation()
		{
			return SKILLS.SKILL_TIER_MORALE_COST[this.tier];
		}

		public bool GivesPerk(SkillPerk perk)
		{
			return this.perks.Contains(perk);
		}

		public bool GivesPerk(HashedString perkId)
		{
			using (List<SkillPerk>.Enumerator enumerator = this.perks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IdHash == perkId)
					{
						return true;
					}
				}
			}
			return false;
		}

		public string[] GetRequiredDlcIds()
		{
			return this.requiredDlcIds;
		}

		public string[] GetForbiddenDlcIds()
		{
			return this.forbiddenDlcIds;
		}

		public string description;

		public string[] requiredDlcIds;

		public string[] forbiddenDlcIds;

		public string skillGroup;

		public string hat;

		public string badge;

		public int tier;

		public bool deprecated;

		public List<SkillPerk> perks;

		public List<string> priorSkills;

		public string requiredDuplicantModel;
	}
}
