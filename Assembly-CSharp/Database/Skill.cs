using System;
using System.Collections.Generic;

namespace Database
{
	public class Skill : Resource
	{
		public Skill(string id, string name, string description, int tier, string hat, string skillGroup)
			: base(id, name)
		{
			this.description = description;
			this.tier = tier;
			this.hat = hat;
			this.skillGroup = skillGroup;
			this.perks = new List<SkillPerk>();
			this.priorSkills = new List<string>();
		}

		public bool GivesPerk(SkillPerk perk)
		{
			return this.perks.Contains(perk);
		}

		public bool GivesPerk(HashedString perkId)
		{
			foreach (SkillPerk skillPerk in this.perks)
			{
				if (skillPerk.IdHash == perkId)
				{
					return true;
				}
			}
			return false;
		}

		public string description;

		public string skillGroup;

		public string hat;

		public int tier;

		public List<SkillPerk> perks;

		public List<string> priorSkills;
	}
}
